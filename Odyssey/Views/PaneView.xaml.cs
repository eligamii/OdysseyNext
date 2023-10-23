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
using Odyssey.Shared.DataTemplates.Data;
using System.Threading.Tasks;
using Odyssey.Controls;
using Odyssey.Controls.ContextMenus;
using Odyssey.FWebView;

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

            Current = this;
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
            var pos = e.GetPosition(TabsView);
            int index = (int)(pos.Y / 40); // Get tab index, idk if usable with more than 25 tabs (40 is approx.)

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

                MainView.Current.splitViewContentFrame.Content = tab.MainWebView;
                UpdateTabSelection(sender);
            }
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
    }
}
