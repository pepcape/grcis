// CSG scene:
CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.6, 0.4, 128));
scene.Intersectable = root;

// Background color:
scene.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

// Camera:
scene.Camera = new StaticCamera(new Vector3d(0.7, 0.5, -5.0),
                                new Vector3d(0.0, -0.18, 1.0),
                                50.0);

// Light sources:
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 4.0, -3.0), 1.2));

// --- NODE DEFINITIONS ----------------------------------------------------

// Params dictionary:
Dictionary<string, string> p = Util.ParseKeyValueList(param);

// n = <index-of-refraction>
double n = 1.6;
Util.TryParse(p, "n", ref n);

// rc = <ray-casting-mode>
bool rc = false;
Util.TryParse(p, "rc", ref rc);

// Glass:
PhongMaterial glass = new PhongMaterial(new double[] { 0.0, 0.2, 0.1 }, 0.05, 0.05, 0.1, 128);
glass.n = n;
glass.Kt = 0.9;
if (rc)
  glass = new PhongMaterial(new double[] { 0.1, 0.3, 1.0 }, 0.1, 0.8, 0.1, 32);

// Transparent sphere:
Sphere s = new Sphere();
s.SetAttribute(PropertyName.MATERIAL, glass);

// Sphere with a hole:
CSGInnerNode sub = new CSGInnerNode(SetOperation.Difference);
root.InsertChild(sub, Matrix4d.Identity);
sub.InsertChild(s, Matrix4d.Identity);

Cube cu = new Cube();
cu.SetAttribute(PropertyName.MATERIAL, glass);
sub.InsertChild(cu, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) *
                    Matrix4d.Scale(0.6, 0.6, 4.0) *
                    Matrix4d.CreateRotationY(-0.3));

// Opaque sphere:
s = new Sphere();
if (!rc)
  s.SetAttribute(PropertyName.MATERIAL, glass);
root.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(1.5, 0.2, 2.4));

// Lentil:
CSGInnerNode lentil = new CSGInnerNode(SetOperation.Intersection);

s = new Sphere();
s.SetAttribute(PropertyName.MATERIAL, glass);
lentil.InsertChild(s, Matrix4d.Identity);
s = new Sphere();
s.SetAttribute(PropertyName.MATERIAL, glass);
lentil.InsertChild(s, Matrix4d.CreateTranslation(0.0, 0.0, -1.85));

root.InsertChild(lentil, Matrix4d.CreateTranslation(1.1, -0.55, -1.0));

// Infinite plane with checker:
Plane pl = new Plane();
pl.SetAttribute(PropertyName.COLOR, new double[] { 0.5, 0.0, 0.0 });
pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(6, 6, new double[] { 1.0, 1.0, 1.0 }));
root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));
