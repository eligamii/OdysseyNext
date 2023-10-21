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
using Odyssey.Shared.DataTemplates.Data;
using Odyssey.Data.Main;
using Odyssey.Views;
using Microsoft.Web.WebView2.Core;
using Odyssey.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls
{
    public sealed partial class SearchBar : Flyout
    {
        private FWebView webView;
        private bool webViewUsed = false;
        public SearchBar()
        {
            this.InitializeComponent();
            if(FWebView.CurrentlySelected == null) webView = FWebView.New();
        }

        private void Flyout_Opened(object sender, object e)
        {
            
        }

        private void Flyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            if(!webViewUsed)
            {
                webView.Close();
                webView = null;
            }
        }

        private void mainSearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                string text = (sender as TextBox).Text;
                string url = SearchUrlHelper.ToUrl(text);

                if(url != string.Empty) // The request will be treated differently with commands and app uris
                {
                    if (FWebView.CurrentlySelected == null)
                    {
                        Tab tab = new()
                        {
                            Title = "Test",
                            ToolTip = "Testing is something cool",
                        };

                        webView.Source = new Uri(url);

                        tab.MainWebView = webView;
                        Tabs.TabsList.Add(tab);
                        PaneView.Current.TabsView.SelectedItem = tab;

                        MainView.Current.splitViewContentFrame.Content = webView;
                        webViewUsed = true;
                    }
                }

                Hide();
            }
        }
    }
}
