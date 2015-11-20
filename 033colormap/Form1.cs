using System;
using System.Drawing;
using System.Windows.Forms;

namespace _033colormap
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    /// <summary>
    /// Output color array = color palette.
    /// </summary>
    public static Color[] colors;

    //--------------------------------------------------
    // Input parameters for palette generation:
    public static Color baseColor1 = Color.Black;
    public static Color baseColor2 = Color.White;
    public static int numCol = 4;
    public static string param = "";

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';
      InitPaletteData();

      baseBox1.color = baseColor1;
      label1.Text = color2string( baseColor1 );
      baseBox2.color = baseColor2;
      label2.Text = color2string( baseColor2 );
      textParam.Text = param;
      numColors.Value = numCol;

      Colormap.Generate( baseColor1, baseColor2, numCol, param, out colors );
      pictureBox1.Invalidate();
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
      param = textParam.Text;

      Colormap.Generate( baseColor1, baseColor2, numCol, param, out colors );
      pictureBox1.Invalidate();
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
      param = textParam.Text;

      Colormap.Generate( baseColor1, baseColor2, numCol, param, out colors );
      pictureBox1.Invalidate();
    }

    private void numColors_ValueChanged ( object sender, EventArgs e )
    {
      numCol = (int)numColors.Value;
      if ( numCol > 9 ) numCol = 9;
      if ( numCol < 3 ) numCol = 3;
      param = textParam.Text;

      Colormap.Generate( baseColor1, baseColor2, numCol, param, out colors );
      pictureBox1.Invalidate();
    }

    public static string color2string ( Color col )
    {
      return "[" + col.R + ',' + col.G + ',' + col.B + ']';
    }

    private void textParam_KeyPress ( object sender, KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        param = textParam.Text;
        Colormap.Generate( baseColor1, baseColor2, numCol, param, out colors );
        pictureBox1.Invalidate();
      }
    }
  }
}
