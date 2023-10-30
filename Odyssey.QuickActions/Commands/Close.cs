using Odyssey.Data.Main;
using System.Linq;

namespace Odyssey.QuickActions.Commands
{
    internal static class Close
    {
        internal static bool Exec(string[] options)
        {
            if (options.Count() == 0 || options[0] == "this")
            {
                // close the window
                QACommands.MainWindow.Close();
                return true;
            }
            else if (options[0] == "tabs")
            {
                // close every tab
                foreach (var item in Data.Main.Tabs.Items)
                {
                    // Close every tab's webview
                    if (item.MainWebView != null) { item.MainWebView.Close(); }
                }

                Tabs.Items.Clear();
                Tabs.Save(); // Prevent the tabs from being restored after crash
                return true;
            }
            else
            {
                return false; // incorrect command
            }
        }

    }
}
