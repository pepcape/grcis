//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

//////////////////////////////////////////////////
// CSG scene.

CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {0.5, 0.5, 0.5}, 0.2, 0.8, 0.1, 16));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.5, 0.7, 0.6};

// Camera.
scene.Camera = new StaticCamera(new Vector3d(0.0, 0.0, -11.0),
                                new Vector3d(0.0, 0.0, 1.0),
                                60.0);

// Light sources.
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(1.0));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 3.0, -3.0), 1.2));

// --- NODE DEFINITIONS ----------------------------------------------------

// Vlevo nahore: hlava panacka - vyuziva implementace Xor na rty, vnitrek ust je tvoren pomoci Intersection, jinak je zalozena na Union a Difference
root.InsertChild(Scenes.head(), Matrix4d.CreateTranslation(-6, 1.5, 0) * Matrix4d.RotateY(System.Math.PI / 10) * Matrix4d.Scale(0.7));

// Uprostred nahode: testovani funkcnosti difference pro sest kouli
root.InsertChild(Scenes.test6Spheres(SetOperation.Difference), Matrix4d.CreateTranslation(0, 1.5, 0) * Matrix4d.Scale(0.7));

// Vpravo nahore: testovani operace Xor tak, ze se funkce test6Spheres vola jednou s Union, jednou s Xor a vysledky se odectou
root.InsertChild(Scenes.testXor2(), Matrix4d.CreateTranslation(6, 1.5, 0) * Matrix4d.RotateX(System.Math.PI / 3) * Matrix4d.RotateY(System.Math.PI / 4) * Matrix4d.Scale(0.6));

// Vlevo dole: test xoru s Cylindry - opet jako Union - Xor = vysledek
root.InsertChild(Scenes.testWithCylindersXor(), Matrix4d.CreateTranslation(-5, -2.5, 0) * Matrix4d.Scale(0.7));

// Uprostred dole: prosty Xor valcu
root.InsertChild(Scenes.testWithCylinders(SetOperation.Xor), Matrix4d.CreateTranslation(0, -2.5, 0) * Matrix4d.Scale(0.7));

// Vpravo dole: testovani Intersection - prosty prusecik dvou kouli
root.InsertChild(Scenes.test2Spheres(SetOperation.Intersection), Matrix4d.CreateTranslation(4, -1.5, 0));
