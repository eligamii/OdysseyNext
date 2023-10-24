using Odyssey.Shared.ViewModels.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Odyssey.Data.Main
{
    public class Favorites
    {

        public static ObservableCollection<Favorite> Items { get; set; }

        internal static void SaveFavorites()
        {
            // If someone wants to mnually edit JSON save files
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Items, options);
            File.WriteAllText(Data.FavoritesFilePath, jsonString);
        }

        internal static void Load()
        {
            if (File.Exists(Data.FavoritesFilePath))
            {
                string jsonString = File.ReadAllText(Data.FavoritesFilePath);

                Items = JsonSerializer.Deserialize<ObservableCollection<Favorite>>(jsonString);
            }

            Items = new ObservableCollection<Favorite>();

            Items.CollectionChanged += (s, a) => SaveFavorites();
        }
    }
}
