using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView.Controls.Flyouts
{
    public sealed partial class DownloadsFlyout : Flyout
    {
        public static ObservableCollection<Shared.ViewModels.Data.DonwloadItem> Items { get; set; } = new();
        public DownloadsFlyout()
        {
            this.InitializeComponent();
            Items.CollectionChanged += Items_CollectionChanged;

            DownloadItemsListView.ItemsSource = Items;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
             if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                Shared.ViewModels.Data.DonwloadItem item = (Shared.ViewModels.Data.DonwloadItem)e.NewItems[0];
                
                if(item.downloadOperation == null)
                {
                    var downloadFolder = Shared.Helpers.KnownFolders.GetPath(Shared.Helpers.KnownFolder.Downloads);
                    var download = AriaSharp.Downloader.Download(item.DownloadUrl, downloadFolder);

                    download.DownloadProgressChanged += (s, a) =>
                    {
                        if(a.Status == AriaSharp.DownloadStatus.Downloading)
                        {
                            item.Name = s.OutputPath;
                            item.Progress = a.Progress;

                        }
                    };
                }
            }
        }

        private void DownloadItemsListView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            
        }
    }
}
