using Odyssey.Data.Main;
using Odyssey.Migration.Helpers;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Migration
{
    public class MigrationData
    {
        public List<Login> Logins { get; set; }
        public List<HistoryItem> History { get; set; }
        public List<Tab> Bookmarks { get; set; }
        public List<Tab> MostVisitedWebsites { get; set; }
    }

    public class Browser
    {
        public enum Name
        {
            Chrome,
            Edge,
            Opera,

            Firefox
        }

        public enum Base
        {
            Chromium,
            Gecko
        }
        public Name BrowserName { get; set; }
        public Base BrowserBase { get; set; }


        public static Browser Edge { get; } = new() { BrowserName = Name.Edge, BrowserBase = Base.Chromium };
        public static Browser Chrome { get; } = new() { BrowserName = Name.Chrome, BrowserBase = Base.Chromium };
        public static Browser Opera { get; } = new() { BrowserName = Name.Opera, BrowserBase = Base.Chromium };
        public static Browser Firefox { get; } = new() { BrowserName = Name.Firefox, BrowserBase = Base.Gecko };
        /// <summary>
        /// Use this list to get the path of a browser by its Name (enum) as an int
        /// </summary>
        public static List<string> Paths { get; } = new() // in the exact same order than the Name enum
        {
            @"Local\Microsoft\Edge\User Data",
            @"Local\Google\Chrome\User Data",
            @"Roaming\Opera Software\Opera Stable"
        };
    }



    public class Migration
    {
        public static void Migrate(Browser from)
        {
            MigrationData data = new();
            DirectoryInfo file = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            string appData = file.Parent.FullName + @"\";

            // Get the path of the data folder of the browser
            string datapath = Browser.Paths.ElementAt((int)from.BrowserName);

            if (from.BrowserBase == Browser.Base.Chromium)
            {
                data.Logins = Chromium.Passwords.Get(datapath);
                data.History = Chromium.History.Get(datapath);

            }
        }
    }
}
