using Odyssey.Data.Main;
using Odyssey.QuickActions.Objects;
using Odyssey.Shared.ViewModels.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

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
            else if (options[0] == "all")
            {
                // close every tab
                foreach (var item in Tabs.Items)
                {
                    // Close every tab's webview
                    if (item.MainWebView != null) { item.MainWebView.Close(); }
                }

                Tabs.Items.Clear();
                Tabs.Save(); // Prevent the tabs from being restored after crash
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(options[0]))
            {
                try
                {
                    var list = new TabList(options[0]).Items;

                    if (options[0].Contains("0;"))
                    {
                        foreach (var item in list)
                        {
                            Favorites.Items.Remove(item as Favorite);
                        }
                    }
                    else if (options[0].Contains("1;"))
                    {
                        foreach (var item in list)
                        {
                            Pins.Items.Remove(item as Pin);
                        }
                    }
                    else
                    {
                        foreach (var item in list)
                        {
                            Tabs.Items.Remove(item);
                        }
                    }

                    return true;
                }
                catch { return false; }
                
            }
            else
            {
                return false; // incorrect command
            }
        }

        

    }
}
