// CSG scene:
Rendering.CSGInnerNode root = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
root.SetAttribute(Rendering.PropertyName.REFLECTANCE_MODEL, new Rendering.PhongModel());
root.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 0.5, 0.5, 0.5 }, 0.2, 0.7, 0.2, 16));
sc.Intersectable = root;

// Background color:
sc.BackgroundColor = new double[] { 0.0, 0.15, 0.2 };

// Camera:
sc.Camera = new Rendering.StaticCamera(new OpenTK.Vector3d(0.0, 0.0, -10.0),
new OpenTK.Vector3d(0.0, -0.04, 1.0),
60.0);

// Light sources:
sc.Sources = new System.Collections.Generic.LinkedList<Rendering.ILightSource>();
sc.Sources.Add(new Rendering.AmbientLightSource(1.8));
sc.Sources.Add(new Rendering.PointLightSource(new OpenTK.Vector3d(-5.0, 3.0, -3.0), 2.0));

// --- NODE DEFINITIONS ----------------------------------------------------

Rendering.Sphere s;
Rendering.Torus t;

t = new Rendering.Torus(2.0, 1.0);
t.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0, 0, 0.7 });
t.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(50, 20, new double[] { 0.9, 0.9, 0 }));
root.InsertChild(t, OpenTK.Matrix4d.Scale(0.8) * OpenTK.Matrix4d.CreateRotationX(0.9) * OpenTK.Matrix4d.CreateTranslation(2.7, 0.6, 0.0));

t = new Rendering.Torus(1.0, 1.5);
t.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1, 0, 0 });
t.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(40, 40, new double[] { 0.8, 1.0, 1.0 }));

Rendering.CSGInnerNode donut = new Rendering.CSGInnerNode(Rendering.SetOperation.Difference);
donut.InsertChild(t, OpenTK.Matrix4d.Identity);
s = new Rendering.Sphere();
s.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 0.5, 0.5, 0.5 });
s.SetAttribute(Rendering.PropertyName.TEXTURE, new Rendering.CheckerTexture(100, 100, new double[] { 1, 0.5, 0 }));
donut.InsertChild(s, OpenTK.Matrix4d.Scale(3) * OpenTK.Matrix4d.CreateTranslation(0, -3, 0));
root.InsertChild(donut, OpenTK.Matrix4d.CreateRotationX(0.8) * OpenTK.Matrix4d.CreateTranslation(-2.5, 0.0, 0.0));

int number = 14;
for (int i = 0; i<number; i++)
{
t = new Rendering.Torus(0.2, 0.01 + 0.185 * i / number);
t.SetAttribute(Rendering.PropertyName.COLOR, new double[] { 1, 1, 1 });
root.InsertChild(t, OpenTK.Matrix4d.CreateRotationX(0.5 * i) * OpenTK.Matrix4d.CreateTranslation(10.0 * (1.0 * i / number) - 4.7, -2.5, 0));
}