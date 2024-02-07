using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Odyssey.Data.Main;
using Odyssey.Data.Settings;
using Odyssey.FWebView;
using Odyssey.FWebView.Classes;
using Odyssey.QuickActions;
using Odyssey.QuickActions.Objects;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Shared.ViewModels.WebSearch;
using Odyssey.Views;
using Odyssey.WebSearch;
using Odyssey.WebSearch.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using static Odyssey.WebSearch.Helpers.WebSearchStringKindHelpers;
using Uri = System.Uri;




namespace Odyssey.Controls
{
    public sealed partial class SearchBar : Flyout
    {
        private string mainIcon = string.Empty;
        private bool newTab;
        private bool startNewTab;
        private string command;

        private static FontFamily cascadiaCode; 

        public SearchBar(bool newTab = false)
        {
            this.InitializeComponent();
            this.newTab = startNewTab = newTab || MainView.CurrentlySelectedWebView == null;

            mainIcon = this.newTab ? "\uEC6C" : "\uE11A";
            searchBarIcon.Glyph = mainIcon;

            Opened += SearchBar_Opened;
        }

        // For the <ask> QACommand variable
        public async Task<string> GetText()
        {
            FlyoutShowOptions options = new FlyoutShowOptions();
            options.Placement = FlyoutPlacementMode.Bottom;
            options.Position = new Point(MainView.Current.splitViewContentFrame.ActualWidth / 2, 100);

            this.ShowAt(MainView.Current.splitViewContentFrame, options);

            searchBarIcon.Glyph = "\uE11B";
            mainSearchBox.TextChanged -= mainSearchBox_TextChanged;
            mainSearchBox.KeyDown -= mainSearchBox_KeyDown;

            suggestionListView.Visibility = Visibility.Collapsed;
            bool keydown = false;

            mainSearchBox.KeyDown += (s, a) =>
            {
                if (a.Key == Windows.System.VirtualKey.Enter)
                {
                    keydown = true;
                }
            };

            // Wait the user to enter [Enter]
            while (!keydown) await Task.Delay(100);

            return mainSearchBox.Text;
        }

        public void SummitSuggestion(Suggestion suggestion)
        {
            switch (suggestion.Kind)
            {
                case SuggestionKind.Shortcut or SuggestionKind.Search or SuggestionKind.MathematicalExpression or SuggestionKind.History or SuggestionKind.Url:
                    if (MainView.CurrentlySelectedWebView != null)
                    {
                        MainView.CurrentlySelectedWebView.CoreWebView2.Navigate(suggestion.Url);
                    }
                    else
                    {
                        FWebView.WebView webView = FWebView.WebView.Create(suggestion.Url);

                        Tab tab = new()
                        {
                            Title = suggestion.Title,
                            ToolTip = suggestion.Url,
                        };

                        tab.MainWebView = webView;
                        Tabs.Items.Add(tab);
                        PaneView.Current.TabsView.SelectedItem = tab;

                        webView.LinkedTab = tab;

                        MainView.Current.splitViewContentFrame.Content = webView;
                    }
                    break;

                case SuggestionKind.Tab:
                    var keyVauePairs = suggestion.Tab;
                    switch (keyVauePairs.Key)
                    {
                        case TabLocation.Favorites: PaneView.Current.FavoriteGrid.SelectedItem = keyVauePairs.Value; break;

                        case TabLocation.Tabs: PaneView.Current.TabsView.SelectedItem = keyVauePairs.Value; break;

                        case TabLocation.Pins: PaneView.Current.PinsTabView.SelectedItem = keyVauePairs.Value; break;
                    }
                    break;

            }
        }

        private void SearchBar_Opened(object sender, object e)
        {
            if (SearchBarShortcuts.Items.Count > 0)
            {
                suggestionListView.Visibility = Visibility.Visible;
                suggestionListView.ItemsSource = SearchBarShortcuts.Items;
            }
        }

        private void Flyout_Opened(object sender, object e)
        {

        }

        private void Flyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {

        }

        bool suggestionChosen = false;


        bool isImFeelingLuckyEnabled = false;
        private async void mainSearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Shift)
                newTab = true;

