using Odyssey.FWebView;
using Odyssey.Migration;
using Odyssey.QuickActions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Commands
{
    internal class Back
    {
        internal static Res Exec(string[] options)
        {
            if(WebView.SelectedWebView != null)
            {
                if(WebView.SelectedWebView.CanGoBack)
                {
                    WebView.SelectedWebView.GoBack();
                    return new Res(true);
                }

                return new Res(true, null, "the webview cannot go back");
            }
            return new Res(false, null, "no tab is currently selected");
        }
    }
}
