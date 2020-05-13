using OpenTK;
using Rendering;

/// <summary>
/// Implementation of AbstractRayRegisterer
/// Registers rays to both AdditionalViews for maps and to RayVisualizer
/// </summary>
class MainRayRegisterer: AbstractRayRegisterer
{
  private readonly AdditionalViews additionalViews;
  private readonly RayVisualizer rayVisualizer;

  public MainRayRegisterer (AdditionalViews additionalViews, RayVisualizer rayVisualizer)
  {
    this.additionalViews = additionalViews;
    this.rayVisualizer = rayVisualizer;
  }

  /// <summary>
  /// Translate function for several ray register methods from AdditionalViews and RayVisualizer
  /// Be careful with params - not type safe
  /// </summary>
  /// <param name="rayType">RayType which chooses method to be called</param>
  /// <param name="parameters">All possible parameters that can be passed to individual ray register methods</param>
  public override void RegisterRay ( AbstractRayRegisterer.RayType rayType, params object[] parameters )
  {
    if ( rayType == AbstractRayRegisterer.RayType.unknown )
      rayType = DetermineRayType ();

    switch ( rayType )
    {
      //ray for statistics and maps (AdditionalViews)
      case AbstractRayRegisterer.RayType.mapsNormal:
        //register ray for statistics and maps
        additionalViews?.Register ( (int) parameters[0], (Vector3d) parameters[1], (Intersection) parameters[2] );
        break;

      //ray for RayVisualizer
      case AbstractRayRegisterer.RayType.rayVisualizerNormal:
        if ( !MT.singleRayTracing )
          return;
        if ( parameters[2] is Vector3d vector )
          rayVisualizer?.RegisterRay ( (Vector3d) parameters[1], vector );
        if ( parameters[2] is Intersection intersection )
          rayVisualizer?.RegisterRay ( (Vector3d) parameters[1], intersection.CoordWorld );
        break;

      //shadow ray for RayVisualizer
      case AbstractRayRegisterer.RayType.rayVisualizerShadow:         
        if ( !MT.singleRayTracing )
          return;
        rayVisualizer?.RegisterShadowRay ( (Vector3d) parameters [0], (Vector3d) parameters [1] );
        break;
    }     
  }

  /// <summary>
  /// Determined whether ray comes from normal rendering or single ray rendering (for information about pixel and RayVisualizer)
  /// </summary>
  /// <returns>Returns chosen RayType</returns>
  private AbstractRayRegisterer.RayType DetermineRayType ()
  {
    if ( MT.singleRayTracing )
    {
      return AbstractRayRegisterer.RayType.rayVisualizerNormal;
    }
    else
    {
      return AbstractRayRegisterer.RayType.mapsNormal;
    }
  }
}