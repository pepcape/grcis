using OpenTK;

namespace Rendering
{
  /// <summary>
  /// Dummy for non-fancy Ray-tracing projects.
  /// </summary>
  public class Master
  {
    public static Master singleton;

    public PointCloudDummy pointCloud;
  }

  /// <summary>
  /// Dummy for PointCloud.
  /// </summary>
  public class PointCloudDummy
  {
    public void AddToPointCloud (Vector3d coord, double[] color, Vector3d normal, int index)
    {}
  }
}
