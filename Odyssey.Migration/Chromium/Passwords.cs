using Microsoft.Data.Sqlite;
using Odyssey.Migration.Helpers;
using Odyssey.Shared.ViewModels.Data;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using static Odyssey.Migration.Chromium.Passwords;

namespace Odyssey.Migration.Chromium
{
    public class Passwords
    {
        public class os_crypt // Object made to easily get the decryption key
        {
            public string encrypted_key { get; set; }
        }

        public class Key
        {
            public os_crypt os_crypt { get; set; }
        }

        private static List<Login> Get()
        {
            List<Login> logins = new();

            string path = @"C:\Users\eliga\AppData\Local\Microsoft\Edge\User Data\Default\Login Data";

            using (SqliteConnection connection = new SqliteConnection($"Filename={path}"))
            {
                connection.Open();

                SqliteCommand selectHistoryCommand = new SqliteCommand("SELECT action_url, username_value, password_value FROM logins", connection);
                SqliteDataReader query = selectHistoryCommand.ExecuteReader();

                while(query.Read())
                {
                    Login login = new()
                    {
                        Host = query.GetString(0),
                        Username = query.GetString(1),
                    };

                    var pass = (byte[])query[2];
                    var key = DataDecryptionHelpers.GetKey(GetKeyString());

                    byte[] nonce, ciphertextTag;
                    DataDecryptionHelpers.Prepare(pass, out nonce, out ciphertextTag);
                    string passString = DataDecryptionHelpers.Decrypt(ciphertextTag, key, nonce);

                    login.Password = passString;
                    logins.Add(login);
                }
            }

            return logins;
        }

        private static string GetKeyString()
        {
            // Get decryption key stored in the browser's 'Local State' json file
            string localState = File.ReadAllText("C:\\Users\\eliga\\AppData\\Local\\Microsoft\\Edge\\User Data\\Local State");

            var obj = JsonSerializer.Deserialize<Key>(localState);

            return obj.os_crypt.encrypted_key;
        }

        

        
    }
}
