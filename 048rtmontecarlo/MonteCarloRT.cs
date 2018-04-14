using System;
using System.Collections.Generic;
using System.Diagnostics;
using MathSupport;
using OpenTK;
using Rendering;
using Utilities;

namespace _048rtmontecarlo
{
  public class FormSupport
  {
    /// <summary>
    /// Prepare form data (e.g. combo-box with available scenes).
    /// </summary>
    public static void InitializeScenes ( string[] args, out string name )
    {
      name = "Josef Pelikán";

      Form1 f = Form1.singleton;

      // 1. default scenes from RayCastingScenes
      f.sceneRepository = new Dictionary<string, object>( Scenes.staticRepository );

      // 2. optionally add custom scenes
      f.sceneRepository[ "Test scene" ] = new InitSceneParamDelegate( CustomScene.TestScene );

      // 3. fill the combo-box
      foreach ( string key in f.sceneRepository.Keys )
        f.ComboScene.Items.Add( key );

      // .. and set your favorite scene here:
      f.ComboScene.SelectedIndex = f.ComboScene.Items.IndexOf( "Test scene" );

      // default image parameters?
      f.ImageWidth = 800;
      f.ImageHeight = 540;
      f.NumericSupersampling.Value = 4;
      f.CheckMultithreading.Checked = true;
      f.TextParam.Text = "";
    }

    /// <summary>
    /// Initialize the ray-scene.
    /// </summary>
    public static IRayScene getScene ()
    {
      return Form1.singleton.SceneByComboBox();
    }

    /// <summary>
    /// Initialize ray-scene and image function (good enough for simple samples).
    /// </summary>
    public static IImageFunction getImageFunction ( IRayScene scene, string param )
    {
      return new RayTracing( scene );
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// </summary>
    public static IRenderer getRenderer ( IImageFunction imf, int superSampling, double jittering, string param )
    {
      Dictionary<string,string> p = Util.ParseKeyValueList( param );

      string isType;
      IRenderer r = null;
      if ( p.TryGetValue( "sampling", out isType ) )
        switch ( isType )
        {
          case "adapt1":
            double threshold = 0.004;
            Util.TryParse( p, "threshold", ref threshold );
            AdaptiveSupersamplingJR adis = new AdaptiveSupersamplingJR( threshold );
            adis.ImageFunction = imf;
            adis.Supersampling = superSampling;
            adis.Jittering     = jittering;
            r = adis;
            break;

          case "adapt2":
            AdaptiveSupersampling sis = new AdaptiveSupersampling();
            sis.ImageFunction = imf;
            sis.Supersampling = superSampling;
            sis.Jittering     = jittering;
            r = sis;
            break;
        }

      if ( r == null )
      {
        SupersamplingImageSynthesizer jit = new SupersamplingImageSynthesizer();
        jit.ImageFunction = imf;
        jit.Supersampling = superSampling;
        jit.Jittering     = jittering;
        r = jit;
      }

      return r;
    }
  }
}

namespace Rendering
{
  /// <summary>
  /// Super-samples only pixels/pixel parts which actually need it!
  /// </summary>
  public class AdaptiveSupersampling : SupersamplingImageSynthesizer
  {
    public AdaptiveSupersampling ()
      : base( 16 )
    {
    }

    /// <summary>
    /// Renders the single pixel of an image (using required super-sampling).
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    public override void RenderPixel ( int x, int y, double[] color )
    {
      Debug.Assert( color != null );
      Debug.Assert( MT.rnd != null );

      // !!!{{ TODO: this is exactly the code inherited from static sampling - make it adaptive!

      int bands = color.Length;
      int b;
      Array.Clear( color, 0, bands );
      double[] tmp = new double[ bands ];

      int i, j;
      double step = 1.0 / superXY;
      double amplitude = Jittering * step;
      double origin = 0.5 * (step - amplitude);
      double x0, y0;
      MT.StartPixel( x, y, Supersampling );

      for ( j = 0, y0 = y + origin; j++ < superXY; y0 += step )
        for ( i = 0, x0 = x + origin; i++ < superXY; x0 += step )
        {
          ImageFunction.GetSample( x0 + amplitude * MT.rnd.UniformNumber(),
                                   y0 + amplitude * MT.rnd.UniformNumber(),
                                   tmp );
          MT.NextSample();
          for ( b = 0; b < bands; b++ )
            color[ b ] += tmp[ b ];
        }

      double mul = step / superXY;
      if ( Gamma > 0.001 )
      {                                     // gamma-encoding and clamping
        double g = 1.0 / Gamma;
        for ( b = 0; b < bands; b++ )
          color[ b ] = Arith.Clamp( Math.Pow( color[ b ] * mul, g ), 0.0, 1.0 );
      }
      else                                  // no gamma, no clamping (for HDRI)
        for ( b = 0; b < bands; b++ )
          color[ b ] *= mul;

      // !!!}}
    }
  }

