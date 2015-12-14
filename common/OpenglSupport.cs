using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using Utilities;
using System.Collections.Generic;
using System.Text;

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
    Dictionary<ShaderType, GlShader> shaders = new Dictionary<ShaderType, GlShader>();
    Dictionary<string, AttributeInfo> attributes = new Dictionary<string, AttributeInfo>();
    Dictionary<string, UniformInfo> uniforms = new Dictionary<string, UniformInfo>();
    Dictionary<string, uint> buffers = new Dictionary<string, uint>();

    /// <summary>
    /// Program Id by CreateProgram()
    /// </summary>
    public int Id = -1;

    /// <summary>
    /// Nonzero value if ok.
    /// </summary>
    public int Status = 0;

    public string Message = "Ok.";

    public GlProgram ()
    {
      Id = GL.CreateProgram();
    }

    public void Dispose ()
    {
      foreach ( var shader in shaders.Values )
        shader.Dispose();
      shaders.Clear();

      if ( Id >= 0 )
      {
        GL.DeleteProgram( Id );
        Id = -1;
      }
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
      GL.GetProgram( Id, ProgramParameter.LinkStatus, out Status );
      Message = GL.GetProgramInfoLog( Id );

      if ( Status == 0 )
        return false;

      int attrCount, uniformCount;
      GL.GetProgram( Id, ProgramParameter.ActiveAttributes, out attrCount );
      GL.GetProgram( Id, ProgramParameter.ActiveUniforms, out uniformCount );

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

    public bool GenBuffers ()
    {
      buffers.Clear();

      foreach ( var attr in attributes.Values )
      {
        uint buffer = 0;
        GL.GenBuffers( 1, out buffer );
        buffers.Add( attr.Name, buffer );
      }

      foreach ( var uni in uniforms.Values )
      {
        uint buffer = 0;
        GL.GenBuffers( 1, out buffer );
        buffers.Add( uni.Name, buffer );
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

    public int GetAttribute ( string name )
    {
      AttributeInfo ai;
      if ( attributes.TryGetValue( name, out ai ) )
        return ai.Address;
      return -1;
    }

    public int GetUniform ( string name )
    {
      UniformInfo ui;
      if ( uniforms.TryGetValue( name, out ui ) )
        return ui.Address;

      return -1;
    }

    public uint GetBuffer ( string name )
    {
      uint address = 0;
      buffers.TryGetValue( name, out address );
      return address;
    }
  }
}
