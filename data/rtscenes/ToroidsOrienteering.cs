//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

context[PropertyName.CTX_ALGORITHM] = new RayTracing();

public class ObStripesTexture : CheckerTexture
{
  public ObStripesTexture(double fu, double fv, double[] color)
    : base(fu, fv, color)
  {
  }

  public override long Apply(Intersection inter)
  {
    double u = inter.TextureCoord.X * Fu;
    double v = inter.TextureCoord.Y * Fv;

    long ui = (long)System.Math.Floor(u);
    long vi = (long)System.Math.Floor(v);

    bool parity = ((ui + vi) & 1) != 0;
    if (parity == ((u - ui) > (v - vi)))
      System.Array.Copy(Color2, inter.SurfaceColor, System.Math.Min(Color2.Length, inter.SurfaceColor.Length));

    inter.textureApplied = true;

    return ui + (long)MathSupport.RandomStatic.numericRecipes((ulong)vi);
  }
}

//////////////////////////////////////////////////
// CSG scene.

var white = new double[] {1, 1, 1};
var root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(white, 0.2, 0.8, 0.0, 1));
scene.Intersectable = root;

// Background color.
scene.BackgroundColor = new double[] {0.05, 0.05, 0.05};

// Camera.
scene.Camera = new StaticCamera(new Vector3d(0.0, 0.0, -10.0),
                                new Vector3d(0.0, -0.04, 1.0),
                                60.0);

// Light sources.
scene.Sources = new System.Collections.Generic.LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(1.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 3.0, -3.0), 2.0));

// --- NODE DEFINITIONS ----------------------------------------------------

var red = new double[] { 0.7, 0, 0 };

var t = new Torus(2.0, 1.0);
t.SetAttribute(PropertyName.TEXTURE, new ObStripesTexture(2, 2, red));
root.InsertChild(t, Matrix4d.Scale(0.8) * Matrix4d.CreateRotationX(0.9) * Matrix4d.CreateTranslation(2.7, 1.0, 0.0));

t = new Torus(1.5, 0.8);
t.SetAttribute(PropertyName.TEXTURE, new ObStripesTexture(4, 4, red));
root.InsertChild(t, Matrix4d.CreateRotationX(0.8) * Matrix4d.CreateTranslation(-2.5, 0.0, 0.0));

t = new Torus(2.0, 1.0);
t.SetAttribute(PropertyName.TEXTURE, new ObStripesTexture(6, 2, red));
root.InsertChild(t, Matrix4d.Scale(0.6) * Matrix4d.CreateRotationX(1.3) * Matrix4d.CreateTranslation(1.6, -2.2, 0.0));
