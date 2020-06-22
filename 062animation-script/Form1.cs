using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GuiSupport;
using MathSupport;
using Rendering;
using Utilities;

namespace _062animation
{
  using ScriptContext = Dictionary<string, object>;

  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion("$Rev$");

    public static Form1 singleton = null;

    /// <summary>
    /// Output raster image.
    /// </summary>
    protected Bitmap outputImage = null;

    /// <summary>
    /// Main animation-rendering thread.
    /// </summary>
    protected Thread aThread = null;

    /// <summary>
    /// Progress info / user break handling.
    /// Used also as input lock for MT animation computation.
    /// </summary>
    protected Progress progress = new Progress();

    /// <summary>
    /// Explicit CS-script scene-file-name.
    /// </summary>
    public string sceneFileName = "";

    /// <summary>
    /// Image width in pixels, 0 for default value (according to panel size).
    /// </summary>
    public int ImageWidth = 640;

    /// <summary>
    /// Image height in pixels, 0 for default value (according to panel size).
    /// </summary>
    public int ImageHeight = 480;

    /// <summary>
    /// Param string tooltip = help.
    /// </summary>
    private string tooltip = "";

    /// <summary>
    /// Shared ToolTip instance.
    /// </summary>
    private ToolTip tt = new ToolTip();

    /// <summary>
    /// Working context for the CS-script definitions.
    /// </summary>
    private ScriptContext ctx = null;

    private void EnableRendering (bool enable)
    {
      buttonRender.Enabled =
      buttonRenderAnim.Enabled =
      buttonScene.Enabled =
      buttonRes.Enabled =
      textParam.Enabled =
      numTime.Enabled =
      numFrom.Enabled =
      numTo.Enabled =
      numFps.Enabled =
      numericSupersampling.Enabled = enable;
      buttonStop.Enabled = !enable;
    }

    /// <summary>
    /// Create a scene from the defined CS-script file-name.
    /// Returns null if failed.
    /// </summary>
    public IRayScene SceneFromScript (
      out IImageFunction imf,
      out IRenderer rend,
      ref int width,
      ref int height,
      ref int superSampling,
      ref double minTime,
      ref double maxTime,
      ref double fps,
      in double? time = null)
    {
      if (string.IsNullOrEmpty(sceneFileName))
      {
        imf     = null;
        rend    = null;
        tooltip = "";
        return null;
      }

      if (ctx == null)
      {
        ctx = new ScriptContext();    // we need a new context object for each computing batch..
        Scripts.SetScene(ctx, new AnimatedRayScene());
      }

      if (Scripts.ContextInit(
        ctx,
        Path.GetFileName(sceneFileName),
        width,
        height,
        superSampling,
        minTime,
        maxTime,
        fps))
      {
        // Script needs to be called.

        if (time.HasValue)
          ctx[PropertyName.CTX_TIME] = time.Value;

        Scripts.SceneFromObject(
          ctx,
          sceneFileName,
          textParam.Text,
          (sc) => AnimatedScene.Init(sc, textParam.Text),
          SetText);
      }

      IRayScene scene = Scripts.ContextMining(
        ctx,
        out imf,
        out rend,
        out tooltip,
        ref width,
        ref height,
        ref superSampling,
        ref minTime,
        ref maxTime,
        ref fps);

      return scene;
    }

