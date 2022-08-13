using EduardHopfer;

Debug.Assert(scene != null);
Debug.Assert(context != null);

context[PropertyName.CTX_WIDTH]         = 640; // 1800, 1920
context[PropertyName.CTX_HEIGHT]        = 480;

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {0.5, 0.5, 0.5}, 0.3, 0.6, 0.1, 16));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.01, 0.03};
scene.Background = new DefaultBackground(scene.BackgroundColor);

// Camera.
scene.Camera = new StaticCamera(new Vector3d(0.0, 2.0, -10.0),
                                new Vector3d(0.0, 0.0, 1.0),
                                60.0);

// Light sources.
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 5.0, 3.0), 1.0));

// THE SCENE --------------
var sphere = new DistanceField(ImplicitCommon.Sphere);
sphere.SetAttribute(PropertyName.COLOR, new double[] {0.3, 1.0, 0.0});
PhongMaterial pm = new PhongMaterial( new double[] { 0.0, 0.2, 0.1 }, 0.05, 0.05, 0.1, 128 );
pm.n = 1.6;
pm.Kt = 0.4;
//sphere.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel());
//sphere.SetAttribute( PropertyName.MATERIAL, pm );
//root.InsertChild(sphere, Matrix4d.Identity);
//root.InsertChild(sphere, Matrix4d.Scale(2.0, 1.0, 1.0) * Matrix4d.RotateY(1.0));

var box = new DistanceField(ImplicitCommon.Box);
box.SetAttribute(PropertyName.COLOR, new double[] {1.0, 0.6, 0.0});

var inner = new ImplicitInnerNode(ImplicitOperation.Union);
inner.InsertChild(sphere, Matrix4d.CreateTranslation(0.0, 0.0, 0.0));
inner.InsertChild(box,  Matrix4d.RotateY(1.0) * Matrix4d.CreateTranslation(2.4, 0.0, 0.0));

root.InsertChild(inner, Matrix4d.Identity);

//root.InsertChild(sphere, Matrix4d.Identity);
//root.InsertChild(box,  Matrix4d.RotateY(1.0) * Matrix4d.CreateTranslation(2.4, 0.0, 0.0));
// ------------------------

Plane pl = new Plane();
pl.SetAttribute(PropertyName.COLOR, new double[] {0.3, 0.0, 0.0});
pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.6, 0.6, new double[] {1.0, 1.0, 1.0}));
root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));