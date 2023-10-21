using Odyssey.Data.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Odyssey.WebSearch.Helpers.WebUrlHelpers;

namespace Odyssey.Helpers
{
    public class SearchUrlHelper
    {
        // Get the corresponding url from a query (ex: "whats 1+1?" can return "https://www.google.com/search?q=whats+1%2B1%3F")
        public static string ToUrl(string query)
        {
            var kind = GetStringKind(query);

            switch(kind)
            {
                case StringKind.Url or StringKind.OdysseyUrl or StringKind.MathematicalExpression: return query; // do nothing
                case StringKind.SearchKeywords: return SearchEngine.ToSearchEngineObject((SearchEngines)Settings.SelectedSearchEngine).SearchUrl + WebUtility.UrlEncode(query); // url encode as "1+1" will be treated as "1 1"
                default: return string.Empty;
            }
        }
    }
}
