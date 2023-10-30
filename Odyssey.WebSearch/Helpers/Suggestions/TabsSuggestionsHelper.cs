using Odyssey.Data.Main;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Shared.ViewModels.WebSearch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Odyssey.WebSearch.Helpers.Suggestions
{
    internal static class TabsSuggestionsHelper
    {
        internal static List<Suggestion> SearchForMatchingTabs(string query)
        {
            List<Suggestion> suggestions = new List<Suggestion>();

            var tabs = Tabs.Items.Where(p => MatchHelper.IsMatch(query, p.Title, 0.8f).Success || MatchHelper.IsMatch(query, p.Url, 0.8f).Success);
            var pins = Pins.Items.Where(p => MatchHelper.IsMatch(query, p.Title, 0.8f).Success || MatchHelper.IsMatch(query, p.Url, 0.8f).Success);
            var favorites = Favorites.Items.Where(p => MatchHelper.IsMatch(query, p.Title, 0.8f).Success || MatchHelper.IsMatch(query, p.Url, 0.8f).Success);

            var list = tabs.Concat(pins).Concat(favorites).ToList();

            foreach (var item in list)
            {
                Suggestion suggestion = new();
                TabLocation tabLocation = new();

                if (tabs.Contains(item)) tabLocation = TabLocation.Tabs;
                else if (pins.Contains(item)) tabLocation = TabLocation.Pins;
                else if (favorites.Contains(item)) tabLocation = TabLocation.Favorites;

                KeyValuePair<TabLocation, Tab> keyValuePair = new(tabLocation, item);

                suggestion.Tab = keyValuePair;
                suggestion.Title = item.Title;
                suggestion.Kind = SuggestionKind.Tab;

                suggestions.Add(suggestion);
            }

            return suggestions;

        }
    }
}
