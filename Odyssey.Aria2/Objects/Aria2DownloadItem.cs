using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using Odyssey.FWebView.Helpers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Windows.Storage;
using System.Threading.Tasks;

namespace Odyssey.Downloads.Objects
{
    public class DownloadItem : INotifyPropertyChanged
    {
        private string resultPath = string.Empty;
        private float progress = 0;
        private string totalFileSize = "0B";
        private string totalDownloadedDataSize = "0B";
        private string downloadSpeed = "0B";
        private string title = "Downloading...";
        private string description = string.Empty;
        private bool isProgressIntermeinate = true;
        private bool isProgressActive = true;
        private BitmapImage icon = new();

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// The result path of the downloaded file. Returns an empry string if the file is not downloaded
        /// </summary>
        public string ResultPath
        {
            get { return resultPath; }
            set
            {
                if (value != resultPath)
                {
                    resultPath = value;
                    OnPropertyChanged();
                }
            }
        }
        public float Progress
        {
            get { return progress; }
            set
            {
                if (value != progress)
                {
                    progress = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TotalFileSize // 2nd match of the "downloadInfoRegex" regex
        {
            get { return totalFileSize; }
            set
            {
                if (value != totalFileSize)
                {
                    totalFileSize = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TotalDownloadedDataSize // 1st match
        {
            get { return totalDownloadedDataSize; }
            set
            {
                if (value != totalDownloadedDataSize)
                {
                    totalDownloadedDataSize = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DownloadSpeed // 3rd match
        {
            get { return downloadSpeed; }
            set
            {
                if (value != downloadSpeed)
                {
                    downloadSpeed = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                if (value != description)
                {
                    description = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsProgressActive
        {
            get
            {
                return isProgressActive;
            }
            set
            {
                if (value != isProgressActive)
                {
                    isProgressActive = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsProgressIntermeinate
        {
            get
            {
                return isProgressIntermeinate;
            }
            set
            {
                if (value != isProgressIntermeinate)
                {
                    isProgressIntermeinate = value;
                    OnPropertyChanged();
                }

            }
        }
        public BitmapImage Icon
        {
            get
            {
                return icon;
            }
            set
            {
                if(value != icon)
                {
                    icon = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;




        // ************** Downloads Downloads

        public DownloadItem(string url)
        {
            Download(url);
        }
        private async void Download(string url)
        {
            Process process = new();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = Aria2.Aria2cPath,
                ArgumentList = { "-d " + Aria2.DlFolderPath, url },
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            process.StartInfo = startInfo;
            process.Start();




            while (!process.HasExited)
            {
                // Read a line from the StandardOutput stream
                string line = await process.StandardOutput.ReadLineAsync();

                if (line != null)
                {
                    // Process the line as needed
                    UpdateValues(line);
                }


            }

            IsProgressActive = false;
            Description = "Open file";

            try
            {
                await Task.Delay(100);
                StorageFile file = await StorageFile.GetFileFromPathAsync(ResultPath);

                // Get file icon
                var fileIcon = await FileIconHelper.GetFileIcon(file);
                var img = new BitmapImage();
                img.SetSource(fileIcon);

                Icon = img;
            } catch { }
        }
        private async void UpdateValues(string data)
        {

            if (!string.IsNullOrEmpty(data))
            {
                IsProgressActive = true;

                Regex downloadInfoRegex = new("[0-9]{1,4}[a-zA-Z]{0,2}B"); // will match with the download info  (ex: with [#a139c3 4.9MiB/5.0MiB(98%) CN:1 DL:1.0MiB])
                var matches = downloadInfoRegex.Matches(data);

                if (matches.Count == 3) // Test if data match with the regex (will match with 3 part of the string)
                {
                    TotalDownloadedDataSize = matches[0].Value; // ex (in relation with the previous ex): 4.9MiB
                    TotalFileSize = matches[1].Value; // ex: 5.0MiB
                    DownloadSpeed = matches[2].Value; // ex: 1.0MiB (2 to 5 times faster than regular downloads on the network i tested) 

                    Regex percentageRegex = new(@"\d{1,3}%"); // will match only if a percentage is present on the string
                    var percentage = percentageRegex.Match(data);

                    Description = $"{TotalDownloadedDataSize}/{TotalFileSize} Speed: {downloadSpeed}/s";

                    if (percentage.Success)
                    {
                        Progress = float.Parse(percentage.Value.Replace("%", ""));
                        IsProgressIntermeinate = false;
                    }
                    else
                    {
                        IsProgressIntermeinate = true;
                    }
                }
                else
                {
                    Regex pathRegex = new("[A-Z]:/.*[a-zA-Z]"); // will match with the result file path if the the string is a "DownloadItem completed" string
                    var path = pathRegex.Match(data);
                    if (path.Success)
                    {
                        ResultPath = path.Value.Replace("/", @"\");
                        isProgressActive = false;

                        StorageFile file = await StorageFile.GetFileFromPathAsync(ResultPath);
                        Title = file.Name;

                        // Get file icon
                        var fileIcon = await FileIconHelper.GetFileIcon(file);
                        var img = new BitmapImage();
                        img.SetSource(fileIcon);

                        Icon = img;

                    }
                }
            }
        }




        // ************** WebView2 Downloads

        private CoreWebView2DownloadOperation _downloadOperation;
        public DownloadItem(CoreWebView2DownloadOperation downloadOperation)
        {
            _downloadOperation = downloadOperation;
            ResultPath = _downloadOperation.ResultFilePath;

            _downloadOperation.BytesReceivedChanged += _downloadOperation_BytesReceivedChanged;
            _downloadOperation.StateChanged += _downloadOperation_StateChanged;
            _downloadOperation.EstimatedEndTimeChanged += _downloadOperation_EstimatedEndTimeChanged;

            Title = new FileInfo(ResultPath).Name;
        }
        private void _downloadOperation_EstimatedEndTimeChanged(CoreWebView2DownloadOperation sender, object args)
        {
            Description = sender.EstimatedEndTime;
        }
        private async void _downloadOperation_StateChanged(CoreWebView2DownloadOperation sender, object args)
        {
            switch (sender.State)
            {
                case CoreWebView2DownloadState.Completed:
                    IsProgressActive = false;
                    Description = "Open file";

                    StorageFile file = await StorageFile.GetFileFromPathAsync(ResultPath);

                    // Get file icon
                    var fileIcon = await FileIconHelper.GetFileIcon(file);
                    var img = new BitmapImage();
                    img.SetSource(fileIcon);

                    Icon = img;
                    break;

                case CoreWebView2DownloadState.Interrupted: //[TODO] 
                    break;

                case CoreWebView2DownloadState.InProgress:
                    break;



            }
        }
        private void _downloadOperation_BytesReceivedChanged(CoreWebView2DownloadOperation sender, object args)
        {
            try
            {
                long progress = 100 * sender.BytesReceived / sender.TotalBytesToReceive;
                Description = sender.EstimatedEndTime;
                Progress = progress;
            }
            catch { }
        }




        public DownloadItem()
        {
            SetIcon();
            IsProgressActive = false;
        }

        private async void SetIcon()
        {
            while (string.IsNullOrEmpty(ResultPath)) { await Task.Delay(100); } // Need a remplacement

            StorageFile file = await StorageFile.GetFileFromPathAsync(ResultPath);

            try
            {
                // Get file icon
                var fileIcon = await FileIconHelper.GetFileIcon(file);
                var img = new BitmapImage();
                img.SetSource(fileIcon);

                Icon = img;
            }
            catch { }
        }
    }
}
