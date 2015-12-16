using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    public static void LogGLProperties ( bool ext =false )
    {
      // 1. OpenGL version, vendor, ..
      string version = GL.GetString( StringName.Version );
      string vendor = GL.GetString( StringName.Vendor );
      string renderer = GL.GetString( StringName.Renderer );
      string shVer = GL.GetString( StringName.ShadingLanguageVersion );
      Util.LogFormat( "OpenGL version: {0}, shading language: {1}",
                      version ?? "-", shVer ?? "-" );
      Util.LogFormat( "Vendor: {0}, driver: {1}",
                      vendor ?? "-", renderer ?? "-" );

      // 2. OpenGL parameters:
      int maxVertices = GL.GetInteger( GetPName.MaxElementsVertices );
      int maxIndices = GL.GetInteger( GetPName.MaxElementsIndices );
      string extensions = GL.GetString( StringName.Extensions );
      int extLen = (extensions == null) ? 0 : extensions.Split( ' ' ).Length;
      Util.LogFormat( "Max-vertices: {0}, max-indices: {1}, extensions: {2}",
                      maxVertices, maxIndices, extLen );

      // 3. OpenGL extensions:
      if ( ext &&
           extensions != null )
        while ( extensions.Length > 0 )
        {
          int split = Math.Min( extensions.Length, 80 ) - 1;
          while ( split < extensions.Length &&
                  !char.IsWhiteSpace( extensions[ split ] ) )
            split++;
          Util.LogFormat( "Ext: {0}", extensions.Substring( 0, split ) );
          if ( split + 1 >= extensions.Length )
            break;

          extensions = extensions.Substring( split + 1 );
        }
    }

    /// <summary>
    /// Checks OpenGL error and logs a message eventually.
    /// </summary>
    /// <param name="checkpoint">Optional checkpoint identification.</param>
    public static void LogError ( string checkpoint ="?" )
    {
      ErrorCode err = GL.GetError();
      if ( err == ErrorCode.NoError )
        return;

      Util.LogFormat( "OpenGL error {0} at {1}", err, checkpoint );
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

    public GlShaderInfo ( ShaderType st, string source, string hint =null )
    {
      type = st;
      sourceFile = source;
      hintDir = hint;
      shader = null;
    }

    public bool Compile ()
    {
      shader = new GlShader();
      if ( !shader.CompileShader( type, sourceFile, hintDir ) )
      {
        Util.LogFormat( "{0} compile error: {1} .. giving up", type.ToString(), shader.Message );
        shader.Dispose();
        shader = null;
        return false;
      }
      return true;
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

    public GlProgramInfo ( string _name, IEnumerable<GlShaderInfo> si =null )
    {
      name = _name;
      program = null;
      shaders = (si == null) ? new List<GlShaderInfo>() : new List<GlShaderInfo>( si );
    }

    public bool Setup ()
    {
      bool okProgram = true;
      program = null;

      foreach ( var shaderInfo in shaders )
        if ( !shaderInfo.Compile() )
        {
          okProgram = false;
          break;
        }

      if ( okProgram )
      {
        // all shaders compiled ok, now we'll try to link them together..
        program = new GlProgram();
        foreach ( var shaderInfo in shaders )
          if ( !program.AttachShader( shaderInfo.shader ) )
          {
            Util.LogFormat( "GLSL program attach error: {0}", program.Message );
            okProgram = false;
            break;
          }

        if ( okProgram )
        {
          okProgram = program.Link();
          if ( okProgram )
            program.LogProgramInfo();
          else
            Util.LogFormat( "GLSL program link error: {0}", program.Message );
        }
      }

      if ( !okProgram )
      {
        if ( program != null )
        {
          program.Dispose();
          program = null;
        }
        foreach ( var shaderInfo in shaders )
          if ( shaderInfo.shader != null )
          {
            shaderInfo.shader.Dispose();
            shaderInfo.shader = null;
          }
      }

      return okProgram;
    }
  }

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
      if ( Id < 0 )
        return;

      GL.DeleteShader( Id );
      Id = -1;
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
        Status = 0;
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
        Status = 0;
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
    public string Name = "";
    public int Address = -1;
    public int Size = 0;
    public ActiveUniformType Type;
  }

  public class AttributeInfo
  {
    public string Name = "";
    public int Address = -1;
    public int Size = 0;
    public ActiveAttribType Type;
  }

  public class GlProgram : IDisposable
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

    public GlProgram ( string name ="default" )
    {
      Name = name;
      Id = GL.CreateProgram();
    }

    public void Dispose ()
    {
      if ( Id < 0 )
        return;

      foreach ( var shader in shaders.Values )
        GL.DetachShader( Id, shader.Id );
      GL.DeleteProgram( Id );
      Id = -1;
    }

    public bool AttachShader ( GlShader shader )
    {
      if ( Id < 0 ||
           shader == null ||
           shader.Id < 0 )
      {
        Message = "AttachShader: invalid program or shader.";
        Status = 0;
        return false;
      }

      GlShader old = null;
      shaders.TryGetValue( shader.Type, out old );
      if ( old != null )
      {
        GL.DetachShader( Id, old.Id );
        old.Dispose();
      }

      GL.AttachShader( Id, shader.Id );

      // ??? How to test if everything went well ???
      Status = 1;
      Message = GL.GetProgramInfoLog( Id );

      shaders[ shader.Type ] = shader;

      return true;
    }

    public bool Link ()
    {
      if ( Id < 0 )
      {
        Message = "Link: invalid program.";
        Status = 0;
        return false;
      }

      GL.LinkProgram( Id );
      GL.GetProgram( Id, GetProgramParameterName.LinkStatus, out Status );
      Message = GL.GetProgramInfoLog( Id );

      if ( Status == 0 )
        return false;

      GL.ValidateProgram( Id );
      GL.GetProgram( Id, GetProgramParameterName.ValidateStatus, out Status );
      Message = GL.GetProgramInfoLog( Id );

      if ( Status == 0 )
        return false;

      int attrCount, uniformCount;
      GL.GetProgram( Id, GetProgramParameterName.ActiveAttributes, out attrCount );
      GL.GetProgram( Id, GetProgramParameterName.ActiveUniforms, out uniformCount );

      int i, len;
      StringBuilder name = new StringBuilder();

      // attributes:
      attributes.Clear();
      for ( i = 0; i < attrCount; i++ )
      {
        AttributeInfo info = new AttributeInfo();
        len = 0;
        name.Clear();

        GL.GetActiveAttrib( Id, i, 256, out len, out info.Size, out info.Type, name );
        info.Name = name.ToString();
        info.Address = GL.GetAttribLocation( Id, info.Name );
        attributes.Add( info.Name, info );
      }

      // uniforms:
      uniforms.Clear();
      for ( i = 0; i < uniformCount; i++ )
      {
        UniformInfo info = new UniformInfo();
        len = 0;
        name.Clear();

        GL.GetActiveUniform( Id, i, 256, out len, out info.Size, out info.Type, name );
        info.Name = name.ToString();
        info.Address = GL.GetUniformLocation( Id, info.Name );
        uniforms.Add( info.Name, info );
      }

      return true;
    }

    public void EnableVertexAttribArrays ()
    {
      foreach ( var attr in attributes.Values )
        GL.EnableVertexAttribArray( attr.Address );
    }

    public void DisableVertexAttribArrays ()
    {
      foreach ( var attr in attributes.Values )
        GL.DisableVertexAttribArray( attr.Address );
    }

    public bool HasAttribute ( string name )
    {
      return attributes.ContainsKey( name );
    }

    static HashSet<string> unknownNames = new HashSet<string>();

    public int GetAttribute ( string name )
    {
      AttributeInfo ai;
      if ( attributes.TryGetValue( name, out ai ) )
        return ai.Address;

      if ( !unknownNames.Contains( name ) )
      {
        Util.LogFormat( "GetAttribute: unknown attribute '{0}'", name );
        unknownNames.Add( name );
      }

      return -1;
    }

    public int GetUniform ( string name )
    {
      UniformInfo ui;
      if ( uniforms.TryGetValue( name, out ui ) )
        return ui.Address;

      if ( !unknownNames.Contains( name ) )
      {
        Util.LogFormat( "GetUniform: unknown uniform '{0}'", name );
        unknownNames.Add( name );
      }

      return -1;
    }

    public void LogProgramInfo ()
    {
      Util.LogFormat( "GLSL program '{0}': {1}, shaders: {2}",
                      Name, Id, shaders.Count );
      foreach ( var shader in shaders.Values )
        Util.LogFormat( "  {0}: {1}",
                        shader.Type, shader.Id );

      // program attributes:
      Util.LogFormat( "Attributes[ {0} ]:", attributes.Count );
      foreach ( var attr in attributes.Values )
        Util.LogFormat( "  {0}: {1}, {2}, {3}",
                        attr.Name, attr.Address, attr.Type, attr.Size );

      // program uniforms:
      Util.LogFormat( "Uniforms[ {0} ]:", uniforms.Count );
      foreach ( var uni in uniforms.Values )
        Util.LogFormat( "  {0}: {1}, {2}, {3}",
                        uni.Name, uni.Address, uni.Type, uni.Size );
    }
  }
}
