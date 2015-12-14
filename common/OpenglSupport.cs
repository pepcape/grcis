using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using Utilities;
using System.Collections.Generic;

namespace OpenglSupport
{
  /// <summary>
  /// GLSL shader object.
  /// </summary>
  public class GlShader : IDisposable
  {
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
      if ( Id >= 0 )
      {
        GL.DeleteShader( Id );
        Id = -1;
      }
    }

    protected bool CompileShader ()
    {
      GL.CompileShader( Id );
      GL.GetShader( Id, ShaderParameter.CompileStatus, out Status );
      Message = GL.GetShaderInfoLog( Id );
      return( Status != 0 );
    }

    /// <summary>
    /// Compile shader from string source.
    /// </summary>
    public bool CompileShader ( ShaderType type, string source )
    {
      if ( source == null ||
           source.Length == 0 )
      {
        Status = 1;
        Message = "Empty shader source.";
        return false;
      }

      Id = GL.CreateShader( Type = type );
      GL.ShaderSource( Id, source );
      return CompileShader();
    }

    /// <summary>
    /// Compile shader from a file.
    /// </summary>
    public bool CompileShader ( ShaderType type, string fileName, string folderHint )
    {
      string fn = Util.FindSourceFile( fileName, folderHint );
      if ( fn == null )
      {
        Status = 1;
        Message = "Shader file '" + fileName + "' not found.";
        return false;
      }

      string source = null;
      using ( StreamReader sr = new StreamReader( fn ) )
        source = sr.ReadToEnd();

      return CompileShader( type, source );
    }
  }

  public class UniformInfo
  {
    public string name = "";
    public int address = -1;
    public int size = 0;
    public ActiveUniformType type;
  }

  public class AttributeInfo
  {
    public string name = "";
    public int address = -1;
    public int size = 0;
    public ActiveAttribType type;
  }

  public class GlProgram : IDisposable
  {
    Dictionary<ShaderType, GlShader> shaders = new Dictionary<ShaderType, GlShader>();
    Dictionary<string, AttributeInfo> attributes = new Dictionary<string, AttributeInfo>();
    Dictionary<string, UniformInfo> uniforms = new Dictionary<string, UniformInfo>();
    Dictionary<string, uint> buffers = new Dictionary<string, uint>();
  }

}
