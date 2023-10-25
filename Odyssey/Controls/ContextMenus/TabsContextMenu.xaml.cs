using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Data.Main;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Shared.ViewModels.WebSearch;
using Odyssey.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls.ContextMenus
{
    public sealed partial class TabsContextMenu : MenuFlyout
    {
        private Tab item = null;
        public TabsContextMenu(Pin item)
        {
            this.InitializeComponent();

            Opening += TabsContextMenu_Opening;
            this.item = item;
            
        }

        public TabsContextMenu(Tab item)
        {
            this.InitializeComponent();

            Opening += TabsContextMenu_Opening;
            this.item = item;

        }

        private void TabsContextMenu_Opening(object sender, object e)
        {
            foreach(var menuItem in this.Items.Where(p => p.Tag != null))
            {
                bool shouldCollapse = false;

                foreach(string tag in menuItem.Tag.ToString().Split(","))
                {
                    switch(tag)
                    {
                        case "favorite":
                            if (item.GetType() != typeof(Favorite)) shouldCollapse = true;
                            break;

                        case "notfavorite":
                            if (item.GetType() == typeof(Favorite)) shouldCollapse = true;
                            break;

                        case "pinned":
                            if (item.GetType() != typeof(Pin)) shouldCollapse = true;
                            break;

                        case "notpinned":
                            if (item.GetType() == typeof(Pin)) shouldCollapse = true;
                            break;

                        case "tab":
                            if (item.GetType() != typeof(Tab)) shouldCollapse = true;
                            break;

                        case "multiple":
                            if(Tabs.Items.Count < 2) shouldCollapse = true;  
                            break;

                        case "muted":
                            if (!item.MainWebView.CoreWebView2.IsMuted) shouldCollapse = true;
                            break;

                        case "notmuted":
                            if (item.MainWebView.CoreWebView2.IsMuted) shouldCollapse = true;
                            break;

                        case "pinnedtosearch":
                            if (!SearchBarShortcuts.Items.Any(p => p.Url == item.MainWebView.Source.ToString()))
                                shouldCollapse = true;
                            break;

                        case "notpinnedtosearch":
                            if (SearchBarShortcuts.Items.Any(p => p.Url == item.MainWebView.Source.ToString()))
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
                MainWebView = item.MainWebView,
                Title = item.Title,
                ImageSource = item.ImageSource,
                Url = item.Url
            };
            ((FWebView.WebView)pin.MainWebView).LinkedTab = pin;

            Pins.Items.Add(pin);
            PaneView.Current.PinsTabView.ItemsSource = Pins.Items;
            PaneView.Current.TabsView.SelectedItem = pin;

            Tabs.Items.Remove(item);
        }

        private void UnpinMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Pins.Items.Remove((Pin)item);

            // Convert the pinned item to a tab to be recognized as a Tab when right-clicking
            Tab tab = new()
            {
                MainWebView = item.MainWebView,
                Title = item.Title,
                ImageSource = item.ImageSource
            };

            ((FWebView.WebView)tab.MainWebView).LinkedTab = tab;

            Tabs.Items.Add(tab);
            PaneView.Current.TabsView.SelectedItem = tab;
        }

        private void CloseMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            // Remove the tab's WebViews
            item.MainWebView.Close();
            if (item.SplitViewWebView != null) item.SplitViewWebView.Close();
            item.MainWebView = item.SplitViewWebView = null;

            // Getting the index of the item to remove to change the selected tab later
            int index = Tabs.Items.IndexOf(item);

            // Remove the tab from the tabs listView
            Tabs.Items.Remove(item);

            // Changing the selected tab to another
            if (index > 0) PaneView.Current.TabsView.SelectedIndex = index - 1;
            else if (Tabs.Items.Count > 0) PaneView.Current.TabsView.SelectedIndex = 0;
            else PaneView.Current.TabsView.SelectedIndex = -1;
        }

        private void CloseOtherTabsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var itemsToRemove = new List<Tab>();
            foreach(Tab tab in Tabs.Items.Where(p => p != item))
            {
                tab.MainWebView.Close();
                if (tab.SplitViewWebView != null) tab.SplitViewWebView.Close();
                tab.MainWebView = tab.SplitViewWebView = null;

                itemsToRemove.Add(tab);
            }

            foreach(Tab tab in itemsToRemove)
            {
                Tabs.Items.Remove(tab);
            }
            PaneView.Current.TabsView.SelectedItem = item;
        }

        private void DuplicateMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Tab tab = new()
            {
                MainWebView = FWebView.WebView.New(item.MainWebView.Source.ToString()),
                Title = item.Title,
                ToolTip = item.ToolTip,
                ImageSource = item.ImageSource,
            };
            int index = Tabs.Items.IndexOf(item);

            ((FWebView.WebView)tab.MainWebView).LinkedTab = tab;

            Tabs.Items.Insert(index + 1, tab);
        }

        private void MuteUnmuteMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            item.MainWebView.CoreWebView2.IsMuted ^= true;
        }

        private void CopyLinkMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var package = new DataPackage();
            package.SetText(item.MainWebView.Source.ToString());
            Clipboard.SetContent(package);

        }

        private void RefreshMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            item.MainWebView.Reload();
        }

        private void UnpinSearchBarMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Suggestion suggestion = SearchBarShortcuts.Items.Where(p => p.Url == item.Url).ToList().ElementAt(0);

            SearchBarShortcuts.Items.Remove(suggestion);
        }

        private void PinSearchBarMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Suggestion suggestion = new()
            {
                Kind = SuggestionKind.Shortcut,
                Title = item.Title,
                Url = item.Url
            };

            SearchBarShortcuts.Items.Add(suggestion);
        }
    }
}
