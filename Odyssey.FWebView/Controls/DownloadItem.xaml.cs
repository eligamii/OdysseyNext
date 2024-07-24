using Downloader;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Odyssey.Shared.Helpers;
using System.Diagnostics;
using System.IO;




namespace Odyssey.FWebView.Controls
{
    public sealed partial class DownloadItem : ListViewItem
    {
        public DownloadItem(string url)
        {
            this.InitializeComponent();

            var downloadOpt = new DownloadConfiguration()
            {
                ChunkCount = 8,
            };

            var downloader = new DownloadService(downloadOpt);

            var downloadFolder = KnownFolders.GetPath(KnownFolder.Downloads);
            DirectoryInfo folderInfo = new(downloadFolder);

            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;

            downloader.DownloadFileTaskAsync(url, folderInfo);
        }

        private void Downloader_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressRing.Value = e.ProgressPercentage;
        }

        /*
        private async void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                downloadData.Title = "Downloading...";
            });
            DownloadItem progress = DownloadItem.ToAria2DownloadProgress(e.Data);
            
            if(progress.ResultPath == string.Empty)
            {
                downloadData.IsProgressActive = true;
                if (progress.Progress == 0) downloadData.IsProgressIntermeinate = true;

                downloadData.ProgressValue = progress.Progress * 100;

                downloadData.Title = "Downloading...";
                downloadData.Subtitle = $"{progress.TotalDownloadedDataSize}/{progress.TotalFileSize} - {progress.DownloadSpeed}/s";
            }
            else
            {
                FileInfo fileInfo = new(progress.ResultPath);

                downloadData.IsProgressActive = false;
                downloadData.Title = fileInfo.Name;
                downloadData.Subtitle = fileInfo.FullName;
            }
        }
        */

        private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

        }

        private void DeleteMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowInFolderMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
