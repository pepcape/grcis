using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Utilities;

namespace _112dials
{
  public class Simulation
  {
    /// <summary>
    /// Data initialization.
    /// </summary>
    public static void InitParams ( out int wid, out int hei, out string param, out string name )
    {
      wid = 640;
      hei = 480;
      param = "";
      name = "Josef Pelikán";
    }

    /// <summary>
    /// Prime-time initialization.
    /// </summary>
    /// <param name="width">Visualization bitmap width.</param>
    /// <param name="height">Visualization bitmap height.</param>
    /// <param name="param">String representation of the initial state.</param>
    public Simulation ( int width, int height, string param )
    {
      Width  = width;
      Height = height;
      Reset( param );
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
    /// Test variable: dial frequency.
    /// </summary>
    protected double freq;

    /// <summary>
    /// Current simulated time.
    /// </summary>
    protected double time;

    /// <summary>
    /// Simulation reset.
    /// Can be called at any time after instance construction.
    /// </summary>
    /// <param name="param">String representation of the initial state.</param>
    public void Reset ( string param )
    {
      // !!!{{ TODO: put your simulation-reset code here

      Frame = 0;
      freq  = 1.0;
      time  = 0.0;
      Update( param );

      // !!!}}
    }

    /// <summary>
    /// Mouse-down response.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseDown ( Point location )
    {
      // !!!{{ TODO: put your editing / drawing logic here

      freq *= 4.0;

      return false;

      // !!!}}
    }

    /// <summary>
    /// Mouse-up response.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseUp ( Point location )
    {
      // !!!{{ TODO: put your editing / drawing logic here

      freq *= 0.25;

      return false;

      // !!!}}
    }

    /// <summary>
    /// Mouse-move response.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseMove ( Point location )
    {
      // !!!{{ TODO: put your editing / drawing logic here

      return false;

      // !!!}}
    }

    /// <summary>
    /// Update of the animation state (speed, target state, ..).
    /// Can be called anytime.
    /// </summary>
    /// <param name="param">String representation of the required state.</param>
    public void Update ( string param )
    {
      // input params:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count == 0 )
        return;

      // launchers: frequency
      if ( Util.TryParse( p, "freq", ref freq ) )
      {
        if ( freq < 0.0 )
          freq = 0.01;
      }
    }

    /// <summary>
    /// Simulation of a single timestep.
    /// </summary>
    public void Simulate ()
    {
      // !!!{{ TODO: put your simulation code here

      Frame++;
      time += freq;

      // !!!}}
    }

    /// <summary>
    /// Visualization of the current state.
    /// </summary>
    /// <returns>Result bitmap. Creates a new Bitmap instance every time.</returns>
    public Bitmap Visualize ()
    {
      // !!!{{ TODO: put your visualization code here

      Bitmap result = new Bitmap( Width, Height, PixelFormat.Format24bppRgb );
      using ( Graphics gr = Graphics.FromImage( result ) )
      {
        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        gr.Clear( Color.LightGray );
        float cx = Width * 0.5f;
        float cy = Height * 0.5f;
        float rad = 0.8f * Math.Min( cx, cy );
        Pen penw = new Pen( Color.White, 4.0f );
        gr.DrawEllipse( penw, cx - rad, cy - rad, 2.0f * rad, 2.0f * rad );
        Pen penb = new Pen( Color.Black, 3.0f );
        double alpha = time * 0.002;
        gr.DrawLine( penb, cx, cy, cx + (float)Math.Sin( alpha ) * rad * 0.9f, cy - (float)Math.Cos( alpha ) * rad * 0.9f );
        alpha /= 12.0;
        gr.DrawLine( penb, cx, cy, cx + (float)Math.Sin( alpha ) * rad * 0.8f, cy - (float)Math.Cos( alpha ) * rad * 0.8f );
      }

      return result;

      // !!!}}
    }
  }
}
