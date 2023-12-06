using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Odyssey.Data.Main;
using Odyssey.Data.Settings;
using Odyssey.FWebView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Views.Options
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MiscPage : Page
    {
        private static WebView2 _webViewToEngageReset = null;
        public MiscPage()
        {
            this.InitializeComponent();
            singleInstanceToggleSwitch.IsOn = Settings.IsSingleInstanceEnabled;

            if (_webViewToEngageReset == null) _webViewToEngageReset = WebView.Create();
        }

        private void singleInstanceToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.IsSingleInstanceEnabled = singleInstanceToggleSwitch.IsOn;
        }

        private int _clickCount = 0;
        private async void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            _clickCount++;

            if (_clickCount == 100)
            {
                await Data.Main.Data.Reset(_webViewToEngageReset.CoreWebView2);
                MainWindow.ResetEngaged = true;
                MainWindow.Current.Close();
            }
        }


    }
}
