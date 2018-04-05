using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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

      public static void RenderDepthMap()
      {
        

        Bitmap depthMapBitmap = new Bitmap(DepthMapImageWidth, DepthMapImageHeight, PixelFormat.Format24bppRgb);
      }

      public static Bitmap GetBitmap ()
      {
        throw new NotImplementedException();
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

      private static Color highIntensity = Color.Red;
      private static Color lowIntensity = Color.Blue;

      internal static int[,] intensityMap;

      private static Bitmap intensityMapBitmap;

      public static void RenderIntensityMap()
      {
        if ( IntensityMapImageWidth == 0 || IntensityMapImageHeight == 0)
        {
          Initialize ();
        }

        int maxIntensity = 0;
        int minIntensity = int.MaxValue;

        // get maximal and minimal intensity - later used to get appropriate color for each intensity
        for ( int x = 0; x < IntensityMapImageWidth; x++ )
        {
          for ( int y = 0; y < IntensityMapImageHeight; y++ )
          {
            if ( intensityMap[ x, y ] > maxIntensity )
              maxIntensity = intensityMap[ x, y ];

            if (intensityMap[x, y] < minIntensity)
              minIntensity = intensityMap[x, y];
          }
        }

        intensityMapBitmap = new Bitmap(IntensityMapImageWidth, IntensityMapImageHeight, PixelFormat.Format24bppRgb);

        for (int x = 0; x < IntensityMapImageWidth; x++)
        {
          for (int y = 0; y < IntensityMapImageHeight; y++)
          {
            intensityMapBitmap.SetPixel ( x, y, GetAppropriateColor( minIntensity, maxIntensity, intensityMap[ x,y ] ));
          }
        }              
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

    private static Color GetAppropriateColor(double minValue, double maxValue, double newValue)
    {
      double colorValue = (newValue - minValue) / (maxValue - minValue) * 240;

      if (double.IsNaN(colorValue) || double.IsInfinity(colorValue))
      {
        colorValue = 0;
      }

      return Arith.HSVToColor(240 - colorValue, 1, 1);
    }


    public static void SetNewDimensions ()
    {
      DepthMap.Initialize ();
      IntensityMap.Initialize ();
    }

    public static void NewRenderInitialization ()
    {
      DepthMap.depthMap = null;
      IntensityMap.intensityMap = null;
    }
  }
}
