using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;

namespace Utilities
{
  /// <summary>
  /// Assorted utilities.
  /// </summary>
  public class Util
  {
    static Util ()
    {
      SetVersion("$Rev$");
    }

    /// <summary>
    /// Program version.
    /// </summary>
    public static string ProgramVersion = "";

    /// <summary>
    /// Target (compiled against) .NET framework version.
    /// </summary>
    public static string TargetFramework = "";

    /// <summary>
    /// Currently running .NET framework version.
    /// </summary>
    public static string RunningFramework = "";

    public static string SetVersion (string revision)
    {
      GetFrameworkVersions(out TargetFramework, out RunningFramework);
      return ProgramVersion = revision.Replace('$', ' ').Trim();
    }

    public static string AssemblyVersion (Type t)
    {
      return Assembly.GetAssembly(t).GetName().Version.ToString();
    }

    public static void GetFrameworkVersions (out string targetFramework, out string runningFramework)
    {
      // .NET CLR version this program was build against.
      Assembly assembly = Assembly.GetExecutingAssembly();
      targetFramework = "Unknown";
      object[] targetFrameworkAttributes =
        assembly.GetCustomAttributes(typeof(TargetFrameworkAttribute), true);
      if (targetFrameworkAttributes.Length > 0)
      {
        if (targetFrameworkAttributes[0] is TargetFrameworkAttribute targetFrameworkAttribute)
          targetFramework = targetFrameworkAttribute.FrameworkDisplayName;
      }

      // Currently running framework version.
      // System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion()
      runningFramework = Environment.Version.ToString();
    }

    /// <summary>
    /// One day in seconds.
    /// </summary>
    public const long DAY = 24 * 3600L;

    private static readonly Lazy<bool> runningOnMono =
      new Lazy<bool>(() => Type.GetType("Mono.Runtime") != null);

    /// <summary>
    /// Is the current running runtime Mono?
    /// </summary>
    public static bool IsRunningOnMono => runningOnMono.Value;

    /// <summary>
    /// Clamp any comparable value to the given range.
    /// </summary>
    public static T Clamp<T> (T val, T min, T max) where T : IComparable<T>
    {
      if (val.CompareTo(min) < 0) return min;
      if (val.CompareTo(max) > 0) return max;
      return val;
    }

    /// <summary>
    /// Returns number of bits set in the given mask
    /// </summary>
    public static int bits (int i)
    {
      i = i - ((i >> 1) & 0x55555555);
      i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
      return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
    }

    private static Dictionary<Type, object> defaultsCache = new Dictionary<Type, object>();

    public static T DefaultValue<T> ()
    {
      object val;
      if (!defaultsCache.TryGetValue(typeof(T), out val))
      {
        // We want an Func<T> which returns the default.
        // Create that expression here.
        System.Linq.Expressions.Expression<Func<T>> e = System.Linq.Expressions.Expression.Lambda<Func<T>>(
          // The default value, always get what the *code* tells us.
          System.Linq.Expressions.Expression.Default(typeof(T)));

        // Compile and return the value.
        defaultsCache[typeof(T)] = val = e.Compile()();
      }

      return (T)val;
    }

    /// <summary>
    /// Positive value (1, yes, y, true, ano, a)
    /// </summary>
    public static bool positive (string val)
    {
      if (string.IsNullOrEmpty(val))
        return false;

      int i;
      if (int.TryParse(val, out i))
        return i > 0;

      char answ = char.ToLower(val[0]);
      return answ == 'y' || answ == 'a' || answ == 't';
    }

    /// <summary>
    /// Strictly positive value (1, yes, true, ano)
    /// </summary>
    public static bool positiveStrict (string val)
    {
      if (string.IsNullOrEmpty(val)) return false;

      val = val.ToLower();
      return val == "1" || val == "yes" || val == "true" || val == "ano";
    }

    /// <summary>
    /// Yes/no from a boolean.
    /// </summary>
    public static string YesNo (bool b)
    {
      return b ? "yes" : "no";
    }

    public static double percent (double count, double total)
    {
      return 100.0 * count / Math.Max(total, 1.0);
    }

    public static double percent (long count, long total)
    {
      return 100.0 * count / Math.Max(total, 1L);
    }

    public static double percent (int count, int total)
    {
      return 100.0 * count / Math.Max(total, 1);
    }

    /// <summary>
    /// Converts empty strings to null.
    /// </summary>
    public static string EmptyNull (string s)
    {
      return string.IsNullOrEmpty(s) ? null : s;
    }

    /// <summary>
    /// Converts empty strings to null, interns other ones.
    /// </summary>
    public static string EmptyNullIntern (string s)
    {
      return string.IsNullOrEmpty(s) ? null : string.Intern(s);
    }

    /// <summary>
    /// Converts a string into a correct file-name.
    /// </summary>
    public static string FileNameString (string s)
    {
      StringBuilder sb = new StringBuilder();

      foreach (char ch in s)
        if (ch == '/' || ch == '\\' || ch == '?' || ch == '%' ||
            ch == '*' || ch == ':' || ch == '|' || ch == '"' ||
            ch == '<' || ch == '>' || ch == ' ')
        {
          //sb.Append( '_' );
        }
        else
          sb.Append(ch);

      return sb.Length == 0 ? "_" : sb.ToString();
    }

    /// <summary>
    /// Hexa-coded color, not quoted.
    /// </summary>
    /// <returns>#RRGGBB</returns>
    public static string d3HexColor (float r, float g, float b)
    {
      return string.Format("#{0:X2}{1:X2}{2:X2}", (int)Math.Round(r), (int)Math.Round(g),
                           (int)Math.Round(b));
    }

    /// <summary>
    /// Nice color palette for D3.js.
    /// </summary>
    /// <param name="size">Number of colors needed.</param>
    /// <returns>JavaScript code.</returns>
    public static string d3ColorPalette (int size)
    {
      if (size <= 5)
        return "scale5()";
      if (size <= 10)
        return "d3.scale.category10()";
      return "d3.scale.category20()";
    }

    /// <summary>
    /// Selects between two text-color classes: green (positive) and red (negative).
    /// </summary>
    public static string GreenRedClass (bool green)
    {
      return green ? "greent" : "redt";
    }

    public static string UpDownArrow (bool green)
    {
      return green ? "&uarr;" : "&darr;";
    }

    /// <summary>
    /// Linearly interpolated color palette for D3.js.
    /// </summary>
    /// <param name="size">Number of colors needed.</param>
    /// <returns>JavaScript code.</returns>
    public static string d3LinearPalette (int size, int r1, int g1, int b1, int r2, int g2, int b2)
    {
      StringBuilder sb = new StringBuilder("d3.scale.ordinal().range([");
      sb.AppendFormat("\"{0}\"", d3HexColor(r1, g1, b1));
      if (size > 1)
      {
        size--;
        float r  = r1;
        float g  = g1;
        float b  = b1;
        float dr = ( r2 - r ) / size;
        float dg = ( g2 - g ) / size;
        float db = ( b2 - b ) / size;
        while (size-- > 0)
        {
          r += dr;
          g += dg;
          b += db;
          sb.AppendFormat(",\"{0}\"", d3HexColor(r, g, b));
        }
      }

      sb.Append("])");
      return sb.ToString();
    }

    public static string d3LinearPalette (int size, int[] colors)
    {
      return d3LinearPalette(size, colors[0], colors[1], colors[2], colors[3], colors[4], colors[5]);
    }

    public static string d3DiscretePalette (ICollection<Tuple<byte, byte, byte>> table)
    {
      StringBuilder sb   = new StringBuilder("d3.scale.ordinal().range([");
      bool          cont = false;
      foreach (var c in table)
      {
        if (cont)
          sb.Append(',');
        else
          cont = true;
        sb.AppendFormat("\"{0}\"", d3HexColor(c.Item1, c.Item2, c.Item3));
      }

      sb.Append("])");
      return sb.ToString();
    }

    public static string CompactFloatHTML (double a)
    {
      bool positive = a > 1.0e-3;
      return string.Format(CultureInfo.InvariantCulture, "{0}{1:f0}{2}",
                           positive ? "<b>" : "", a, positive ? "</b>" : "");
    }

    public static readonly DateTime DATE_EPSILON = DateTime.MinValue.AddDays(1);

