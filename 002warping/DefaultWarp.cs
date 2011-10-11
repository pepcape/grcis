using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _002warping
{
  public abstract class DefaultWarp : IWarp
  {
    public virtual double Factor
    {
      get;
      set;
    }

    protected double scale = 1.0;

    protected int iwidth = 1;

    protected int iheight = 1;

    protected int owidth = 1;

    protected int oheight = 1;

    public virtual void SetScale ( double scale )
    {
      this.scale = scale;
    }

    public virtual void OutputSize ( int iwidth, int iheight, out int owidth, out int oheight )
    {
      this.iwidth  = iwidth;
      this.iheight = iheight;
      this.owidth  = owidth  = (int)(iwidth * scale);
      this.oheight = oheight = (int)(iheight * scale);
    }

    public abstract void F ( double x, double y, out double u, out double v );

    public abstract void FInv ( double u, double v, out double x, out double y );
  }
}
