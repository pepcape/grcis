// Author: Josef Pelikan

using System.IO;
using OpenTK;

namespace Scene3D
{
  public class Construction
  {
    #region Instance data

    // !!! If you need any instance data, put them here..

    #endregion

    #region Construction

    public Construction ()
    {
      // !!! Any one-time initialization code goes here..
    }

    #endregion

    #region Mesh construction

    /// <summary>
    /// Construct a new Brep solid (preferebaly closed = regular one).
    /// </summary>
    /// <param name="scene">B-rep scene to be modified</param>
    /// <param name="m">Transform matrix (object-space to world-space)</param>
    /// <param name="variant">Shape variant if needed</param>
    /// <returns>Number of generated faces (0 in case of failure)</returns>
    public int AddMesh ( SceneBrep scene, Matrix4 m, float variant )
    {
      // !!!{{ TODO: put your Mesh-construction code here

      return 0;

      // !!!}}
    }

    #endregion
  }
}
