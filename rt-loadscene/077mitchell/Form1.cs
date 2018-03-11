using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Utilities;

// Unreachable core warning:
#pragma warning disable 162

namespace _077mitchell
{
  public partial class Form1 : Form
  {
    private Canvas drawingCanvas;

    static string rev = "$Rev$".Split( ' ' )[ 1 ];

    public Form1 ()
    {
      InitializeComponent();
      Text = "Mitchell (rev: " + rev + ')';

      // Init dynamic form content.
      InitializeComboBoxes();
      InitializeParams();

      // boss settings:
      //comboSampling.SelectedIndex = comboSampling.Items.IndexOf( "Random" );
      //comboDensity.SelectedIndex = comboDensity.Items.IndexOf( DefaultPdf.PDF_UNIFORM );
      //densityFile = "";
      //numericSamples.Value = 1024;
      //numericSeed.Value = 12;
      //numericResolution.Value = 512;
      //textParams.Text = "k=6";

      // WPF hosted Canvas:
      drawingCanvas = new Canvas();
      drawingCanvas.Background = System.Windows.Media.Brushes.White;
      elementHostCanvas.Child = drawingCanvas;
    }

    /// <summary>
    /// Initialize combo-boxes:
    /// 1. sampling methods: source - sampling registry DefaultSampling.Samplings.
    /// 2. density functions: uniform, file, xxx?.
    /// </summary>
    protected void InitializeComboBoxes ()
    {
      // 1. sampling methods
      foreach ( var kvp in DefaultSampling.Samplings )
        comboSampling.Items.Add( kvp.Key );

      comboSampling.SelectedIndex = comboSampling.Items.IndexOf( "Random" );

      // 1. density functions
      comboDensity.Items.Add( DefaultPdf.PDF_UNIFORM );
      comboDensity.Items.Add( DefaultPdf.PDF_IMAGE );
      comboDensity.Items.Add( DefaultPdf.PDF_RAMP );
      comboDensity.Items.Add( DefaultPdf.PDF_COSRR );
      comboDensity.Items.Add( DefaultPdf.PDF_SINCR );
      comboDensity.Items.Add( DefaultPdf.PDF_SINCOS );

      comboDensity.SelectedIndex = comboDensity.Items.IndexOf( DefaultPdf.PDF_UNIFORM );
    }

    /// <summary>
    /// GUI update induced by a new sample-set.
    /// </summary>
    public void InitFromSampleSet ()
    {
      if ( sset == null )
        return;

      // GUI:
      numericSamples.Value = sset.samples.Count;
      comboSampling.SelectedIndex = comboSampling.Items.IndexOf( sset.samplingSource );
      numericSeed.Value = sset.seed;
      textParams.Text = sset.samplingParams;
    }

    private double currSizeX = 100.0;
    private double currSizeY = 100.0;
    private bool dirty = false;

    private double rectangleSize = 100.0;
    private double origX = 0.0;
    private double origY = 0.0;

    /// <summary>
    /// Absolute (negative) or relative (positive) measure: pen width,
    /// point diameter, etc.
    /// </summary>
    /// <returns>Absolute measure in pixels.</returns>
    private double AbsRelSize ( double x )
    {
      return (x < 0.0) ? -x : rectangleSize * x;
    }

    /// <summary>
    /// Update geometry constants (origin, scale factor..) from (a new) canvas geometry.
    /// </summary>
    private void updateGeometry ()
    {
      rectangleSize = Math.Min( currSizeX, currSizeY );
      double border = AbsRelSize( 0.005 );
      origX = 0.5 * (currSizeX - rectangleSize);
      origY = 0.5 * (currSizeY - rectangleSize);
      rectangleSize -= border;
      origX += 0.5 * border;
      origY += 0.5 * border;
    }

    /// <summary>
    /// Check for window-geometry change.
    /// </summary>
    /// <returns>True if changed.</returns>
    private bool checkCanvasSize ()
    {
      var sizeX = drawingCanvas.ActualWidth;
      var sizeY = drawingCanvas.ActualHeight;
      if ( sizeX == currSizeX && sizeY == currSizeY && !dirty )
        return false;

      dirty = false;
      currSizeX = sizeX;
      currSizeY = sizeY;

      return true;
    }

