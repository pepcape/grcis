using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _058marbles
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    #region OpenGL globals

    public static uint[] VBOid = new uint[ 2 ];       // vertex array (colors, normals, coords), index array

    #endregion

    #region FPS counter

    long lastFpsTime = 0L;
    int frameCounter = 0;
    static public long triangleCounter = 0L;

    double lastFrameTime = DateTime.Now.Ticks * 1.0e-7;
    
    #endregion

    public Form1 ()
    {
      InitializeComponent();
      String[] tok = "$Rev$".Split( new char[] { ' ' } );
      Text += " (rev: " + tok[ 1 ] + ')';
    }

    private void buttonInit_Click ( object sender, EventArgs e )
    {
      Cursor.Current = Cursors.WaitCursor;
      string param = textParam.Text;

      // TODO: stop the simulatino thread

      if ( world == null )
        world = new MarblesWorld();
      world.Init( param );
      data = null;

      // TODO: start the simulation thread

      Cursor.Current = Cursors.Default;

      glControl1.Invalidate();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      loaded = true;

      // OpenGL init code:
      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      // Simulation scene and related stuff:
      world = new MarblesWorld();
      renderer = new MarblesRenderer();
      world.Init( "" );

      SetupViewport();

      Application.Idle += new EventHandler( Application_Idle );

      // TODO: start the simulation thread
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
