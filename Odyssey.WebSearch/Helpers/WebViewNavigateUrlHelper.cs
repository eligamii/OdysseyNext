using Odyssey.Data.Settings;
using System.Net;
using System.Threading.Tasks;
using static Odyssey.WebSearch.Helpers.WebSearchStringKindHelpers;

namespace Odyssey.WebSearch.Helpers
{
    public class WebViewNavigateUrlHelper
    {
        /// <summary>
        /// Return an url usable for WebView2.CoreWebView2.Navigate() and WebView2.Source (ex: "whats 1+1?" can return "https://www.google.com/search?q=whats+1%2B1%3F")
        /// </summary>
        /// <param name="query">The string to convert into a WebView2 url</param>
        /// <returns></returns>
        public static async Task<string> ToWebView2Url(string query)
        {
            var kind = await GetStringKindAsync(query);

            switch (kind)
            {
                case StringKind.Url or StringKind.InternalUrl:
                    if (!query.Contains("://")) query = "https://" + query;
                    return query;

                case StringKind.SearchKeywords or StringKind.MathematicalExpression: return SearchEngine.ToSearchEngineObject((SearchEngines)Settings.SelectedSearchEngine).SearchUrl + WebUtility.UrlEncode(query); // url encode as "1+1" will be treated as "1 1"

                default: return string.Empty;
            }
        }
    }
}
