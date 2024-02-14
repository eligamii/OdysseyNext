using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Data.Main;
using Odyssey.QuickActions;
using Odyssey.QuickActions.Controls;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView.Controls.Flyouts
{
    public sealed partial class PreviewFlyout : Flyout
    {
        public PreviewFlyout()
        {
            this.InitializeComponent();
            Closing += PreviewFlyout_Closing;


        }

        private void PreviewFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            args.Cancel = true;
        }

        private void webView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36 Edg/120.0.0.0";
            sender.CoreWebView2.Settings.AreDevToolsEnabled = false;
            sender.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            sender.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;

            Fix();
        }

        private async void Fix() // TO REMOVE
        {
            await Task.Delay(1000);
            Closing -= PreviewFlyout_Closing;
        }
        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            string source = webView.Source.ToString();

            WebView web = WebView.Create(webView.Source.ToString());
            Tab tab = new()
            {
                Url = source,
                ToolTip = source,
                ImageSource = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(webView.CoreWebView2.FaviconUri))
            };

            web.ParentTab = WebView.SelectedWebView.LinkedTab;
            web.LinkedTab = tab;

            tab.MainWebView = web;
            Tabs.Items.Add(tab);

            webView.Close();
            WebView.TabsView.SelectedItem = tab;

        }
    }
}
