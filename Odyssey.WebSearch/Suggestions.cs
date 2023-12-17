﻿using Odyssey.Shared.ViewModels.WebSearch;
using Odyssey.WebSearch.Helpers;
using Odyssey.WebSearch.Helpers.Suggestions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Odyssey.WebSearch
{
    public static class Suggestions
    {
        public static string CurrentQuery { get; set; }
        public async static Task<List<Suggestion>> Suggest(string query, int maxSuggestions)
        {
            List<Suggestion> suggestions = new();
            var kind = await WebSearchStringKindHelpers.GetStringKindAsync(query);

            if (kind == WebSearchStringKindHelpers.StringKind.MathematicalExpression)
            {
                var suggestion = await MathematicalExpressionsSuggestionsHelper.EvaluateExpression(query);
                suggestions.Add(suggestion);
            }

            suggestions = suggestions.Concat(TabsSuggestionsHelper.SearchForMatchingTabs(query)).ToList();

            try
            {
               // var ddgSuggestions = (await DuckDuckGoSuggestionsHelper.GetFromDuckDuckGoSuggestions(query)).Where(p => p.Query == CurrentQuery);
                //suggestions = suggestions.Concat(ddgSuggestions).ToList();
            } catch (HttpRequestException) { }

            return suggestions.Take(maxSuggestions).ToList();
        }
    }
}
