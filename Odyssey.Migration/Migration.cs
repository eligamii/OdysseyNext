using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
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
            WebView2,

            Firefox
        }

        public enum Base
        {
            Chromium,
            Gecko
        }
        public Name BrowserName { get; set; }
        public Base BrowserBase { get; set; }


        public static Browser Edge = new() { BrowserName = Name.Edge, BrowserBase = Base.Chromium };
        public static Browser Chrome = new() { BrowserName = Name.Chrome, BrowserBase = Base.Chromium };
        public static Browser Opera = new() { BrowserName = Name.Opera, BrowserBase = Base.Chromium };
        public static Browser Firefox = new() { BrowserName = Name.Firefox, BrowserBase = Base.Gecko };
    }



    public class Migration
    {
        public static MigrationData ConvertData(Browser from)
        {
            MigrationData data = new MigrationData();

        }
    }
}
