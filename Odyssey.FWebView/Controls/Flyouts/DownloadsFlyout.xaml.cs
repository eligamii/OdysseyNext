using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
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
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                Shared.ViewModels.Data.DonwloadItem item = (Shared.ViewModels.Data.DonwloadItem)e.NewItems[0];

                if (item.downloadOperation == null)
                {
                    var downloadFolder = Shared.Helpers.KnownFolders.GetPath(Shared.Helpers.KnownFolder.Downloads);
                    var download = AriaSharp.Downloader.Download(item.DownloadUrl, downloadFolder);

                    download.DownloadProgressChanged += (s, a) =>
                    {
                        if (!string.IsNullOrWhiteSpace(s.Filename))
                        {
                            item.Name = s.Filename;
                        }

                        item.Progress = a.Progress;

                        if (a.Status == AriaSharp.AriaDownloadOperation.Status.Completed)
                        {
                            try
                            {
                                /*
                                var file = await StorageFile.GetFileFromPathAsync(s.OutputPath.FullName);
                                var icon = await Shared.Helpers.FileIconHelper.GetFileIconAsync(file);
                                BitmapImage bitmap = new();
                                await bitmap.SetSourceAsync(icon);
                                item.ImageSource = bitmap;*/
                            }
                            catch { }
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
