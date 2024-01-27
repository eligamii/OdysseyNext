using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics;




namespace Odyssey.FWebView.Controls
{
    public sealed partial class DownloadItem : ListViewItem
    {
        public DownloadItem(Process process)
        {
            this.InitializeComponent();
            //process.OutputDataReceived += Process_OutputDataReceived; // Get the download info

            //this.DataContext = downloadData;
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
