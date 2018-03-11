// author: Josef Pelikan
// pure single version

// Mandelbrot index (single version)
inline int mandelbrotSingle ( const float x0, const float y0, const int iter )
{
  float y = y0;
  float x = x0;
  float yy = y * y;
  float xx = x * x;
  int i = iter;

  while ( --i && (xx + yy < 4.0f) )
  {
    y = x * y * 2.0f + y0;
    x = xx - yy + x0;
    yy = y * y;
    xx = x * x;
  }

  return i;
}

// Mandelbrot OpenCL kernel (single precision)
kernel void mandelSingle ( global write_only uchar4 *dst, // 0
                           const int width,               // 1
                           const int height,              // 2
                           const int iter,                // 3
                           const float xOrig,             // 4
                           const float yOrig,             // 5
                           const float dxy,               // 6
                           global read_only uchar4* cmap, // 7
                           const int cmapSize )           // 8
{
  const int ix = get_global_id(0);
  const int iy = get_global_id(1);

  if ( ix < width &&
       iy < height )
  {
    // location:
    const float x0 = ix * dxy + xOrig;
    const float y0 = iy * dxy + yOrig;

    // calculate the Mandelbrot index:
    int m = mandelbrotSingle( x0, y0, iter );
    m = m > 0 ? iter - m : 0;

    // convert the Madelbrot index into a color:
    uchar4 color;

    if ( m <= 0 )
      color.xyzw = 0;
    else
      color = cmap[ m % cmapSize ];

    // write the pixel:
    dst[ width * iy + ix ] = color;
  }
}
