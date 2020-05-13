using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using Utilities;

namespace Rendering
{
  public class AdditionalViews
  {
    public static AdditionalViews singleton;

    private List<IMap> allMaps;

    public RaysMap   primaryRaysMap;
    public RaysMap   allRaysMap;
    public DepthMap  depthMap;
    public NormalMap normalMapRelative;
    public NormalMap normalMapAbsolute;

    public bool mapsEmpty = true;
    public PointCloud pointCloud;

	  private readonly CheckBox collectDataCheckBox;
	  private readonly Action<string, string, int> notification;

    public AdditionalViewsForm form;

	  public AdditionalViews ( CheckBox collectDataCheckBox , Action<string, string, int> notification )
	  {
      this.collectDataCheckBox = collectDataCheckBox;
      this.notification = notification;

      singleton = this;
    }

    internal void Initialize ()
    {
      primaryRaysMap    = new RaysMap ();
      allRaysMap        = new RaysMap ();
      depthMap          = new DepthMap ();
      normalMapRelative = new NormalMap ( relative: true );
      normalMapAbsolute = new NormalMap ( relative: false );

      normalMapAbsolute.mapArray             = normalMapRelative.mapArray;
      normalMapAbsolute.intersectionMapArray = normalMapRelative.intersectionMapArray;

      allMaps = new List<IMap> ();

      foreach ( FieldInfo fieldInfo in typeof ( AdditionalViews ).GetFields () )
      {
        if ( typeof ( IMap ).IsAssignableFrom ( fieldInfo.FieldType ) )
          allMaps.Add ( (IMap) fieldInfo.GetValue ( singleton ) );
      }
    }

    /// <summary>
    /// Registers one intersection of ray
    /// </summary>
    /// <param name="level">To differentiate between primary and all rays</param>
    /// <param name="rayOrigin">Origin of ray / Centre of camera</param>
    /// <param name="firstIntersection">First element of array of Intersections</param>
    public void Register ( int level, Vector3d rayOrigin, Intersection firstIntersection )
    {
      if ( !collectDataCheckBox.Checked )
        return;

      // Initial check for null references
      if ( primaryRaysMap == null || allRaysMap == null || depthMap == null || normalMapRelative == null )
        Initialize ();

      if ( primaryRaysMap.mapArray == null )
        primaryRaysMap.Initialize ();

      if ( allRaysMap.mapArray == null )
        allRaysMap.Initialize ();

      if ( depthMap.mapArray == null )
        depthMap.Initialize ();

      if ( normalMapRelative.mapArray == null || 
           normalMapRelative.intersectionMapArray == null || 
           normalMapAbsolute.mapArray == null )
      {
        normalMapRelative.Initialize ();
        normalMapRelative.rayOrigin = rayOrigin;

        normalMapAbsolute.Initialize ();
        normalMapAbsolute.rayOrigin = rayOrigin;
      }


      double depth = Vector3d.Distance ( rayOrigin, firstIntersection.CoordWorld );

      // actual registering - increasing/writing to desired arrays
      if ( level == 0 )
      {
        // register depth
        depthMap.mapArray [ MT.x, MT.y ] += depth;

        // register primary rays (those with level 0)
        primaryRaysMap.mapArray [ MT.x, MT.y ] += 1; // do not use ++ instead - causes problems with strong type T in Map<T>

        // register normal vector
        normalMapRelative.intersectionMapArray[MT.x, MT.y] += firstIntersection.CoordWorld;
        normalMapRelative.mapArray[MT.x, MT.y] += firstIntersection.Normal;
      }

      // register all rays
      allRaysMap.mapArray [ MT.x, MT.y ] += 1;

      mapsEmpty = false;
    }


