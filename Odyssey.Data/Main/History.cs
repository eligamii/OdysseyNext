using Odyssey.Shared.DataTemplates.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Odyssey.Data.Main
{
    public class History
    {

        public static ObservableCollection<HistoryItem> HistoryList { get; set; }

        internal static void Save()
        {
            // If someone wants to mnually edit JSON save files
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(HistoryList, options);
            File.WriteAllText(Data.HistoryFilePath, jsonString);
        }

        internal static void Load()
        {
            if (File.Exists(Data.HistoryFilePath))
            {
                string jsonString = File.ReadAllText(Data.HistoryFilePath);
                HistoryList = JsonSerializer.Deserialize<ObservableCollection<HistoryItem>>(jsonString);
            }

            HistoryList = new ObservableCollection<HistoryItem>();

            HistoryList.CollectionChanged += (s, a) => Save();
        }
    }
}
