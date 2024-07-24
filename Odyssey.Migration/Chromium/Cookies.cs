using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Odyssey.Migration.Helpers;
using System;
using System.IO;

namespace Odyssey.Migration.Chromium
{
    public static class Cookies
    {
        private static WebView2 manipulationWebView = new();

        public static async void Apply(string default_path)
        {
            await manipulationWebView.EnsureCoreWebView2Async();
            CoreWebView2 core = manipulationWebView.CoreWebView2;
            CoreWebView2CookieManager manager = core.Profile.CookieManager;

            string path = Path.Combine(default_path, "Default", "Network", "Cookies");

            using (SqliteConnection connection = new SqliteConnection($"Filename={path}"))
            {
                connection.Open();

                SqliteCommand selectHistoryCommand = new SqliteCommand("SELECT name, encrypted_value, host_key, path, is_secure, is_httponly, samesite FROM cookies", connection);
                SqliteDataReader query = selectHistoryCommand.ExecuteReader();

                while (query.Read())
                {
                    // Decrypt the cookie
                    var encryptedCookie = (byte[])query[1];
                    string decryptedCookieValue = ChromiumDataDecryptionHelpers.Decrypt(encryptedCookie, default_path);

                    // Convert the cookie
                    var cookie = manager.CreateCookie(query.GetString(0), decryptedCookieValue, query.GetString(2), query.GetString(3));
                    cookie.IsSecure = query.GetInt16(4) == 1;
                    cookie.IsHttpOnly = query.GetInt16(5) == 1;

                    short sameSite = query.GetInt16(6);
                    if (sameSite != -1) cookie.SameSite = (CoreWebView2CookieSameSiteKind)sameSite;

                    // Add the cookie
                    manager.AddOrUpdateCookie(cookie);
                }
            }
        }

    }
}
