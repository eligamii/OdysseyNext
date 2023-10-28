using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Data.Main;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView.Controls.Flyouts
{
    public sealed partial class HistoryFlyout : Flyout
    {
        public HistoryFlyout()
        {
            this.InitializeComponent();
        }

        private void historyItemsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            HistoryItem item = e.ClickedItem as HistoryItem;
            if(WebView.SelectedWebView != null)
            {
                WebView.SelectedWebView.CoreWebView2.Navigate(item.Url);
            }
            else
            {
                WebView webView = WebView.Create(item.Url);
                Tab tab = new()
                {
                    MainWebView = webView,
                    Title = item.Title,
                    ToolTip = item.Url,
                };

                Tabs.Items.Add(tab);
            }
        }

        private void historyItemsListView_Loaded(object sender, RoutedEventArgs e)
        {
            historyItemsListView.ItemsSource = History.Items;
        }
    }
}
