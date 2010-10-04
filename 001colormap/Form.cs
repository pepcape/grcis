using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _001colormap
{
  public partial class Form1 : Form
  {
    public static Color[] colors;

    public static Color baseColor1 = Color.FromArgb( 40, 160, 0 );
    public static Color baseColor2 = Color.FromArgb( 0, 200, 50 );

    public Form1 ()
    {
      InitializeComponent();
      baseBox1.color = baseColor1;
      baseBox2.color = baseColor2;
      colors = new[] { Color.FromArgb( 0, 0, 0 ),
                       Color.FromArgb( 255, 0, 0 ),
                       Color.FromArgb( 0, 255, 0 ),
                       Color.FromArgb( 0, 0, 255 ) };
    }

    private void button1_Click ( object sender, EventArgs e )
    {
      this.Close();
    }

    private void buttonColor1_Click ( object sender, EventArgs e )
    {
      DialogResult res = colorDialog1.ShowDialog();
      baseColor1 = colorDialog1.Color;
      baseBox1.color = baseColor1;
      label1.Text = "[" + baseColor1.R + ',' + baseColor1.G + ',' + baseColor1.B + ']';
      baseBox1.Invalidate();
      label1.Invalidate();
      generate();
    }

    private void buttonColor2_Click ( object sender, EventArgs e )
    {
      DialogResult res = colorDialog1.ShowDialog();
      baseColor2 = colorDialog1.Color;
      baseBox2.color = baseColor2;
      label2.Text = "[" + baseColor2.R + ',' + baseColor2.G + ',' + baseColor2.B + ']';
      baseBox2.Invalidate();
      label2.Invalidate();
      generate();
    }

    protected void generate ()
    {
      // !!!{{ TODO - generate custom palette based on baseColor1 & baseColor2

      colors = new Color[ 5 ];
      colors[ 0 ] = baseColor1;
      float r = baseColor1.R;
      float g = baseColor1.G;
      float b = baseColor1.B;
      float dr = (baseColor2.R - r) / 4.0f;
      float dg = (baseColor2.G - g) / 4.0f;
      float db = (baseColor2.B - b) / 4.0f;
      r += dr; g += dg; b += db;
      colors[ 1 ] = Color.FromArgb( (int)r, (int)g, (int)b );
      r += dr; g += dg; b += db;
      colors[ 2 ] = Color.FromArgb( (int)r, (int)g, (int)b );
      r += dr; g += dg; b += db;
      colors[ 3 ] = Color.FromArgb( (int)r, (int)g, (int)b );
      colors[ 4 ] = baseColor2;

      // !!!}}

      pictureBox1.Invalidate();
    }
  }
}
