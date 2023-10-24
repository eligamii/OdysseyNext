using ABI.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Odyssey.Data.Main;
using Odyssey.FWebView.Classes;
using Odyssey.Helpers;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Shared.ViewModels.WebSearch;
using Odyssey.Views;
using Odyssey.WebSearch;
using Odyssey.WebSearch.Helpers;
using System;
using System.Linq;
using static Odyssey.WebSearch.Helpers.WebSearchStringKindHelpers;
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
            this.newTab = newTab || MainView.CurrentlySelectedWebView == null;

            mainIcon = this.newTab ? "\uEC6C" : "\uE11A";
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
                string url = await WebViewNavigateUrlHelper.ToUrl(text);

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
            else if (suggestionListView.Items.Count > 0)
            {
                bool navigated = false;

                if (e.Key == Windows.System.VirtualKey.Up)
                {
                    if (suggestionListView.SelectedIndex > 0)
                        suggestionListView.SelectedIndex--;

                    navigated = true;
                }
                else if (e.Key == Windows.System.VirtualKey.Down)
                {

                    if (suggestionListView.SelectedIndex == -1)
                        suggestionListView.SelectedIndex = 0;
                    else if (suggestionListView.SelectedIndex != suggestionListView.Items.Count - 1)
                        suggestionListView.SelectedIndex++;

                    navigated = true;
                }
                else if (e.Key == Windows.System.VirtualKey.Tab)
                {
                    if (suggestionListView.SelectedIndex == -1)
                        suggestionListView.SelectedIndex = 0;
                    else if (suggestionListView.SelectedIndex != suggestionListView.Items.Count - 1)
                        suggestionListView.SelectedIndex++;
                    else
                        suggestionListView.SelectedIndex = 0;

                    navigated = true;
                }

                if (suggestionListView.SelectedIndex != -1 && navigated)
                {
                    var item = suggestionListView.SelectedItem as Suggestion;

                    if (!string.IsNullOrEmpty(item.Url))
                    {
                        mainSearchBox.Text = item.Url;
                    }
                    else
                    {
                        mainSearchBox.Text = item.Title;
                    }

                    mainSearchBox.SelectionStart = mainSearchBox.Text.Length;
                    mainSearchBox.Focus(FocusState.Programmatic);
                }
                else
                {
                    suggestionListView.SelectedIndex = -1;
                }
            }
        }

        private void IconRectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainSearchBox.Focus(FocusState.Programmatic);
        }

        private async void mainSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateIcon(mainSearchBox.Text);
            if(mainSearchBox.Text != string.Empty)
            {
                try
                {
                    var list = await WebSearch.Suggestions.Suggest(mainSearchBox.Text, 8);

                    Suggestion defaultSuggestion = new() { Kind = SuggestionKind.Search, Title = mainSearchBox.Text, Url = await WebViewNavigateUrlHelper.ToUrl(mainSearchBox.Text) };
                    list.Add(defaultSuggestion);

                    suggestionListView.ItemsSource = list;

                    suggestionListView.Visibility = list.Where(p => p != defaultSuggestion).ToList().Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                }
                catch { }
            }
            else
            {
                suggestionListView.Visibility = Visibility.Collapsed;
            }
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

        private void suggestionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Suggestion suggestion = e.ClickedItem as Suggestion;

            switch(suggestion.Kind)
            {
                case SuggestionKind.Search or SuggestionKind.MathematicalExpression or SuggestionKind.History or SuggestionKind.Url:
                    MainView.CurrentlySelectedWebView.CoreWebView2.Navigate(suggestion.Url); break;

                case SuggestionKind.Tab:
                    var keyVauePairs = suggestion.Tab;
                    switch(keyVauePairs.Key)
                    {
                        case TabLocation.Favorites: PaneView.Current.FavoriteGrid.SelectedItem = keyVauePairs.Value; break;

                        case TabLocation.Tabs: PaneView.Current.TabsView.SelectedItem = keyVauePairs.Value; break;

                        case TabLocation.Pins: PaneView.Current.PinsTabView.SelectedItem = keyVauePairs.Value; break;
                    }
                    break;

            }

            Hide();
        }
    }
}
