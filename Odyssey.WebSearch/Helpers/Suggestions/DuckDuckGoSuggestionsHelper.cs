using Odyssey.Shared.ViewModels.WebSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Odyssey.WebSearch.Helpers.Suggestions
{
    internal static class DuckDuckGoSuggestionsHelper
    {
        public class DDGSuggestion //DuckDuckGo JSON suggestions
        {
            public string phrase { get; set; }
        }

        public async static Task<List<Suggestion>> GetFromDuckDuckGoSuggestions(string query)
        {
            List<Suggestion> suggestions = new();
            string url = $"https://ac.duckduckgo.com/ac/?q={WebUtility.UrlEncode(query)}";

            using (HttpClient client = new HttpClient())
            {
                string str = await client.GetStringAsync(url);
                var ddgsuggestions = JsonSerializer.Deserialize<List<DDGSuggestion>>(str);

                foreach (var ddgsuggestion in ddgsuggestions)
                {
                    Suggestion suggestion = new();
                    suggestion.Title = ddgsuggestion.phrase;
                    suggestion.Url = await WebViewNavigateUrlHelper.ToUrl(ddgsuggestion.phrase);
                    suggestion.Kind = SuggestionKind.Search;

                    suggestions.Add(suggestion);
                }
            }

            return null; //[TOREMOVE]
        }
    }
}
