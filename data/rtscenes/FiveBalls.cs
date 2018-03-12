// CSG scene:
Rendering.CSGInnerNode root = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
root.SetAttribute(Rendering.PropertyName.REFLECTANCE_MODEL, new Rendering.PhongModel());
root.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 0.5, 0.5, 0.5 }, 0.1, 0.6, 0.3, 16));
sc.Intersectable = root;

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

// --- NODE DEFINITIONS ----------------------------------------------------

// sphere 1:
Rendering.Sphere s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 0.6, 0.0 });
root.InsertChild(s, OpenTK.Matrix4d.Identity);

// sphere 2:
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.2, 0.9, 0.5 });
root.InsertChild(s, OpenTK.Matrix4d.CreateTranslation(-2.2, 0.0, 0.0));

// sphere 3:
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.1, 0.3, 1.0 });
root.InsertChild(s, OpenTK.Matrix4d.CreateTranslation(-4.4, 0.0, 0.0));

// sphere 4:
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1.0, 0.2, 0.2 });
root.InsertChild(s, OpenTK.Matrix4d.CreateTranslation(2.2, 0.0, 0.0));

// sphere 5:
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.1, 0.4, 0.0 });
s.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(80.0, 40.0, new double[] { 1.0, 0.8, 0.2 }));
root.InsertChild(s, OpenTK.Matrix4d.CreateTranslation(4.4, 0.0, 0.0));
