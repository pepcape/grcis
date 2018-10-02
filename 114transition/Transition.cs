using System.Collections.Generic;
using MathSupport;
using Utilities;

namespace _114transition
{
  public class Transition
  {
    /// <summary>
    /// Form data initialization.
    /// </summary>
    /// <param name="name">Your first-name and last-name.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams ( out string name, out string param, out string tooltip )
    {
      // {{
      name = "Josef Pelikán";
      param = "morph=false, par=true";
      tooltip = "t=<double>, morph=<bool>, par=<bool>";
      // }}
    }

    //==========================================================================
    //  Transition instance

    /// <summary>
    /// Image width in pixels.
    /// </summary>
    protected int width;

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    protected int height;

    /// <summary>
    /// True if the MorphingFunction should be used, false for the BlendingFunction.
    /// </summary>
    protected bool useMorphing;

    public Transition ( int wid, int hei, string param )
    {
      Reset( wid, hei, param );
    }

    /// <summary>
    /// Reset the Transition object instance.
    /// </summary>
    /// <param name="wid">New image width in pixels.</param>
    /// <param name="hei">New image height in pixels.</param>
    /// <param name="param">Optional string parameter (its content and format is entierely up to you).</param>
    public void Reset ( int wid, int hei, string param )
    {
      width  = wid;
      height = hei;

      // {{

      // !!! TODO: process the param string if you need !!!
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count > 0 )
      {
        // morph=<bool>
        if ( !Util.TryParse( p, "morph", ref useMorphing ) )
          useMorphing = false;

        // ... you can add more parameters here ...
      }

      // }}
    }

    /// <summary>
    /// Do we use morphing (true) or blending (false)?
    /// </summary>
    public bool UseMorphing ()
    {
      return useMorphing;
    }

    /// <summary>
    /// Simple image blending w/o geometric transform.
    /// </summary>
    /// <param name="t">Time variable (0.0 to 1.0).</param>
    /// <param name="x">Horizontal pixel coordinate.</param>
    /// <param name="y">Vertical pixel coordinate.</param>
    /// <returns>0.0 for the input1 value, 1.0 for the input2 (and anything in between).</returns>
    public double BlendingFunction (
      double t, int x, int y )
    {
      // {{

      // !!! TODO: modify this function if you will be using simple blending !!!
      return Util.Clamp( t, 0.0, 1.0 );

      // }}
    }

    /// <summary>
    /// Image morphing: every time a blend of arbitrary input1 and input2 pixels can be used.
    /// </summary>
    /// <param name="t">Time variable (0.0 to 1.0).</param>
    /// <param name="x">Horizontal target pixel coordinate.</param>
    /// <param name="y">Vertical target pixel coordinate.</param>
    /// <param name="srcX1">Input1 source pixel horizontal coordinate.</param>
    /// <param name="srcY1">Input1 source pixel vertical coordinate.</param>
    /// <param name="srcX2">Input2 source pixel horizontal coordinate.</param>
    /// <param name="srcY2">Input2 source pixel vertical coordinate.</param>
    /// <param name="blend">Blend factor: 0.0 for input1, 1.0 for input2 (and anything in between).</param>
    public void MorphingFunction (
      double t, int x, int y,
      out double srcX1, out double srcY1,
      out double srcX2, out double srcY2,
      out double blend )
    {
      // {{

      // !!! TODO: modify this function if you will be using morphing !!!

      // Default implementation - blending.
      srcX1 = x;
      srcY1 = y;

      srcX2 = x + (1.0 - t) * width;
      srcY2 = y;

      if ( srcX2 >= width )
      {
        srcX2 = width - 1;
        blend = 0.0;
      }
      else
        blend = 1.0;

      // }}
    }
  }
}
