﻿using System.IO;

namespace Odyssey.FWebView.Helpers
{
    internal class DownloadHelper
    {
        private static string aria2cPath = string.Empty;
        internal static void Download(string url)
        {
            if (aria2cPath == string.Empty)
            {
                string aria2cPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Odyssey.FWebView", "Binary", "aria2c.exe");

            }
        }
    }
}
