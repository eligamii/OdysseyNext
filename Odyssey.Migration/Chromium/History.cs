using Microsoft.Data.Sqlite;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace Odyssey.Migration.Chromium
{
    public static class History
    {
        public static List<HistoryItem> Get(string path)
        {
            path = Path.Combine(path, "Default", "History");

            using (SqliteConnection connection = new SqliteConnection($"Filename={path}"))
            {
                connection.Open();

                string lastUrl = "https://lorem.placeholder.ipsum";
                string lastTitle = "";

                List<HistoryItem> list = new();

                SqliteCommand selectHistoryCommand = new SqliteCommand("SELECT url, title, hidden FROM urls", connection);
                SqliteDataReader query = selectHistoryCommand.ExecuteReader();

                while (query.Read())
                {
                    // Simple filter save only 
                    if (query.GetString(1) != string.Empty && lastUrl != query.GetString(0) && query.GetString(2) != "1" && !(new Uri(lastUrl).Host == new Uri(query.GetString(0)).Host && lastTitle == query.GetString(1)))
                    {
                        HistoryItem historyItem = new HistoryItem();
                        historyItem.Title = query.GetString(1);
                        historyItem.Url = query.GetString(0);

                        historyItem.Description = historyItem.Title + "\n\n" + historyItem.Url;

                        list.Insert(0, historyItem);

                        lastUrl = query.GetString(0);
                        lastTitle = query.GetString(1);
                    }
                }

                connection.Close();
                return list;
            }

        }

    }
}
