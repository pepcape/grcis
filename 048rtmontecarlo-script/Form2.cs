using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
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

      if (AdvancedTools.instance == null)
      {
        AdvancedTools.instance = new AdvancedTools();
      }      

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

    private void SaveMapButton_Click ( object sender, EventArgs e )
    {
      Panel panel = (sender as Button).Parent as Panel;

      string mapName = panel.Tag.ToString ();

      PictureBox pictureBox = panel.Controls.Find(mapName + "PictureBox", true).FirstOrDefault() as PictureBox;

      outputImage = (Bitmap) pictureBox.Image;

      if (outputImage == null)
        return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title        = "Save PNG file";
      sfd.Filter       = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName     = mapName + ".png";
      if (sfd.ShowDialog() != DialogResult.OK)
        return;

      outputImage.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
    }

    private void RenderMapButton_Click ( object sender, EventArgs e )
    {
      if ( Form1.singleton.outputImage == null )
        return;

      string fieldName = ( sender as Button ).Tag.ToString ();

      var map = AdvancedTools.instance.GetType ().GetField ( fieldName ).GetValue ( AdvancedTools.instance );

      (map as IMap).RenderMap ();

      Panel panel = ( sender as Button ).Parent as Panel;

      string fieldNameCamelCase = Char.ToUpper ( fieldName[ 0 ] ) + fieldName.Substring ( 1 );

      PictureBox pictureBox = panel.Controls.Find (fieldNameCamelCase + "PictureBox", true).FirstOrDefault() as PictureBox;
      pictureBox.Image = (map as IMap).GetBitmap();

      Button saveButton = panel.Controls.Find("Save" + fieldNameCamelCase + "Button", true).FirstOrDefault() as Button;    
      saveButton.Enabled = true;
    }

    public void SetNewDimensions ( int formImageWidth, int formImageHeight )
    {
      PrimaryRaysMapPictureBox.Width =
      AllRaysMapPictureBox.Width =
      DepthMapPictureBox.Width =
      NormalMapPictureBox.Width = formImageWidth;

      PrimaryRaysMapPictureBox.Height =
      AllRaysMapPictureBox.Height =
      DepthMapPictureBox.Height =
      NormalMapPictureBox.Height = formImageHeight;

      AdvancedTools.instance.SetNewDimensions ();
    }

    private void DepthMapPictureBox_MouseDownAndMouseMove ( object sender, MouseEventArgs e )
    {
      if ( ( (PictureBox) sender ).Image != null && e.Button == MouseButtons.Left && ((PictureBox)sender).ClientRectangle.Contains(e.Location))
      {
        Point coordinates = e.Location;

        double depth = AdvancedTools.instance.depthMap.GetValueAtCoordinates ( coordinates.X, coordinates.Y );

        DepthMap_Coordinates.Text = String.Format ( "X: {0}\r\nY: {1}\r\nDepth:\r\n{2:0.00}",
                                                    coordinates.X,
                                                    coordinates.Y,
                                                    depth );
      }
    }

    private void PrimaryRaysMapPictureBox_MouseDownAndMouseMove ( object sender, MouseEventArgs e )
    {
      if (((PictureBox)sender).Image != null && e.Button == MouseButtons.Left && ((PictureBox)sender).ClientRectangle.Contains(e.Location))
      {
        RaysMapPictureBox_MouseDownAndMouseMove ( AdvancedTools.instance.primaryRaysMap, PrimaryRaysMapCoordinates, e.Location );
      }
    }

    private void AllRaysMapPictureBox_MouseDownAndMouseMove(object sender, MouseEventArgs e)
    {
      if (((PictureBox)sender).Image != null && e.Button == MouseButtons.Left && ((PictureBox)sender).ClientRectangle.Contains(e.Location))
      {
        RaysMapPictureBox_MouseDownAndMouseMove(AdvancedTools.instance.allRaysMap, AllRaysMapCoordinates, e.Location);
      }
    }

    private void NormalMapPictureBox_MouseDownAndMouseMove(object sender, MouseEventArgs e)
    {
      if (((PictureBox)sender).Image != null && e.Button == MouseButtons.Left && ((PictureBox)sender).ClientRectangle.Contains(e.Location))
      {
        Point coordinates = e.Location;

        double angle = AdvancedTools.instance.normalMap.GetValueAtCoordinates ( coordinates.X, coordinates.Y );

        char degreesChar = '°';
        if ( double.IsInfinity(angle) || double.IsNaN(angle) )
        {
          degreesChar = '\0';
        }

        NormalMapCoordinates.Text = String.Format ( "X: {0}\r\nY: {1}\r\n\r\nAngle of\r\nnormal vector:\r\n{2:0.00}{3}",
                                                    coordinates.X,
                                                    coordinates.Y,
                                                    angle,
                                                    degreesChar);
      }
    }

    private void RaysMapPictureBox_MouseDownAndMouseMove ( AdvancedTools.RaysMap raysMap, Label label, Point coordinates )
    {
      int raysCount = raysMap.GetValueAtCoordinates(coordinates.X, coordinates.Y);

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

    // Here should be all render buttons
    public void RenderButtonsEnabled ( bool newStatus )
    {
      RenderDepthMapButton.Enabled =
      RenderAllRaysMapButtona.Enabled =
      RenderPrimaryRaysMapButton.Enabled = 
      RenderNormalMapButton.Enabled = newStatus;
    }       
  }
}