    /// <summary>
    /// File-name of the image function used for sampling density.
    /// </summary>
    public string densityFile = "";

    /// <summary>
    /// Global stopwatch for sampling thread. Locked access.
    /// </summary>
    protected Stopwatch sw = new Stopwatch();

    /// <summary>
    /// Sampling master thread.
    /// </summary>
    protected Thread aThread = null;

    /// <summary>
    /// Current sampling class.
    /// </summary>
    protected ISampling sampler = null;

    /// <summary>
    /// Current density-defining object.
    /// </summary>
    protected IPdf density = null;

    /// <summary>
    /// Current sample-set.
    /// </summary>
    protected SampleSet sset = null;

    delegate void SetSampleSetCallback ( SampleSet newSampleSet );

    protected void SetSampleSet ( SampleSet newSampleSet )
    {
      if ( elementHostCanvas.InvokeRequired )
      {
        SetSampleSetCallback sss = new SetSampleSetCallback( SetSampleSet );
        BeginInvoke( sss, new object[] { newSampleSet } );
      }
      else
      {
        numericSamples.Value = newSampleSet.samples.Count;
        regenerateSampleSet( newSampleSet );
        elementHostCanvas.Invalidate();
      }
    }

    delegate void SetTextCallback ( string text );

    protected void SetText ( string text )
    {
      if ( labelStatus.InvokeRequired )
      {
        SetTextCallback st = new SetTextCallback( SetText );
        BeginInvoke( st, new object[] { text } );
      }
      else
        labelStatus.Text = text;
    }

    protected void Buttons ( bool enable )
    {
      buttonGenerate.Enabled       = enable;
      buttonRedraw.Enabled         = enable;
      buttonResize.Enabled         = enable;
      buttonSaveSamples.Enabled    = enable;
      buttonExportDrawing.Enabled  = enable;
      buttonExportImage.Enabled    = enable;
      buttonDensityFile.Enabled    = enable;

      buttonStop.Enabled           = !enable;
    }

    delegate void StopGeneratingCallback ();

    protected void StopGenerating ()
    {
      if ( aThread == null )
        return;

      if ( buttonGenerate.InvokeRequired )
      {
        StopGeneratingCallback ea = new StopGeneratingCallback( StopGenerating );
        BeginInvoke( ea );
      }
      else
      {
        // actually stop the sampling:
        sampler.Break = true;
        aThread.Join();
        aThread = null;

        // GUI stuff:
        Buttons( true );
      }
    }

    /// <summary>
    /// Generates sampling set with current settings.
    /// </summary>
    private void Generate ()
    {
      lock ( sw )
      {
        sw.Restart();
      }

      // sampler parameters:
      sampler.Break = false;
      long seed = sampler.SetSeed( (long)numericSeed.Value );

      // result sample set object:
      SampleSet newSet = new SampleSet();
      newSet.samplingSource = sampler.Name;
      newSet.samplingParams = textParams.Text;
      newSet.seed = seed;

      // generating actual samples:
      int number = sampler.GenerateSampleSet( newSet, (int)numericSamples.Value, textParams.Text );

      long elapsed;
      lock ( sw )
      {
        sw.Stop();
        elapsed = sw.ElapsedMilliseconds;
      }

      SetSampleSet( newSet );

      long[] stat = sampler.Stat;
      string msg = string.Format( CultureInfo.InvariantCulture,
                                  "{0} samples generated in {1:f2}s, seed={2}, hash={3:X}, stat={4}-{5}-{6}",
                                  number, 1.0e-3 * elapsed, seed,
                                  sampler.Hash, stat[0], stat[1], stat[2] );
      SetText( msg );
      Util.Log( string.Format( CultureInfo.InvariantCulture, "Generating samples finished in {0:f2}s: {1}, samples={2}, param='{3}', seed={4}",
                               1.0e-3 * elapsed, newSet.samplingSource, number, newSet.samplingParams, seed ) );

      StopGenerating();
    }

