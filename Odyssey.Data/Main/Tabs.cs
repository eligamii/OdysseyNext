using Newtonsoft.Json;
using Odyssey.Shared.ViewModels.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Odyssey.Data.Main
{
    public class Tabs
    {
        public static ObservableCollection<Tab> Items { get; set; } = new();

        public static void Save()
        {
            try
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
                File.WriteAllText(Data.TabsFilePath, serializedObject);
            }
            catch { }
        }

        internal static void Load()
        {
            Items.CollectionChanged += (s, a) => Save();
        }

        public static ObservableCollection<Tab> Restore()
        {
            if (File.Exists(Data.TabsFilePath))
            {
                string jsonString = File.ReadAllText(Data.TabsFilePath);

                Items = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Tab>>(jsonString);
            }

            return Items;

        }
    }
}
