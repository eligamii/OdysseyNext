using ABI.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

// All creadit comes to the Aria2 creators. See https://github.com/aria2/aria2 and https://aria2.github.io/

namespace Odyssey.Aria2
{
    public static class Aria2
    {
        private static string aria2cPath;
        private static string dlFolderPath;
        public async static void Init()
        {
            aria2cPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Odyssey.Aria2", "Assets", "aria2c.exe");

            // Get the donwload folder path
            StorageFolder dlFolder = await (await (await DownloadsFolder.CreateFolderAsync(Guid.NewGuid().ToString())).GetParentAsync()).GetParentAsync();
            dlFolderPath = dlFolder.Path;

            // Start aria2 to avoid erors
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = aria2cPath,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            Process process = new();

            process.StartInfo = startInfo;
            process.Start();
        }

        public static async void Downlaod(string url)
        {
            Process process = new();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = aria2cPath,
                ArgumentList = { "-d " + dlFolderPath, url },
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            process.OutputDataReceived += Process_OutputDataReceived;

            process.StartInfo = startInfo;
            process.Start();
            process.BeginOutputReadLine();

            await process.WaitForExitAsync();
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(!string.IsNullOrEmpty(e.Data))
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

                    if(percentage.Success)
                    {
                        int pers = int.Parse(percentage.Value.Replace("%", "")) ;
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
