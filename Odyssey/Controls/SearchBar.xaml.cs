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
using Odyssey.WebSearch.Helpers;
using static Odyssey.WebSearch.Helpers.WebUrlHelpers;
using Odyssey.FWebView.Classes;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls
{
    public sealed partial class SearchBar : Flyout
    {
        public SearchBar()
        {
            this.InitializeComponent();
            
        }

        private void Flyout_Opened(object sender, object e)
        {
            
        }

        private void Flyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
 
        }

        private void mainSearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                string text = (sender as TextBox).Text;
                string url = SearchUrlHelper.ToUrl(text);

                if(url != string.Empty) // The request will be treated differently with commands and app uris
                {
                    if (FWebView.WebView.CurrentlySelected == null)
                    {
                        FWebView.WebView webView = FWebView.WebView.New(url);

                        Tab tab = new()
                        {
                            Title = text,
                            ToolTip = url,
                        };

                        tab.MainWebView = webView;
                        Tabs.Items.Add(tab);
                        PaneView.Current.TabsView.SelectedItem = tab;

                        webView.LinkedTab = tab;

                        MainView.Current.splitViewContentFrame.Content = webView;
                    }
                }
                else
                {
                    StringKind kind = GetStringKind(text);
                    if (kind == StringKind.ExternalAppUri) AppUriLaunch.Launch(new Uri(text));
                }

                Hide();
            }
        }
    }
}