    public class DepthMap: Map<double, double>
    {
      public override void RenderMap ()
      {
        if ( mapImageWidth == 0 || mapImageHeight == 0 )
          Initialize ();

        AverageMap ();

        SetReferenceMinAndMaxValues ();

        singleton.GetMinimumAndMaximum ( ref minValue, ref maxValue, mapArray );

        mapBitmap = new Bitmap ( mapImageWidth, mapImageHeight, PixelFormat.Format24bppRgb );

        PopulateArray2D<double> ( mapArray, maxValue, 0, true );

        minValue = double.MaxValue;
        singleton.GetMinimumAndMaximum ( ref minValue, ref maxValue, mapArray ); // New minimum after replacing all zeroes

        for ( int x = 0; x < mapImageWidth; x++ )
          for ( int y = 0; y < mapImageHeight; y++ )
            mapBitmap.SetPixel ( x, y, GetAppropriateColor ( x, y ) );
      }

      protected override Color GetAppropriateColor ( int x, int y )
      {
        return GetAppropriateColorBlackAndWhiteLogarithmic ( minValue, maxValue, mapArray [ x, y ] );
      }

      protected override void DivideArray ( int x, int y )
      {
        mapArray [ x, y ] /= singleton.primaryRaysMap.mapArray [ x, y ];
      }

      public override double GetValueAtCoordinates ( int x, int y )
      {
        return mapArray [ x, y ] >= maxValue ? double.PositiveInfinity : mapArray [ x, y ];
      }
    }

    /// <summary>
    /// Used for PrimaryRaysMap, AllRaysMap and potentially other maps based on rays count
    /// </summary>
    public class RaysMap: Map<int, int>
    {
      protected override Color GetAppropriateColor ( int x, int y )
      {
        return GetAppropriateColorLinear ( minValue, maxValue, mapArray [ x, y ] );
      }

      protected override void DivideArray ( int x, int y )
      {
        mapArray [ x, y ] /= singleton.primaryRaysMap.mapArray [ x, y ];
      }

      public override int GetValueAtCoordinates ( int x, int y )
      {
        if ( x < 0 || x >= mapImageWidth || y < 0 || y >= mapImageHeight )
          return -1;

        return mapArray [ x, y ];
      }
    }

    /// <summary>
    /// Used for Absolute- and Relative-NormaMap
    /// Shows normals at first intersection of primary rays with scene (averages in case of multiple primary rays per pixel)
    /// </summary>
    public class NormalMap: Map<Vector3d, double>
    {
      private delegate Color AppropriateColor ( Vector3d normalVector, Vector3d intersectionVector );

      private readonly AppropriateColor appropriateColor;

      internal Vector3d[,] intersectionMapArray;

      public Vector3d rayOrigin;

      public NormalMap ( bool relative = true )
      {
        if ( relative )
          appropriateColor = GetAppropriateColorRelative;
        else
          appropriateColor = GetAppropriateColorAbsolute;

        mapArray             = new Vector3d[mapImageWidth, mapImageHeight];
        intersectionMapArray = new Vector3d[mapImageWidth, mapImageHeight];
      }

      public new void Initialize ( int formImageWidth = 0, int formImageHeight = 0 )
      {
        base.Initialize ( formImageWidth, formImageHeight );

        if ( formImageWidth != 0 )
        {
          mapImageWidth  = formImageWidth;
          mapImageHeight = formImageHeight;
        }

        mapArray             = new Vector3d[mapImageWidth, mapImageHeight];
        intersectionMapArray = new Vector3d[mapImageWidth, mapImageHeight];

        singleton.normalMapAbsolute.mapArray             = singleton.normalMapRelative.mapArray;
        singleton.normalMapAbsolute.intersectionMapArray = singleton.normalMapRelative.intersectionMapArray;
      }

      public override void RenderMap ()
      {
        if ( mapImageWidth == 0 || mapImageHeight == 0 )
          Initialize ();

        if ( !wasAveraged )
        {
          AverageMap ();

          singleton.normalMapAbsolute.wasAveraged = true;
          singleton.normalMapRelative.wasAveraged = true;
        }

        base.RenderMap ();
      }

