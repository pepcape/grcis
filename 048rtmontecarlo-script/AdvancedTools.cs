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
    private static Form2 form2;

    public static void Register(int level, Vector3d rayOrigin, Intersection firstIntersection)
    {
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
        if ( IntensityMap.intensityMap[MT.x, MT.y] == 0)
        {
          DepthMap.depthMap[MT.x, MT.y] = depth;
        }
        else
        {
          DepthMap.depthMap[MT.x, MT.y] = (DepthMap.depthMap[MT.x, MT.y] + depth) / 2;
        }

        IntensityMap.intensityMap[ MT.x, MT.y ]++;
      }


    }


    public static class DepthMap
    {
      static DepthMap ()
      {
        DepthMapImageWidth = form2.DepthMapPictureBox.Width;
        DepthMapImageHeight = form2.DepthMapPictureBox.Height;

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
      static IntensityMap()
      {
        IntensityMapImageWidth = form2.IntensityMapPictureBox.Width;
        IntensityMapImageHeight = form2.IntensityMapPictureBox.Height;

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
        int maxIntensity = 0;
        int minIntensity = int.MaxValue;

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
            intensityMapBitmap.SetPixel ( x, y, GetAppropriateColor( x, y ));
          }
        }

        Color GetAppropriateColor ( int x, int y )
        {
          double colorValue = (intensityMap[x, y] - minIntensity) / (maxIntensity - minIntensity) * 240;

          if (double.IsNaN(colorValue) || double.IsInfinity(colorValue))
          {
            colorValue = 0;
          }

          return Arith.HSVToColor(240 - colorValue, 1, 1);
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
  }
}
