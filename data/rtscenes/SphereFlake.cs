//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

// Tooltip (if script uses values from 'param').
context[PropertyName.CTX_TOOLTIP] = "n=<double> (index of refraction)\rmat={mirror|glass}\rdepth=<int> (recursion depth)\rcoef=<double> (scale coefficient)";

// Params dictionary.
Dictionary<string, string> p = Util.ParseKeyValueList(param);

// depth = <depth>
int depth = 5;
Util.TryParse(p, "depth", ref depth);

// n = <index-of-refraction>
double n = 1.6;
Util.TryParse(p, "n", ref n);

// coef = <scale-coef>
double coef = 0.4;
Util.TryParse(p, "coef", ref coef);

// mat = {mirror|glass|diffuse}
PhongMaterial pm = new PhongMaterial(new double[] {1.0, 0.6, 0.1}, 0.1, 0.85, 0.05, 16);
string mat;
if (p.TryGetValue("mat", out mat))
  switch (mat)
  {
    case "mirror":
      pm = new PhongMaterial(new double[] {1.0, 1.0, 0.8}, 0.0, 0.1, 0.9, 128);
      break;

    case "glass":
      pm = new PhongMaterial(new double[] {0.0, 0.2, 0.1}, 0.05, 0.05, 0.1, 128);
      pm.n = n;
      pm.Kt = 0.9;
      break;
  }

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.8, 0.1}, 0.1, 0.6, 0.4, 128));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.05, 0.07};

// Camera.
scene.Camera = new StaticCamera(new Vector3d(1.6, 2.2, -7.0),
                                new Vector3d(-0.12, -0.3, 1.0),
                                50.0);

// Light sources.
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 4.0, -3.0), 1.2));

// --- NODE DEFINITIONS ----------------------------------------------------

// Sphere flake - recursive definition.
ISceneNode sf = Scenes.flake(depth, coef);
sf.SetAttribute(PropertyName.MATERIAL, pm);
root.InsertChild(sf, Matrix4d.Identity);

// Infinite plane with checker.
Plane pl = new Plane();
pl.SetAttribute(PropertyName.COLOR, new double[] {0.3, 0.0, 0.0});
pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.6, 0.6, new double[] {1.0, 1.0, 1.0}));
root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));
