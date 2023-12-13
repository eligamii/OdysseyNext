using Microsoft.UI.Xaml.Controls;
using Odyssey.FWebView;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls.Tips
{
    public sealed partial class InformationTip : TeachingTip
    {
        public InformationTip()
        {
            this.InitializeComponent();
        }

        public void Open(WebView webView)
        {
            Subtitle = webView.Source.Host;
            var ci = webView.SecurityInformation;
            if (ci != null)
            {
                StateTextBlock.Text = ci.VisibleSecurityState.SecurityState; // ex: secure
                CertificateIssuerTextBlock.Text = ci.VisibleSecurityState.CertificateSecurityState.Issuer;

                int timestamp = ci.VisibleSecurityState.CertificateSecurityState.ValidTo;
                var dt = new DateTime(1970, 1, 1, 1, 1, 1).AddSeconds(timestamp).ToLocalTime();

                ValidToTextBlock.Text = dt.ToString("D");
            }
            else
            {

            }

            this.IsOpen = true;
        }
    }
}
