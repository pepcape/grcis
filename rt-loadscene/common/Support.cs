using System.Globalization;
using System.Text;

// Support code.
namespace Support
{
  /// <summary>
  /// Text utilities.
  /// </summary>
  public class TextUtils
  {
    /// <summary>
    /// Removes diacritics from a string.
    /// </summary>
    public static string RemoveDiacritics ( string str )
    {
      string formD = str.Normalize( NormalizationForm.FormD );
      StringBuilder sb = new StringBuilder();

      foreach ( char ch in formD )
        if ( CharUnicodeInfo.GetUnicodeCategory( ch ) != UnicodeCategory.NonSpacingMark )
          sb.Append( ch );

      return( sb.ToString().Normalize( NormalizationForm.FormC ) );
    }
  }
}
