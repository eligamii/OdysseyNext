using Odyssey.Data.Helpers;
using Odyssey.Shared.ViewModels.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Odyssey.Data.Main
{
    public class Logins
    {

        public static ObservableCollection<Login> Items { get; set; }

        public static void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Items, options);

            // Encrypt the string
            byte[] encryptedJsonString = EncryptionHelpers.ProtectString(jsonString);

            File.WriteAllBytes(Data.LoginsFilePath, encryptedJsonString);
        }

        internal static void Load()
        {
            if (File.Exists(Data.LoginsFilePath))
            {
                // Get encrypted json
                byte[] encryptedJsonString = File.ReadAllBytes(Data.LoginsFilePath);

                // Decrypt the encrypted string
                string jsonString = EncryptionHelpers.UnprotectToString(encryptedJsonString);

                Items = JsonSerializer.Deserialize<ObservableCollection<Login>>(jsonString);
            }
            else
            {
                Items = new ObservableCollection<Login>();
            }

            

            Items.CollectionChanged += (s, a) => Save();
        }
    }
}