  /// <summary>
  /// Adaptive supersampling inspired by a quad-tree.
  /// Original author: Jan Roztocil, 2012.
  /// Update: Josef Pelikan, 2018.
  /// </summary>
  public class AdaptiveSupersamplingJR : SupersamplingImageSynthesizer
  {
    public AdaptiveSupersamplingJR ( double colThreshold =0.004 )
      : base( 16 )
    {
      bands = 0;
      colorThreshold = colThreshold;
    }

    protected int bands;

    protected double colorThreshold;

    /// <summary>
    /// Ternary tree to sture sampling results.
    /// </summary>
    class Node
    {
      public Node ( double x, double y, double step, int level, int bands )
      {
        this.x = x;
        this.y = y;
        this.step = step;
        this.level = level;
        children = null;
        result = new Result( bands );
      }

      public Node addChild ( Node child, int index )
      {
        if ( children == null )
          children = new Node[ 4 ];

        children[ index ] = child;
        return child;
      }

      public void setResult ( Result res )
      {
        result = res;
      }

      public double x;
      public double y;
      public double step;
      public int level;
      public Node[] children;
      public Result result;
    }

    /// <summary>
    /// Intersection result stored together with color & hash.
    /// </summary>
    class Result
    {
      public Result ( int bands )
      {
        color = new double[ bands ];
        hash = 0L;
      }

      public double[] color;
      public long hash;
    }

    /// <summary>
    /// Returns true if the two colors are similar
    /// </summary>
    private bool similarColor ( double[] col1, double[] col2 )
    {
      double r_diff = col1[ 0 ] - col2[ 0 ];
      double g_diff = col1[ 1 ] - col2[ 1 ];
      double b_diff = col1[ 2 ] - col2[ 2 ];
      return Math.Sqrt( r_diff * r_diff + g_diff * g_diff + b_diff * b_diff ) < colorThreshold;
    }

    private bool subdivisionNeeded ( Result res1, Result res2 )
    {
      if ( res1.hash != res2.hash )
        return true;   // two different objects were hit

      return !similarColor( res1.color, res2.color );
    }

    private void castRay ( double x0, double y0, double step, Result result )
    {
      result.hash = ImageFunction.GetSample( x0 + step * MT.rnd.UniformNumber(),
                                             y0 + step * MT.rnd.UniformNumber(),
                                             result.color );
    }

    /// <summary>
    /// Subdivides the node into quadrants.
    /// </summary>
    private void subdivide ( Node root, int maxDepth )
    {
      Result[] result = new Result[ 4 ];
      for ( int i = 0; i < 4; )
        result[ i++ ] = new Result( bands );

      double step = root.step * 0.5;
      double x0 = root.x;
      double y0 = root.y;

      Vector2d[] pos = new Vector2d[ 4 ];
      pos[ 0 ].X = x0;
      pos[ 0 ].Y = y0;

      pos[ 1 ].X = x0;
      pos[ 1 ].Y = y0 + step;

      pos[ 2 ].X = x0 + step;
      pos[ 2 ].Y = y0 + step;

      pos[ 3 ].X = x0 + step;
      pos[ 3 ].Y = y0;

      // one ray into each quadrant
      for ( int i = 0; i < 4; i++ )
        castRay( pos[ i ].X, pos[ i ].Y, step, result[ i ] );

      // results are stored in the tree
      for ( int i = 0; i < 4; i++ )
      {
        Node node = new Node( pos[ i ].X, pos[ i ].Y, step, root.level + 1, bands );
        node.setResult( result[ i ] );

        root.addChild( node, i );
      }

      // 2^(root.level + 1)
      int depth = 1 << (root.level + 1);

      // tree-depth check
      if ( depth >= maxDepth )
        return;

      bool[] toSubdivide = new bool[ 4 ];

      // neighbour checks
      if ( subdivisionNeeded( result[ 0 ], result[ 1 ] ) )
      {
        toSubdivide[ 0 ] = true;
        toSubdivide[ 1 ] = true;
      }

      if ( subdivisionNeeded( result[ 1 ], result[ 2 ] ) )
      {
        toSubdivide[ 1 ] = true;
        toSubdivide[ 2 ] = true;
      }

      if ( subdivisionNeeded( result[ 2 ], result[ 3 ] ) )
      {
        toSubdivide[ 2 ] = true;
        toSubdivide[ 3 ] = true;
      }

      if ( subdivisionNeeded( result[ 0 ], result[ 3 ] ) )
      {
        toSubdivide[ 0 ] = true;
        toSubdivide[ 3 ] = true;
      }

      // divide and conquer:
      for ( int i = 0; i < 4; i++ )
        if ( toSubdivide[ i ] )
          subdivide( root.children[ i ], maxDepth );
    }

