using Odyssey.Shared.ViewModels.WebSearch;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Odyssey.WebSearch.Helpers.Suggestions
{
    internal static class DuckDuckGoSuggestionsHelper
    {
        private static List<Suggestion> suggestions = new(); // Save suggestions for faster suggestions
        public class DDGSuggestion //DuckDuckGo JSON suggestions
        {
            public string phrase { get; set; }
        }


        private static HttpClient client = new HttpClient();
        public async static Task<List<Suggestion>> GetFromDuckDuckGoSuggestions(string query)
        {
            if (!suggestions.Any(p => p.Query == query))
            {
                string url = $"https://ac.duckduckgo.com/ac/?q={WebUtility.UrlEncode(query)}";

                string str = await client.GetStringAsync(url);
                var ddgsuggestions = JsonSerializer.Deserialize<List<DDGSuggestion>>(str);

                foreach (var ddgsuggestion in ddgsuggestions)
                {
                    Suggestion suggestion = new();
                    suggestion.Title = ddgsuggestion.phrase;
                    suggestion.Url = await WebViewNavigateUrlHelper.ToUrl(ddgsuggestion.phrase);
                    suggestion.Kind = SuggestionKind.Search;
                    suggestion.Query = query;

                    suggestions.Add(suggestion);
                }
            }

            return suggestions;
        }
    }
}
