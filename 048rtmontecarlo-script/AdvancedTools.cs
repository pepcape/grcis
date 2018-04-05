using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MathSupport;
using OpenTK;
using Rendering;

namespace _048rtmontecarlo
{
  public static class AdvancedTools
  {
    public static void Register(int level, Vector3d rayOrigin, Intersection firstIntersection)
    {
      if ( Form2.singleton == null )
      {
        return;
      }

      if ( DepthMap.depthMap == null )
        DepthMap.Initialize ();

      if (IntensityMap.intensityMap == null)
        IntensityMap.Initialize ();

      double depth;

      if ( firstIntersection == null )
      {
        depth = 1000; // CHANGE - placeholder for "infinity"
      }
      else
      {
        depth = Vector3d.Distance(rayOrigin, firstIntersection.CoordWorld);
      }
      

      if ( level == 0 )
      {

        // register depth
        if ( IntensityMap.intensityMap[MT.x, MT.y] == 0)
        {
          DepthMap.depthMap[MT.x, MT.y] = depth;
        }
        else
        {
          DepthMap.depthMap[MT.x, MT.y] = (DepthMap.depthMap[MT.x, MT.y] + depth) / 2;
        }

        // register intensity
        IntensityMap.intensityMap[ MT.x, MT.y ]++;
      }


    }


    public static class DepthMap
    {
      public static void Initialize()
      {
        DepthMapImageWidth = Form2.singleton.DepthMapPictureBox.Width;
        DepthMapImageHeight = Form2.singleton.DepthMapPictureBox.Height;

        depthMap = new double[DepthMapImageWidth, DepthMapImageHeight];
      }

      /// <summary>
      /// Image width in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int DepthMapImageWidth;

      /// <summary>
      /// Image height in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int DepthMapImageHeight;

      internal static double[,] depthMap;

      private static Bitmap depthMapBitmap;

      public static void RenderDepthMap()
      {
        if (DepthMapImageWidth == 0 || DepthMapImageHeight == 0)
        {
          Initialize();
        }

        double maxDepth = Double.MinValue;
        double minDepth = Double.MaxValue;

        GetMinimumAndMaximum(ref minDepth, ref maxDepth, depthMap);

        depthMapBitmap = new Bitmap(DepthMapImageWidth, DepthMapImageHeight, PixelFormat.Format24bppRgb);

        MapAppropriateColors ( minDepth, maxDepth, depthMap, depthMapBitmap);
      }
     

      public static Bitmap GetBitmap ()
      {
        if (depthMapBitmap == null)
        {
          RenderDepthMap();
        }

        return depthMapBitmap;
      }
    }

    public static class IntensityMap
    {
      public static void Initialize()
      {
        IntensityMapImageWidth = Form2.singleton.IntensityMapPictureBox.Width;
        IntensityMapImageHeight = Form2.singleton.IntensityMapPictureBox.Height;

        intensityMap = new int[IntensityMapImageWidth, IntensityMapImageHeight];
      }

      /// <summary>
      /// Image width in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int IntensityMapImageWidth;

      /// <summary>
      /// Image height in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int IntensityMapImageHeight;

      internal static int[,] intensityMap;

      private static Bitmap intensityMapBitmap;

      public static void RenderIntensityMap()
      {
        if ( IntensityMapImageWidth == 0 || IntensityMapImageHeight == 0)
        {
          Initialize ();
        }

        int maxIntensity = Int32.MinValue;
        int minIntensity = Int32.MaxValue;

        GetMinimumAndMaximum ( ref minIntensity, ref maxIntensity, intensityMap );

        intensityMapBitmap = new Bitmap(IntensityMapImageWidth, IntensityMapImageHeight, PixelFormat.Format24bppRgb);

        MapAppropriateColors ( minIntensity, maxIntensity, intensityMap, intensityMapBitmap );

        /*for (int x = 0; x < IntensityMapImageWidth; x++)
        {
          for (int y = 0; y < IntensityMapImageHeight; y++)
          {
            intensityMapBitmap.SetPixel ( x, y, GetAppropriateColor( minIntensity, maxIntensity, intensityMap[ x,y ] ));
          }
        }  */
      }      

