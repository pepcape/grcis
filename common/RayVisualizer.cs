#define USE_INVALIDATE

using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK;

namespace Rendering
{
  /// <summary>
  /// Takes care of registering rays into lists which are later used for their rendering
  /// </summary>
  public class RayVisualizer
  {
    public static RayVisualizer singleton;

    public RayVisualizerForm form;

    internal List<Vector3> rays;
    internal List<Vector3> shadowRays;

    private static readonly Vector3d axesCorrectionVector = new Vector3d ( 1, 1, -1 );

    private const int initialListCapacity = 32;

    /// <summary>
    /// Prepares lists
    /// Initial list capacity is small enough to accomodate all elements in most cases
    /// (used for speed-up since ray registration and rendering should be in real-time)
    /// </summary>
    public RayVisualizer ()
    {
      rays       = new List<Vector3> ( initialListCapacity );
      shadowRays = new List<Vector3> ( initialListCapacity );

      singleton = this;
    }

    /// <summary>
    /// Registers normal ray
    /// </summary>
    /// <param name="rayOrigin">Position of the beginning of ray</param>
    /// <param name="rayTarget">Position of the end of ray</param>
    public void RegisterRay ( Vector3d rayOrigin, Vector3d rayTarget )
    {
      rays.Add ( (Vector3) AxesCorrector ( rayOrigin ) );
      rays.Add ( (Vector3) AxesCorrector ( rayTarget ) );
    }

    /// <summary>
    /// Registers shadow ray (from intersection to position of light source)
    /// </summary>
    /// <param name="rayOrigin">Position of the beginning of ray - intersection with scene</param>
    /// <param name="rayTarget">Position of the end of ray - position of light source</param>
    public void RegisterShadowRay ( Vector3d rayOrigin, Vector3d rayTarget )
    {
      shadowRays.Add ( (Vector3) AxesCorrector ( rayOrigin ) );
      shadowRays.Add ( (Vector3) AxesCorrector ( rayTarget ) );
    }

    /// <summary>
    /// Empties lists to prepare them for a new set of rays
    /// </summary>
    public void Reset ()
    {
      rays       = new List<Vector3> ( initialListCapacity );
      shadowRays = new List<Vector3> ( initialListCapacity );
    }

    public void AddingRaysFinished ()
    {
      form?.UpdateRaysVBO ( rays, shadowRays );
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
        return position.Value * axesCorrectionVector;
    }

    public IRayScene scene;
    public int[] backgroundColor;
    /// <summary>
    /// Called when there is new scene about to be rendered
    /// </summary>
    /// <param name="newScene">New scene</param>
    public void UpdateScene ( IRayScene newScene )
    {
      scene = newScene;
      rays?.Clear ();
      shadowRays?.Clear ();

      backgroundColor = new int[] 
      {
        (int) ( newScene.BackgroundColor [0] * 255 ),
        (int) ( newScene.BackgroundColor [1] * 255 ),
        (int) ( newScene.BackgroundColor [2] * 255 )
      };

      if ( form != null )
        if ( form.InvokeRequired )
          form?.Invoke ( (MethodInvoker) delegate { form.UpdateScene ( newScene ); } );
        else
          form.UpdateScene ( newScene );      
    }
  }
}
