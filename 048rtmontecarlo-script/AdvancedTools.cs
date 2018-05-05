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

    internal static RaysMap PrimaryRaysMap;
    internal static RaysMap AllRaysMap;

    private static void Initialize ()
    {
      PrimaryRaysMap = new RaysMap ();
      AllRaysMap = new RaysMap ();
    }

    public static void Register ( int level, Vector3d rayOrigin, Intersection firstIntersection )
    {
      if ( Form2.singleton == null )
      {
        return;
      }

      if ( PrimaryRaysMap == null || AllRaysMap == null )
      {
        Initialize ();       
      }

      if ( PrimaryRaysMap.mapArray == null || AllRaysMap.mapArray == null )
      {
        PrimaryRaysMap.Initialize ();
        AllRaysMap.Initialize ();
      }

      if ( DepthMap.mapArray == null )
        DepthMap.Initialize ();

      if ( NormalMap.mapArray == null || NormalMap.intersectionMapArray == null)
        NormalMap.Initialize ( rayOrigin );

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
        DepthMap.mapArray[MT.x, MT.y] += depth;

        // register primary rays
        PrimaryRaysMap.mapArray[MT.x, MT.y] += 1;

        if ( firstIntersection != null )
        {
          // register normal vector
          NormalMap.intersectionMapArray[ MT.x, MT.y ] += firstIntersection.CoordWorld;
          NormalMap.mapArray[ MT.x, MT.y ] += firstIntersection.Normal;       
        }
      }

      // register all rays
      AllRaysMap.mapArray[ MT.x, MT.y ] += 1;
    }


    public static class DepthMap
    {
      public static void Initialize()
      {
        mapImageWidth = Form2.singleton.DepthMapPictureBox.Width;
        mapImageHeight = Form2.singleton.DepthMapPictureBox.Height;

        mapArray = new double[mapImageWidth, mapImageHeight];

        wasAveraged = false;
      }

      /// <summary>
      /// Image width in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int mapImageWidth;

      /// <summary>
      /// Image height in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int mapImageHeight;

      internal static double[,] mapArray;

      private static Bitmap mapBitmap;

      private static double maxDepth;
      private static double minDepth;

      public static void RenderMap()
      {
        if ( mapImageWidth == 0 || mapImageHeight == 0 )
        {
          Initialize ();
        }

        AverageMap ();

        maxDepth = double.MinValue;
        minDepth = double.MaxValue;

        GetMinimumAndMaximum ( ref minDepth, ref maxDepth, mapArray );

        mapBitmap = new Bitmap ( mapImageWidth, mapImageHeight, PixelFormat.Format24bppRgb );

        PopulateArray2D<double> ( mapArray, maxDepth, 0, true );

        minDepth = double.MaxValue;
        GetMinimumAndMaximum ( ref minDepth, ref maxDepth, mapArray ); // TODO: New minimum after replacing all zeroes

        for ( int x = 0; x < mapImageWidth; x++ )
        {
          for ( int y = 0; y < mapImageHeight; y++ )
          {
            mapBitmap.SetPixel ( x, y, GetAppropriateColorLogarithmicReversed ( minDepth, maxDepth, mapArray[ x, y ] ) );
          }
        }
      }

      public static bool wasAveraged;

      private static void AverageMap ()
      {
        if ( PrimaryRaysMap == null )
        {
          AdvancedTools.Initialize ();
        }

        if ( PrimaryRaysMap.mapArray == null )
        {
          PrimaryRaysMap.Initialize ();
        }

        if ( wasAveraged )
        {
          return;
        }

        for ( int i = 0; i < mapImageWidth; i++ )
        {
          for ( int j = 0; j < mapImageHeight; j++ )
          {
            if ( PrimaryRaysMap.mapArray[ i, j ] != 0 ) // TODO: Fix 0 rays count
            {
              mapArray[ i, j ] /= PrimaryRaysMap.mapArray[ i, j ];
            }
          }
        }

        wasAveraged = true;
      }

      public static double GetDepthAtLocation ( int x, int y )
      {
        if ( mapArray[ x, y ] >= maxDepth ) // TODO: PositiveInfinity in depthMap?
        {
          return double.PositiveInfinity;
        }
        else
        {
          return mapArray[ x, y ];
        }
      }

      public static Bitmap GetBitmap ()
      {
        return GetBitmapGeneral(mapBitmap, RenderMap);
      }
    }

    public class RaysMap : Map<int>
    {
      public override void SetReferenceMinAndMaxValues ()
      {
        maxValue = int.MinValue;
        minValue = int.MaxValue;
      }

      public override Color GetAppropriateColor ( dynamic mapArrayElement )
      {
        return GetAppropriateColorLinear ( minValue, maxValue, mapArrayElement );
      }

      public override void DivideArray ( int x, int y )
      {
        mapArray[x, y] /= PrimaryRaysMap.mapArray[ x, y ];
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

    public static class NormalMap
    {
      /// <summary>
      /// Image width in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int mapImageWidth;

      /// <summary>
      /// Image height in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int mapImageHeight;

      internal static Vector3d[,] mapArray;
      internal static Vector3d[,] intersectionMapArray;

      private static Bitmap mapBitmap;

      public static Vector3d rayOrigin;      

      public static void Initialize ( Vector3d newRayOrigin )
      {
        mapImageWidth  = Form2.singleton.PrimaryRaysMapPictureBox.Width; // TODO: can it be hard coded for PRIMARYraysMapPictureBox
        mapImageHeight = Form2.singleton.PrimaryRaysMapPictureBox.Height;

        mapArray = new Vector3d[mapImageWidth, mapImageHeight];
        intersectionMapArray = new Vector3d[mapImageWidth, mapImageHeight];

        rayOrigin = newRayOrigin;

        wasAveraged = false;
      }

      public static void RenderMap()
      {
        if (mapImageWidth == 0 || mapImageHeight == 0)
        {
          Initialize ( rayOrigin );
        }

        AverageMap ( intersectionMapArray );

        mapBitmap = new Bitmap(mapImageWidth, mapImageHeight, PixelFormat.Format24bppRgb);

        for ( int x = 0; x < mapImageWidth; x++ )
        {
          for ( int y = 0; y < mapImageHeight; y++ )
          {
            mapBitmap.SetPixel ( x, y, GetAppropriateColorForNormalVectorSimpleVersion( mapArray[ x, y ], intersectionMapArray[ x, y ] ) );
            //mapBitmap.SetPixel ( x, y, GetAppropriateColorForNormalVector ( mapArray[ x, y ], intersectionMapArray[x, y]) );
          }
        }
      }

      public static bool wasAveraged;

      private static void AverageMap ( Vector3d[,] map )
      {
        if ( wasAveraged )
        {
          return;
        }     

        for ( int i = 0; i < mapImageWidth; i++ )
        {
          for ( int j = 0; j < mapImageHeight; j++ )
          {
            if ( PrimaryRaysMap.mapArray[ i, j ] != 0 ) // TODO: Fix 0 rays count
            {
              map[ i, j ] /= PrimaryRaysMap.mapArray[ i, j ];
            }
          }
        }

        wasAveraged = true;
      }

      public static Bitmap GetBitmap()
      {
        return GetBitmapGeneral ( mapBitmap, RenderMap );
      }

      public static double GetNormalVectorAngleAtLocation(int x, int y)
      {
        return Vector3d.CalculateAngle ( mapArray[ x, y ], rayOrigin - intersectionMapArray[ x, y ] ) * 180 / Math.PI;
      }

      private static Color GetAppropriateColorForNormalVector ( Vector3d normalizedNormalVector, Vector3d intersectionVector)
      {
        Vector3d normalVector = normalizedNormalVector + intersectionVector;

        double angle = Vector3d.CalculateAngle ( normalVector, rayOrigin );

        throw new NotImplementedException();
      }

      private static Color GetAppropriateColorForNormalVectorSimpleVersion(Vector3d normalizedNormalVector, Vector3d intersectionVector)
      {
        double angle = Vector3d.CalculateAngle ( normalizedNormalVector, rayOrigin - intersectionVector ) * 180 / Math.PI;

        double colorValue = angle / 90 * 240;

        if ( double.IsNaN( colorValue ) )
        {
          return Color.FromArgb ( 1, 1, 1, 1 );
        }

        return Arith.HSVToColor ( 240 - colorValue, 1, 1 );
      }
    }


    public abstract class Map<T>: IMap where T : IComparable
    {
      /// <summary>
      /// Image width in pixels, 0 for default value (according to panel size).
      /// </summary>
      public int mapImageWidth;

      /// <summary>
      /// Image height in pixels, 0 for default value (according to panel size).
      /// </summary>
      public int mapImageHeight;

      private Bitmap mapBitmap;

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

      public void RenderMap()
      {
        if (mapImageWidth == 0 || mapImageHeight == 0)
        {
          Initialize();
        }

        RemoveNullsFromArray ();

        SetMinimumAndMaximum<T> ();

        mapBitmap = new Bitmap ( mapImageWidth, mapImageHeight, PixelFormat.Format24bppRgb );

        for ( int x = 0; x < mapImageWidth; x++ )
        {
          for ( int y = 0; y < mapImageHeight; y++ )
          {
            mapBitmap.SetPixel ( x, y, GetAppropriateColor ( mapArray[ x, y ] ) );
          }
        }
      }

      private void RemoveNullsFromArray ()
      {
        for (int i = 0; i < mapArray.GetLength(0); i++)
        {
          for (int j = 0; j < mapArray.GetLength(1); j++)
          {
            if (Object.ReferenceEquals(null, mapArray[i, j]))
            {
              mapArray[i, j] = default(T);
            }           
          }
        }
      }

      public abstract void SetReferenceMinAndMaxValues ();

      public abstract Color GetAppropriateColor ( dynamic mapArrayElement );

      public bool wasAveraged;

      private void AverageMap()
      {
        if ( wasAveraged )
        {
          return;
        }

        for ( int i = 0; i < mapImageWidth; i++ )
        {
          for ( int j = 0; j < mapImageHeight; j++ )
          {
            if ( PrimaryRaysMap.mapArray[i, j] != 0 ) // TODO: Fix 0 rays count
            {
              DivideArray(i, j);
              //mapArray[ i, j ] /= PrimaryRaysMap.mapArray[ i, j ];
            }
          }
        }

        wasAveraged = true;
      }

      public abstract void DivideArray (int x, int y);

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
      private void SetMinimumAndMaximum<T> () where T : IComparable
      {
        SetReferenceMinAndMaxValues ();

        for ( int x = 0; x < DepthMap.mapImageWidth; x++ )
        {
          for ( int y = 0; y < DepthMap.mapImageHeight; y++ )
          {          
            if ( (mapArray[ x, y ]).CompareTo ( maxValue ) > 0 )
              maxValue = mapArray[ x, y ];
            if ( mapArray[ x, y ].CompareTo ( minValue ) < 0 )
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
      for ( int x = 0; x < DepthMap.mapImageWidth; x++ )
      {
        for ( int y = 0; y < DepthMap.mapImageHeight; y++ )
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
      if ( PrimaryRaysMap == null || AllRaysMap == null )
      {
        Initialize();
      }

      DepthMap.Initialize ();
      PrimaryRaysMap.Initialize ();
      AllRaysMap.Initialize ();
    }

    /// <summary>
    /// Removes all maps and unnecessary stuff
    /// </summary>
    public static void NewRenderInitialization ()
    {
      if ( PrimaryRaysMap != null )
      {
        PrimaryRaysMap.mapArray = null;
      }

      if ( AllRaysMap != null )
      {
        AllRaysMap.mapArray = null;
      }

      DepthMap.mapArray = null;

      NormalMap.mapArray = null;
      NormalMap.intersectionMapArray = null;
    }
  }
}


/// <summary>
/// Interface for all maps
/// </summary>
interface IMap
{
  void Initialize();

  void RenderMap();

  Bitmap GetBitmap ();

  dynamic GetValueAtCoordinates ( int x, int y );
}