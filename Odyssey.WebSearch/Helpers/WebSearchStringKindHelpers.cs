﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.System;

namespace Odyssey.WebSearch.Helpers
{
    public class WebSearchStringKindHelpers
    {
        private readonly static string urlRegex = @"^(https?:\/\/){0,1}(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,12}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";
        private readonly static string odysseyUrlRegex = @"^(edge|chrome|odyssey)://[-a-zA-Z0-9@:%._\+~#=]{1,256}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";
        private readonly static string externalAppUriRegex = @"^[-a-zA-Z0-9]{2,20}:;*";
        private readonly static string mathematicalExpressionRegex = @"^[a-zA-Z0-9\(\)]+([-+/*^\(\),=][a-zA-Z0-9\(\)\=]+(\.[a-zA-Z0-9\(\)]+)?){1,}$"; // match also with functions (Pow(),...) but has false positive (ex: pow1,1)
        private readonly static string quickActionCommandsRegex = @"\$([a-z]{2,}|<[a-z]{2,}>)( [a-z]*(:((""[-a-zA-Z0-9()@:%_\+.~#?&/\\=<>; ]{1,}"")|([a-z;]{1,}|<[a-z]{3,}>))){0,1})*"; // ex: $flyout pos:default content:<linkuri>
        public enum StringKind
        {
            Url,                        // https://
            OdysseyUrl,                 // odyssey://
            ExternalAppUri,             // sl:
            SearchKeywords,             // "hello world"
            QuickActionCommand,         // $flyout content:...
            MathematicalExpression      // 1+2*3/(3+4)

        }

        public static async Task<StringKind> GetStringKind(string str)
        {
            if (Regex.IsMatch(str, quickActionCommandsRegex)) return StringKind.QuickActionCommand;
            else if (Regex.IsMatch(str, urlRegex)) return StringKind.Url;
            else if (Regex.IsMatch(str, odysseyUrlRegex)) return StringKind.OdysseyUrl;
            else if (Regex.IsMatch(str, mathematicalExpressionRegex)) return StringKind.MathematicalExpression;
            else if (Regex.IsMatch(str, externalAppUriRegex))
            {
                try
                {
                    LaunchQuerySupportStatus res = await Launcher.QueryUriSupportAsync(new System.Uri(str), LaunchQuerySupportType.Uri);
                    if (res == LaunchQuerySupportStatus.Available && !str.StartsWith("http")) // prevent some webpages from opening in both Odyssey and the default browser
                    {
                        return StringKind.ExternalAppUri;
                    }
                    else return StringKind.SearchKeywords;
                }
                catch (UriFormatException)
                {
                    return StringKind.SearchKeywords;
                }
            }
            else return StringKind.SearchKeywords;
        }
    }
}
