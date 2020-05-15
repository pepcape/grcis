//////////////////////////////////////////////////
// Externals.

using JosefPelikan;     // for StarBackground

//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

// Override image resolution and supersampling.
context[PropertyName.CTX_WIDTH]         = 800;    // whatever is convenient for your debugging/testing/final rendering
context[PropertyName.CTX_HEIGHT]        = 600;
context[PropertyName.CTX_SUPERSAMPLING] =  64;

context[PropertyName.CTX_START_ANIM]    =  0.0;
context[PropertyName.CTX_END_ANIM]      = 20.0;

//context[PropertyName.CTX_ALGORITHM]     = new RayTracing();

int ss = 0;
if (Util.TryParse(context, PropertyName.CTX_SUPERSAMPLING, ref ss) &&
    ss > 1)
  context[PropertyName.CTX_SYNTHESIZER] = new SupersamplingImageSynthesizer
  {
    Supersampling = ss,
    Jittering = 1.0
  };

// Tooltip (if script uses values from 'param').
context[PropertyName.CTX_TOOLTIP] = "n=<double> (index of refraction)";

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.8, 0.1}, 0.1, 0.6, 0.4, 128));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.01, 0.03};
scene.Background = new StarBackground(scene.BackgroundColor, 600, 0.004); // 1000, 0.002

// Camera.
AnimatedCamera cam = new AnimatedCamera(new Vector3d(0.7, -0.4,  0.0),
                                        new Vector3d(0.7,  0.8, -6.0),
                                        50.0);

cam.End = 20.0; // one complete turn takes 20.0 seconds
AnimatedRayScene ascene = scene as AnimatedRayScene;
if (ascene != null)
  ascene.End = 20.0;
scene.Camera  = cam;

// Light sources:
scene.Sources = new LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 4.0, -3.0), 1.2));

// --- NODE DEFINITIONS ----------------------------------------------------

// Params dictionary.
Dictionary<string, string> p = Util.ParseKeyValueList(param);

// n = <index-of-refraction>
double n = 1.6;
Util.TryParse(p, "n", ref n);

// Transparent sphere.
Sphere s;
s = new Sphere();
PhongMaterial pm = new PhongMaterial(new double[] {0.0, 0.2, 0.1}, 0.03, 0.03, 0.08, 128);
pm.n  = n;
pm.Kt = 0.9;
s.SetAttribute(PropertyName.MATERIAL, pm);
root.InsertChild(s, Matrix4d.Identity);

// Opaque sphere.
s = new Sphere();
root.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(1.5, 0.2, 2.4));

// Infinite plane with checker texture.
Plane pl = new Plane();
pl.SetAttribute(PropertyName.COLOR, new double[] {0.2, 0.03, 0.0});
pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.6, 0.6, new double[] {1.0, 1.0, 1.0}));
root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));
