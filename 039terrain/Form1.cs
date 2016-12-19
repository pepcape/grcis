using System;
using System.Windows.Forms;
using Scene3D;
using Utilities;

namespace _039terrain
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    SceneBrep scene = new SceneBrep();

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    #region FPS counter

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long triangleCounter = 0L;
    double lastFps = 0.0;
    double lastTps = 0.0;

    #endregion

    public Form1 ()
    {
      InitializeComponent();

      int iter;
      float rough;
      string param;
      string name;
      InitParams( out iter, out rough, out param, out name );
      textParam.Text = param ?? "";
      upDownIterations.Value = iter;
      upDownRoughness.Value = (decimal)rough;

      Text += " (" + rev + ") '" + name + '\'';
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      //if ( outputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      //outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      SetupViewport();
      glControl1.Invalidate();
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      if ( checkAnim.Checked )
        Simulate( DateTime.Now.Ticks * 1.0e-7 );

      Render();
    }

    private void upDownIterations_ValueChanged ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      int iterations  = (int)upDownIterations.Value;
      float roughness = (float)upDownRoughness.Value;
      Regenerate( iterations, roughness, textParam.Text );
      labelStatus.Text = "Triangles: " + scene.Triangles;
    }

    private void buttonResetCam_Click ( object sender, EventArgs e )
    {
      ResetCamera();
    }

    private void checkVsync_CheckedChanged ( object sender, EventArgs e )
    {
      glControl1.VSync = checkVsync.Checked;
    }

    private void checkAnim_CheckedChanged ( object sender, EventArgs e )
    {
      if ( checkAnim.Checked )
        InitSimulation( false );
    }

    private void buttonRegenerate_Click ( object sender, EventArgs e )
    {
      int iterations = (int)upDownIterations.Value;
      float roughness = (float)upDownRoughness.Value;
      Regenerate( iterations, roughness, textParam.Text );
      labelStatus.Text = "Triangles: " + scene.Triangles;
    }
  }
}
