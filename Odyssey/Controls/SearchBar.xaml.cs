using ABI.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Odyssey.Data.Main;
using Odyssey.FWebView.Classes;
using Odyssey.Helpers;
using Odyssey.Shared.DataTemplates.Data;
using Odyssey.Views;
using Odyssey.WebSearch.Helpers;
using System;
using static Odyssey.WebSearch.Helpers.WebUrlHelpers;
using Uri = System.Uri;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls
{
    public sealed partial class SearchBar : Flyout
    {
        private string mainIcon = string.Empty;
        private bool newTab;

        public SearchBar(bool newTab = false)
        {
            this.InitializeComponent();
            this.newTab = newTab;

            mainIcon = newTab ? "\uF7ED" : "\uE11A";
            searchBarIcon.Glyph = mainIcon;
        }

        private void Flyout_Opened(object sender, object e)
        {
            
        }

        private void Flyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
 
        }

        private async void mainSearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                string text = (sender as TextBox).Text;
                string url = await SearchUrlHelper.ToUrl(text);

                if(url != string.Empty) // The request will be treated differently with commands and app uris
                {
                    if (MainView.CurrentlySelectedWebView == null || newTab)
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
                    else
                    {
                        MainView.CurrentlySelectedWebView.CoreWebView2.Navigate(url);
                    }
                }
                else
                {
                    StringKind kind = await GetStringKind(text);
                    if (kind == StringKind.ExternalAppUri) AppUriLaunch.Launch(new Uri(text));
                }

                Hide();
            }
        }

        private void IconRectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainSearchBox.Focus(FocusState.Programmatic);
        }

        private void mainSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateIcon(mainSearchBox.Text);
        }


        private async void UpdateIcon(string text)
        {
            StringKind kind = await GetStringKind(text);

            switch (kind)
            {
                case StringKind.SearchKeywords: searchBarIcon.Glyph = mainIcon; break;

                case StringKind.Url: searchBarIcon.Glyph = "\uE128"; break;

                case StringKind.MathematicalExpression: searchBarIcon.Glyph = "\uE1D0"; break;

                case StringKind.ExternalAppUri: searchBarIcon.Glyph = "\uECAA"; break;

                case StringKind.QuickActionCommand: searchBarIcon.Glyph = "\uE756"; break;

                case StringKind.OdysseyUrl: searchBarIcon.Glyph = "\uE115"; break;
            }
             
        }
    }
}
