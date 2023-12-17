using Odyssey.QuickActions.Objects;

namespace Odyssey.QuickActions.Commands
{
    internal class Forward
    {
        internal static Res Exec(string[] options)
        {
            if (Variables.SelectedWebView != null)
            {
                if (Variables.SelectedWebView.CanGoForward)
                {
                    Variables.SelectedWebView.GoForward();
                    return new Res(true);
                }

                return new Res(true, null, "the webview cannot go forward");
            }
            return new Res(false, null, "no tab is currently selected");
        }
    }
}
