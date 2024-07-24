using Odyssey.QuickActions.Objects;

namespace Odyssey.QuickActions.Commands
{
    internal class Webview
    {
        internal static Res Exec(string[] options)
        {
            if (Variables.SelectedWebView != null)
            {
                switch (options[0])
                {
                    case "devtools": Variables.SelectedWebView.CoreWebView2?.OpenDevToolsWindow(); break;
                    case "taskmgr": Variables.SelectedWebView.CoreWebView2?.OpenTaskManagerWindow(); break;

                    default: return new Res(false, null, "Invalid parameter");
                }
            }
            return new Res(false, null, "No tab is currently selected");
        }
    }
}