      public static Bitmap GetBitmap()
      {
        if ( intensityMapBitmap == null )
        {
          RenderIntensityMap ();        
        }

        return intensityMapBitmap;
      }
    }


    /// <summary>
    /// Returns minimal and maximal values found in a specific map
    /// Values are returned via reference and must be of type IComparable
    /// </summary>
    /// <typeparam name="T">Type IComparable</typeparam>
    /// <param name="minValue">Must be initially set to max value of corresponding type</param>
    /// <param name="maxValue">Must be initially set to min value of corresponding type</param>
    /// <param name="map">2D array of values</param>
    private static void GetMinimumAndMaximum<T> ( ref T minValue, ref T maxValue, T[,] map ) where T: IComparable
    {
      for (int x = 0; x < DepthMap.DepthMapImageWidth; x++)
      {
        for (int y = 0; y < DepthMap.DepthMapImageHeight; y++)
        {
          if ( map[ x, y ].CompareTo ( maxValue ) > 0 )
            maxValue = map[ x, y ];

          if ( map[ x, y ].CompareTo ( maxValue ) < 0 )
            minValue = map[ x, y ];


          /*if (map[x, y] > maxValue)
            maxValue = map[x, y];

          if (map[x, y] < minValue)
            minValue = map[x, y];*/
        }
      }
    }

    /// <summary>
    /// Returns color based on range
    /// Returned color is either dark blue (close to minValue) or red (close to maxValue)
    /// Between than color is lineary transited changing value in HSV model
    /// Dark blue -> light blue -> turquoise -> green -> yellow -> orange -> red (does not go to purple)
    /// </summary>
    /// <param name="minValue">Start of range (dark blue color)</param>
    /// <param name="maxValue">End of range (red color)</param>
    /// <param name="newValue">Value for which we want color</param>
    /// <returns></returns>
    private static Color GetAppropriateColor<T> ( T minValue, T maxValue, T newValue )
    {
      dynamic colorValue = Divide ( Subtract ( newValue, minValue ), Subtract ( maxValue, minValue ) );
      colorValue *= 240;
      //double colorValue = (newValue - minValue) / (maxValue - minValue) * 240;

      if (double.IsNaN(colorValue) || double.IsInfinity(colorValue))
      {
        colorValue = 0;
      }

      return Arith.HSVToColor(240 - colorValue, 1, 1);
    }

    private static void MapAppropriateColors<T> ( T minValue, T maxValue, T[,] map, Bitmap bitmap )
    {
      for ( int x = 0; x < DepthMap.DepthMapImageWidth; x++ )
      {
        for ( int y = 0; y < DepthMap.DepthMapImageHeight; y++ )
        {
          bitmap.SetPixel(x, y, GetAppropriateColor( minValue, maxValue, map[x, y]));
        }
      }
    }

    static T Subtract<T>(T a, T b)
    {
      ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                          paramB = Expression.Parameter(typeof(T), "b");
      BinaryExpression body = Expression.Subtract(paramA, paramB);
      Func<T, T, T> subtract = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
      return subtract(a, b);
    }

    static T Divide<T>(T a, T b)
    {
      ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                          paramB = Expression.Parameter(typeof(T), "b");
      BinaryExpression body     = Expression.Divide(paramA, paramB);
      Func<T, T, T> divide = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
      return divide(a, b);
    }

    /// <summary>
    /// Calls Initialize method of all subclasses in AdvancedTools
    /// </summary>
    public static void SetNewDimensions ()
    {
      DepthMap.Initialize ();
      IntensityMap.Initialize ();
    }

    /// <summary>
    /// Removes all maps and unnecessary stuff
    /// </summary>
    public static void NewRenderInitialization ()
    {
      DepthMap.depthMap = null;
      IntensityMap.intensityMap = null;
    }
  }
}
