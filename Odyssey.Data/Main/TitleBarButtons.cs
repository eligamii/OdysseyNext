using Newtonsoft.Json;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.Data.Main
{
    public static class TitleBarButtons
    {
        public static List<TitleBarButton> Items { get; set; } = new();

        public static void Save()
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
            File.WriteAllText(Data.TitleBarButtonsFilePath, serializedObject);
        }

        internal static void Load()
        {
            if (File.Exists(Data.TitleBarButtonsFilePath))
            {
                string jsonString = File.ReadAllText(Data.TitleBarButtonsFilePath);

                Items = System.Text.Json.JsonSerializer.Deserialize<List<TitleBarButton>>(jsonString);
            }
            else
            {
                // Create the home and search buttons
                Items = [new TitleBarButton() { Id = 3, Icon = "\uE721", Title = "Search" }, new TitleBarButton() { Id = 9, Icon = "\uE80F", Title = "Home" }];
            }
        }
    }
}
