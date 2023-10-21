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
            TabsView.ItemsSource = Tabs.TabsList;

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

        }

        private void AddTabButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pos = e.GetPosition(TabsView);
            int index = (int)(pos.Y / 40); // Get tab index

            var tabToRemove = Tabs.TabsList.ElementAt(index);

            // Remove the tab's WebViews
            tabToRemove.MainWebView = tabToRemove.SplitViewWebView = null;

            // Remove the tab from the tabs listView
            Tabs.TabsList.Remove(tabToRemove);

            // Changing the selected tab to another
            if (index > 0) TabsView.SelectedIndex = index - 1;
            else if (Tabs.TabsList.Count > 0) TabsView.SelectedIndex = 0;
            else TabsView.SelectedIndex = -1;
        }
    }
}
