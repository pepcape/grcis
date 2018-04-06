using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;

namespace _048rtmontecarlo
{
  public partial class Form2 : Form
  {
    public static Form2 singleton = null;

    protected Bitmap outputImage = null;

    public Form2()
    {
      singleton = this;

      Form1.singleton.AdvancedToolsButton.Enabled = false;

      InitializeComponent();

      this.StartPosition = FormStartPosition.Manual;

      // Sets location of Form2 (Advanced Tools) to either right or left of Form1 (Main) 
      // depending on position of Form1 (whether it is close to right edge of primary screen)
      if ( Form1.singleton.Location.X + Form1.singleton.Width + this.Width < Screen.PrimaryScreen.WorkingArea.Width ||
           Form1.singleton.Location.X - this.Width < 0)
      {
        this.Location = new Point(Form1.singleton.Location.X + Form1.singleton.Width, Form1.singleton.Location.Y);        // place to the right of Form1
      }
      else
      {
        this.Location = new Point(Form1.singleton.Location.X - this.Width, Form1.singleton.Location.Y);   // place to the left of Form1
      }
    }

    private void Form2_FormClosed(object sender, FormClosedEventArgs e)
    {
      Form1.singleton.AdvancedToolsButton.Enabled = true;

      singleton = null;
    }

    private void SaveDepthMapButton_Click(object sender, EventArgs e)
    {
      SavePictureButton ( DepthMapPictureBox.Image, "DepthMap" );
    }
    private void SaveIntensityMapButton_Click(object sender, EventArgs e)
    {
      SavePictureButton ( IntensityMapPictureBox.Image, "IntensityMap" );
    }

    /// <summary>
    /// Opens save file dialog for selected image (from pictureBox)
    /// Returns bool whether operation was successful
    /// </summary>
    private bool SavePictureButton (Image image, string defaultName)
    {
      outputImage = (Bitmap)image;

      if (outputImage == null)
        return false;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title        = "Save PNG file";
      sfd.Filter       = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName     = defaultName + ".png";
      if (sfd.ShowDialog() != DialogResult.OK)
        return false;

      outputImage.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);

      return true;
    }

    private void RenderDepthMapButton_Click(object sender, EventArgs e)
    {
      if (Form1.singleton.outputImage == null)
        return;

      AdvancedTools.DepthMap.RenderDepthMap ();

      DepthMapPictureBox.Image = AdvancedTools.DepthMap.GetBitmap ();

      SaveDepthMapButton.Enabled = true;
    }

    private void RenderIntensityMapButton_Click(object sender, EventArgs e)
    {
      if (Form1.singleton.outputImage == null)
        return;

      AdvancedTools.IntensityMap.RenderIntensityMap();

      IntensityMapPictureBox.Image = AdvancedTools.IntensityMap.GetBitmap();

      SaveIntensityMapButton.Enabled = true;
    }

    public void SetNewDimensions ( int formImageWidth, int formImageHeight )
    {
      IntensityMapPictureBox.Width = formImageWidth;
      IntensityMapPictureBox.Height = formImageHeight;

      DepthMapPictureBox.Width = formImageWidth;
      DepthMapPictureBox.Height = formImageHeight;

      AdvancedTools.SetNewDimensions ();
    }
  }
}
