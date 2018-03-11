using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace _059graph
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    public Form1 ()
    {
      InitializeComponent();
      string[] tok = "$Rev$".Split( ' ' );
      Text += " (rev: " + tok[ 1 ] + ')';
    }

    private void buttonRedefine_Click ( object sender, EventArgs e )
    {
      Cursor.Current = Cursors.WaitCursor;
      string expression = textFunction.Text;
      string intervals = textIntervals.Text;

      // re-generate the graph..
      string err = RegenerateGraph( expression, intervals );
      Cursor.Current = Cursors.Default;

      if ( err != null )
      {
        MessageBox.Show( err, "Error" );
        return;
      }

      glControl1.Invalidate();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      loaded = true;

      // OpenGL init code:
      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      // VBO init:
      GL.GenBuffers( 2, VBOid );

      SetupViewport();

      Application.Idle += new EventHandler( Application_Idle );
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      SetupViewport();
      glControl1.Invalidate();
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      Render();
    }
  }
}
