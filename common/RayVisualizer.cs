#define USE_INVALIDATE

using System.Collections.Generic;
using OpenTK;

namespace Rendering
{
  /// <summary>
  /// Takes care of registering rays into lists which are later used for their rendering
  /// </summary>
  public class RayVisualizer
  {
    public static RayVisualizer singleton;

    internal List<Vector3d> rays;
    internal List<Vector3d> shadowRays;

    private static readonly Vector3d axesCorrectionVector = new Vector3d ( 1, 1, -1 );

    private const int initialListCapacity = 32;

    /// <summary>
    /// Prepares lists
    /// Initial list capacity is small enough to accomodate all elements in most cases
    /// (used for speed-up since ray registration and rendering should be in real-time)
    /// </summary>
    public RayVisualizer ()
    {
      rays       = new List<Vector3d> ( initialListCapacity );
      shadowRays = new List<Vector3d> ( initialListCapacity );
    }

    /// <summary>
    /// Registers normal ray
    /// </summary>
    /// <param name="rayOrigin">Position of the beginning of ray</param>
    /// <param name="rayTarget">Position of the end of ray</param>
    public void RegisterRay ( Vector3d rayOrigin, Vector3d rayTarget )
    {
      rays.Add ( AxesCorrector ( rayOrigin ) );
      rays.Add ( AxesCorrector ( rayTarget ) );
    }

    /// <summary>
    /// Registers shadow ray (from intersection to position of light source)
    /// </summary>
    /// <param name="rayOrigin">Position of the beginning of ray - intersection with scene</param>
    /// <param name="rayTarget">Position of the end of ray - position of light source</param>
    public void RegisterShadowRay ( Vector3d rayOrigin, Vector3d rayTarget )
    {
      shadowRays.Add ( AxesCorrector ( rayOrigin ) );
      shadowRays.Add ( AxesCorrector ( rayTarget ) );
    }

    /// <summary>
    /// Empties lists to prepare them for a new set of rays
    /// </summary>
    public void Reset ()
    {
      rays       = new List<Vector3d> ( initialListCapacity );
      shadowRays = new List<Vector3d> ( initialListCapacity );
    }

    /// <summary>
    /// Corrects axes (different axes labels/positioning system for scene in original raytracer and OpenGL system)
    /// Inverts 3rd axis of position
    /// </summary>
    /// <param name="position">Position in original raytracer scene</param>
    /// <returns>Position in OpenGL system (or zero vector if input is null)</returns>
    public static Vector3d AxesCorrector ( Vector3d? position )
    {
      if ( position == null )
        return new Vector3d ( 0, 0, 0 );
      else
        return (Vector3d) position * axesCorrectionVector;
    }

    public static IRayScene rayScene;
    /// <summary>
    /// Called when there is a new scene about to be rendered
    /// </summary>
    /// <param name="scene">Scene which is about to be rendered</param>
    public static void UpdateRayScene ( IRayScene scene )
    {
      rayScene = scene;
      singleton?.rays?.Clear ();
      singleton?.shadowRays?.Clear ();
    }
  }
}
