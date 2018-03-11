using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using GuiSupport;
using MathSupport;
using Utilities;

namespace _029flow
{
  public class SyncObject
  {
    public Bitmap bmp;

    public long totalSpawned;

    public double simTime;
  }

  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    /// <summary>
    /// Current output raster image. Locked access.
    /// </summary>
    public Bitmap outputImage = null;

    /// <summary>
    /// Index of the current (selected) scene.
    /// </summary>
    public int selectedWorld = 0;

    /// <summary>
    /// Image width in pixels, 0 for default value (according to panel size).
    /// </summary>
    public int ImageWidth = 0;

    /// <summary>
    /// Image height in pixels, 0 for default value (according to panel size).
    /// </summary>
    public int ImageHeight = 0;

    /// <summary>
    /// Master thread - rendering, data collection.
    /// </summary>
    protected Thread aThread = null;

    public ComboBox ComboScene
    {
      get { return comboScene; }
    }

    public CheckBox CheckMultithreading
    {
      get { return checkMultithreading; }
    }

    public class SimulationProgress : Progress
    {
      protected Form1 f;

      protected FlowVisualization fv;

      public bool pressure = false;

      public bool velocity = false;

      public bool custom = false;

      public string param;

      protected long lastSync = 0L;

      public SimulationProgress ( Form1 _f, FlowVisualization _fv )
      {
        f = _f;
        fv = _fv;
        pressure = f.checkPressure.Checked;
        velocity = f.checkVelocity.Checked;
        custom   = f.checkCustom.Checked;
        param    = f.textParam.Text;
      }

      public void Reset ()
      {
        lastSync = 0L;
      }

      public bool NeedsSync ()
      {
        if ( fv.sw.ElapsedMilliseconds - lastSync < SyncInterval )
          return false;

        return true;
      }

      public override void Sync ( Object msg )
      {
        if ( !NeedsSync() )
          return;

        lastSync = fv.sw.ElapsedMilliseconds;
        SyncObject so = msg as SyncObject;
        if ( so == null )
          return;

        f.SetText( string.Format( CultureInfo.InvariantCulture, "Sync {0:f1}s: sim {1:f1}s, spawned {2}",
                                  1.0e-3 * lastSync, so.simTime, Util.kmg( so.totalSpawned ) ) );
        Bitmap nb;
        lock ( so.bmp )
          nb = (Bitmap)so.bmp.Clone();
        f.SetImage( nb );
      }
    }

    /// <summary>
    /// Progress info / user break handling.
    /// </summary>
    protected SimulationProgress progress = null;

    /// <summary>
    /// Current Flow-visualization object.
    /// </summary>
    FlowVisualization fVis = null;

    /// <summary>
    /// Mouse probe.
    /// </summary>
    private void probe ( int x, int y )
    {
      if ( fVis == null )
        return;

      labelSample.Text = fVis.Probe( x, y );
    }

    delegate void SetImageCallback ( Bitmap newImage );

    public void SetImage ( Bitmap newImage )
    {
      if ( pictureBox1.InvokeRequired )
      {
        SetImageCallback si = new SetImageCallback( SetImage );
        BeginInvoke( si, new object[] { newImage } );
      }
      else
      {
        pictureBox1.Image = newImage;
        pictureBox1.Invalidate();
      }
    }

    delegate void SetTextCallback ( string text );

    public void SetText ( string text )
    {
      if ( labelElapsed.InvokeRequired )
      {
        SetTextCallback st = new SetTextCallback( SetText );
        BeginInvoke( st, new object[] { text } );
      }
      else
        labelElapsed.Text = text;
    }

    delegate void StopSimulationCallback ();

    public void StopSimulation ()
    {
      if ( aThread == null )
        return;

      if ( buttonSimulation.InvokeRequired )
      {
        StopSimulationCallback ea = new StopSimulationCallback( StopSimulation );
        BeginInvoke( ea );
      }
      else
      {
        // actually stop the rendering:
        lock ( progress )
        {
          progress.Continue = false;
        }
        aThread.Join();
        aThread = null;

        // GUI stuff:
        SimModeGUI( false );
      }
    }

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';

