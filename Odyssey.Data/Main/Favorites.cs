using Newtonsoft.Json;
using Odyssey.Shared.ViewModels.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Odyssey.Data.Main
{
    public class Favorites
    {

        public static ObservableCollection<Favorite> Items { get; set; }

        internal static void Save()
        {
            string serializedObject = JsonConvert.SerializeObject(Items, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true,
                    IgnoreSerializableInterface = true,
                    IgnoreShouldSerializeMembers = true,
                },
                Formatting = Newtonsoft.Json.Formatting.Indented,
            });
            File.WriteAllText(Data.FavoritesFilePath, serializedObject);
        }

        internal static void Load()
        {
            if (File.Exists(Data.FavoritesFilePath))
            {
                string jsonString = File.ReadAllText(Data.FavoritesFilePath);

                Items = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Favorite>>(jsonString);
            }
            else
            {
                Items = new ObservableCollection<Favorite>();
            }

            Items.CollectionChanged += (s, a) => Save();
        }
    }
}
