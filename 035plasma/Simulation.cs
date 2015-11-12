// Author: Josef Pelikan

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace _035plasma
{
  public class Simulation
  {
    /// <summary>
    /// Prime-time initialization.
    /// </summary>
    /// <param name="width">Visualization bitmap width.</param>
    /// <param name="height">Visualization bitmap height.</param>
    public Simulation ( int width, int height )
    {
      Width  = width;
      Height = height;
      Radius = 3;
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
    /// Can be called at any time after instance construction.
    /// </summary>
    public void Reset ()
    {
      // !!!{{ TODO: put your simulation-reset code here

      Frame     = 0;
      simWidth  = Width;     /* might be a fraction of rendering size.. */
      simHeight = Height;    /* might be a fraction of rendering size.. */
      s = new float[ simHeight, simWidth ];
      rnd = new Random();    /* add seed-initialization here? */

      // !!!}}
    }

    /// <summary>
    /// Drawing tool radius in pixels.
    /// </summary>
    public int Radius
    {
      get;
      set;
    }

    /// <summary>
    /// Support draw function (feel free to override/remove it).
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    protected void Draw ( Point location )
    {
      // !!!{{ TODO: put your drawing code here

      int x = location.X;                /* do proper coordinate-transform here! */
      int y = location.Y;                /* do proper coordinate-transform here! */
      if ( x < Radius )
        x = Radius;
      if ( x > simWidth - Radius )
        x = simWidth - Radius;
      if ( y < Radius )
        y = Radius;
      if ( y > simHeight - Radius )
        y = simHeight - Radius;

      for ( int i = y - Radius; i < y + Radius; i++ )
        for ( int j = x - Radius; j < x + Radius; j++ )
          s[ i, j ] = 1.0f;              /* permanent white color .. 1.0f .. see Simulate() */

      // !!!}}
    }

    /// <summary>
    /// Mouse-down response.
    /// Draws permanent 1.0f rectangle in pilot implementation.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseDown ( Point location )
    {
      // !!!{{ TODO: put your drawing logic here

      Draw( location );
      return true;

      // !!!}}
    }

    /// <summary>
    /// Mouse-up response.
    /// Draws permanent 1.0f rectangle in pilot implementation.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseUp ( Point location )
    {
      // !!!{{ TODO: put your drawing logic here

      Draw( location );
      return true;

      // !!!}}
    }

    /// <summary>
    /// Mouse-move response.
    /// Draws permanent 1.0f rectangle in pilot implementation.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseMove ( Point location )
    {
      // !!!{{ TODO: put your drawing logic here

      Draw( location );
      return true;

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
    /// The simulation array itself.
    /// Rewrite this declaration if you want to use multi-band simulation..
    /// </summary>
    protected float[ , ] s;

    /// <summary>
    /// Globally allocated (shared) random generator.
    /// </summary>
    protected Random rnd;

    /// <summary>
    /// Simulation of a single timestep.
    /// </summary>
    public void Simulate ()
    {
      // !!!{{ TODO: put your simulation code here

      for ( int i = 0; i < simHeight; i++ )
        for ( int j = 0; j < simWidth; j++ )
          if ( s[ i, j ] != 1.0f )       /* permanent white color */
            s[ i, j ] = (float)rnd.NextDouble();

      Frame++;

      // !!!}}
    }

    /// <summary>
    /// Visualization of the current state.
    /// </summary>
    /// <returns>Visualization bitmap.</returns>
    public Bitmap Visualize ()
    {
      // !!!{{ TODO: put your visualization code here

      PixelFormat fmt = PixelFormat.Format24bppRgb;
      int dO = Image.GetPixelFormatSize( fmt ) / 8;
      Bitmap result = new Bitmap( Width, Height, fmt );

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
            ptr += dO;
          }
        }
      }
      result.UnlockBits( data );

      return result;

      // !!!}}
    }
  }
}
