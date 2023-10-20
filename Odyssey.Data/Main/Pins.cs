using Odyssey.Shared.DataTemplates.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Odyssey.Data.Main
{
    public class Pins
    {

        public static ObservableCollection<Pin> PinsList { get; set; }

        internal static void Save()
        {
            // If someone wants to mnually edit JSON save files
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(PinsList, options);
            File.WriteAllText(Data.PinsFilePath, jsonString);
        }

        internal static void Load()
        {
            if (File.Exists(Data.PinsFilePath))
            {
                string jsonString = File.ReadAllText(Data.PinsFilePath);

                PinsList = JsonSerializer.Deserialize<ObservableCollection<Pin>>(jsonString);
            }

            PinsList = new ObservableCollection<Pin>();

            PinsList.CollectionChanged += (s, a) => Save();
        }
    }
}
