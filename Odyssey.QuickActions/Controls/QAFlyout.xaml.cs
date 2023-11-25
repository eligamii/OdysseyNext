using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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
            sender.CoreWebView2.SourceChanged += (s, a) => Variables.QAFlyoutUrl = s.Source;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Command))
            {
                QACommands.Execute(Command);
                Hide();
            }
        }
    }
}
