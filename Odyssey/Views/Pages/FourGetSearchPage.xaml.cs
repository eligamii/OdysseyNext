using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Data.Main;
using Odyssey.FWebView;
using Odyssey.LibreYAPI.Objects;
using Odyssey.Shared.ViewModels.Data;
using System.Collections.Generic;
using System.Linq;




namespace Odyssey.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FourGetSearchPage : Page
    {
        public FourGetSearchPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            string param = e.Parameter as string;

            var res = await LibreYAPI.LibreYAPI.SearchAsync(param, 0);
            List<SearchResult> results = res.Where(p => p.Value.title != null).Select(p => p.Value).ToList();

            list.ItemsSource = results;
            searchBox.Visibility = Visibility.Visible;

            base.OnNavigatedTo(e);
        }

        private async void searchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var res = await LibreYAPI.LibreYAPI.SearchAsync(searchBox.Text, 0);
                List<SearchResult> results = res.Where(p => p.Value.title != null).Select(p => p.Value).ToList();

                list.ItemsSource = results;
            }
        }

        private void list_ItemClick(object sender, ItemClickEventArgs e)
        {
            SearchResult clickedItem = e.ClickedItem as SearchResult;
            WebView webView = WebView.Create(clickedItem.url);

            Tab tab = new()
            {
                Url = clickedItem.url,
                Title = clickedItem.title,
                ToolTip = clickedItem.url
            };

            tab.MainWebView = webView;

            webView.LinkedTab = tab;

            Tabs.Items.Add(tab);

            PaneView.Current.TabsView.SelectedItem = tab;
        }
    }
}
