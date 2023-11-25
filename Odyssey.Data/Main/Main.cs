using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Data.Main
{
    public class Data
    {
        internal static string SearchBarShortcutsFilePath { get; private set; }
        internal static string QuickActionFilePath { get; private set; } // %localappdata%\...\LocalState\Data\QuickActions.json
        internal static string FavoritesFilePath { get; private set; } // %localappdata%\...\LocalState\Data\Favorites.json
        internal static string HistoryFilePath { get; private set; } // %localappdata%\...\LocalState\Data\History.json (custom file for performances and control)
        internal static string LoginsFilePath { get; private set; } // %localappdata%\...\LocalState\Data\Logins.json
        internal static string PinsFilePath { get; private set; } // %localappdata%\...\LocalState\Data\Pins.json
        internal static string TabsFilePath { get; private set; }
        internal static string TotpFilePath { get; private set; }



        private static StorageFolder dataFolder;

        public static async Task Init()
        {
            // Create the main folder for storing JSON-based data
            dataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            string path = dataFolder.Path;

            SearchBarShortcutsFilePath = Path.Combine(path, "SearchBarShortcuts.json");
            QuickActionFilePath = Path.Combine(path, "QuickActions.json");
            FavoritesFilePath = Path.Combine(path, "Favorites.json");
            HistoryFilePath = Path.Combine(path, "History.json");
            LoginsFilePath = Path.Combine(path, "Logins.json");
            PinsFilePath = Path.Combine(path, "Pins.json");
            TabsFilePath = Path.Combine(path, "Tabs.json");


            SearchBarShortcuts.Load();
            QuickActions.Load();
            Favorites.Load();
            History.Load();
            Logins.Load();
            Pins.Load();
            Tabs.Load();
        }
    }
}
