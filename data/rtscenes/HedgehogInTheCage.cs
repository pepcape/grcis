//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {0.5, 0.5, 0.5}, 0.2, 0.6, 0.2, 16));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.05, 0.05};

// Camera.
scene.Camera = new StaticCamera(new Vector3d(0.0, 2.0, -7.0),
                                new Vector3d(0.0, -0.32, 1.0),
                                40.0);

// Light sources.
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-8.0, 5.0, -3.0), 1.0));

// --- NODE DEFINITIONS ----------------------------------------------------

// cage
CSGInnerNode cage = new CSGInnerNode(SetOperation.Difference);
cage.SetAttribute(PropertyName.COLOR, new double[] {0.70, 0.93, 0.20});

// cylinder1
CSGInnerNode cylinder1 = new CSGInnerNode(SetOperation.Intersection);
Sphere s = new Sphere();
cylinder1.InsertChild(s, Matrix4d.Scale(1.0, 1000.0, 1.0));
s = new Sphere();
cylinder1.InsertChild(s, Matrix4d.Scale(1000.0, 1.5, 1000.0));
cage.InsertChild(cylinder1, Matrix4d.Identity);

// cylinder2
CSGInnerNode cylinder2 = new CSGInnerNode(SetOperation.Intersection);
s = new Sphere();
cylinder2.InsertChild(s, Matrix4d.Scale(1.0, 1000.0, 1.0));
s = new Sphere();
cylinder2.InsertChild(s, Matrix4d.Scale(1000.0, 1.5, 1000.0));
cage.InsertChild(cylinder2, Matrix4d.Scale(0.9));

// holeUpDown
Sphere holeUpDown = new Sphere();
cage.InsertChild(holeUpDown, Matrix4d.Scale(0.5, 1000.0, 0.5));

// hole1
CSGInnerNode hole1 = new CSGInnerNode(SetOperation.Intersection);
s = new Sphere();
hole1.InsertChild(s, Matrix4d.Scale(1000.0, 1.1, 1000.0));
s = new Sphere();
hole1.InsertChild(s, Matrix4d.Scale(0.4, 1000.0, 1000.0));
cage.InsertChild(hole1, Matrix4d.Identity);

// hole2
CSGInnerNode hole2 = new CSGInnerNode(SetOperation.Intersection);
s = new Sphere();
hole2.InsertChild(s, Matrix4d.Scale(1000.0, 1.1, 1000.0));
s = new Sphere();
hole2.InsertChild(s, Matrix4d.Scale(0.4, 1000.0, 1000.0));
cage.InsertChild(hole2, Matrix4d.RotateY(System.Math.PI / 3));

// hole3
CSGInnerNode hole3 = new CSGInnerNode(SetOperation.Intersection);
s = new Sphere();
hole3.InsertChild(s, Matrix4d.Scale(1000.0, 1.1, 1000.0));
s = new Sphere();
hole3.InsertChild(s, Matrix4d.Scale(0.4, 1000.0, 1000.0));
cage.InsertChild(hole3, Matrix4d.RotateY(System.Math.PI / -3));

// hedgehog
CSGInnerNode hedgehog = new CSGInnerNode(SetOperation.Union);
hedgehog.SetAttribute(PropertyName.COLOR, new double[] { 0.4, 0.05, 0.05 });

s = new Sphere();
hedgehog.InsertChild(s, Matrix4d.Scale(0.48));

// spine1
CSGInnerNode spine1 = new CSGInnerNode(SetOperation.Intersection);
s = new Sphere();
spine1.InsertChild(s, Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Sphere();
spine1.InsertChild(s, Matrix4d.Scale(1.2));
hedgehog.InsertChild(spine1,Matrix4d.Identity);

// spine2
CSGInnerNode spine2 = new CSGInnerNode(SetOperation.Intersection);
s = new Sphere();
spine2.InsertChild(s, Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Sphere();
spine2.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(0.0, 0.1, 0.0));
hedgehog.InsertChild(spine2, Matrix4d.RotateX(System.Math.PI / -3));

// spine3
CSGInnerNode spine3 = new CSGInnerNode(SetOperation.Intersection);
s = new Sphere();
spine3.InsertChild(s, Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Sphere();
spine3.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(0.0, -0.25, 0.0));
hedgehog.InsertChild(spine3, Matrix4d.RotateX(System.Math.PI / -3) * Matrix4d.RotateY(System.Math.PI* -0.4));

// spine4
CSGInnerNode spine4 = new CSGInnerNode(SetOperation.Intersection);
s = new Sphere();
spine4.InsertChild(s, Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Sphere();
spine4.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(0.0, 0.2, 0.0));
hedgehog.InsertChild(spine4, Matrix4d.RotateX(System.Math.PI / -3) * Matrix4d.RotateY(System.Math.PI* -0.8));

// spine5
CSGInnerNode spine5 = new CSGInnerNode(SetOperation.Intersection);
s = new Sphere();
spine5.InsertChild(s, Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Sphere();
spine5.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(0.0, -0.2, 0.0));
hedgehog.InsertChild(spine5, Matrix4d.RotateX(System.Math.PI / -3) * Matrix4d.RotateY(System.Math.PI* 0.4));

// spine6
CSGInnerNode spine6 = new CSGInnerNode(SetOperation.Intersection);
s = new Sphere();
spine6.InsertChild(s, Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Sphere();
spine6.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(0.0, -0.25, 0.0));
hedgehog.InsertChild(spine6, Matrix4d.RotateX(System.Math.PI / -3) * Matrix4d.RotateY(System.Math.PI* 0.8));

// all
CSGInnerNode all = new CSGInnerNode(SetOperation.Union);
all.InsertChild(cage, Matrix4d.RotateY(0.25));
all.InsertChild(hedgehog, Matrix4d.Rotate(new Vector3d(0.0, 1.0, 1.0), 0.1) * Matrix4d.CreateTranslation(0.0, -0.1, 0.0));

root.InsertChild(all, Matrix4d.RotateZ(0.1));
