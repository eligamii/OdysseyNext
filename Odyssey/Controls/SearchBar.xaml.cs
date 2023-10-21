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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls
{
    public sealed partial class SearchBar : Flyout
    {
        FWebView webView = FWebView.New();
        public SearchBar()
        {
            this.InitializeComponent();
        }

        private void Flyout_Opened(object sender, object e)
        {

        }

        private void mainSearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                if(FWebView.CurrentlySelected == null)
                {
                    Tab tab = new()
                    {
                        Title = "Test",
                        ToolTip = "Testing is something cool",
                    };

                    webView.Source = new Uri((sender as TextBox).Text);

                    tab.MainWebView = webView;
                    Tabs.TabsList.Add(tab);
                    PaneView.Current.TabsView.SelectedItem = tab;

                    MainView.Current.splitViewContentFrame.Content = webView;
                }

                Hide();
            }
        }
    }
}
