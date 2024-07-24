using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView.Controls.Flyouts
{
    public sealed partial class QuickSearchFlyout : Flyout
    {
        DispatcherTimer cooldownTimer = new() { Interval = TimeSpan.FromMilliseconds(800) };
        public string SearchText { get; set; }
        public QuickSearchFlyout()
        {
            this.InitializeComponent();
            cooldownTimer.Tick += CooldownTimer_Tick;

            mainSearchBox.Loaded += (s, a) => mainSearchBox.Text = SearchText;
            Closed += (s, a) => webView.Close();
        }

        private void CooldownTimer_Tick(object sender, object e)
        {
            webView.CoreWebView2.Navigate(Data.Settings.SearchEngine.Google.SearchUrl + mainSearchBox.Text);
            cooldownTimer.Stop();
        }

        private async void webView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36 Edg/120.0.0.0";
            sender.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            sender.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false; // TODO: Implement a limited version of the custom flyout
            webView.CoreWebView2.NavigationStarting += (s, a) => webView.Opacity = 0;
            webView.CoreWebView2.DOMContentLoaded += (s, a) => { webView.Opacity = 1; sender.ExecuteScriptAsync("document.querySelector(\"header\").remove() "); };

        }


        private async void CoreWebView2_NavigationCompleted(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            await sender.ExecuteScriptAsync("document.querySelector(\"header\").remove() ");
        }

        private void mainSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            cooldownTimer.Stop();
            
            if(mainSearchBox.Text == string.Empty)
                webViewRowDef.Height = new GridLength(0);
            else
            {
                webViewRowDef.Height = new GridLength(400);
                cooldownTimer.Start();
            }

            
        }
    }
}
