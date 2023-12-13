using Odyssey.FWebView;
using Odyssey.QuickActions.Objects;

namespace Odyssey.QuickActions.Commands
{
    internal class Webview
    {
        internal static Res Exec(string[] options)
        {
            if (WebView.SelectedWebView != null)
            {
                switch (options[0])
                {
                    case "devtools": WebView.SelectedWebView.CoreWebView2?.OpenDevToolsWindow(); break;
                    case "taskmgr": WebView.SelectedWebView.CoreWebView2?.OpenTaskManagerWindow(); break;
                    case "downloads": WebView.OpenDownloadDialog(); break;
                    case "history": WebView.OpenHistoryDialog(); break;

                    default: return new Res(false, null, "Invalid parameter");
                }
            }
            return new Res(false, null, "No tab is currently selected");
        }
    }
}
