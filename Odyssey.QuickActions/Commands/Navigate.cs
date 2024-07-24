using Odyssey.QuickActions.Objects;
using System;
using System.Linq;

namespace Odyssey.QuickActions.Commands
{
    internal class Navigate
    {
        internal static Res Exec(string[] options)
        {
            if (options.Count() == 0) return new Res(false, null, "This command requires one parameter: url");

            var opt = new Option(options[0]);

            if (opt.Name != "url") return new Res(false, null, "Invalid parameter (only parameter accepted: url)");

            string option = opt.Value;
            var kind = WebSearch.Helpers.WebSearchStringKindHelpers.GetStringKind(option);

            if (kind != WebSearch.Helpers.WebSearchStringKindHelpers.StringKind.Url) return new Res(false, null, "The parameter should be an url");
            else
            {
                NavigateTo(option);
                return new Res(true);
            }

        }

        private static async void NavigateTo(string str)
        {
            string url = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToWebView2Url(str);
            Variables.SelectedWebView.CoreWebView2.Navigate(url);
        }
    }
}
