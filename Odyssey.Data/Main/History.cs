using Odyssey.Shared.ViewModels.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Odyssey.Data.Main
{
    public class History
    {

        public static ObservableCollection<HistoryItem> Items { get; set; }

        internal static void Save()
        {
            // If someone wants to mnually edit JSON save files
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Items, options);
            File.WriteAllText(Data.HistoryFilePath, jsonString);
        }

        internal static void Load()
        {
            if (File.Exists(Data.HistoryFilePath))
            {
                string jsonString = File.ReadAllText(Data.HistoryFilePath);
                Items = JsonSerializer.Deserialize<ObservableCollection<HistoryItem>>(jsonString);
            }
            else
            {
                Items = new ObservableCollection<HistoryItem>();
            }
            

            Items.CollectionChanged += (s, a) => Save();
        }
    }
}
