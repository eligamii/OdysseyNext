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
    // This is also a way to load a CoreWebView2 Environnement without the user noticing the lag
    public sealed partial class HomePage : Page
    {
        WebView webView = WebView.Create("about:blank");
        public HomePage()
        {
            this.InitializeComponent();
            Loaded += (s, a) => searchBox.Focus(FocusState.Programmatic);
        }


        private void TextBox_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            args.TryCancel();
        }

        private async void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                string text = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToUrl(searchBox.Text);
                webView.CoreWebView2.Navigate(text);

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
    }
}