    /// <summary>
    /// Converts a day-component DateTime (birth-date for example) into string.
    /// </summary>
    /// <param name="invalid">String used for invalid date.</param>
    /// <returns>String representation or invalid.</returns>
    public static string DayToString (DateTime dt, string invalid = "")
    {
      if (dt < DATE_EPSILON)
        return invalid;

      return dt.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// All accepted date formats.
    /// </summary>
    public static readonly string[] DATE_FORMATS =
    {
      "yyyy/M/d", "M/d/yyyy", "MM/dd/yyyy", "yyyy-M-d", "yyyy-d-M", "d-M-yyyy",
      "yyyy", "d.M.yyyy", "d.M. yyyy", "d. M.yyyy", "d. M. yyyy", "d.M yyyy"
    };

    /// <summary>
    /// Converts string back to a day-component DateTime.
    /// </summary>
    /// <param name="s">Empty string is converted into invalid value.</param>
    public static DateTime StringToDay (string s)
    {
      if (string.IsNullOrEmpty(s))
        return DateTime.MinValue;

      DateTime result;
      if (DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out result) ||
          DateTime.TryParseExact(s, DATE_FORMATS, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
        result = result.ToUniversalTime();

      if (result < DATE_EPSILON)
        return DateTime.MinValue;

      return result;
    }

    public static bool StringToDay (string s, out DateTime dt)
    {
      if (string.IsNullOrEmpty(s) ||
          (!DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out dt) &&
           !DateTime.TryParseExact(s, DATE_FORMATS, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dt)))
      {
        dt = DateTime.MinValue;
        return false;
      }

      dt = dt.ToUniversalTime();
      return true;
    }

    public static int CharsInString (
      string s,
      char ch)
    {
      if (string.IsNullOrEmpty(s))
        return 0;

      int count = 0;
      foreach (char c in s)
        if (c == ch)
          count++;
      return count;
    }

    public static void BitArraySet (BitArray ba, int i, int granularity = 32)
    {
      if (i >= ba.Count)
        ba.Length = (granularity < 2) ? i + 1 : ((i + granularity) / granularity) * granularity;

      ba[i] = true;
    }

    public static BitArray BitArrayTrim (BitArray ba, int granularity = 32)
    {
      if (ba == null ||
           ba.Count < 2 * granularity)
        return ba;

      int i = ba.Count;
      while (--i >= granularity && !ba[i]) ;
      i = ((i + granularity) / granularity) * granularity;
      ba.Length = i;

      return ba;
    }

    public static string BitArrayToString (BitArray ba, int granularity = 0)
    {
      if (ba == null)
        return "";

      if (granularity > 0)
        BitArrayTrim(ba, granularity);

      if (ba.Count == 0)
        return "";

      StringBuilder sb      = new StringBuilder();
      int           i       = 0;
      int           buff    = 0;
      int           mask    = 8;
      int           runLen  = 0;
      char          runChar = '0';
      string        digit;

      while (i < ba.Count)
      {
        if (ba[i++])
          buff += mask;
        mask >>= 1;
        if (mask == 0)
        {
          digit = buff.ToString("X");

          if (digit[0] == runChar)
            runLen++;
          else
          {
            // flush the old run:
            if (runLen > 0)
            {
              if (runLen > 3)
                sb.Append(runChar == '0' ? '(' : '{').Append(runLen).Append(runChar == '0' ? ')' : '}');
              else
                do
                  sb.Append(runChar);
                while (--runLen > 0);
              runLen = 0;
            }

            if (digit[0] == '0' ||
                digit[0] == 'F')
            {
              runChar = digit[0];
              runLen = 1;
            }
            else
              sb.Append(digit);
          }

          buff = 0;
          mask = 8;
        }
      }

      if (runLen > 0 &&
          ((mask != 8 && buff != 0) || runChar != '0'))
      {
        // unfinished run:
        if (runLen > 3)
          sb.Append(runChar == '0' ? '(' : '{').Append(runLen).Append(runChar == '0' ? ')' : '}');
        else
          do
            sb.Append(runChar);
          while (--runLen > 0);
      }

      if (mask != 8 &&
          buff != 0)
        // unfinished digit:
        sb.Append(buff.ToString("X"));

      return sb.ToString();
    }

    public static int HexDigit (char d)
    {
      if (d >= '0' && d <= '9')
        return d - '0';

      if (d >= 'a' && d <= 'f')
        return 10 + d - 'a';

      if (d >= 'A' && d <= 'F')
        return 10 + d - 'A';

      return 0;
    }

    public static BitArray StringToBitArray (string s, int granularity = 32)
    {
      if (string.IsNullOrEmpty(s))
        return null;

      BitArray ba = new BitArray ( granularity );
      int      digit;
      for (int i = 0, bai = 0; i < s.Length; i++)
      {
        if (s[i] == '(' ||
            s[i] == '{')
        {
          bool fillTrue = ( s [ i ] == '{' );
          int  end      = i + 1;
          while (end < s.Length && char.IsDigit(s, end))
            end++;
          int.TryParse(s.Substring(i + 1, end - i - 1), out digit);
          while (digit-- > 0)
          {
            while (bai + 4 >= ba.Count)
              ba.Length = ba.Count + granularity;
            ba[bai] =
            ba[bai + 1] =
            ba[bai + 2] =
            ba[bai + 3] = fillTrue;
            bai += 4;
          }

          i = end;
          continue;
        }

        digit = HexDigit(s[i]);
        while (bai + 4 >= ba.Count)
          ba.Length = ba.Count + granularity;
        ba[bai] = (digit & 8) > 0;
        ba[bai + 1] = (digit & 4) > 0;
        ba[bai + 2] = (digit & 2) > 0;
        ba[bai + 3] = (digit & 1) > 0;
        bai += 4;
      }

      BitArrayTrim(ba, granularity);
#if false
      if (Options.options.MsgMode("verbose"))
        if (s != BitArrayToString(ba))
          LogFormat("BitArray persistence error: {0}-{1}", s, BitArrayToString(ba));
#endif

      return ba;
    }

    public static void CheckRebuildCachedList (BitArray ba, ref List<ushort> proc)
    {
      if (ba == null ||
          (proc != null && proc.Count == ba.Count))
        return;

      int len = ba.Count;
      proc = new List<ushort>(len);
      int act = 0;
      for (int i = 0; i < len; i++)
        proc.Add((ushort)(ba.Get(i) ? (act = 0) : ++act));
    }

    /// <summary>
    /// Substitutes all tag occurences with the given value.
    /// </summary>
    /// <param name="str">Source/result string.</param>
    /// <param name="tag">Tag name.</param>
    /// <param name="value">Value to substitute (can be null).</param>
    public static string Substitute (ref string str, string tag, string value)
    {
      return str = Substitute(str, tag, value);
    }

    /// <summary>
    /// Substitutes all tag occurences with the given value.
    /// </summary>
    /// <param name="str">Source string.</param>
    /// <param name="tag">Tag name.</param>
    /// <param name="value">Value to substitute (can be null).</param>
    public static string Substitute (string str, string tag, string value)
    {
      tag = "<%" + tag + '>';
      if (!str.Contains(tag))
        return str;

      return str.Replace(tag, value ?? "");
    }

    /// <summary>
    /// Substitutes all tag occurences with the given formatted string.
    /// </summary>
    /// <param name="str">Source/result string.</param>
    /// <param name="tag">Tag name.</param>
    /// <param name="fmt">Format string.</param>
    /// <param name="values">Array of arguments.</param>
    public static string SubstituteFormat (ref string str, string tag, string fmt, params object[] values)
    {
      return str = SubstituteFormat(str, tag, fmt, values);
    }

    /// <summary>
    /// Substitutes all tag occurences with the given formatted string.
    /// </summary>
    /// <param name="str">Source string.</param>
    /// <param name="tag">Tag name.</param>
    /// <param name="fmt">Format string.</param>
    /// <param name="values">Array of arguments.</param>
    public static string SubstituteFormat (string str, string tag, string fmt, params object[] values)
    {
      tag = "<%" + tag + '>';
      if (!str.Contains(tag))
        return str;

      fmt = string.Format(CultureInfo.InvariantCulture, fmt, values);
      return str.Replace(tag, fmt);
    }

    public static string logFileName = "log.txt";

    static bool firstLog = true;

    /// <summary>
    /// Logging (log message is always appended to the log-file).
    /// </summary>
    /// <param name="msg">The explicit message.</param>
    public static string Log (string msg)
    {
      try
      {
        lock (logFileName)
          using (StreamWriter log = new StreamWriter(logFileName, true))
          {
            if (firstLog)
              log.WriteLine();
            log.WriteLine(string.Format("{0}: {1}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), msg));
          }
      }
      catch (IOException e)
      {
        Console.WriteLine("Log - unhandled IOException: " + e.Message);
        Console.WriteLine("Stack: " + e.StackTrace);
      }

      firstLog = false;

      return msg;
    }

    /// <summary>
    /// Logging (log message is always appended to the log-file).
    /// </summary>
    /// <param name="fmt">Format string.</param>
    /// <param name="pars">Values to substitute.</param>
    public static string LogFormat (string fmt, params object[] pars)
    {
      fmt = string.Format(CultureInfo.InvariantCulture, fmt, pars);
      return Log(fmt);
    }

    /// <summary>
    /// Logging the float[] array for debugging purposes..
    /// </summary>
    public static void LogArray (string prefix, float[] rcoord, string format = "f4")
    {
      StringBuilder sbb = new StringBuilder(prefix);
      foreach (var comp in rcoord)
        sbb.AppendFormat(CultureInfo.InvariantCulture, " {0:" + format + '}', comp);
      Log(sbb.ToString());
    }

    public static void LogArray (string prefix, string[] names)
    {
      LogFormat("{0} \"{1}\"", prefix, string.Join("\" \"", names));
    }

    public static void LogArray (string prefix, bool[] flags)
    {
      StringBuilder sbb = new StringBuilder(prefix);
      foreach (var flag in flags)
        sbb.AppendFormat(" \"{0}\"", flag);
      Log(sbb.ToString());
    }

    /// <summary>
    /// Checks validity of the given IP address.
    /// </summary>
    /// <param name="ip">IP address as a string.</param>
    /// <returns>True if the IP-format is OK.</returns>
    public static bool IsIpAddress (string ip)
    {
      int len = ip.Length;
      if (len < 7 || !char.IsDigit(ip, 0))
        return false;
      int i = 1;

      for (int j = 0; j++ < 3;)
      {
        while (i < len && char.IsDigit(ip, i))
          i++;
        if (i >= len - 1 || ip[i] != '.' || !char.IsDigit(ip, i + 1))
          return false;
        i += 2;
      }

      while (i < len && char.IsDigit(ip, i))
        i++;
      return (i == len);
    }

    /// <summary>
    /// Check Id validity (integer).
    /// </summary>
    public static bool IsId (string id)
    {
      if (id == null)
        return false;

      long val;
      return long.TryParse(id, out val);
    }

    /// <summary>
    /// Returns numerical prefix of the given string.
    /// </summary>
    /// <param name="val">Non-null string.</param>
    /// <returns>Non-null string (decimal number).</returns>
    public static string NumberPrefix (string val)
    {
      int len = val.Length;
      int i   = 0;
      while (i < len && char.IsDigit(val, i))
        i++;

      return (i < len) ? val.Substring(0, i) : val;
    }

    /// <summary>
    /// Prints time value
    /// </summary>
    /// <param name="time">Time in seconds.</param>
    public static string FormatTime (long time)
    {
      long seconds = time % 60L;
      time /= 60L;
      long minutes = time % 60L;
      time /= 60L;
      long hours = time % 24L;
      time /= 24L;
      return time == 0L
        ? (hours == 0L
          ? string.Format("{0}:{1:d2}", minutes, seconds)
          : string.Format("{0}:{1:d2}:{2:d2}", hours, minutes, seconds))
        : string.Format("{0}:{1:d2}:{2:d2}:{3:d2}", time, hours, minutes, seconds);
    }

    /// <summary>
    /// Origin of the POSIX time.
    /// </summary>
    readonly static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Minimum number of seconds added to DateTime 1970
    /// </summary>
    public const long MIN_UNIX_TIME = -100L * 365L * DAY;

    /// <summary>
    /// Maximum number of seconds added to DateTime 1970
    /// </summary>
    public const long MAX_UNIX_TIME = 130L * 365L * DAY;

    /// <summary>
    /// POSIX time-stamp to DateTime conversion
    /// </summary>
    public static DateTime UnixTimeStampToDateTime (long unixTimeStamp)
    {
      // POSIX timestamp is seconds past epoch
      unixTimeStamp = Clamp(unixTimeStamp, MIN_UNIX_TIME, MAX_UNIX_TIME);
      return epoch.AddSeconds(unixTimeStamp).ToLocalTime();
    }

    /// <summary>
    /// POSIX DateTime to time-stamp conversion
    /// </summary>
    public static long DateTimeToUnixTimeStamp (DateTime dt)
    {
      // POSIX timestamp is seconds past epoch
      return (long)dt.ToUniversalTime().Subtract(epoch).TotalSeconds;
    }

    /// <summary>
    /// Now in POSIX timestamp format.
    /// </summary>
    public static long UnixTimeNow ()
    {
      return (long)DateTime.UtcNow.Subtract(epoch).TotalSeconds;
    }

    public static string FormatNowUtc (string fmt = "yyyy-MM-dd HH:mm:ss")
    {
      return DateTime.UtcNow.ToString(fmt);
    }

    /// <summary>
    /// Returns time relative to current local time.
    /// </summary>
    /// <param name="rel">Time-delta in seconds.</param>
    /// <returns>POSIX time-stamp format of the result.</returns>
    public static long RelativeTime (long rel)
    {
      DateTime now = DateTime.Now.AddSeconds ( rel );
      return DateTimeToUnixTimeStamp(now);
    }

    /// <summary>
    /// Parse relative time (w, d, h, m suffixes are allowed).
    /// </summary>
    /// <param name="value">Input value.</param>
    /// <param name="time">Output value (left unchanged if not valid input).</param>
    /// <returns>True if changed.</returns>
    public static bool ParseRelativeTime (string value, ref long time)
    {
      char suff = 's';
      int  len  = value.Length;
      if (char.IsLetter(value, len - 1))
      {
        suff = char.ToLower(value[len - 1]);
        value = value.Substring(0, len - 1);
      }

      long newLong;
      if (!long.TryParse(value, out newLong))
        return false;

      switch (suff)
      {
        case 'w':
          newLong *= 7L * DAY;
          break;
        case 'd':
          newLong *= DAY;
          break;
        case 'h':
          newLong *= 3600L;
          break;
        case 'm':
          newLong *= 60L;
          break;
      }

      time = newLong;
      return true;
    }

    public static long DecodeNumber (byte[] data, int i, int len)
    {
      if (data == null ||
          i < 0 || len < 1 ||
          data.Length < i + len)
        return 0L;

      long result = data [ i++ ];
      int  shift  = 8;
      while (--len > 0)
      {
        result |= ((long)data[i++]) << shift;
        shift += 8;
      }

      return result;
    }

    public static uint DecodeUnsigned (byte[] data, int i)
    {
      return (uint)DecodeNumber(data, i, 4);
    }

    public static long DecodeLong (byte[] data, int i)
    {
      return DecodeNumber(data, i, 8);
    }

    public static short DecodeShort (byte[] data, int i)
    {
      return (short)DecodeNumber(data, i, 2);
    }

    /// <summary>
    /// Remove diacritics from a string.
    /// </summary>
    public static string RemoveDiacritics (string stIn)
    {
      if (string.IsNullOrEmpty(stIn))
        return "";

      string        stFormD = stIn.Normalize(NormalizationForm.FormD);
      StringBuilder sb      = new StringBuilder();

      for (int ich = 0; ich < stFormD.Length; ich++)
      {
        UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
        if (uc != UnicodeCategory.NonSpacingMark)
          sb.Append(stFormD[ich]);
      }

      return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Looks for the required file in many locations.
    /// </summary>
    /// <param name="fileName">Simple file-name (w/o directories).</param>
    /// <param name="hint">Optional hint directory.</param>
    /// <returns>File-name or null if not found.</returns>
    public static string FindSourceFile (string fileName, string hint = null)
    {
      if (fileName == null ||
          fileName.Length == 0)
        return null;

      // 1. local folder
      if (File.Exists(fileName))
        return fileName;

      // 2. parent folders
      string fn = "../" + fileName;
      if (File.Exists(fn))
        return fn;

      fn = "../" + fn;
      if (File.Exists(fn))
        return fn;

      fn = "../" + fn;
      if (File.Exists(fn))
        return fn;

      if (hint != null ||
          hint.Length == 0)
        return null;

      // 3. hint folder tries
      fn = "../" + hint + '/' + fileName;
      if (File.Exists(fn))
        return fn;

      fn = "../" + fn;
      if (File.Exists(fn))
        return fn;

      fn = "../" + fn;
      if (File.Exists(fn))
        return fn;

      return null;
    }

    /// <summary>
    /// Reads the given text file-name into the string variable.
    /// </summary>
    /// <returns>True if ok.</returns>
    public static bool ReadTextFile (string filename, out string content)
    {
      if (!File.Exists(filename))
      {
        content = string.Empty;
        return false;
      }

      try
      {
        using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read))
        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, true))
          content = sr.ReadToEnd();
      }
      catch (Exception)
      {
        LogFormat("Error reading text file '{0}'!", filename);
        content = string.Empty;
        return false;
      }

      return true;
    }

