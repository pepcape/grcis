using System;
using System.Drawing;
using System.Windows.Forms;
using Rendering;

namespace _048rtmontecarlo
{
  public partial class Form2 : Form
  {
    public static Form2 singleton = null;

    protected Bitmap outputImage = null;

    public Form2 ()
    {
      singleton = this;

      Form1.singleton.AdvancedToolsButton.Enabled = false;

      InitializeComponent ();

      this.StartPosition = FormStartPosition.Manual;

      // Sets location of Form2 (Advanced Tools) to either right or left of Form1 (Main) 
      // depending on position of Form1 (whether it is close to right edge of primary screen)
      if ( Form1.singleton.Location.X + Form1.singleton.Width + this.Width < Screen.PrimaryScreen.WorkingArea.Width ||
           Form1.singleton.Location.X - this.Width < 0 )
      {
        this.Location =
          new Point ( Form1.singleton.Location.X + Form1.singleton.Width,
                      Form1.singleton.Location.Y ); // place to the right of Form1
      }
      else
      {
        this.Location =
          new Point ( Form1.singleton.Location.X - this.Width,
                      Form1.singleton.Location.Y ); // place to the left of Form1
      }
    }

    private void Form2_FormClosed ( object sender, FormClosedEventArgs e )
    {
      Form1.singleton.AdvancedToolsButton.Enabled = true;

      singleton = null;
    }

    private void SaveDepthMapButton_Click ( object sender, EventArgs e )
    {
      SavePictureButton ( DepthMapPictureBox.Image, "DepthMap" );
    }

    private void SavePrimaryRaysMapButton_Click ( object sender, EventArgs e )
    {
      SavePictureButton ( PrimaryRaysMapPictureBox.Image, "PrimaryRaysMap" );
    }

    private void SaveAllRaysMapButton_Click(object sender, EventArgs e)
    {
      SavePictureButton ( AllRaysMapPictureBox.Image, "AllRaysMap" );
    }

    /// <summary>
    /// Opens save file dialog for selected image (from pictureBox)
    /// Returns bool whether operation was successful
    /// </summary>
    private bool SavePictureButton ( Image image, string defaultName )
    {
      outputImage = (Bitmap) image;

      if ( outputImage == null )
        return false;

      SaveFileDialog sfd = new SaveFileDialog ();
      sfd.Title        = "Save PNG file";
      sfd.Filter       = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName     = defaultName + ".png";
      if ( sfd.ShowDialog () != DialogResult.OK )
        return false;

      outputImage.Save ( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );

      return true;
    }

    private void RenderDepthMapButton_Click ( object sender, EventArgs e )
    {
      if ( Form1.singleton.outputImage == null )
        return;

      AdvancedTools.DepthMap.RenderDepthMap ();

      DepthMapPictureBox.Image = AdvancedTools.DepthMap.GetBitmap ();

      SaveDepthMapButton.Enabled = true;    
    }

    private void RenderPrimaryRaysMapButton_Click ( object sender, EventArgs e )
    {
      if (Form1.singleton.outputImage == null)
        return;

      RenderRaysMap(AdvancedTools.PrimaryRaysMap, PrimaryRaysMapPictureBox, SavePrimaryRaysMapButton);

      SetTotalAndAveragePrimaryRaysCount ( Intersection.countRays, PrimaryRaysMapPictureBox.Image.Width, PrimaryRaysMapPictureBox.Image.Height );
    }

    private void RenderAllRaysMapButton_Click(object sender, EventArgs e)
    {
      if (Form1.singleton.outputImage == null)
        return;

      RenderRaysMap ( AdvancedTools.AllRaysMap, AllRaysMapPictureBox, SaveAllRaysMapButton );

      SetTotalAndAverageAllRaysCount ( Intersection.countRays, AllRaysMapPictureBox.Image.Width, AllRaysMapPictureBox.Image.Height );
    }

    private void RenderRaysMap ( AdvancedTools.RaysMap raysMap, PictureBox pictureBox, Button saveButton )
    {
      if (Form1.singleton.outputImage == null)
        return;

      raysMap.RenderRaysMap();

      pictureBox.Image = raysMap.GetBitmap();

      saveButton.Enabled = true;
    }

    public void SetNewDimensions ( int formImageWidth, int formImageHeight )
    {
      PrimaryRaysMapPictureBox.Width  = formImageWidth;
      PrimaryRaysMapPictureBox.Height = formImageHeight;

      AllRaysMapPictureBox.Width  = formImageWidth;
      AllRaysMapPictureBox.Height = formImageHeight;

      DepthMapPictureBox.Width  = formImageWidth;
      DepthMapPictureBox.Height = formImageHeight;

      AdvancedTools.SetNewDimensions ();
    }

    private void DepthMapPictureBox_MouseDownAndMouseMove ( object sender, MouseEventArgs e )
    {
      if ( ( (PictureBox) sender ).Image != null && e.Button == MouseButtons.Left && DepthMapPictureBox.ClientRectangle.Contains(e.Location))
      {
        Point coordinates = e.Location;

        double depth = AdvancedTools.DepthMap.GetDepthAtLocation ( coordinates.X, coordinates.Y );

        DepthMap_Coordinates.Text = String.Format ( "X: {0}\r\nY: {1}\r\nDepth:\r\n{2:0.00}",
                                                    coordinates.X,
                                                    coordinates.Y,
                                                    depth );
      }
    }

    private void PrimaryRaysMapPictureBox_MouseDownAndMouseMove ( object sender, MouseEventArgs e )
    {
      if (((PictureBox)sender).Image != null && e.Button == MouseButtons.Left && PrimaryRaysMapPictureBox.ClientRectangle.Contains(e.Location))
      {
        RaysMapPictureBox_MouseDownAndMouseMove ( AdvancedTools.PrimaryRaysMap, PrimaryRaysMapCoordinates, e.Location );
      }
    }

    private void AllRaysMapPictureBox_MouseDownAndMouseMove(object sender, MouseEventArgs e)
    {
      if (((PictureBox)sender).Image != null && e.Button == MouseButtons.Left && AllRaysMapPictureBox.ClientRectangle.Contains(e.Location))
      {
        RaysMapPictureBox_MouseDownAndMouseMove(AdvancedTools.AllRaysMap, AllRaysMapCoordinates, e.Location);
      }
    }

    private void RaysMapPictureBox_MouseDownAndMouseMove ( AdvancedTools.RaysMap raysMap, Label label, Point coordinates )
    {
      int raysCount = raysMap.GetRaysCountAtLocation(coordinates.X, coordinates.Y);

      label.Text = String.Format("X: {0}\r\nY: {1}\r\nRays count:\r\n{2}",
                                 coordinates.X,
                                 coordinates.Y,
                                 raysCount);
    }

    public void SetTotalAndAveragePrimaryRaysCount ( long totalCount, int width, int height )
    {
      TotalPrimaryRaysCount.Text = String.Format ( "Total rays\r\ncount:\r\n{0}", totalCount );  //TODO: All rays, not only primary

      AveragePrimaryRaysCount.Text = String.Format ( "Average rays\r\ncount\r\nper pixel:\r\n{0}", totalCount / ( width * height ) );
    }

    public void SetTotalAndAverageAllRaysCount(long totalCount, int width, int height)
    {
      TotalAllRaysCount.Text = String.Format ( "Total all\r\nrays count:\r\n{0}", totalCount );

      AverageAllRaysCount.Text = String.Format ( "Average all\r\nray count\r\nper pixel:\r\n{0}", totalCount / ( width * height ) );
    }
  }
}