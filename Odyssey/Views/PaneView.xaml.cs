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
using Odyssey.Data.Main;
using Odyssey.Shared.ViewModels.Data;
using System.Threading.Tasks;
using Odyssey.Controls;
using Odyssey.Controls.ContextMenus;
using Odyssey.FWebView;
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PaneView : Page
    {
        public static PaneView Current { get; set; }
        public PaneView()
        {
            this.InitializeComponent();
            TabsView.ItemsSource = Tabs.Items;
            PinsTabView.ItemsSource = Pins.Items;

            Loaded += PaneView_Loaded;

            Current = this;
        }

        private void PaneView_Loaded(object sender, RoutedEventArgs e)
        {
            // Restore tabs icons based on the Favicon Kit api

            try // Prevent crash on timing issues
            {
                foreach (Tab tab in Tabs.Items)
                {
                    if (tab.ImageSource == null && tab.Url != null)
                    {
                        tab.ImageSource = new()
                        {
                            UriSource = new System.Uri($"https://muddy-jade-bear.faviconkit.com/{new System.Uri(tab.Url).Host}/21")
                        };
                    }
                }

                foreach (Tab tab in Pins.Items)
                {
                    if (tab.ImageSource == null && tab.Url != null)
                    {
                        tab.ImageSource = new()
                        {
                            UriSource = new System.Uri($"https://muddy-jade-bear.faviconkit.com/{new System.Uri(tab.Url).Host}/21")
                        };
                    }
                }
            }
            catch { }
        }

        private void FullScreenControlsPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void FavoriteGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void PinsTabView_Loaded(object sender, RoutedEventArgs e)
        {
            PinsTabView.ItemsSource = Pins.Items;
        }

        private void AddTabButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBar searchBar = new SearchBar(true);
            FlyoutShowOptions options = new FlyoutShowOptions();
            options.Placement = FlyoutPlacementMode.Bottom;
            options.Position = new Point(MainView.Current.splitViewContentFrame.ActualWidth / 2, 100);

            searchBar.ShowAt(MainView.Current.splitViewContentFrame, options);
        }

        private void CloseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(sender as ListView == TabsView)
            {
                var pos = e.GetPosition(TabsView);
                int index = (int)(pos.Y / 40); // Get tab index

                var tabToRemove = Tabs.Items.ElementAt(index);

                // Remove the tab's WebViews
                tabToRemove.MainWebView.Close();
                if (tabToRemove.SplitViewWebView != null) tabToRemove.SplitViewWebView.Close();
                tabToRemove.MainWebView = tabToRemove.SplitViewWebView = null;

                // Remove the tab from the tabs listView
                Tabs.Items.Remove(tabToRemove);

                // Changing the selected tab to another
                if (index > 0) TabsView.SelectedIndex = index - 1;
                else if (Tabs.Items.Count > 0) TabsView.SelectedIndex = 0;
                else TabsView.SelectedIndex = -1;
            }
            else
            {
                var pos = e.GetPosition(PinsTabView);
                int index = (int)(pos.Y / 40); // Get pin index

                var pinToRemove = Pins.Items.ElementAt(index);

                // Remove the pin's WebViews
                if (pinToRemove.MainWebView != null)
                {
                    pinToRemove.MainWebView.Close();
                    if (pinToRemove.SplitViewWebView != null) pinToRemove.SplitViewWebView.Close();
                    pinToRemove.MainWebView = pinToRemove.SplitViewWebView = null;
                }

                // Remove the pin from the pins listView
                Pins.Items.Remove(pinToRemove);

                PinsTabView.SelectedIndex = -1;

                PinsTabView.ItemsSource = Pins.Items;
            }
        }

        private async void TabsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                var tab = e.AddedItems[0] as Tab;
                if (tab.MainWebView == null)
                {
                    WebView webView = WebView.New(tab.Url);
                    webView.LinkedTab = tab;
                    await webView.EnsureCoreWebView2Async();
                    tab.MainWebView = webView;
                }


                if((tab.MainWebView as WebView).IsPageLoading)
                {
                    MainView.Current.Favicon.Source = null;
                    MainView.Current.progressRing.Visibility = Visibility.Visible;
                }
                else
                {
                    MainView.Current.Favicon.Source = tab.ImageSource;
                    MainView.Current.progressRing.Visibility = Visibility.Collapsed;
                }

                FWebView.Classes.DynamicTheme.UpdateDynamicTheme(tab.MainWebView);

                MainView.Current.splitViewContentFrame.Content = tab.MainWebView;
                UpdateTabSelection(sender);
            }

            if (e.AddedItems.Count != 0) // Save tabs as much as possible to avoid data loss after crash
                Tabs.Save();
        }

        private void TabsView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            TabsContextMenu tabsContextMenu;

            if ((sender as ListView) == PinsTabView)
            {
                var pos = e.GetPosition(PinsTabView);
                int index = (int)(pos.Y / 40);

                var rightClickedTab = Pins.Items.ElementAt(index);
                tabsContextMenu = new(rightClickedTab);
            }
            else
            {
                var pos = e.GetPosition(TabsView);
                int index = (int)(pos.Y / 40);

                var rightClickedTab = Tabs.Items.ElementAt(index);
                tabsContextMenu = new(rightClickedTab); 
            }


            FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
            flyoutShowOptions.Position = e.GetPosition(this);

            tabsContextMenu.ShowAt(this, flyoutShowOptions);
        }

        private void UpdateTabSelection(object selectedTabsView)
        {
            if ((ListView)selectedTabsView != TabsView) TabsView.SelectedIndex = -1;
            if ((ListView)selectedTabsView != PinsTabView) PinsTabView.SelectedIndex = -1;
            if ((ListView)selectedTabsView != FavoriteGrid) FavoriteGrid.SelectedIndex = -1;
        }



        // Drag and drop system
        Tab hoveredItem;
        Tab draggedItem;
        private void TabsView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            draggedItem = hoveredItem;
        }

        
        private void TabsView_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if(sender as ListView == PinsTabView)
            {
                var pos = e.GetCurrentPoint(PinsTabView);
                int index = (int)(pos.Position.Y / 40);

                hoveredItem = Pins.Items.ElementAt(index);
            }
            else
            {
                var pos = e.GetCurrentPoint(TabsView);
                int index = (int)(pos.Position.Y / 40);

                hoveredItem = Tabs.Items.ElementAt(index);
            }
        }

        private void TabsView_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = draggedItem != null ? DataPackageOperation.Move : DataPackageOperation.None;
            if(e.DataView.Contains(StandardDataFormats.Uri) || e.DataView.Contains(StandardDataFormats.Text))
            {
                e.AcceptedOperation = DataPackageOperation.Link;
            }

        }

        private void TabsView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            draggedItem = null;
        }



        private async void PinsTabView_Drop(object sender, DragEventArgs e)
        {
            Pin pin;

            if(draggedItem != null)
            {
                pin = new()
                {
                    Url = draggedItem.Url,
                    ImageSource = draggedItem.ImageSource,
                    MainWebView = draggedItem.MainWebView,
                    ToolTip = draggedItem.ToolTip,
                    Title = draggedItem.Title
                };

                Tabs.Items.Remove(draggedItem);
            }
            else
            {
                string text = await e.DataView.GetTextAsync();

                string url = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToUrl(text);

                pin = new()
                {
                    Url = url,
                    ToolTip = url,
                    Title = text
                };

                WebView webView = WebView.New(url);
                webView.LinkedTab = pin;
                pin.MainWebView = webView;
            }

            var pos = e.GetPosition(PinsTabView);
            int index = (int)(pos.Y / 40);

            Pins.Items.Insert(index, pin);
            PinsTabView.ItemsSource = Pins.Items;

            PinsTabView.SelectedItem = pin;

            
        }

        private async void TabsView_Drop(object sender, DragEventArgs e)
        {
            Tab tab;

            if (draggedItem != null)
            {
                tab = new()
                {
                    Url = draggedItem.Url,
                    ImageSource = draggedItem.ImageSource,
                    MainWebView = draggedItem.MainWebView,
                    ToolTip = draggedItem.ToolTip,
                    Title = draggedItem.Title
                };

                Pins.Items.Remove(draggedItem as Pin);
            }
            else
            {
                string text = await e.DataView.GetTextAsync();

                string url = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToUrl(text);

                tab = new()
                {
                    Url = url,
                    ToolTip = url,
                    Title = text
                };

                WebView webView = WebView.New(url);
                webView.LinkedTab = tab;
                tab.MainWebView = webView;
            }

            var pos = e.GetPosition(TabsView);
            int index = (int)(pos.Y / 40);

            Tabs.Items.Insert(index, tab);

            TabsView.SelectedItem = tab;

        }
    }
}
