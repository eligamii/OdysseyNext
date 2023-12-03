using System.Text.RegularExpressions;

namespace Monaco.Helpers
{
    public static class JavascriptHelpers
    {
        public static string ToCSharpString(this string str)
        {
            if(str != "null")
            {
                // convert \x things
                str = str.Replace(@"\\", "\\")
                         .Replace(@"\n", "\n")
                         .Replace(@"\r", "\r")
                         .Replace(@"\t", "\t")
                         .Replace(@"\b", "\b")
                         .Replace(@"\f", "\f")
                         .Replace("\\\"", "\"");

                // convert unicode
                var matches = Regex.Matches(str, @"\\u[A-Z0-9]{4}");
                if (matches.Count >= 0)
                {
                    foreach (Match match in matches)
                    {
                        int unicode = int.Parse(match.Value.Replace("\\u", ""), System.Globalization.NumberStyles.HexNumber);
                        char c = (char)unicode;
                        str = str.Replace(match.Value, c.ToString());
                    }
                }

                if (str.StartsWith('"') && str.EndsWith('"'))
                {
                    str = str.Substring(1);
                    str = str.Remove(str.Length - 1);
                }

                return str;
            }

            return string.Empty;
        }
    }
}
