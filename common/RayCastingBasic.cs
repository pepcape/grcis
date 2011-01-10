using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Diagnostics;
using OpenTK;

namespace Rendering
{
  public class SimpleImageSynthesizer : IRenderer
  {
    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public int Width
    {
      get;
      set;
    }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public int Height
    {
      get;
      set;
    }

    public IImageFunction ImageFunction
    {
      get;
      set;
    }

    /// <summary>
    /// Renders the single pixel of an image.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    public void RenderPixel ( int x, int y, double[] color )
    {
      ImageFunction.GetSample( x, y, color );
    }

    /// <summary>
    /// Renders the given rectangle into the given raster image.
    /// </summary>
    /// <param name="image">Pre-initialized raster image.</param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    public void RenderRectangle ( Bitmap image, int x1, int y1, int x2, int y2 )
    {
      double[] color = new double[ 3 ];
      for ( int y = y1; y < y2; y++ )
        for ( int x = x1; x < x2; x++ )
        {
          ImageFunction.GetSample( x, y, color );
          image.SetPixel( x, y, Color.FromArgb( (int)(color[ 0 ] * 255.0),
                                                (int)(color[ 1 ] * 255.0),
                                                (int)(color[ 2 ] * 255.0) ) );
        }
    }
  }
}
