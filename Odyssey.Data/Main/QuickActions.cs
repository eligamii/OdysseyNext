using Odyssey.Shared.ViewModels.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Odyssey.Data.Main
{
    public class QuickActions
    {
        public static ObservableCollection<QuickAction> Items { get; set; }

        internal static void Save()
        {

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Items, options);
            File.WriteAllText(Data.QuickActionFilePath, jsonString);
        }

        internal static void Load()
        {
            if (File.Exists(Data.QuickActionFilePath))
            {
                string jsonString = File.ReadAllText(Data.QuickActionFilePath);

                Items = JsonSerializer.Deserialize<ObservableCollection<QuickAction>>(jsonString);
            }
            else
            {
                Items = new ObservableCollection<QuickAction>();
            }

            

            Items.CollectionChanged += (s, a) => Save();
        }
    }
}
