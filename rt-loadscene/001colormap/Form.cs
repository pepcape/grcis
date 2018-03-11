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

    public static Color baseColor1 = Color.FromArgb( 180,  60,   0 );
    public static Color baseColor2 = Color.FromArgb( 255, 240, 220 );
    public static int numCol = 4;

    public Form1 ()
    {
      InitializeComponent();
      baseBox1.color = baseColor1;
      label1.Text = color2string( baseColor1 );
      baseBox2.color = baseColor2;
      label2.Text = color2string( baseColor2 );
      generate();
    }

    private void button1_Click ( object sender, EventArgs e )
    {
      this.Close();
    }

    private void buttonColor1_Click ( object sender, EventArgs e )
    {
      DialogResult res = colorDialog1.ShowDialog();
      if ( res == DialogResult.Cancel ) return;
      baseColor1 = colorDialog1.Color;
      baseBox1.color = baseColor1;
      label1.Text = color2string( baseColor1 );
      baseBox1.Invalidate();
      label1.Invalidate();
      generate();
    }

    private void buttonColor2_Click ( object sender, EventArgs e )
    {
      DialogResult res = colorDialog1.ShowDialog();
      if ( res == DialogResult.Cancel ) return;
      baseColor2 = colorDialog1.Color;
      baseBox2.color = baseColor2;
      label2.Text = color2string( baseColor2 );
      baseBox2.Invalidate();
      label2.Invalidate();
      generate();
    }

    private void numColors_ValueChanged ( object sender, EventArgs e )
    {
      numCol = (int)numColors.Value;
      if ( numCol > 9 ) numCol = 9;
      if ( numCol < 3 ) numCol = 3;
      generate();
    }

    public static string color2string ( Color col )
    {
      return "[" + col.R + ',' + col.G + ',' + col.B + ']';
    }

    protected void generate ()
    {
      // !!!{{ TODO - generate custom palette based on numCol, baseColor1 [and baseColor2]

      colors = new Color[ numCol ];
      colors[ 0 ] = baseColor1;
      float r = baseColor1.R;
      float g = baseColor1.G;
      float b = baseColor1.B;
      float dr = (baseColor2.R - r) / (numCol - 1.0f);
      float dg = (baseColor2.G - g) / (numCol - 1.0f);
      float db = (baseColor2.B - b) / (numCol - 1.0f);
      for ( int i = 1; i < numCol; i++ )
      {
        r += dr; g += dg; b += db;
        colors[ i ] = Color.FromArgb( (int)r, (int)g, (int)b );
      }

      // !!!}}

      pictureBox1.Invalidate();
    }
  }
}
