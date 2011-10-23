using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;

namespace _035plasma
{
  public class Simulation
  {
    public Simulation ( int width, int height )
    {
      Width  = width;
      Height = height;
      Reset();
    }

    /// <summary>
    /// Width of the visualization bitmap in pixels.
    /// </summary>
    public int Width
    {
      get;
      set;
    }

    /// <summary>
    /// Height of the visualization bitmap in pixels.
    /// </summary>
    public int Height
    {
      get;
      set;
    }

    /// <summary>
    /// Frame number starting from 0.
    /// </summary>
    public long Frame
    {
      get;
      set;
    }

    /// <summary>
    /// Simulation reset.
    /// </summary>
    public void Reset ()
    {
      // !!!{{ TODO: put your simulation-reset code here

      Frame = 0;
      simWidth  = Width;  /* / 4 */
      simHeight = Height; /* / 4 */
      s = new float[ simHeight, simWidth ];
      rnd = new Random();

      // !!!}}
    }

    /// <summary>
    /// Width of the simulation array.
    /// </summary>
    protected int simWidth;

    /// <summary>
    /// Height of the simulation array.
    /// </summary>
    protected int simHeight;

    /// <summary>
    /// The simulation array.
    /// </summary>
    protected float[ , ] s;

    /// <summary>
    /// Globally allocated (shared) random generator.
    /// </summary>
    protected Random rnd;

    /// <summary>
    /// Simulation of a single timestep.
    /// </summary>
    /// <returns>Visualization bitmap.</returns>
    public Bitmap Simulate ()
    {
      // !!!{{ TODO: put your simulation code here

      PixelFormat fmt = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
      Bitmap result = new Bitmap( Width, Height, fmt );

      for ( int i = 0; i < simHeight; i++ )
        for ( int j = 0; j < simWidth; j++ )
          s[ i, j ] = (float)rnd.NextDouble();

      BitmapData data = result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, fmt );
      unsafe
      {
        byte* ptr;

        for ( int y = 0; y < Height; y++ )
        {
          ptr = (byte*)data.Scan0 + y * data.Stride;

          for ( int x = 0; x < Width; x++ )
          {
            byte c = (byte)(255.0f * s[ y % simHeight, x % simWidth ]);
            ptr[ 0 ] = ptr[ 1 ] = ptr[ 2 ] = c;
            ptr += 3;
          }
        }
      }
      result.UnlockBits( data );

      Frame++;

      return result;

      // !!!}}
    }
  }
}
