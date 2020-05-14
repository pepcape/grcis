//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.6, 0.1}, 0.1, 0.6, 0.4, 16));
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

// Sphere.
Sphere s = new Sphere();
root.InsertChild(s, Matrix4d.Identity);

// Infinite plane with checker.
Plane pl = new Plane();
pl.SetAttribute(PropertyName.COLOR, new double[] {0.5, 0.0, 0.0});
pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(1.0, 1.0, new double[] { 1.0, 1.0, 1.0 }));
root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));
