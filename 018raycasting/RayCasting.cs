using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using OpenTK;

namespace Rendering
{
  public class RayScene : IRayScene
  {
    public IIntersectable Intersectable
    {
      get;
      set;
    }

    public double[] BackgroundColor
    {
      get;
      set;
    }

    public ICamera Camera
    {
      get;
      set;
    }

    public ICollection<ILightSource> Sources
    {
      get;
      set;
    }

    public RayScene ()
    {
      Camera = new StaticCamera( new Vector3d( 0.0, 0.0, -10.0 ),
                                 new Vector3d( 0.0, 0.0, 1.0 ), 60.0 );
      Sources = new LinkedList<ILightSource>();
      Sources.Add( new PointLightSource( new Vector4d( -10.0, 8.0, 3.0, 1.0 ), 1.0 ) );
      BackgroundColor = new double[] { 0.0, 0.1, 0.2 };
    }
  }
}
