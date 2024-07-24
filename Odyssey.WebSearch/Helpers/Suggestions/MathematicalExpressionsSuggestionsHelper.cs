using NCalc;
using Odyssey.Shared.ViewModels.WebSearch;
using System.Threading.Tasks;

namespace Odyssey.WebSearch.Helpers.Suggestions
{
    internal class MathematicalExpressionsSuggestionsHelper
    {
        internal static async Task<Suggestion> EvaluateExpression(string query)
        {
            Suggestion suggestion = new();
            Expression e = new Expression(query);

            try
            {
                var res = e.Evaluate();
                string resString = res.ToString();

                suggestion.Title = resString;
                suggestion.Url = await WebViewNavigateUrlHelper.ToWebView2Url(resString);
                suggestion.Kind = SuggestionKind.MathematicalExpression;

                return suggestion;
            }
            catch
            {
                return null;
            }
        }
    }
}