    /// <summary>
    /// File-path relativization.
    /// </summary>
    /// <param name="basePath">Base path (folder).</param>
    /// <param name="path">Path to relativize.</param>
    /// <param name="sep">Dir-separation character.</param>
    /// <returns>Relative file path.</returns>
    public static string MakeRelativePath (string basePath, string path, char sep = '/')
    {
      if (string.IsNullOrEmpty(basePath))
        throw new ArgumentNullException("basePath");
      if (string.IsNullOrEmpty(path))
        throw new ArgumentNullException("path");

      Uri fromUri = new Uri(Path.GetFullPath(basePath));
      Uri toUri   = new Uri(Path.GetFullPath(path));

      //if (!fromUri.Scheme.Equals(toUri.Scheme, StringComparison.InvariantCultureIgnoreCase))
      //  return path;   // cannot relativize different schemes..

      Uri    relativeUri  = fromUri.MakeRelativeUri(toUri);
      string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

      //if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
      //  relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

      return relativePath.Replace(Path.DirectorySeparatorChar, sep);
    }

    /// <summary>
    /// Separator for the config-file lists.
    /// </summary>
    public const char COMMA = ',';

    public static string[] ParseList (string value, char sep = COMMA)
    {
      string[] list = value.Split ( sep );
      if (list.Length < 1) return null;

      for (int i = 0; i < list.Length; i++)
        list[i] = list[i].Trim();

      return list;
    }

    /// <summary>
    /// One or more Id (number) values in comma-separated list.
    /// </summary>
    public static HashSet<string> ParseIdList (string value)
    {
      HashSet<string> stringList = ParseStringList ( value );
      if (stringList == null)
        return null;

      HashSet<string> result = new HashSet<string> ();
      foreach (var id in stringList)
        if (IsId(id))
          result.Add(id);

      return (result.Count > 0) ? result : null;
    }

    /// <summary>
    /// One or more string values in comma-separated list.
    /// </summary>
    /// <param name="quotes">True to remove quotes.</param>
    public static HashSet<string> ParseStringList (string value, bool quotes = false)
    {
      if (value == null || value.Length < 1)
        return null;

      string[] list;

      if (value.Length > 2 &&
          value.StartsWith("[") &&
          value.EndsWith("]"))
        list = ParseList(value.Substring(1, value.Length - 2));
      else if (value.Contains(","))
        list = ParseList(value);
      else
        list = new string[] { value.Trim() };

      if (list == null ||
           list.Length < 1)
        return null;

      if (quotes)
        for (int i = 0; i < list.Length; i++)
          list[i] = ParseString(list[i]);

      return new HashSet<string>(list);
    }

    public static string ParseString (string value)
    {
      if (value == null)
        return null;

      int len = value.Length;
      if (len < 2)
        return value;

      if (value[0] == value[len - 1] &&
          (value[0] == '\'' || value[0] == '"'))
        return value.Substring(1, len - 2);

      return value;
    }

    /// <summary>
    /// Can be used for parsing JSON int-arrays as well.
    /// </summary>
    public static List<int> ParseIntList (string value, char sep = COMMA)
    {
      if (value == null || value.Length < 1)
        return null;

      string[] list;

      if (value.Length > 2 &&
           value.StartsWith("[") &&
           value.EndsWith("]"))
        list = ParseList(value.Substring(1, value.Length - 2), sep);
      else if (value.Contains("" + sep))
        list = ParseList(value, sep);
      else
        list = new string[] { value.Trim() };

      int len;
      if (list == null ||
           (len = list.Length) < 1)
        return null;

      List<int> result = new List<int>(len);
      int       newVal;
      for (int i = 0; i < len; i++)
        if (int.TryParse(list[i], out newVal))
          result.Add(newVal);

      return result;
    }

    /// <summary>
    /// Can be used for parsing JSON float-arrays as well.
    /// </summary>
    public static List<float> ParseFloatList (string value, char sep = COMMA)
    {
      if (value == null || value.Length < 1)
        return null;

      string[] list;

      if (value.Length > 2 &&
          value.StartsWith("[") &&
          value.EndsWith("]"))
        list = ParseList(value.Substring(1, value.Length - 2), sep);
      else if (value.Contains("" + sep))
        list = ParseList(value, sep);
      else
        list = new string[] { value.Trim() };

      int len;
      if (list == null ||
          (len = list.Length) < 1)
        return null;

      List<float> result = new List<float>(len);
      float       newVal;
      for (int i = 0; i < len; i++)
        if (float.TryParse(list[i], NumberStyles.Float, CultureInfo.InvariantCulture, out newVal))
          result.Add(newVal);

      return result;
    }

    /// <summary>
    /// Can be used for parsing JSON double-arrays as well.
    /// </summary>
    public static List<double> ParseDoubleList (string value, char sep = COMMA)
    {
      if (value == null || value.Length < 1)
        return null;

      string[] list;

      if (value.Length > 2 &&
          value.StartsWith("[") &&
          value.EndsWith("]"))
        list = ParseList(value.Substring(1, value.Length - 2), sep);
      else if (value.Contains("" + sep))
        list = ParseList(value, sep);
      else
        list = new string[] { value.Trim() };

      int len;
      if (list == null ||
          (len = list.Length) < 1)
        return null;

      List<double> result = new List<double>(len);
      double       newVal;
      for (int i = 0; i < len; i++)
        if (double.TryParse(list[i], NumberStyles.Float, CultureInfo.InvariantCulture, out newVal))
          result.Add(newVal);

      return result;
    }

    public static Tuple<byte, byte, byte> ColorTuple (string rs, string gs, string bs)
    {
      int R = 0;
      if (!string.IsNullOrWhiteSpace(rs))
        int.TryParse(rs, out R);
      int G = 0;
      if (!string.IsNullOrWhiteSpace(gs))
        int.TryParse(gs, out G);
      int B = 0;
      if (!string.IsNullOrWhiteSpace(bs))
        int.TryParse(bs, out B);

      return new Tuple<byte, byte, byte>((byte)Clamp(R, 0, 255),
                                         (byte)Clamp(G, 0, 255),
                                         (byte)Clamp(B, 0, 255));
    }

