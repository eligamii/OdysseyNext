using Newtonsoft.Json;
using Odyssey.TwoFactorsAuthentification.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Odyssey.TwoFactorsAuthentification.Data
{
    internal static class TwoFactAuthData
    {
        public static ObservableCollection<TwoFactAuth> Items { get; set; }
        private static string dataPath;

        internal static void Save()
        {
            // If someone wants to mnually edit JSON save files
            string serializedObject = JsonConvert.SerializeObject(Items, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true,
                    IgnoreSerializableInterface = true,
                    IgnoreShouldSerializeMembers = true,
                },
                Formatting = Formatting.Indented,
            });
            File.WriteAllText(dataPath, serializedObject);
        }

        internal static async void Load()
        {
            var dataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            string path = dataFolder.Path;

            dataPath = Path.Combine(path, "2FA.json");

            if (File.Exists(dataPath))
            {
                string jsonString = File.ReadAllText(dataPath);

                Items = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<TwoFactAuth>>(jsonString);
            }
            else
            {
                Items = new ObservableCollection<TwoFactAuth>();
            }



            Items.CollectionChanged += (s, a) => Save();
        }
    }
}
