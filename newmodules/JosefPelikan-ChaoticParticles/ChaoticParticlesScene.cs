//////////////////////////////////////////////////
// Externals.

using JosefPelikan;     // ChaoticParticles + CatmullRomAnimator + StarBackground

//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

// Override image resolution and supersampling.
context[PropertyName.CTX_WIDTH]         = 640;    // whatever is convenient for your debugging/testing/final rendering
context[PropertyName.CTX_HEIGHT]        = 360;
context[PropertyName.CTX_SUPERSAMPLING] =  16;

double end = 24.0;
context[PropertyName.CTX_START_ANIM]    =  0.0;
context[PropertyName.CTX_END_ANIM]      =  end;
context[PropertyName.CTX_FPS]           = 25.0;

//////////////////////////////////////////////////
// Preprocessing stage support.

// Uncomment the block if you need preprocessing.
/*
if (Util.TryParseBool(context, PropertyName.CTX_PREPROCESSING))
{
  double time = 0.0;
  bool single = Util.TryParse(context, PropertyName.CTX_TIME, ref time);
  // if (single) simulate only for a single frame with the given 'time'

  // TODO: put your preprocessing code here!
  //
  // It will be run only this time.
  // Store preprocessing results to arbitrary (non-reserved) context item,
  //  subsequent script calls will find it there...
}
*/

// Optional override of rendering algorithm and/or renderer.

/*
context[PropertyName.CTX_ALGORITHM] = new RayTracing();

int ss = 0;
if (Util.TryParse(context, PropertyName.CTX_SUPERSAMPLING, ref ss) &&
    ss > 1)
  context[PropertyName.CTX_SYNTHESIZER] = new SupersamplingImageSynthesizer
  {
    Supersampling = ss,
    Jittering = 1.0
  };
*/

//////////////////////////////////////////////////
// Param string from UI.

// Params dictionary.
Dictionary<string, string> p = Util.ParseKeyValueList(param);

// n = <index-of-refraction>
double n = 1.6;
Util.TryParse(p, "n", ref n);

// mat = {mirror|glass|diffuse}
PhongMaterial pm = new PhongMaterial(new double[] {0.7, 0.5, 1.0}, 0.01, 0.02, 0.9, 8192);

if (p.TryGetValue("mat", out string mat))
  switch (mat)
  {
    case "diffuse":
      pm = new PhongMaterial(new double[] {0.1, 0.1, 0.6}, 0.1, 0.8, 0.2, 16);
      break;

    case "glass":
      pm = new PhongMaterial(new double[] {0.0, 0.2, 0.1}, 0.05, 0.05, 0.1, 128);
      pm.n = n;
      pm.Kt = 0.9;
      break;

    default:
    case "mirror":
      break;
  }

// Tooltip (if script uses values from 'param').
context[PropertyName.CTX_TOOLTIP] = "n=<double> (index of refraction)\rmat={mirror|glass|diffuse}}";

//////////////////////////////////////////////////
// CSG scene.

AnimatedCSGInnerNode root = new AnimatedCSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.8, 0.1}, 0.1, 0.6, 0.4, 128));
scene.Intersectable = root;

// Optional Animator.
string namePos   = "partPos";
string nameColor = "colPos";

CatmullRomAnimator pa = new CatmullRomAnimator()
{
  Start =  0.0,
  End   =  end
};
pa.newProperty(namePos, 0.0, end, 8.0,
               PropertyAnimator.InterpolationStyle.Cyclic,
               new List<Vector4d[]>()
               {
                 new Vector4d[]
                 {
                   new Vector4d(0.0, 0.2,-0.2, 0.4),
                   new Vector4d(2.0, 0.2,-0.2, 0.2),
                   new Vector4d(2.0, 1.2, 0.0, 1.5),
                   new Vector4d(0.0, 2.2, 0.0, 1.8),
                 },
                 new Vector4d[]
                 {
                   new Vector4d(2.0, 0.2,-0.2, 0.2),
                   new Vector4d(2.0, 1.2, 0.0, 1.5),
                   new Vector4d(0.0, 2.2, 0.0, 1.8),
                   new Vector4d(0.0, 0.2,-0.2, 0.4),
                 },
                 new Vector4d[]
                 {
                   new Vector4d(2.0, 1.2, 0.0, 1.5),
                   new Vector4d(0.0, 2.2, 0.0, 1.8),
                   new Vector4d(0.0, 0.2,-0.2, 0.4),
                   new Vector4d(2.0, 0.2,-0.2, 0.2),
                 },
                 new Vector4d[]
                 {
                   new Vector4d(0.0, 2.2, 0.0, 1.8),
                   new Vector4d(0.0, 0.2,-0.2, 0.4),
                   new Vector4d(2.0, 0.2,-0.2, 0.2),
                   new Vector4d(2.0, 1.2, 0.0, 1.5),
                 },
               },
               true);
