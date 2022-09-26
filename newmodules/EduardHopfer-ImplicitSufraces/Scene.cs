using EduardHopfer;
using JosefPelikan; // For CatmullRomAnimator

Debug.Assert(scene != null);
Debug.Assert(context != null);

context[PropertyName.CTX_WIDTH]         = 320; // 1800, 1920
context[PropertyName.CTX_HEIGHT]        = 200;
context[PropertyName.CTX_SUPERSAMPLING] = 4;

double end = 24.0;
context[PropertyName.CTX_START_ANIM]    = 0.0;
context[PropertyName.CTX_END_ANIM]      = end;
context[PropertyName.CTX_FPS]           = 20.0;

context[PropertyName.CTX_ALGORITHM]     = new SphereTracing();

AnimatedCSGInnerNode root = new AnimatedCSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {0.5, 0.5, 0.5}, 0.3, 0.6, 0.1, 16));
scene.Intersectable = root;

string name = "bananko";
CatmullRomAnimator pa = new CatmullRomAnimator()
{
    Start = 0.0,
    End   = end
};
pa.newProperty(name, 0.0, end, 8,
               PropertyAnimator.InterpolationStyle.Cyclic,
               new List<Vector3d>() {
                 new Vector3d(0.0, 0.2,-0.5),
                 new Vector3d(1.0, 0.2,-0.5),
                 new Vector3d(1.0, 1.6, 0.0),
                 new Vector3d(0.0, 1.2, 0.0)},
               true);
scene.Animator = pa;

// Background color.
scene.BackgroundColor = new double[] {0.0, 0.01, 0.03};
scene.Background = new DefaultBackground(scene.BackgroundColor);

// Camera zommed to sphere
// scene.Camera = new StaticCamera(new Vector3d(0.0, 1.5, -0.35),
//                                 new Vector3d(-1.6, -4.5, 1.7),
//                                 60.0);

// Static camera
// scene.Camera = new StaticCamera(new Vector3d(0.0, 1.5, -6),
//                                 new Vector3d(0, -1, 4),
//                                 60.0);

// Animated camera
AnimatedCamera cam = new AnimatedCamera(new Vector3d(2.0,  0.1,  0.0),
                                        new Vector3d(0.7,  0.8, -7.0),
                                        50.0);
cam.End = end;
AnimatedRayScene ascene = scene as AnimatedRayScene;
if (ascene != null)
    ascene.End = end;
scene.Camera = cam;

// Light sources.
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 4.0, 3.0), 1.0));

// THE SCENE --------------
//var sphere = new DistanceField(ImplicitCommon.Sphere);
//sphere.SetAttribute(PropertyName.COLOR, new double[] {0.3, 1.0, 0.0}); // 0.3 1.0 0.0
PhongMaterial pm = new PhongMaterial( new double[] { 0.0, 0.2, 0.1 }, 0.05, 0.05, 0.1, 128 );
pm.n = 1.6; //2.41diamond 1.6glass
pm.Kt = 0.4;
//sphere.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel());
//sphere.SetAttribute( PropertyName.MATERIAL, pm );
//root.InsertChild(sphere, Matrix4d.Identity);
//root.InsertChild(sphere, Matrix4d.Scale(2.0, 1.0, 1.0) * Matrix4d.RotateY(1.0));

var box = new DistanceField(ImplicitCommon.Box);
box.SetAttribute(PropertyName.COLOR, new double[] {1.0, 0.6, 0.0});

var inner = new ImplicitInnerNode(new ImplicitUnion(3));
//inner.InsertChild(sphere, Matrix4d.CreateTranslation(-0.5, 0.0, 0.0));
//inner.InsertChild(box,  Matrix4d.RotateY(0.6) * Matrix4d.CreateTranslation(1.8, 0.0, 0.0));

// Animation node.
AnimatedNodeTranslate an = new AnimatedNodeTranslate(
  name,
  new Vector3d(0.0, 0.2,-0.5),
  Matrix4d.Identity,
  0.0, 20.0);
inner.InsertChild(an, Matrix4d.Identity);

var torus = new DistanceField(ImplicitCommon.Torus);
//torus.SetAttribute(PropertyName.COLOR, new double[] {0.0, 0.1, 1.0});
torus.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel());
torus.SetAttribute( PropertyName.MATERIAL, pm );
an.InsertChild(torus, Matrix4d.Identity);

//inner.InsertChild(torus, Matrix4d.Identity);
inner.InsertChild(box,  Matrix4d.RotateY(0.6) * Matrix4d.CreateTranslation(3.3, 0.0, 0.0));

root.InsertChild(inner, Matrix4d.Identity);



//root.InsertChild(sphere, Matrix4d.Identity);
//root.InsertChild(box,  Matrix4d.RotateY(1.0) * Matrix4d.CreateTranslation(2.4, 0.0, 0.0));
// ------------------------

Plane pl = new Plane();
pl.SetAttribute(PropertyName.COLOR, new double[] {0.3, 0.0, 0.0});
pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.6, 0.6, new double[] {1.0, 1.0, 1.0}));
root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));