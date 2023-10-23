﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Odyssey.WebSearch.Helpers
{
    public class WebUrlHelpers
    {
        private readonly static string urlRegex = @"^(https?:\/\/){0,1}(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";
        private readonly static string odysseyUrlRegex = @"^(edge|chrome|odyssey)://[-a-zA-Z0-9@:%._\+~#=]{1,256}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";
        private readonly static string externalAppUriRegex = @"^[-a-zA-Z0-9]{2,20}:;*";
        private readonly static string mathematicalExpressionRegex = @"^\d+([-+/*]\d+(\.\d+)?){1,}$";

        public enum StringKind
        {
            Url,                        // https://
            OdysseyUrl,                 // odyssey://
            ExternalAppUri,             // sl:
            SearchKeywords,             // "hello world"
            QuickActionCommand,         // flyout content:...
            MathematicalExpression      // 1+2*3/(3+4)

        }

        public static StringKind GetStringKind(string str)
        {
            if (Regex.IsMatch(str, urlRegex)) return StringKind.Url;
            else if (Regex.IsMatch(str, odysseyUrlRegex)) return StringKind.OdysseyUrl;
            else if (Regex.IsMatch(str, externalAppUriRegex)) return StringKind.ExternalAppUri;
            else if (Regex.IsMatch(str, mathematicalExpressionRegex)) return StringKind.MathematicalExpression;
            else return StringKind.SearchKeywords;
        }
    }
}
