using Downloader;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Odyssey.Shared.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Windows.Storage;




namespace Odyssey.FWebView.Controls.Flyouts
{
    public sealed partial class DownloadsFlyout : Flyout
    {
        public static ObservableCollection<Shared.ViewModels.Data.DownloadItem> Items { get; set; } = new();
        
        public DownloadsFlyout()
        {
            this.InitializeComponent();
            Items.CollectionChanged += Items_CollectionChanged;

            DownloadItemsListView.ItemsSource = Items;


            
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var item = e.NewItems[0] as Shared.ViewModels.Data.DownloadItem;

                if (item.DownloadUrl != null) HandleDownloadEventsForItem(item);

            }
        }

        private void HandleDownloadEventsForItem(Shared.ViewModels.Data.DownloadItem item)
        {
            DownloadConfiguration downloadOpt = new()
            {
                ChunkCount = 8,
            };

            DownloadService downloadService = new(downloadOpt);


            var downloadFolder = Shared.Helpers.KnownFolders.GetPath(KnownFolder.Downloads);
            DirectoryInfo folderInfo = new(downloadFolder);

            downloadService.DownloadStarted += (s, a) => 
            {
                DispatcherQueue.TryEnqueue(() => item.Name = new FileInfo(a.FileName).Name); 
            };

            downloadService.DownloadProgressChanged += (s, a) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    item.Progress = (long)a.ProgressPercentage;

                    string speed = UnitsRepresentationHelper.ToString(a.AverageBytesPerSecondSpeed) + "/s";
                    string totalDownloadedBytes = UnitsRepresentationHelper.ToString(a.ReceivedBytesSize) + " / " + UnitsRepresentationHelper.ToString(a.TotalBytesToReceive);
                    item.Subtitle = speed + " - " + totalDownloadedBytes;
                    item.downloadOperation = downloadService.Package;
                });

            };

            downloadService.DownloadFileCompleted += (s, a) =>
            {
                DispatcherQueue.TryEnqueue(async () =>
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(downloadService.Package.FileName);
                    BitmapImage image = new();
                    image.SetSource(await FileIconHelper.GetFileIconAsync(file));

                    item.OutputFile = file.Path;
                    item.DownloadInProgress = false;
                    item.ImageSource = image;
                    item.Name = file.Name; // Set the name again in case the DownloadProgressEvent was not raised
                    item.Subtitle = UnitsRepresentationHelper.ToString(downloadService.Package.TotalFileSize) + " - " + ResourceString.GetString("From", "Shared") + " " + new Uri(item.DownloadUrl).Host;
                });
            };


            downloadService.DownloadFileTaskAsync(item.DownloadUrl, folderInfo);
        }

        private void DownloadItemsListView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void DownloadItemsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Shared.ViewModels.Data.DownloadItem item = e.ClickedItem as Shared.ViewModels.Data.DownloadItem; 

            // Use this instead of a launcher to launch any file (executables or non-executables)
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C start {item.OutputFile}";
            process.StartInfo = startInfo;
            process.Start();
        }

        private void ListViewItem_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            Grid grid = item.Content as Grid;
            grid.ColumnDefinitions[2].Width = new Microsoft.UI.Xaml.GridLength(40);
        }

        private void ListViewItem_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            Grid grid = item.Content as Grid;
            grid.ColumnDefinitions[2].Width = new Microsoft.UI.Xaml.GridLength(0);
        }
    }
}