    public static T GetFirst<T> (HashSet<T> set) where T : class
    {
      if (set == null ||
          set.Count < 1)
        return null;

      HashSet<T>.Enumerator en = set.GetEnumerator();
      if (en.MoveNext())
        return en.Current;

      return null;
    }

    /// <summary>
    /// Compares two string keys first as numeric Id-s then alphabetically.
    /// </summary>
    public static int KeyComparer (string a, string b)
    {
      long ai, bi;
      if (long.TryParse(a, out ai))
      {
        if (long.TryParse(b, out bi))
          return (ai < bi) ? -1 : ((ai > bi) ? 1 : 0);
        return -1;
      }

      if (long.TryParse(b, out bi))
        return 1;

      if ("-" == a)
        return 1;

      if ("-" == b)
        return -1;

      return string.Compare(a, b);
    }

    /// <summary>
    /// Converts a comma-separated list into a dictionary of [key,value] tuples.
    /// </summary>
    /// <param name="str">String to parse.</param>
    /// <param name="separator">Optional specification of the separator character.</param>
    public static Dictionary<string, string> ParseKeyValueList (string str, char separator = COMMA)
    {
      int                        len    = str.Length;
      int                        start  = 0;
      Dictionary<string, string> result = new Dictionary<string, string>();

      while (start < len)
      {
        int end = str.IndexOf(separator, start);
        if (end == start)
        {
          start++;
          continue;
        }

        if (end < 0) end = len;
        int eq = str.IndexOf('=', start);
        if (eq != start)
        {
          if (eq < 0 || eq > end) // only key (tag) is present, assume empty value..
            eq = end;
          string value = (eq < end - 1) ? str.Substring(eq + 1, end - eq - 1) : "";
          string key   = str.Substring(start, eq - start);
          result[key.Trim()] = value.Trim();
        }

        start = end + 1;
      }

      return result;
    }

    /// <summary>
    /// Parses integer value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well.</returns>
    public static bool TryParseInt (Dictionary<string, string> rec, string key, ref int val)
    {
      string sval;
      return (rec.TryGetValue(key, out sval) &&
              int.TryParse(sval, out val));
    }

    /// <summary>
    /// Parses integer value (or 'min'-'max' range) from the dictionary.
    /// </summary>
    /// <returns>True if everything went well.</returns>
    public static bool TryParseIntRange (Dictionary<string, string> rec, string key, ref int val)
    {
      string sval;
      if (!rec.TryGetValue(key, out sval)) return false;
      int pos = sval.IndexOf('-');
      if (pos < 0)
        return int.TryParse(sval, out val);

      int  val1 = 0, val2 = 0;
      bool was1 = int.TryParse(sval.Substring(0, pos), out val1);
      bool was2 = int.TryParse(sval.Substring(pos + 1), out val2);

      if (was1)
      {
        if (was2)
          val = (val1 + val2) / 2;
        else
          val = val1;
        return true;
      }

      val = val2;
      return was2;
    }

    /// <summary>
    /// Parses integer value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well, keeps the original value otherwise.</returns>
    public static bool TryParse (Dictionary<string, string> rec, string key, ref int val)
    {
      string sval;
      int    v;
      if (rec == null ||
          !rec.TryGetValue(key, out sval) ||
          !int.TryParse(sval, out v))
        return false;

      val = v;
      return true;
    }

    /// <summary>
    /// Parses long value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well, keeps the original value otherwise.</returns>
    public static bool TryParse (Dictionary<string, string> rec, string key, ref long val)
    {
      string sval;
      long   v;
      if (rec == null ||
          !rec.TryGetValue(key, out sval) ||
          !long.TryParse(sval, out v))
        return false;

      val = v;
      return true;
    }

    /// <summary>
    /// Parses float value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well, keeps the original value otherwise.</returns>
    public static bool TryParse (Dictionary<string, string> rec, string key, ref float val)
    {
      string sval;
      float  v;
      if (rec == null ||
          !rec.TryGetValue(key, out sval) ||
          !float.TryParse(sval, NumberStyles.Number, CultureInfo.InvariantCulture, out v))
        return false;

      val = v;
      return true;
    }

