﻿using Odyssey.Data.Helpers;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Odyssey.Data.Main
{
    public class TwoFactorsAuthentification
    {
        public static ObservableCollection<TwoFactorAuthItem> Items { get; set; }

        public static void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Items, options);

            // Encrypt the string
            byte[] encryptedJsonString = EncryptionHelpers.ProtectString(jsonString);

            File.WriteAllBytes(Data.TotpFilePath, encryptedJsonString);
        }

        public static async Task<ObservableCollection<TwoFactorAuthItem>> Load()
        {
            if (File.Exists(Data.TotpFilePath))
            {
                // Get encrypted json
                byte[] encryptedJsonString = File.ReadAllBytes(Data.TotpFilePath);

                // Decrypt the encrypted string
                string jsonString = EncryptionHelpers.UnprotectToString(encryptedJsonString);

                Items = JsonSerializer.Deserialize<ObservableCollection<TwoFactorAuthItem>>(jsonString);
            }
            else
            {
                Items = new ObservableCollection<TwoFactorAuthItem>();
            }

            await Task.Delay(0);

            Items.CollectionChanged += (s, a) => Save();
            return Items;
        }
    }
}
