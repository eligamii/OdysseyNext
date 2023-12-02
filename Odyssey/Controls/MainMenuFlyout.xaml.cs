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
using Odyssey.Views;
using System.Web;
using System.Net;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls
{
    public sealed partial class MainMenuFlyout : Flyout
    {
        public MainMenuFlyout()
        {
            this.InitializeComponent();
            Opening += MainMenuFlyout_Opening;
        }

        private void MainMenuFlyout_Opening(object sender, object e)
        {
            if(MainView.CurrentlySelectedWebView != null)
            {
                urlTextBox.Text = MainView.CurrentlySelectedWebView.Source.ToString();
            }
        }

        private async void urlTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                var kind = await WebSearch.Helpers.WebSearchStringKindHelpers.GetStringKindAsync(urlTextBox.Text);
                if(kind == WebSearch.Helpers.WebSearchStringKindHelpers.StringKind.Url)
                {
                    string finalUrl = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToUrl(urlTextBox.Text);
                    MainView.CurrentlySelectedWebView.CoreWebView2.Navigate(finalUrl);
                }
            }
        }

        private void urlTextBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void urlTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
