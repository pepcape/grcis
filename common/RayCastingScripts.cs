using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Rendering
{
  /// <summary>
  /// Delegate used for GUI messaging.
  /// </summary>
  public delegate void StringDelegate ( string msg );

  /// <summary>
  /// CSscripting support functions.
  /// </summary>
  public class Scripts
  {
    /// <summary>
    /// Reads additional scenes defined in a command-line arguments.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <param name="repo">Existing scene repository to be modified (sceneName -&gt; sceneDelegate | scriptFileName).</param>
    /// <returns>How many scenes were found.</returns>
    public static int ReadFromConfig ( string[] args, Dictionary<string, object> repo )
    {
      // <sceneFile.cs>
      // -scene <sceneFile>
      // -mask <sceneFile-mask>
      // -dir <directory>
      // more options to add? (super-sampling factor, output image file-name, output resolution, rendering flags, ..)

      int count = 0;
      for ( int i = 0; i < args.Length; i++ )
      {
        if ( string.IsNullOrEmpty( args[ i ] ) )
          continue;

        string fileName = null;   // file-name or file-mask
        string dir = null;        // directory

        if ( args[ i ][ 0 ] != '-' )
        {
          if ( File.Exists( args[ i ] ) )
            fileName = Path.GetFullPath( args[ i ] );
        }
        else
        {
          string opt = args[ i ].Substring( 1 );
          if ( opt == "nodefault" )
            repo.Clear();
          else if ( opt == "scene" && i + 1 < args.Length )
          {
            if ( File.Exists( args[ ++i ] ) )
              fileName = Path.GetFullPath( args[ i ] );
          }
          else if ( opt == "dir" && i + 1 < args.Length )
          {
            if ( Directory.Exists( args[ ++i ] ) )
            {
              dir = Path.GetFullPath( args[ i ] );
              fileName = "*.cs";
            }
          }
          else if ( opt == "mask" && i + 1 < args.Length )
          {
            dir = Path.GetFullPath( args[ ++i ] );
            fileName = Path.GetFileName( dir );
            dir = Path.GetDirectoryName( dir );
          }

          // Here new commands will be handled..
          // else if ( opt == 'xxx' ..
        }

        if ( !string.IsNullOrEmpty( dir ) )
        {
          if ( !string.IsNullOrEmpty( fileName ) )
          {
            // valid dir & file-mask:
            try
            {
              string[] search = Directory.GetFiles( dir, fileName );
              foreach ( string fn in search )
              {
                string path = Path.GetFullPath( fn );
                if ( File.Exists( path ) )
                {
                  string key = Path.GetFileName( path );
                  if ( key.EndsWith( ".cs" ) )
                    key = key.Substring( 0, key.Length - 3 );

                  repo[ "* " + key ] = path;
                  count++;
                }
              }
            }
            catch ( IOException )
            {
              Console.WriteLine( $"Warning: I/O error in dir/mask command: '{dir}'/'{fileName}'" );
            }
            catch ( UnauthorizedAccessException )
            {
              Console.WriteLine( $"Warning: access error in dir/mask command: '{dir}'/'{fileName}'" );
            }
          }
        }
        else if ( !string.IsNullOrEmpty( fileName ) )
        {
          // single scene file:
          try
          {
            string path = Path.GetFullPath( fileName );
            if ( File.Exists( path ) )
            {
              string key = Path.GetFileName( path );
              if ( key.EndsWith( ".cs" ) )
                key = key.Substring( 0, key.Length - 3 );

              repo[ "* " + key ] = path;
              count++;
            }
          }
          catch ( IOException )
          {
            Console.WriteLine( $"Warning: I/O error in scene command: '{fileName}'" );
          }
          catch ( UnauthorizedAccessException )
          {
            Console.WriteLine( $"Warning: access error in scene command: '{fileName}'" );
          }
        }
      }

      return count;
    }

    public class Globals
    {
      /// <summary>
      /// Scene name (not used yet, might be useful).
      /// </summary>
      public string sceneName;

      /// <summary>
      /// Scene object to be filled.
      /// </summary>
      public IRayScene scene;

      /// <summary>
      /// Optional text parameter (usually from form's 'Params:' field).
      /// </summary>
      public string param;

      /// <summary>
      /// Parameter map defined by the script.
      /// </summary>
      public Dictionary<string, object> outParam;
    }

    protected static int count = 0;

    /// <summary>
    /// Compute a scene based on general description object 'definition' (one of delegate functions or CSscript file-name).
    /// </summary>
    /// <param name="name">Readable short scene name.</param>
    /// <param name="definition">Scene definition object.</param>
    /// <param name="par">Text parameter (from form's text field..).</param>
    /// <param name="message">Message function</param>
    /// <returns>New initialized instance of a IRayScene object.</returns>
    public static IRayScene SceneFromObject ( DefaultRayScene sc, string name, object definition, string par,
                                              InitSceneDelegate defaultScene, StringDelegate message =null, Dictionary<string, object> outPar =null )
    {
      InitSceneDelegate isd = definition as InitSceneDelegate;
      InitSceneParamDelegate ispd = definition as InitSceneParamDelegate;
      string scriptFileName = definition as string;
      string scriptSource = null;

      if ( !string.IsNullOrEmpty( scriptFileName ) &&
           File.Exists( scriptFileName ) )
      {
        try
        {
          scriptSource = File.ReadAllText( scriptFileName );
        }
        catch ( IOException )
        {
          Console.WriteLine( $"Warning: I/O error in scene read: '{scriptFileName}'" );
          scriptSource = null;
        }
        catch ( UnauthorizedAccessException )
        {
          Console.WriteLine( $"Warning: access error in scene read: '{scriptFileName}'" );
          scriptSource = null;
        }

        if ( !string.IsNullOrEmpty( scriptSource ) )
        {
          message?.Invoke( $"Compiling and running scene script '{name}' ({++count}).." );

          // interpret the CS-script defining the scene:
          var assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

          List<Assembly> assemblies = new List<Assembly>();
          assemblies.Add( Assembly.GetExecutingAssembly() );
          foreach ( var assemblyName in assemblyNames )
            assemblies.Add( Assembly.Load( assemblyName ) );

          List<string> imports = new List<string>();
          imports.Add( "System.Collections.Generic" );
          imports.Add( "OpenTK" );
          imports.Add( "Rendering" );
          imports.Add( "Utilities" );

          bool ok = true;
          Globals globals = new Globals { sceneName = name, scene = sc, param = par, outParam = outPar ?? new Dictionary<string, object>() };
          try
          {
            var task = CSharpScript.RunAsync( scriptSource, globals: globals, options: ScriptOptions.Default.WithReferences( assemblies ).AddImports( imports ) );
            Task.WaitAll( task );
          }
          catch ( CompilationErrorException e )
          {
            MessageBox.Show( $"Error compiling scene script: {e.Message}, using default scene", "CSscript Error" );
            ok = false;
          }

          if ( ok )
          {
            message?.Invoke( $"Script '{name}' finished ok, rendering.." );
            return globals.scene;
          }
        }

        message?.Invoke( "Using default scene.." );
        defaultScene( sc );
        return sc;
      }

      if ( isd != null )
        isd( sc );
      else
        ispd?.Invoke( sc, par );

      message?.Invoke( $"Rendering '{name}' ({++count}).." );
      return sc;
    }
  }
}
