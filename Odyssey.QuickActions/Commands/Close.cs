using Odyssey.Data.Main;
using Odyssey.QuickActions.Objects;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Linq;

namespace Odyssey.QuickActions.Commands
{
    internal static class Close
    {
        internal static Res Exec(string[] options)
        {
            if (options.Count() == 0 || options[0] == "this")
            {
                // close the window
                QACommands.MainWindow.Close();
                return new Res(true);
            }
            else if (options[0] == "all")
            {
                int count = Tabs.Items.Count();
                // close every tab
                foreach (var item in Tabs.Items)
                {
                    // Close every tab's webview
                    if (item.MainWebView != null) { item.MainWebView.Close(); }
                }

                Tabs.Items.Clear();
                Tabs.Save(); // Prevent the tabs from being restored after crash
                return new Res(true, count.ToString());
            }
            else if (!string.IsNullOrWhiteSpace(options[0]))
            {
                try
                {
                    var list = new TabList(options[0]).Items;
                    int count = list.Count();

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

                    return new Res(true, count.ToString());
                }
                catch (Exception e) { return new Res(false, null, e.Message); }

            }
            else
            {
                return new Res(false, null, "Incorrect parameters");
            }
        }

    }
}
