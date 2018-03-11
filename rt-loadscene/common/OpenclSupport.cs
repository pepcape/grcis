using System;
using System.IO;
using System.Runtime.InteropServices;
using Cloo;
using Utilities;

public static class OpenGL
{
  [DllImport( "opengl32.dll" )]
  extern public static IntPtr wglGetCurrentDC ();
}

namespace OpenclSupport
{
  public static class ClInfo
  {
    /// <summary>
    /// Logs OpenCL context properties.
    /// </summary>
    /// <param name="ext">Print detailed list of extensions as well?</param>
    public static void LogCLProperties ( ComputeContext context, bool ext =true )
    {
      ComputePlatform platform = context.Platform;
      Util.LogFormat( "OpenCL host: {0}, platform: {1}",
                      Environment.OSVersion, platform.Name );
      Util.LogFormat( "Vendor: {0}, version: {1}, profile: {2}",
                      platform.Vendor, platform.Version, platform.Profile );
      // optional extension list:
      if ( ext )
        Util.LogFormat( "Extensions: {0}",
                        string.Join( ", ", platform.Extensions ) );

      // device list:
      Util.Log( "OpenCL devices:" );
      foreach ( ComputeDevice device in context.Devices )
      {
        Util.LogFormat( "Name: {0}", device.Name );
        Util.LogFormat( "  Vendor: {0}", device.Vendor );
        Util.LogFormat( "  Driver version: {0}", device.DriverVersion );
        Util.LogFormat( "  OpenCL version: {0}", device.Version );
        Util.LogFormat( "  Compute units: {0}", device.MaxComputeUnits );
        Util.LogFormat( "  Global memory: {0} bytes", device.GlobalMemorySize );
        Util.LogFormat( "  Local memory: {0} bytes", device.LocalMemorySize );
        Util.LogFormat( "  Image support: {0}", device.ImageSupport );
        Util.LogFormat( "  Extensions: {0}", device.Extensions.Count );
        if ( ext )
          Util.LogFormat( "    {0}", string.Join( ", ", device.Extensions ) );
      }
    }

    public static bool ExtensionCheck ( ComputeContext context, string extName )
    {
      if ( context == null ||
           string.IsNullOrEmpty( extName ) )
        return false;

      ComputeDevice device = context.Devices[ 0 ];
      return device != null &&
             device.Extensions.Contains( extName );
    }

    /// <summary>
    /// Reads source code from a disk file.
    /// </summary>
    public static string ReadSourceFile ( string fileName, string folderHint )
    {
      string fn = Util.FindSourceFile( fileName, folderHint );
      if ( fn == null )
        return null;

      string source = null;
      using ( StreamReader sr = new StreamReader( fn ) )
        source = sr.ReadToEnd();

      return source;
    }
  }
}
