using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Odyssey.Aria2.Objects
{
    public class Aria2DownloadProgress
    {
        /// <summary>
        /// The result path of the downloaded file. Returns an empry string if the file is not downloaded
        /// </summary>
        public string ResultPath { get; set; } = string.Empty;
        public float Progress { get; set; } = 0;
        public string TotalFileSize { get; set; } = "0B"; // 2nd match of the "downloadInfoRegex" regex
        public string TotalDownloadedDataSize { get; set; } = "0B"; // 1st match
        public string DownloadSpeed { get; set; } = "0B"; // 3rd match

        public static Aria2DownloadProgress ToAria2DownloadProgress(string str)
        {
            Aria2DownloadProgress prog = new();

            if (!string.IsNullOrEmpty(str))
            {
                Regex downloadInfoRegex = new("[0-9]{1,4}[a-zA-Z]{0,2}B"); // will match with the download info  (ex: with [#a139c3 4.9MiB/5.0MiB(98%) CN:1 DL:1.0MiB])
                var matches = downloadInfoRegex.Matches(str);

                if (matches.Count == 3) // Test if str match with the regex (will match with 3 part of the string)
                {
                    prog.TotalDownloadedDataSize = matches[0].Value; // ex (in relation with the previous ex): 4.9MiB
                    prog.TotalFileSize = matches[1].Value; // ex: 5.0MiB
                    prog.DownloadSpeed = matches[2].Value; // ex: 1.0MiB

                    Regex percentageRegex = new(@"\d{1,3}%"); // will match only if a percentage is present on the string
                    var percentage = percentageRegex.Match(str);

                    if (percentage.Success)
                    {
                        prog.Progress = float.Parse(percentage.Value.Replace("%", "")) / 100f;
                    }
                }
                else
                {
                    Regex pathRegex = new("[A-Z]:/.*"); // will match with the result file path if the the string is a "Download completed" string
                    var path = pathRegex.Match(str);
                    if (path.Success)
                    {
                        prog.ResultPath = path.Value; 
                    }
                }
            }

            return prog;
        }

    }
}
