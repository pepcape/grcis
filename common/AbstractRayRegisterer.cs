public abstract class AbstractRayRegisterer
{
  public abstract void RegisterRay(RayType rayType, params object[] parameters);

  public enum RayType
  {
    rayVisualizerNormal,
    rayVisualizerShadow,
    mapsNormal,
    unknown
  };
}