      // Init scenes etc.
      fVis = new FlowVisualization( this );
      fVis.progress = progress = new SimulationProgress( this, fVis );
      fVis.InitializeScenes();
      buttonRes.Text = FormResolution.GetLabel( ref ImageWidth, ref ImageHeight );
    }

    protected void SimModeGUI ( bool sim )
    {
      buttonSimulation.Enabled =
      comboScene.Enabled    =
      buttonRes.Enabled     =
      buttonLoad.Enabled    =
      buttonSave.Enabled    = !sim;
      buttonStop.Enabled    = sim;
    }

    private void buttonRes_Click ( object sender, EventArgs e )
    {
      if ( aThread != null )
        return;

      FormResolution form = new FormResolution( ImageWidth, ImageHeight );
      if ( form.ShowDialog() == DialogResult.OK )
      {
        ImageWidth  = form.ImageWidth;
        ImageHeight = form.ImageHeight;
        buttonRes.Text = FormResolution.GetLabel( ref ImageWidth, ref ImageHeight );
        fVis.dirty = true;
      }
    }

    private void buttonSimulation_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ||
           fVis == null )
        return;

      SimModeGUI( true );
      lock ( progress )
      {
        progress.Continue = true;
      }

      // determine output image size:
      int width = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      int height = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;

      fVis.Width  = width;
      fVis.Height = height;
      fVis.UseMultithreading = checkMultithreading.Checked;

      aThread = new Thread( new ThreadStart( fVis.RunSimulation ) );
      aThread.Start();
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      StopSimulation();
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( outputImage == null ||
           aThread != null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void buttonResults_Click ( object sender, EventArgs e )
    {
      if ( fVis == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save CSV file";
      sfd.Filter = "CSV Files|*.csv";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      fVis.WorldName = comboScene.Items[ selectedWorld ].ToString();
      fVis.SaveResults( sfd.FileName );
    }

    private void buttonLoad_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ) return;

      OpenFileDialog ofd = new OpenFileDialog();
      ofd.Title = "Open CSV File";
      ofd.Filter = "CSV Files|*.csv;*.csv.gz" +
          "|Text Files|*.txt;*.txt.gz" +
          "|All file types|*.csv;*.csv.gz;*.txt;*.txt.gz";
      ofd.FilterIndex = 0;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      if ( fVis.LoadResults( ofd.FileName ) )
      {
        ImageWidth = fVis.Width;
        ImageHeight = fVis.Height;
        buttonRes.Text = FormResolution.GetLabel( ref ImageWidth, ref ImageHeight );
        comboScene.SelectedIndex = comboScene.Items.IndexOf( fVis.WorldName );
      }
    }

    private void comboScene_SelectedIndexChanged ( object sender, EventArgs e )
    {
      StopSimulation();
      selectedWorld = comboScene.SelectedIndex;
      fVis.WorldName = comboScene.Items[ fVis.SelectedWorld = selectedWorld ].ToString();
      fVis.dirty = true;
    }

    private void checkVelocity_CheckedChanged ( object sender, EventArgs e )
    {
      lock ( progress )
        progress.velocity = checkVelocity.Checked;
    }

    private void checkPressure_CheckedChanged ( object sender, EventArgs e )
    {
      lock ( progress )
        progress.pressure = checkPressure.Checked;
    }

    private void checkCustom_CheckedChanged ( object sender, EventArgs e )
    {
      lock ( progress )
        progress.custom = checkCustom.Checked;
    }

    private void textParam_TextChanged ( object sender, EventArgs e )
    {
      lock ( progress )
        progress.param = textParam.Text;
    }

    private void pictureBox1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( e.Button == MouseButtons.Left )
        probe( e.X, e.Y );
    }

    private void pictureBox1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( e.Button == MouseButtons.Left )
        probe( e.X, e.Y );
    }

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      StopSimulation();
    }
  }
}
