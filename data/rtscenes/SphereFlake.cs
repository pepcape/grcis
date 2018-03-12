// CSG scene:
Rendering.CSGInnerNode root = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
root.SetAttribute(Rendering.PropertyName.REFLECTANCE_MODEL, new Rendering.PhongModel());
root.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.6, 0.4, 128));
sc.Intersectable = root;

// Background color:
sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

// Camera:
sc.Camera = new Rendering.StaticCamera(new OpenTK.Vector3d(1.6, 2.2, -7.0),
new OpenTK.Vector3d(-0.12, -0.3, 1.0),
50.0);

// Light sources:
sc.Sources = new System.Collections.Generic.LinkedList<Rendering.ILightSource>();
sc.Sources.Add(new Rendering.AmbientLightSource(0.8));
sc.Sources.Add(new Rendering.PointLightSource(new OpenTK.Vector3d(-5.0, 4.0, -3.0), 1.2));

// --- NODE DEFINITIONS ----------------------------------------------------

// Params dictionary:
System.Collections.Generic.Dictionary<string, string> p = Utilities.Util.ParseKeyValueList(param);

// depth = <depth>
int depth = 5;
Utilities.Util.TryParse(p, "depth", ref depth);

// n = <index-of-refraction>
double n = 1.6;
Utilities.Util.TryParse(p, "n", ref n);

// coef = <scale-coef>
double coef = 0.4;
Utilities.Util.TryParse(p, "coef", ref coef);

// mat = {mirror|glass|diffuse}
Rendering.PhongMaterial pm = new Rendering.PhongMaterial(new double[] { 1.0, 0.6, 0.1 }, 0.1, 0.85, 0.05, 16);
string mat;
if (p.TryGetValue("mat", out mat))
switch (mat)
{
case "mirror":
pm = new Rendering.PhongMaterial(new double[] { 1.0, 1.0, 0.8 }, 0.0, 0.1, 0.9, 128);
break;

case "glass":
pm = new Rendering.PhongMaterial(new double[] { 0.0, 0.2, 0.1 }, 0.05, 0.05, 0.1, 128);
pm.n = n;
pm.Kt = 0.9;
break;
}

// Sphere flake - recursive definition:
Rendering.ISceneNode sf = Rendering.Scenes.flake(depth, coef);
sf.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
root.InsertChild(sf, OpenTK.Matrix4d.Identity);

// Infinite plane with checker:
Rendering.Plane pl = new Rendering.Plane();
pl.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 });
pl.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(0.6, 0.6, new double[] { 1.0, 1.0, 1.0 }));
root.InsertChild(pl, OpenTK.Matrix4d.RotateX(-OpenTK.MathHelper.PiOver2) * OpenTK.Matrix4d.CreateTranslation(0.0, -1.0, 0.0));