      protected new void SetReferenceMinAndMaxValues () { } // intentionally left blank for speed-up

      protected override void SetMinimumAndMaximum () { } // intentionally left blank for speed-up

      protected override Color GetAppropriateColor ( int x, int y )
      {
        return appropriateColor ( mapArray [ x, y ], intersectionMapArray [ x, y ] );
      }

      protected override void DivideArray ( int x, int y )
      {
        intersectionMapArray [ x, y ] /= singleton.primaryRaysMap.mapArray [ x, y ];
        mapArray [ x, y ]             /= singleton.primaryRaysMap.mapArray [ x, y ];
      }

      public override double GetValueAtCoordinates ( int x, int y )
      {
        return Vector3d.CalculateAngle ( mapArray [ x, y ], rayOrigin - intersectionMapArray [ x, y ] ) * 180 / Math.PI;
      }

      private const double COLOR_MUL_HALF = 127.499;

      private Color GetAppropriateColorRelative ( Vector3d normalVector, Vector3d intersectionVector )
      {
        Vector3d relativeNormalVector = rayOrigin - intersectionVector - normalVector;

        if ( normalVector == Vector3d.Zero )
          return Color.Black;

        if ( relativeNormalVector != Vector3d.Zero )
          relativeNormalVector.Normalize ();

        int red   =       (int)((relativeNormalVector.X + 1) * COLOR_MUL_HALF);
        int green =       (int)((relativeNormalVector.Y + 1) * COLOR_MUL_HALF);
        int blue  = 255 - (int)((relativeNormalVector.Z + 1) * COLOR_MUL_HALF);

        return Color.FromArgb ( red, green, blue );
      }

      private static Color GetAppropriateColorAbsolute ( Vector3d normalVector, Vector3d intersectionVector )
      {
        if ( normalVector == Vector3d.Zero )
          return Color.Black;

        normalVector.Normalize ();

        int red   =       (int)((normalVector.X + 1) * COLOR_MUL_HALF);
        int green =       (int)((normalVector.Y + 1) * COLOR_MUL_HALF);
        int blue  = 255 - (int)((normalVector.Z + 1) * COLOR_MUL_HALF);

        return Color.FromArgb ( red, green, blue );
      }

      private Color GetAppropriateColorSimple ( Vector3d normalVector, Vector3d intersectionVector )
      {
        double angle = Vector3d.CalculateAngle ( normalVector, rayOrigin - intersectionVector ) * 180 / Math.PI;

        double colorValue = angle / 90 * 240;

        if ( double.IsNaN ( colorValue ) )
          return Color.FromArgb ( 1, 1, 1, 1 );
        else
          return Arith.HSVToColor ( 240 - colorValue, 1, 1 );
      }

      public new void Reset ()
      {
        base.Reset ();

        intersectionMapArray = null;
      }
    }


    /// <summary>
    /// Base class for all maps
    /// </summary>
    /// <typeparam name="T">Type of value stored in map</typeparam>
    /// <typeparam name="U">Type of value returned when asked for value at specific location (usually same as T)</typeparam>
    public abstract class Map<T, U>: IMap
    {
      // Image width and heightin pixels, 0 for default value (according to panel size)
      // Based on dimensions of main image in Form1
      public int mapImageWidth;
      public int mapImageHeight;

      // For displaying in PictureBox (and therefore also saving as file)
      internal Bitmap mapBitmap;

      // Main array for counting
      // Inherited classes may contain other helper arrays of same size
      internal T[,] mapArray;

      // Used for getting appropriate color (based on linear/logarithmic color gradient)
      internal T maxValue;
      internal T minValue;

      private bool dirtyRendered;

      /// <summary>
      /// Sets mapImageWidth and mapImageHeight according to dimensions of main image in Form1
      /// Initializes mapArray and other
      /// </summary>
      public void Initialize ( int formImageWidth = 0, int formImageHeight = 0 )
      {
        if ( formImageWidth != 0 )
        {
          mapImageWidth  = formImageWidth;
          mapImageHeight = formImageHeight;
        }

        mapArray = new T[mapImageWidth, mapImageHeight];

        wasAveraged = false;
        dirtyRendered = false;
      }

