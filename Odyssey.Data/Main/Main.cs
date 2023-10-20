using System;
using System.IO;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Data.Main
{
    public class Data
    {
        internal static string QuickActionFilePath { get; private set; } // %localappdata%\...\LocalState\Data\QuickActions.json
        internal static string FavoritesFilePath { get; private set; } // %localappdata%\...\LocalState\Data\Favorites.json
        internal static string HistoryFilePath { get; private set; } // %localappdata%\...\LocalState\Data\History.json (custom file for performances and control)
        internal static string LoginsFilePath { get; private set; } // %localappdata%\...\LocalState\Data\Logins.json
        internal static string PinsFilePath { get; private set; } // %localappdata%\...\LocalState\Data\Pins.json


        private static StorageFolder dataFolder;

        public static async void Init()
        {
            // Create the main folder for storing JSON-based data
            dataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            string path = dataFolder.Path;

            QuickActionFilePath = Path.Combine(path, "QuickActions.json");
            FavoritesFilePath = Path.Combine(path, "Favorites.json");
            HistoryFilePath = Path.Combine(path, "History.json");
            LoginsFilePath = Path.Combine(path, "Logins.json");
            PinsFilePath = Path.Combine(path, "Pins.json");

            QuickActions.Load();
            Favorites.Load();
            History.Load();
            Logins.Load();
            Pins.Load();
        }
    }
}
