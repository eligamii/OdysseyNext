using Newtonsoft.Json;
using Odyssey.Shared.ViewModels.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Odyssey.Data.Main
{
    public class Downloads
    {
        public static ObservableCollection<DonwloadItem> Items { get; set; }

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
                Formatting = Formatting.Indented,
            });
            File.WriteAllText(Data.DownloadsFilePath, serializedObject);
        }

        internal static void Load()
        {
            if (File.Exists(Data.DownloadsFilePath))
            {
                string jsonString = File.ReadAllText(Data.DownloadsFilePath);

                Items = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<DonwloadItem>>(jsonString);
            }
            else
            {
                Items = new ObservableCollection<DonwloadItem>();
            }

            Items.CollectionChanged += (s, a) => Save();
        }
    }
}