      /// <summary>
      /// Main method for creating mapBitmap from mapArray and other arrays using GetAppropriateColor for each pixel of mapBitmap
      /// </summary>
      public virtual void RenderMap ()
      {
        if ( dirtyRendered )
          return;

        if ( mapImageWidth == 0 || mapImageHeight == 0 )
          Initialize ();

        SetMinimumAndMaximum ();

        mapBitmap = new Bitmap ( mapImageWidth, mapImageHeight, PixelFormat.Format24bppRgb );

        for ( int x = 0; x < mapImageWidth; x++ )
          for ( int y = 0; y < mapImageHeight; y++ )
            mapBitmap.SetPixel ( x, y, GetAppropriateColor ( x, y ) );

        dirtyRendered = true;
      }

      /// <summary>
      /// Set MaxValue constant of T to local minValue variable and vice-versa
      /// Done through reflection using ReadStaticField method
      /// </summary>
      protected void SetReferenceMinAndMaxValues ()
      {
        minValue = ReadStaticField ( "MaxValue" );
        maxValue = ReadStaticField ( "MinValue" );
      }

      /// <summary>
      /// Returns value of static field with given name
      /// </summary>
      /// <param name="name">Name of static field</param>
      /// <returns></returns>
      private static T ReadStaticField ( string name )
      {
        FieldInfo field = typeof ( T ).GetField ( name, BindingFlags.Public | BindingFlags.Static );

        if ( field == null )
          throw new InvalidOperationException ( "Invalid type argument for NumericUpDown<T>: " + typeof ( T ).Name );

        return (T) field.GetValue ( null );
      }

      /// <summary>
      /// Used for later deciding for appropriate method to calculate color of specific pixel
      /// Usually used with linear or logarithmic color gradient from red to dark blue (no purple and red again) by changing value in HSV model
      /// </summary>
      /// <param name="x">X coordinate of current pixel</param>
      /// <param name="y">Y coordinate of current pixel</param>
      /// <returns></returns>
      protected abstract Color GetAppropriateColor ( int x, int y );

      // indication whether map is already averaged
      protected bool wasAveraged;

      /// <summary>
      /// Averages all elements in mapArray (standard arithmetic average)
      /// </summary>
      protected void AverageMap ()
      {
        if ( singleton.primaryRaysMap == null )
          singleton.Initialize ();

        if ( singleton.primaryRaysMap.mapArray == null )
          singleton.primaryRaysMap.Initialize ();

        if ( wasAveraged )
          return;

        for ( int i = 0; i < mapImageWidth; i++ )
          for ( int j = 0; j < mapImageHeight; j++ )
            if ( singleton.primaryRaysMap.mapArray [ i, j ] != 0 )
              DivideArray ( i, j ); // Separate method for division because of generic T

        wasAveraged = true;
      }

      /// <summary>
      /// Implementation of standard division operator (needed because of generic T)
      /// </summary>
      protected abstract void DivideArray ( int x, int y );

      /// <summary>
      /// Returns mapBitmap usually to PictureBox to display it in Form1
      /// </summary>
      /// <returns></returns>
      public Bitmap GetBitmap ()
      {
        if ( mapBitmap == null )
          RenderMap ();

        return mapBitmap;
      }

      /// <summary>
      /// Used by AdditionalViewsForm to display info for mouse down and mouse move while mouse down
      /// </summary>
      /// <param name="x">X coordinate of cursor relative to bitmap/mapArray</param>
      /// <param name="y">Y coordinate of cursor relative to bitmap/mapArray</param>
      /// <returns>Return U - in most maps it is same as T</returns>
      public abstract U GetValueAtCoordinates ( int x, int y );