    /// <summary>
    /// Final colour gathering.
    /// </summary>
    private void gatherColors ( Node node, double[] color )
    {
      if ( node.children != null )
        // inner node
        foreach ( Node child in node.children )
          gatherColors( child, color );
      else
      {
        // leaf node
        double mult = node.step * node.step;
        for ( int i = 0; i < bands; i++ )
          color[ i ] += node.result.color[ i ] * mult;
      }
    }

    /// <summary>
    /// Renders the single pixel of an image (using required super-sampling).
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    public override void RenderPixel ( int x, int y, double[] color )
    {
      Debug.Assert( color != null );
      Debug.Assert( MT.rnd != null );

      MT.StartPixel( x, y, Supersampling );

      bands = color.Length;
      Array.Clear( color, 0, bands );

      // we are starting from the whole pixel area = unit square
      Node root = new Node( x, y, 1.0, 0, bands );
      subdivide( root, superXY );

      // gather result color
      gatherColors( root, color );

      if ( Gamma > 0.001 )
      {                                     // gamma-encoding and clamping
        double g = 1.0 / Gamma;
        for ( int b = 0; b < bands; b++ )
          color[ b ] = Arith.Clamp( Math.Pow( color[ b ], g ), 0.0, 1.0 );
      }
    }
  }

  /// <summary>
  /// Custom scene for adaptive super-sampling (derived from "Sphere on the Plane").
  /// </summary>
  public class CustomScene
  {
    public static void TestScene ( IRayScene sc, string param )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.6, 0.4, 128 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.7, 0.5, -5.0 ),
                                    new Vector3d( 0.0, -0.18, 1.0 ),
                                    50.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 4.0, -3.0 ), 1.2 ) );

      /*
      // horizontal stick source:
      //RectangleLightSource rls = new RectangleLightSource( new Vector3d( -5.0, 4.0, -6.0 ),
      //                                                     new Vector3d( 0.0, 0.0, 6.0 ),
      //                                                     new Vector3d( 0.0, 0.1, 0.0 ), 2.2 );
      // vertical stick source:
      RectangleLightSource rls = new RectangleLightSource( new Vector3d( -5.0, 1.0, -3.0 ),
                                                           new Vector3d( 0.0, 0.0, 0.1 ),
                                                           new Vector3d( 0.0, 6.0, 0.0 ), 2.2 );
      // rectangle source:
      //RectangleLightSource rls = new RectangleLightSource( new Vector3d( -5.0, 1.0, -6.0 ),
      //                                                     new Vector3d( 0.0, 0.0, 6.0 ),
      //                                                     new Vector3d( 0.0, 6.0, 0.0 ), 2.2 );
      rls.Dim = new double[] { 1.0, 0.04, 0.0 };
      sc.Sources.Add( rls );
      */

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Sphere:
      Sphere s = new Sphere();
      root.InsertChild( s, Matrix4d.Identity );

      // Infinite plane with checker:
      Plane pl = new Plane();
      pl.SetAttribute( PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 } );
      pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 0.6, 0.6, new double[] { 1.0, 1.0, 1.0 } ) );
      root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, -1.0, 0.0 ) );
    }
  }
}