            if (e.Key == Windows.System.VirtualKey.Control)
                isImFeelingLuckyEnabled = true;

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (suggestionListView.SelectedItem == null)
                {
                    string text, url;

                    if(isImFeelingLuckyEnabled)
                    {
                        var res = await LibreYAPI.LibreYAPI.SearchAsync(mainSearchBox.Text, 0);
                        text = url = res.Where(p => p.Value.url != null).Select(p => p.Value).ElementAt(0).url;
                    }
                    else
                    {
                        text = (sender as TextBox).Text;
                        url = await WebViewNavigateUrlHelper.ToWebView2Url(text);
                    }

                    
                    bool ask = false;

                    if (url != string.Empty) // The request will be treated differently with commands and app uris
                    {
                        if (MainView.CurrentlySelectedWebView == null || newTab)
                        {
                            FWebView.WebView webView = FWebView.WebView.Create(url);

                            Tab tab = new()
                            {
                                Title = text,
                                ToolTip = url,
                            };

                            tab.MainWebView = webView;
                            Tabs.Items.Add(tab);
                            try
                            {
                                await Task.Delay(100);
                                PaneView.Current.TabsView.SelectedItem = tab;
                            }
                            catch { }

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
                        StringKind kind = await GetStringKindAsync(text);
                        if (kind == StringKind.ExternalAppUri) AppUriLaunch.Launch(new Uri(text));
                        else if (kind == StringKind.InternalUrl)
                        {
                            // Redirect edge://downloads and edge://history to the Odyssey flyouts
                            if (Regex.IsMatch(text, ".*/downloads/{0,1}.*", RegexOptions.IgnoreCase))
                            {
                                WebView.OpenDownloadDialog();
                            }
                            else if (Regex.IsMatch(text, ".*/history/{0,1}.*", RegexOptions.IgnoreCase))
                            {
                                WebView.OpenHistoryDialog();
                            }
                        }
                        else if (kind == StringKind.QuickActionCommand)
                        {
                            if (text.Contains("<ask>"))
                            {
                                ask = true;

                                mainSearchBox.Text = string.Empty;
                                command = text;

                                searchBarIcon.Glyph = "\uE11B";
                                mainSearchBox.TextChanged -= mainSearchBox_TextChanged;
                                mainSearchBox.KeyDown -= mainSearchBox_KeyDown;
                                mainSearchBox.PlaceholderText = string.Empty;

                                suggestionListView.Visibility = Visibility.Collapsed;

                                mainSearchBox.KeyDown += async (s, a) =>
                                {
                                    if (a.Key == Windows.System.VirtualKey.Enter)
                                    {
                                        Variables.AskText = mainSearchBox.Text;
                                        Res res = await QACommands.Execute(command);

                                        if (!res.Success && Settings.DisplayQACommandErrors) await QACommands.Execute($"toast title:\"Exception\" content:\"{res.Message}\"");

                                        Hide();
                                    }
                                };
                            }
                            else
                            {
                                Res res = await QACommands.Execute(text);

                                if (!res.Success && Settings.DisplayQACommandErrors) await QACommands.Execute($"toast title:\"Exception\" content:\"{res.Message}\"");
                            }
                        }
                    }

                    if (!ask) Hide();
                }
                else
                {
                    Suggestion suggestion = suggestionListView.SelectedItem as Suggestion;

                    SummitSuggestion(suggestion);

                    Hide();
                }
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
                    suggestionChosen = true;
                    var item = suggestionListView.SelectedItem as Suggestion;

                    mainSearchBox.Text = item.Title;

                    mainSearchBox.SelectionStart = mainSearchBox.Text.Length;
                }
                else
                {
                    suggestionListView.SelectedIndex = -1;
                }
            }
        }

        private async void mainSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Suggestions.CurrentQuery = mainSearchBox.Text;
            UpdateIcon(mainSearchBox.Text);
            if (!suggestionChosen)
            {
                if (mainSearchBox.Text != string.Empty)
                {
                    try
                    {
                        var list = new List<Suggestion>();
                        Suggestion defaultSuggestion = new() { Kind = SuggestionKind.Search, Title = mainSearchBox.Text, Url = await WebViewNavigateUrlHelper.ToWebView2Url(mainSearchBox.Text) };
                        list.Add(defaultSuggestion);

                        var package = Clipboard.GetContent();
                        if (package.Contains(StandardDataFormats.Text))
                        {
                            var clipboardText = await package.GetTextAsync();
                            if (GetStringKind(clipboardText) == StringKind.Url)
                            {
                                Suggestion suggestion = new() { Kind = SuggestionKind.Url, Title = clipboardText, Url = await WebViewNavigateUrlHelper.ToWebView2Url(clipboardText) };
                                list.Add(suggestion);
                            }
                        }


                        list = list.Concat(await Suggestions.Suggest(mainSearchBox.Text, 8)).ToList();

                        // prevent the list from being empty when the suggestions come after the searchbox text was deleted
                        if (list.Where(p => p != defaultSuggestion).ToList().Count == 0 && mainSearchBox.Text == string.Empty)
                        {
                            list = SearchBarShortcuts.Items.ToList();
                        }

                        suggestionListView.ItemsSource = list;

                        suggestionListView.Visibility = list.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                    }
                    catch { }
                }
                else
                {
                    if (SearchBarShortcuts.Items.Count == 0)
                    {
                        suggestionListView.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        suggestionListView.ItemsSource = SearchBarShortcuts.Items;
                        suggestionListView.Visibility = Visibility.Visible;
                    }
                }
            }

            suggestionChosen = false;
        }

        private async void UpdateIcon(string text)
        {
            StringKind kind = await GetStringKindAsync(text);
            bool enableCode = false;

            switch (kind)
            {
                case StringKind.SearchKeywords: searchBarIcon.Glyph = mainIcon; break;

                case StringKind.Url: searchBarIcon.Glyph = "\uE128"; break;

                case StringKind.MathematicalExpression: searchBarIcon.Glyph = "\uE1D0"; break;

                case StringKind.ExternalAppUri: searchBarIcon.Glyph = "\uECAA"; break;

                case StringKind.QuickActionCommand: searchBarIcon.Glyph = "\uE756"; enableCode = true; break;

                case StringKind.InternalUrl: searchBarIcon.Glyph = "\uE115"; break;
            }

            EnableCodeMode(enableCode);

        }

        private void EnableCodeMode(bool enable)
        {
            if(enable)
            {
                mainSearchBox.FontFamily = new("Consolas");

                mainSearchBox.TextChanging += MainSearchBoxInCodeMode_TextChanging;
                mainSearchBox.TextChanged += MainSearchBoxInCodeMode_TextChanged;
            }
            else
            {
                mainSearchBox.FontFamily = new("Segoe UI Variable");

                mainSearchBox.TextChanging -= MainSearchBoxInCodeMode_TextChanging;
                mainSearchBox.TextChanged -= MainSearchBoxInCodeMode_TextChanged;
            }
        }

        string oldText = string.Empty;
        private void MainSearchBoxInCodeMode_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            oldText = mainSearchBox.Text;
        }

        private void MainSearchBoxInCodeMode_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newText = mainSearchBox.Text;

            if(newText.Length - oldText.Length == 1) // One character added at the end 
            {
                
            }
        }

        private void suggestionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Suggestion suggestion = e.ClickedItem as Suggestion;

            SummitSuggestion(suggestion);

            Hide();
        }

        private void suggestionListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var pos = e.GetPosition(suggestionListView);
            int index = (int)(pos.Y / 40);

            Suggestion rightClickedSuggestion = suggestionListView.Items.ElementAt(index) as Suggestion;

            if (rightClickedSuggestion.Kind == SuggestionKind.Shortcut)
            {
                MenuFlyout menu = new MenuFlyout();
                MenuFlyoutItem menuFlyoutItem = new()
                {
                    Icon = new SymbolIcon { Symbol = Symbol.UnPin },
                    Text = "Unpin from the search bar"
                };

                menuFlyoutItem.Click += (s, a) => SearchBarShortcuts.Items.Remove(rightClickedSuggestion);

                menu.Items.Add(menuFlyoutItem);

                FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
                flyoutShowOptions.Position = e.GetPosition(suggestionListView);

                menu.ShowAt(suggestionListView, flyoutShowOptions);
            }
        }

        private void mainSearchBox_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            args.TryCancel();
        }

        private void mainSearchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Shift)
            {
                newTab = startNewTab;
            }

            if(e.Key == Windows.System.VirtualKey.Control)
            {
                isImFeelingLuckyEnabled = false;
            }
        }
    }
}