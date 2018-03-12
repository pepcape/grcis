// CSG scene:
Rendering.CSGInnerNode root = new Rendering.CSGInnerNode(Rendering.SetOperation.Union);
root.SetAttribute(Rendering.PropertyName.REFLECTANCE_MODEL, new Rendering.PhongModel());
root.SetAttribute(Rendering.PropertyName.MATERIAL, new Rendering.PhongMaterial(new double[] { 0.5, 0.5, 0.5 }, 0.2, 0.8, 0.1, 16));
sc.Intersectable = root;

// Background color:
sc.BackgroundColor = new double[] { 0.5, 0.7, 0.6 };

// Camera:
sc.Camera = new Rendering.StaticCamera(new OpenTK.Vector3d(0.0, 0.0, -11.0),
new OpenTK.Vector3d(0.0, 0.0, 1.0),
60.0);

// Light sources:
sc.Sources = new System.Collections.Generic.LinkedList<Rendering.ILightSource>();
sc.Sources.Add(new Rendering.AmbientLightSource(1.0));
sc.Sources.Add(new Rendering.PointLightSource(new OpenTK.Vector3d(-5.0, 3.0, -3.0), 1.2));

// --- NODE DEFINITIONS ----------------------------------------------------

// Vlevo nahore: hlava panacka - vyuziva implementace Xor na rty, vnitrek ust je tvoren pomoci Intersection, jinak je zalozena na Union a Difference
root.InsertChild(Rendering.Scenes.head(), OpenTK.Matrix4d.CreateTranslation(-6, 1.5, 0) * OpenTK.Matrix4d.RotateY(System.Math.PI / 10) * OpenTK.Matrix4d.Scale(0.7));

// Uprostred nahode: testovani funkcnosti difference pro sest kouli
root.InsertChild(Rendering.Scenes.test6Spheres(Rendering.SetOperation.Difference), OpenTK.Matrix4d.CreateTranslation(0, 1.5, 0) * OpenTK.Matrix4d.Scale(0.7));

// Vpravo nahore: testovani operace Xor tak, ze se funkce test6Spheres vola jednou s Union, jednou s Xor a vysledky se odectou
root.InsertChild(Rendering.Scenes.testXor2(), OpenTK.Matrix4d.CreateTranslation(6, 1.5, 0) * OpenTK.Matrix4d.RotateX(System.Math.PI / 3) * OpenTK.Matrix4d.RotateY(Math.PI / 4) * OpenTK.Matrix4d.Scale(0.6));

// Vlevo dole: test xoru s Cylindry - opet jako Union - Xor = vysledek
root.InsertChild(Rendering.Scenes.testWithCylindersXor(), OpenTK.Matrix4d.CreateTranslation(-5, -2.5, 0) * OpenTK.Matrix4d.Scale(0.7));

// Uprostred dole: prosty Xor valcu
root.InsertChild(Rendering.Scenes.testWithCylinders(Rendering.SetOperation.Xor), OpenTK.Matrix4d.CreateTranslation(0, -2.5, 0) * OpenTK.Matrix4d.Scale(0.7));

// Vpravo dole: testovani Intersection - prosty prusecik dvou kouli
root.InsertChild(Rendering.Scenes.test2Spheres(Rendering.SetOperation.Intersection), OpenTK.Matrix4d.CreateTranslation(4, -1.5, 0));