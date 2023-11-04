using Odyssey.Data.Main;
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
                foreach (var item in Data.Main.Tabs.Items)
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
                    var list = GetTabs(options[0]);

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

        private static List<Tab> GetTabs(string option)
        {
            List<Tab> tabs = new();
            string[] tabIndex = option.Split(";");

            Tab[] list = null;

            switch(tabIndex[0])
            {
                case "0":
                    list = Favorites.Items.ToArray();
                    break;
                    
                case "1":
                    list = Pins.Items.ToArray();
                    break;

                case "2":
                    list = Tabs.Items.ToArray() ;
                    break;
            }

            int test = 0;

            if (tabIndex[1].Contains(","))
            {
                string[] indexes = tabIndex[1].Split(",");
                foreach (string index in indexes)
                {
                    int i = int.Parse(index);
                    try
                    {
                        tabs.Add(list[i]);
                    }
                    catch { }
                }
            }
            else if (tabIndex[1].Contains("-"))
            {
                string[] min = tabIndex[1].Split("-");
                for (int i = int.Parse(min[0]); i != int.Parse(min[1]); i++)
                {
                    try
                    {
                        tabs.Add(list[i]);
                    }
                    catch { }
                }
            }
            else
            {
                try
                {
                    tabs.Add(list[int.Parse(tabIndex[1])]);
                }
                catch 
                {
                    tabs = list.ToList().Where(p => p.Title.Contains(tabIndex[1].Replace("\"", ""))).ToList();
                }
                
            }

            return tabs;
        }

    }
}
