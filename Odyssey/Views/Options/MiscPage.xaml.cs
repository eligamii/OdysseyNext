using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.Data.Settings;
using Odyssey.FWebView;




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
            openTabAtStartupToggleSwitch.IsOn = Settings.OpenTabAtStartup;

            if (_webViewToEngageReset == null) _webViewToEngageReset = WebView.Create();
        }

        private void singleInstanceToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.IsSingleInstanceEnabled = singleInstanceToggleSwitch.IsOn;
        }
        private void openTabAtStartupToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.OpenTabAtStartup = openTabAtStartupToggleSwitch.IsOn;
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
