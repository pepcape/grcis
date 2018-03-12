// CSG scene:
Rendering.CSGInnerNode root = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
root.SetAttribute(Rendering.PropertyName.REFLECTANCE_MODEL, new Rendering.PhongModel());
root.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 0.6, 0.0, 0.0 }, 0.15, 0.8, 0.15, 16));
sc.Intersectable = root;

// Background color:
sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

// Camera:
sc.Camera = new Rendering.StaticCamera(new OpenTK.Vector3d(0.7, 3.0, -10.0),
new OpenTK.Vector3d(0.0, -0.2, 1.0),
50.0);

// Light sources:
sc.Sources = new System.Collections.Generic.LinkedList<Rendering.ILightSource>();
sc.Sources.Add(new Rendering.AmbientLightSource(1.0));
sc.Sources.Add(new Rendering.PointLightSource(new OpenTK.Vector3d(-5.0, 3.0, -3.0), 1.6));

// --- NODE DEFINITIONS ----------------------------------------------------

// Base plane
Rendering.Plane pl = new Rendering.Plane();
pl.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.0, 0.2, 0.0 });
pl.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(0.5, 0.5, new double[] { 1.0, 1.0, 1.0 }));
root.InsertChild(pl, OpenTK.Matrix4d.RotateX(-OpenTK.MathHelper.PiOver2) * OpenTK.Matrix4d.CreateTranslation(0.0, -1.0, 0.0));

// Cylinders
Rendering.Cylinder c = new Rendering.Cylinder();
root.InsertChild(c, OpenTK.Matrix4d.RotateX(OpenTK.MathHelper.PiOver2) * OpenTK.Matrix4d.CreateTranslation(-2.1, 0.0, 1.0));
c = new Rendering.Cylinder();
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.2, 0.0, 0.7 });
c.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(12.0, 1.0, new double[] { 0.0, 0.0, 0.3 }));
root.InsertChild(c, OpenTK.Matrix4d.RotateY(-0.4) * OpenTK.Matrix4d.CreateTranslation(1.0, 0.0, 1.0));
c = new Rendering.Cylinder(0.0, 100.0);
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.1, 0.7, 0.0 });
root.InsertChild(c, OpenTK.Matrix4d.RotateY(0.2) * OpenTK.Matrix4d.CreateTranslation(5.0, 0.3, 4.0));
c = new Rendering.Cylinder(-0.5, 0.5);
c.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.8, 0.6, 0.0 });
root.InsertChild(c, OpenTK.Matrix4d.Scale(2.0) * OpenTK.Matrix4d.RotateX(1.2) * OpenTK.Matrix4d.CreateTranslation(2.0, 1.8, 16.0));