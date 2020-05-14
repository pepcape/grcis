//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.8, 0.1}, 0.1, 0.5, 0.5, 64));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.05, 0.07};

// Camera.
scene.Camera = new StaticCamera(new Vector3d(0.7, 0.5, -5.0),
                                new Vector3d(0.0, -0.18, 1.0),
                                50.0);

// Light sources.
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 3.0, -3.0), 1.0));

// --- NODE DEFINITIONS ----------------------------------------------------

// Bezier patch.
BezierSurface b = new BezierSurface(1, 2, new double[] {
0.0, 0.0, 3.0,  // row 0
1.0, 0.0, 3.0,
2.0, 0.0, 3.0,
3.0, 0.0, 3.0,
4.0, 0.0, 3.0,
5.0, 0.0, 3.0,
6.0, 0.0, 3.0,
0.0, 0.0, 2.0,  // row 1
1.0, 0.0, 2.0,
2.0, 3.0, 2.0,
3.0, 3.0, 2.0,
4.0, 3.0, 2.0,
5.0, 0.0, 2.0,
6.0, 0.0, 2.0,
0.0, 0.0, 1.0,  // row 2
1.0, 0.0, 1.0,
2.0, 0.0, 1.0,
3.0, 1.5, 1.0,
4.0, 3.0, 1.0,
5.0, 0.0, 1.0,
6.0, 0.0, 1.0,
0.0, 0.0, 0.0,  // row 3
1.0, 0.0, 0.0,
2.0, 0.0, 0.0,
3.0, 0.0, 0.0,
4.0, 0.0, 0.0,
5.0, 0.0, 0.0,
6.0, 0.0, 0.0,
});
b.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(10.5, 12.0, new double[] {0.0, 0.0, 0.1}));
root.InsertChild(b, Matrix4d.RotateY(-0.4) * Matrix4d.CreateTranslation(-1.1, -0.9, 0.0));

// Cylinders for reflections..
Cylinder c = new Cylinder();
c.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {0.0, 0.6, 0.0}, 0.2, 0.6, 0.3, 8));
root.InsertChild(c, Matrix4d.Scale(0.15) * Matrix4d.RotateX(MathHelper.PiOver2) * Matrix4d.CreateTranslation(-0.4, 0.0, 0.0));
c = new Cylinder();
c.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {0.8, 0.2, 0.0}, 0.2, 0.6, 0.3, 8));
root.InsertChild(c, Matrix4d.Scale(0.2) * Matrix4d.RotateX(MathHelper.PiOver2) * Matrix4d.CreateTranslation(-1.9, 0.0, 3.0));

// Infinite plane with checker.
Plane pl = new Plane();
pl.SetAttribute(PropertyName.COLOR, new double[] {0.3, 0.0, 0.0});
pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.6, 0.6, new double[] {1.0, 1.0, 1.0}));
root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));
