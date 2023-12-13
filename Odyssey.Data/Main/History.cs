using Odyssey.Data.Helpers;
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

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Items, options);

            // Encrypt the string
            byte[] encryptedJsonString = EncryptionHelpers.ProtectString(jsonString);

            File.WriteAllBytes(Data.HistoryFilePath, encryptedJsonString);
        }

        internal static void Load()
        {
            if (File.Exists(Data.HistoryFilePath))
            {
                // Get encrypted json
                byte[] encryptedJsonString = File.ReadAllBytes(Data.HistoryFilePath);

                // Decrypt the encrypted string
                string jsonString = EncryptionHelpers.UnprotectToString(encryptedJsonString);

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
