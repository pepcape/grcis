using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Dynamic;
using MathSupport;
using OpenTK;
using Rendering;

namespace _048rtmontecarlo
{
  public static class AdvancedTools
  {
    public delegate void RenderMap ();

    internal static RaysMap primaryRaysMap;
    internal static RaysMap allRaysMap;
    internal static DepthMap depthMap;
    internal static NormalMap normalMap;

    private static void Initialize ()
    {
      primaryRaysMap = new RaysMap ();
      allRaysMap = new RaysMap ();
      depthMap = new DepthMap ();
      normalMap = new NormalMap ();
    }

    public static void Register ( int level, Vector3d rayOrigin, Intersection firstIntersection )
    {
      if ( Form2.singleton == null )
      {
        return;
      }

      if ( primaryRaysMap == null || allRaysMap == null || depthMap == null || normalMap == null)
        Initialize ();       

      if ( primaryRaysMap.mapArray == null )
        primaryRaysMap.Initialize ();

      if ( allRaysMap.mapArray == null )
        allRaysMap.Initialize();

      if ( depthMap.mapArray == null )
        depthMap.Initialize ();

      if ( normalMap.mapArray == null || normalMap.intersectionMapArray == null )
      {
        normalMap.Initialize();
        normalMap.rayOrigin = rayOrigin;
      }
        

      double depth;

      if ( firstIntersection == null )
      {
        depth = 10000; // TODO: CHANGE - placeholder for "infinity"
      }
      else
      {
        depth = Vector3d.Distance ( rayOrigin, firstIntersection.CoordWorld );
      }
     

      if ( level == 0 )
      {
        // register depth
        depthMap.mapArray[MT.x, MT.y] += depth;

        // register primary rays
        primaryRaysMap.mapArray[MT.x, MT.y] += 1;

        if ( firstIntersection != null )
        {
          // register normal vector
          normalMap.intersectionMapArray[ MT.x, MT.y ] += firstIntersection.CoordWorld;
          normalMap.mapArray[ MT.x, MT.y ] += firstIntersection.Normal;       
        }
      }

      // register all rays
      allRaysMap.mapArray[ MT.x, MT.y ] += 1;
    }


    public class DepthMap : Map<double>
    {
      public override void RenderMap()
      {
        if ( mapImageWidth == 0 || mapImageHeight == 0 )
        {
          Initialize ();
        }

        AverageMap ();

        SetReferenceMinAndMaxValues ();

        GetMinimumAndMaximum ( ref minValue, ref maxValue, mapArray );

        mapBitmap = new Bitmap ( mapImageWidth, mapImageHeight, PixelFormat.Format24bppRgb );

        PopulateArray2D<double> ( mapArray, maxValue, 0, true );

        minValue = double.MaxValue;
        GetMinimumAndMaximum ( ref minValue, ref maxValue, mapArray ); // TODO: New minimum after replacing all zeroes

        for ( int x = 0; x < mapImageWidth; x++ )
        {
          for ( int y = 0; y < mapImageHeight; y++ )
          {
            mapBitmap.SetPixel ( x, y, GetAppropriateColorLogarithmicReversed ( minValue, maxValue, mapArray[ x, y ] ) );
          }
        }
      }

      protected override void SetReferenceMinAndMaxValues ()
      {
        maxValue = double.MinValue;
        minValue = double.MaxValue;
      }

      protected override Color GetAppropriateColor ( int x, int y )
      {
        return GetAppropriateColorLogarithmicReversed ( minValue, maxValue, mapArray[x, y]);
      }

      protected override void DivideArray ( int x, int y )
      {
        mapArray[ x, y ] /= primaryRaysMap.mapArray[ x, y ];
      }

      public override dynamic GetValueAtCoordinates( int x, int y )
      {
        if ( mapArray[ x, y ] >= maxValue ) // TODO: PositiveInfinity in depthMap?
        {
          return double.PositiveInfinity;
        }
        else
        {
          return mapArray[ x, y ];
        }
      }
    }

    public class RaysMap : Map<int>
    {
      protected override void SetReferenceMinAndMaxValues ()
      {
        maxValue = int.MinValue;
        minValue = int.MaxValue;
      }

      protected override Color GetAppropriateColor ( int x, int y )
      {
        return GetAppropriateColorLinear ( minValue, maxValue, mapArray[ x, y ] );
      }

      protected override void DivideArray ( int x, int y )
      {
        mapArray[x, y] /= primaryRaysMap.mapArray[ x, y ];
      }

      public override dynamic GetValueAtCoordinates ( int x, int y )
      {
        if ( x < 0 || x >= mapImageWidth || y < 0 || y >= mapImageHeight )
        {
          return -1;
        }

        return mapArray[x, y];
      }
    }

