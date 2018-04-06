using System;
using System.Drawing;
using System.Drawing.Imaging;
using MathSupport;
using OpenTK;
using Rendering;

namespace _048rtmontecarlo
{
  public static class AdvancedTools
  {
    internal static RaysMap PrimaryRaysMap;
    internal static RaysMap AllRaysMap;

    private static void Initialize ()
    {
      PrimaryRaysMap = new RaysMap ();
      AllRaysMap = new RaysMap ();
    }

    public static void Register(int level, Vector3d rayOrigin, Intersection firstIntersection)
    {
      if ( Form2.singleton == null )
      {
        return;
      }

      if ( PrimaryRaysMap == null || AllRaysMap == null )
      {
        Initialize ();       
      }

      if (PrimaryRaysMap.raysMap == null || AllRaysMap.raysMap == null)
      {
        PrimaryRaysMap.Initialize ();
        AllRaysMap.Initialize ();
      }

      if ( DepthMap.depthMap == null )
        DepthMap.Initialize ();

      if (PrimaryRaysMap.raysMap == null)
        PrimaryRaysMap.Initialize ();

      double depth;

      if ( firstIntersection == null )
      {
        depth = 10000; // TODO: CHANGE - placeholder for "infinity"
      }
      else
      {
        depth = Vector3d.Distance(rayOrigin, firstIntersection.CoordWorld);
      }
     

      if ( level == 0 )
      {
        // register depth
        DepthMap.depthMap[MT.x, MT.y] += depth;

        // register primary rays
        PrimaryRaysMap.raysMap[MT.x, MT.y]++;       
      }

      // register all rays
      AllRaysMap.raysMap[ MT.x, MT.y ]++;
    }


    public static class DepthMap
    {
      public static void Initialize()
      {
        DepthMapImageWidth = Form2.singleton.DepthMapPictureBox.Width;
        DepthMapImageHeight = Form2.singleton.DepthMapPictureBox.Height;

        depthMap = new double[DepthMapImageWidth, DepthMapImageHeight];

        wasAveraged = false;
      }

      /// <summary>
      /// Image width in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int DepthMapImageWidth;

      /// <summary>
      /// Image height in pixels, 0 for default value (according to panel size).
      /// </summary>
      public static int DepthMapImageHeight;

      internal static double[,] depthMap;

      private static Bitmap depthMapBitmap;

      private static double maxDepth;
      private static double minDepth;

