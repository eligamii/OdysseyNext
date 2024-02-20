using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using Odyssey.Shared.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Windows.Storage;

namespace Odyssey.Shared.ViewModels.Data
{
    [DataContract]
    public class DownloadItem : INotifyPropertyChanged
    {
        private string name;
        private string subtitle;
        private BitmapImage image = new();
        private bool downloadInProgress = true;
        public object downloadOperation;
        private long progress = 0;

        public DownloadItem(string url) => DownloadUrl = url;


        private void Operation_EstimatedEndTimeChanged(CoreWebView2DownloadOperation sender, object args)
        {
            Subtitle = sender.EstimatedEndTime;
        }

        private async void Operation_StateChanged(CoreWebView2DownloadOperation sender, object args)
        {
            if (sender.State == CoreWebView2DownloadState.Completed)
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(sender.ResultFilePath);
                var icon = await FileIconHelper.GetFileIconAsync(file);

                ImageSource.SetSource(icon);
            }
        }

        private void Operation_BytesReceivedChanged(CoreWebView2DownloadOperation sender, object args)
        {
            FileInfo file = new(sender.ResultFilePath);
            OutputFile = file.FullName;
            Name = file.Name;
            DownloadUrl = sender.Uri;
            Progress = sender.BytesReceived / sender.TotalBytesToReceive * 100;
        }



        [DataMember]
        public string OutputFile
        {
            get; set;
        }
        [DataMember]
        public string DownloadUrl
        {
            get; set;
        }
        [DataMember]
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [DataMember]
        public long Progress
        {
            get { return progress; }
            set
            {
                if (value != progress)
                {
                    progress = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [DataMember]
        public bool DownloadInProgress
        {
            get { return downloadInProgress; }
            set
            {
                if (value != downloadInProgress)
                {
                    downloadInProgress = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public BitmapImage ImageSource
        {
            get { return image; }
            set
            {
                if (value != image)
                {
                    image = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Subtitle
        {
            get { return subtitle; }
            set
            {
                if (value != subtitle)
                {
                    subtitle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
