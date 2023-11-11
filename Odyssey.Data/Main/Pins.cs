using Newtonsoft.Json;
using Odyssey.Shared.ViewModels.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Odyssey.Data.Main
{
    public class Pins
    {

        public static ObservableCollection<Pin> Items { get; set; }

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
            File.WriteAllText(Data.PinsFilePath, serializedObject);
        }

        internal static void Load()
        {
            if (File.Exists(Data.PinsFilePath))
            {
                string jsonString = File.ReadAllText(Data.PinsFilePath);

                Items = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Pin>>(jsonString);
            }
            else
            {
                Items = new ObservableCollection<Pin>();
            }



            Items.CollectionChanged += (s, a) => Save();
        }
    }
}
