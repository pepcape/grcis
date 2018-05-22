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
    public static Form2 instance; //singleton

    /// <summary>
    /// Constructor which sets location of Form2 window and initializes AdvancedTools class
    /// </summary>
    public Form2 ()
    {
      instance = this;

      Form1.singleton.AdvancedToolsButton.Enabled = false;

      if (AdvancedTools.instance == null)
      {
        AdvancedTools.instance = new AdvancedTools();
      }      

      InitializeComponent ();

      this.StartPosition = FormStartPosition.Manual;

      // Sets location of Form2 (Advanced Tools) to either right or left of Form1 (Main) 
      // depending on position of Form1 (whether it is close to right edge of primary screen) //TODO: fix fullscreen/no more space on screen for Form2

      if (( Form1.singleton.Location.X - this.Width < 0 ) && 
          ( Form1.singleton.Location.X + Form1.singleton.Width + this.Width > SystemInformation.VirtualScreen.Width ) )
      {
        this.Location =
          new Point( Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2,
                     Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2); // place in the middle of screen if no space around Form1 is available
      }
      else if ( Form1.singleton.Location.X + Form1.singleton.Width + this.Width < Screen.PrimaryScreen.WorkingArea.Width ||
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

      RenderButtonsEnabled ( false );
    }

    private void Form2_FormClosed ( object sender, FormClosedEventArgs e )
    {
      Form1.singleton.AdvancedToolsButton.Enabled = true;

      instance = null;
    }

    /// <summary>
    /// Universal method for saving maps to .png format
    /// Image must be rendered (otherwise button is disabled)
    /// </summary>
    /// <param name="sender">Should be only SaveButton</param>
    /// <param name="e"></param>
    private void SaveMapButton_Click ( object sender, EventArgs e )
    {
      Panel panel = (sender as Button).Parent as Panel;

      string mapName = panel.Tag.ToString ();

      PictureBox pictureBox = panel.Controls.Find(mapName + "PictureBox", true).FirstOrDefault() as PictureBox;

      Bitmap outputImage = (Bitmap) pictureBox.Image;

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

    /// <summary>
    /// Universal method for calling render method of map class
    /// Correct map class is chosen via reflection - sender(render button) must have set Tag to name of class instance in AdvancedTools class
    /// </summary>
    /// <param name="sender">Should be only Render button</param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Sets new dimensions for all PictureBoxes
    /// Called after dimensions of main image in Form1 are changed
    /// </summary>
    /// <param name="formImageWidth"></param>
    /// <param name="formImageHeight"></param>
    public void SetNewDimensions ( int formImageWidth, int formImageHeight )
    {
      PrimaryRaysMapPictureBox.Width =
      AllRaysMapPictureBox.Width =
      DepthMapPictureBox.Width =
      NormalMapRelativePictureBox.Width = formImageWidth;

      PrimaryRaysMapPictureBox.Height =
      AllRaysMapPictureBox.Height =
      DepthMapPictureBox.Height =
      NormalMapRelativePictureBox.Height = formImageHeight;

      AdvancedTools.instance.SetNewDimensions (formImageWidth, formImageHeight); //makes all maps to initialize again
    }

    /// <summary>
    /// Displays depth in scene of selected pixel (clicked or hovered over while mouse down)
    /// </summary>
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

    /// <summary>
    /// Displays number of primary rays sent to selected pixel (clicked or hovered over while mouse down)
    /// </summary>
    private void PrimaryRaysMapPictureBox_MouseDownAndMouseMove ( object sender, MouseEventArgs e )
    {
      if (((PictureBox)sender).Image != null && e.Button == MouseButtons.Left && ((PictureBox)sender).ClientRectangle.Contains(e.Location))
      {
        RaysMapPictureBox_MouseDownAndMouseMove ( AdvancedTools.instance.primaryRaysMap, PrimaryRaysMapCoordinates, e.Location );
      }
    }

    /// <summary>
    /// Displays number of all rays sent to selected pixel (clicked or hovered over while mouse down)
    /// </summary>
    private void AllRaysMapPictureBox_MouseDownAndMouseMove(object sender, MouseEventArgs e)
    {
      if (((PictureBox)sender).Image != null && e.Button == MouseButtons.Left && ((PictureBox)sender).ClientRectangle.Contains(e.Location))
      {
        RaysMapPictureBox_MouseDownAndMouseMove(AdvancedTools.instance.allRaysMap, AllRaysMapCoordinates, e.Location);
      }
    }

    /// <summary>
    /// Displays angle of rayOrigin-intersection and normal vector in place of intersection in selected pixel (clicked or hovered over while mouse down)
    /// Intersections as well as normal vectors are averaged through all such vectors in selected pixel
    /// </summary>
    private void NormalMapPictureBox_MouseDownAndMouseMove(object sender, MouseEventArgs e)
    {
      if (((PictureBox)sender).Image != null && e.Button == MouseButtons.Left && ((PictureBox)sender).ClientRectangle.Contains(e.Location))
      {
        Point coordinates = e.Location;

        double angle = AdvancedTools.instance.normalMapRelative.GetValueAtCoordinates ( coordinates.X, coordinates.Y );

        char degreesChar = '°';
        if ( double.IsInfinity(angle) || double.IsNaN(angle) )
        {
          degreesChar = '\0';
        }

        NormalMapRelativeCoordinates.Text =
        NormalMapAbsoluteCoordinates.Text =
          String.Format ( "X: {0}\r\nY: {1}\r\n\r\nAngle of\r\nnormal vector:\r\n{2:0.00}{3}",
                                                    coordinates.X,
                                                    coordinates.Y,
                                                    angle,
                                                    degreesChar);
      }
    }

    /// <summary>
    /// Called only from methods which choose raysMap (All/Primary rays)
    /// </summary>
    /// <param name="raysMap">AllRaysMap or PrimaryRaysMap</param>
    /// <param name="label">UI element where to write info</param>
    /// <param name="coordinates">Point where mouse was clicked / hovered over while held down</param>
    private void RaysMapPictureBox_MouseDownAndMouseMove ( AdvancedTools.RaysMap raysMap, Label label, Point coordinates )
    //TODO: refactor to get rid of PrimaryRaysMapPictureBox_MouseDownAndMouseMove and AllRaysMapPictureBox_MouseDownAndMouseMove
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

    public void SetTotalAndAverageAllRaysCount( long totalCount, int width, int height )
    {
      TotalAllRaysCount.Text = String.Format ( "Total all\r\nrays count:\r\n{0}", totalCount );

      AverageAllRaysCount.Text = String.Format ( "Average all\r\nray count\r\nper pixel:\r\n{0}", totalCount / ( width * height ) );
    }

    /// <summary>
    /// Goes through all controls of root (initially Form2) and sets new status 
    /// for Enabled property for all controls/buttons which has "Render" and "Button" in their name
    /// </summary>
    /// <param name="newStatus">Enabled/Disabled buttons</param>
    /// <param name="root">Used for recursion</param>
    public void RenderButtonsEnabled ( bool newStatus, Control root = null)
    {
      if ( root == null )
      {
        root = this;
      }

      foreach ( Control control in root.Controls)
      {
        if ( control.Name.Contains(@"Render") && control.Name.Contains(@"Button"))
        {
          control.Enabled = newStatus;
        }

        if ( control.Controls.Count != 0 )
        {
          RenderButtonsEnabled ( newStatus, control );
        }
      }
    }
  }
}