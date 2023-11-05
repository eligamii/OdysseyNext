﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Odyssey.QuickActions.Data
{
    public class UserVariables
    {
        public static List<KeyValuePair<string, string>> Items { get; set; }
        private static string dataPath;

        public static void Save()
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

        public static async Task Load()
        {
            if(Items == null)
            {
                var dataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
                string path = dataFolder.Path;

                dataPath = Path.Combine(path, "UserVariables.json");

                if (File.Exists(dataPath))
                {
                    string jsonString = File.ReadAllText(dataPath);

                    Items = System.Text.Json.JsonSerializer.Deserialize<List<KeyValuePair<string, string>>>(jsonString);
                }
                else
                {
                    Items = new List<KeyValuePair<string, string>>();
                }
            }
        }
    }
}