    /// <summary>
    /// Parses double as a rational number (pi multiplication is supported).
    /// </summary>
    /// <returns>True if everything went well, keeps the original value otherwise.</returns>
    public static bool ParseRational (string sval, ref double val)
    {
      bool pi = false;
      if (sval.EndsWith("pi"))
      {
        // Pi multiplication.
        pi = true;
        sval = sval.Substring(0, sval.Length - 2).Trim();
      }

      double result;
      int pos = sval.IndexOf('/');
      if (pos >= 0)
      {
        // The actual fraction.
        long numerator = 1L;
        if (pos > 0)
          long.TryParse(sval.Substring(0, pos), out numerator);
        long denominator = 1L;
        if (pos + 1 < sval.Length)
          long.TryParse(sval.Substring(pos + 1), out denominator);
        if (denominator == 0L)
          denominator = 1L;

        result = numerator / (double)denominator;
      }
      else
        if (!double.TryParse(sval, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
          return false;

      val = pi ? Math.PI * result : result;
      return true;
    }

    /// <summary>
    /// Parses double value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well, keeps the original value otherwise.</returns>
    public static bool TryParse (Dictionary<string, string> rec, string key, ref double val)
    {
      string sval;
      double v;
      if (rec == null ||
          !rec.TryGetValue(key, out sval) ||
          !double.TryParse(sval, NumberStyles.Number, CultureInfo.InvariantCulture, out v))
        return false;

      val = v;
      return true;
    }

    /// <summary>
    /// Parses double value from the dictionary.
    /// Rational numbers are supported as well as pi multiplication ("pi" suffix).
    /// </summary>
    /// <returns>True if everything went well, keeps the original value otherwise.</returns>
    public static bool TryParseRational (Dictionary<string, string> rec, string key, ref double val)
    {
      string sval;
      double v = 0.0;
      if (rec == null ||
          !rec.TryGetValue(key, out sval) ||
          !ParseRational(sval, ref v))
        return false;

      val = v;
      return true;
    }

    /// <summary>
    /// Parses boolean value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well, keeps the original value otherwise.</returns>
    public static bool TryParse (Dictionary<string, string> rec, string key, ref bool val)
    {
      string sval;
      if (rec == null ||
          !rec.TryGetValue(key, out sval))
        return false;

      val = string.IsNullOrEmpty(sval) ||
            positive(sval);
      return true;
    }

    /// <summary>
    /// Parses list of doubles from the dictionary.
    /// </summary>
    /// <returns>True if everything went well, keeps the original value otherwise.</returns>
    public static bool TryParse (Dictionary<string, string> rec, string key, ref List<double> val, char sep = COMMA)
    {
      string sval;
      if (rec == null ||
          !rec.TryGetValue(key, out sval))
        return false;

      val = ParseDoubleList(sval, sep);
      return true;
    }

    /// <summary>
    /// Parses list of integers from the dictionary.
    /// </summary>
    /// <returns>True if everything went well, keeps the original value otherwise.</returns>
    public static bool TryParse (Dictionary<string, string> rec, string key, ref List<int> val, char sep = COMMA)
    {
      string sval;
      if (rec == null ||
          !rec.TryGetValue(key, out sval))
        return false;

      val = ParseIntList(sval, sep);
      return true;
    }

    /// <summary>
    /// Returns first defined value (for the given keys) or null.
    /// </summary>
    public static string FirstDefined (Dictionary<string, string> rec, string[] keys)
    {
      string val;
      foreach (var key in keys)
        if (rec.TryGetValue(key, out val))
          return val;
      return null;
    }

    /// <summary>
    /// Returns first defined nonempty value (for the given keys) or null.
    /// </summary>
    public static string FirstDefinedNonempty (Dictionary<string, string> rec, string[] keys)
    {
      string val;
      foreach (var key in keys)
        if (rec.TryGetValue(key, out val) &&
             val.Length > 0)
          return val;
      return null;
    }

    /// <summary>
    /// Returns key for the first defined nonempty value (for the given keys) or null.
    /// </summary>
    public static string FirstDefinedKey (Dictionary<string, string> rec, string[] keys)
    {
      string val;
      foreach (var key in keys)
        if (rec.TryGetValue(key, out val) &&
            val.Length > 0)
          return key;
      return null;
    }

    /// <summary>
    /// Returns first defined nonempty value only from letters (for the given keys) or null.
    /// </summary>
    public static string FirstDefinedLetters (Dictionary<string, string> rec, string[] keys)
    {
      string val;
      foreach (var key in keys)
        if (rec.TryGetValue(key, out val))
        {
          int len = val.Length;
          int i   = 0;
          while (i < len && char.IsLetter(val, i)) i++;
          if (i > 0)
            return (i == len) ? val : val.Substring(0, i);
        }

      return null;
    }

    /// <summary>
    /// Returns first defined integer value (for the given keys) or null.
    /// </summary>
    public static string FirstDefinedInteger (Dictionary<string, string> rec, string[] keys)
    {
      string val;
      foreach (var key in keys)
        if (rec.TryGetValue(key, out val))
        {
          val = NumberPrefix(val);
          if (val.Length > 0)
            return val;
        }

      return null;
    }

    public static KeyValuePair<string, string> Pair<T> (string key, T val)
    {
      return new KeyValuePair<string, string>(key, val.ToString());
    }

    /// <summary>
    /// Splits the given string into parts using '|' as separator,
    /// handles "\|" sequences correctly..
    /// </summary>
    /// <param name="str">String to be split.</param>
    /// <param name="start">Starting index.</param>
    /// <returns>List of non-null strings.</returns>
    public static List<string> BarSplit (string str, int start)
    {
      int len = str.Length;
      if (start > len)
        return null;

      List<string> res = new List<string>();
      if (start == len)
      {
        res.Add("");
        return res;
      }

      StringBuilder sb = null;
      string        seg;
      do
      {
        int pos = str.IndexOf('|', start);

        if (pos < 0)
        {
          seg = str.Substring(start);
          if (sb != null)
          {
            sb.Append(seg);
            res.Add(sb.ToString());
            sb = null;
          }
          else
            res.Add(seg);

          break;
        }

        if (pos == start)
        {
          if (sb != null)
          {
            res.Add(sb.ToString());
            sb = null;
          }
          else
            res.Add("");
        }
        else if (str[pos - 1] == '\\')
        {
          // escaped | => keep it
          if (sb == null)
            sb = new StringBuilder();
          sb.Append(str.Substring(start, pos - 1 - start)).Append('|');
        }
        else
        {
          // regular | => separator
          seg = str.Substring(start, pos - start);
          if (sb != null)
          {
            sb.Append(seg);
            res.Add(sb.ToString());
            sb = null;
          }
          else
            res.Add(seg);
        }

        start = pos + 1;
      } while (true);

      return res;
    }

    /// <summary>
    /// Converts a &-separated list into a sequence of key=value tuples.
    /// </summary>
    /// <param name="list">Result sequence, already initialized (and possibly nonempty).</param>
    /// <param name="prefix">Key-prefix.</param>
    /// <param name="str">String to parse.</param>
    /// <param name="start">Where to start..</param>
    /// <param name="separator">Optional specification of the separator character.</param>
    /// <param name="keepEmpty">Keep empty values?</param>
    public static void ParseList (
      List<KeyValuePair<string, string>> list,
      string prefix,
      string str,
      int start,
      bool keepEmpty = false,
      char separator = '&')
    {
      int len = str.Length;
      while (start < len)
      {
        int end = str.IndexOf(separator, start);
        if (end == start)
        {
          start++;
          continue;
        }

        if (end < 0) end = len;
        int eq           = str.IndexOf('=', start);
        if (eq != start)
        {
          if (eq < 0 || eq > end) // only key (tag) is present, assume empty value..
            eq = end;
          string value = ( eq < end - 1 ) ? str.Substring(eq + 1, end - eq - 1) : "";
          if (keepEmpty || value.Length > 0)
          {
            string key = str.Substring(start, eq - start);
            list.Add(Pair(prefix + key, value));
          }
        }

        start = end + 1;
      }
    }

    public static int OccupiedString (string s)
    {
      if (s == null ||
          string.IsInterned(s) != null)
        return 4;

      int occ = 8 + 4 + 2 + 2 * s.Length;
      return ((occ + 3) & -4);
    }

    public static int OccupiedStrings (IEnumerable<string> ss)
    {
      if (ss == null)
        return 0;

      int total = 0;
      foreach (string s in ss)
        total += OccupiedString(s);
      return total;
    }

    public static void StringStat (string s, long[] result)
    {
      if (s == null ||
          result == null ||
          result.Length < 4) return;

      int len = s.Length;
      result[0]++;
      result[1] += len;
      if (string.IsInterned(s) != null)
      {
        result[2]++;
        result[3] += len;
      }
    }

    public static void StringStat (IEnumerable<string> ss, long[] result)
    {
      if (ss == null ||
          result == null ||
          result.Length < 4) return;

      foreach (string s in ss)
        StringStat(s, result);
    }

    public static string kmg (long n)
    {
      if (n < 8192L) return n.ToString();
      n >>= 10;
      if (n < 8192L) return $"{n}K";
      n >>= 10;
      if (n < 8192L) return $"{n}M";
      return $"{n >> 10}G";
    }

    /// <summary>
    /// Returns list of interned strings from the given source.
    /// </summary>
    /// <param name="input">Source string container.</param>
    public static List<string> ListStringInterned (IEnumerable<string> input)
    {
      if (input == null) return null;

      List<string> result = new List<string>();
      foreach (string s in input)
        result.Add(string.Intern(s));

      return result;
    }

    /// <summary>
    /// Merge cyclic-lists of two groups stored in common int[] array.
    /// </summary>
    /// <param name="arr">The common array of indices.</param>
    /// <param name="start1">Representant of set #1.</param>
    /// <param name="start2">Representant of set #2.</param>
    public static void MergeCyclicLists (int[] arr, int start1, int start2)
    {
      int origStart1 = arr[start1];
      arr[start1] = arr[start2];
      arr[start2] = origStart1;
    }

    /// <summary>
    /// Creates int[]-based set of single-member cyclic lists.
    /// </summary>
    /// <param name="n">Number of lists to create.</param>
    public static int[] CyclicArray (int n)
    {
      int[] gr = new int[n];
      for (int i = 0; i < n; i++)
        gr[i] = i;
      return gr;
    }

    public static int IntersectionSize<T> (ICollection<T> a, ICollection<T> b)
    {
      int iSize = 0;

      foreach (T aa in a)
        if (b.Contains(aa))
          iSize++;

      return iSize;
    }
  }

  /// <summary>
  /// Class for mutable floating-point numbers.
  /// </summary>
  public class MutableFloat
  {
    public float value;

    public MutableFloat (float val)
    {
      value = val;
    }

    public static implicit operator float (MutableFloat mf)
    {
      return mf.value;
    }
  }

  /// <summary>
  /// Class for mutable long integers.
  /// </summary>
  public class MutableLong
  {
    public long value;

    public MutableLong (long val = 0L)
    {
      value = val;
    }

    public static implicit operator long (MutableLong ml)
    {
      return ml.value;
    }

    public static MutableLong operator ++ (MutableLong ml)
    {
      ml.value++;
      return ml;
    }

    public long Inc (long val)
    {
      return value += val;
    }
  }

  /// <summary>
  /// Class for mutable integers.
  /// </summary>
  public class MutableInt
  {
    public int value;

    public MutableInt (int val = 0)
    {
      value = val;
    }

    public static implicit operator int (MutableInt mi)
    {
      return mi.value;
    }

    public static MutableInt operator ++ (MutableInt mi)
    {
      mi.value++;
      return mi;
    }

    public static MutableInt operator | (MutableInt mi, int m)
    {
      mi.value |= m;
      return mi;
    }

    public int Inc (int val)
    {
      return value += val;
    }

    public int Or (int mask)
    {
      return value |= mask;
    }
  }

  /// <summary>
  /// Class for computing ETF (Estimated Time of Finish).
  /// </summary>
  public class ETF
  {
    float lastTime;
    float lastFinished;
    float total;

    public ETF ()
    {
      lastTime = 0.0f;
      lastFinished = 0.0f;
      total = 0.0f;
    }

    /// <summary>
    /// 60% of current speed, 40% of global state
    /// </summary>
    /// <param name="time">Currently spent time.</param>
    /// <param name="finished">Currently finished part.</param>
    /// <param name="etf">Estimated time to finish.</param>
    /// <returns>Total estimate</returns>
    public float Estimate (float time, float finished, out float etf)
    {
      if (finished <= float.Epsilon)
        return etf = 0.0f;

      float total0 = time / finished;
      if (total == 0.0f)
        total = total0;
      float total1 = (finished == lastFinished) ? total : (time - lastTime) / (finished - lastFinished);
      total = 0.5f * (total + total1);
      lastTime = time;
      lastFinished = finished;
      float tot =  total0 * 0.4f + total * 0.6f ;
      etf = (1.0f - finished) * tot;
      return tot;
    }
  }

  public interface IAggregator<T> : IComparer<T>
  {
    T ReduceKey (T key);
  }

  /// <summary>
  /// Reverse variant of any IComparable<T>
  /// </summary>
  /// <typeparam name="T">Type to compare.</typeparam>
  public sealed class ReverseComparer<T> : IComparer<T> where T : IComparable<T>
  {
    public int Compare (T x, T y)
    {
      if (x == null) // handle nulls according to MSDN
        return (y == null) ? 0 : 1;

      if (y == null) // y null but x not, so x > y, but reversing to -1!
        return -1;

      // if neither arg is null, pass on to CompareTo in reverse order.
      return y.CompareTo(x);
    }
  }

  /// <summary>
  /// Aggregating comparer (first occurance of the given character cuts the rest of the key string).
  /// </summary>
  public sealed class StringAggregatorFirstChar : IAggregator<string>
  {
    private char separator = '(';

    public StringAggregatorFirstChar (char sep = '(')
    {
      separator = sep;
    }

    public int Compare (string x, string y)
    {
      x = ReduceKey(x);
      y = ReduceKey(y);

      return string.Compare(x, y);
    }

    public string ReduceKey (string key)
    {
      if (key == null)
        return null;

      int pos = key.IndexOf(separator);
      return (pos >= 0) ? key.Substring(0, pos) : key;
    }
  }

  /// <summary>
  /// Simple histogram. Each unique object has its own counter.
  /// </summary>
  /// <typeparam name="T">Any type usable as a key in a Dictionary.</typeparam>
  public class Histogram<T> : Dictionary<T, MutableLong>
  {
    public long Other { get; set; }

    public new void Clear ()
    {
      base.Clear();
      Other = 0L;
    }

    public long Inc (T key)
    {
      MutableLong count;
      if (!TryGetValue(key, out count))
        Add(key, count = new MutableLong());

      count++;
      return count.value;
    }

    public long Inc (T key, long val)
    {
      MutableLong count;
      if (!TryGetValue(key, out count))
        Add(key, count = new MutableLong());

      return count.Inc(val);
    }

    /// <summary>
    /// Adds the given histogram to this one.
    /// </summary>
    /// <param name="h">Histogram to add.</param>
    public void Add (Histogram<T> h)
    {
      if (h != null)
        foreach (var kvp in h)
          Inc(kvp.Key, kvp.Value);
    }

    public long Freq (T key)
    {
      MutableLong count = null;
      if (!TryGetValue(key, out count))
        return 0L;
      return count.value;
    }

    /// <summary>
    /// Delegate for histogram printing.
    /// </summary>
    public delegate string BinLabel (T key);

    public static string DefaultLabel (T key)
    {
      return key.ToString();
    }

    /// <summary>
    /// Optional description string appended at the end of the line.
    /// </summary>
    public delegate string Description (T key);

    /// <summary>
    /// Assembles set of keys for descending frequency order. Can handle the "other" bin.
    /// </summary>
    /// <param name="lowerLimit">Maximal bound for the "other" bin.</param>
    /// <returns></returns>
    public List<T> KeySet (double lowerLimit = 0.0)
    {
      long otherLimit = (lowerLimit <= 0.0) ? 0L : (long)(Total () * lowerLimit);

      SortedSet<long> freq = new SortedSet<long>(new ReverseComparer<long>());
      foreach (var val in Values)
        freq.Add(val.value);
      HashSet<T> done   = new HashSet<T>();
      List<T>    result = new List<T>();

      foreach (var val in freq)
        if (val >= otherLimit)
          foreach (var key in Keys)
            if (this[key] == val && !done.Contains(key))
            {
              result.Add(key);
              done.Add(key);
            }

      return result;
    }

    public bool Top (ref T maxKey)
    {
      if (Count == 0)
        return false;

      long max = 0L;
      foreach (var kvp in this)
        if (kvp.Value.value > max)
        {
          max = kvp.Value.value;
          maxKey = kvp.Key;
        }

      return true;
    }

    /// <summary>
    /// Aggregate the finished histogram using the given comparer.
    /// </summary>
    /// <param name="merge">Zero return values mean the two classes should merge.</param>
    public void Aggregate (IAggregator<T> merge)
    {
      HashSet<T> toRemove = new HashSet<T>();
      HashSet<T> done     = new HashSet<T>();
      foreach (var k1 in Keys)
        if (!toRemove.Contains(k1))
        {
          done.Add(k1);
          foreach (var k2 in Keys)
            if (!done.Contains(k2) &&
                !toRemove.Contains(k2) &&
                merge.Compare(k1, k2) == 0)
            {
              toRemove.Add(k2);
              this[k1].value += this[k2];
            }
        }

      foreach (var k in toRemove)
        Remove(k);

      foreach (var k in done)
      {
        T reduced = merge.ReduceKey(k);
        if (!reduced.Equals(k))
        {
          this[reduced] = this[k];
          Remove(k);
        }
      }
    }

    /// <summary>
    /// Discards all values less than the given limit.
    /// Returns summary of the discarded values.
    /// </summary>
    public long LimitValues (long limit)
    {
      HashSet<T> toRemove = new HashSet<T>();
      Other = 0L;
      foreach (var kvp in this)
        if (kvp.Value.value < limit)
        {
          toRemove.Add(kvp.Key);
          Other += kvp.Value.value;
        }

      foreach (var k in toRemove)
        Remove(k);

      return Other;
    }

    /// <summary>
    /// Keeps max number of items, the rest will be included in Other.
    /// Returns summary of the discarded values.
    /// </summary>
    public long LimitItems (int max)
    {
      if (max < 1 ||
          max >= Count)
        return 0L;

      List<long> sorted = new List<long>();
      foreach (var v in Values)
        sorted.Add(v);
      sorted.Sort(new ReverseComparer<long>());

      return LimitValues(sorted[max - 1]);
    }

    /// <summary>
    /// Substitutes old key into new one.
    /// </summary>
    /// <param name="from">The old key (need not be present).</param>
    /// <param name="to">The new key (can be already present).</param>
    public void SubstituteKey (T from, T to)
    {
      MutableLong fr;
      if (!TryGetValue(from, out fr))
        return;

      Remove(from);
      Inc(to, fr);
    }

    /// <summary>
    /// Print the histogram (sorted by frequency or key).
    /// </summary>
    /// <param name="sb">Output to write to.</param>
    /// <param name="limit">Lines limit (0 means "no limits")</param>
    /// <param name="percent">Include percent column?</param>
    /// <param name="perCum">Include cumulative-percent column?</param>
    /// <param name="comp">Comparison object or null for sorting by frequency.</param>
    /// <param name="label">Delegate for labeling histogram bins (can be null).</param>
    /// <param name="label">Delegate for histogram bins description (can be null).</param>
    public void Print (
      StringBuilder sb,
      int limit = 0,
      bool percent = false,
      bool perCum = false,
      IComparer<T> comp = null,
      BinLabel label = null,
      Description descr = null)
    {
      double multi = 100.0 / Math.Max(1L, Total());
      if (limit == 0)
        limit = Count;
      if (label == null)
        label = DefaultLabel;
      long soFar   = 0L; // accumulator
      long rest    = 0L;
      bool regular = true;

      int maxWidth = 19;
      foreach (var key in Keys)
      {
        int len = label(key).Length;
        if (len > maxWidth)
          maxWidth = len;
      }

      string fmt = "{0," + (maxWidth + 1) + "}: {1,10}";

      if (comp == null) // sorted by frequency
      {
        SortedSet<long> freq = new SortedSet<long>(new ReverseComparer<long>());
        foreach (var val in Values)
          freq.Add(val.value);
        HashSet<T> done = new HashSet<T>();
        foreach (var val in freq)
          foreach (var key in Keys)
            if (this[key] == val && !done.Contains(key))
            {
              soFar += val;
              if (regular)
              {
                sb.AppendFormat(fmt, label(key), val);
                if (percent)
                  sb.Append(string.Format(CultureInfo.InvariantCulture, "{0,8:f2}%", val * multi));
                if (perCum)
                  sb.Append(string.Format(CultureInfo.InvariantCulture, "{0,8:f2}%", soFar * multi));
                if (descr != null)
                  sb.Append("  ").Append(descr(key));
                sb.AppendLine();
                if (--limit <= 0)
                  regular = false;
              }
              else
                rest += val;

              done.Add(key);
            }
      }
      else // sorted by key
      {
        SortedDictionary<T, MutableLong> sorted = new SortedDictionary<T, MutableLong>(this, comp);
        foreach (var kvp in sorted)
        {
          soFar += kvp.Value.value;
          if (regular)
          {
            sb.AppendFormat(fmt, label(kvp.Key), kvp.Value.value);
            if (percent)
              sb.Append(string.Format(CultureInfo.InvariantCulture, "{0,8:f2}%", kvp.Value.value * multi));
            if (perCum)
              sb.Append(string.Format(CultureInfo.InvariantCulture, "{0,8:f2}%", soFar * multi));
            if (descr != null)
              sb.Append("  ").Append(descr(kvp.Key));
            sb.AppendLine();
            if (--limit <= 0)
              regular = false;
          }
          else
            rest += kvp.Value.value;
        }
      }

      if (rest > 0L)
      {
        sb.AppendFormat(fmt, "Rest", rest);
        if (percent)
          sb.Append(string.Format(CultureInfo.InvariantCulture, "{0,8:f2}%", rest * multi));
        sb.AppendLine();
      }
    }

    /// <summary>
    /// Print the histogram (sorted by frequency or key).
    /// </summary>
    /// <param name="o">Output to write to.</param>
    /// <param name="limit">Lines limit (0 means "no limits")</param>
    /// <param name="percent">Include percent column?</param>
    /// <param name="perCum">Include cumulative-percent column?</param>
    /// <param name="comp">Comparison object or null for sorting by frequency.</param>
    /// <param name="label">Delegate for labeling histogram bins (can be null).</param>
    /// <param name="label">Delegate for histogram bins description (can be null).</param>
    public void Print (
      TextWriter o,
      int limit = 0,
      bool percent = false,
      bool perCum = false,
      IComparer<T> comp = null,
      BinLabel label = null,
      Description descr = null)
    {
      StringBuilder sb = new StringBuilder();
      Print(sb, limit, percent, perCum, comp, label, descr);
      o.Write(sb.ToString());
    }

    /// <summary>
    /// Print the histogram in HTML format (sorted by frequency or key).
    /// </summary>
    /// <param name="sb">Output to write to.</param>
    /// <param name="limit">Lines limit (0 means "no limits")</param>
    /// <param name="percent">Include percent column?</param>
    /// <param name="perCum">Include cumulative-percent column?</param>
    /// <param name="comp">Comparison object or null for sorting by frequency.</param>
    /// <param name="label">Delegate for labeling histogram bins (can be null).</param>
    /// <param name="label">Delegate for histogram bins description (can be null).</param>
    public void PrintHtml (
      StringBuilder sb,
      int limit = 0,
      bool percent = false,
      bool perCum = false,
      IComparer<T> comp = null,
      BinLabel label = null,
      Description descr = null)
    {
      double multi = 100.0 / Math.Max(1L, Total());
      if (limit == 0)
        limit = Count;
      if (label == null)
        label = DefaultLabel;
      long soFar   = 0L; // accumulator
      long rest    = 0L;
      bool regular = true;

      if (comp == null) // sorted by frequency
      {
        SortedSet<long> freq = new SortedSet<long>(new ReverseComparer<long>());
        foreach (var val in Values)
          freq.Add(val.value);
        HashSet<T> done = new HashSet<T>();

        foreach (var val in freq)
          foreach (var key in Keys)
            if (this[key] == val && !done.Contains(key))
            {
              soFar += val;
              if (regular)
              {
                sb.AppendFormat(" <tr><td class=\"r\">{0}</td><td class=\"r\">{1}</td>",
                                label(key), val);
                if (percent)
                  sb.Append(string.Format(CultureInfo.InvariantCulture, "<td class=\"r\">{0:f2}%</td>", val * multi));
                if (perCum)
                  sb.Append(string.Format(CultureInfo.InvariantCulture, "<td class=\"r\">{0:f2}%</td>", soFar * multi));
                if (descr != null)
                  sb.AppendFormat("<td>{0}</td>", descr(key));
                sb.Append("</tr>\r\n");

                if (--limit <= 0)
                  regular = false;
              }
              else
                rest += val;

              done.Add(key);
            }
      }
      else // sorted by key
      {
        SortedDictionary<T, MutableLong> sorted = new SortedDictionary<T, MutableLong>(this, comp);
        foreach (var kvp in sorted)
        {
          soFar += kvp.Value.value;
          if (regular)
          {
            sb.AppendFormat(" <tr><td class=\"r\">{0}</td><td class=\"r\">{1}</td>",
                            label(kvp.Key), kvp.Value.value);
            if (percent)
              sb.Append(string.Format(CultureInfo.InvariantCulture, "<td class=\"r\">{0:f2}%</td>", kvp.Value.value * multi));
            if (perCum)
              sb.Append(string.Format(CultureInfo.InvariantCulture, "<td class=\"r\">{0:f2}%</td>", soFar * multi));
            if (descr != null)
              sb.AppendFormat("<td>{0}</td>", descr(kvp.Key));
            sb.Append("</tr>\r\n");

            if (--limit <= 0)
              regular = false;
          }
          else
            rest += kvp.Value.value;
        }
      }

      if (rest > 0L)
      {
        sb.AppendFormat(" <tr><td class=\"r\"><b>Rest</b></td><td class=\"r\">{0}</td>", rest);
        if (percent)
          sb.Append(string.Format(CultureInfo.InvariantCulture, "<td class=\"r\">{0:f2}%</td>", rest * multi));
        if (perCum)
          sb.Append("<td>&nbsp;</td>");
        if (descr != null)
          sb.Append("<td>&nbsp;</td>");
        sb.Append("</tr>\r\n");
      }
    }

    /// <summary>
    /// Sum of all bins.
    /// </summary>
    public long Total ()
    {
      long sum = Other;
      foreach (var val in Values)
        sum += val;
      return sum;
    }
  }

  /// <summary>
  /// Int-keyed histogram with arbitrary bin size.
  /// </summary>
  public class HistogramInt : Histogram<int>
  {
    public int BinSize { get; set; }

    public HistogramInt (int binSize = 1)
    {
      BinSize = binSize;
    }

    public new long Inc (int key)
    {
      return base.Inc(key / BinSize);
    }

    public new string DefaultLabel (int key)
    {
      return (key * BinSize).ToString();
    }

    public new void Print (
      TextWriter o,
      int limit = 0,
      bool percent = false,
      bool perCum = false,
      IComparer<int> comp = null,
      BinLabel label = null,
      Description descr = null)
    {
      base.Print(o, limit, percent, perCum, comp, label ?? DefaultLabel, descr);
    }

    public new void PrintHtml (
      StringBuilder sb,
      int limit = 0,
      bool percent = false,
      bool perCum = false,
      IComparer<int> comp = null,
      BinLabel label = null,
      Description descr = null)
    {
      base.PrintHtml(sb, limit, percent, perCum, comp, label ?? DefaultLabel, descr);
    }
  }

  /// <summary>
  /// Long-keyed histogram with arbitrary bin size.
  /// </summary>
  public class HistogramLong : Histogram<long>
  {
    public long BinSize { get; set; }

    public HistogramLong (long binSize = 1)
    {
      BinSize = binSize;
    }

    public new long Inc (long key)
    {
      return base.Inc(key / BinSize);
    }

    public new string DefaultLabel (long key)
    {
      return (key * BinSize).ToString();
    }

    public new void Print (
      TextWriter o,
      int limit = 0,
      bool percent = false,
      bool perCum = false,
      IComparer<long> comp = null,
      BinLabel label = null,
      Description descr = null)
    {
      base.Print(o, limit, percent, perCum, comp, label ?? DefaultLabel, descr);
    }

    public new void PrintHtml (
      StringBuilder sb,
      int limit = 0,
      bool percent = false,
      bool perCum = false,
      IComparer<long> comp = null,
      BinLabel label = null,
      Description descr = null)
    {
      base.PrintHtml(sb, limit, percent, perCum, comp, label ?? DefaultLabel, descr);
    }
  }


  /// <summary>
  /// 2D histogram based on Histogram&lt;T&gt;
  /// </summary>
  /// <typeparam name="Tpri">Primary key type (inner table).</typeparam>
  /// <typeparam name="Tsec">Secondary key type (outer table).</typeparam>
  public class Histogram2D<Tpri, Tsec> : Dictionary<Tsec, Histogram<Tpri>>
  {
    /// <summary>
    /// Sparse 2D frequency table.
    /// </summary>
    //public Dictionary<Tsec, Histogram<Tpri>> table = new Dictionary<Tsec, Histogram<Tpri>>();

    public long Inc (Tpri key1, Tsec key2)
    {
      if (key1 == null ||
          key2 == null)
        return 0L;

      Histogram<Tpri> inner;
      if (!TryGetValue(key2, out inner))
        Add(key2, inner = new Histogram<Tpri>());

      return inner.Inc(key1);
    }

    public long Inc (Tpri key1, Tsec key2, long count)
    {
      if (key1 == null ||
          key2 == null)
        return 0L;

      Histogram<Tpri> inner;
      if (!TryGetValue(key2, out inner))
        Add(key2, inner = new Histogram<Tpri>());

      return inner.Inc(key1, count);
    }

    public void Add (Histogram<Tpri> h, Tsec key2)
    {
      if (key2 == null)
        return;

      Histogram<Tpri> inner;
      if (!TryGetValue(key2, out inner))
        Add(key2, inner = new Histogram<Tpri>());

      inner.Add(h);
    }

    public long Freq (Tpri key1, Tsec key2)
    {
      Histogram<Tpri> inner;
      if (key2 == null ||
          !TryGetValue(key2, out inner))
        return 0L;

      return inner.Freq(key1);
    }

    /// <summary>
    /// Sum of all bins.
    /// </summary>
    public long Total ()
    {
      long sum = 0L;
      foreach (var h in Values)
        sum += h.Total();
      return sum;
    }
  }

  /// <summary>
  /// Set of Id-s allowing to effectively store id-intervals.
  /// </summary>
  public class IdSet : HashSet<string>
  {
    class Interval : ICloneable
    {
      /// <summary>
      /// Optional id-prefix (can be null).
      /// </summary>
      string prefix;

      /// <summary>
      /// Minimum value or long.MinValue.
      /// </summary>
      long min;

      /// <summary>
      /// Maximum value or long.MaxValue.
      /// </summary>
      long max;

      public object Clone ()
      {
        return new Interval(prefix, min, max);
      }

      public static bool IsInterval (string str)
      {
        if (str == null ||
             str.Length < 2)
          return false;

        int pos = str.IndexOf('-');
        return pos >= 0;
      }

      public Interval (string str)
      {
        prefix = null;
        min = long.MinValue;
        max = long.MaxValue;

        string[] tokens = str.Split('-');
        if (tokens.Length != 2 ||
             (string.IsNullOrEmpty(tokens[0]) &&
              string.IsNullOrEmpty(tokens[1])))
          return;

        int i0 = tokens [ 0 ].Length;
        while (i0 > 0 && char.IsDigit(tokens[0][i0 - 1]))
          i0--;

        int i1 = tokens [ 1 ].Length;
        while (i1 > 0 && char.IsDigit(tokens[1][i1 - 1]))
          i1--;

        if (i0 == i1 &&
            i0 > 0 &&
            tokens[0].Substring(0, i0) == tokens[1].Substring(0, i0))
          prefix = tokens[0].Substring(0, i0);

        long mi, ma;
        if (long.TryParse(tokens[0].Substring(i0), out mi))
          min = mi;
        if (long.TryParse(tokens[1].Substring(i1), out ma))
          max = ma;
        if (max < min)
          max = min;
      }

      public Interval (string pref, long mi, long ma = long.MaxValue)
      {
        prefix = pref;
        min = mi;
        max = Math.Max(min, ma);
      }

      public bool Contains (string str)
      {
        if (str == null ||
             (prefix != null &&
              !str.StartsWith(prefix)))
          return false;

        if (prefix != null)
          str = str.Substring(prefix.Length);

        long val;
        return (long.TryParse(str, out val) &&
                val >= min &&
                val <= max);
      }

      public bool IsTrivial ()
      {
        return prefix == null && min == long.MinValue && max == long.MaxValue;
      }
    }

    /// <summary>
    /// Current intervals.
    /// </summary>
    List<Interval> intervals;

    public IdSet ()
    {
      intervals = new List<Interval>();
    }

    public IdSet (IEnumerable<string> collection)
      : base(collection)
    {
      intervals = new List<Interval>();
    }

    public IdSet (IdSet idset)
      : this()
    {
      if (idset != null)
      {
        UnionWith(idset);
        foreach (var i in idset.intervals)
          intervals.Add(i.Clone() as Interval);
      }
    }

    new public bool Add (string str)
    {
      if (string.IsNullOrEmpty(str))
        return false;

      if (Interval.IsInterval(str))
      {
        Interval i = new Interval(str);
        if (i.IsTrivial())
          return true;

        intervals.Add(i);
        return false;
      }

      return base.Add(str);
    }

    new public int Count => base.Count + intervals.Count;

    new public void Clear ()
    {
      base.Clear();
      intervals.Clear();
    }

    new public bool Contains (string str)
    {
      if (base.Contains(str))
        return true;

      foreach (var i in intervals)
        if (i.Contains(str))
          return true;

      return false;
    }
  }

  /// <summary>
  /// ObjectId to real object mapping delegate.
  /// </summary>
  public delegate Type GetCheckObjectDelegate<Type> (string id);

  /// <summary>
  /// Dictionary of string-sets. Key -> ( Object )*
  /// Both key and object have string Ids (controlling the maps) and actual values (potantially different classes).
  /// </summary>
  /// <typeparam name="Key">Actual key type (can be general = Object).</typeparam>
  /// <typeparam name="Type">Actual object type (can be general = Object).</typeparam>
  public class KeyObjectMaps<Key, Type>
  {
    public GetCheckObjectDelegate<Key> GetKey { protected get; set; }

    public GetCheckObjectDelegate<Key> CheckKey { protected get; set; }

    public GetCheckObjectDelegate<Type> GetObject { protected get; set; }

    public GetCheckObjectDelegate<Type> CheckObject { protected get; set; }

    public List<string> AllKeyIds => new List<string>(sets.Keys);

    public List<string> AllObjectIds
    {
      get
      {
        HashSet<string> allObjects = new HashSet<string>();
        foreach (var set in sets.Values)
          allObjects.UnionWith(set);

        return new List<string>(allObjects);
      }
    }

    public List<string> ObjectIds (string setId)
    {
      HashSet<string> set = null;
      if (setId != null)
        sets.TryGetValue(setId, out set);
      return new List<string>(set ?? new HashSet<string>());
    }

    public int Count => sets.Count;

    public int Card (string setId)
    {
      HashSet<string> set = null;
      if (setId != null)
        sets.TryGetValue(setId, out set);
      return (set == null) ? 0 : set.Count;
    }

    public KeyObjectMaps (GetCheckObjectDelegate<Key> getKey = null, GetCheckObjectDelegate<Key> checkKey = null,
                          GetCheckObjectDelegate<Type> getObject = null,
                          GetCheckObjectDelegate<Type> checkObject = null)
    {
      sets = new Dictionary<string, HashSet<string>>();

      if (getKey != null)
        GetKey = getKey;
      else
        GetKey = (id) => Util.DefaultValue<Key>();

      if (checkKey != null)
        CheckKey = checkKey;
      else
        CheckKey = (id) => Util.DefaultValue<Key>();

      if (getObject != null)
        GetObject = getObject;
      else
        GetObject = (id) => Util.DefaultValue<Type>();

      if (checkObject != null)
        CheckObject = checkObject;
      else
        CheckObject = (id) => Util.DefaultValue<Type>();
    }

    public bool AddObject (string setId, string objectId)
    {
      if (setId == null ||
          objectId == null)
        return false;

      HashSet<string> set;
      if (!sets.TryGetValue(setId, out set))
        sets[setId] = set = new HashSet<string>();

      return set.Add(objectId);
    }

    public void AddObjects (string setId, IEnumerable<string> objectIds)
    {
      if (setId == null ||
          objectIds == null ||
          !objectIds.GetEnumerator().MoveNext())
        return;

      HashSet<string> set;
      if (sets.TryGetValue(setId, out set))
        set.UnionWith(objectIds);
      else
        sets[setId] = new HashSet<string>(objectIds);
    }

    /// <summary>
    /// Construction if inverse mapping.
    /// </summary>
    public KeyObjectMaps<Type, Key> Inverse ()
    {
      KeyObjectMaps<Type, Key> inv = new KeyObjectMaps<Type, Key>
      {
        GetKey      = GetObject,
        CheckKey    = CheckObject,
        GetObject   = GetKey,
        CheckObject = CheckKey
      };

      foreach (var kvp in sets)
        foreach (string objId in kvp.Value)
          inv.AddObject(objId, kvp.Key);

      return inv;
    }

    public bool Contains (string setId, string objectId)
    {
      HashSet<string> set;
      return setId != null &&
             sets.TryGetValue(setId, out set) &&
             set.Contains(objectId);
    }

    //--- implementation ---

    protected Dictionary<string, HashSet<string>> sets;
  }

  /// <summary>
  /// Class for simple statistical analysis of numeric scalar quantity.
  /// Mean value and variance is implemented, optional Quantiles.
  /// </summary>
  /// <typeparam name="T">Numeric base type.</typeparam>
  public class Quantiles<T> : List<T>
  {
    /// <summary>
    /// If true, individual values are stored and Quantiles will be able to compute.
    /// </summary>
    protected bool storeValues;

    /// <summary>
    /// Number of inserted values.
    /// If 'storeValues', it has to be equal to base.Count.
    /// </summary>
    protected int N;

    /// <summary>
    /// Sum of inserted values.
    /// </summary>
    protected double Sx;

    /// <summary>
    /// Sum of squares of inserted values.
    /// </summary>
    protected double Sxx;

    /// <summary>
    /// True if the array needs sorting.
    /// </summary>
    protected bool dirty;

    /// <summary>
    /// Sole constructor.
    /// </summary>
    /// <param name="quantiles">If positive, quantiles can be computed.
    /// If negative, only mean value and variance will be provided.</param>
    /// <param name="capacity">Initial array capacity.</param>
    public Quantiles (bool quantiles = true, int capacity = 0)
      : base(quantiles ? Math.Max(capacity, 0) : 0)
    {
      storeValues = quantiles;
      N = 0;
      Sx = Sxx = 0.0;
    }

    public new void Clear ()
    {
      base.Clear();
      N = 0;
      Sx = Sxx = 0.0;
    }

    protected void AddStat (T x)
    {
      double val = Convert.ToDouble(x);
      N++;
      Sx += val;
      Sxx += val * val;
      dirty = true;
    }

    protected void RemoveStat (T x)
    {
      double val = Convert.ToDouble(x);
      N--;
      Sx -= val;
      Sxx -= val * val;
      dirty = true;
    }

    /// <summary>
    /// Mean (average) value of inserted values.
    /// </summary>
    public double Mean => (N > 0) ? Sx / N : double.NaN;

    /// <summary>
    /// Variance of the set of inserted values (for sample from a larger population use <see cref="VarianceBessel"/>).
    /// </summary>
    public double Variance => (N > 0) ? Math.Sqrt((Sxx - Sx * Sx / N) / N) : double.NaN;

    /// <summary>
    /// Bessel's correction: if our set of values is actually a sample from larger population, we should use this formula.
    /// </summary>
    public double VarianceBessel => (N > 1) ? Math.Sqrt((Sxx - Sx * Sx / N) / (N - 1.0)) : double.NaN;

    /// <summary>
    /// <![CDATA[Computes the i-th q-quantile (0 <= i <= q).]]>
    /// </summary>
    /// <param name="q">Number of quantiles (1 for min/max, 2 for median, 4 for quartiles, 10 for deciles..).</param>
    /// <param name="i">Index of the quantile (0 .. min, q .. max).</param>
    /// <returns>Quantile value.</returns>
    public double Quantile (int q, int i)
    {
      if (!storeValues)
        return double.NaN;

      if (dirty)
      {
        Debug.Assert(N == Count);
        if (N > 0)
          Sort();
        dirty = false;
      }

      if (N <= 0)
        return double.NaN;

      if (N == 1 ||
          i <= 0)
        return Convert.ToDouble(base[0]);

      if (i >= q)
        return Convert.ToDouble(base[N - 1]);

      double di = (N - 1.0) * i / q;
      int    i0 = (int)Math.Floor(di);
      di -= i0;

      return (1.0 - di) * Convert.ToDouble(base[i0]) + di * Convert.ToDouble(base[i0 + 1]);
    }

    public double Min => Quantile(1, 0);

    public double Max => Quantile(1, 1);

    public double Median => Quantile(2, 1);

    public double Quartile (int i)
    {
      return Quantile(4, i);
    }

    //--- all the boring stuff is below this line: we have to implement (override) all meaningful methods of List<T> ---

    public new void Add (T x)
    {
      if (storeValues)
        base.Add(x);

      AddStat(x);
    }

    public new void AddRange (IEnumerable<T> collection)
    {
      foreach (T val in collection)
        Add(val);
    }

    public new void Insert (int index, T x)
    {
      if (storeValues)
        base.Insert(index, x);

      AddStat(x);
    }

    public new void InsertRange (int index, IEnumerable<T> collection)
    {
      AddRange(collection);
    }

    public new void Remove (T x)
    {
      if (storeValues)
        base.Remove(x);

      RemoveStat(x);
    }

    public new void RemoveAt (int index)
    {
      if (!storeValues ||
          index < 0 ||
          index >= Count)
        return;

      RemoveStat(base[index]);
      base.RemoveAt(index);
    }

    public new void RemoveRange (int start, int count)
    {
      while (count-- > 0)
        RemoveAt(start);
    }

    public new void RemoveAll (Predicate<T> match)
    {
      if (!storeValues)
        return;

      foreach (T val in this)
        if (match(val))
          RemoveStat(val);

      base.RemoveAll(match);
    }
  }

  /// <summary>
  /// Simple key[(value[,..])][;key[(value..)]] representation.
  /// Used for data normalization (id-based sorting,..).
  /// </summary>
  class OpenRecord : IEquatable<OpenRecord>
  {
    public const char ITEM_SEPARATOR = ';';

    public const char VALUE_SEPARATOR = ',';

    public const char VALUE_PREFIX = '(';

    public const char VALUE_SUFFIX = ')';

    protected int hash;

    public Dictionary<string, List<string>> data;

    /// <summary>
    /// Add/replace the given item.
    /// </summary>
    /// <returns>True if added, false if replaced.</returns>
    public bool AddItem (string key, string val = null)
    {
      if (key == null)
        return true;

      List<string> valArray;

      if (data.ContainsKey(key))
      {
        valArray = data[key];
        if (val == null)
          data[key] = null;
        else
        {
          if (valArray == null)
            data[key] = valArray = new List<string>();
          else
            valArray.Clear();
          valArray.Add(val);
        }

        hash = int.MinValue;
        return false;
      }

      if (val != null)
      {
        valArray = new List<string>();
        valArray.Add(val);
      }
      else
        valArray = null;

      data.Add(key, valArray);

      hash = int.MinValue;
      return true;
    }

    /// <summary>
    /// Remove item with the given key.
    /// </summary>
    /// <returns>True if an item was actually removed.</returns>
    public bool RemoveItem (string key)
    {
      if (key == null)
        return false;

      hash = int.MinValue;
      return data.Remove(key);
    }

    /// <summary>
    /// Adds another value to the given key.
    /// Values are not sorted, multiple equal values can be used.
    /// </summary>
    public void AddValue (string key, string val)
    {
      if (key == null ||
          val == null)
        return;

      List<string> valArray;

      if (!data.ContainsKey(key) ||
          (valArray = data[key]) == null)
        data[key] = valArray = new List<string>();

      valArray.Add(val);
      hash = int.MinValue;
    }

    /// <summary>
    /// Normalize the valule arrays using provided comparison function.
    /// </summary>
    /// <param name="comparison">If null, default string comparison will be used.</param>
    public void NormalizeValues (Comparison<string> comparison = null)
    {
      if (comparison == null)
        comparison = string.Compare;

      foreach (var kvp in data)
        if (kvp.Value != null)
          kvp.Value.Sort(comparison);

      hash = int.MinValue;
    }

    public string ToString (Comparison<string> keyComparison, Comparison<string> valueComparison = null)
    {
      if (data.Count == 0)
        return "_";

      if (valueComparison != null)
        NormalizeValues(valueComparison);

      List<string> keys = new List<string>(data.Keys);
      if (keyComparison != null)
        keys.Sort(keyComparison);

      StringBuilder sb = new StringBuilder();
      List<string>  valArray;

      bool cont = false;
      foreach (var key in keys)
      {
        if (cont)
          sb.Append(ITEM_SEPARATOR);
        sb.Append(key);

        if ((valArray = data[key]) != null &&
            valArray.Count > 0)
        {
          sb.Append(VALUE_PREFIX).Append(valArray[0]);
          for (int i = 1; i < valArray.Count; i++)
            sb.Append(VALUE_SEPARATOR).Append(valArray[i]);
          sb.Append(VALUE_SUFFIX);
        }

        cont = true;
      }

      return sb.ToString();
    }

    public override string ToString ()
    {
      return ToString(string.Compare);
    }

    public OpenRecord ()
    {
      data = new Dictionary<string, List<string>>();
      hash = int.MinValue;
    }

    public override int GetHashCode ()
    {
      if (data.Count == 0) return 11;

      if (hash == int.MinValue)
      {
        hash = 1;
        List<string> keys = new List<string>(data.Keys);
        keys.Sort();
        foreach (var key in keys)
        {
          hash = 7 * hash + key.GetHashCode();
          if (data[key] != null)
            foreach (var s in data[key])
              hash = 13 * hash + s.GetHashCode();
        }
      }

      return hash;
    }

    public override bool Equals (object o)
    {
      if (o == null)
        return false;

      return Equals(o as OpenRecord);
    }

    public bool Equals (OpenRecord or)
    {
      if (or == null ||
          data.Count != or.data.Count)
        return false;

      foreach (var kvp in data)
      {
        if (!or.data.ContainsKey(kvp.Key))
          return false;

        List<string> orValues = or.data[kvp.Key];
        if (kvp.Value == null != (orValues == null))
          return false;

        if (orValues != null)
        {
          if (kvp.Value.Count != orValues.Count)
            return false;
          for (int i = 0; i < orValues.Count; i++)
            if (kvp.Value[i] != orValues[i])
              return false;
        }
      }

      return true;
    }
  }

  public static class TypeLoader
  {
    public static IEnumerable<Type> GetLoadableTypes (this Assembly assembly)
    {
      if (assembly == null)
        throw new ArgumentNullException("assembly");

      try
      {
        return assembly.GetTypes();
      }
      catch (ReflectionTypeLoadException e)
      {
        return e.Types.Where(t => t != null);
      }
    }

    public static IEnumerable<Type> GetTypesWithInterface<Interface> ()
    {
      var it = typeof(Interface);
      List<Type> result = new List<Type>();

      foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        result.AddRange(GetLoadableTypes(asm)
          .Where(it.IsAssignableFrom)
          .Where(t => !t.Equals(it))
            .ToList());

      return result;
    }
  }
}
