using System;
using System.Drawing;
using System.Windows.Forms;
using MathSupport;
using Utilities;

namespace _115colormandala
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    /// <summary>
    /// Generated colormap.
    /// </summary>
    public static Color[] colors;

    /// <summary>
    /// Required colormap size (5 to 40).
    /// </summary>
    public static int numCol = 10;

    /// <summary>
    /// Param string tooltip = help.
    /// </summary>
    string tooltip = "";

    /// <summary>
    /// Shared ToolTip instance.
    /// </summary>
    ToolTip tt = new ToolTip();

    public Form1 ()
    {
      InitializeComponent();

      string name;
      string param;
      Colormap.InitParams( out name, out param, out tooltip );
      textParam.Text = param ?? "";
      Text += " (" + rev + ") '" + name + '\'';

      recompute();
    }

    private void recompute ()
    {
      numCol = Arith.Clamp( (int)numColors.Value, 5, 40 );
      Colormap.Generate( numCol, out colors, textParam.Text );
      pictureBox1.Invalidate();
    }

    private void numColors_ValueChanged ( object sender, EventArgs e )
    {
      recompute();
    }

    private void buttonRecompute_Click ( object sender, EventArgs e )
    {
      recompute();
    }

    private void buttonRecompute_MouseHover ( object sender, EventArgs e )
    {
      tt.Show( Util.TargetFramework + " (" + Util.RunningFramework + ")",
               (IWin32Window)sender, 10, -25, 2000 );
    }

    private void textParam_MouseHover ( object sender, EventArgs e )
    {
      tt.Show( tooltip, (IWin32Window)sender, 10, -25, 2000 );
    }

    private void textParam_KeyPress ( object sender, KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        recompute();
      }
    }
  }
}
