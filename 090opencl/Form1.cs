using System;
using System.Windows.Forms;
using Cloo;
using OpenTK.Graphics;
using OpenclSupport;
using Utilities;
using System.Collections.Generic;

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

    /// <summary>
    /// Are we able to use OpenCL-GL interop?
    /// </summary>
    public bool CanUseInterop = false;

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

      int platforms = ComputePlatform.Platforms.Count;
      if ( platforms == 0 )
      {
        platforms = 1;
        checkOpenCL.Enabled = false;
      }

      object[] availablePlatforms = new object[ platforms ];
      for ( int i = 0; i < platforms; i++ )
        availablePlatforms[i] = (i < ComputePlatform.Platforms.Count) ? ComputePlatform.Platforms[ i ].Name : "- no OpenCL available -";

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
      comboBoxDevice.Items.Clear();
      if ( comboBoxPlatform.SelectedIndex >= ComputePlatform.Platforms.Count )
      {
        clPlatform = null;
        return;
      }

      clPlatform = ComputePlatform.Platforms[ comboBoxPlatform.SelectedIndex ];

      object[] availableDevices = new object[ clPlatform.Devices.Count ];
      for ( int i = 0; i < availableDevices.Length; i++ )
        availableDevices[ i ] = clPlatform.Devices[ i ].Name;

      comboBoxDevice.Items.AddRange( availableDevices );
      comboBoxDevice.SelectedIndex = 0;
    }

    public void SetupClContext ()
    {
      CanUseDouble = CanUseInterop = false;

      if ( clPlatform == null ||
           comboBoxDevice.SelectedIndex >= clPlatform.Devices.Count )
      {
        clDevice = null;
        clContext = null;
        return;
      }

      try
      {
        clDevice = clPlatform.Devices[ comboBoxDevice.SelectedIndex ];
        ComputeContextPropertyList properties = new ComputeContextPropertyList( clPlatform );
        IGraphicsContextInternal ctx = (IGraphicsContextInternal)GraphicsContext.CurrentContext;
        properties.Add( new ComputeContextProperty( ComputeContextPropertyName.CL_GL_CONTEXT_KHR, ctx.Context.Handle ) );
        properties.Add( new ComputeContextProperty( ComputeContextPropertyName.CL_WGL_HDC_KHR,    OpenGL.wglGetCurrentDC() ) );
        properties.Add( new ComputeContextProperty( ComputeContextPropertyName.Platform,          clPlatform.Handle.Value ) );
        clContext = new ComputeContext( new ComputeDevice[] { clDevice }, properties, null, IntPtr.Zero );

        // check the double-extension:
        CanUseDouble  = ClInfo.ExtensionCheck( clContext, "cl_khr_fp64" );
        CanUseInterop = ClInfo.ExtensionCheck( clContext, "cl_khr_gl_sharing" );
        ClInfo.LogCLProperties( clContext, true );
      }
      catch ( Exception exc )
      {
        Util.LogFormat( "OpenCL error: {0}", exc.Message );
        clDevice = null;
        clContext = null;
      }

      bool enableDouble = !checkOpenCL.Checked ||
                          CanUseDouble;
      checkDouble.Enabled = enableDouble;
      checkInterop.Enabled = CanUseInterop;
    }

    private void comboBoxDevice_SelectedIndexChanged ( object sender, EventArgs e )
    {
      clDirty   = true;
      clDevice  = null;
      clContext = null;
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

    private void checkInterop_CheckedChanged ( object sender, EventArgs e )
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