pa.newProperty(nameColor, 0.0, end, 6.0,
               PropertyAnimator.InterpolationStyle.Cyclic,
               new List<Vector3[]>()
               {
                 new Vector3[]
                 {
                   new Vector3(1.0f, 0.3f, 0.2f),
                   new Vector3(0.5f, 0.9f, 0.5f),
                   new Vector3(0.2f, 0.2f, 1.0f),
                   new Vector3(0.0f, 0.8f, 1.0f),
                 },
                 new Vector3[]
                 {
                   new Vector3(0.3f, 0.9f, 0.2f),
                   new Vector3(0.0f, 0.3f, 0.7f),
                   new Vector3(0.4f, 0.4f, 0.1f),
                   new Vector3(0.3f, 0.7f, 0.6f),
                 },
                 new Vector3[]
                 {
                   new Vector3(0.5f, 0.5f, 0.8f),
                   new Vector3(0.7f, 0.4f, 0.6f),
                   new Vector3(0.2f, 0.6f, 0.8f),
                   new Vector3(0.2f, 0.4f, 1.0f),
                 },
               },
               true);
scene.Animator = pa;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.02, 0.02};
scene.Background = new StarBackground(scene.BackgroundColor, 600, 0.008, 0.5, 1.6, 1.0);

// Camera.
AnimatedCamera cam = new AnimatedCamera(new Vector3d(0.7,  1.0,   0.0),
                                        new Vector3d(0.7,  0.8, -10.0),
                                        50.0);
cam.End = end;      // one complete turn takes 24.0 seconds
AnimatedRayScene ascene = scene as AnimatedRayScene;
if (ascene != null)
  ascene.End = end;
scene.Camera  = cam;

// Light sources:
scene.Sources = new LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 4.0, -3.0), 1.2));

// --- NODE DEFINITIONS ----------------------------------------------------

ISolid s;

// Mirror/transparent sphere.
s = new Sphere();
s.SetAttribute(PropertyName.MATERIAL, pm);
root.InsertChild(s, Matrix4d.CreateTranslation(0.0, 0.0, 0.5));

// Opaque sphere.
s = new Sphere();
root.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(1.5, 0.2, 2.4));

// Infinite plane with checker texture.
s = new Plane();
s.SetAttribute(PropertyName.COLOR, new double[] {0.4, 0.08, 0.0});
s.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.6, 0.6, new double[] {1.0, 1.0, 1.0}));
root.InsertChild(s, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));

// Callback for glowing objects.
RecursionFunction del = (Intersection i, Vector3d dir, double importance, out RayRecursion rr) =>
{
  double direct = 1.0 - i.TextureCoord.X;
  direct = Math.Pow(direct * direct, 6.0);

  rr = new RayRecursion(
    Util.ColorClone(i.SurfaceColor, direct),
    new RayRecursion.RayContribution(i, dir, importance));

  return 144L;
};

// Particle object.
s = new ChaoticParticles(
  new Vector4d[]
  {
    new Vector4d(0.0, 0.2,-0.2, 0.4),
    new Vector4d(2.0, 0.2,-0.2, 0.2),
    new Vector4d(2.0, 1.2, 0.0, 1.5),
    new Vector4d(0.0, 2.2, 0.0, 1.8),
  },
  new Vector3[]
  {
    new Vector3(1.0f, 0.3f, 0.2f),
    new Vector3(0.5f, 0.9f, 0.5f),
    new Vector3(0.2f, 0.2f, 1.0f),
    new Vector3(0.0f, 0.8f, 1.0f),
  },
  namePos, nameColor);
s.SetAttribute(PropertyName.RECURSION, del);
s.SetAttribute(PropertyName.NO_SHADOW, true);
s.SetAttribute(PropertyName.COLOR, new double[] {0.3, 0.9, 1.0});
root.InsertChild(s, Matrix4d.CreateTranslation(0.0, -0.5, -2.0));
