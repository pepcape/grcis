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
  static class AdvancedTools
  {
    static void ShadeStart(RandomJames rnd, int level, Intersection i)
    {
      //RTPP instance = ThreadFromRnd(rnd);
      //instance.ShadeStart(level, i);
    }

    static class DepthMap
    {
      /// <summary>
      /// Image width in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int DepthMapImageWidth = 0;

      /// <summary>
      /// Image height in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int DepthMapImageHeight = 0;

      static Form2 form2;

      public static void RenderDepthMap()
      {
        int width = DepthMapImageWidth;
        if (width <= 0)
          width = form2.pictureBox1.Width;

        int height = DepthMapImageHeight;
        if (height <= 0)
          height = form2.pictureBox1.Height;

        Bitmap depthMap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
      }

      public static Bitmap GetBitmap ()
      {
        throw new NotImplementedException();
      }

      public static void RegisterDepth(double x, double y, Vector3d p1)
      {

      }
    }
  }
}
