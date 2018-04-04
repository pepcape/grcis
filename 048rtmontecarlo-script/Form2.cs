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

      InitializeComponent();
    }

    private void Form2_FormClosed(object sender, FormClosedEventArgs e)
    {
      Form2.singleton = null;
    }

    private void SaveDepthMapButton_Click(object sender, EventArgs e)
    {
      if (outputImage == null /*||
          aThread != null*/) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title        = "Save PNG file";
      sfd.Filter       = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName     = "DepthMap.png";
      if (sfd.ShowDialog() != DialogResult.OK)
        return;

      outputImage.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
    }
    private void SaveIntensityMapButton_Click(object sender, EventArgs e)
    {

    }

    private void RenderDepthMapButton_Click(object sender, EventArgs e)
    {
      AdvancedTools.DepthMap.RenderDepthMap ();

      singleton.DepthMapPictureBox.Image = AdvancedTools.DepthMap.GetBitmap ();
    }

    private void RenderIntensityMapButton_Click(object sender, EventArgs e)
    {
      AdvancedTools.IntensityMap.RenderIntensityMap();

      singleton.DepthMapPictureBox.Image = AdvancedTools.IntensityMap.GetBitmap();
    }    
  }
}
