//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

// Override image resolution and supersampling.
context[PropertyName.CTX_WIDTH]         = 640;    // whatever is convenient for your debugging/testing/final rendering
context[PropertyName.CTX_HEIGHT]        = 640;

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.8, 0.1}, 0.1, 0.6, 0.4, 128));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.05, 0.07};

// Camera.
scene.Camera = new FishEyeCamera (new Vector3d(0.0, 2.0, 4.0),
                                  new Vector3d(0.0, 0.0, 1.0),
                                  360);

// Light sources.
scene.Sources = new LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.1));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 4.0, -3.0), 1.5));

// --- NODE DEFINITIONS ----------------------------------------------------

// Init.
Cube c;
PhongMaterial pm = new PhongMaterial(new double[] {0.20, 0.20, 0.15}, 0.05, 0.05, 0.1, 128);
pm.n = 2.8;
pm.Kt = 0.8;

CSGInnerNode Monoliths = new CSGInnerNode(SetOperation.Union);
Monoliths.SetAttribute(PropertyName.MATERIAL, pm);
root.InsertChild(Monoliths, Matrix4d.CreateTranslation(0.0, -1.0, 0.0));

// Front left.
CSGInnerNode FrontLeft = new CSGInnerNode(SetOperation.Union);
Monoliths.InsertChild(FrontLeft, Matrix4d.CreateTranslation(-7.5, 0.0, 3.5));
c = new Cube();
FrontLeft.InsertChild(c,Matrix4d.Scale(1.5, 4.0, 1.0) * Matrix4d.RotateY(0.6) * Matrix4d.CreateTranslation(0.0, 0.0, 1.5));
c = new Cube();
FrontLeft.InsertChild(c,Matrix4d.Scale(1.5, 4.0, 1.0) * Matrix4d.RotateY(0.6) * Matrix4d.CreateTranslation(2.0, 0.0, 3.5));
c = new Cube();
FrontLeft.InsertChild(c,Matrix4d.Scale(0.7, 5.0, 1.5) * Matrix4d.RotateZ((float)Math.PI*3/2) * Matrix4d.RotateY(-0.8) * Matrix4d.CreateTranslation(1.0, 4.5, 0.0));

// Front.
CSGInnerNode Front = new CSGInnerNode(SetOperation.Union);
Monoliths.InsertChild(Front, Matrix4d.RotateY(-0.2) * Matrix4d.CreateTranslation(-0.5, 0.0, 8.5));
c = new Cube();
Front.InsertChild(c,Matrix4d.Scale(0.8, 4.0, 1.5) * Matrix4d.CreateTranslation(0.0, 0.0, 0.0));
c = new Cube();
Front.InsertChild(c,Matrix4d.Scale(0.8, 4.0, 1.5) * Matrix4d.CreateTranslation(2.0, 0.0, 0.0));

// Front right.
CSGInnerNode FrontRight = new CSGInnerNode(SetOperation.Union);
Monoliths.InsertChild(FrontRight, Matrix4d.CreateTranslation(9.0, 0.0, 5.5));
c = new Cube();
FrontRight.InsertChild(c,Matrix4d.Scale(1.0, 4.0, 1.0) * Matrix4d.RotateY(-0.5));
c = new Cube();
FrontRight.InsertChild(c,Matrix4d.Scale(1.0, 7.0, 1.0) * Matrix4d.RotateX(-0.7) * Matrix4d.RotateY(-0.5) * Matrix4d.CreateTranslation(-2.5, 0.0, 3.5));

// Left.
CSGInnerNode Left = new CSGInnerNode(SetOperation.Union);
Monoliths.InsertChild(Left, Matrix4d.CreateTranslation(-8.0, -1.0, -1.5));
c = new Cube();
Left.InsertChild(c,Matrix4d.Scale(1.1, 4.0, 0.8));
c = new Cube();
Left.InsertChild(c,Matrix4d.Scale(1.4, 2.2, 1.0) * Matrix4d.CreateTranslation(0.0, 0.0, -3.0));
c = new Cube();
Left.InsertChild(c,Matrix4d.Scale(1.2, 4.0, 0.9) * Matrix4d.RotateX(4.2) * Matrix4d.CreateTranslation(0.0, 4.0, 0.5));

// Back left.
CSGInnerNode BackLeft = new CSGInnerNode(SetOperation.Union);
Monoliths.InsertChild(BackLeft, Matrix4d.CreateTranslation(-1.0, 0.0, -8.5));
c = new Cube();
BackLeft.InsertChild(c,Matrix4d.Scale(1.5, 4.0, 1.0) * Matrix4d.RotateZ(2.3) * Matrix4d.RotateX(4.7));

// Back right.
CSGInnerNode BackRight = new CSGInnerNode(SetOperation.Union);
Monoliths.InsertChild(BackRight, Matrix4d.RotateY(2.6) * Matrix4d.CreateTranslation(3.0, 0.0, -9.0));
c = new Cube();
BackRight.InsertChild(c,Matrix4d.Scale(0.8, 4.0, 1.5));
c = new Cube();
BackRight.InsertChild(c,Matrix4d.Scale(0.8, 4.0, 1.5) * Matrix4d.CreateTranslation(-3.5, 0.0, 0.0));
c = new Cube();
BackRight.InsertChild(c,Matrix4d.Scale(0.7, 5.0, 1.5) * Matrix4d.RotateZ(4.7) * Matrix4d.CreateTranslation(-3.5, 4.0, 0.0));


