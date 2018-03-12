// CSG scene:
Rendering.CSGInnerNode flags = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
flags.SetAttribute(Rendering.PropertyName.REFLECTANCE_MODEL, new Rendering.PhongModel());
flags.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 0.5, 0.5, 0.5 }, 0.2, 0.7, 0.1, 16));
sc.Intersectable = flags;
Rendering.Sphere s;
Rendering.Cube c;

// Background color:
sc.BackgroundColor = new double[] { 0.0, 0.05, 0.05 };

// Camera:
sc.Camera = new Rendering.StaticCamera(new OpenTK.Vector3d(0.0, 0.0, -10.0),
new OpenTK.Vector3d(0.0, 0.0, 1.0),
60.0);

// Light sources:
sc.Sources = new System.Collections.Generic.LinkedList<Rendering.ILightSource>();
sc.Sources.Add(new Rendering.AmbientLightSource(0.8));
sc.Sources.Add(new Rendering.PointLightSource(new OpenTK.Vector3d(-5.0, 3.0, -3.0), 1.0));
sc.Sources.Add(new Rendering.PointLightSource(new OpenTK.Vector3d(5.0, 3.0, -3.0), 1.0));

// --- NODE DEFINITIONS ----------------------------------------------------

// Latvian flag (intersection, difference and xor):
Rendering.CSGInnerNode latvia = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 });
latvia.InsertChild(c, OpenTK.Matrix4d.Scale(4.0, 2.0, 0.4) * OpenTK.Matrix4d.CreateTranslation(-2.0, -1.0, 0.0));
Rendering.CSGInnerNode latviaFlag = new Rendering.CSGInnerNode(Rendering.SetOperation.Xor);
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 });
latviaFlag.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(4.0, 2.0, 2.0));
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
latviaFlag.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(4.2, 0.5, 0.5));
latvia.InsertChild(latviaFlag, OpenTK.Matrix4d.Identity);
flags.InsertChild(latvia, OpenTK.Matrix4d.Scale(0.7) * OpenTK.Matrix4d.CreateRotationX(System.Math.PI / 8) * OpenTK.Matrix4d.CreateTranslation(-3.5, 1.5, 0.0));

// Czech flag (difference):
Rendering.CSGInnerNode czech = new Rendering.CSGInnerNode(Rendering.SetOperation.Difference);
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.2, 0.2, 0.2 });
czech.InsertChild(s, OpenTK.Matrix4d.Identity);
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
czech.InsertChild(s, OpenTK.Matrix4d.CreateTranslation(0.0, 0.8, -1.0));
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 });
czech.InsertChild(s, OpenTK.Matrix4d.CreateTranslation(0.0, -0.8, -1.1));
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
czech.InsertChild(s, OpenTK.Matrix4d.CreateTranslation(-1.0, 0.0, -0.8));
flags.InsertChild(czech, OpenTK.Matrix4d.Scale(1.6, 1.0, 1.0) * OpenTK.Matrix4d.CreateRotationY(System.Math.PI / 8) *
OpenTK.Matrix4d.CreateTranslation(4.0, -1.5, 0.0));

