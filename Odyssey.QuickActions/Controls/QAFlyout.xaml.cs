using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;




namespace Odyssey.QuickActions.Controls
{
    public sealed partial class QAFlyout : Flyout
    {
        public string Command { get; set; } = string.Empty;
        public QAFlyout()
        {
            this.InitializeComponent();
        }

        private void webView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36 Edg/120.0.0.0";
            sender.CoreWebView2.SourceChanged += (s, a) => Variables.QAFlyoutUrl = s.Source;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Command))
            {
                await QACommands.Execute(Command);
                Hide();
            }
        }
    }
}
