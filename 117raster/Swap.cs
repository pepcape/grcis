// Text params -> script context.
// Any global pre-processing is allowed here.
formula.contextCreate = (in Bitmap input, in string param) =>
{
  if (string.IsNullOrEmpty(param))
    return null;

  Dictionary<string, string> p = Util.ParseKeyValueList(param);

  float coeff = 1.0f;

  // coeff=<float>
  if (Util.TryParse(p, "coeff", ref coeff))
    coeff = Util.Saturate(coeff);

  float freq = 12.0f;

  // freq=<float>
  if (Util.TryParse(p, "freq", ref freq))
    freq = Util.Clamp(freq, 0.01f, 1000.0f);

  Dictionary<string, object> sc = new Dictionary<string, object>();
  sc["coeff"]   = coeff;
  sc["freq"]    = freq;
  sc["tooltip"] = "coeff=<float> .. swap coefficient (0.0 - no swap, 1.0 - complete swap)\r" +
                  "freq=<float> .. density frequency for image generation (default=12)";

  return sc;
};

// R <-> B channel swap with weights.
formula.pixelTransform0 = (
  in ImageContext ic,
  ref float R,
  ref float G,
  ref float B) =>
{
  float coeff = 0.0f;
  Util.TryParse(ic.context, "coeff", ref coeff);

  float r = Util.Saturate(R * (1.0f - coeff) + B * coeff);
  float b = Util.Saturate(R * coeff          + B * (1.0f - coeff));
  R = r;
  B = b;

  // Output color was modified.
  return true;
};

// Test create function: sinc(r^2)
formula.pixelCreate = (
  in ImageContext ic,
  out float R,
  out float G,
  out float B) =>
{
  // [x, y] in {0, 1]
  double x = ic.x / (double)Math.Max(1, ic.width  - 1);
  double y = ic.y / (double)Math.Max(1, ic.height - 1);

  // I need uniform scale (x-scale == y-scale) with origin at the image center.
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

  // Custom scales.
  float freq = 12.0f;
  Util.TryParse(ic.context, "freq", ref freq);

  x *= freq;
  y *= freq;

  // Periodic function of r^2.
  double rr = x * x + y * y;
  bool odd = ((int)Math.Round(rr) & 1) > 0;

  // Simple color palette (yellow, blue).
  R = odd ? 0.0f : 1.0f;
  G = R;
  B = 1.0f - R;
};
