// Author: Jan Dupej, Josef Pelikan

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MathSupport;
using Raster;

namespace _029flow
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Initialize the simulator object.
    /// </summary>
    private FluidSimulator getSimulator ( int ord )
    {
      DateTime now = DateTime.UtcNow;
      FluidSimulator s = new FluidSimulator( progress, new RandomJames( now.Ticks * (ord + 1) ) );
      worldInitFunctions[ selectedWorld ]( s );
      return s;
    }

    /// <summary>
    /// Prepare combo-box of available scenes.
    /// </summary>
    private void InitializeScenes ()
    {
      worldInitFunctions = new List<InitWorldDelegate>( Worlds.InitFunctions );

      // 1. default worlds from Worlds
      foreach ( string name in Worlds.Names )
        comboScene.Items.Add( name );

      // 2. eventually add custom worlds
      //worldInitFunctions.Add( new InitWorldDelegate( xxx ) );
      //comboScene.Items.Add( "xxx" );

      // .. and set your favorite scene here:
      comboScene.SelectedIndex = comboScene.Items.IndexOf( "Roof" );

      // default visualization parameters?
      ImageWidth  = 600;
      ImageHeight = 200;
      checkMultithreading.Checked = true;
    }

    /// <summary>
    /// Specimen - velocity visualization.
    /// </summary>
    /// <param name="so">so.bmp is the target Bitmap.</param>
    /// <param name="x0">Visualization origin - x.</param>
    /// <param name="y0">Visualization origin - y.</param>
    private void VisualizeVelocity ( SyncObject so, int x0, int y0 )
    {
      int r, g, b, num;
      int x, y;
      Color col;
      double vMul = 128.0 / maxV;

      for ( y = 0; y < height; y++ )
        for ( x = 0; x < width; x++ )
        {
          if ( (num = cell[ y, x ]) < 1 )
            col = Color.FromArgb( 0, 0, 128 );
          else
          {
            r = (int)(128 + vMul * vx[ y, x ] / num);
            g = (int)(128 + vMul * vy[ y, x ] / num);
            b = 0;
            col = Color.FromArgb( Arith.Clamp( r, 0, 255 ),
                                  Arith.Clamp( g, 0, 255 ),
                                  Arith.Clamp( b, 0, 255 ) );
          }
          so.bmp.SetPixel( x0 + x, y0 + y, col );
        }
    }

    /// <summary>
    /// Specimen - pressure visualization.
    /// </summary>
    /// <param name="so">so.bmp is the target Bitmap.</param>
    /// <param name="x0">Visualization origin - x.</param>
    /// <param name="y0">Visualization origin - y.</param>
    private void VisualizePressure ( SyncObject so, int x0, int y0 )
    {
      int num, x, y;
      Color col;
      double pressMul = 2.0 / Math.Sqrt( maxV2N );

      for ( y = 0; y < height; y++ )
        for ( x = 0; x < width; x++ )
        {
          if ( (num = cell[ y, x ]) < 1 )
            col = Color.FromArgb( 0, 0, 128 );
          else
            col = Draw.ColorRamp( pressMul * Math.Sqrt( num * power[ y, x ] ) );
          so.bmp.SetPixel( x0 + x, y0 + y, col );
        }
    }

    /// <summary>
    /// Custom visualization.
    /// </summary>
    /// <param name="so">so.bmp is the target Bitmap.</param>
    /// <param name="x0">Visualization origin - x.</param>
    /// <param name="y0">Visualization origin - y.</param>
    /// <param name="param">Optional text parameter[s].</param>
    private void VisualizeCustom ( SyncObject so, int x0, int y0, string param )
    {
      // !!!{{ TODO: put your visualization code here

      int x, y;
      for ( y = 0; y < height; y++ )
        for ( x = 0; x < width; x++ )
        {
          so.bmp.SetPixel( x0 + x, y0 + y, Draw.ColorRamp( (double)x / width ) );
        }

      // !!!}}
    }
  }
}
