//////////////////////////////////////////////////
// Externals.

using JosefPelikan;     // AnimatedNodeTranslate + CatmullRomAnimator

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

// Tooltip (if script uses values from 'param').
context[PropertyName.CTX_TOOLTIP] = "n=<double> (index of refraction)";

//////////////////////////////////////////////////
// CSG scene.

AnimatedCSGInnerNode root = new AnimatedCSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.8, 0.1}, 0.1, 0.6, 0.4, 128));
scene.Intersectable = root;

// Optional Animator.
string name = "translatePath";

CatmullRomAnimator pa = new CatmullRomAnimator()
{
  Start =  0.0,
  End   =  end
};
pa.newProperty(name, 0.0, end, 8.0,
               PropertyAnimator.InterpolationStyle.Cyclic,
               new List<Vector3d>() {
                 new Vector3d(0.0, 0.2,-0.5),
                 new Vector3d(1.0, 0.2,-0.5),
                 new Vector3d(1.0, 1.6, 0.0),
                 new Vector3d(0.0, 1.2, 0.0)},
               true);
scene.Animator = pa;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.01, 0.03};
scene.Background = new DefaultBackground(scene.BackgroundColor);

// Camera.
AnimatedCamera cam = new AnimatedCamera(new Vector3d(0.7,  0.1,  0.0),
                                        new Vector3d(0.7,  0.8, -8.0),
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

// Params dictionary.
Dictionary<string, string> p = Util.ParseKeyValueList(param);

// n = <index-of-refraction>
double n = 1.6;
Util.TryParse(p, "n", ref n);

ISolid s;
// Animation node.
AnimatedNodeTranslate an = new AnimatedNodeTranslate(
  name,
  new Vector3d(0.0, 0.2,-0.5),
  Matrix4d.Identity,
  0.0, 20.0);
root.InsertChild(an, Matrix4d.Identity);

// Transparent sphere.
s = new Sphere();
PhongMaterial pm = new PhongMaterial(new double[] {0.0, 0.2, 0.1}, 0.03, 0.03, 0.08, 128);
pm.n  = n;
pm.Kt = 0.9;
s.SetAttribute(PropertyName.MATERIAL, pm);
an.InsertChild(s, Matrix4d.Identity);

// Opaque sphere.
s = new Sphere();
root.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(1.5, 0.2, 2.4));

// Infinite plane with checker texture.
s = new Plane();
s.SetAttribute(PropertyName.COLOR, new double[] {0.2, 0.03, 0.0});
s.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.6, 0.6, new double[] {1.0, 1.0, 1.0}));
root.InsertChild(s, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));
