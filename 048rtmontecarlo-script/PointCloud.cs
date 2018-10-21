using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloud
{
  public class PointCloud
  {
    private Vertex[][][] cloud;

    private const int sizeX = 200;
    private const int sizeY = 200;
    private const int sizeZ = 200;

    private int vertexCounter;

    public PointCloud ()
    {
      InitializeCloudArray ();
    }

    private void InitializeCloudArray ()
    {  
      cloud = new Vertex[sizeX][][];

      for ( int i = 0; i < sizeX; i++ )
      {
        cloud [ i ] = new Vertex[sizeY][];

        for ( int j = 0; j < sizeY; j++ )
        {
          cloud [ i ] [ j ] = new Vertex[sizeZ];
        }
      }

      vertexCounter = 0;
    }

    public void StoreToCloud ( int x, int y, int z, Vertex vertex )
    {


      vertexCounter++;
    }

    private bool SaveToPLYFile ( string fileName )
    {
      int counter = 0;

      if ( fileName == null )
        fileName = "PointCloud.obj";

      if ( fileName.Length < 4 || fileName.Substring ( fileName.Length - 4 ) != ".obj" )
        fileName += ".obj";


      try
      {
        using ( StreamWriter streamWriter = new StreamWriter ( fileName ) )
        {
          WritePLYFileHeader ( streamWriter );

          for ( int x = 0; x < sizeX; x++ )
          {
            for ( int y = 0; y < sizeY; y++ )
            {
              for ( int z = 0; z < sizeZ; z++ )
              {
                Vertex vertex = cloud [ x ] [ y ] [ z ];

                streamWriter.Write ( "{0} {1} {2} ", x, y, z );
                streamWriter.Write ( "{0} {1} {2} ", vertex.color[0], vertex.color[1], vertex.color[2] );
                streamWriter.Write ( "{0} {1} {2}\n", vertex.normal[0], vertex.normal[1], vertex.normal[2] );

                counter++;
              }
            }
          }
        }
      }
      catch ( IOException )
      {
        return false;
      }

      return true;
    }

    private void WritePLYFileHeader ( StreamWriter streamWriter )
    {
      streamWriter.WriteLine ( "ply" );
      streamWriter.WriteLine ( "format ascii 1.0" );
      streamWriter.WriteLine ( "element vertex {0}", vertexCounter );
      streamWriter.WriteLine ( "property float x" );
      streamWriter.WriteLine ( "property float y" );
      streamWriter.WriteLine ( "property float z" );
      streamWriter.WriteLine ( "property float red" );
      streamWriter.WriteLine ( "property float green" );
      streamWriter.WriteLine ( "property float blue" );
      streamWriter.WriteLine ( "property float nx" );
      streamWriter.WriteLine ( "property float ny" );
      streamWriter.WriteLine ( "property float nz" );
      streamWriter.WriteLine ( "end_header" );
    }

    private void ReadFromFile ( string fileName )
    {
      InitializeCloudArray ();


    }
  }

  /// <summary>
  /// Data class containing infor about 1 vertex (color and normal vector) in point cloud
  /// </summary>
  public class Vertex
  {
    public float[] color  = new float[3];
    public float[] normal = new float[3];

    public Vertex () : base ()
    { }

    public Vertex ( float[] color )
    {
      this.color = color ?? throw new ArgumentNullException ( nameof ( color ) );
    }

    public Vertex ( float[] color, float[] normal )
    {
      this.color  = color ?? throw new ArgumentNullException ( nameof ( color ) );
      this.normal = normal ?? throw new ArgumentNullException ( nameof ( normal ) );
    }
  }
}