    private void regenerateSampleSet ( SampleSet newSampleSet =null )
    {
      if ( newSampleSet != null )
        sset = newSampleSet;

      checkCanvasSize();
      updateGeometry();

      // recreate all the graphics objects:
      drawingCanvas.Children.Clear();

      if ( sset != null )
      {
        double pointSize = AbsRelSize( 0.005 );
        double hPointSize = 0.5 * pointSize;
        SolidColorBrush brush = System.Windows.Media.Brushes.DarkRed;
        foreach ( var v in sset.samples )
        {
          Ellipse el = new Ellipse
          {
            Fill = brush,
            Width = pointSize,
            Height = pointSize
          };
          Canvas.SetLeft( el, origX + rectangleSize * v.X - hPointSize );
          Canvas.SetTop( el, origY + rectangleSize * v.Y - hPointSize );
          drawingCanvas.Children.Add( el );
        }
      }

      decorateCanvas();

      Size currSize = elementHostCanvas.Size;
      int res = (int)numericResolution.Value;
      labelDrawing.Text = string.Format( "{0}: {1} x {2}", res, currSize.Width, currSize.Height );
    }

    /// <summary>
    /// Draw the whole canvas graphics from scratch.
    /// </summary>
    private void drawCanvas ()
    {
      regenerateSampleSet();
    }

    /// <summary>
    /// Prepare the sampler instance.
    /// </summary>
    /// <param name="gui">Sampler type from GUI (combo-box)?</param>
    private void checkSampler ( bool gui )
    {
      string name = comboSampling.Text;
      if ( !gui && sset != null )
        name = sset.samplingSource;
      if ( DefaultSampling.Samplings.ContainsKey( name ) )
        sampler = DefaultSampling.Samplings[ name ];
      else
        sampler = new RandomSampling();

      string dname = comboDensity.Text;
      switch ( dname )
      {
        case DefaultPdf.PDF_IMAGE:
          if ( densityFile == null ||
                !File.Exists( densityFile ) )
          {
            Util.Log( "Invalid density file: " + densityFile ?? "null" );
            density = new DensityFunction( -1 );
            break;
          }
          density = new RasterPdf( densityFile, checkNegative.Checked );
          break;

        case DefaultPdf.PDF_UNIFORM:
          density = new DensityFunction( -1 );
          break;

        case DefaultPdf.PDF_RAMP:
          density = new DensityFunction( 0 );
          break;

        case DefaultPdf.PDF_COSRR:
          density = new DensityFunction( 1 );
          break;

        case DefaultPdf.PDF_SINCR:
          density = new DensityFunction( 2 );
          break;

        case DefaultPdf.PDF_SINCOS:
          density = new DensityFunction( 3 );
          break;
      }
      sampler.Density = density;

      sampler.Break = false;
    }

    private void decorateCanvas ()
    {
      double border = AbsRelSize( 0.005 );
      System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
      {
        Width  = rectangleSize + border,
        Height = rectangleSize + border,
        Stroke = System.Windows.Media.Brushes.DarkBlue,
        StrokeThickness = border
      };
      Canvas.SetLeft( rect, origX - 0.5 * border );
      Canvas.SetTop(  rect, origY - 0.5 * border );
      drawingCanvas.Children.Add( rect );
    }

    private void ResizeWindow ()
    {
      Size currSize = elementHostCanvas.Size;
      Size newSize = Size;
      int res = (int)numericResolution.Value;
      if ( currSize.Width < res )
        newSize.Width += res - currSize.Width;
      newSize.Height += res - currSize.Height;
      Size = newSize;
    }

    /// <summary>
    /// Exports the given Canvas to an PNG raster file.
    /// </summary>
    /// <param name="path">PNG file-name.</param>
    /// <param name="surface">Vector surface to render.</param>
    public void ExportToPng ( string path, Canvas surface )
    {
      if ( path == null ||
           surface == null ) return;

      // Save current canvas transform
      Transform transform = surface.LayoutTransform;
      // reset current transform (in case it is scaled or rotated)
      surface.LayoutTransform = null;

      // Get the size of canvas
      System.Windows.Size size = new System.Windows.Size( surface.ActualWidth, surface.ActualHeight );
      // Measure and arrange the surface
      // VERY IMPORTANT
      surface.Measure( size );
      surface.Arrange( new System.Windows.Rect( size ) );

      // Create a render bitmap and push the surface to it
      RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
        (int)size.Width,
        (int)size.Height,
        96d,
        96d,
        PixelFormats.Pbgra32 );
      renderBitmap.Render( surface );

