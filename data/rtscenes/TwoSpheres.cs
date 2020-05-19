//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

// Override image resolution and supersampling.
context[PropertyName.CTX_WIDTH]         = 640; // 1800, 1920
context[PropertyName.CTX_HEIGHT]        = 480; // 1200, 1080
//context[PropertyName.CTX_SUPERSAMPLING] =   4; //  400,   64

// Tooltip (if script uses values from 'param').
context[PropertyName.CTX_TOOLTIP] = "n=<double> (index of refraction)\rschlick=<double> (Schlick coeff)\rmat={mirror|glass|diffuse}}";

// Params dictionary.
Dictionary<string, string> p = Util.ParseKeyValueList(param);

// schlick = <schlick-coeff>
double sch = 1.0;
Util.TryParse(p, "schlick", ref sch);

// n = <index-of-refraction>
double n = 1.6;
Util.TryParse(p, "n", ref n);

// mat = {mirror|glass|diffuse}
PhongMaterial pm = new PhongMaterial(new double[] {0.0, 0.2, 0.1}, 0.05, 0.05, 0.1, 128, sch);
pm.n  = n;
pm.Kt = 0.9;

if (p.TryGetValue("mat", out string mat))
  switch (mat)
  {
    case "diffuse":
      pm = new PhongMaterial(new double[] {0.1, 0.1, 0.6}, 0.1, 0.8, 0.2, 16, sch);
      break;

    case "mirror":
      pm = new PhongMaterial(new double[] {1.0, 1.0, 0.8}, 0.01, 0.0, 1.1, 8192, sch);
      break;

    default:
    case "glass":
      break;
  }

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.7, 0.1}, 0.1, 0.7, 0.3, 128, sch));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.01, 0.03};
scene.Background = new DefaultBackground(scene.BackgroundColor);

// Camera.
scene.Camera = new StaticCamera(new Vector3d(0.7, 0.5, -5.0),
                                new Vector3d(0.0, -0.18, 1.0),
                                50.0);

// Light sources.
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 4.0, -3.0), 1.2));

// --- NODE DEFINITIONS ----------------------------------------------------

// Transparent/mirror/diffuse sphere.
Sphere s;
s = new Sphere();
s.SetAttribute(PropertyName.MATERIAL, pm);
root.InsertChild(s, Matrix4d.Identity);

// Opaque sphere.
s = new Sphere();
root.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(1.5, 0.2, 2.4));

// Infinite plane with checker.
Plane pl = new Plane();
pl.SetAttribute(PropertyName.COLOR, new double[] {0.3, 0.0, 0.0});
pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.6, 0.6, new double[] {1.0, 1.0, 1.0}));
root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));
