using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Rendering
{
  public partial class AdvancedToolsForm: Form
  {
    private readonly PanAndZoomSupport[] PanAndZoomControls;

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

      if ( AdvancedTools.singleton.mapsEmpty || MT.renderingInProgress )
      {
        RenderButtonsEnabled ( false );
        ExportDataButtonsEnabled ( false );
      }        
      else
      {
        RenderButtonsEnabled ( true );
        ExportDataButtonsEnabled ( true );
      }

      PanAndZoomControls = new PanAndZoomSupport[MapsTabControl.TabCount];

      for ( int i = 0; i < MapsTabControl.TabCount; i++ )
      {
        Func<Control, bool> condition = c => ( c.Name.Contains ( "PictureBox" ) && c.GetType () == typeof ( PictureBox ) );
        PictureBox pictureBox = FindAllControls ( condition , MapsTabControl.TabPages [i] )[0] as PictureBox;
        pictureBox.MouseWheel += new MouseEventHandler ( pictureBox_MouseWheel );
        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

        PanAndZoomControls [i] = new PanAndZoomSupport ( pictureBox, null, SetWindowTitleSuffix );
      }

      KeyPreview = true;
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
      string mapName = MapsTabControl.SelectedTab.Tag.ToString ();

      Bitmap outputImage = (Bitmap) PanAndZoomControls [MapsTabControl.SelectedIndex].image;

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
      Form1.singleton?.Notification ( @"File succesfully saved", $"Image file \"{sfd.FileName}\" succesfully saved.", 30000 );
    }
   
    /// <summary>
    /// Universal method for calling render method of map class
    /// Correct map class is chosen via reflection - panel (parent of sender) must have Tag set to name of class instance in AdvancedTools class
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RenderMapButton_Click ( object sender, EventArgs e )
    {
      if ( Form1.singleton.outputImage == null || MT.renderingInProgress )
        return;

      TabPage tabPage = MapsTabControl.SelectedTab;

      string mapName = tabPage.Tag.ToString ();

      object map = AdvancedTools.singleton.GetType ().GetField ( char.ToLower ( mapName [ 0 ] ) + mapName.Substring ( 1 ) ).GetValue ( AdvancedTools.singleton );

      ( map as IMap ).RenderMap ();

      PanAndZoomControls [MapsTabControl.SelectedIndex].SetNewImage ( ( map as IMap ).GetBitmap () );

      SaveButtonsEnabled ( true, tabPage );
      ResetButtonsEnabled ( true, tabPage );

      SetTotalAndAveragePrimaryRaysCount ();
      SetTotalAndAverageAllRaysCount ();
    }

    /// <summary>
    /// Displays depth in scene of selected pixel (clicked or hovered over while mouse down)
    /// </summary>
    private void DepthMapPictureBox_MouseDown ( object sender, MouseEventArgs e )
    {    
      CommonMouseDown ( e, DepthMapDisplayStats );
    }

    private void DepthMapPictureBox_MouseMove ( object sender, MouseEventArgs e )
    {
      CommonMouseMove ( e, DepthMapDisplayStats );
    }

    /// <summary>
    /// Displays angle of rayOrigin-intersection and normal vector in place of intersection in selected pixel (clicked or hovered over while mouse down)
    /// Intersections as well as normal vectors are averaged through all such vectors in selected pixel
    /// </summary>
    private void NormalMapPictureBox_MouseDown ( object sender, MouseEventArgs e )
    {
      CommonMouseDown ( e, NormalMapDisplayStats );
    }

    private void NormalMapPictureBox_MouseMove ( object sender, MouseEventArgs e )
    {
      CommonMouseMove ( e, NormalMapDisplayStats );
    }

    /// <summary>
    /// Displays number of rays of specific RaysMap (PrimaryRaysMap, AllRaysMap, ...) sent to selected pixel (clicked or moved over while mouse down)
    /// </summary>
    private void RaysMapPictureBox_MouseDown ( object sender, MouseEventArgs e )
    {
      CommonMouseDown ( e, RaysMapDisplayStats );
    }

    private void RaysMapPictureBox_MouseMove ( object sender, MouseEventArgs e )
    {
      CommonMouseMove ( e, RaysMapDisplayStats );
    }


    private void DepthMapDisplayStats ( int X, int Y )
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture ( "en-GB" ); // needed for dot as decimal separator in float

      double depth = AdvancedTools.singleton.depthMap.GetValueAtCoordinates ( X, Y );

      DepthMap_Coordinates.Text = $"X: {X}\r\nY: {Y}\r\nDepth: {depth:0.00}";
    }


    private void NormalMapDisplayStats ( int X, int Y )
    {
      double angle = AdvancedTools.singleton.normalMapRelative.GetValueAtCoordinates ( X, Y );

      char degreesChar = '°';
      if ( double.IsInfinity ( angle ) || double.IsNaN ( angle ) )
        degreesChar = '\0';

      NormalMapRelativeCoordinates.Text =
        NormalMapAbsoluteCoordinates.Text =
          $"X: {X}\r\nY: {Y}\r\nAngle of normal vector: {angle:0.00}{degreesChar}";
    }


    private void RaysMapDisplayStats ( int X, int Y )
    {
      TabPage tabPage = MapsTabControl.SelectedTab;

      string mapName = tabPage.Tag.ToString ();

      string fieldName = char.ToLower ( mapName [ 0 ] ) + mapName.Substring ( 1 );

      AdvancedTools.RaysMap raysMap = AdvancedTools.singleton.GetType ().GetField ( fieldName ).GetValue ( AdvancedTools.singleton ) as AdvancedTools.RaysMap;


      string fieldNameCamelCase = char.ToUpper ( fieldName [ 0 ] ) + fieldName.Substring ( 1 );

      Label label = tabPage.Controls.Find ( fieldNameCamelCase + "Coordinates", true ).FirstOrDefault () as Label;


      int raysCount = raysMap.GetValueAtCoordinates ( X, Y );

      label.Text = $"X: {X}\r\nY: {Y}\r\nRays count: {raysCount}";
    }

    private void CommonMouseDown ( MouseEventArgs e, Action<int, int> displayStats )
    {
      bool condition = PanAndZoomControls [MapsTabControl.SelectedIndex].image != null && e.Button == MouseButtons.Left;

      PanAndZoomControls[MapsTabControl.SelectedIndex].MouseDownRegistration ( e, displayStats, condition, ModifierKeys, out Cursor cursor );

      if ( cursor != null )
        Cursor = cursor;

      /*
      if ( PanAndZoomControls[MapsTabControl.SelectedIndex].image != null && e.Button == MouseButtons.Left )
      {
        PointF relative = PanAndZoomControls[MapsTabControl.SelectedIndex].GetRelativeCursorLocation ( e.X, e.Y );

        if ( !float.IsNaN( relative.X ) )
          displayStats ( (int) relative.X, (int) relative.Y );
      }

      if ( !ModifierKeys.HasFlag ( Keys.Control ) && e.Button == MouseButtons.Left && !mousePressed ) //holding down CTRL key prevents panning
      {
        mousePressed = true;
        PanAndZoomControls[MapsTabControl.SelectedIndex].MouseDown ( e.Location );
      }
      else
      {
        Cursor = Cursors.Cross;
      }*/
    }


    private void CommonMouseMove ( MouseEventArgs e, Action<int, int> displayStats )
    {
      bool condition = PanAndZoomControls [MapsTabControl.SelectedIndex].image != null && e.Button == MouseButtons.Left;

      PanAndZoomControls[MapsTabControl.SelectedIndex].MouseMoveRegistration ( e, displayStats, condition, ModifierKeys, out Cursor cursor );

      if ( cursor != null )
        Cursor = cursor;

      /*if ( PanAndZoomControls[MapsTabControl.SelectedIndex].image != null && e.Button == MouseButtons.Left )
      {
        PointF relative = PanAndZoomControls[MapsTabControl.SelectedIndex].GetRelativeCursorLocation ( e.X, e.Y );

        if ( !float.IsNaN ( relative.X ) && !mousePressed )
          displayStats ( (int) relative.X, (int) relative.Y );
      }

      if ( mousePressed && e.Button == MouseButtons.Left )
      {
        Cursor = Cursors.NoMove2D;
        PanAndZoomControls[MapsTabControl.SelectedIndex].MouseMove ( e.Location );
      }*/
    }

    private void CommonMouseUp ( object sender, MouseEventArgs e )
    {
      PanAndZoomControls [MapsTabControl.SelectedIndex].MouseUpRegistration ( out Cursor cursor );

      Cursor = cursor;
    }

    private void pictureBox_MouseWheel ( object sender, MouseEventArgs e )
    {
      PanAndZoomControls [MapsTabControl.SelectedIndex].MouseWheelRegistration ( e, ModifierKeys );
    }

    public void SetTotalAndAveragePrimaryRaysCount ()
    {
      TotalPrimaryRaysCount.Text = $"Total primary rays count: {Statistics.primaryRaysCount:n0}";

      AveragePrimaryRaysCount.Text =
        $"Average primary rays count per pixel: {Statistics.primaryRaysCount / ( PrimaryRaysMapPictureBox.Width * PrimaryRaysMapPictureBox.Height ):n0}";
    }

    public void SetTotalAndAverageAllRaysCount ()
    {
      TotalAllRaysCount.Text = $"Total rays count: {Statistics.allRaysCount:n0}";

      AverageAllRaysCount.Text = $"Average rays count per pixel: {Statistics.allRaysCount / ( AllRaysMapPictureBox.Width * AllRaysMapPictureBox.Height ):n0}";
    }

    /// <summary>
    /// Sets "enable" properties of all buttons in children of root (initially Form) recursively with text Render in their name to new status
    /// </summary>
    /// <param name="newStatus">Enabled/Disabled buttons</param>
    /// /// <param name="root">Root control where to start looking for button (initially Form [this])</param>
    public void RenderButtonsEnabled ( bool newStatus, Control root = null )
    {
      ButtonsEnabled ( newStatus, "Render", root );
    }

    /// <summary>
    /// Sets "enable" properties of all buttons in children of root (initially Form) recursively with text Save in their name to new status
    /// </summary>
    /// <param name="newStatus">Enabled/Disabled buttons</param>
    /// /// <param name="root">Root control where to start looking for button (initially Form [this])</param>
    public void SaveButtonsEnabled ( bool newStatus, Control root = null )
    {
      ButtonsEnabled ( newStatus, "Save", root );
    }

    /// <summary>
    /// Sets "enable" properties of all buttons in children of root (initially Form) recursively with text ExportData in their name to new status
    /// </summary>
    /// <param name="newStatus">Enabled/Disabled buttons</param>
    /// /// <param name="root">Root control where to start looking for button (initially Form [this])</param>
    public void ExportDataButtonsEnabled ( bool newStatus, Control root = null )
    {     
      ButtonsEnabled ( newStatus, "ExportData", root );
    }

    /// <summary>
    /// Sets "enable" properties of all buttons in children of root (initially Form) recursively with text Reset in their name to new status
    /// </summary>
    /// <param name="newStatus">Enabled/Disabled buttons</param>
    /// /// <param name="root">Root control where to start looking for button (initially Form [this])</param>
    public void ResetButtonsEnabled ( bool newStatus, Control root = null )
    {
      ButtonsEnabled ( newStatus, "Reset", root );
    }

    /// <summary>
    /// Goes through all controls of root and sets new status 
    /// for Enabled property for all controls/buttons which has buttonName and "Button" in their name
    /// </summary>
    /// <param name="newStatus">Enabled/Disabled buttons</param>
    /// <param name="buttonName">Name of button to look for in Name property</param>
    /// <param name="root">Used for recursion</param>
    private void ButtonsEnabled ( bool newStatus, string buttonName, Control root = null )
    {
      if ( root == null )
        root = this;

      List<Control> buttons = FindAllControls ( c => ( c.Name.Contains ( buttonName ) && c.GetType() == typeof ( Button ) ), root );

      foreach ( Control button in buttons )
        ( button as Button ).Enabled = newStatus;  
    }

    private List<Control> FindAllControls ( Func<Control, bool> condition, Control root = null )
    {
      List<Control> returnControls = new List<Control> ();

      if ( root == null )
        root = this;

      foreach ( Control control in root.Controls )
      {
        if ( condition ( control ) )
          returnControls.Add ( control );

        if ( control.Controls.Count != 0 )
          returnControls.AddRange ( FindAllControls ( condition, control ) );
      }

      return returnControls;
    }

    private void ExportDataButton_Click ( object sender, EventArgs e )
    {
      if ( Form1.singleton.outputImage == null || MT.renderingInProgress )
        return;

      TabPage tabPage = MapsTabControl.SelectedTab;

      string mapName = tabPage.Tag.ToString ();

      object map = AdvancedTools.singleton.GetType ().GetField ( char.ToLower ( mapName [ 0 ] ) + mapName.Substring ( 1 ) ).GetValue ( AdvancedTools.singleton );

      ( map as IMap ).ExportData ( mapName );
    }

    private readonly string winTitle = "Advanced Tools";

    /// <summary>
    /// Adds suffix to default text in Form>text property (title text in upper panel, between icon and minimize and close buttons)
    /// </summary>
    /// <param name="suffix"></param>
    void SetWindowTitleSuffix ( string suffix )
    {
      if ( string.IsNullOrEmpty ( suffix ) )
        Text = winTitle;
      else
        Text = winTitle + ' ' + suffix;
    }

    /// <summary>
    /// Called every time main picture box is needed to be re-painted
    /// Used for re-painting after request for zoom in/out or pan
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PictureBox_Paint ( object sender, PaintEventArgs e )
    {
      PanAndZoomControls [MapsTabControl.SelectedIndex].Paint ( e );
    }

    /// <summary>
    /// Catches +/PageUp for zoom in or -/PageDown for zoom out of image in picture box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AdvancedToolsForm_KeyDown ( object sender, KeyEventArgs e )
    {
      PanAndZoomControls [MapsTabControl.SelectedIndex].KeyDownRegistration ( e.KeyCode, ModifierKeys );
    }

    /// <summary>
    /// Resets image in picture box to 100% zoom and default position
    /// (left upper corner of image in left upper conrner of picture box)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResetZoomAndPanButton_Click ( object sender, EventArgs e )
    {
      PanAndZoomControls[MapsTabControl.SelectedIndex].Reset ();
    }
  }
}