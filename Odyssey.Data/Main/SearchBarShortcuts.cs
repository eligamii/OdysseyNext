using Newtonsoft.Json;
using Odyssey.Shared.ViewModels.WebSearch;
using System.Collections.ObjectModel;
using System.IO;

namespace Odyssey.Data.Main
{
    public class SearchBarShortcuts
    {
        public static ObservableCollection<Suggestion> Items { get; set; }

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
            File.WriteAllText(Data.SearchBarShortcutsFilePath, serializedObject);
        }

        internal static void Load()
        {
            if (File.Exists(Data.SearchBarShortcutsFilePath))
            {
                string jsonString = File.ReadAllText(Data.SearchBarShortcutsFilePath);

                Items = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Suggestion>>(jsonString);
            }
            else
            {
                Items = new ObservableCollection<Suggestion>();
            }



            Items.CollectionChanged += (s, a) => Save();
        }
    }
}
