using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace OpenglSupport
{
  public static class GlInfo
  {
    /// <summary>
    /// Logs OpenGL properties.
    /// </summary>
    /// <param name="ext">Print detailed list of extensions as well?</param>
    public static void LogGLProperties (bool ext = false)
    {
      // 0. .NET, OpenTK
      Util.LogFormat("{0} ({1}), OpenTK {2}",
                     Util.TargetFramework, Util.RunningFramework, Util.AssemblyVersion(typeof(Vector3)));

      // 1. OpenGL version, vendor, ..
      string version  = GL.GetString(StringName.Version);
      string vendor   = GL.GetString(StringName.Vendor);
      string renderer = GL.GetString(StringName.Renderer);
      string shVer    = GL.GetString(StringName.ShadingLanguageVersion);
      Util.LogFormat("OpenGL version: {0}, shading language: {1}",
                     version ?? "-", shVer ?? "-");
      Util.LogFormat("Vendor: {0}, driver: {1}",
                     vendor ?? "-", renderer ?? "-");

      // 2. OpenGL parameters:
      int    maxVertices = GL.GetInteger(GetPName.MaxElementsVertices);
      int    maxIndices  = GL.GetInteger(GetPName.MaxElementsIndices);
      string extensions  = GL.GetString(StringName.Extensions);
      int    extLen      = (extensions == null) ? 0 : extensions.Split(' ').Length;
      Util.LogFormat("Max-vertices: {0}, max-indices: {1}, extensions: {2}",
                     maxVertices, maxIndices, extLen);

      // 3. OpenGL extensions:
      if (ext &&
          extensions != null)
        while (extensions.Length > 0)
        {
          int split = Math.Min(extensions.Length, 80) - 1;
          while (split < extensions.Length &&
                 !char.IsWhiteSpace(extensions[split]))
            split++;
          Util.LogFormat("Ext: {0}", extensions.Substring(0, split));
          if (split + 1 >= extensions.Length)
            break;

          extensions = extensions.Substring(split + 1);
        }
    }

    /// <summary>
    /// Checks OpenGL error and logs a message eventually.
    /// </summary>
    /// <param name="checkpoint">Optional checkpoint identification.</param>
    /// <returns>True in case of error.</returns>
    public static bool LogError (string checkpoint = "?")
    {
      ErrorCode err = GL.GetError ();
      if (err == ErrorCode.NoError)
        return false;

      Util.LogFormat("OpenGL error {0} at {1}", err, checkpoint);
      return true;
    }
  }

  /// <summary>
  /// OpenGL canvas snapshot support.
  /// </summary>
  public class Snapshots
  {
    /// <summary>
    /// Current screencast frame number.
    /// </summary>
    protected static int frameCounter = 0;

    /// <summary>
    /// frameCounter guard.
    /// </summary>
    protected static object frameLock = new object();

    /// <summary>
    /// Reset frame number.
    /// </summary>
    public static void ResetFrameNumber ()
    {
      lock (frameLock)
        frameCounter = 0;
    }

    /// <summary>
    /// Takes current snapshot of the given GLControl and returns it as a Bitmap.
    /// </summary>
    public static Bitmap TakeScreenshot (GLControl glc)
    {
      if (GraphicsContext.CurrentContext == null)
        return null;

      GL.Finish();
      GL.Flush();
      int    wid = glc.ClientSize.Width;
      int    hei = glc.ClientSize.Height;
      Bitmap bmp = new Bitmap(wid, hei);
      System.Drawing.Imaging.BitmapData data = bmp.LockBits(glc.ClientRectangle,
                                                            System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      GL.ReadPixels(0, 0, wid, hei, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
      bmp.UnlockBits(data);
      bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
      return bmp;
    }

    /// <summary>
    /// Saves GLControl snapshot synchronously.
    /// </summary>
    public static void SaveScreenshot (GLControl glc, string fileNameTemplate = "out{0:00000}.png")
    {
      Bitmap bmp = TakeScreenshot(glc);
      if (bmp != null)
      {
        // save the image file:
        string fileName;
        lock (frameLock)
          fileName = string.Format(fileNameTemplate, frameCounter++);
        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
        bmp.Dispose();
      }
    }

    /// <summary>
    /// One screencast frame.
    /// </summary>
    public class Result : IDisposable
    {
      public Bitmap image;
      public int    frameNumber;

      public void Dispose ()
      {
        if (image != null)
        {
          image.Dispose();
          image = null;
        }
      }
    }

    /// <summary>
    /// Template (format) of the output image files.
    /// Should consume one integer parameter ({0}), preferably in
    /// a fixed format, e.g. {0:00000}.
    /// </summary>
    public string FileNameTemplate { get; set; }

    /// <summary>
    /// Frame-saving thread or null if async saving machine is not active.
    /// </summary>
    protected Thread aThread = null;

    /// <summary>
    /// Semaphore guarding the output queue.
    /// Signaled if there are results ready..
    /// </summary>
    protected Semaphore semResults = null;

    /// <summary>
    /// Output queue.
    /// </summary>
    protected Queue<Result> queue = null;

    /// <summary>
    /// True if frame-saving thread should do its job.
    /// </summary>
    protected bool cont = true;

    public int Queue
    {
      get
      {
        int size = 0;
        lock (this)
          if (queue != null)
            size = queue.Count;
        return size;
      }
    }

    public Snapshots (bool resetFrameNumber = true, string fnTemplate = "out{0:00000}.png")
    {
      FileNameTemplate = fnTemplate;
      if (resetFrameNumber)
        ResetFrameNumber();
    }

    protected void resetQueue ()
    {
      lock (this)
        if (queue != null)
          while (queue.Count > 0)
            queue.Dequeue().Dispose();
    }

    protected void initQueue (int maxSize = 500)
    {
      lock (this)
      {
        if (queue == null)
          queue = new Queue<Result>(maxSize);
        else
          while (queue.Count > 0)
            queue.Dequeue().Dispose();

        semResults = new Semaphore(0, maxSize);
      }
    }

    /// <summary>
    /// Start async frame-saving machine.
    /// </summary>
    /// <param name="maxQueue"></param>
    public void StartSaveThread (int maxQueue = 500)
    {
      lock (this)
      {
        if (aThread != null)
          return;

        cont = true;
        initQueue(maxQueue);

        // Start main rendering thread:
        aThread = new Thread(new ThreadStart(SaveFrames));
        aThread.Start();
      }
    }

    /// <summary>
    /// Stop async frame-saving machine.
    /// </summary>
    public void StopSaveThread ()
    {
      Thread stopThread;

      lock (this)
      {
        if (aThread == null)
          return;

        cont = false;
        stopThread = aThread;
      }

      semResults.Release();
      stopThread.Join();

      lock (this)
      {
        aThread = null;
        resetQueue();
      }
    }

    /// <summary>
    /// Frame-saver thread.
    /// </summary>
    protected void SaveFrames ()
    {
      while (true)
      {
        semResults.WaitOne(); // wait until a frame is finished

        lock (this) // regular finish test
          if (!cont)
            return;

        // there should be a frame to process:
        Result r = null;
        lock (this)
        {
          if (queue.Count == 0)
            continue;

          r = queue.Dequeue();
        }

        // save the image file:
        string fileName = string.Format( FileNameTemplate, r.frameNumber );
        r.image.RotateFlip(RotateFlipType.RotateNoneFlipY);
        r.image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
        r.Dispose();
      }
    }

    /// <summary>
    /// Saves current snapshot asynchronously using the running save-thread.
    /// </summary>
    /// <returns>True if a screenshot was acquired correctly.</returns>
    public bool SaveScreenshotAsync (GLControl glc)
    {
      if (GraphicsContext.CurrentContext == null)
        return false;

      GL.Finish();
      GL.Flush();
      int    wid = glc.ClientSize.Width;
      int    hei = glc.ClientSize.Height;
      Bitmap bmp = new Bitmap(wid, hei);
      System.Drawing.Imaging.BitmapData data = bmp.LockBits(glc.ClientRectangle,
                                                            System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      GL.ReadPixels(0, 0, wid, hei, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
      bmp.UnlockBits(data);

      // set up the new result record:
      Result r = new Result();
      r.image = bmp;

      lock (this)
      {
        if (aThread == null ||
            !cont)
        {
          r.Dispose();
          return false;
        }

        lock (frameLock)
          r.frameNumber = frameCounter++;

        // ... and put the result into the output queue:
        queue.Enqueue(r);
      }

      semResults.Release(); // notify the frame-saver thread
      return true;
    }
  }

  /// <summary>
  /// Item of shader repository database.
  /// </summary>
  public class GlShaderInfo
  {
    public ShaderType type;

    public string sourceFile;

    /// <summary>
    /// Optional hint of directory in which shader source file will be looked for.
    /// </summary>
    public string hintDir;

    /// <summary>
    /// Shader id if compiled successfully.
    /// </summary>
    public GlShader shader;

    public GlShaderInfo (ShaderType st, string source, string hint = null)
    {
      type       = st;
      sourceFile = source;
      hintDir    = hint;
      shader     = null;
    }

    public bool Compile ()
    {
      shader = new GlShader();
      if (shader.CompileShader(type, sourceFile, hintDir))
        return true;

      Util.LogFormat("{0} compile error: {1} .. giving up", type.ToString(), shader.Message);
      shader.Dispose();
      shader = null;
      return false;
    }

    public void Destroy ()
    {
      if (shader != null)
      {
        shader.Dispose();
        shader = null;
      }
    }
  }

  /// <summary>
  /// Item of program repository database.
  /// </summary>
  public class GlProgramInfo
  {
    /// <summary>
    /// Name used in GUI, etc.
    /// </summary>
    public string name;

    /// <summary>
    /// Program id if assembled, linked and verified successfully.
    /// </summary>
    public GlProgram program;

    public List<GlShaderInfo> shaders;

    public GlProgramInfo (string _name, IEnumerable<GlShaderInfo> si = null)
    {
      name    = _name;
      program = null;
      shaders = (si == null) ? new List<GlShaderInfo>() : new List<GlShaderInfo>(si);
    }

    public bool Setup ()
    {
      bool okProgram = true;
      program = null;

      foreach (var shaderInfo in shaders)
        if (!shaderInfo.Compile())
        {
          okProgram = false;
          break;
        }

      if (okProgram)
      {
        // all shaders compiled ok, now we'll try to link them together..
        program = new GlProgram();
        foreach (var shaderInfo in shaders)
          if (!program.AttachShader(shaderInfo.shader))
          {
            Util.LogFormat("GLSL program attach error: {0}", program.Message);
            okProgram = false;
            break;
          }

        if (okProgram)
        {
          okProgram = program.Link();
          if (okProgram)
            program.LogProgramInfo();
          else
            Util.LogFormat("GLSL program link error: {0}", program.Message);
        }
      }

      if (!okProgram)
        Destroy();

      return okProgram;
    }

    public void Destroy ()
    {
      if (program != null)
      {
        program.Dispose();
        program = null;
      }

      foreach (var shaderInfo in shaders)
        shaderInfo.Destroy();
    }
  }

  /// <summary>
  /// GLSL shader object.
  /// </summary>
  public class GlShader: IDisposable
  {
    /// <summary>
    /// Global repository of active shaders.
    /// </summary>
    static HashSet<int> liveShaders = new HashSet<int>();

    /// <summary>
    /// Shader Id by CreateShader()
    /// </summary>
    public int Id = -1;

    public ShaderType Type;

    /// <summary>
    /// Nonzero value if ok.
    /// </summary>
    public int Status = 0;

    public string Message = "Ok.";

    public void Dispose ()
    {
      if (Id < 0)
        return;

      lock (liveShaders)
        if (liveShaders.Contains(Id))
        {
          GL.DeleteShader(Id);
          liveShaders.Remove(Id);
        }

      Id = -1;
    }

    protected bool CompileShader ()
    {
      GL.CompileShader(Id);
      GL.GetShader(Id, ShaderParameter.CompileStatus, out Status);
      Message = GL.GetShaderInfoLog(Id);
      return Status != 0;
    }

    /// <summary>
    /// Compile shader from string source.
    /// </summary>
    public bool CompileShader (ShaderType type, string source)
    {
      if (string.IsNullOrEmpty(source))
      {
        Status = 0;
        Message = "Empty shader source.";
        return false;
      }

      Id = GL.CreateShader(Type = type);
      lock (liveShaders)
        liveShaders.Add(Id);
      GL.ShaderSource(Id, source);
      return CompileShader();
    }

    /// <summary>
    /// Compile shader from a file.
    /// </summary>
    public bool CompileShader (ShaderType type, string fileName, string folderHint)
    {
      string fn = Util.FindSourceFile(fileName, folderHint);
      if (fn == null)
      {
        Status  = 0;
        Message = "Shader file '" + fileName + "' not found.";
        return false;
      }

      string source = null;
      using (StreamReader sr = new StreamReader(fn))
        source = sr.ReadToEnd();

      return CompileShader(type, source);
    }
  }

  public class UniformInfo
  {
    public string            Name    = "";
    public int               Address = -1;
    public int               Size    = 0;
    public ActiveUniformType Type;
  }

  public class AttributeInfo
  {
    public string           Name    = "";
    public int              Address = -1;
    public int              Size    = 0;
    public ActiveAttribType Type;
  }

  public class GlProgram: IDisposable
  {
    /// <summary>
    /// All attached shaders by their type.
    /// </summary>
    Dictionary<ShaderType, GlShader> shaders = new Dictionary<ShaderType, GlShader>();

    /// <summary>
    /// All active vertex attributes.
    /// Attribute identifier in GLSL is the key.
    /// </summary>
    Dictionary<string, AttributeInfo> attributes = new Dictionary<string, AttributeInfo>();

    /// <summary>
    /// All active uniforms.
    /// Uniform identifier in GLSL is the key.
    /// </summary>
    Dictionary<string, UniformInfo> uniforms = new Dictionary<string, UniformInfo>();

    /// <summary>
    /// Program name, can be used in GUI.
    /// </summary>
    public string Name;

    /// <summary>
    /// Program Id by CreateProgram()
    /// </summary>
    public int Id = -1;

    /// <summary>
    /// Nonzero value if ok.
    /// </summary>
    public int Status = 0;

    public string Message = "Ok.";

    public GlProgram (string name = "default")
    {
      Name = name;
      Id   = GL.CreateProgram();
    }

    public void Dispose ()
    {
      if (Id < 0)
        return;

      foreach (var shader in shaders.Values)
        GL.DetachShader(Id, shader.Id);
      GL.DeleteProgram(Id);
      Id = -1;
    }

    public bool AttachShader (GlShader shader)
    {
      if (Id < 0 ||
          shader == null ||
          shader.Id < 0)
      {
        Message = "AttachShader: invalid program or shader.";
        Status  = 0;
        return false;
      }

      GlShader old;
      shaders.TryGetValue(shader.Type, out old);
      if (old != null)
      {
        GL.DetachShader(Id, old.Id);
        old.Dispose();
      }

      GL.AttachShader(Id, shader.Id);

      // ??? How to test if everything went well ???
      Status  = 1;
      Message = GL.GetProgramInfoLog(Id);

      shaders[shader.Type] = shader;

      return true;
    }

    public bool Link ()
    {
      if (Id < 0)
      {
        Message = "Link: invalid program.";
        Status  = 0;
        return false;
      }

      GL.LinkProgram(Id);
      GL.GetProgram(Id, GetProgramParameterName.LinkStatus, out Status);
      Message = GL.GetProgramInfoLog(Id);

      if (Status == 0)
        return false;

      GL.ValidateProgram(Id);
      GL.GetProgram(Id, GetProgramParameterName.ValidateStatus, out Status);
      Message = GL.GetProgramInfoLog(Id);

      if (Status == 0)
        return false;

      int attrCount, uniformCount;
      GL.GetProgram(Id, GetProgramParameterName.ActiveAttributes, out attrCount);
      GL.GetProgram(Id, GetProgramParameterName.ActiveUniforms, out uniformCount);

      int i, len;

      // attributes:
      attributes.Clear();
      for (i = 0; i < attrCount; i++)
      {
        AttributeInfo info = new AttributeInfo ();

        GL.GetActiveAttrib(Id, i, 256, out len, out info.Size, out info.Type, out info.Name);
        info.Address = GL.GetAttribLocation(Id, info.Name);
        attributes.Add(info.Name, info);
      }

      // uniforms:
      uniforms.Clear();
      for (i = 0; i < uniformCount; i++)
      {
        UniformInfo info = new UniformInfo ();

        GL.GetActiveUniform(Id, i, 256, out len, out info.Size, out info.Type, out info.Name);
        info.Address = GL.GetUniformLocation(Id, info.Name);
        uniforms.Add(info.Name, info);
      }

      return true;
    }

    public void EnableVertexAttribArrays ()
    {
      foreach (var attr in attributes.Values)
        GL.EnableVertexAttribArray(attr.Address);
    }

    public void DisableVertexAttribArrays ()
    {
      foreach (var attr in attributes.Values)
        GL.DisableVertexAttribArray(attr.Address);
    }

    public bool HasAttribute (string name)
    {
      return attributes.ContainsKey(name);
    }

    static HashSet<string> unknownNames = new HashSet<string>();

    public int GetAttribute (string name)
    {
      AttributeInfo ai;
      if (attributes.TryGetValue(name, out ai))
        return ai.Address;

      if (!unknownNames.Contains(name))
      {
        Util.LogFormat("GetAttribute: unknown attribute '{0}'", name);
        unknownNames.Add(name);
      }

      return -1;
    }

    public int GetUniform (string name)
    {
      UniformInfo ui;
      if (uniforms.TryGetValue(name, out ui))
        return ui.Address;

      if (!unknownNames.Contains(name))
      {
        Util.LogFormat("GetUniform: unknown uniform '{0}'", name);
        unknownNames.Add(name);
      }

      return -1;
    }

    public void LogProgramInfo ()
    {
      Util.LogFormat("GLSL program '{0}': {1}, shaders: {2}",
                     Name, Id, shaders.Count);
      foreach (var shader in shaders.Values)
        Util.LogFormat("  {0}: {1}",
                       shader.Type, shader.Id);

      // program attributes:
      Util.LogFormat("Attributes[ {0} ]:", attributes.Count);
      foreach (var attr in attributes.Values)
        Util.LogFormat("  {0}: {1}, {2}, {3}",
                       attr.Name, attr.Address, attr.Type, attr.Size);

      // program uniforms:
      Util.LogFormat("Uniforms[ {0} ]:", uniforms.Count);
      foreach (var uni in uniforms.Values)
        Util.LogFormat("  {0}: {1}, {2}, {3}",
                       uni.Name, uni.Address, uni.Type, uni.Size);
    }
  }

  /// <summary>
  /// Abstract object rendered using points/lines/triangles.
  /// There are two phases:
  /// <list type=">">
  /// <item>1. determining vertex-buffer size & stride (separately for points/lines/triangles)</item>
  /// <item>2. actually writing data to the memory (mapped VBO buffer or VAO buffer)</item>
  /// </list>
  /// </summary>
  public interface IRenderObject
  {
    /// <summary>
    /// Number of triangles to render (could be 0).
    /// </summary>
    uint Triangles { get; }

    /// <summary>
    /// Triangles: returns vertex-array size (if ptr is null) or fills vertex array.
    /// </summary>
    /// <param name="ptr">Data pointer (null for determining buffer size).</param>
    /// <param name="origin">Index number in the global vertex array. Will point after the used index-range after the call.</param>
    /// <param name="stride">Vertex size (stride) in bytes.</param>
    /// <param name="col">Use color attribute?</param>
    /// <param name="txt">Use txtCoord attribute?</param>
    /// <param name="normal">Use normal vector attribute?</param>
    /// <param name="ptsize">Use point-size/line-width attribute?</param>
    /// <returns>Data size of the vertex-set (in bytes).</returns>
    unsafe int TriangleVertices (
      ref float* ptr,
      ref uint origin,
      out int stride,
      bool txt,
      bool col,
      bool normal,
      bool ptsize);

    /// <summary>
    /// Triangles: returns index-array size (if ptr is null) or fills index array.
    /// </summary>
    /// <param name="ptr">Data pointer (null for determining buffer size).</param>
    /// <param name="origin">First index to use.</param>
    /// <returns>Data size of the index-set (in bytes).</returns>
    unsafe int TriangleIndices (
      ref uint* ptr,
      uint origin);

    /// <summary>
    /// Number of lines to render (could be 0).
    /// </summary>
    uint Lines { get; }

    /// <summary>
    /// Lines: returns vertex-array size (if ptr is null) or fills vertex array.
    /// </summary>
    /// <param name="ptr">Data pointer (null for determining buffer size).</param>
    /// <param name="origin">Index number in the global vertex array. Will point after the used index-range after the call.</param>
    /// <param name="stride">Vertex size (stride) in bytes.</param>
    /// <param name="col">Use color attribute?</param>
    /// <param name="txt">Use txtCoord attribute?</param>
    /// <param name="normal">Use normal vector attribute?</param>
    /// <param name="ptsize">Use point-size/line-width attribute?</param>
    /// <returns>Data size of the vertex-set (in bytes).</returns>
    unsafe int LineVertices (
      ref float* ptr,
      ref uint origin,
      out int stride,
      bool txt,
      bool col,
      bool normal,
      bool ptsize);

    /// <summary>
    /// Lines: returns index-array size (if ptr is null) or fills index array.
    /// </summary>
    /// <param name="ptr">Data pointer (null for determining buffer size).</param>
    /// <param name="origin">First index to use.</param>
    /// <returns>Data size of the index-set (in bytes).</returns>
    unsafe int LineIndices (
      ref uint* ptr,
      uint origin);

    /// <summary>
    /// Number of points to render (could be 0).
    /// </summary>
    uint Points { get; }

    /// <summary>
    /// Points: returns vertex-array size (if ptr is null) or fills vertex array.
    /// </summary>
    /// <param name="ptr">Data pointer (null for determining buffer size).</param>
    /// <param name="origin">Index number in the global vertex array. Will point after the used index-range after the call.</param>
    /// <param name="stride">Vertex size (stride) in bytes.</param>
    /// <param name="col">Use color attribute?</param>
    /// <param name="txt">Use txtCoord attribute?</param>
    /// <param name="normal">Use normal vector attribute?</param>
    /// <param name="ptsize">Use point-size/line-width attribute?</param>
    /// <returns>Data size of the vertex-set (in bytes).</returns>
    unsafe int PointVertices (
      ref float* ptr,
      ref uint origin,
      out int stride,
      bool txt,
      bool col,
      bool normal,
      bool ptsize);
  }

  /// <summary>
  /// Default implementation of IRenderObject.
  /// *Vertices() and *Indices() functions can be used for buffer size determination
  /// (no actual data are written to the buffer except for LineIndices()).
  /// <see cref="TriVertices"/> should usually be overriden (if triangles are used at all),
  /// <see cref="LinVertices"/> only for shared line-vertices.
  /// </summary>
  public abstract class DefaultRenderObject : IRenderObject
  {
    /// <summary>
    /// Support function writing a [x,y] object to the given buffer.
    /// </summary>
    public static unsafe void Fill (ref float* ptr, float x, float y)
    {
      *ptr++ = x;
      *ptr++ = y;
    }

    /// <summary>
    /// Support function writing a [x,y] object to the given buffer.
    /// </summary>
    public static unsafe void Fill (ref float* ptr, float x, float y, float z)
    {
      *ptr++ = x;
      *ptr++ = y;
      *ptr++ = z;
    }

    /// <summary>
    /// Support function writing a Vector2 object to the given buffer.
    /// </summary>
    public static unsafe void Fill (ref float* ptr, Vector2 v)
    {
      *ptr++ = v.X;
      *ptr++ = v.Y;
    }

    /// <summary>
    /// Support function writing a Vector2 object to the given buffer.
    /// </summary>
    public static unsafe void Fill (ref float* ptr, ref Vector2 v)
    {
      *ptr++ = v.X;
      *ptr++ = v.Y;
    }

    /// <summary>
    /// Support function writing a Vector3 object to the given buffer.
    /// </summary>
    public static unsafe void Fill (ref float* ptr, Vector3 v)
    {
      *ptr++ = v.X;
      *ptr++ = v.Y;
      *ptr++ = v.Z;
    }

    /// <summary>
    /// Support function writing a Vector3 object to the given buffer.
    /// </summary>
    public static unsafe void Fill (ref float* ptr, ref Vector3 v)
    {
      *ptr++ = v.X;
      *ptr++ = v.Y;
      *ptr++ = v.Z;
    }

    /// <summary>
    /// Support function writing a Vector3d object to the given buffer.
    /// </summary>
    public static unsafe void Fill (ref float* ptr, Vector3d v)
    {
      *ptr++ = (float)v.X;
      *ptr++ = (float)v.Y;
      *ptr++ = (float)v.Z;
    }

    /// <summary>
    /// Support function writing a Vector3d object to the given buffer.
    /// </summary>
    public static unsafe void Fill (ref float* ptr, ref Vector3d v)
    {
      *ptr++ = (float)v.X;
      *ptr++ = (float)v.Y;
      *ptr++ = (float)v.Z;
    }

    /// <summary>
    /// Number of triangles to render (0 by default).
    /// </summary>
    public virtual uint Triangles => 0;

    /// <summary>
    /// Number of vertices for triangle rendering (not shared vertices by default).
    /// </summary>
    public virtual uint TriVertices => Triangles * 3;

    /// <summary>
    /// Doesn't fill any vertex data, only counts the stride and the total buffer size.
    /// Necessary to override if you need triangles.
    /// </summary>
    /// <returns>Data size of the vertex-set (in bytes).</returns>
    public virtual unsafe int TriangleVertices (
      ref float* ptr,
      ref uint origin,
      out int stride,
      bool txt,
      bool col,
      bool normal,
      bool ptsize)
    {
      stride = 3;
      if (txt)
        stride += 2;
      if (col)
        stride += 3;
      if (normal)
        stride += 3;
      if (ptsize)
        stride++;

      origin += TriVertices;

      return (int)TriVertices * (stride *= sizeof(float));
    }

    /// <summary>
    /// Doesn't fill any index data, only counts the buffer size.
    /// Necessary to override if you need triangles.
    /// </summary>
    /// <returns>Data size of the index-set (in bytes).</returns>
    public virtual unsafe int TriangleIndices (
      ref uint* ptr,
      uint origin)
    {
      return (int)Triangles * 3 * sizeof(uint);
    }

    /// <summary>
    /// Number of lines to render (0 by default).
    /// </summary>
    public virtual uint Lines => 0;

    /// <summary>
    /// Number of vertices for line rendering (not shared vertices by default).
    /// </summary>
    public virtual uint LinVertices => Lines * 2;

    /// <summary>
    /// Doesn't fill any vertex data, only counts the stride and the total buffer size.
    /// Necessary to override if you need lines.
    /// </summary>
    /// <returns>Data size of the vertex-set (in bytes).</returns>
    public virtual unsafe int LineVertices (
      ref float* ptr,
      ref uint origin,
      out int stride,
      bool txt,
      bool col,
      bool normal,
      bool ptsize)
    {
      stride = 3;
      if (txt)
        stride += 2;
      if (col)
        stride += 3;
      if (normal)
        stride += 3;
      if (ptsize)
        stride++;

      origin += LinVertices;

      return (int)LinVertices * (stride *= sizeof(float));
    }

    /// <summary>
    /// Fills simple nonshared line indices, counts the buffer size.
    /// Necessary to override if you need shared vertices.
    /// </summary>
    /// <returns>Data size of the index-set (in bytes).</returns>
    public virtual unsafe int LineIndices (
      ref uint* ptr,
      uint origin)
    {
      int indices = (int) Lines * 2;
      if (ptr != null)
        for (int i = 0; i++ < indices;)
          *ptr++ = origin++;

      return indices * sizeof(uint);
    }

    /// <summary>
    /// Number of points to render (0 by default).
    /// </summary>
    public virtual uint Points => 0;

    /// <summary>
    /// Doesn't fill any vertex data, only counts the stride and the total buffer size.
    /// Necessary to override if you need point-sprites.
    /// </summary>
    /// <returns>Data size of the vertex-set (in bytes).</returns>
    public virtual unsafe int PointVertices (
      ref float* ptr,
      ref uint origin,
      out int stride,
      bool txt,
      bool col,
      bool normal,
      bool ptsize)
    {
      stride = 3;
      if (txt)
        stride += 2;
      if (col)
        stride += 3;
      if (normal)
        stride += 3;
      if (ptsize)
        stride++;

      origin += Points;

      return (int)Points * (stride *= sizeof(float));
    }
  }

  /// <summary>
  /// Simple 3D coordinate axis object for debugging purposes.
  /// </summary>
  public class CoordinateAxes : DefaultRenderObject
  {
    /// <summary>
    /// Tick length for all axes.
    /// </summary>
    public float tick;

    /// <summary>
    /// X-axis length in ticks.
    /// </summary>
    public int ticksX;

    /// <summary>
    /// Y-axis length in ticks.
    /// </summary>
    public int ticksY;

    /// <summary>
    /// Z-axis length in ticks.
    /// </summary>
    public int ticksZ;

    /// <summary>
    /// X-axis color.
    /// </summary>
    public Vector3 colorX;

    /// <summary>
    /// Y-axis color.
    /// </summary>
    public Vector3 colorY;

    /// <summary>
    /// Z-axis color.
    /// </summary>
    public Vector3 colorZ;

    public CoordinateAxes (
      Vector3 colX,
      Vector3 colY,
      Vector3 colZ,
      float t = 1.0f,
      int tX = 5,
      int tY = 5,
      int tZ = 5)
    {
      colorX = colX;
      colorY = colY;
      colorZ = colZ;
      tick   = t;
      ticksX = Math.Max(tX, 1);
      ticksY = Math.Max(tY, 1);
      ticksZ = Math.Max(tZ, 1);
    }

    public CoordinateAxes (
      float t = 1.0f,
      int tX = 5,
      int tY = 5,
      int tZ = 5)

      : this(
          new Vector3(1.0f, 0.3f, 0.0f),
          new Vector3(0.0f, 0.8f, 0.2f),
          new Vector3(0.2f, 0.4f, 1.0f),
          t, tX, tY, tZ)
    {}

    //--- rendering ---

    public override uint Lines => (uint)(ticksX + ticksY + ticksZ);

    /// <summary>
    /// Lines: returns vertex-array size (if ptr is null) or fills vertex array.
    /// </summary>
    /// <param name="ptr">Data pointer (null for determining buffer size).</param>
    /// <param name="origin">Index number in the global vertex array.</param>
    /// <param name="stride">Vertex size (stride) in bytes.</param>
    /// <param name="col">Use color attribute?</param>
    /// <param name="txt">Use txtCoord attribute?</param>
    /// <param name="normal">Use normal vector attribute?</param>
    /// <param name="ptsize">Use point-size/line-width attribute?</param>
    /// <returns>Data size of the vertex-set (in bytes).</returns>
    public override unsafe int LineVertices (
      ref float* ptr,
      ref uint origin,
      out int stride,
      bool txt,
      bool col,
      bool normal,
      bool ptsize)
    {
      int total = base.LineVertices( ref ptr, ref origin, out stride, txt, col, normal, ptsize);
      if (ptr == null)
        return total;

      int   i;
      float coord, s;
      float tickSize = tick * 0.08f;

      // 1. X-axis
      if (txt)
        Fill(ref ptr, 0.0f, 0.0f);
      if (col)
        Fill(ref ptr, ref colorX);
      if (normal)
        Fill(ref ptr, Vector3.UnitZ);
      if (ptsize)
        *ptr++ = 1.0f;
      Fill(ref ptr, Vector3.Zero);

      if (txt)
        Fill(ref ptr, 1.0f, 0.0f);
      if (col)
        Fill(ref ptr, ref colorX);
      if (normal)
        Fill(ref ptr, Vector3.UnitZ);
      if (ptsize)
        *ptr++ = 1.0f;
      Fill(ref ptr, (tick * ticksX) * Vector3.UnitX);

      // X-ticks
      for (i = 1; i < ticksX; i++)
      {
        coord = i * tick;
        s = i / (float)ticksX;

        if (txt)
          Fill(ref ptr, s, 0.0f);
        if (col)
          Fill(ref ptr, ref colorX);
        if (normal)
          Fill(ref ptr, Vector3.UnitZ);
        if (ptsize)
          *ptr++ = 1.0f;
        Fill(ref ptr, new Vector3(coord, 0.0f, 0.0f));

        if (txt)
          Fill(ref ptr, s, 0.1f);
        if (col)
          Fill(ref ptr, ref colorX);
        if (normal)
          Fill(ref ptr, Vector3.UnitZ);
        if (ptsize)
          *ptr++ = 1.0f;
        Fill(ref ptr, new Vector3(coord, tickSize, 0.0f));
      }

      // 2. Y-axis
      if (txt)
        Fill(ref ptr, 0.0f, 0.0f);
      if (col)
        Fill(ref ptr, ref colorY);
      if (normal)
        Fill(ref ptr, Vector3.UnitX);
      if (ptsize)
        *ptr++ = 1.0f;
      Fill(ref ptr, Vector3.Zero);

      if (txt)
        Fill(ref ptr, 1.0f, 0.0f);
      if (col)
        Fill(ref ptr, ref colorY);
      if (normal)
        Fill(ref ptr, Vector3.UnitX);
      if (ptsize)
        *ptr++ = 1.0f;
      Fill(ref ptr, (tick * ticksY) * Vector3.UnitY);

      // Y-ticks
      for (i = 1; i < ticksY; i++)
      {
        coord = i * tick;
        s = i / (float)ticksY;

        if (txt)
          Fill(ref ptr, s, 0.0f);
        if (col)
          Fill(ref ptr, ref colorY);
        if (normal)
          Fill(ref ptr, Vector3.UnitX);
        if (ptsize)
          *ptr++ = 1.0f;
        Fill(ref ptr, new Vector3(0.0f, coord, 0.0f));

        if (txt)
          Fill(ref ptr, s, 0.1f);
        if (col)
          Fill(ref ptr, ref colorY);
        if (normal)
          Fill(ref ptr, Vector3.UnitX);
        if (ptsize)
          *ptr++ = 1.0f;
        Fill(ref ptr, new Vector3(0.0f, coord, tickSize));
      }

      // 3. Z-axis
      if (txt)
        Fill(ref ptr, 0.0f, 0.0f);
      if (col)
        Fill(ref ptr, ref colorZ);
      if (normal)
        Fill(ref ptr, Vector3.UnitY);
      if (ptsize)
        *ptr++ = 1.0f;
      Fill(ref ptr, Vector3.Zero);

      if (txt)
        Fill(ref ptr, 1.0f, 0.0f);
      if (col)
        Fill(ref ptr, ref colorZ);
      if (normal)
        Fill(ref ptr, Vector3.UnitY);
      if (ptsize)
        *ptr++ = 1.0f;
      Fill(ref ptr, (tick * ticksZ) * Vector3.UnitZ);

      // Z-ticks
      for (i = 1; i < ticksZ; i++)
      {
        coord = i * tick;
        s = i / (float)ticksZ;

        if (txt)
          Fill(ref ptr, s, 0.0f);
        if (col)
          Fill(ref ptr, ref colorZ);
        if (normal)
          Fill(ref ptr, Vector3.UnitY);
        if (ptsize)
          *ptr++ = 1.0f;
        Fill(ref ptr, new Vector3(0.0f, 0.0f, coord));

        if (txt)
          Fill(ref ptr, s, 0.1f);
        if (col)
          Fill(ref ptr, ref colorZ);
        if (normal)
          Fill(ref ptr, Vector3.UnitY);
        if (ptsize)
          *ptr++ = 1.0f;
        Fill(ref ptr, new Vector3(tickSize, 0.0f, coord));
      }

      return total;
    }
  }

  public static class Transformations
  {
    /// <summary>
    /// Applies transformation matrix to vector
    /// </summary>
    /// <param name="vector">1x3 vector</param>
    /// <param name="transformation">4x4 transformation matrix</param>
    /// <returns>Transformed vector 1x3</returns>
    public static Vector3d ApplyTransformation (Vector3d vector, Matrix4d transformation)
    {
      return Vector3d.TransformPosition(vector, transformation);
      /*
      Vector4d transformedVector = MultiplyVectorByMatrix(new Vector4d(vector, 1), transformation); // ( vector, 1 ) is extenstion [x  y  z] -> [x  y  z  1]

      return new Vector3d ( transformedVector.X / transformedVector.W, //[x  y  z  w] -> [x/w  y/w  z/w]
                            transformedVector.Y / transformedVector.W,
                            transformedVector.Z / transformedVector.W );
      */
    }

    /*
    public static Vector3d ApplyTransformation ( Vector3d vector, Matrix4 transformation )
    {
      Vector4d transformedVector = MultiplyVectorByMatrix ( new Vector4d ( vector, 1 ), transformation ); //( vector, 1 ) is extenstion [x  y  z] -> [x  y  z  1]

      return new Vector3d ( transformedVector.X / transformedVector.W, //[x  y  z  w] -> [x/w  y/w  z/w]
                            transformedVector.Y / transformedVector.W,
                            transformedVector.Z / transformedVector.W );
    }

    /// <summary>
    /// Multiplication of Vector4d and Matrix4d
    /// </summary>
    /// <param name="vector">Vector 1x4 on left side</param>
    /// <param name="matrix">Matrix 4x4 on right side</param>
    /// <returns>Result of multiplication - 1x4 vector</returns>
    public static Vector4d MultiplyVectorByMatrix ( Vector4d vector, Matrix4d matrix )
    {
      Vector4d result = new Vector4d (0, 0, 0, 0);

      for ( int i = 0; i < 4; i++ )
      {
        for ( int j = 0; j < 4; j++ )
        {
          result[i] += vector[j] * matrix[j, i];
        }
      }

      return result;
    }

    public static Vector4d MultiplyVectorByMatrix ( Vector4d vector, Matrix4 matrix )
    {
      Vector4d result = new Vector4d (0, 0, 0, 0);

      for ( int i = 0; i < 4; i++ )
      {
        for ( int j = 0; j < 4; j++ )
        {
          result[i] += vector[j] * matrix[j, i];
        }
      }

      return result;
    }
    */
  }
}
