using Odyssey.Shared.ViewModels.WebSearch;
using Odyssey.WebSearch.Helpers;
using Odyssey.WebSearch.Helpers.Suggestions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.WebSearch
{
    public class Suggestions
    {
        public async static Task<List<Suggestion>> Suggest(string query, int maxSuggestions)
        {
            List<Suggestion> suggestions = new();
            var kind = await WebSearchStringKindHelpers.GetStringKind(query);

            if(kind == WebSearchStringKindHelpers.StringKind.MathematicalExpression)
            {
                var suggestion = await MathematicalExpressionsSuggestionsHelper.EvaluateExpression(query);
                suggestions.Add(suggestion);
            }

            suggestions = suggestions.Concat(TabsSuggestionsHelper.SearchForMatchingTabs(query)).ToList();

            //suggestions = suggestions.Concat(await DuckDuckGoSuggestionsHelper.GetFromDuckDuckGoSuggestions(query)).ToList();

            return suggestions;
        }
    }
}