      public static void RenderDepthMap()
      {
        if ( DepthMapImageWidth == 0 || DepthMapImageHeight == 0 )
        {
          Initialize ();
        }

        AverageMap ();

        maxDepth = double.MinValue;
        minDepth = double.MaxValue;

        GetMinimumAndMaximum ( ref minDepth, ref maxDepth, depthMap );

        depthMapBitmap = new Bitmap ( DepthMapImageWidth, DepthMapImageHeight, PixelFormat.Format24bppRgb );

        PopulateArray2D<double> ( depthMap, maxDepth, 0, true );

        minDepth = double.MaxValue;
        GetMinimumAndMaximum ( ref minDepth, ref maxDepth, depthMap ); // TODO: New minimum after replacing all zeroes

        for ( int x = 0; x < DepthMapImageWidth; x++ )
        {
          for ( int y = 0; y < DepthMapImageHeight; y++ )
          {
            depthMapBitmap.SetPixel ( x, y, GetAppropriateColorLogarithmicReversed ( minDepth, maxDepth, depthMap[ x, y ] ) );
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

        if ( PrimaryRaysMap.raysMap == null )
        {
          PrimaryRaysMap.Initialize ();
        }

        if ( wasAveraged )
        {
          return;
        }

        for ( int i = 0; i < DepthMapImageWidth; i++ )
        {
          for ( int j = 0; j < DepthMapImageHeight; j++ )
          {
            if ( PrimaryRaysMap.raysMap[ i, j ] != 0 ) // TODO: Fix 0 rays count
            {
              depthMap[ i, j ] /= PrimaryRaysMap.raysMap[ i, j ];
            }
          }
        }

        wasAveraged = true;
      }

      public static double GetDepthAtLocation ( int x, int y )
      {
        if ( depthMap[ x, y ] >= maxDepth ) // TODO: PositiveInfinity in depthMap?
        {
          return double.PositiveInfinity;
        }
        else
        {
          return depthMap[ x, y ];
        }
      }

      public static Bitmap GetBitmap ()
      {
        if (depthMapBitmap == null)
        {
          RenderDepthMap();
        }

        return depthMapBitmap;
      }
    }

    public class RaysMap : IRaysMap
    {
      /// <summary>
      /// Image width in pixels, 0 for default value (according to panel size).
      /// </summary>
      public int RaysMapImageWidth;

      /// <summary>
      /// Image height in pixels, 0 for default value (according to panel size).
      /// </summary>
      public int RaysMapImageHeight;

      internal int[,] raysMap;

      private Bitmap raysMapBitmap;

      public void Initialize ()
      {
        RaysMapImageWidth = Form2.singleton.PrimaryRaysMapPictureBox.Width;   // TODO: can it be hard coded for PRIMARYraysMapPictureBox
        RaysMapImageHeight = Form2.singleton.PrimaryRaysMapPictureBox.Height;

        raysMap = new int[RaysMapImageWidth, RaysMapImageHeight];
      }

      public void RenderRaysMap ()
      {
        if (RaysMapImageWidth == 0 || RaysMapImageHeight == 0)
        {
          Initialize();
        }

        int maxValue = int.MinValue;
        int minValue = int.MaxValue;

        GetMinimumAndMaximum(ref minValue, ref maxValue, raysMap);

        raysMapBitmap = new Bitmap(RaysMapImageWidth, RaysMapImageHeight, PixelFormat.Format24bppRgb);

        for (int x = 0; x < RaysMapImageWidth; x++)
        {
          for (int y = 0; y < RaysMapImageHeight; y++)
          {
            raysMapBitmap.SetPixel(x, y, GetAppropriateColorLinear(minValue, maxValue, raysMap[x, y]));
          }
        }
      }

      public Bitmap GetBitmap ()
      {
        if (raysMapBitmap == null)
        {
          RenderRaysMap ();
        }

        return raysMapBitmap;
      }

      public int GetRaysCountAtLocation ( int x, int y )
      {
        if ( x < 0 || x >= RaysMapImageWidth || y < 0 || y >= RaysMapImageHeight )
        {
          return -1;
        }

        return raysMap[x, y];
      }
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
      for (int x = 0; x < DepthMap.DepthMapImageWidth; x++)
      {
        for (int y = 0; y < DepthMap.DepthMapImageHeight; y++)
        {
          if ( map[ x, y ].CompareTo ( maxValue ) > 0 )
            maxValue = map[ x, y ];

          if ( map[ x, y ].CompareTo ( minValue ) < 0 )
            minValue = map[ x, y ];


          /*if (map[x, y] > maxValue)
            maxValue = map[x, y];

          if (map[x, y] < minValue)
            minValue = map[x, y];*/
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
      double colorValue = (newValue - minValue) / (maxValue - minValue) * 240;

      if (double.IsNaN(colorValue) || double.IsInfinity(colorValue))  // TODO: Needed or just throw exception?
      {
        colorValue = 0;
      }

      return Arith.HSVToColor(240 - colorValue, 1, 1);
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
      double colorValue = Math.Log((newValue - minValue + 1), (maxValue - minValue + 1)) * 240;

      if (double.IsNaN(colorValue) || double.IsInfinity(colorValue))  // TODO: Needed or just throw exception?
      {
        colorValue = 0;
      }

      return Arith.HSVToColor(colorValue, 1, 1);
    }

    /// <summary>
    /// Sets all values of 2D array to specific value
    /// </summary>
    /// <typeparam name="T">Type of array and value</typeparam>
    /// <param name="array">Array to populate</param>
    /// <param name="value">Desired value</param>
    /// <param name="selectedValue">Only these values are replaced if selected is true</param>
    /// <param name="selected">Switches between changing all values or only values equal to selectedValue</param>
    private static void PopulateArray2D<T>(this T[,] array, T value, T selectedValue, bool selected)
    {
      if ( selected )
      {
        for (int i = 0; i < array.GetLength(0); i++)
        {
          for (int j = 0; j < array.GetLength(1); j++)
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
        for (int i = 0; i < array.GetLength(0); i++)
        {
          for (int j = 0; j < array.GetLength(1); j++)
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
      if (PrimaryRaysMap == null || AllRaysMap == null)
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
      if (PrimaryRaysMap != null)
      {
        PrimaryRaysMap.raysMap = null;
      }

      if (AllRaysMap != null)
      {
        AllRaysMap.raysMap = null;
      }

      DepthMap.depthMap = null;
    }
  }
}


/// <summary>
/// Interface for maps based on rays count such as PrimaryRaysMap and AllRaysMap
/// </summary>
interface IRaysMap
{
  void Initialize ();

  void RenderRaysMap ();

  Bitmap GetBitmap ();

  int GetRaysCountAtLocation ( int x, int y );
}