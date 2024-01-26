using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView.Controls.Flyouts
{
    public sealed partial class FindFlyout : TeachingTip // it's a teachingtip but is used like a flyout here
    {
        // works with the window.find(). See https://developer.mozilla.org/fr/docs/Web/API/Window/find
        private WebView2 webView;
        public FindFlyout(WebView2 webView)
        {
            this.InitializeComponent();
            this.webView = webView;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
             _ = webView.ExecuteScriptAsync($@"window.find(""{searchBox.Text}"", false, true)");
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            _ = webView.ExecuteScriptAsync($@"window.find(""{searchBox.Text}"", false, false)");
        }
    }
}
