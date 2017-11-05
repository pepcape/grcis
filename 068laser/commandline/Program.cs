using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Raster;
using Utilities;

namespace _068laser
{
  public class CmdOptions : Options
  {
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static new CmdOptions options = (CmdOptions)(Options.options = new CmdOptions());

    public override void StringStatistics ( long[] result )
    {
      if ( result == null || result.Length < 4 )
        return;

      Util.StringStat( commands, result );
    }

    static CmdOptions ()
    {
      project = "cmd068";
      TextPersistence.Register( new CmdOptions(), 0 );

      RegisterMsgModes( "debug" );
    }

    public CmdOptions ()
    {
      // default values of structured members:
      baseDir = @"./input/";
    }

    public static void Touch ()
    {
      if ( options == null )
        Util.Log( "EvalOptions not initialized!" );
    }

    //--- project-specific options ---

    public string outDir = @"./output/";

    public string param = "";

    public string name = "pilot";

    /// <summary>
    /// Parse additional keys.
    /// </summary>
    /// <param name="key">Key string (non-empty, trimmed).</param>
    /// <param name="value">Value string (non-null, trimmed).</param>
    /// <returns>True if recognized.</returns>
    public override bool AdditionalKey ( string key, string value, string line )
    {
      if ( base.AdditionalKey( key, value, line ) )
        return true;

      //int newInt = 0;
      //double newDouble = 0.0;

      switch ( key )
      {
        case "outDir":
          outDir = value;
          break;

        case "image":
          if ( File.Exists( value ) )
            inputFiles.Add( value );
          else
          if ( File.Exists( baseDir + value ) )
            inputFiles.Add( baseDir + value );
          else
            Console.WriteLine( $"Warning: ignoring nonexistent image '{value}' ({FileLineNo()})" );
          break;

        case "name":
          name = value;
          break;

        case "param":
          param = value;
          break;

        default:
          return false;
      }

      return true;
    }

    /// <summary>
    /// How to handle the "key=" config line?
    /// </summary>
    /// <returns>True if config line was handled.</returns>
    public override bool HandleEmptyValue ( string key )
    {
      switch ( key )
      {
        case "image":
          inputFiles.Clear();
          return true;
      }

      return false;
    }

    /// <summary>
    /// How to handle the non-key-value config line?
    /// </summary>
    /// <param name="line">The nonempty config line.</param>
    /// <returns>True if config line was handled.</returns>
    public override bool HandleCommand ( string line )
    {
      switch ( line )
      {
        case "compute":
          Program.Compute();
          return true;
      }

      return false;
    }
  }

  class Form1
  {
    public static bool cont = true;
  }

  class Program
  {
    /// <summary>
    /// Defines program version (Rev#).
    /// </summary>
    static Program ()
    {
      Util.SetVersion( "$Rev$" );
    }

    static void Main ( string[] args )
    {
      CmdOptions.Touch();
      string tooltip;
      Dither.InitParams( out CmdOptions.options.param, out tooltip, out CmdOptions.options.name );
      Draw.SetPens( 100 );

      if ( args.Length < 1 )
        Console.WriteLine( "Warning: no command-line options, using default values!" );
      else
        for ( int i = 0; i < args.Length; i++ )
          if ( !string.IsNullOrEmpty( args[ i ] ) )
          {
            string opt = args[ i ];
            if ( !CmdOptions.options.ParseOption( args, ref i ) )
              Console.WriteLine( $"Warning: invalid option '{opt}'!" );
          }

      //--- do the job ---
      if ( !wasComputed )
        Compute();
    }

    static bool wasComputed = false;

    static Stopwatch sw = new Stopwatch();

    static public void Compute ()
    {
      wasComputed = true;
      foreach ( var imageFn in CmdOptions.options.inputFiles )
      {
        Bitmap inp = null;
        try
        {
          inp = (Bitmap)Image.FromFile( imageFn );
        }
        catch ( Exception )
        {
        }

        if ( inp == null )
          inp = Draw.TestImageGray( 1200, 900, 12 );

        Bitmap bmp;
        sw.Restart();

        long dots = Dither.TransformImage( inp, out bmp, inp.Width, inp.Height, CmdOptions.options.param );

        sw.Stop();
        float elapsed = 1.0e-3f * sw.ElapsedMilliseconds;

        bmp.SetResolution( 1200, 1200 );

        Util.Log( CmdOptions.options.param );
        Util.LogFormat( "Name: '{0}', input: '{1}', elapsed: {2:f3}s, dots: {3}, dps: {4}, hash: {5:X16}",
                        CmdOptions.options.name, imageFn, elapsed, Util.kmg( dots ), Util.kmg( (long)(dots / elapsed) ),
                        Draw.Hash( bmp ) );

        string fileName = CmdOptions.options.outputFileName;
        if ( string.IsNullOrEmpty( fileName ) )
          fileName = Path.GetFileName( imageFn );
        string outFn = Path.Combine( CmdOptions.options.outDir, fileName );
        string ext = Path.GetExtension( outFn );
        if ( ext.ToLower() != ".png" )
          outFn = outFn.Substring( 0, outFn.Length - ext.Length ) + ".png";
        bmp.Save( outFn, System.Drawing.Imaging.ImageFormat.Png );

        Util.LogFormat( "Output: '{0} ({1}x{2}px, {3:f1}x{4:f1}cm)' .. saved",
                        outFn, bmp.Width, bmp.Height,
                        bmp.Width * 2.54 / 1200.0, bmp.Height * 2.54 / 1200.0 );
        inp.Dispose();
        bmp.Dispose();
      }
    }
  }
}
