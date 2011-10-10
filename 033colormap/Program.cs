using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace _033colormap
{
  [Designer( "System.Windows.Forms.Design.PictureBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" )]
  public class MyBox : PictureBox
  {
    protected override void OnPaint ( PaintEventArgs pe )
    {
      if ( Form1.colors == null ) return;

      // Get the graphics object
      Graphics gfx = pe.Graphics;

      int width = this.Size.Width;
      int height = this.Size.Height;
      int stripes = Form1.colors.Length;
      int stripHeight = height / stripes;
      int columnWidth = width / stripes;
      int y = 0;

      // 1. create color brushes:
      Brush[] brushes = new Brush[ stripes ];
      for ( int i = 0; i < stripes; i++ )
        brushes[ i ] = new SolidBrush( Form1.colors[ i ] );
      Brush brBlack = new SolidBrush( Color.Black );
      Brush brWhite = new SolidBrush( Color.White );

      // 2. draw color stripes:
      Font myFont = new System.Drawing.Font( "Helvetica", 10, FontStyle.Regular );
      for ( int i = 0; i < stripes; i++, y += stripHeight )
      {
        gfx.FillRectangle( brushes[ i ], 0, y, width, stripHeight );
        for ( int j = 0; j < stripes; j++ )
        {
          gfx.DrawString( "Sample", myFont, brushes[ j ], 4 + j * columnWidth, y + 4 );
        }
        gfx.DrawString( Form1.color2string( Form1.colors[ i ] ),
                        myFont, Form1.colors[ i ].GetBrightness() < 0.5f ? brWhite : brBlack,
                        12, y + stripHeight - 20 );
      }
    }
  }

  [Designer( "System.Windows.Forms.Design.PictureBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" )]
  public class BaseBox : PictureBox
  {
    public Color color
    {
      get;
      set;
    }

    protected override void OnPaint ( PaintEventArgs pe )
    {
      Graphics gfx = pe.Graphics;

      int width = this.Size.Width;
      int height = this.Size.Height;

      Brush brush = new SolidBrush( color );

      gfx.FillRectangle( brush, 0, 0, width, height );
    }
  }

  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main ()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault( false );
      Application.Run( new Form1() );
    }
  }
}