// Croatian flag (union, intersection):
Rendering.CSGInnerNode croatianFlag = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
Rendering.CSGInnerNode croatianSign = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
Rendering.CSGInnerNode checkerBoard = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 });
c.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(10.0, 10.0, new double[] { 1.0, 1.0, 1.0 }));
croatianSign.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(2.0, 2.0, 0.4));
Rendering.CSGInnerNode sign = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
sign.InsertChild(s, OpenTK.Matrix4d.Identity);
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
sign.InsertChild(c, OpenTK.Matrix4d.Scale(1.8) * OpenTK.Matrix4d.CreateTranslation(-0.9, 0.1, -0.9));
croatianSign.InsertChild(sign, OpenTK.Matrix4d.Scale(0.5, 0.33, 0.5) * OpenTK.Matrix4d.CreateTranslation(0.44, -0.5, 0.0));
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 });
croatianFlag.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(4.0, 0.6, 0.6) *
OpenTK.Matrix4d.RotateX(System.Math.PI / 4) * OpenTK.Matrix4d.CreateTranslation(0.5, 1.0, 1.0));
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
croatianFlag.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(4.0, 0.6, 0.6) *
OpenTK.Matrix4d.RotateX(System.Math.PI / 4) * OpenTK.Matrix4d.CreateTranslation(0.5, 0.3, 1.0));
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
croatianFlag.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(4.0, 0.6, 0.6) *
OpenTK.Matrix4d.RotateX(System.Math.PI / 4) * OpenTK.Matrix4d.CreateTranslation(0.5, -0.4, 1.0));
croatianFlag.InsertChild(croatianSign, OpenTK.Matrix4d.Scale(0.8) * OpenTK.Matrix4d.CreateTranslation(0.4, 0.5, 0.0));
flags.InsertChild(croatianFlag, OpenTK.Matrix4d.Scale(0.8) * OpenTK.Matrix4d.CreateRotationY(System.Math.PI / 8) * OpenTK.Matrix4d.CreateTranslation(-0.4, 1.5, 1.0));

// Brazilian flag (union):
Rendering.CSGInnerNode brazilianFlag = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.0, 0.8, 0.0 });
brazilianFlag.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(4.0, 2.0, 0.2));
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 1.0, 0.0 });
brazilianFlag.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.CreateRotationZ(System.Math.PI / 4) *
OpenTK.Matrix4d.Scale(2.0, 1.0, 0.4));
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
brazilianFlag.InsertChild(s, OpenTK.Matrix4d.Scale(0.5) * OpenTK.Matrix4d.CreateTranslation(0.0, 0.0, 0.0));
flags.InsertChild(brazilianFlag, OpenTK.Matrix4d.Scale(0.9) * OpenTK.Matrix4d.RotateY(-System.Math.PI / 8) * OpenTK.Matrix4d.CreateTranslation(0.0, -1.8, 1.0));

// Finnish flag (intersection and difference):
Rendering.CSGInnerNode finlandFlag = new Rendering.CSGInnerNode(Rendering.SetOperation.Difference);
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
finlandFlag.InsertChild(s, OpenTK.Matrix4d.Scale(2.0, 1.0, 0.5) * OpenTK.Matrix4d.CreateTranslation(0.0, 0.0, 0.3));
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
finlandFlag.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(4.2, 0.5, 0.5));
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
finlandFlag.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(0.5, 4.2, 0.5) * OpenTK.Matrix4d.CreateTranslation(-0.5, 0.0, 0.0));
flags.InsertChild(finlandFlag, OpenTK.Matrix4d.Scale(0.7) * OpenTK.Matrix4d.CreateTranslation(3.5, 1.5, 0.0));

// Cuban flag (union and intersection):
Rendering.CSGInnerNode cubanFlag = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
for (int i = 0; i< 5; i++)
{
c = new Rendering.Cube();
if (i % 2 == 0)
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
else
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
cubanFlag.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(4.0, 0.4, 0.4) *
OpenTK.Matrix4d.CreateTranslation(new OpenTK.Vector3d(0.0, 0.0 - i* 0.4, 0.8 - i* 0.2)));
}
Rendering.CSGInnerNode wedge = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 });
wedge.InsertChild(c, OpenTK.Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * OpenTK.Matrix4d.Scale(2.0, 2.0, 2.0) *
OpenTK.Matrix4d.CreateRotationZ(System.Math.PI / 4));
c = new Rendering.Cube();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 });
wedge.InsertChild(c, OpenTK.Matrix4d.Scale(4.0) * OpenTK.Matrix4d.CreateTranslation(0.0, -2.0, -2.0));
cubanFlag.InsertChild(wedge, OpenTK.Matrix4d.Scale(0.7) * OpenTK.Matrix4d.CreateTranslation(-2.0001, -0.8, 0.4999));
flags.InsertChild(cubanFlag, OpenTK.Matrix4d.Scale(0.7) * OpenTK.Matrix4d.RotateY(System.Math.PI / 8) * OpenTK.Matrix4d.CreateTranslation(-4.0, -1.0, 0.0));