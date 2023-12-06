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
using Odyssey.Views;
using System.Web;
using System.Net;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls
{
    public sealed partial class MainMenuFlyout : Flyout
    {
        public MainMenuFlyout()
        {
            this.InitializeComponent();
            Opening += MainMenuFlyout_Opening;
        }

        private void MainMenuFlyout_Opening(object sender, object e)
        {
            if(MainView.CurrentlySelectedWebView != null)
            {
                var info = MainView.CurrentlySelectedWebView.SecurityInformation;
            }
        }

        private async void urlTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                
            }
        }

        private void urlTextBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void urlTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
