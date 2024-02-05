using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Odyssey.Controls;
using Odyssey.Controls.ContextMenus;
using Odyssey.Data.Main;
using Odyssey.Data.Settings;
using Odyssey.Dialogs;
using Odyssey.FWebView;
using Odyssey.OtherWindows;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;




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
            FavoriteGrid.ItemsSource = Favorites.Items;

            WebView.TabsView = TabsView;

            Loaded += PaneView_Loaded;

            Current = this;
        }

        private void PaneView_Loaded(object sender, RoutedEventArgs e)
        {
            // Restore tabs icons with the the Favicon Kit api

            try // Prevent crash on timing issues
            {
                foreach (Tab tab in Tabs.Items)
                {
                    if (tab.ImageSource == null && tab.Url != null)
                    {
                        try
                        {
                            if (FWebView.Helpers.WebView2SavedFavicons.GetFaviconAsBitmapImage(tab.Url, out BitmapImage image))
                            {
                                tab.ImageSource = image;
                            }
                            else
                            {
                                tab.ImageSource = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage { UriSource = new Uri($"https://muddy-jade-bear.faviconkit.com/{new System.Uri(tab.Url).Host}/21") };
                            }
                        }
                        catch { }
                    }
                }

                foreach (Tab tab in Pins.Items)
                {
                    if (tab.ImageSource == null && tab.Url != null)
                    {
                        try
                        {
                            if (FWebView.Helpers.WebView2SavedFavicons.GetFaviconAsBitmapImage(tab.Url, out BitmapImage image))
                            {
                                tab.ImageSource = image;
                            }
                            else
                            {
                                tab.ImageSource = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage { UriSource = new Uri($"https://muddy-jade-bear.faviconkit.com/{new System.Uri(tab.Url).Host}/21") };
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        private void FullScreenControlsPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshFavoriteItemsWidth()
        {
            switch (FavoriteGrid.Items.Count)
            {
                case 1:
                    SetFavWidth(204);
                    break;

                case 2:
                    SetFavWidth(100);
                    break;

                case 3:
                    SetFavWidth(65);
                    break;

                case 4:
                    SetFavWidth(48);
                    break;
            }

        }

        private void SetFavWidth(int width)
        {
            foreach (Favorite item in Favorites.Items)
            {
                item.Width = width;
            }
        }

        private void FavoriteGrid_Loaded(object sender, RoutedEventArgs e)
        {
            FavoriteGrid.ItemsSource = Favorites.Items;
            FavoriteGrid.Visibility = Favorites.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

            RefreshFavoriteItemsWidth();

            Favorites.Items.CollectionChanged += (s, a) =>
            {
                FavoriteGrid.Visibility = Favorites.Items.Count() == 0 ? Visibility.Collapsed : Visibility.Visible;
                RefreshFavoriteItemsWidth();
            };

            foreach (var item in Favorites.Items)
            {
                WebView webView = WebView.Create(item.Url);
                webView.LinkedTab = item;

                item.MainWebView = webView;
            }
        }

        private void PinsTabView_Loaded(object sender, RoutedEventArgs e)
        {
            PinsTabView.ItemsSource = Pins.Items;
            pinsTextBlock.Visibility = Pins.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

            Pins.Items.CollectionChanged += (s, a) =>
            {
                pinsTextBlock.Visibility = Pins.Items.Count() == 0 ? Visibility.Collapsed : Visibility.Visible;
            };
        }

        private void TabsView_Loaded(object sender, RoutedEventArgs e)
        {
            newTabButtonSeparator.Visibility = tabsTextBlock.Visibility = Tabs.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            newTabButton.Background = Tabs.Items.Count == 0 ? Application.Current.Resources["ControlFillColorDefaultBrush"] as Brush : new SolidColorBrush(Colors.Transparent);

            Tabs.Items.CollectionChanged += (s, a) =>
            {
                newTabButtonSeparator.Visibility = tabsTextBlock.Visibility = Tabs.Items.Count() == 0 ? Visibility.Collapsed : Visibility.Visible;
                newTabButton.Background = Tabs.Items.Count() == 0 ? Application.Current.Resources["ControlFillColorDefaultBrush"] as Brush : new SolidColorBrush(Colors.Transparent);

            };
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
            if ((sender as Button).Tag.ToString() == "pins")
            {
                var pos = e.GetPosition(PinsTabView);
                int index = (int)(pos.Y / 40); // Get pin index

                var pinToRemove = Pins.Items.ElementAt(index);
                Tab parentTab = null;

                // Remove the pin's WebViews
                if (pinToRemove.MainWebView != null)
                {
                    parentTab = ((WebView)pinToRemove.MainWebView).ParentTab;

                    pinToRemove.MainWebView.Close();
                    if (pinToRemove.SplitViewWebView != null) pinToRemove.SplitViewWebView.Close();
                    pinToRemove.MainWebView = pinToRemove.SplitViewWebView = null;
                }

                // Remove the pin from the pins listView
                Pins.Items.Remove(pinToRemove);

                if (parentTab != null)
                {
                    if (parentTab.GetType() == typeof(Pin))
                    {
                        if (Pins.Items.Contains(parentTab as Pin))
                            PinsTabView.SelectedItem = parentTab as Pin;
                    }
                    else if (parentTab.GetType() == typeof(Favorite))
                    {
                        if (Favorites.Items.Contains(parentTab))
                            FavoriteGrid.SelectedItem = parentTab;
                    }
                    else
                    {
                        if (Tabs.Items.Contains(parentTab))
                            TabsView.SelectedItem = parentTab;
                    }
                }

                PinsTabView.ItemsSource = Pins.Items;
            }
            else
            {
                var pos = e.GetPosition(TabsView);
                int index = (int)(pos.Y / 40); // Get favorite index

                var tabToRemove = Tabs.Items.ElementAt(index);
                Tab parentTab = null;

                if (tabToRemove.MainWebView != null) // Prevent crashes when the favorite was restored and with no webview
                {
                    parentTab = ((WebView)tabToRemove.MainWebView).ParentTab; // getting the paent favorite if one

                    // Remove the favorite's WebViews
                    tabToRemove.MainWebView.Close();
                    if (tabToRemove.SplitViewWebView != null) tabToRemove.SplitViewWebView.Close();
                    tabToRemove.MainWebView = tabToRemove.SplitViewWebView = null;
                }

                // Remove the favorite from the tabs listView
                Tabs.Items.Remove(tabToRemove);

                // Changing the selected favorite to another
                if (parentTab != null)
                {
                    // Select the parent favorite if one 
                    if (parentTab.GetType() == typeof(Pin))
                    {
                        if (Pins.Items.Contains(parentTab as Pin))
                            PinsTabView.SelectedItem = parentTab as Pin;
                    }
                    else if (parentTab.GetType() == typeof(Favorite))
                    {
                        if (Favorites.Items.Contains(parentTab))
                            FavoriteGrid.SelectedItem = parentTab;
                    }
                    else
                    {
                        if (Tabs.Items.Contains(parentTab))
                            TabsView.SelectedItem = parentTab;
                    }
                }
                else
                {
                    if (index > 0) TabsView.SelectedIndex = index - 1;
                    else if (Tabs.Items.Count > 0) TabsView.SelectedIndex = 0;
                    else TabsView.SelectedIndex = -1;
                }
            }
        }

        private void ItemsViews_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {
                MainView.Current.splitViewContentFrame.Content = null;

                var tab = e.AddedItems.FirstOrDefault() as Tab;
                if (tab.MainWebView == null)
                {
                    if (tab.Url != null)
                    {
                        WebView webView = WebView.Create(tab.Url);
                        webView.LinkedTab = tab;
                        tab.MainWebView = webView;
                    }
                    else
                    {
                        WebView webView = WebView.Create(SearchEngine.ToSearchEngineObject((SearchEngines)Settings.SelectedSearchEngine).Url);
                        webView.LinkedTab = tab;
                        tab.MainWebView = webView;
                    }
                }

                MainView.Current.splitViewContentFrame.Content = tab.MainWebView;

                MainView.Current.documentTitle.Text = tab.Title;

                _ = FWebView.Classes.DynamicTheme.UpdateDynamicThemeAsync(tab.MainWebView);
                UpdateTabSelection(sender);
            }
            else if(e.RemovedItems.Count > 0 && e.AddedItems.Count == 0)
            {
                Tab removedItem = e.RemovedItems[0] as Tab;
                Tab parentTab = null;

                if (removedItem.MainWebView != null) // Prevent crashes when the favorite was restored and with no webview
                {
                    parentTab = ((WebView)removedItem.MainWebView).ParentTab; // getting the paent favorite if one

                    // Remove the favorite's WebViews
                    removedItem.MainWebView.Close();
                    if (removedItem.SplitViewWebView != null) removedItem.SplitViewWebView.Close();
                    removedItem.MainWebView = removedItem.SplitViewWebView = null;
                }

                // Remove the favorite from the tabs listView
                Tabs.Items.Remove(removedItem);

                // Changing the selected favorite to another
                if (parentTab != null)
                {
                    // Select the parent favorite if one 
                    if (parentTab.GetType() == typeof(Pin))
                    {
                        if (Pins.Items.Contains(parentTab as Pin))
                            PinsTabView.SelectedItem = parentTab as Pin;
                    }
                    else if (parentTab.GetType() == typeof(Favorite))
                    {
                        if (Favorites.Items.Contains(parentTab))
                            FavoriteGrid.SelectedItem = parentTab;
                    }
                    else
                    {
                        if (Tabs.Items.Contains(parentTab))
                            TabsView.SelectedItem = parentTab;
                    }
                }
            }


            if (e.AddedItems.Count != 0) // Save tabs as much as possible to avoid data loss after crash
                Tabs.Save();

            MainView.Current.SetTotpButtonVisibility();

            try
            {
                // Enable picture in picture
                if (Settings.IsAutoPictureInPictureEnabled != false)
                {
                    if (e.RemovedItems.Count > 0)
                    {
                        Tab item = e.RemovedItems[0] as Tab;
                        if (item.MainWebView != null)
                        {
                            _ = item.MainWebView.ExecuteScriptAsync("document.querySelector(\"video\").requestPictureInPicture();");
                        }
                    }
                    if (e.AddedItems.Count > 0)
                    {
                        Tab addedItem = e.AddedItems[0] as Tab;
                        if (addedItem.MainWebView != null)
                        {
                            _ = addedItem.MainWebView.ExecuteScriptAsync("document.exitPictureInPicture();");
                        }
                    }
                }
            }
            catch (InvalidOperationException) { }
        }
        private void FavoriteGridItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent((DependencyObject)sender);
            parent = VisualTreeHelper.GetParent((DependencyObject)parent);

            Favorite favorite = ((GridViewItem)parent).Content as Favorite;

            TabsContextMenu tabsContextMenu = new(favorite);

            FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
            flyoutShowOptions.Position = e.GetPosition(this);

            tabsContextMenu.ShowAt(this, flyoutShowOptions);
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
            if ((ListViewBase)selectedTabsView != TabsView) TabsView.SelectedIndex = -1;
            if ((ListViewBase)selectedTabsView != PinsTabView) PinsTabView.SelectedIndex = -1;
            if ((ListViewBase)selectedTabsView != FavoriteGrid) FavoriteGrid.SelectedIndex = -1;
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
            if (sender as ListView == PinsTabView)
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
            if (e.DataView.Contains(StandardDataFormats.Uri) || e.DataView.Contains(StandardDataFormats.Text))
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

            if (draggedItem != null)
            {
                pin = new()
                {
                    Url = draggedItem.Url,
                    ImageSource = draggedItem.ImageSource,
                    MainWebView = draggedItem.MainWebView,
                    ToolTip = draggedItem.ToolTip,
                    Title = draggedItem.Title
                };

                if (Favorites.Items.Contains(draggedItem)) Favorites.Items.Remove(draggedItem as Favorite);
                else Tabs.Items.Remove(draggedItem);
            }
            else
            {
                string text = await e.DataView.GetTextAsync();

                string url = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToWebView2Url(text);

                pin = new()
                {
                    Url = url,
                    ToolTip = url,
                    Title = text
                };

                WebView webView = WebView.Create(url);
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

                if (Pins.Items.Contains(draggedItem)) Pins.Items.Remove(draggedItem as Pin);
                else Favorites.Items.Remove(draggedItem as Favorite);
            }
            else
            {
                string text = await e.DataView.GetTextAsync();

                string url = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToWebView2Url(text);

                tab = new()
                {
                    Url = url,
                    ToolTip = url,
                    Title = text
                };

                WebView webView = WebView.Create(url);
                webView.LinkedTab = tab;
                tab.MainWebView = webView;
            }

            var pos = e.GetPosition(TabsView);
            int index = (int)(pos.Y / 40);

            Tabs.Items.Insert(index, tab);

            TabsView.SelectedItem = tab;

        }

        private void FavoriteGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            // Calculate the index of the hovered item
            var pos = e.GetCurrentPoint(FavoriteGrid);

            int itemWidth = Favorites.Items.First().Width; // Same width for every FavoriteItems
            float scaleAjustement = 1.2f;

            int columnIndex = (int)(pos.Position.X / (itemWidth * scaleAjustement));
            int rowIndex = Favorites.Items.Count <= 4 ? 0 : (int)(pos.Position.Y / 60);

            int index = columnIndex + 4 * rowIndex;

            // Set the hoveredItem to null if no item was hovered
            hoveredItem = index < Favorites.Items.Count ? Favorites.Items.ElementAt(index) : null;
        }

        private async void FavoriteGrid_Drop(object sender, DragEventArgs e)
        {
            Favorite favorite;

            if (draggedItem != null)
            {
                favorite = new()
                {
                    Url = draggedItem.Url,
                    ImageSource = draggedItem.ImageSource,
                    MainWebView = draggedItem.MainWebView,
                    ToolTip = draggedItem.ToolTip,
                    Title = draggedItem.Title
                };

                if (Pins.Items.Contains(draggedItem)) Pins.Items.Remove(draggedItem as Pin);
                else Tabs.Items.Remove(draggedItem);
            }
            else
            {
                string text = await e.DataView.GetTextAsync();

                string url = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToWebView2Url(text);

                favorite = new()
                {
                    Url = url,
                    ToolTip = url,
                    Title = text
                };

                WebView webView = WebView.Create(url);
                webView.LinkedTab = favorite;
                favorite.MainWebView = webView;
            }



            var pos = e.GetPosition(FavoriteGrid);

            int itemWidth = Favorites.Items.First().Width;
            float scaleAjustement = 1.2f;

            int columnIndex = (int)(pos.X / (itemWidth * scaleAjustement));
            int rowIndex = Favorites.Items.Count <= 4 ? 0 : (int)(pos.Y / 60);

            int index = columnIndex + 4 * rowIndex;

            if (index < Favorites.Items.Count)
                Favorites.Items.Insert(index, favorite);
            else
                Favorites.Items.Insert(Favorites.Items.Count, favorite);



            FavoriteGrid.SelectedItem = favorite;
        }


        private void ViewportBehavior_EnteredViewport(object sender, EventArgs e)
        {
            secondNewTabButton.Visibility = ViewportBehavior.IsFullyInViewport ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SettingsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsDialog settingsDialog = new()
            {
                XamlRoot = this.XamlRoot
            };

            _ = settingsDialog.ShowAsync();
        }

        private void DevToolsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            DevToolsWindow devToolsWindow = new();
            devToolsWindow.Activate();
        }
    }
}
