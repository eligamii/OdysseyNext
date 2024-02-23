using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView.Controls
{
    public sealed partial class ExtensionPopupFlyout : Flyout
    {
        public ExtensionPopupFlyout(BrowserExtensionInfo info)
        {
            this.InitializeComponent();
            popupWebView.Source = new Uri(info.DefaultPopup);
            popupWebView.CoreWebView2Initialized += PopupWebView_CoreWebView2Initialized;
        }

        private async void PopupWebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.WebMessageReceived += Sender_WebMessageReceived;
            await sender.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("function message() {window.chrome.webview.postMessage(\"CHANGED\")} window.addEventListener('resize', handle)");
        }

        private async void Sender_WebMessageReceived(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            if (args.WebMessageAsJson == "CHANGED")
            {
                int height, width = 0;
                bool succed = int.TryParse(await sender.ExecuteScriptAsync("document.documentElement.clientHeight"), out height) &&
                              int.TryParse(await sender.ExecuteScriptAsync("document.documentElement.clientWidth"), out width);

                if(succed)
                {
                    sender.Height = height;
                    sender.Width = width;
                }


            }
        }
    }
}
