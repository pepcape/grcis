// Petr Hudeček
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace _035plasma
{
  struct Pixel
  {
      public float Intensity;
      public PixelType Type;
      public Pixel(float intensity, PixelType type)
      {
          Type = type;
          Intensity = intensity;
      }
  }
  enum PixelType
  {
      SourceTail,
      BrickBorder,
      Brick,
      BrickFire,
      Background
  }
  class Source
  {
      public int X { get; set; }
      public int Y { get; set; }
      public int Xspeed { get; set; }
      public int Yspeed { get; set; }
      public Source(int x, int y, int xs, int ys)
      {
          X = x;
          Y = y;
          Xspeed = xs;
          Yspeed = ys;
      }
  }
  public class Simulation
  {
      private List<Source> Sources = new List<Source>();
    /// <summary>
    /// Prime-time initialization.
    /// </summary>
    /// <param name="width">Visualization bitmap width.</param>
    /// <param name="height">Visualization bitmap height.</param>
    public Simulation ( int width, int height )
    {
      Width  = width;
      Height = height;
      Reset();
    }
    /// <summary>
    /// Width of the visualization bitmap in pixels.
    /// </summary>
    public int Width
    {
      get;
      set;
    }
    /// <summary>
    /// Height of the visualization bitmap in pixels.
    /// </summary>
    public int Height
    {
      get;
      set;
    }
    /// <summary>
    /// Frame number starting from 0.
    /// </summary>
    public long Frame
    {
      get;
      set;
    }
    /// <summary>
    /// Simulation reset.
    /// Can be called at any time after instance construction.
    /// </summary>
    public void Reset ()
    {
      // !!!{{ TODO: put your simulation-reset code here
      Frame     = 0;
      simWidth  = Width; 
      simHeight = Height;
      s = new Pixel[simHeight, simWidth];
      s2 = new Pixel[simHeight, simWidth];
      for (int x = 0; x < simWidth; x++ )
      {
          for (int  y= 0;y<simHeight;y++)
          {
              s[y,x] = new Pixel(0, PixelType.Background);
          }
      }
      int BRICK_HEIGHT = simHeight / 20;
      int BRICK_WIDTH = simWidth / 13;
      int nx = (simWidth - BRICK_WIDTH * 10) / 2;
      int ny = 50;
      for (int yb = 0; yb < 4; yb++ )
      {
          for (int xb = 0; xb < 10; xb++)
          {
              int left = nx + (BRICK_WIDTH+2) * xb;
              int top = ny + (BRICK_HEIGHT+2) * yb;
              int right = left + BRICK_WIDTH;
              int bottom = top + BRICK_HEIGHT;
              // Fill
              for (int x = left; x < right; x++)
              {
                  for (int y= top; y < bottom; y++)
                  {
                      s[y, x] = new Pixel(1, PixelType.Brick);
                  }
                  s[top, x] = new Pixel(1, PixelType.BrickBorder);
                  s[bottom -1, x] = new Pixel(1, PixelType.BrickBorder);
              } 
              for (int y = top; y < bottom; y++)
              {
                  s[y, left] = new Pixel(1, PixelType.BrickBorder);
                  s[y, right - 1] = new Pixel(1, PixelType.BrickBorder);
              }

          }
      }
        for (int x = 0; x < simWidth; x++)
      {
          for (int y = 0; y < simHeight; y++)
          {
              s2[y, x] = s[y, x];
          }
      }
          Sources.Clear();
      Sources.Add(new Source(simWidth / 2, simHeight * 4 / 5, -1, -1));
      rnd = new Random();  
      // !!!}}
    }
    /// <summary>
    /// Mouse-down response.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseDown ( Point location )
    {
      // !!!{{ TODO: put your drawing logic here
      return false;
      // !!!}}
    }

    /// <summary>
    /// Mouse-up response.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseUp ( Point location )
    {
      // !!!{{ TODO: put your drawing logic here
        Sources.Add(new Source(location.X, location.Y, -1, -1));
        return false;
      // !!!}}
    }

    /// <summary>
    /// Mouse-move response.
    /// </summary>
    /// <param name="location">Mouse pointer location.</param>
    /// <returns>True if the visualization bitmap was altered.</returns>
    public bool MouseMove ( Point location )
    {
      // !!!{{ TODO: put your drawing logic here
      return false;
      // !!!}}
    }

    /// <summary>
    /// Width of the simulation array.
    /// </summary>
    protected int simWidth;

    /// <summary>
    /// Height of the simulation array.
    /// </summary>
    protected int simHeight;

    /// <summary>
    /// The simulation array itself.
    /// Rewrite this declaration if you want to use multi-band simulation..
    /// </summary>
    private Pixel[,] s;
    private Pixel[,] s2;
    private bool visualizeFirst = false;

    /// <summary>
    /// Globally allocated (shared) random generator.
    /// </summary>
    protected Random rnd;

    /// <summary>
    /// Simulation of a single timestep.
    /// </summary>
    public void Simulate ()
    {
    // !!!{{ 
    visualizeFirst = !visualizeFirst;
    Pixel[,] vis = (visualizeFirst ? s : s2);
    Pixel[,] source = (visualizeFirst ? s2 : s);

    foreach(Source src in Sources)
    {
        // Move X
        bool reversed = false;
        src.X += src.Xspeed;
        if (src.X < 0) { src.X = 0; src.Xspeed = -src.Xspeed; }
        if (src.X >= simWidth) { src.X = simWidth-1; src.Xspeed = -src.Xspeed; }
        // Source touches brick
        if (source[src.Y, src.X].Type == PixelType.BrickBorder && source[src.Y, src.X].Intensity == 1)
        {
            source[src.Y, src.X].Intensity *= 0.99f;
            src.Xspeed *= -1;
            reversed = true;
        }
        src.Y += src.Yspeed;
        if (src.Y < 0) { src.Y = 0; src.Yspeed = -src.Yspeed; }
        if (src.Y >= simHeight) { src.Y = simHeight - 1; src.Yspeed = -src.Yspeed; }
        // Source touches brick
        if (!reversed && source[src.Y, src.X].Type == PixelType.BrickBorder && source[src.Y, src.X].Intensity == 1)
        {
            source[src.Y, src.X].Intensity *= 0.99f;
            src.Yspeed *= -1;
            reversed = true;
        }

        if (!reversed)
        {
            // Make sources generate heat
            source[src.Y, src.X] = new Pixel(1, PixelType.SourceTail);
        }
    }

    // Move pixels
    for ( int y = 1; y < simHeight-1; y++ )
        for (int x = 1; x < simWidth - 1; x++)
        {
            if (source[y,x].Type == PixelType.Brick)
            {
                vis[y, x].Type = PixelType.Brick;
                bool adjacentOnFire = false;
                if (source[y - 1, x - 1].Type == PixelType.BrickFire) { adjacentOnFire = true; }
                if (source[y - 1, x].Type == PixelType.BrickFire) { adjacentOnFire = true; }
                if (source[y - 1, x+1].Type == PixelType.BrickFire) { adjacentOnFire = true; }
                if (source[y, x - 1].Type == PixelType.BrickFire) { adjacentOnFire = true; }
                if (source[y, x + 1].Type == PixelType.BrickFire ) { adjacentOnFire = true; }
                if (source[y + 1, x-1].Type == PixelType.BrickFire) { adjacentOnFire = true; }
                if (source[y + 1, x].Type == PixelType.BrickFire) { adjacentOnFire = true; }
                if (source[y + 1, x+1].Type == PixelType.BrickFire) { adjacentOnFire = true; }
                if (adjacentOnFire)
                {
                    vis[y, x].Type = PixelType.BrickFire;
                }
            }
            else 
            if (source[y,x].Type == PixelType.BrickFire)
            {
                vis[y, x].Type = PixelType.BrickFire;
                // Simply burn.
                vis[y, x].Intensity *= 0.8f;
                if (vis[y,x].Intensity < 0.01f)
                {
                    vis[y, x].Type = PixelType.SourceTail;
                }
            }
            else 
            if (source[y,x].Type == PixelType.BrickBorder)
            {
                // Were you already on fire? Then let's burn.
                if (source[y,x].Intensity < 1)
                {
                    vis[y, x].Intensity *= 0.99f;
                    vis[y, x].Type = PixelType.BrickFire;
                    Debug.Print("level " + visualizeFirst + "; " + y + ":" + x + " => on fire");
                    continue;
                }
                // Is an adjacent brick border on fire? If so, set yourself on fire
                bool adjacentOnFire = false;
                if (source[y, x - 1].Type == PixelType.BrickBorder && source[y, x - 1].Intensity < 1) { adjacentOnFire = true; }
                if (source[y, x + 1].Type == PixelType.BrickBorder && source[y, x + 1].Intensity < 1) { adjacentOnFire = true; }
                if (source[y-1, x ].Type == PixelType.BrickBorder && source[y-1, x ].Intensity < 1) { adjacentOnFire = true; }
                if (source[y+1, x ].Type == PixelType.BrickBorder && source[y+1, x ].Intensity < 1) { adjacentOnFire = true; }

                if (adjacentOnFire)
                {
                    vis[y, x].Intensity *= 0.99f;
                    Debug.Print("level " + visualizeFirst + "; " + y + ":" + x + " => starts to burn");
                }
            }
            else if (source[y,x].Type == PixelType.Background || source[y,x].Type == PixelType.SourceTail)
            {
                // Source tail fall
                int sourceCount = 1;
                float intensity = source[y, x].Intensity;

                if (source[y, x - 1].Type != PixelType.BrickBorder)
                {
                    intensity += source[y, x - 1].Intensity;
                    sourceCount += 1;
                }
                if (source[y, x + 1].Type != PixelType.BrickBorder)
                {
                    intensity += source[y, x + 1].Intensity;
                    sourceCount += 1;
                }
                if (source[y-1,x].Type != PixelType.BrickBorder)
                {
                    intensity += source[y - 1, x].Intensity * 4f;
                    sourceCount += 4;
                }
                if (source[y+1,x].Type != PixelType.BrickBorder)
                {
                    intensity += source[y + 1, x].Intensity;
                    sourceCount += 1;
                }
                intensity /= 8;

                vis[y, x].Type = PixelType.SourceTail;
                vis[y, x].Intensity = intensity;
            }
        }
      Frame++;
      // !!!}}
    }

    /// <summary>
    /// Visualization of the current state.
    /// </summary>
    /// <returns>Visualization bitmap.</returns>
    public Bitmap Visualize ()
    {
      // !!!{{ TODO: put your visualization code here

      PixelFormat fmt = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
      Bitmap result = new Bitmap( Width, Height, fmt );


      Pixel[,] vis = (visualizeFirst ? s : s2);

      BitmapData data = result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, fmt );
      unsafe
      {
        byte* ptr;

        for ( int y = 0; y < Height; y++ )
        {
          ptr = (byte*)data.Scan0 + y * data.Stride;

          for ( int x = 0; x < Width; x++ )
          {
            Pixel pixel = vis[ y % simHeight, x % simWidth ];
            byte c = (byte)(pixel.Intensity * 255);

            if (pixel.Type == PixelType.SourceTail)
            {
                ptr[0] = 0;
                ptr[1] = c;
                ptr[2] = c;
            }
            else if (pixel.Type == PixelType.Brick)
            {
                ptr[0] = 0;
                ptr[1] = 0;
                ptr[2] = c;
            }
            else if (pixel.Type == PixelType.BrickBorder)
            {
                ptr[0] = c;
                ptr[1] = c;
                ptr[2] = c;
            }
            else if (pixel.Type == PixelType.BrickFire)
            {
                ptr[0] = c;
                ptr[1] = c;
                ptr[2] = c;
            }
            else if (pixel.Type == PixelType.Background)
            {
                ptr[0] = 0;
                ptr[1] = 0;
                ptr[2] = 0;
            }
            ptr += 3;
          }
        }
      }
      result.UnlockBits( data );

      return result;

      // !!!}}
    }
  }
}
