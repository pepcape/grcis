using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MathSupport;

namespace _115colormandala
{
  [Designer( "System.Windows.Forms.Design.PictureBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" )]
  public class MyBox : PictureBox
  {
    protected int xy1, xy2, size1, size2;
    protected int r1, r2, cxy;

    protected void FillSlice ( Graphics g, Color c, float angle, float width )
    {
      Brush b = new SolidBrush( c );
      GraphicsPath p = new GraphicsPath();
      p.AddArc( xy1, xy1, size1, size1, angle, width );
      double sina = Math.Sin( Arith.DegreeToRadian( angle + width ) );
      double cosa = Math.Cos( Arith.DegreeToRadian( angle + width ) );
      p.AddLine( (float)(cxy + r1 * cosa),
                 (float)(cxy + r1 * sina),
                 (float)(cxy + r2 * cosa),
                 (float)(cxy + r2 * sina) );
      p.AddArc( xy2, xy2, size2, size2, angle + width, -width );
      p.CloseFigure();
      g.FillPath( b, p );
    }

    protected override void OnPaint ( PaintEventArgs pe )
    {
      if ( Form1.colors == null ||
           Form1.colors.Length < 2 ) return;

      // Get the graphics object
      Graphics gfx = pe.Graphics;

      int width = Size.Width;
      int height = Size.Height;
      int segments = Math.Min(Form1.colors.Length, 50);

      Brush brWhite = new SolidBrush( Color.White );
      gfx.FillRectangle( brWhite, 0, 0, width, height );

      // Outer perimeter.
      xy1 = 10;
      size1 = Math.Min(width, height) - 2 * xy1;
      r1 = size1 / 2;
      cxy = xy1 + r1;

      // Inner perimeter.
      size2 = (2 * size1) / 3;
      r2 = size2 / 2;
      xy2 = xy1 + (size1 - size2) / 2;

      const float GAP = 2.5f;
      float dangle = (float)(360.0 / segments);
      float wangle = dangle - Math.Max(GAP, dangle * 0.12f);
      float angle = 0.5f * (dangle - wangle);

      for ( int i = 0; i < segments; i++, angle += dangle )
        FillSlice( gfx, Form1.colors[ i ], angle, wangle );

      // cxy remains the same.
      size1 = size2 - 20;
      r1 = size1 / 2;
      xy1 = xy2 + (size2 - size1) / 2;

      // New inner perimeter.
      size2 = size1 / 2;
      r2 = size2 / 2;
      xy2 = xy1 + (size1 - size2) / 2;

      angle = 0.5f * (2.0f * dangle - wangle);
      for ( int i = segments; --i >= 0; angle += dangle )
        FillSlice( gfx, Form1.colors[ i ], angle, wangle );
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
