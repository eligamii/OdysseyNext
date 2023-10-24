using Odyssey.Shared.ViewModels.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Odyssey.Data.Main
{
    public class Logins
    {

        public static ObservableCollection<Login> LoginList { get; set; }

        internal static void Save()
        {
            // If someone wants to mnually edit JSON save files
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(LoginList, options);
            File.WriteAllText(Data.PinsFilePath, jsonString);
        }

        internal static void Load()
        {
            if (File.Exists(Data.LoginsFilePath))
            {
                string jsonString = File.ReadAllText(Data.LoginsFilePath);

                LoginList = JsonSerializer.Deserialize<ObservableCollection<Login>>(jsonString);
            }

            LoginList = new ObservableCollection<Login>();

            LoginList.CollectionChanged += (s, a) => Save();
        }
    }
}
