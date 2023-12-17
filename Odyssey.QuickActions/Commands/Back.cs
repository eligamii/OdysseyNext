using Odyssey.QuickActions.Objects;

namespace Odyssey.QuickActions.Commands
{
    
    internal class Back
    {
        internal static Res Exec(string[] options)
        {
            if (Variables.SelectedWebView != null)
            {
                if (Variables.SelectedWebView.CanGoBack)
                {
                    Variables.SelectedWebView.GoBack();
                    return new Res(true);
                }

                return new Res(true, null, "the webview cannot go back");
            }
            return new Res(false, null, "no tab is currently selected");
        }
    }
}
