// CSG scene:
Rendering.CSGInnerNode root = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
root.SetAttribute(Rendering.PropertyName.REFLECTANCE_MODEL, new Rendering.PhongModel());
root.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 0.5, 0.5, 0.5 }, 0.2, 0.6, 0.2, 16));
sc.Intersectable = root;

// Background color:
sc.BackgroundColor = new double[] { 0.0, 0.05, 0.05 };

// Camera:
sc.Camera = new Rendering.StaticCamera(new OpenTK.Vector3d(0.0, 2.0, -7.0),
new OpenTK.Vector3d(0.0, -0.32, 1.0),
40.0);

// Light sources:
sc.Sources = new System.Collections.Generic.LinkedList<Rendering.ILightSource>();
sc.Sources.Add(new Rendering.AmbientLightSource(0.8));
sc.Sources.Add(new Rendering.PointLightSource(new OpenTK.Vector3d(-8.0, 5.0, -3.0), 1.0));

// --- NODE DEFINITIONS ----------------------------------------------------

// cage
Rendering.CSGInnerNode cage = new Rendering.CSGInnerNode(Rendering.SetOperation.Difference);
cage.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.70, 0.93, 0.20 });

// cylinder1
Rendering.CSGInnerNode cylinder1 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
Rendering.Sphere s = new Rendering.Sphere();
cylinder1.InsertChild(s, OpenTK.Matrix4d.Scale(1.0, 1000.0, 1.0));
s = new Rendering.Sphere();
cylinder1.InsertChild(s, OpenTK.Matrix4d.Scale(1000.0, 1.5, 1000.0));
cage.InsertChild(cylinder1, OpenTK.Matrix4d.Identity);

// cylinder2
Rendering.CSGInnerNode cylinder2 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
s = new Rendering.Sphere();
cylinder2.InsertChild(s, OpenTK.Matrix4d.Scale(1.0, 1000.0, 1.0));
s = new Rendering.Sphere();
cylinder2.InsertChild(s, OpenTK.Matrix4d.Scale(1000.0, 1.5, 1000.0));
cage.InsertChild(cylinder2, OpenTK.Matrix4d.Scale(0.9));

// holeUpDown
Rendering.Sphere holeUpDown = new Rendering.Sphere();
cage.InsertChild(holeUpDown, OpenTK.Matrix4d.Scale(0.5, 1000.0, 0.5));

// hole1
Rendering.CSGInnerNode hole1 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
s = new Rendering.Sphere();
hole1.InsertChild(s, OpenTK.Matrix4d.Scale(1000.0, 1.1, 1000.0));
s = new Rendering.Sphere();
hole1.InsertChild(s, OpenTK.Matrix4d.Scale(0.4, 1000.0, 1000.0));
cage.InsertChild(hole1, OpenTK.Matrix4d.Identity);

// hole2
Rendering.CSGInnerNode hole2 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
s = new Rendering.Sphere();
hole2.InsertChild(s, OpenTK.Matrix4d.Scale(1000.0, 1.1, 1000.0));
s = new Rendering.Sphere();
hole2.InsertChild(s, OpenTK.Matrix4d.Scale(0.4, 1000.0, 1000.0));
cage.InsertChild(hole2, OpenTK.Matrix4d.RotateY(System.Math.PI / 3));

// hole3
Rendering.CSGInnerNode hole3 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
s = new Rendering.Sphere();
hole3.InsertChild(s, OpenTK.Matrix4d.Scale(1000.0, 1.1, 1000.0));
s = new Rendering.Sphere();
hole3.InsertChild(s, OpenTK.Matrix4d.Scale(0.4, 1000.0, 1000.0));
cage.InsertChild(hole3, OpenTK.Matrix4d.RotateY(System.Math.PI / -3));

// hedgehog
Rendering.CSGInnerNode hedgehog = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
hedgehog.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.4, 0.05, 0.05 });

s = new Rendering.Sphere();
hedgehog.InsertChild(s, OpenTK.Matrix4d.Scale(0.48));

// spine1
Rendering.CSGInnerNode spine1 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
s = new Rendering.Sphere();
spine1.InsertChild(s, OpenTK.Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Rendering.Sphere();
spine1.InsertChild(s, OpenTK.Matrix4d.Scale(1.2));
hedgehog.InsertChild(spine1,OpenTK.Matrix4d.Identity);

// spine2
Rendering.CSGInnerNode spine2 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
s = new Rendering.Sphere();
spine2.InsertChild(s, OpenTK.Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Rendering.Sphere();
spine2.InsertChild(s, OpenTK.Matrix4d.Scale(1.2) * OpenTK.Matrix4d.CreateTranslation(0.0, 0.1, 0.0));
hedgehog.InsertChild(spine2, OpenTK.Matrix4d.RotateX(System.Math.PI / -3));

// spine3
Rendering.CSGInnerNode spine3 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
s = new Rendering.Sphere();
spine3.InsertChild(s, OpenTK.Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Rendering.Sphere();
spine3.InsertChild(s, OpenTK.Matrix4d.Scale(1.2) * OpenTK.Matrix4d.CreateTranslation(0.0, -0.25, 0.0));
hedgehog.InsertChild(spine3, OpenTK.Matrix4d.RotateX(System.Math.PI / -3) * OpenTK.Matrix4d.RotateY(System.Math.PI* -0.4));

// spine4
Rendering.CSGInnerNode spine4 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
s = new Rendering.Sphere();
spine4.InsertChild(s, OpenTK.Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Rendering.Sphere();
spine4.InsertChild(s, OpenTK.Matrix4d.Scale(1.2) * OpenTK.Matrix4d.CreateTranslation(0.0, 0.2, 0.0));
hedgehog.InsertChild(spine4, OpenTK.Matrix4d.RotateX(System.Math.PI / -3) * OpenTK.Matrix4d.RotateY(System.Math.PI* -0.8));

// spine5
Rendering.CSGInnerNode spine5 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
s = new Rendering.Sphere();
spine5.InsertChild(s, OpenTK.Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Rendering.Sphere();
spine5.InsertChild(s, OpenTK.Matrix4d.Scale(1.2) * OpenTK.Matrix4d.CreateTranslation(0.0, -0.2, 0.0));
hedgehog.InsertChild(spine5, OpenTK.Matrix4d.RotateX(System.Math.PI / -3) * OpenTK.Matrix4d.RotateY(System.Math.PI* 0.4));

// spine6
Rendering.CSGInnerNode spine6 = new Rendering.CSGInnerNode(Rendering.SetOperation.Intersection);
s = new Rendering.Sphere();
spine6.InsertChild(s, OpenTK.Matrix4d.Scale(0.06, 1000.0, 0.06));
s = new Rendering.Sphere();
spine6.InsertChild(s, OpenTK.Matrix4d.Scale(1.2) * OpenTK.Matrix4d.CreateTranslation(0.0, -0.25, 0.0));
hedgehog.InsertChild(spine6, OpenTK.Matrix4d.RotateX(System.Math.PI / -3) * OpenTK.Matrix4d.RotateY(System.Math.PI* 0.8));

// all
Rendering.CSGInnerNode all = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
all.InsertChild(cage, OpenTK.Matrix4d.RotateY(0.25));
all.InsertChild(hedgehog, OpenTK.Matrix4d.Rotate(new OpenTK.Vector3d(0.0, 1.0, 1.0), 0.1) * OpenTK.Matrix4d.CreateTranslation(0.0, -0.1, 0.0));

root.InsertChild(all, OpenTK.Matrix4d.RotateZ(0.1));