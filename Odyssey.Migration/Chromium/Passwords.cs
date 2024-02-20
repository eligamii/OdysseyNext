using Microsoft.Data.Sqlite;
using Odyssey.Migration.Helpers;
using Odyssey.Shared.ViewModels.Data;
using System.Collections.Generic;
using System.IO;

namespace Odyssey.Migration.Chromium
{
    public static class Passwords
    {


        public static List<Login> Get(string default_path)
        {
            List<Login> logins = new();

            string path = Path.Combine(default_path, "Default", "Login Data");

            using (SqliteConnection connection = new SqliteConnection($"Filename={path}"))
            {
                connection.Open();

                SqliteCommand selectHistoryCommand = new SqliteCommand("SELECT action_url, username_value, password_value FROM logins", connection);
                SqliteDataReader query = selectHistoryCommand.ExecuteReader();

                while (query.Read())
                {
                    Login login = new()
                    {
                        Host = query.GetString(0),
                        Username = query.GetString(1),
                    };

                    var encryptedPassword = (byte[])query[2];
                    string passString = ChromiumDataDecryptionHelpers.Decrypt(encryptedPassword, default_path);

                    login.Password = passString;
                    logins.Add(login);
                }
            }

            return logins;
        }





    }
}
