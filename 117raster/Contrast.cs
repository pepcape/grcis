// Default = contrast enhancing function.

using System;

formula.pixelTransform0 = (
  ImageContext ic,
  ref float R,
  ref float G,
  ref float B) =>
{
  R = Util.Saturate(0.5f + 1.3f * (R - 0.5f));
  G = Util.Saturate(0.5f + 1.3f * (G - 0.5f));
  B = Util.Saturate(0.5f + 1.3f * (B - 0.5f));
  return true;
};

// Test create function: sinc(r^2)
formula.pixelCreate = (
  ImageContext ic,
  out float R,
  out float G,
  out float B) =>
{
  // [x, y] in {0, 1]
  double x = ic.x / (double)Math.Max(1, ic.width  - 1);
  double y = ic.y / (double)Math.Max(1, ic.height - 1);

  // I need uniform scale (x-scale == y-scale).
  if (ic.width > ic.height)
  {
    // Landscape.
    x -= 0.5;
    y = ic.height * (y - 0.5) / ic.width;
  }
  else
  {
    // Portrait.
    x = ic.width * (x - 0.5) / ic.height;
    y -= 0.5;
  }

  // Faster scales.
  x *= 12;
  y *= 12;

  // sinc function: sin(x) / x
  // I'm using sinc(r^2) here..
  double rr = Math.Max(double.Epsilon, x * x + y * y);
  double v = Math.Sin(rr) / rr;

  // Simple color palette (blue -> red).
  R = (float)Util.Saturate(0.6 * (v + 0.5));
  G = (float)Util.Saturate(0.5 * (0.5 - v));
  B = (float)Util.Saturate(1.2 * (0.5 - v));
};
