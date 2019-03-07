using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace _035plasma
{
  public class Simulation
  {
    /// <summary>
    /// Optional form-data initialization.
    /// </summary>
    /// <param name="name">Return your full name.</param>
    /// <param name="width">Simulation field's width in pixels.</param>
    /// <param name="height">Simulation field's height in pixels.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams ( out string name, out int width, out int height, out string param, out string tooltip )
    {
      // {{

      name    = "Josef Pelikán";
      width   = 640;
      height  = 360;
      param   = "";
      tooltip = "nothing yet..";

      // }}
    }

    /// <summary>
    /// Prime-time initialization.
    /// </summary>
    /// <param name="width">Visualization bitmap width.</param>
    /// <param name="height">Visualization bitmap height.</param>
    /// <param name="param">Optional text parameter from the form.</param>
    public Simulation ( int width, int height, string param )
    {
      Radius = 3;
      Reset( width, height, param );
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
    /// Simulation parameter change.
    /// </summary>
    /// <param name="param">Optional text parameter from the form.</param>
    public void Change ( string param )
    {
      // {{ TODO: put your simulation-param change here

      // }}
    }

    /// <summary>
    /// Simulation reset.
    /// Can be called at any time after instance construction.
    /// </summary>
    /// <param name="width">Visualization bitmap width.</param>
    /// <param name="height">Visualization bitmap height.</param>
    /// <param name="param">Optional text parameter from the form.</param>
    public void Reset ( int width, int height, string param )
    {
      // {{ TODO: put your simulation-reset code here

      Frame     = 0;
      simWidth  = Width = width;    // might be a fraction of rendering size..
      simHeight = Height = height;  // might be a fraction of rendering size..
      s         = new float[ simHeight, simWidth ];
      rnd       = new Random();     // add seed-initialization here?

      // }}
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
    protected void Draw ( Point location, float color )
    {
      // {{ TODO: put your drawing code here

      int x = location.X;                // do proper coordinate-transform here!
      int y = location.Y;                // do proper coordinate-transform here!
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
          s[ i, j ] = color;             // permanent white color .. 1.0f .. see Simulate()

      // }}
    }

    /// <summary>
    /// Mouse-down response.
    /// Draws permanent 1.0f rectangle in pilot implementation.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <param name="mb">Which mouse button was pressed?</param>
    /// <param name="altKeys">Which control keys were down?</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseDown ( Point location, MouseButtons mb, Keys altKeys )
    {
      // {{ TODO: put your drawing logic here

      Draw( location, (altKeys & Keys.Shift) > 0 ? 0.0f : 1.0f );
      return true;

      // }}
    }

    /// <summary>
    /// Mouse-up response.
    /// Draws permanent 1.0f rectangle in pilot implementation.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <param name="mb">Which mouse button was released?</param>
    /// <param name="altKeys">Which control keys were down?</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseUp ( Point location, MouseButtons mb, Keys altKeys )
    {
      // {{ TODO: put your drawing logic here

      Draw( location, (altKeys & Keys.Shift) > 0 ? 0.0f : 1.0f );
      return true;

      // }}
    }

    /// <summary>
    /// Mouse-move response.
    /// Draws permanent 1.0f rectangle in pilot implementation.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <param name="mb">Which mouse buttons are down?</param>
    /// <param name="altKeys">Which control keys were down?</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseMove ( Point location, MouseButtons mb, Keys altKeys )
    {
      // {{ TODO: put your drawing logic here

      Draw( location, (altKeys & Keys.Shift) > 0 ? 0.0f : 1.0f );
      return true;

      // }}
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
      // {{ TODO: put your simulation code here

      for ( int i = 0; i < simHeight; i++ )
        for ( int j = 0; j < simWidth; j++ )
          if ( s[ i, j ] != 1.0f )          // permanent white color
            s[ i, j ] = (float)rnd.NextDouble();

      Frame++;

      // }}
    }

    /// <summary>
    /// Visualization of the current state.
    /// </summary>
    /// <returns>Visualization bitmap.</returns>
    public Bitmap Visualize ()
    {
      // {{ TODO: put your visualization code here

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

      // }}
    }
  }
}
