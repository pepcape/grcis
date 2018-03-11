using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _002warping
{
  public interface IWarp
  {
    /// <summary>
    /// Parameter value for the warping function..
    /// </summary>
    double Factor
    {
      get;
      set;
    }

    /// <summary>
    /// Sets output scaling.
    /// </summary>
    /// <param name="scale"></param>
    void SetScale ( double scale );

    /// <summary>
    /// Declares output image size.
    /// </summary>
    /// <param name="iwidth"></param>
    /// <param name="iheight"></param>
    /// <param name="owidth"></param>
    /// <param name="oheight"></param>
    void OutputSize ( int iwidth, int iheight, out int owidth, out int oheight );

    /// <summary>
    /// Forward warping transform.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="u"></param>
    /// <param name="v"></param>
    void F ( double x, double y, out double u, out double v );

    /// <summary>
    /// Backward (inverse) warping transform.
    /// </summary>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void FInv ( double u, double v, out double x, out double y );
  }
}
