//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {0.5, 0.5, 0.5}, 0.2, 0.7, 0.2, 16));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.15, 0.2};

// Camera.
scene.Camera = new StaticCamera(new Vector3d(0.0, 0.0, -10.0),
                                new Vector3d(0.0, -0.04, 1.0),
                                60.0);

// Light sources.
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(1.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 3.0, -3.0), 2.0));

// --- NODE DEFINITIONS ----------------------------------------------------

Sphere s;
Torus t;

t = new Torus(2.0, 1.0);
t.SetAttribute(PropertyName.COLOR, new double[] {0, 0, 0.7});
t.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(50, 20, new double[] {0.9, 0.9, 0}));
root.InsertChild(t, Matrix4d.Scale(0.8) * Matrix4d.CreateRotationX(0.9) * Matrix4d.CreateTranslation(2.7, 0.6, 0.0));

t = new Torus(1.0, 1.5);
t.SetAttribute(PropertyName.COLOR, new double[] {1, 0, 0});
t.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(40, 40, new double[] {0.8, 1.0, 1.0}));

CSGInnerNode donut = new CSGInnerNode(SetOperation.Difference);
donut.InsertChild(t, Matrix4d.Identity);
s = new Sphere();
s.SetAttribute(PropertyName.COLOR, new double[] {0.5, 0.5, 0.5});
s.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(100, 100, new double[] {1, 0.5, 0}));
donut.InsertChild(s, Matrix4d.Scale(3) * Matrix4d.CreateTranslation(0, -3, 0));
root.InsertChild(donut, Matrix4d.CreateRotationX(0.8) * Matrix4d.CreateTranslation(-2.5, 0.0, 0.0));

int number = 14;
for (int i = 0; i<number; i++)
{
  t = new Torus(0.2, 0.01 + 0.185 * i / number);
  t.SetAttribute(PropertyName.COLOR, new double[] {1, 1, 1});
  root.InsertChild(t, Matrix4d.CreateRotationX(0.5 * i) * Matrix4d.CreateTranslation(10.0 * (1.0 * i / number) - 4.7, -2.5, 0));
}
