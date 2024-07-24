using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Odyssey.FWebView.Helpers
{
    public class WebView2SavedFavicons
    {
        private static List<IconMap> iconMaps = new();
        private static List<Icon> icons = new();
        public class IconMap
        {
            public List<string> Urls { get; set; }
            public int IconId { get; set; } = 0;
        }

        public class Icon
        {
            public int IconId { get; set; }
            public BitmapImage Image { get; set; }
            public int Quality { get; set; }
        }


        /// <summary>
        /// Load saved favicons
        /// </summary>
        public static async void Init()
        {
            try
            {
                // Favicons database
                string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "EBWebView", "Default", "Favicons");
                if (!Directory.Exists(path)) return;

                using SqliteConnection connection = new SqliteConnection($"Filename={path}");
                connection.Open();

                SqliteCommand command = new SqliteCommand("SELECT page_url, icon_id FROM icon_mapping", connection);
                SqliteDataReader query = command.ExecuteReader();

                while (query.Read())
                {
                    string url = query.GetString(0);
                    int id = query.GetInt32(1);

                    if (iconMaps.Any(p => p.IconId == id))
                    {
                        iconMaps.Where(p => p.IconId == id).FirstOrDefault().Urls.Add(url);
                    }
                    else
                    {
                        IconMap iconMap = new()
                        {
                            IconId = id,
                            Urls = new List<string> { url }
                        };

                        iconMaps.Add(iconMap);
                    }
                }




                command = new SqliteCommand("SELECT image_data, icon_id, width FROM favicon_bitmaps", connection);
                query = command.ExecuteReader();

                while (query.Read())
                {
                    int id = query.GetInt32(1);
                    var bytes = (byte[])query[0];
                    int quality = query.GetInt32(2);

                    InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                    await randomAccessStream.WriteAsync(bytes.AsBuffer());
                    randomAccessStream.Seek(0);

                    Icon icon = new();
                    icon.IconId = id;
                    icon.Quality = quality;

                    BitmapImage image = new();
                    image.SetSource(randomAccessStream);

                    icon.Image = image;

                    icons.Add(icon);


                }
            }
            catch { }
        }

        private static int GetIdFromUrl(string url)
        {
            var map = iconMaps.Where(p => p.Urls.Contains(url)).FirstOrDefault();
            return map == null ? 0 : map.IconId; // The first iconId is 1 so return 0
        }

        public static bool GetFaviconAsBitmapImage(string url, out BitmapImage bitmapImage, bool maxQuality = false)
        {
            int id = GetIdFromUrl(url);
            var iconsToUse = icons.Where(p => p.IconId == id).ToList();

            if (iconsToUse.Count > 0)
            {
                if (maxQuality)
                {
                    Icon maxQualityIcon = iconsToUse.Where(p => p.Quality > 16).FirstOrDefault();
                    bitmapImage = maxQualityIcon.Image;
                }
                else
                {
                    Icon icon = iconsToUse.Where(p => p.Quality == 16).FirstOrDefault();
                    bitmapImage = icon.Image;
                }

                return true;
            }
            else
            {
                bitmapImage = null;
                return false;
            }
        }

    }
}
