using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Controls;
using Odyssey.FWebView;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Data.Main;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Views.Pages
{
    // This home page will be shown only on startup by purpose
    // This is also a way to load a CoreWebView2 Environnement without the user noticing the lag caused by its creation

    public sealed partial class HomePage : Page
    {
        WebView webView;
        public HomePage()
        {
            this.InitializeComponent();
<<<<<<< Updated upstream
            Loaded += (s, a) => searchBox.Focus(FocusState.Programmatic);
        }

=======
            Loaded += (s, a) =>  searchBox.Focus(FocusState.Programmatic);
            lastSessionListView.Loaded += LastSessionListView_Loaded;

            Init();
        }

        private async void Init()
        {
            webView = await WebView.Create("about:blank");
            string s = "t";
        }
    

        private void LastSessionListView_Loaded(object sender, RoutedEventArgs e)
        {
            lastSessionListView.ItemsSource = Tabs.Get();
            foreach(Tab tab in lastSessionListView.Items)
            {
                try
                {
                    tab.ImageSource = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage { UriSource = new Uri($"https://muddy-jade-bear.faviconkit.com/{new System.Uri(tab.Url).Host}/21") };

                } catch { }

            }
        }
>>>>>>> Stashed changes

        private void TextBox_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            args.TryCancel();
        }

        private async void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                string text = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToUrl(searchBox.Text);
                webView.WebView2Runtime.CoreWebView2.Navigate(text);

                Tab tab = new()
                {
                    Url = text,
                    ToolTip = text,
                    MainWebView = webView,
                    Title = text
                };

                webView.LinkedTab = tab;
                Tabs.Items.Add(tab);

                PaneView.Current.TabsView.SelectedItem = tab;

            }
        }
<<<<<<< Updated upstream
=======

        private void lastSessionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Tab clickedItem = e.ClickedItem as Tab;
            webView.WebView2Runtime.CoreWebView2.Navigate(clickedItem.Url);
            Tab tab = new()
            {
                Url = clickedItem.Url,
                Title = clickedItem.Title,
                ToolTip = clickedItem.Url
            };

            tab.MainWebView = webView;

            webView.LinkedTab = tab;

            Tabs.Items.Add(tab);

            PaneView.Current.TabsView.SelectedItem = tab;
        }
>>>>>>> Stashed changes
    }
}
