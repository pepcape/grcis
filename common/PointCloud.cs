using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using Utilities;

namespace Rendering
{
  /// <summary>
  /// This class takes care of point cloud representation, storing, saving as well as reading from file.
  /// </summary>
  public class PointCloud
  {
    public List<float>[] cloud;

    public int numberOfElements;

    private Action PointCloudSavingStart;
    private MethodInvoker PointCloudSavingEnd;
    private Func<MethodInvoker, object> Invoke;

    private Action<string, string, int> Notification;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="numberOfThreads">Number of threads used for rendering depicts number of separate lists in cloud data structure</param>
    public PointCloud (int numberOfThreads)
    {
      InitializeCloudArray(numberOfThreads);
    }

    /// <summary>
    /// Sets necessary Form's methods
    /// </summary>
    /// <param name="newPointCloud">Bool whether new point cloud should be created</param>
    /// <param name="PointCloudSavingStart">Delegate to form's method which sets GUI at the beginning of point cloud saving</param>
    /// <param name="PointCloudSavingEnd">MethodInvoker to form's method which sets GUI at the end of point cloud saving</param>
    /// <param name="Notification">Delegate to notification method</param>
    /// <param name="Invoke">Form's Invoke method</param>
    public void SetNecessaryFields (Action PointCloudSavingStart, MethodInvoker PointCloudSavingEnd,
                                    Action<string, string, int> Notification, Func<MethodInvoker, object> Invoke)
    {
      this.PointCloudSavingStart = PointCloudSavingStart;
      this.PointCloudSavingEnd = PointCloudSavingEnd;
      this.Notification = Notification;
      this.Invoke = Invoke;
    }

    /// <summary>
    /// Creates new cloud
    /// </summary>
    private void InitializeCloudArray (int numberOfThreads, int initialListCapacity = 0)
    {
      if (initialListCapacity == 0)
        initialListCapacity = 100000 * 9 / numberOfThreads;

      cloud = new List<float>[numberOfThreads];

      for (int i = 0; i < cloud.Length; i++)
      {
        cloud[i] = new List<float>(initialListCapacity);
      }

      numberOfElements = 0;
    }

    /// <summary>
    /// Adds a new vertex to point cloud data structure
    /// Thread-safe
    /// </summary>
    /// <param name="coord">Coordinates of vertex</param>
    /// <param name="color">Color of vertex (RGB values between 0-1)</param>
    /// <param name="normal">Normal vector in vertex</param>
    /// <param name="index">Index of list where to store data - is usually MT.threadID, unique for each thread to accomplish thread-safety</param>
    public void AddToPointCloud (Vector3d coord, double[] color, Vector3d normal, int index)
    {
      Vector3d fixedCoordinates = AxesCorrector(coord);
      Vector3d fixedNormals = AxesCorrector(normal);

      float[] fixedColors =
      {
        (float)color[0],
        (float)color[1],
        (float)color[2]
      };

      float[] coordArrray = new float[] {(float)fixedCoordinates.X, (float)fixedCoordinates.Y, (float)fixedCoordinates.Z};
      float[] normalArray = new float[] {(float)fixedNormals.X,     (float)fixedNormals.Y,     (float)fixedNormals.Z};

      cloud[index].AddRange(coordArrray);
      cloud[index].AddRange(fixedColors);
      cloud[index].AddRange(normalArray);

      numberOfElements++;
    }

    public bool IsCloudEmpty => cloud == null;

    /// <summary>
    /// Saves content of cloud to external file (can be opened in MashLab or other software with ASCII .ply support)
    /// </summary>
    /// <param name="fileName">Name of external file - .ply file extension is added if it does not contain it already</param>
    public void SaveToPLYFile (string fileName)
    {
      PointCloudSavingStart();
      MT.pointCloudSavingInProgress = true;

      if (fileName == null)
        fileName = "PointCloud.ply";

      if (fileName.Length < 4 ||
          fileName.Substring(fileName.Length - 4) != ".ply")
        fileName += ".ply";

      Thread fileSaver = new Thread(() => ActualSave(fileName))
      {
        Name = "FileSaver"
      };

      fileSaver.Start ();
    }

    /// <summary>
    /// Actual file save
    /// </summary>
    /// <param name="fileName"></param>
    private void ActualSave (string fileName)
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; // needed for dot as decimal separator in float

      using (StreamWriter streamWriter = new StreamWriter(fileName))
      {
        WritePLYFileHeader(streamWriter);

        foreach (List<float> list in cloud)
        {
          for (int i = 0; i < list.Count; i += 9)
          {
            streamWriter.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
                                   list[i], list[i + 1], list[i + 2],
                                   Util.Clamp((int)(list[i + 3] * 255), 0, 255),
                                   Util.Clamp((int)(list[i + 4] * 255), 0, 255),
                                   Util.Clamp((int)(list[i + 5] * 255), 0, 255),
                                   list[i + 6], list[i + 7], list[i + 8]);
          }
        }