      // Create a file stream for saving image
      using ( FileStream outStream = new FileStream( path, FileMode.Create ) )
      {
        // Use png encoder for our data
        PngBitmapEncoder encoder = new PngBitmapEncoder();
        // push the rendered bitmap to it
        encoder.Frames.Add( BitmapFrame.Create( renderBitmap ) );
        // save the data to the stream
        encoder.Save( outStream );
      }

      // Restore previously saved layout
      surface.LayoutTransform = transform;
    }

    //----------- GUI stuff -----------

    private void buttonGenerate_Click ( object sender, EventArgs e )
    {
      if ( aThread != null )
        return;

      // GUI stuff:
      Buttons( false );

      // Prepare sampler class:
      checkSampler( true );
      DefaultSampling.tweakGrid = false;

      // Do the job in separate thread:
      aThread = new Thread( new ThreadStart( this.Generate ) );
      aThread.Start();
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      StopGenerating();
    }

    private void Form1_Load ( object sender, EventArgs e )
    {
      checkCanvasSize();
      updateGeometry();
      decorateCanvas();
    }

    private void Form1_Resize ( object sender, EventArgs e )
    {
      if ( WindowState == FormWindowState.Minimized )
        return;

      dirty = true;
      drawCanvas();
    }

    private void buttonSaveSamples_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ||
           sset == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save SampleSet file";
      sfd.Filter = "SYT Files|*.syt";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      sw.Restart();

      StreamWriter wri = new StreamWriter( sfd.FileName );
      sset.TextWrite( wri, '|' );
      wri.Flush();
      long len = wri.BaseStream.Position;
      wri.Close();

      sw.Stop();
      long elapsed = sw.ElapsedMilliseconds;
      labelStatus.Text = string.Format( CultureInfo.InvariantCulture, "SampleSet saved in {0:f2}s: {1}, {2}bytes",
                                        1.0e-3 * elapsed, sfd.FileName, Util.kmg( len ) );
    }

    private void buttonExportImage_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ||
           sset == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      sw.Restart();

      int res = (int)numericResolution.Value;

      Bitmap image = sset.ExportImage( res );
      try
      {
        image.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
        image.Dispose();
      }
      catch ( Exception ex )
      {
        MessageBox.Show( "Error saving image '" + sfd.FileName + "'!" );
        Util.Log( ex.ToString() );
      }

      sw.Stop();
      long elapsed = sw.ElapsedMilliseconds;
      labelStatus.Text = string.Format( CultureInfo.InvariantCulture, "Image export finished in {0:f2}s: resolution={1}",
                                        1.0e-3 * elapsed, res );

      Util.Log( string.Format( CultureInfo.InvariantCulture, "Image export finished in {0:f2}s: resolution={1}, file={2}",
                               1.0e-3 * elapsed, res, sfd.FileName ) );
    }

    private void buttonResize_Click ( object sender, EventArgs e )
    {
      ResizeWindow();
    }

    private void buttonExportDrawing_Click ( object sender, EventArgs e )
    {
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      sw.Restart();

      ExportToPng( sfd.FileName, drawingCanvas );

      sw.Stop();
      long elapsed = sw.ElapsedMilliseconds;
      string msg = string.Format( CultureInfo.InvariantCulture, "Drawing export finished in {0:f2}s: file={1}",
                                  1.0e-3 * elapsed, sfd.FileName );
      labelStatus.Text = msg;
      Util.Log( msg );
    }

    private void buttonDensityFile_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Density Image File";
      ofd.Filter = "PNG Files|*.png" +
          "|All image types|*.png;*.jpg;*.bmp";

      ofd.FilterIndex = 1;
      ofd.FileName = densityFile;
      if ( ofd.ShowDialog() != DialogResult.OK )
      {
        buttonDensityFile.Text = "Density file";
        return;
      }

      string fullPath = ofd.FileName;
      string fileOnly = System.IO.Path.GetFileName( fullPath );
      string directory = System.IO.Path.GetDirectoryName( fullPath );
      string currentDir = Environment.CurrentDirectory;
      densityFile = (directory == currentDir) ? fileOnly : fullPath;

      buttonDensityFile.Text = fileOnly;
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ||
           sset == null )
        return;

      if ( sampler == null )
        checkSampler( false );

      dirty = true;
      drawCanvas();
    }
  }
}