    /// <summary>
    /// Redraws single image.
    /// </summary>
    private void RenderImage ()
    {
      Cursor.Current = Cursors.WaitCursor;

      EnableRendering(false);

      ActualWidth = ImageWidth;
      if (ActualWidth <= 0)
        ActualWidth = panel1.Width;
      ActualHeight = ImageHeight;
      if (ActualHeight <= 0)
        ActualHeight = panel1.Height;

      bool debug     = false;
      superSampling  = (int)numericSupersampling.Value;
      double minTime = (double)numFrom.Value;
      double maxTime = (double)numTo.Value;
      double fps     = (double)numFps.Value;
      double time    = (double)numTime.Value;

      // Params: <debug> disables multi-threading.
      Dictionary<string, string> p = Util.ParseKeyValueList(textParam.Text);
      if (Util.TryParse(p, "debug", ref debug) &&
          debug)
        superSampling = 1;

      // Force preprocessing.
      ctx = null;

      // 1. preprocessing - compute simulation, animation data, etc.
      _ = FormSupport.getScene(
        out _, out _,
        ref ActualWidth,
        ref ActualHeight,
        ref superSampling,
        ref minTime,
        ref maxTime,
        ref fps,
        textParam.Text,
        time);

      if (debug)
        superSampling = 1;

      // 2. compute regular frame (using the pre-computed context).
      IRayScene scene = FormSupport.getScene(
        out IImageFunction imf,
        out IRenderer rend,
        ref ActualWidth,
        ref ActualHeight,
        ref superSampling,
        ref minTime,
        ref maxTime,
        ref fps,
        textParam.Text,
        time);

      if (debug)
        superSampling = 1;

      // Update GUI.
      if (ImageWidth > 0)   // preserving default (form-size) resolution
      {
        ImageWidth  = ActualWidth;
        ImageHeight = ActualHeight;
        UpdateResolutionButton();
      }
      UpdateSupersampling(superSampling);
      UpdateAnimationTiming(minTime, maxTime, fps);

      // IImageFunction.
      if (imf == null)      // not defined in the script
        imf = FormSupport.getImageFunction();

      // IRenderer.
      if (rend == null)        // not defined in the script
        rend = FormSupport.getRenderer(superSampling);
      rend.Adaptive     = 0;
      rend.ProgressData = progress;
      progress.Continue = true;

      // Set TLS.
      MT.threads     = 1;
      MT.threadID    = 0;
      MT.imageWidth  = ActualWidth;
      MT.imageHeight = ActualHeight;
      MT.InitThreadData();
      MT.SetRendering(scene, imf, rend);

      // Animation time has to be set.
      if (scene is ITimeDependent sc)
        sc.Time = time;

      // Output image.
      outputImage = new Bitmap(ActualWidth, ActualHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

      Stopwatch sw = new Stopwatch();
      sw.Start();

      rend.RenderRectangle(outputImage, 0, 0, ActualWidth, ActualHeight);

      sw.Stop();
      labelElapsed.Text = string.Format(CultureInfo.InvariantCulture, "Elapsed: {0:f1}s", 1.0e-3 * sw.ElapsedMilliseconds);

      pictureBox1.Image = outputImage;

      // Reset TLS.
      MT.ResetRendering();
      EnableRendering(true);

      Cursor.Current = Cursors.Default;
    }

    delegate void SetImageCallback (Bitmap newImage);

    protected void SetImage (Bitmap newImage)
    {
      if (pictureBox1.InvokeRequired)
      {
        SetImageCallback si = new SetImageCallback(SetImage);
        BeginInvoke(si, new object[] {newImage});
      }
      else
      {
        pictureBox1.Image = newImage;
        pictureBox1.Invalidate();
      }
    }

    delegate void SetTextCallback (string text);

    protected void SetText (string text)
    {
      if (labelElapsed.InvokeRequired)
      {
        SetTextCallback st = new SetTextCallback(SetText);
        BeginInvoke(st, new object[] {text});
      }
      else
        labelElapsed.Text = text;
    }

    delegate void UpdateResolutionCallback ();

    protected void UpdateResolutionButton ()
    {
      if (buttonRes.InvokeRequired)
      {
        UpdateResolutionCallback ur = new UpdateResolutionCallback(UpdateResolutionButton);
        BeginInvoke(ur);
      }
      else
        buttonRes.Text = FormResolution.GetLabel(ref ImageWidth, ref ImageHeight);
    }

    delegate void UpdateSupersamplingCallback (int superSampling);

    protected void UpdateSupersampling (int superSampling)
    {
      if (numericSupersampling.InvokeRequired)
      {
        UpdateSupersamplingCallback us = new UpdateSupersamplingCallback(UpdateSupersampling);
        BeginInvoke(us, new object[] {superSampling});
      }
      else
        numericSupersampling.Value = superSampling;
    }

    delegate void UpdateAnimationCallback (double minTime, double maxTime, double fps);

    protected void UpdateAnimationTiming (double minTime, double maxTime, double fps)
    {
      if (numFrom.InvokeRequired)
      {
        UpdateAnimationCallback ua = new UpdateAnimationCallback(UpdateAnimationTiming);
        BeginInvoke(ua, new object[] { minTime, maxTime, fps });
      }
      else
      {
        numFrom.Value = (decimal)minTime;
        numTo.Value   = (decimal)maxTime;
        numFps.Value  = (decimal)fps;
      }
    }

    delegate void StopAnimationCallback ();

    protected void StopAnimation ()
    {
      if (aThread == null)
        return;

      if (buttonRenderAnim.InvokeRequired)
      {
        StopAnimationCallback ea = new StopAnimationCallback(StopAnimation);
        BeginInvoke(ea);
      }
      else
      {
        // actually stop the animation:
        lock (progress)
        {
          progress.Continue = false;
        }
        aThread.Join();
        aThread = null;

        // GUI stuff:
        EnableRendering(true);
      }
    }

    public Form1 (string[] args)
    {
      singleton = this;
      InitializeComponent();

      // Init rendering params:
      string name;
      FormSupport.InitializeParams(args, out name);
      if (!string.IsNullOrEmpty(sceneFileName))
      {
        sceneFileName = Path.GetFullPath(sceneFileName);
        buttonScene.Text = sceneFileName;
      }
      Text += " (rev: " + rev + ") '" + name + '\'';

      UpdateResolutionButton();
    }

    private void buttonRes_Click (object sender, EventArgs e)
    {
      FormResolution form = new FormResolution(ImageWidth, ImageHeight);
      if (form.ShowDialog() == DialogResult.OK)
      {
        ImageWidth  = form.ImageWidth;
        ImageHeight = form.ImageHeight;
        UpdateResolutionButton();
      }
    }

    private void buttonRender_Click (object sender, EventArgs e)
    {
      RenderImage();
    }

    private void buttonStop_Click (object sender, EventArgs e)
    {
      StopAnimation();
    }

    private void Form1_FormClosing (object sender, FormClosingEventArgs e)
    {
      StopAnimation();
    }

    //============================================================
    //===      Animation rendering using multiple threads      ===
    //============================================================

    //============================================================
    //   Constant data:

    /// <summary>
    /// Frame width in pixels.
    /// </summary>
    protected int ActualWidth;

    /// <summary>
    /// Frame height in pixels.
    /// </summary>
    protected int ActualHeight;

    /// <summary>
    /// Time of the last frame.
    /// </summary>
    protected double end;

    /// <summary>
    /// Time delta.
    /// </summary>
    protected double dt;

    /// <summary>
    /// Supersampling factor.
    /// </summary>
    public int superSampling;

    //============================================================
    //   Variable data ("progress" is used as "input data lock"):

    /// <summary>
    /// Frame number to compute.
    /// </summary>
    protected volatile int frameNumber;

    /// <summary>
    /// Frame time to compute.
    /// </summary>
    protected double time;

    /// <summary>
    /// One computed animation frame.
    /// </summary>
    public class Result
    {
      public Bitmap image;
      public int frameNumber;
    }

    /// <summary>
    /// Semaphore guarding the output queue.
    /// </summary>
    protected Semaphore sem = null;

    /// <summary>
    /// Output queue.
    /// </summary>
    protected Queue<Result> queue = null;

    protected void initQueue ()
    {
      if (queue == null)
        queue = new Queue<Result>();
      else
      {
        while (queue.Count > 0)
        {
          Result r = queue.Dequeue();
          r.image.Dispose();
        }
      }
    }

    /// <summary>
    /// Animation rendering prolog: prepare all the global (uniform) values, start the main thread.
    /// </summary>
    private void buttonRenderAnim_Click (object sender, EventArgs e)
    {
      if (aThread != null)
        return;

      EnableRendering(false);
      lock (progress)
      {
        progress.Continue = true;
      }

      ActualWidth = ImageWidth;
      if (ActualWidth <= 0)
        ActualWidth = panel1.Width;
      ActualHeight = ImageHeight;
      if (ActualHeight <= 0)
        ActualHeight = panel1.Height;

      // Start main rendering thread:
      aThread = new Thread(new ThreadStart(RenderAnimation));
      aThread.Start();
    }

    /// <summary>
    /// Worker-thread-specific data.
    /// </summary>
    protected class WorkerThreadInit
    {
      /// <summary>
      /// [Animated] scene object.
      /// </summary>
      public IRayScene scene;

      /// <summary>
      /// [Animated] image function.
      /// </summary>
      public IImageFunction imageFunction;

      /// <summary>
      /// Actual rendering object.
      /// </summary>
      public IRenderer rend;

      // Future MT.threadID.
      public int threadID;

      public WorkerThreadInit (
        IRenderer r,
        IRayScene sc,
        IImageFunction imf,
        int thrId)
      {
        scene         = sc;
        imageFunction = imf;
        rend          = r;
        threadID      = thrId;
      }
    }

    /// <summary>
    /// Main animation rendering thread.
    /// Initializes worker threads and collects the results.
    /// </summary>
    protected void RenderAnimation ()
    {
      Cursor.Current = Cursors.WaitCursor;

      int threads       = Environment.ProcessorCount;
      int superSampling = (int)numericSupersampling.Value;
      bool debug        = false;

      // Params: <debug> disables multi-threading.
      Dictionary<string, string> p = Util.ParseKeyValueList(textParam.Text);
      if (p.TryGetValue("debug", out string val) &&
          (string.IsNullOrEmpty(val) ||
           Util.positive(val)))
      {
        // Debugging.
        debug         = true;
        threads       = 1;
        superSampling = 1;
      }

      int t;    // thread ordinal number
      double minTime  = (double)numFrom.Value;
      double maxTime  = (double)numTo.Value;
      double fps      = (double)numFps.Value;

      WorkerThreadInit[] wti = new WorkerThreadInit[threads];

      // Force preprocessing.
      ctx = null;

      // 1. preprocessing - compute simulation, animation data, etc.
      _ = FormSupport.getScene(
        out _, out _,
        ref ActualWidth,
        ref ActualHeight,
        ref superSampling,
        ref minTime,
        ref maxTime,
        ref fps,
        textParam.Text);

      IRayScene sc0       = null;
      IImageFunction imf0 = null;
      IRenderer rend0     = null;
      MT.imageWidth       = ActualWidth;
      MT.imageHeight      = ActualHeight;

      for (t = 0; t < threads; t++)
      {
        // Creating a new scene instance for every thread.
        Scripts.SetScene(ctx, new AnimatedRayScene());

        if (debug)
          superSampling = 1;

        // 2. initialize data for regular frames (using the pre-computed context).
        IRayScene sc = FormSupport.getScene(
          out IImageFunction imf,
          out IRenderer rend,
          ref ActualWidth,
          ref ActualHeight,
          ref superSampling,
          ref minTime,
          ref maxTime,
          ref fps,
          textParam.Text);

        if (debug)
          superSampling = 1;

        // Fallback instances.
        if (imf == null)
          imf = FormSupport.getImageFunction();
        if (rend == null)
          rend = FormSupport.getRenderer(superSampling);

        if (t == 0)
        {
          // The 1st thread. I'll compare to this.
          sc0   = sc;
          imf0  = imf;
          rend0 = rend;

          // Update GUI.
          if (ImageWidth > 0)   // preserving default (form-size) resolution
          {
            ImageWidth  = ActualWidth;
            ImageHeight = ActualHeight;
            UpdateResolutionButton();
          }
          UpdateSupersampling(superSampling);
          UpdateAnimationTiming(minTime, maxTime, fps);
        }
        else
        {
          // An additional thread - it must have different instances (if animated).
          if (sc == sc0 &&
              sc is ITimeDependent sca)
            sc = (IRayScene)sca.Clone();

          if (imf == imf0 &&
              imf is ITimeDependent imfa)
            imf = (IImageFunction)imfa.Clone();

          if (rend == rend0 &&
              rend is ITimeDependent renda)
            rend = (IRenderer)renda.Clone();
        }

        // Animation bounds.
        if (sc is ITimeDependent sca1)
        {
          sca1.Start = minTime;
          sca1.End   = maxTime;
        }
        if (imf is ITimeDependent imfa1)
        {
          imfa1.Start = minTime;
          imfa1.End   = maxTime;
        }
        if (rend is ITimeDependent renda1)
        {
          renda1.Start = minTime;
          renda1.End   = maxTime;
        }

        // IRenderer init.
        rend.Adaptive     = 0;   // turn off adaptive bitmap synthesis completely (interactive preview not needed)
        rend.ProgressData = progress;

        wti[t] = new WorkerThreadInit(rend, sc, imf, t);
      }

      // Update animation timing.
      time = minTime;
      end  = maxTime;
      if (end <= time)
        end = time + 1.0;

      dt = (fps > 0.0) ? 1.0 / fps : 25.0;
      end += 0.5 * dt;
      frameNumber = 0;
      float total = (float)((end - time) * fps);

      initQueue();
      sem = new Semaphore(0, 10 * threads);

      // Pool of working threads.
      MT.threads = threads;
      Thread[] pool = new Thread[threads];
      for (t = 0; t < threads; t++)
        pool[t] = new Thread(new ParameterizedThreadStart(RenderWorker));
      for (t = threads; --t >= 0;)
        pool[t].Start(wti[t]);

      // Loop for collection of computed frames.
      int frames = 0;
      int lastDisplayedFrame = -1;
      const long DISPLAY_GAP = 10000L;
      long lastDisplayedTime = -DISPLAY_GAP;
      ETF etf = new ETF(0.6f, threads);
      Stopwatch sw = new Stopwatch();
      sw.Start();

      Result r;
      while (true)
      {
        sem.WaitOne();                    // wait until a frame is finished

        lock (progress)                   // regular finish, escape, user break?
        {
          if (!progress.Continue ||
              time >= end &&
              frames >= frameNumber)
            break;
        }

        // There could be a frame to process.
        lock (queue)
        {
          if (queue.Count == 0)
            continue;
          r = queue.Dequeue();
        }

        // GUI progress indication.
        double seconds = 1.0e-3 * sw.ElapsedMilliseconds;
        double cfps = ++frames / seconds;
        etf.Estimate((float)seconds, frames / total, out float remaining);

        // Time spent.
        int cs = (int)seconds;
        int ch = cs / 3600;
        cs %= 3600;
        int cm = cs / 60;
        cs %= 60;

        // Time remaining.
        int ls = (int)remaining;
        int lh = ls / 3600;
        ls %= 3600;
        int lm = ls / 60;
        ls %= 60;
        SetText(string.Format(CultureInfo.InvariantCulture, "Frames (mt{0}): {1}  ({2}:{3:00}:{4:00} fin, {5}:{6:00}:{7:00} rem)",
                              threads, frames, ch, cm, cs, lh, lm, ls));
        if (r.frameNumber > lastDisplayedFrame &&
            sw.ElapsedMilliseconds > lastDisplayedTime + DISPLAY_GAP)
        {
          lastDisplayedFrame = r.frameNumber;
          lastDisplayedTime = sw.ElapsedMilliseconds;
          SetImage((Bitmap)r.image.Clone());
        }

        // Save the image file.
        string fileName = string.Format("out{0:0000}.png", r.frameNumber);
        r.image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
        r.image.Dispose();
      }

      for (t = 0; t < threads; t++)
      {
        pool[t].Join();
        pool[t] = null;
      }

      Cursor.Current = Cursors.Default;

      StopAnimation();
    }

    /// <summary>
    /// Worker thread (picks up individual frames and renders them one by one).
    /// </summary>
    protected void RenderWorker (object spec)
    {
      // Thread-specific data.
      if (!(spec is WorkerThreadInit init))
        return;

      // Set TLS.
      MT.threadID = init.threadID;
      MT.InitThreadData();
      MT.SetRendering(init.scene, init.imageFunction, init.rend);

      // Worker loop.
      while (true)
      {
        double myTime;
        double myEndTime;
        int myFrameNumber;

        lock (progress)
        {
          if (!progress.Continue ||
              time > end)
          {
            sem.Release();                  // chance for the main animation thread to give up as well..

            // Reset TLS.
            MT.ResetRendering();

            return;
          }

          // I've got a frame to compute.
          myTime = time;
          myEndTime = (time += dt);
          myFrameNumber = frameNumber++;
        }

        // Set up the new result record.
        Result r = new Result();
        r.image = new Bitmap(MT.imageWidth, MT.imageHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        r.frameNumber = myFrameNumber;

        // Set specific time to my scene.
        if (init.scene is ITimeDependent ascene)
        {
#if LOGGING
          Util.Log($"Scene(thr={MT.threadID}) #{ascene.getSerial()} setTime({myTime:f3})");
#endif
          ascene.Time = myTime;
        }

        if (init.imageFunction is ITimeDependent aimf)
          aimf.Time = myTime;

        if (init.rend is ITimeDependent arend)
          arend.Time = myTime;

        // Render the whole frame...
        init.rend.RenderRectangle(r.image, 0, 0, MT.imageWidth, MT.imageHeight);

        // ...and put the result into the output queue.
        lock (queue)
        {
          queue.Enqueue(r);
        }
        sem.Release();                      // notify the main animation thread
      }
    }

    private void buttonScene_Click (object sender, EventArgs e)
    {
      if (aThread != null)
        return;

      OpenFileDialog ofd = new OpenFileDialog()
      {
        Title = "Open Scene Script",
        Filter = "CS-script files|*.cs" +
                 "|All files|*.*",
        FilterIndex = 1,
        FileName = ""
      };

      if (ofd.ShowDialog() != DialogResult.OK)
      {
        buttonScene.Text = "Default scene";
        sceneFileName = "";
        return;
      }

      sceneFileName = ofd.FileName;
      buttonScene.Text = sceneFileName;   // Path.GetFileName(sceneFileName);
    }

    private void textParam_MouseHover (object sender, EventArgs e)
    {
      tt.Show(tooltip, (IWin32Window)sender,
              10, -24 - 15 * Util.CharsInString(tooltip, '\r'), 3000);
    }
  }
}