        streamWriter.Close();
      }

      Invoke(PointCloudSavingEnd);
      MT.pointCloudSavingInProgress = false;

      Notification (@"File succesfully saved", $"Point cloud file \"{fileName}\" succesfully saved.", 30000);
    }

    /// <summary>
    /// Header of PLY file - specifies number of verticies and their format (coordinates[XYZ], color[RGB], normal[XYZ] - one line per vertex)
    /// </summary>
    /// <param name="streamWriter"></param>
    private void WritePLYFileHeader (StreamWriter streamWriter)
    {
      streamWriter.WriteLine("ply");
      streamWriter.WriteLine("format ascii 1.0");
      streamWriter.WriteLine("element vertex {0}", numberOfElements);
      streamWriter.WriteLine("property float x");
      streamWriter.WriteLine("property float y");
      streamWriter.WriteLine("property float z");
      streamWriter.WriteLine("property uchar red");
      streamWriter.WriteLine("property uchar green");
      streamWriter.WriteLine("property uchar blue");
      streamWriter.WriteLine("property float nx");
      streamWriter.WriteLine("property float ny");
      streamWriter.WriteLine("property float nz");
      streamWriter.WriteLine("end_header");
    }

    /// <summary>
    /// Very simple and non-flexible parser of PLY files
    /// Clears point cloud data structure, reads .ply file and parses it as new point cloud
    /// Input .ply file must have exactly same header and body structure as .ply exported by SaveToPLYFile method
    /// Throws an exception if there is something wrong with input file
    /// (there is many cases what can go wrong - this parser is very sensitive to correct structure)
    /// </summary>
    /// <param name="fileName">Name of .ply file to read</param>
    public void ReadFromFile (string fileName)
    {
      InitializeCloudArray(1);

      using (StreamReader streamReader = new StreamReader(fileName))
      {
        int vertexCount = ReadPLYFileHeader(streamReader);

        InitializeCloudArray(1, vertexCount * 9);

        if (vertexCount < 0)
          throw new Exception("Incorrect element count in input file.");


        for (int i = 0; i < vertexCount; i++)
        {
          bool[] returnBools = new bool[9];

          string[] line = streamReader.ReadLine()?.Split(' ');

          if (line == null || line.Length != valuesPerVertex)
            throw new Exception($"Error at line {headerLength + i} in input file.");

          returnBools[0] = float.TryParse(line[0], out float x);
          returnBools[1] = float.TryParse(line[1], out float y);
          returnBools[2] = float.TryParse(line[2], out float z);
          float[] coordinates = new float[] {x, y, z};

          returnBools[3] = float.TryParse(line[3], out float red);
          returnBools[4] = float.TryParse(line[4], out float green);
          returnBools[5] = float.TryParse(line[5], out float blue);
          float[] colors = new float[] {red, green, blue};

          returnBools[6] = float.TryParse(line[6], out float nx);
          returnBools[7] = float.TryParse(line[7], out float ny);
          returnBools[8] = float.TryParse(line[8], out float nz);
          float[] normals = new float[] {nx, ny, nz};

          cloud[0].AddRange(coordinates);
          cloud[0].AddRange(colors);
          cloud[0].AddRange(normals);
        }

        streamReader.Close();
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
    public int ReadPLYFileHeader (StreamReader streamReader)
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

      for (int i = 0; i < correctHeader.Length; i++)
      {
        string line = streamReader.ReadLine()?.Trim ();

        if (i == 3 && line != null)
        {
          string[] elementLine = line.Split(' ');
          if (elementLine[0] != "element" ||
              elementLine[1] != "vertex" ||
              int.TryParse(elementLine[2], out vertexCount))
            throw new Exception ($"Error at line {i + 1} in input file.");

          continue;
        }

        if (line == null ||
            line != correctHeader[i])
          throw new Exception ($"Error at line {i + 1} in input file.");

      }

      return vertexCount;
    }

    private static readonly Vector3d axesCorrectionVector = new Vector3d (1, 1, -1);

    /// <summary>
    /// Corrects axes (different axes labels/positioning system for scene in original raytracer and system used for PLY files)
    /// Inverts 3rd axis of position
    /// </summary>
    /// <param name="position">Position in original raytracer scene</param>
    /// <returns>Position in standard system for PLY files (or zero vector if input is null)</returns>
    public static Vector3d AxesCorrector (Vector3d? position)
    {
      if (position == null)
        return Vector3d.Zero;

      return (Vector3d)position * axesCorrectionVector;
    }
  }
}
