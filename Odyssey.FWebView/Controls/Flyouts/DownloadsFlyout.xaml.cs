using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView.Controls.Flyouts
{
    public sealed partial class DownloadsFlyout : Flyout
    {
        public DownloadsFlyout()
        {
            this.InitializeComponent();
            Downloads.Data.Downloads.Load();
        }

        private void DownloadItemsListView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DownloadItemsListView.ItemsSource = Downloads.Data.Downloads.Items;
        }
    }
}
