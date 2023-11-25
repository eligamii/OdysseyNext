using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Windows.Storage;

// All creadit comes to the Downloads creators. See https://github.com/aria2/aria2 and https://aria2.github.io/

namespace Odyssey.Downloads
{
    public static class Aria2
    {
        public static string Aria2cPath { get; set; }
        public static string DlFolderPath { get; set; }
        public async static void Init()
        {
            Aria2cPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Odyssey.Downloads", "Assets", "aria2c.exe");

            // Get the donwload folder path
            StorageFolder dlFolder = await (await (await DownloadsFolder.CreateFolderAsync(Guid.NewGuid().ToString())).GetParentAsync()).GetParentAsync();
            DlFolderPath = dlFolder.Path;

            // Start aria2 to avoid erors
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = Aria2cPath,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            Process process = new();

            process.StartInfo = startInfo;
            process.Start();
        }

        public static Process Downlaod(string url)
        {
            Process process = new();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = Aria2cPath,
                ArgumentList = { "-d " + DlFolderPath, url },
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };


            process.StartInfo = startInfo;
            process.Start();
            process.BeginOutputReadLine();

            return process;
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Regex regex = new("[0-9]{1,4}[a-zA-Z]{0,2}B");
                var matches = regex.Matches(e.Data);

                if (matches.Count == 3)
                {
                    foreach (Match match in matches)
                    {
                        string s = match.Value;
                    }

                    Regex percentageRegex = new(@"\d{1,3}%");
                    var percentage = percentageRegex.Match(e.Data);

                    if (percentage.Success)
                    {
                        int pers = int.Parse(percentage.Value.Replace("%", ""));
                    }
                }
            }
        }

        private static void Process_Exited(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
