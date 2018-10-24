using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using OpenTK;

namespace Rendering
{
  /// <summary>
  /// This class takes care of point cloud representation, storing, saving as well as reading from file 
  /// </summary>
  public class PointCloud
  {
    private ConcurrentBag<Vertex> cloud;

    public PointCloud ()
    {
      InitializeCloudArray ();
    }

    /// <summary>
    /// Creates new cloud
    /// </summary>
    private void InitializeCloudArray ()
    {
      cloud = new ConcurrentBag<Vertex> ();
    }

    /// <summary>
    /// Adds a new vertex to point cloud data structure
    /// Thread-safe
    /// </summary>
    /// <param name="coord">Coordinates of vertex</param>
    /// <param name="color">Color of vertex (RGB values between 0-1)</param>
    /// <param name="normal">Normal vector in vertex</param>
    public void AddToPointCloud ( Vector3d coord, double[] color, Vector3d normal )
    {
      Vector3d fixedCoordinates = AxesCorrector ( coord );
      Vector3d fixedNormals = AxesCorrector ( normal );
      byte[] fixedColors =
      {
        (byte) ( color [ 0 ] * 255 ),
        (byte) ( color [ 1 ] * 255 ),
        (byte) ( color [ 2 ] * 255 )
      };

      Vertex newVertex = new Vertex ( fixedCoordinates, fixedColors, fixedNormals );

      cloud.Add ( newVertex );
    }

    public bool IsCloudEmpty
    {
      get { return cloud.IsEmpty (); }
    }

    /// <summary>
    /// Saves content of cloud to external file (can be opened in MashLab or other software with ASCII .ply support)
    /// </summary>
    /// <param name="fileName">Name of external file - .ply file extension is added if it does not contain it already</param>
    public void SaveToPLYFile ( string fileName )
    {
      if ( fileName == null )
        fileName = "PointCloud.ply";

      if ( fileName.Length < 4 || fileName.Substring ( fileName.Length - 4 ) != ".ply" )
        fileName += ".ply";

      Thread fileSaver = new Thread ( ActualSave );
      fileSaver.Name = "FileSaver";

      fileSaver.Start ( fileName );
    }

    /// <summary>
    /// Actual file save - abstraction needed for better handling of parameterized thread start
    /// </summary>
    /// <param name="fileName"></param>
    private void ActualSave ( object fileName )
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture ( "en-GB" ); // needed for dot as decimal separator in float

      using ( StreamWriter streamWriter = new StreamWriter ( (string) fileName ) )
      {
        WritePLYFileHeader ( streamWriter );

        foreach ( Vertex vertex in cloud )
        {       
          streamWriter.WriteLine ( "{0} {1} {2} {3} {4} {5} {6} {7} {8}",
                                   vertex.coord.X, vertex.coord.Y, vertex.coord.Z,
                                   (byte) ( vertex.color [ 0 ] * 255 ), (byte) ( vertex.color [ 1 ] * 255 ), (byte) ( vertex.color [ 2 ] * 255 ),
                                   vertex.normal.X, vertex.normal.Y, vertex.normal.Z );
        }

        streamWriter.Close ();
      }

      Form2.singleton?.Notification ( @"File succesfully saved", $"Point cloud file \"{(string) fileName}\" succesfully saved.", 30000 );
    }

    /// <summary>
    /// Header of PLY file - specifies number of verticies and their format (coordinates[XYZ], color[RGB], normal[XYZ] - one line per vertex)
    /// </summary>
    /// <param name="streamWriter"></param>
    private void WritePLYFileHeader ( StreamWriter streamWriter )
    {
      streamWriter.WriteLine ( "ply" );
      streamWriter.WriteLine ( "format ascii 1.0" );
      streamWriter.WriteLine ( "element vertex {0}", cloud.Count );
      streamWriter.WriteLine ( "property float x" );
      streamWriter.WriteLine ( "property float y" );
      streamWriter.WriteLine ( "property float z" );
      streamWriter.WriteLine ( "property uchar red" );
      streamWriter.WriteLine ( "property uchar green" );
      streamWriter.WriteLine ( "property uchar blue" );
      streamWriter.WriteLine ( "property float nx" );
      streamWriter.WriteLine ( "property float ny" );
      streamWriter.WriteLine ( "property float nz" );
      streamWriter.WriteLine ( "end_header" );
    }


