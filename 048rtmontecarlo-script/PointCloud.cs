using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;

namespace Rendering
{
  /// <summary>
  /// This class takes care of point cloud representation, storing, saving as well as reading from file 
  /// </summary>
  public class PointCloud
  {
    public ConcurrentBag<Vertex> cloud;

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


    public void ReadFromFile ( string fileName )
    {
      InitializeCloudArray ();

      throw new NotImplementedException ();
    }
  }

  /// <summary>
  /// Data structure containing info about 1 vertex (coordinates, color and normal vector) in point cloud
  /// </summary>
  public struct Vertex
  {
    public Vector3d coord;
    public double[] color;
    public Vector3d normal;

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="coord">Coordinates of vertex</param>
    /// <param name="color">Color of vertex (RGB as values between 0-1)</param>
    /// <param name="normal">Normal vector in vertex</param>
    public Vertex ( Vector3d coord, double[] color, Vector3d normal )
    {
      this.coord = coord;
      this.color = color;
      this.normal = normal;
    }
  }
}
