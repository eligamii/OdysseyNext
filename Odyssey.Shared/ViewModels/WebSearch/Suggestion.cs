using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.Shared.ViewModels.WebSearch
{
    public enum SuggestionKind
    {
        Search, // ex: "Hello world"
        Url, 
        MathematicalExpression,
        Tab,
        History

    }

    public enum TabLocation
    {
        Favorites,
        Pins,
        Tabs,
    }

    public class Suggestion
    {
        public SuggestionKind Kind { get; set; } = SuggestionKind.Search;
        public string Title { get; set; }
        public string Url { get; set;}
        public KeyValuePair<TabLocation, Tab> Tab { get; set; }
        public string Glyph
        {
            get
            {
                switch (Kind)
                {
                    case SuggestionKind.Search: return "\uE721";
                    case SuggestionKind.History: return "\uE81C";
                    case SuggestionKind.MathematicalExpression: return "\uE1D0";
                    case SuggestionKind.Tab: return "\uE737";
                    case SuggestionKind.Url: return "\uE167";
                    default: return "\uE11A";
                }
            }
        }
    }
}
