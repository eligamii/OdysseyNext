using Newtonsoft.Json;
using Odyssey.Downloads.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace Odyssey.Downloads.Data
{
    public class Downloads
    {
        public static ObservableCollection<DownloadItem> Items { get; set; }
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
                Formatting = Newtonsoft.Json.Formatting.Indented,
            });
            File.WriteAllText(dataPath, serializedObject);
        }

        public static async void Load()
        {
            var dataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            string path = dataFolder.Path;

            dataPath = Path.Combine(path, "Downloas.json");

            if (File.Exists(dataPath))
            {
                string jsonString = File.ReadAllText(dataPath);

                Items = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<DownloadItem>>(jsonString);
            }
            else
            {
                Items = new ObservableCollection<DownloadItem>();
            }



            Items.CollectionChanged += (s, a) => Save();
        }
    }
}