// Right.
CSGInnerNode Right = new CSGInnerNode(SetOperation.Union);
Monoliths.InsertChild(Right, Matrix4d.RotateY((float)Math.PI/2) * Matrix4d.CreateTranslation(10.5, 0.0, -1.5));
c = new Cube();
Right.InsertChild(c,Matrix4d.Scale(0.8, 4.0, 1.3));
c = new Cube();
Right.InsertChild(c,Matrix4d.Scale(0.8, 4.0, 1.3) * Matrix4d.CreateTranslation(-2.5, 0.0, 0.0));
c = new Cube();
Right.InsertChild(c,Matrix4d.Scale(0.5, 5.0, 1.7) * Matrix4d.RotateZ(4.7) * Matrix4d.CreateTranslation(-3.5, 4.0, 0.0));

// Infinite plane with checker.
Plane pl = new Plane();
pl.SetAttribute(PropertyName.COLOR, new double[] {0.9, 0.8, 0.1});
pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.6, 0.6, new double[] {0.1, 0.1, 1.0}));
root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));


/// <summary>
/// Angular fish-eye based camera with adjustable viewing angle
/// Produced image is circle in square (aspect ration always 1:1)
/// </summary>
public class FishEyeCamera : ICamera
{
  protected double width;
  protected double height;
  protected Vector3d center;
  protected Vector3d direction;
  protected double directionAngle;
  protected double aperture;

  /// <summary>
  /// Initializing constructor, able to set all camera parameters.
  /// </summary>
  /// <param name="cen">Position of camera.</param>
  /// <param name="dir">View direction (must not be zero).</param>
  /// <param name="aper">Viewing angle of camera. Can be up to 360 thanks to fish-eye characteristics.</param>
  public FishEyeCamera(Vector3d cen, Vector3d dir, double aper)
  {
    center = cen;
    direction = dir;
    direction.Normalize();
    directionAngle = (Math.Atan(direction.Z / direction.X) / Math.PI * 180) % 360;

    width = 540;
    height = 540;
    aperture = aper / 180 * Math.PI;
  }

  // Width and Height are always same -> aspec ratio is 1:1
  public double AspectRatio
  {
    get => 1;
    set {}
  }

  public double Width
  {
    get => width;
    set
    {
      width = value;
      height = value;
    }
  }

  public double Height
  {
    get => height;
    set
    {
      height = value;
      width = value;
    }
  }

  /// <summary>
  /// Ray-generator. Simple variant, w/o an integration support.
  /// </summary>
  /// <param name="x">Origin position within a viewport (horizontal coordinate).</param>
  /// <param name="y">Origin position within a viewport (vertical coordinate).</param>
  /// <param name="p0">Ray origin.</param>
  /// <param name="p1">Ray direction vector.</param>
  /// <returns>True if the ray (viewport position) is valid.</returns>
  public bool GetRay(double x, double y, out Vector3d p0, out Vector3d p1)
  {
    p0 = center;

    // transformation to normalized coordinates (-1 to 1)
    double normalX = 2 * x / width - 1;
    double normalY = 2 * (height - y) / height - 1;

    // assignment of polar coordinates
    double r = Math.Sqrt(normalX * normalX + normalY * normalY);
    double phi;

    if (r > 1)
    {
      p1 = new Vector3d();   // must be assigned before return command
      return false;           // false return value for nondefined area (outside of circle) => background color in place
    }

    if (Math.Abs(r) < 0.001)
    {
      phi = 0;
    }
    else
    {
      if (normalX < 0)
      {
        phi = Math.PI - Math.Asin(normalY / r);
      }
      else
      {
        phi = Math.Asin(normalY / r);
      }
    }

    double theta = r * aperture / 2;

    p1 = new Vector3d(Math.Sin(theta) * Math.Cos(phi),
                      Math.Sin(theta) * Math.Sin(phi),
                      Math.Cos(theta));

    p1 = Vector3d.TransformVector(p1, Matrix4d.RotateY(-(Math.Atan2(direction.Z, direction.X) - Math.PI / 2)));

    return true;
  }

  /// <summary>
  /// Ray-generator. Internal integration support.
  /// </summary>
  /// <param name="x">Origin position within a viewport (horizontal coordinate).</param>
  /// <param name="y">Origin position within a viewport (vertical coordinate).</param>
  /// <param name="rank">Rank of this ray, 0 <= rank < total (for integration).</param>
  /// <param name="total">Total number of rays (for integration).</param>
  /// <param name="p0">Ray origin.</param>
  /// <param name="p1">Ray direction vector.</param>
  /// <returns>True if the ray (viewport position) is valid.</returns>
  public bool GetRay(double x, double y, int rank, int total, out Vector3d p0, out Vector3d p1)
  {
    return GetRay(x, y, out p0, out p1);
  }
}
