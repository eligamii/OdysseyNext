using Odyssey.FWebView;
using Odyssey.QuickActions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Commands
{
    internal class Forward
    {
        internal static Res Exec(string[] options)
        {
            if (WebView.SelectedWebView != null)
            {
                if (WebView.SelectedWebView.CanGoForward)
                {
                    WebView.SelectedWebView.GoForward();
                    return new Res(true);
                }

                return new Res(true, null, "the webview cannot go forward");
            }
            return new Res(false, null, "no tab is currently selected");
        }
    }
}