      /// <summary>
      /// Sets minimal and maximal values found in mapArray
      /// Must be overriden if T is not IComparable
      /// </summary>
      /// <typeparam name="T">Type IComparable</typeparam>
      protected virtual void SetMinimumAndMaximum ()
      {
        SetReferenceMinAndMaxValues ();

        for ( int x = 0; x < singleton.depthMap.mapImageWidth; x++ )
        {
          for ( int y = 0; y < singleton.depthMap.mapImageHeight; y++ )
          {
            if ( ( mapArray [ x, y ] as IComparable<T> ).CompareTo ( maxValue ) > 0 )
              maxValue = mapArray [ x, y ];
            if ( ( mapArray [ x, y ] as IComparable<T> ).CompareTo ( minValue ) < 0 )
              minValue = mapArray [ x, y ];
          }
        }
      }

      public void Reset ()
      {
        mapArray = null;
        dirtyRendered = false;
      }

      public void ExportData ( string mapName )
      {
        SaveFileDialog sfd = new SaveFileDialog
        {
          Title = @"Save CSV file",
          Filter = @"CSV Files|*.csv",
          AddExtension = true,
          FileName = mapName + ".csv"
        };

        if ( sfd.ShowDialog () != DialogResult.OK )
          return;

        Thread fileSaver = new Thread ( () => singleton.ExportMapData(mapArray, sfd.FileName) );
        fileSaver.Name = "FileSaver";

        fileSaver.Start ();
      }
    }


    void ExportMapData<T> ( T[,] mapArray, string fileName )
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture ( "en-GB" ); // needed for dot as decimal separator in float

      using ( StreamWriter streamWriter = new StreamWriter ( fileName ) )
      {
        for ( int i = 0; i < mapArray.GetLength( 0 ); i++ )
        {
          for ( int j = 0; j < mapArray.GetLength ( 1 ); j++ )
          {
            streamWriter.Write ("{0}, ", mapArray[i, j]);
          }
          streamWriter.WriteLine ();
        }

        streamWriter.Close ();
      }

	    notification ( @"File succesfully saved", $"CSV data file \"{fileName}\" succesfully saved.", 30000 );
    }

    /// <summary>
    /// Returns minimal and maximal values found in a specific map
    /// Values are returned via reference and must be of type IComparable
    /// </summary>
    /// <typeparam name="T">Type IComparable</typeparam>
    /// <param name="minValue">Must be initially set to max value of corresponding type</param>
    /// <param name="maxValue">Must be initially set to min value of corresponding type</param>
    /// <param name="map">2D array of values</param>
    private void GetMinimumAndMaximum<T> ( ref T minValue, ref T maxValue, T[,] map ) where T : IComparable
    {
      for ( int x = 0; x < depthMap.mapImageWidth; x++ )
      {
        for ( int y = 0; y < depthMap.mapImageHeight; y++ )
        {
          if ( map [ x, y ].CompareTo ( maxValue ) > 0 )
            maxValue = map [ x, y ];

          if ( map [ x, y ].CompareTo ( minValue ) < 0 )
            minValue = map [ x, y ];
        }
      }
    }

    /// <summary>
    /// Returns color based on range
    /// Returned color is either dark blue (close to minValue) or red (close to maxValue)
    /// Between that color is lineary transited, changing value in HSV model
    /// Dark blue -> light blue -> turquoise -> green -> yellow -> orange -> red (does not go to purple - reason for value 240 instead of 255)
    /// </summary>
    /// <param name="minValue">Start of range (dark blue color)</param>
    /// <param name="maxValue">End of range (red color)</param>
    /// <param name="newValue">Value for which we want color</param>
    /// <returns>Appropriate color</returns>
    private static Color GetAppropriateColorLinear ( double minValue, double maxValue, double newValue )
    {
      double colorValue = ( newValue - minValue ) / ( maxValue - minValue ) * 240;

      if ( double.IsNaN ( colorValue ) )  // this happens in case minValue and maxValue are same
        return Color.Red;
      else
        return Arith.HSVToColor ( 240 - colorValue, 1, 1 );
    }

