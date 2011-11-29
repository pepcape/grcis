using System;
using System.Windows.Forms;
using Scene3D;

namespace _039terrain
{
  public partial class Form1 : Form
  {
    protected SceneBrep scene = new SceneBrep();

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    #region FPS counter

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long triangleCounter = 0L;

    #endregion

    public Form1 ()
    {
      InitializeComponent();
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
      Render();
    }
  }
}