    public class NormalMap : Map<Vector3d>
    {
      internal Vector3d[,] intersectionMapArray;

      public Vector3d rayOrigin;

      public new void Initialize ()
      {
        base.Initialize ();    

        intersectionMapArray = new Vector3d[mapImageWidth, mapImageHeight];
      }


      public override void RenderMap()
      {
        if (mapImageWidth == 0 || mapImageHeight == 0)
        {
          Initialize ();
        }

        AverageMap ();

        base.RenderMap ();
      }

      protected override void SetReferenceMinAndMaxValues () {}

      protected override void SetMinimumAndMaximum () {}

      protected override Color GetAppropriateColor (int x, int y)
      {
        return GetAppropriateColorForNormalVectorSimpleVersion ( mapArray[ x, y ], intersectionMapArray[ x, y ] );
      }

      protected override void DivideArray ( int x, int y )
      {
        intersectionMapArray[ x, y ] /= primaryRaysMap.mapArray[ x, y ];
      }

      public override dynamic GetValueAtCoordinates(int x, int y)
      {
        return Vector3d.CalculateAngle ( mapArray[ x, y ], rayOrigin - intersectionMapArray[ x, y ] ) * 180 / Math.PI;
      }

      private Color GetAppropriateColorForNormalVector ( Vector3d normalVector, Vector3d intersectionVector)
      {
        double angle = Vector3d.CalculateAngle(normalVector, rayOrigin - intersectionVector) * 180 / Math.PI;

        throw new NotImplementedException();
      }

      private Color GetAppropriateColorForNormalVectorSimpleVersion(Vector3d normalVector, Vector3d intersectionVector)
      {
        double angle = Vector3d.CalculateAngle ( normalVector, rayOrigin - intersectionVector ) * 180 / Math.PI;

        double colorValue = angle / 90 * 240;

        if ( double.IsNaN( colorValue ) )
        {
          return Color.FromArgb ( 1, 1, 1, 1 );
        }

        return Arith.HSVToColor ( 240 - colorValue, 1, 1 );
      }
    }


    public abstract class Map<T>: IMap<T>
    {
      /// <summary>
      /// Image width in pixels, 0 for default value (according to panel size).
      /// </summary>
      public int mapImageWidth;

      /// <summary>
      /// Image height in pixels, 0 for default value (according to panel size).
      /// </summary>
      public int mapImageHeight;

      internal Bitmap mapBitmap;

      internal T[,] mapArray;

      internal T maxValue;
      internal T minValue;

      public void Initialize()
      {
        mapImageWidth  = Form2.singleton.PrimaryRaysMapPictureBox.Width; // TODO: can it be hard coded for PRIMARYraysMapPictureBox
        mapImageHeight = Form2.singleton.PrimaryRaysMapPictureBox.Height;

        mapArray = new T[mapImageWidth, mapImageHeight];      

        wasAveraged = false;
      }

      public virtual void RenderMap()
      {
        if (mapImageWidth == 0 || mapImageHeight == 0)
        {
          Initialize();
        }

        SetMinimumAndMaximum ();

        mapBitmap = new Bitmap ( mapImageWidth, mapImageHeight, PixelFormat.Format24bppRgb );

        for ( int x = 0; x < mapImageWidth; x++ )
        {
          for ( int y = 0; y < mapImageHeight; y++ )
          {
            mapBitmap.SetPixel ( x, y, GetAppropriateColor ( x, y ) );
          }
        }
      }

      protected abstract void SetReferenceMinAndMaxValues ();

      protected abstract Color GetAppropriateColor ( int x, int y );

      protected bool wasAveraged;

      protected void AverageMap()
      {
        if ( primaryRaysMap == null )
        {
          AdvancedTools.Initialize();
        }

        if ( primaryRaysMap.mapArray == null )
        {
          primaryRaysMap.Initialize();
        }

        if ( wasAveraged )
        {
          return;
        }

        for ( int i = 0; i < mapImageWidth; i++ )
        {
          for ( int j = 0; j < mapImageHeight; j++ )
          {
            if ( primaryRaysMap.mapArray[i, j] != 0 ) // TODO: Fix 0 rays count
            {
              DivideArray(i, j);
            }
          }
        }

        wasAveraged = true;
      }

      protected abstract void DivideArray (int x, int y);

      public Bitmap GetBitmap()
      {
        if ( mapBitmap == null )
        {
          RenderMap ();
        }

        return mapBitmap;
      }

      public abstract dynamic GetValueAtCoordinates ( int x, int y );

