/// <summary>
/// Abstract class used for registering rays
/// </summary>
public abstract class AbstractRayRegisterer
{
  /// <summary>
  /// Main and only required method. Registers ray with of type RayType and optional number of parameters
  /// </summary>
  /// <param name="rayType">Type of ray from RayType enum</param>
  /// <param name="parameters">Arbitrary number of any objects used as parameters. RegisterRay itself must decide what to do with them</param>
  public abstract void RegisterRay(RayType rayType, params object[] parameters);

  public enum RayType
  {
    rayVisualizerNormal,
    rayVisualizerShadow,
    mapsNormal,
    unknown
  };
}
