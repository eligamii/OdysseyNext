using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
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
                
            }
        }

        private void DownloadItemsListView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
