//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode flags = new CSGInnerNode(SetOperation.Union);
flags.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
flags.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {0.5, 0.5, 0.5}, 0.2, 0.7, 0.1, 16));
scene.Intersectable = flags;
Sphere s;
Cube c;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.05, 0.05};

// Camera.
scene.Camera = new StaticCamera(new Vector3d(0.0, 0.0, -10.0),
                                new Vector3d(0.0, 0.0, 1.0),
                                60.0);

// Light sources.
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 3.0, -3.0), 1.0));
scene.Sources.Add(new PointLightSource(new Vector3d(5.0, 3.0, -3.0), 1.0));

// --- NODE DEFINITIONS ----------------------------------------------------

// Latvian flag (intersection, difference and xor).
CSGInnerNode latvia = new CSGInnerNode(SetOperation.Intersection);
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 });
latvia.InsertChild(c, Matrix4d.Scale(4.0, 2.0, 0.4) * Matrix4d.CreateTranslation(-2.0, -1.0, 0.0));
CSGInnerNode latviaFlag = new CSGInnerNode(SetOperation.Xor);
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 });
latviaFlag.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(4.0, 2.0, 2.0));
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
latviaFlag.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(4.2, 0.5, 0.5));
latvia.InsertChild(latviaFlag, Matrix4d.Identity);
flags.InsertChild(latvia, Matrix4d.Scale(0.7) * Matrix4d.CreateRotationX(System.Math.PI / 8) * Matrix4d.CreateTranslation(-3.5, 1.5, 0.0));

// Czech flag (difference).
CSGInnerNode czech = new CSGInnerNode(SetOperation.Difference);
s = new Sphere();
s.SetAttribute(PropertyName.COLOR, new double[] { 0.2, 0.2, 0.2 });
czech.InsertChild(s, Matrix4d.Identity);
s = new Sphere();
s.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
czech.InsertChild(s, Matrix4d.CreateTranslation(0.0, 0.8, -1.0));
s = new Sphere();
s.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 });
czech.InsertChild(s, Matrix4d.CreateTranslation(0.0, -0.8, -1.1));
s = new Sphere();
s.SetAttribute(PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
czech.InsertChild(s, Matrix4d.CreateTranslation(-1.0, 0.0, -0.8));
flags.InsertChild(czech, Matrix4d.Scale(1.6, 1.0, 1.0) * Matrix4d.CreateRotationY(System.Math.PI / 8) *
Matrix4d.CreateTranslation(4.0, -1.5, 0.0));

// Croatian flag (union, intersection).
CSGInnerNode croatianFlag = new CSGInnerNode(SetOperation.Union);
CSGInnerNode croatianSign = new CSGInnerNode(SetOperation.Intersection);
CSGInnerNode checkerBoard = new CSGInnerNode(SetOperation.Union);
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 });
c.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(10.0, 10.0, new double[] { 1.0, 1.0, 1.0 }));
croatianSign.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(2.0, 2.0, 0.4));
CSGInnerNode sign = new CSGInnerNode(SetOperation.Union);
s = new Sphere();
s.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
sign.InsertChild(s, Matrix4d.Identity);
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
sign.InsertChild(c, Matrix4d.Scale(1.8) * Matrix4d.CreateTranslation(-0.9, 0.1, -0.9));
croatianSign.InsertChild(sign, Matrix4d.Scale(0.5, 0.33, 0.5) * Matrix4d.CreateTranslation(0.44, -0.5, 0.0));
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 });
croatianFlag.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(4.0, 0.6, 0.6) *
Matrix4d.RotateX(System.Math.PI / 4) * Matrix4d.CreateTranslation(0.5, 1.0, 1.0));
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
croatianFlag.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(4.0, 0.6, 0.6) *
Matrix4d.RotateX(System.Math.PI / 4) * Matrix4d.CreateTranslation(0.5, 0.3, 1.0));
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
croatianFlag.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(4.0, 0.6, 0.6) *
Matrix4d.RotateX(System.Math.PI / 4) * Matrix4d.CreateTranslation(0.5, -0.4, 1.0));
croatianFlag.InsertChild(croatianSign, Matrix4d.Scale(0.8) * Matrix4d.CreateTranslation(0.4, 0.5, 0.0));
flags.InsertChild(croatianFlag, Matrix4d.Scale(0.8) * Matrix4d.CreateRotationY(System.Math.PI / 8) * Matrix4d.CreateTranslation(-0.4, 1.5, 1.0));

// Brazilian flag (union).
CSGInnerNode brazilianFlag = new CSGInnerNode(SetOperation.Union);
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 0.0, 0.8, 0.0 });
brazilianFlag.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(4.0, 2.0, 0.2));
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 1.0, 0.0 });
brazilianFlag.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.CreateRotationZ(System.Math.PI / 4) *
Matrix4d.Scale(2.0, 1.0, 0.4));
s = new Sphere();
s.SetAttribute(PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
brazilianFlag.InsertChild(s, Matrix4d.Scale(0.5) * Matrix4d.CreateTranslation(0.0, 0.0, 0.0));
flags.InsertChild(brazilianFlag, Matrix4d.Scale(0.9) * Matrix4d.RotateY(-System.Math.PI / 8) * Matrix4d.CreateTranslation(0.0, -1.8, 1.0));

// Finnish flag (intersection and difference).
CSGInnerNode finlandFlag = new CSGInnerNode(SetOperation.Difference);
s = new Sphere();
s.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 });
finlandFlag.InsertChild(s, Matrix4d.Scale(2.0, 1.0, 0.5) * Matrix4d.CreateTranslation(0.0, 0.0, 0.3));
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
finlandFlag.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(4.2, 0.5, 0.5));
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 });
finlandFlag.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(0.5, 4.2, 0.5) * Matrix4d.CreateTranslation(-0.5, 0.0, 0.0));
flags.InsertChild(finlandFlag, Matrix4d.Scale(0.7) * Matrix4d.CreateTranslation(3.5, 1.5, 0.0));

// Cuban flag (union and intersection).
CSGInnerNode cubanFlag = new CSGInnerNode(SetOperation.Union);
for (int i = 0; i< 5; i++)
{
  c = new Cube();
  c.SetAttribute(PropertyName.COLOR, (i % 2 == 0) ? new double[] { 0.0, 0.0, 1.0 } : new double[] { 1.0, 1.0, 1.0 });
  cubanFlag.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(4.0, 0.4, 0.4) *
  Matrix4d.CreateTranslation(new Vector3d(0.0, 0.0 - i* 0.4, 0.8 - i* 0.2)));
}
CSGInnerNode wedge = new CSGInnerNode(SetOperation.Intersection);
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 });
wedge.InsertChild(c, Matrix4d.CreateTranslation(-0.5, -0.5, -0.5) * Matrix4d.Scale(2.0, 2.0, 2.0) *
Matrix4d.CreateRotationZ(System.Math.PI / 4));
c = new Cube();
c.SetAttribute(PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 });
wedge.InsertChild(c, Matrix4d.Scale(4.0) * Matrix4d.CreateTranslation(0.0, -2.0, -2.0));
cubanFlag.InsertChild(wedge, Matrix4d.Scale(0.7) * Matrix4d.CreateTranslation(-2.0001, -0.8, 0.4999));
flags.InsertChild(cubanFlag, Matrix4d.Scale(0.7) * Matrix4d.RotateY(System.Math.PI / 8) * Matrix4d.CreateTranslation(-4.0, -1.0, 0.0));