    /// <summary>
    /// Returns color based on range
    /// Returned color is either white (close to minValue) or black (close to maxValue)
    /// Between that color is logarithmically transited, changing all RGB values to the same to get appropriate shade of grey (there is more than 50 of them)
    /// </summary>
    /// <param name="minValue">Start of range (white color)</param>
    /// <param name="maxValue">End of range (black color)</param>
    /// <param name="newValue">Value for which we want color</param>
    /// <returns>Appropriate color</returns>
    private static Color GetAppropriateColorBlackAndWhiteLogarithmic (
      double minValue,
      double maxValue,
      double newValue)
    {
      double colorValue = Math.Log(newValue - minValue + 1, maxValue - minValue + 1) * 255;
      int iColorValue = 255 - Util.Clamp((int)colorValue, 0, 255);

      return Color.FromArgb (iColorValue, iColorValue, iColorValue);
    }

    /// <summary>
    /// Returns color based on range
    /// Returned color is either red (close to minValue) or dark blue (close to maxValue)
    /// Between that color is logarithmically transited, changing value in HSV model
    /// Red -> orange -> yellow -> green -> turquoise -> light blue -> dark blue (does not go to purple - reason for value 240 instead of 255)
    /// </summary>
    /// <param name="minValue">Start of range (red color)</param>
    /// <param name="maxValue">End of range (dark blue color)</param>
    /// <param name="newValue">Value for which we want color</param>
    /// <returns>Appropriate color</returns>
    private static Color GetAppropriateColorLogarithmicReversed ( double minValue, double maxValue, double newValue )
    {
      var colorValue = Math.Log ( newValue - minValue + 1, maxValue - minValue + 1 ) * 240;

      return Arith.HSVToColor ( 240 - colorValue, 1, 1 );
    }

    /// <summary>
    /// Sets all values of 2D array to specific value
    /// </summary>
    /// <typeparam name="T">Type of array and value</typeparam>
    /// <param name="array">Array to populate</param>
    /// <param name="value">Desired value</param>
    /// <param name="selectedValue">Only these values are replaced if selected is true</param>
    /// <param name="selected">Switches between changing all values or only values equal to selectedValue</param>
    private static void PopulateArray2D<T> ( T[,] array, T value, T selectedValue, bool selected )
    {
      if ( selected )
      {
        for ( int i = 0; i < array.GetLength ( 0 ); i++ )
        {
          for ( int j = 0; j < array.GetLength ( 1 ); j++ )
          {
            if ( array [ i, j ].Equals ( selectedValue ) )
            {
              array [ i, j ] = value;
            }
          }
        }
      }
      else
      {
        for ( int i = 0; i < array.GetLength ( 0 ); i++ )
        {
          for ( int j = 0; j < array.GetLength ( 1 ); j++ )
          {
            array [ i, j ] = value;
          }
        }
      }
    }

    /// <summary>
    /// Calls Initialize method of all subclasses in AdditionalViews (from array allMaps)
    /// </summary>
    public void SetNewDimensions ( int formImageWidth, int formImageHeight )
    {
      if ( allMaps == null )
        Initialize ();

      foreach ( IMap map in allMaps )
      {
        if ( map == null )
        {
          Initialize ();
          break;
        }
      }

      foreach ( IMap map in allMaps )
        map.Initialize ( formImageWidth, formImageHeight );
    }

    /// <summary>
    /// Removes all maps and unnecessary stuff
    /// </summary>
    public void NewRenderInitialization ()
    {
      foreach ( IMap map in allMaps )
        map?.Reset ();
    }
  }
}

/// <summary>
/// Interface for all maps
/// </summary>
internal interface IMap
{
  void Initialize ( int formImageWidth = 0, int formImageHeight = 0 );

  void RenderMap ();

  Bitmap GetBitmap ();

  void Reset ();

  void ExportData ( string mapName );
}
