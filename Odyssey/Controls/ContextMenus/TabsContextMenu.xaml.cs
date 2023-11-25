using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.Data.Main;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Shared.ViewModels.WebSearch;
using Odyssey.Views;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls.ContextMenus
{
    public sealed partial class TabsContextMenu : MenuFlyout
    {
        private Tab clickedItem = null;
        public TabsContextMenu(Pin item)
        {
            this.InitializeComponent();

            Opening += TabsContextMenu_Opening;
            this.clickedItem = item;

        }

        public TabsContextMenu(Tab item)
        {
            this.InitializeComponent();

            Opening += TabsContextMenu_Opening;
            this.clickedItem = item;

        }

        private void TabsContextMenu_Opening(object sender, object e)
        {
            foreach (var menuItem in this.Items.Where(p => p.Tag != null))
            {
                bool shouldCollapse = false;

                foreach (string tag in menuItem.Tag.ToString().Split(","))
                {
                    switch (tag)
                    {
                        case "favorite":
                            if (clickedItem.GetType() != typeof(Favorite)) shouldCollapse = true;
                            break;

                        case "notfavorite":
                            if (clickedItem.GetType() == typeof(Favorite)) shouldCollapse = true;
                            break;

                        case "pinned":
                            if (clickedItem.GetType() != typeof(Pin)) shouldCollapse = true;
                            break;

                        case "notpinned":
                            if (clickedItem.GetType() == typeof(Pin)) shouldCollapse = true;
                            break;

                        case "tab":
                            if (clickedItem.GetType() != typeof(Tab)) shouldCollapse = true;
                            break;

                        case "multiple":
                            if (Tabs.Items.Count < 2) shouldCollapse = true;
                            break;

                        case "muted":
                            if (clickedItem.MainWebView != null)
                            {
                                if (!clickedItem.MainWebView.CoreWebView2.IsMuted) shouldCollapse = true;
                            }
                            else shouldCollapse = true;
                            break;

                        case "notmuted":
                            if (clickedItem.MainWebView != null)
                            {
                                if (clickedItem.MainWebView.CoreWebView2.IsMuted) shouldCollapse = true;
                            }
                            else shouldCollapse = true;
                            break;

                        case "pinnedtosearch":
                            if (!SearchBarShortcuts.Items.Any(p => p.Url == clickedItem.Url))
                                shouldCollapse = true;
                            break;

                        case "notpinnedtosearch":
                            if (SearchBarShortcuts.Items.Any(p => p.Url == clickedItem.Url))
                                shouldCollapse = true;
                            break;
                    }
                }

                menuItem.Visibility = shouldCollapse ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void PinMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Pin pin = new Pin()
            {
                MainWebView = clickedItem.MainWebView,
                Title = clickedItem.Title,
                ImageSource = clickedItem.ImageSource,
                Url = clickedItem.Url
            };
            if (pin.MainWebView is not null) ((FWebView.WebView)pin.MainWebView).LinkedTab = pin;

            Pins.Items.Add(pin);
            PaneView.Current.PinsTabView.ItemsSource = Pins.Items;
            PaneView.Current.TabsView.SelectedItem = pin;

            Tabs.Items.Remove(clickedItem);
        }

        private void UnpinMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Pins.Items.Remove((Pin)clickedItem);

            // Convert the pinned clickedItem to a tab to be recognized as a Tab when right-clicking
            Tab tab = new()
            {
                MainWebView = clickedItem.MainWebView,
                Title = clickedItem.Title,
                ImageSource = clickedItem.ImageSource
            };

            ((FWebView.WebView)tab.MainWebView).LinkedTab = tab;

            Tabs.Items.Add(tab);
            PaneView.Current.TabsView.SelectedItem = tab;
        }

        private void CloseMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            // Remove the tab's WebViews
            clickedItem.MainWebView.Close();
            if (clickedItem.SplitViewWebView != null) clickedItem.SplitViewWebView.Close();
            clickedItem.MainWebView = clickedItem.SplitViewWebView = null;

            // Getting the index of the clickedItem to remove to change the selected tab later
            int index = Tabs.Items.IndexOf(clickedItem);

            // Remove the tab from the tabs listView
            Tabs.Items.Remove(clickedItem);

            // Changing the selected tab to another
            if (index > 0) PaneView.Current.TabsView.SelectedIndex = index - 1;
            else if (Tabs.Items.Count > 0) PaneView.Current.TabsView.SelectedIndex = 0;
            else PaneView.Current.TabsView.SelectedIndex = -1;
        }

        private void CloseOtherTabsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var itemsToRemove = new List<Tab>();
            foreach (Tab tab in Tabs.Items.Where(p => p != clickedItem))
            {
                if (tab.MainWebView != null) tab.MainWebView.Close();
                if (tab.SplitViewWebView != null) tab.SplitViewWebView.Close();
                tab.MainWebView = tab.SplitViewWebView = null;

                itemsToRemove.Add(tab);
            }

            foreach (Tab tab in itemsToRemove)
            {
                Tabs.Items.Remove(tab);
            }
            PaneView.Current.TabsView.SelectedItem = clickedItem;
        }

        private void DuplicateMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Tab tab = new()
            {
                MainWebView = FWebView.WebView.Create(clickedItem.MainWebView.Source.ToString()),
                Title = clickedItem.Title,
                ToolTip = clickedItem.ToolTip,
                ImageSource = clickedItem.ImageSource,
            };
            int index = Tabs.Items.IndexOf(clickedItem);

            ((FWebView.WebView)tab.MainWebView).LinkedTab = tab;

            Tabs.Items.Insert(index + 1, tab);
        }

        private void MuteUnmuteMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            clickedItem.MainWebView.CoreWebView2.IsMuted ^= true;
        }

        private void CopyLinkMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var package = new DataPackage();
            package.SetText(clickedItem.MainWebView.Source.ToString());
            Clipboard.SetContent(package);

        }

        private void RefreshMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            clickedItem.MainWebView.Reload();
        }

        private void UnpinSearchBarMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Suggestion suggestion = SearchBarShortcuts.Items.Where(p => p.Url == clickedItem.Url).ToList().ElementAt(0);

            SearchBarShortcuts.Items.Remove(suggestion);
        }

        private void PinSearchBarMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Suggestion suggestion = new()
            {
                Kind = SuggestionKind.Shortcut,
                Title = clickedItem.Title,
                Url = clickedItem.Url
            };

            SearchBarShortcuts.Items.Add(suggestion);
        }

        private void NewWindowMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddToFavoritesMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Favorite favorite = new()
            {
                Url = clickedItem.Url,
                ImageSource = clickedItem.ImageSource
            };

            if (clickedItem.MainWebView != null)
            {
                favorite.MainWebView = clickedItem.MainWebView;
            }

            Favorites.Items.Add(favorite);
        }

        private void RemoveFromFvoritesMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Favorites.Items.Remove(clickedItem as Favorite);
        }
    }
}
