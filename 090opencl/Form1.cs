using System;
using System.Windows.Forms;
using Cloo;
using OpenTK.Graphics.OpenGL;
using OpenclSupport;
using Utilities;

namespace _090opencl
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    /// <summary>
    /// Mandelbrot singleton.
    /// </summary>
    Mandelbrot mandel = null;

    /// <summary>
    /// If true, the whole OpenCL must be [re-]initialized.
    /// </summary>
    public bool clDirty = true;

    /// <summary>
    /// Current OpenCL platform.
    /// </summary>
    public ComputePlatform clPlatform = null;

    /// <summary>
    /// Current OpenCL device.
    /// </summary>
    public ComputeDevice clDevice = null;

    /// <summary>
    /// Current OpenCL context.
    /// </summary>
    public ComputeContext clContext = null;

    /// <summary>
    /// Are we able to use 'double' type at all in current OpenCL context?
    /// </summary>
    public bool CanUseDouble = false;

    public Form1 ()
    {
      InitializeComponent();

      // Mandelbrot singleton:
      mandel = new Mandelbrot( this );

      // Param string:
      string param;
      Mandelbrot.InitParams( out param );
      textParam.Text = param ?? "";
      Text += " (rev: " + rev + ')';

      // Shaders:
      InitShaderRepository();

      // OpenCL GUI elements:
      object[] availablePlatforms = new object[ ComputePlatform.Platforms.Count ];
      for ( int i = 0; i < availablePlatforms.Length; i++ )
        availablePlatforms[ i ] = ComputePlatform.Platforms[ i ].Name;

      comboBoxPlatform.Items.AddRange( availablePlatforms );
      comboBoxPlatform.SelectedIndex = 0;
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      InitOpenGL();
      mandel.UpdateParams( textParam.Text );
      SetupViewport();

      loaded = true;
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
      ComputeRender();
    }

    private void checkVsync_CheckedChanged ( object sender, EventArgs e )
    {
      glControl1.VSync = checkVsync.Checked;
    }

    private void textParam_KeyPress ( object sender, System.Windows.Forms.KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        mandel.UpdateParams( textParam.Text );
      }
    }

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      DestroyClBuffers();
      DestroyTexture( ref texName );
      DestroyShaders();
    }

    private void comboBoxPlatform_SelectedIndexChanged ( object sender, EventArgs e )
    {
      clDirty = true;
      clPlatform = ComputePlatform.Platforms[ comboBoxPlatform.SelectedIndex ];

      object[] availableDevices = new object[ clPlatform.Devices.Count ];
      for ( int i = 0; i < availableDevices.Length; i++ )
        availableDevices[ i ] = clPlatform.Devices[ i ].Name;

      comboBoxDevice.Items.Clear();
      comboBoxDevice.Items.AddRange( availableDevices );
      comboBoxDevice.SelectedIndex = 0;
    }

    private void comboBoxDevice_SelectedIndexChanged ( object sender, EventArgs e )
    {
      clDirty = true;
      if ( clPlatform == null )
      {
        clDevice  = null;
        clContext = null;
        return;
      }

      try
      {
        clDevice = clPlatform.Devices[ comboBoxDevice.SelectedIndex ];
        ComputeContextPropertyList properties = new ComputeContextPropertyList( clPlatform );
        clContext = new ComputeContext( new ComputeDevice[] { clDevice }, properties, null, IntPtr.Zero );

        // check the double-extension:
        CanUseDouble = ClInfo.ExtensionCheck( clContext, "cl_khr_fp64" );
        ClInfo.LogCLProperties( clContext, true );
      }
      catch ( Exception exc )
      {
        Util.LogFormat( "OpenCL error: {0}", exc.Message );
        clDevice  = null;
        clContext = null;
      }

      bool enableDouble = !checkOpenCL.Checked ||
                          CanUseDouble;
      checkDouble.Enabled = enableDouble;
    }

    private void checkOpenCL_CheckedChanged ( object sender, EventArgs e )
    {
      if ( checkOpenCL.Checked )
        checkDouble.Enabled = CanUseDouble;
      else
        checkDouble.Enabled = true;

      PrepareClBuffers();
    }

    private void checkDouble_CheckedChanged ( object sender, EventArgs e )
    {
      PrepareClBuffers();
    }

    private void checkBigGroup_CheckedChanged ( object sender, EventArgs e )
    {
      groupSize = checkBigGroup.Checked ? 16L : 8L;
      PrepareClBuffers();
    }
  }
}
