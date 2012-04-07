using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Compression;
using ScreenShot;

namespace _016videoslow
{
  public partial class Form1 : Form
  {
    protected Bitmap frameImage = null;

    protected string videoFileName = "video.bin";
    
    public Form1 ()
    {
      InitializeComponent();
    }

    private void buttonCapture_Click ( object sender, EventArgs e )
    {
      this.WindowState = FormWindowState.Minimized;
      ScreenCapture sc = new ScreenCapture();
      string fn = String.Format( textInputMask.Text, 0 );
      string dir = Path.GetDirectoryName( fn );
      Directory.CreateDirectory( dir );
      dir += "\\capturelog.txt";
      StreamWriter log = new StreamWriter( new FileStream( dir, FileMode.Create ) );

      Image im;
      Stopwatch sw = new Stopwatch();
      Thread.Sleep( 200 );

      int  frameNo   = 0;
      long msCurrent;       // time of the next capture in milliseconds
      long msTotal   = (long)((double)numericDuration.Value * 1000.0);

      sw.Reset();
      sw.Start();
      do
      {
        im = sc.CaptureScreen();
        fn = String.Format( textInputMask.Text, frameNo++ );
        im.Save( fn, ImageFormat.Png );
        msCurrent = (long)(frameNo * 1000.0 / (double)numericFps.Value);
        long msSleep = msCurrent - sw.ElapsedMilliseconds;
        if ( msSleep > 0 )
        {
          log.WriteLine( "Frame {0:0000}: sleeping {1} ms", frameNo, msSleep );
          Thread.Sleep( (int)msSleep );
        }
        else
          log.WriteLine( "Frame {0:0000}: busy! ({1} ms)", frameNo, msSleep );
      }
      while ( sw.ElapsedMilliseconds < msTotal );
      labelSpeed.Text = String.Format( "Captured {0} frames in {1:f} s!", frameNo, (float)(sw.ElapsedMilliseconds * 0.001) );
      log.Close();
      sw.Stop();

      this.WindowState = FormWindowState.Normal;
    }

    private void buttonEncode_Click ( object sender, EventArgs e )
    {
      string fn = String.Format( textInputMask.Text, 0 );
      if ( !File.Exists( fn ) )
      {
        MessageBox.Show( "Invalid input files!" );
        return;
      }

      string cd = Directory.GetCurrentDirectory();
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save Video File";
      sfd.Filter = "BIN Files|*.bin";
      sfd.AddExtension = true;
      sfd.FileName = videoFileName;
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      videoFileName = Path.GetFullPath( sfd.FileName );
      FileStream fs = new FileStream( videoFileName, FileMode.Create );
      VideoCodec vc = new VideoCodec();
      Directory.SetCurrentDirectory( cd );

      labelSpeed.Text = "Encoding..";
      Stopwatch sw = new Stopwatch();
      sw.Start();

      Image inp = Image.FromFile( fn );
      frameImage = new Bitmap( inp );
      inp.Dispose();
      IEntropyCodec s = vc.EncodeHeader( frameImage.Width, frameImage.Height, (float)numericFps.Value, fs );
      int i = 0;
      do
      {
        vc.EncodeFrame( i, frameImage, s );
        fn = String.Format( textInputMask.Text, ++i );
        if ( !File.Exists( fn ) ) break;
        inp = Image.FromFile( fn );
        frameImage = new Bitmap( inp );
        inp.Dispose();
      } while ( true );

      s.Close();
      labelSpeed.Text = String.Format( "Encoded {0} frames in {1:f} s!", i, (float)(sw.ElapsedMilliseconds * 0.001) );
      sw.Stop();
      fs.Close();
    }

    private void buttonDecode_Click ( object sender, EventArgs e )
    {
      string fn = String.Format( textOutputMask.Text, 0 );
      string dir = Path.GetDirectoryName( fn );
      Directory.CreateDirectory( dir );

      FileStream fs = new FileStream( videoFileName, FileMode.Open );
      IEntropyCodec s;

      if ( fs == null ) return;

      labelSpeed.Text = "Decoding..";
      Stopwatch sw = new Stopwatch();
      sw.Start();

      VideoCodec vc = new VideoCodec();
      s = vc.DecodeHeader( fs );
      int i = 0;
      do
      {
        frameImage = vc.DecodeFrame( i, s );
        if ( frameImage == null )
        {
          s.Close();
          fs.Close();
          labelSpeed.Text = String.Format( "Decoded {0} frames in {1:f} s!", i, (float)(sw.ElapsedMilliseconds * 0.001) );
          sw.Stop();
          return;
        }
        fn = String.Format( textOutputMask.Text, i++ );
        frameImage.Save( fn, ImageFormat.Png );
      }
      while ( true );
    }
  }
}
