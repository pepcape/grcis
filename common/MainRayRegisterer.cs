class MainRayRegisterer: RayRegisterer
{
  /// <summary>
  /// Translate function for several ray register methods from AdditionalViews and RayVisualizer
  /// Be careful with params - not type safe
  /// </summary>
  /// <param name="rayType">RayType which chooses method to be called</param>
  /// <param name="parameters">All possible parameters that can be passed to individual ray register methods</param>
  public void RegisterRay ( RayType rayType, params object[] parameters )
  {
    if ( rayType == RayType.unknown )
      rayType = DetermineRayType ();

    switch ( rayType )
    {
      //ray for statistics and maps (AdditionalViews)
      case RayType.mapsNormal:
        //register ray for statistics and maps
        if ( AdditionalViews.singleton != null)
          AdditionalViews.singleton.Register ( (int) parameters[0], (Vector3d) parameters[1], (Intersection) parameters[2] );         
        break;

      //ray for RayVisualizer
      case RayType.rayVisualizerNormal:
        if ( !MT.singleRayTracing )
          return;
        if ( parameters[2] is Vector3d vector )
          RayVisualizer.singleton?.RegisterRay ( (Vector3d) parameters[1], vector );
        if ( parameters[2] is Intersection intersection )
          RayVisualizer.singleton?.RegisterRay ( (Vector3d) parameters[1], intersection.CoordWorld );
        break;

      //shadow ray for RayVisualizer
      case RayType.rayVisualizerShadow:         
        if ( !MT.singleRayTracing )
          return;
        RayVisualizer.singleton?.RegisterShadowRay ( (Vector3d) parameters [0], (Vector3d) parameters [1] );
        break;
    }     
  }

  /// <summary>
  /// Determined whether ray comes from normal rendering or single ray rendering (for information about pixel and RayVisualizer)
  /// </summary>
  /// <returns>Returns chosen RayType</returns>
  private RayType DetermineRayType ()
  {
    if ( MT.singleRayTracing )
    {
      return RayType.rayVisualizerNormal;
    }
    else
    {
      return RayType.mapsNormal;
    }
  }
}