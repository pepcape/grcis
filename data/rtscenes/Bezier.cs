// CSG scene:
Rendering.CSGInnerNode root = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
root.SetAttribute(Rendering.PropertyName.REFLECTANCE_MODEL, new Rendering.PhongModel());
root.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.5, 0.5, 64));
sc.Intersectable = root;

// Background color:
sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

// Camera:
sc.Camera = new Rendering.StaticCamera(new OpenTK.Vector3d(0.7, 0.5, -5.0),
new OpenTK.Vector3d(0.0, -0.18, 1.0),
50.0);

// Light sources:
sc.Sources = new System.Collections.Generic.LinkedList<Rendering.ILightSource>();
sc.Sources.Add(new Rendering.AmbientLightSource(0.8));
sc.Sources.Add(new Rendering.PointLightSource(new OpenTK.Vector3d(-5.0, 3.0, -3.0), 1.0));

// --- NODE DEFINITIONS ----------------------------------------------------

// Bezier patch (not yet):
Rendering.BezierSurface b = new Rendering.BezierSurface(1, 2, new double[] {
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
b.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(10.5, 12.0, new double[] { 0.0, 0.0, 0.1 }));
root.InsertChild(b, OpenTK.Matrix4d.RotateY(-0.4) * OpenTK.Matrix4d.CreateTranslation(-1.1, -0.9, 0.0));

// Cylinders for reflections..
Rendering.Cylinder c = new Rendering.Cylinder();
c.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 0.0, 0.6, 0.0 }, 0.2, 0.6, 0.3, 8));
root.InsertChild(c, OpenTK.Matrix4d.Scale(0.15) * OpenTK.Matrix4d.RotateX(OpenTK.MathHelper.PiOver2) * OpenTK.Matrix4d.CreateTranslation(-0.4, 0.0, 0.0));
c = new Rendering.Cylinder();
c.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 0.8, 0.2, 0.0 }, 0.2, 0.6, 0.3, 8));
root.InsertChild(c, OpenTK.Matrix4d.Scale(0.2) * OpenTK.Matrix4d.RotateX(OpenTK.MathHelper.PiOver2) * OpenTK.Matrix4d.CreateTranslation(-1.9, 0.0, 3.0));

// Infinite plane with checker:
Rendering.Plane pl = new Rendering.Plane();
pl.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 });
pl.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(0.6, 0.6, new double[] { 1.0, 1.0, 1.0 }));
root.InsertChild(pl, OpenTK.Matrix4d.RotateX(-OpenTK.MathHelper.PiOver2) * OpenTK.Matrix4d.CreateTranslation(0.0, -1.0, 0.0));