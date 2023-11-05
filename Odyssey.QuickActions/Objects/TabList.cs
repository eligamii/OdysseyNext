using Odyssey.Data.Main;
using Odyssey.Shared.ViewModels.Data;
using System.Collections.Generic;
using System.Linq;

namespace Odyssey.QuickActions.Objects
{
    internal class TabList
    {
        internal List<Tab> Items { get; private set; }
        internal TabList(string option) // in x;a-b or x:a,b,c format
        {
            List<Tab> tabs = new(); // tabs which will be the Items value
            List<int> indexes = new();
            string[] tabIndex = option.Split(";");

            Tab[] list = null; // Tab.Items, Favorites.Items or Pins.Items

            // Getting the tab list corresponding to the x value
            switch (tabIndex[0])
            {
                case "0":
                    list = Favorites.Items.ToArray();
                    break;

                case "1":
                    list = Pins.Items.ToArray();
                    break;

                default:
                    list = Tabs.Items.ToArray();
                    break;

            }



            if (tabIndex[1].Contains(","))
            {
                List<string> tabIndexString = tabIndex[1].Split(",").ToList();

                foreach (string index in tabIndexString)
                {
                    if (tabIndex[1].Contains("-"))
                    {
                        string[] min = index.Split("-");
                        for (int i = int.Parse(min[0]); i != int.Parse(min[1]) + 1; i++)
                        {
                            indexes.Add(i);
                        }
                    }
                    else
                    {
                        indexes.Add(int.Parse(index));
                    }
                }
            }
            else
            {
                indexes.Add(int.Parse(tabIndex[1]));
            }

            foreach (int index in indexes)
            {
                try
                {
                    tabs.Add(list.ElementAt(index));
                }
                catch { }
            }

            Items = tabs;

        }
    }
}