      /// <summary>
      /// Sets minimal and maximal values found in a mapArray
      /// </summary>
      /// <typeparam name="T">Type IComparable</typeparam>
      protected virtual void SetMinimumAndMaximum()
      {
        SetReferenceMinAndMaxValues ();

        for ( int x = 0; x < depthMap.mapImageWidth; x++ )
        {
          for ( int y = 0; y < depthMap.mapImageHeight; y++ )
          {          
            if ( ( mapArray[ x, y ] as IComparable<T> ).CompareTo ( maxValue ) > 0 )
              maxValue = mapArray[ x, y ];
            if ( ( mapArray[x, y] as IComparable<T> ).CompareTo ( minValue ) < 0 )
              minValue = mapArray[ x, y ];
          }
        }
      }
    }

    public static Bitmap GetBitmapGeneral ( Bitmap mapBitmap, RenderMap renderMapMethod )
    {
      if ( mapBitmap == null )
      {
        renderMapMethod();
      }

      return mapBitmap;
    }


    /// <summary>
    /// Returns minimal and maximal values found in a specific map
    /// Values are returned via reference and must be of type IComparable
    /// </summary>
    /// <typeparam name="T">Type IComparable</typeparam>
    /// <param name="minValue">Must be initially set to max value of corresponding type</param>
    /// <param name="maxValue">Must be initially set to min value of corresponding type</param>
    /// <param name="map">2D array of values</param>
    private static void GetMinimumAndMaximum<T> ( ref T minValue, ref T maxValue, T[,] map ) where T: IComparable
    {
      for ( int x = 0; x < depthMap.mapImageWidth; x++ )
      {
        for ( int y = 0; y < depthMap.mapImageHeight; y++ )
        {
          if ( map[ x, y ].CompareTo ( maxValue ) > 0 )
            maxValue = map[ x, y ];

          if ( map[ x, y ].CompareTo ( minValue ) < 0 )
            minValue = map[ x, y ];
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

      if ( double.IsNaN ( colorValue ) || double.IsInfinity ( colorValue ) )  // TODO: Needed or just throw exception?
      {
        colorValue = 0;
      }

      return Arith.HSVToColor ( 240 - colorValue, 1, 1 );
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
      double colorValue = Math.Log ( ( newValue - minValue + 1 ), ( maxValue - minValue + 1 ) ) * 240;

      if ( double.IsNaN ( colorValue ) || double.IsInfinity ( colorValue ) )  // TODO: Needed or just throw exception?
      {
        colorValue = 0;
      }

      return Arith.HSVToColor ( colorValue, 1, 1 );
    }

    /// <summary>
    /// Sets all values of 2D array to specific value
    /// </summary>
    /// <typeparam name="T">Type of array and value</typeparam>
    /// <param name="array">Array to populate</param>
    /// <param name="value">Desired value</param>
    /// <param name="selectedValue">Only these values are replaced if selected is true</param>
    /// <param name="selected">Switches between changing all values or only values equal to selectedValue</param>
    private static void PopulateArray2D<T> ( this T[,] array, T value, T selectedValue, bool selected )
    {
      if ( selected )
      {
        for ( int i = 0; i < array.GetLength(0); i++ )
        {
          for ( int j = 0; j < array.GetLength(1); j++ )
          {
            if ( array[i, j].Equals( selectedValue ) )
            {
              array[i, j] = value;
            }            
          }
        }
      }
      else
      {
        for ( int i = 0; i < array.GetLength(0); i++ )
        {
          for ( int j = 0; j < array.GetLength(1); j++ )
          {
            array[i, j] = value;
          }
        }
      }        
    }

    /// <summary>
    /// Calls Initialize method of all subclasses in AdvancedTools
    /// </summary>
    public static void SetNewDimensions ()
    {
      if ( primaryRaysMap == null || allRaysMap == null || depthMap == null)
      {
        Initialize();
      }

      depthMap.Initialize ();
      primaryRaysMap.Initialize ();
      allRaysMap.Initialize ();
    }

    /// <summary>
    /// Removes all maps and unnecessary stuff
    /// </summary>
    public static void NewRenderInitialization ()
    {
      if ( primaryRaysMap != null )
      {
        primaryRaysMap.mapArray = null;
      }

      if ( allRaysMap != null )
      {
        allRaysMap.mapArray = null;
      }

      depthMap.mapArray = null;

      normalMap.mapArray = null;
      normalMap.intersectionMapArray = null;
    }
  }
}


/// <summary>
/// Interface for all maps
/// </summary>
interface IMap<T>
{
  void Initialize();

  void RenderMap();

  Bitmap GetBitmap ();

  dynamic GetValueAtCoordinates ( int x, int y );
}