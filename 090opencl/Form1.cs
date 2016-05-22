using System;
using System.Windows.Forms;
using Cloo;

namespace _090opencl
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    public static Form1 singleton = null;

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
    /// Current OpenCL context.
    /// </summary>
    public ComputeContext clContext = null;

    /// <summary>
    /// Current OpenCL platform.
    /// </summary>
    public ComputePlatform clPlatform = null;

    /// <summary>
    /// Current OpenCL device.
    /// </summary>
    public ComputeDevice clDevice = null;

    public Form1 ()
    {
      singleton = this;

      InitializeComponent();

      // Mandelbrot singleton:
      mandel = new Mandelbrot();

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

    void comboBoxPlatform_SelectedIndexChanged ( object sender, EventArgs e )
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
        clDevice = null;
        return;
      }

      clDevice = clPlatform.Devices[ comboBoxDevice.SelectedIndex ];
    }
  }
}
