using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Rendering
{
  public partial class AdvancedToolsForm: Form
  {
    public static AdvancedToolsForm instance; //singleton

    /// <summary>
    /// Constructor which sets location of Form1 window and initializes AdvancedTools class
    /// </summary>
    public AdvancedToolsForm ()
    {
      instance = this;

      InitializeComponent ();

      this.StartPosition = FormStartPosition.Manual;

      // Sets location of Form1 (Advanced Tools) to either right or left of Form1 (Main) 
      // depending on position of Form1 (whether it is close to right edge of primary screen)

      if ( ( Form1.singleton.Location.X - this.Width < 0 ) &&
           ( Form1.singleton.Location.X + Form1.singleton.Width + this.Width > SystemInformation.VirtualScreen.Width ) )
      {
        this.Location =
          new Point ( Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2,
                      Screen.PrimaryScreen.WorkingArea.Height / 2 -
                      this.Height / 2 ); // place in the middle of screen if no space around Form1 is available
      }
      else if ( Form1.singleton.Location.X + Form1.singleton.Width + this.Width <
                Screen.PrimaryScreen.WorkingArea.Width ||
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

      if ( AdvancedTools.singleton.mapsEmpty || MT.renderingInProgress)
        RenderButtonsEnabled ( false );      
      else
        RenderButtonsEnabled ( true );
    }

    private void AdvancedToolsForm_FormClosed ( object sender, FormClosedEventArgs e )
    {
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
      TabPage tabPage = MapsTabControl.SelectedTab;

      string mapName = tabPage.Tag.ToString ();

      PictureBox pictureBox = tabPage.Controls.Find ( mapName + "PictureBox", true ).FirstOrDefault () as PictureBox;

      Bitmap outputImage = (Bitmap) pictureBox.Image;

      if ( outputImage == null )
        return;

      SaveFileDialog sfd = new SaveFileDialog ();
      sfd.Title        = @"Save PNG file";
      sfd.Filter       = @"PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName     = mapName + ".png";
      if ( sfd.ShowDialog () != DialogResult.OK )
        return;

      outputImage.Save ( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    /// <summary>
    /// Universal method for calling render method of map class
    /// Correct map class is chosen via reflection - panel (parent of sender) must have Tag set to name of class instance in AdvancedTools class
    /// </summary>
    /// <param name="sender">Should be only Render button</param>
    /// <param name="e"></param>
    private void RenderMapButton_Click ( object sender, EventArgs e )
    {
      if ( Form1.singleton.outputImage == null )
        return;

      TabPage tabPage = MapsTabControl.SelectedTab;

      string mapName = tabPage.Tag.ToString ();

      object map = AdvancedTools.singleton.GetType ().GetField ( Char.ToLower ( mapName [ 0 ] ) + mapName.Substring ( 1 ) ).GetValue ( AdvancedTools.singleton );

      ( map as IMap ).RenderMap ();

      PictureBox pictureBox = tabPage.Controls.Find ( mapName + "PictureBox", true ).FirstOrDefault () as PictureBox;
      pictureBox.Image = ( map as IMap ).GetBitmap ();
      pictureBox.Width = newWidth;
      pictureBox.Height = newHeight;

      Button saveButton = tabPage.Controls.Find ( "Save" + mapName + "Button", true ).FirstOrDefault () as Button;
      saveButton.Enabled = true;

      SetTotalAndAveragePrimaryRaysCount ();
      SetTotalAndAverageAllRaysCount ();
    }

    private int newWidth;
    private int newHeight;
    /// <summary>
    /// Sets new dimensions for all PictureBoxes
    /// Called after dimensions of main image in Form1 are changed
    /// </summary>
    /// <param name="formImageWidth"></param>
    /// <param name="formImageHeight"></param>
    public void SetNewDimensions ( int formImageWidth, int formImageHeight )
    {
      newWidth = formImageWidth;
      newHeight = formImageHeight;

      AdvancedTools.singleton.SetNewDimensions ( formImageWidth, formImageHeight ); //makes all maps to initialize again
    }

    /// <summary>
    /// Displays depth in scene of selected pixel (clicked or hovered over while mouse down)
    /// </summary>
    private void DepthMapPictureBox_MouseDownAndMouseMove ( object sender, MouseEventArgs e )
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture ( "en-GB" ); // needed for dot as decimal separator in float

      if ( ( (PictureBox) sender ).Image != null && e.Button == MouseButtons.Left &&
           ( (PictureBox) sender ).ClientRectangle.Contains ( e.Location ) )
      {
        Point coordinates = e.Location;

        double depth = AdvancedTools.singleton.depthMap.GetValueAtCoordinates ( coordinates.X, coordinates.Y );

        DepthMap_Coordinates.Text = String.Format ( "X: {0}\r\nY: {1}\r\nDepth: {2:0.00}",
                                                    coordinates.X,
                                                    coordinates.Y,
                                                    depth );
      }
    }

    /// <summary>
    /// Displays angle of rayOrigin-intersection and normal vector in place of intersection in selected pixel (clicked or hovered over while mouse down)
    /// Intersections as well as normal vectors are averaged through all such vectors in selected pixel
    /// </summary>
    private void NormalMapPictureBox_MouseDownAndMouseMove ( object sender, MouseEventArgs e )
    {
      if ( ( (PictureBox) sender ).Image != null && e.Button == MouseButtons.Left &&
           ( (PictureBox) sender ).ClientRectangle.Contains ( e.Location ) )
      {
        Point coordinates = e.Location;

        double angle = AdvancedTools.singleton.normalMapRelative.GetValueAtCoordinates ( coordinates.X, coordinates.Y );

        char degreesChar = '°';
        if ( double.IsInfinity ( angle ) || double.IsNaN ( angle ) )
        {
          degreesChar = '\0';
        }

        NormalMapRelativeCoordinates.Text =
          NormalMapAbsoluteCoordinates.Text =
            String.Format ( "X: {0}\r\nY: {1}\r\nAngle of normal vector: {2:0.00}{3}",
                            coordinates.X,
                            coordinates.Y,
                            angle,
                            degreesChar );
      }
    }

    /// <summary>
    /// Displays number of rays of specific RaysMap (PrimaryRaysMap, AllRaysMap, ...) sent to selected pixel (clicked or moved over while mouse down)
    /// </summary>
    private void RaysMapPictureBox_MouseDownAndMouseMove ( object sender, MouseEventArgs e )
    {
      if ( ( (PictureBox) sender ).Image == null || e.Button != MouseButtons.Left ||
           !( ( (PictureBox) sender ).ClientRectangle.Contains ( e.Location ) ) )
      {
        return;
      }

      TabPage tabPage = MapsTabControl.SelectedTab;

      string mapName = tabPage.Tag.ToString ();

      string fieldName = Char.ToLower ( mapName [ 0 ] ) + mapName.Substring ( 1 );

      AdvancedTools.RaysMap raysMap = AdvancedTools.singleton.GetType ().GetField ( fieldName ).GetValue ( AdvancedTools.singleton ) as AdvancedTools.RaysMap;
     

      string fieldNameCamelCase = Char.ToUpper ( fieldName [ 0 ] ) + fieldName.Substring ( 1 );

      Label label = tabPage.Controls.Find ( fieldNameCamelCase + "Coordinates", true ).FirstOrDefault () as Label;


      int raysCount = raysMap.GetValueAtCoordinates ( e.X, e.Y );

      label.Text = String.Format ( "X: {0}\r\nY: {1}\r\nRays count: {2}",
                                   e.X,
                                   e.Y,
                                   raysCount );
    }

    public void SetTotalAndAveragePrimaryRaysCount ()
    {
      TotalPrimaryRaysCount.Text = String.Format ( "Total primary rays count: {0:n0}", Statistics.primaryRaysCount );

      AveragePrimaryRaysCount.Text = String.Format ( "Average primary rays count per pixel: {0:n0}",
                                                     Statistics.primaryRaysCount /
                                                     ( PrimaryRaysMapPictureBox.Width *
                                                       PrimaryRaysMapPictureBox.Height ) );
    }

    public void SetTotalAndAverageAllRaysCount ()
    {
      TotalAllRaysCount.Text = String.Format ( "Total rays count: {0:n0}", Statistics.allRaysCount );

      AverageAllRaysCount.Text = String.Format ( "Average rays count per pixel: {0:n0}",
                                                 Statistics.allRaysCount /
                                                 ( AllRaysMapPictureBox.Width * AllRaysMapPictureBox.Height ) );
    }

    /// <summary>
    /// Goes through all controls of root (initially Form1) and sets new status 
    /// for Enabled property for all controls/buttons which has "Render" and "Button" in their name
    /// </summary>
    /// <param name="newStatus">Enabled/Disabled buttons</param>
    /// <param name="root">Used for recursion</param>
    public void RenderButtonsEnabled ( bool newStatus, Control root = null )
    {
      if ( root == null )
        root = this;

      foreach ( Control control in root.Controls )
      {
        if ( control.Name.Contains ( @"Render" ) && control.Name.Contains ( @"Button" ) )
          control.Enabled = newStatus;

        if ( control.Controls.Count != 0 )
          RenderButtonsEnabled ( newStatus, control );
      }
    }
  }
}