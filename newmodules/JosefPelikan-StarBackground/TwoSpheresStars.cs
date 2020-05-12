using JosefPelikan;     // for StarBackground

//////////////////////////////////////////////////
// Preprocessing stage support.

bool preprocessing = false;

// Params dictionary.
Dictionary<string, string> p = Util.ParseKeyValueList(param);

// n = <index-of-refraction>
double n = 1.6;
Util.TryParse(p, "n", ref n);

// mat = {mirror|glass|diffuse}
PhongMaterial pm = new PhongMaterial(new double[] {0.0, 0.2, 0.1}, 0.05, 0.05, 0.1, 128);
pm.n = n;
pm.Kt = 0.9;

if (p.TryGetValue("mat", out string mat))
  switch (mat)
  {
    case "diffuse":
      pm = new PhongMaterial(new double[] { 0.1, 0.1, 0.6 }, 0.1, 0.8, 0.2, 16);
      break;

    case "mirror":
      pm = new PhongMaterial(new double[] {1.0, 1.0, 0.8}, 0.01, 0.0, 1.1, 8192);
      break;

    default:
    case "glass":
      break;
  }

if (context != null)
{
  // Let renderer application know required parameters soon..
  context[PropertyName.CTX_WIDTH]         = 640;
  context[PropertyName.CTX_HEIGHT]        = 480;
  context[PropertyName.CTX_SUPERSAMPLING] =  16;
  //context[PropertyName.CTX_WIDTH]         = 1800;
  //context[PropertyName.CTX_HEIGHT]        = 1200;
  //context[PropertyName.CTX_SUPERSAMPLING] =  400;

  // context["ToolTip"] indicates whether the script is running for the first time (preprocessing) or for regular rendering.
  preprocessing = !context.ContainsKey(PropertyName.CTX_TOOLTIP);
  if (preprocessing)
  {
    context[PropertyName.CTX_TOOLTIP] = "n=<double> (index of refraction)\rmat={mirror|glass|diffuse}}";
    return;
  }

}

if (scene.BackgroundColor != null)
  return;    // scene can be shared!

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.7, 0.1}, 0.1, 0.85, 0.05, 64));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.01, 0.03};
scene.Background = new StarBackground(scene.BackgroundColor, 600, 0.006, 0.5, 1.6, 1.0);

// Camera.
scene.Camera = new StaticCamera(new Vector3d(0.7, 0.5, -5.0),
                                new Vector3d(0.0, 0.18, 1.0),
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