    /// <summary>
    /// Very simple and non-flexible parser of PLY files
    /// Clears point cloud data structure, reads .ply file and parses it as new point cloud
    /// Input .ply file must have exactly same header and body structure as .ply exported by SaveToPLYFile method
    /// Throws an exception if there is something wrong with input file
    /// (there is many cases what can go wrong - this parser is very sensitive to correct structure)
    /// </summary>
    /// <param name="fileName">Name of .ply file to read</param>
    public void ReadFromFile ( string fileName )
    {
      InitializeCloudArray ();

      using ( StreamReader streamReader = new StreamReader ( fileName ) )
      {
        int vertexCount = ReadPLYFileHeader ( streamReader );

        if ( vertexCount < 0 )
          throw new Exception ( "Incorrect element count in input file." );


        for ( int i = 0; i < vertexCount; i++ )
        {
          bool[] returnBools = new bool[9];

          string[] line = streamReader.ReadLine ()?.Split ( ' ' );

          if ( line == null || line.Length != valuesPerVertex )
            throw new Exception ( $"Error at line {headerLength + i} in input file." );

          returnBools [ 0 ] = double.TryParse ( line [ 0 ], out double x );
          returnBools [ 1 ] = double.TryParse ( line [ 1 ], out double y );
          returnBools [ 2 ] = double.TryParse ( line [ 2 ], out double z );
          Vector3d coordinates = new Vector3d ( x, y, z );

          returnBools [ 3 ] = byte.TryParse ( line [ 3 ], out byte red );
          returnBools [ 4 ] = byte.TryParse ( line [ 4 ], out byte green );
          returnBools [ 5 ] = byte.TryParse ( line [ 5 ], out byte blue );
          byte[] colors = new byte[] { red, green, blue };

          returnBools [ 6 ] = double.TryParse ( line [ 6 ], out double nx );
          returnBools [ 7 ] = double.TryParse ( line [ 7 ], out double ny );
          returnBools [ 8 ] = double.TryParse ( line [ 8 ], out double nz );
          Vector3d normals = new Vector3d ( nx, ny, nz );

          Vertex newVertex = new Vertex ( coordinates, colors, normals );

          cloud.Add ( newVertex );
        }

        streamReader.Close ();
      }
    }


    private const int headerLength = 14;
    private const int valuesPerVertex = 9;
    /// <summary>
    /// Reads header of PLY file
    /// Input .ply file must have exactly same header and body structure as .ply exported by SaveToPLYFile method
    /// Throws an exception if there is something wrong with input file
    /// (there is many cases what can go wrong - this parser is very sensitive to correct structure)
    /// </summary>
    /// <param name="streamReader">StreamReader to use for .ply file</param>
    /// <returns>Number of elements (vertices) defined on 3rd line of .ply file</returns>
    public int ReadPLYFileHeader ( StreamReader streamReader )
    {
      string[] correctHeader = new string[]
      {
        "ply",
        "format ascii 1.0",
        "element vertex **",
        "property float x",
        "property float y",
        "property float z",
        "property uchar red",
        "property uchar green",
        "property uchar blue",
        "property float nx",
        "property float ny",
        "property float nz",
        "end_header"
      };

      int vertexCount = -1;

      for ( int i = 0; i < correctHeader.Length; i++ )
      {
        string line = streamReader.ReadLine ()?.Trim ();

        if ( i == 3 && line != null)
        {
          string[] elementLine = line.Split ( ' ' );
          if ( elementLine[0] != "element" || elementLine[1] != "vertex" || int.TryParse ( elementLine[2], out vertexCount ) )
            throw new Exception ( $"Error at line {i + 1} in input file." );

          continue;
        }

        if ( line == null || line != correctHeader[i] )
          throw new Exception ( $"Error at line {i + 1} in input file." );

      }

      return vertexCount;
    }


    private static readonly Vector3d axesCorrectionVector = new Vector3d ( 1, 1, -1 );
    /// <summary>
    /// Corrects axes (different axes labels/positioning system for scene in original raytracer and system used for FLY files)
    /// Inverts 3rd axis of position
    /// </summary>
    /// <param name="position">Position in original raytracer scene</param>
    /// <returns>Position in OpenGL system (or zero vector if input is null)</returns>
    public static Vector3d AxesCorrector ( Vector3d? position )
    {
      if ( position == null )
      {
        return new Vector3d ( 0, 0, 0 );
      }

      return (Vector3d) position * axesCorrectionVector;
    }
  }

  /// <summary>
  /// Data structure containing info about 1 vertex (coordinates, color and normal vector) in point cloud
  /// </summary>
  public struct Vertex
  {
    public Vector3d coord;
    public byte[] color;
    public Vector3d normal;

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="coord">Coordinates of vertex</param>
    /// <param name="color">Color of vertex (RGB 0-255)</param>
    /// <param name="normal">Normal vector in vertex</param>
    public Vertex ( Vector3d coord, byte[] color, Vector3d normal )
    {
      this.coord = coord;
      this.color = color;
      this.normal = normal;
    }
  }
}
