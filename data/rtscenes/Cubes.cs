// CSG scene:
Rendering.CSGInnerNode root = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
root.SetAttribute(Rendering.PropertyName.REFLECTANCE_MODEL, new Rendering.PhongModel());
root.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 1.0, 0.6, 0.1 }, 0.1, 0.8, 0.2, 16));
sc.Intersectable = root;

// Background color:
sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

// Camera:
sc.Camera = new Rendering.StaticCamera(new OpenTK.Vector3d(0.7, 3.0, -10.0),
new OpenTK.Vector3d(0.0, -0.3, 1.0),
50.0);

// Light sources:
sc.Sources = new System.Collections.Generic.LinkedList<Rendering.ILightSource>();
sc.Sources.Add(new Rendering.AmbientLightSource(0.8));
sc.Sources.Add(new Rendering.PointLightSource(new OpenTK.Vector3d(-5.0, 3.0, -3.0), 1.0));

// --- NODE DEFINITIONS ----------------------------------------------------

// Params dictionary:
System.Collections.Generic.Dictionary<string, string> p = Utilities.Util.ParseKeyValueList(param);

// n = <index-of-refraction>
double n = 1.6;
Utilities.Util.TryParse(p, "n", ref n);

// mat = {mirror|glass|diffuse}
Rendering.PhongMaterial pm = new Rendering.PhongMaterial(new double[] { 1.0, 0.6, 0.1 }, 0.1, 0.8, 0.2, 16);
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

// Base plane
Rendering.Plane pl = new Rendering.Plane();
pl.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.6, 0.0, 0.0 });
pl.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(0.5, 0.5, new double[] { 1.0, 1.0, 1.0 }));
root.InsertChild(pl, OpenTK.Matrix4d.RotateX(-OpenTK.MathHelper.PiOver2) * OpenTK.Matrix4d.CreateTranslation(0.0, -1.0, 0.0));

// Cubes
Rendering.Cube c;
// front row:
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateY(0.6) * OpenTK.Matrix4d.CreateTranslation(-3.5, -0.8, 0.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateY(1.2) * OpenTK.Matrix4d.CreateTranslation(-1.5, -0.8, 0.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateY(1.8) * OpenTK.Matrix4d.CreateTranslation(0.5, -0.8, 0.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateY(2.4) * OpenTK.Matrix4d.CreateTranslation(2.5, -0.8, 0.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateY(3.0) * OpenTK.Matrix4d.CreateTranslation(4.5, -0.8, 0.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
// back row:
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateX(3.5) * OpenTK.Matrix4d.CreateTranslation(-4.0, 1.0, 2.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateX(3.0) * OpenTK.Matrix4d.CreateTranslation(-2.5, 1.0, 2.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateX(2.5) * OpenTK.Matrix4d.CreateTranslation(-1.0, 1.0, 2.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateX(2.0) * OpenTK.Matrix4d.CreateTranslation(0.5, 1.0, 2.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateX(1.5) * OpenTK.Matrix4d.CreateTranslation(2.0, 1.0, 2.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateX(1.0) * OpenTK.Matrix4d.CreateTranslation(3.5, 1.0, 2.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);
c = new Rendering.Cube();
root.InsertChild(c, OpenTK.Matrix4d.RotateX(0.5) * OpenTK.Matrix4d.CreateTranslation(5.0, 1.0, 2.0));
c.SetAttribute(Rendering.PropertyName.MATERIAL, pm);