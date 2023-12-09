using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.System;

namespace Odyssey.WebSearch.Helpers
{
    public class WebSearchStringKindHelpers
    {
        private readonly static Regex urlRegex = new( @"^(https?://)?[a-zA-Z0-9]{0,63}(?<!/)\.{0,1}([a-zA-Z0-9]|(?<!\.)-){1,63}\.[a-zA-Z]{1,63}(/.*)*$", RegexOptions.Compiled);
        private readonly static Regex odysseyUrlRegex = new(@"^(edge|chrome|odyssey)://[-a-zA-Z0-9@:%._\+~#=]{1,256}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)", RegexOptions.Compiled);
        private readonly static Regex externalAppUriRegex = new(@"^[-a-zA-Z0-9]{2,20}:;*", RegexOptions.Compiled);
        private readonly static Regex mathematicalExpressionRegex = new(@"^[a-zA-Z0-9\(\)]+([-+/*^\(\),=][a-zA-Z0-9\(\)\=]+(\.[a-zA-Z0-9\(\)]+)?){1,}$", RegexOptions.Compiled); // match also with functions (Pow(),...) but has false positive (ex: pow1,1)
        private readonly static Regex quickActionCommandsRegex = new(@"\$([a-z]{2,}|<[a-z]{2,}>)( [a-z]*(:((""[-a-zA-Z0-9()@:%_\+.~#?&/\\=<>; ]{1,}"")|([a-z;]{1,}|<[a-z]{3,}>))){0,1})*", RegexOptions.Compiled); // ex: $flyout pos:default content:<linkuri>
        public enum StringKind
        {
            Url,                        // https://
            OdysseyUrl,                 // odyssey://
            ExternalAppUri,             // sl:
            SearchKeywords,             // "hello world"
            QuickActionCommand,         // $flyout content:...
            MathematicalExpression      // 1+2*3/(3+4)

        }

        public static async Task<StringKind> GetStringKindAsync(string str)
        {
            if (quickActionCommandsRegex.IsMatch(str)) return StringKind.QuickActionCommand;
            else if (urlRegex.IsMatch(str)) return StringKind.Url;
            else if (odysseyUrlRegex.IsMatch(str)) return StringKind.OdysseyUrl;
            else if (mathematicalExpressionRegex.IsMatch(str)) return StringKind.MathematicalExpression;
            else if (externalAppUriRegex.IsMatch(str))
            {
                try
                {
                    if(Uri.IsWellFormedUriString(str, UriKind.Absolute))
                    {
                        LaunchQuerySupportStatus res = await Launcher.QueryUriSupportAsync(new System.Uri(str), LaunchQuerySupportType.Uri);
                        if (res == LaunchQuerySupportStatus.Available && !str.StartsWith("http")) // prevent some webpages from opening in both Odyssey and the default browser
                        {
                            return StringKind.ExternalAppUri;
                        }
                        else return StringKind.SearchKeywords;
                    }
                    else
                    {
                        return StringKind.SearchKeywords;
                    }
                }
                catch (UriFormatException)
                {
                    return StringKind.SearchKeywords;
                }
            }
            else return StringKind.SearchKeywords;
        }


        public static StringKind GetStringKind(string str)
        {
            if (quickActionCommandsRegex.IsMatch(str)) return StringKind.QuickActionCommand;
            else if (urlRegex.IsMatch(str)) return StringKind.Url;
            else if (odysseyUrlRegex.IsMatch(str)) return StringKind.OdysseyUrl;
            else if (mathematicalExpressionRegex.IsMatch(str)) return StringKind.MathematicalExpression;
            else return StringKind.SearchKeywords;
        }
    }
}
