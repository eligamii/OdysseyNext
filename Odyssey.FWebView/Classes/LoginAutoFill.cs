extern alias webview;
using Microsoft.UI.Xaml.Controls;
using webview::Microsoft.Web.WebView2.Core;
using Odyssey.Data.Main;
using Odyssey.Shared.ViewModels.Data;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Odyssey.FWebView.Classes
{
    public class LoginPageDetectedChangedEventArgs
    {
        public LoginPageDetectedChangedEventArgs(List<Login> availableLogins, bool detected)
        {
            AvailableLogins = availableLogins;
            LoginPageDetected = detected;
        }
        public bool LoginPageDetected { get; set; } = false;
        public List<Login> AvailableLogins { get; set; } = new();
    }
   

    public class LoginAutoFill
    {
        public delegate void LoginPageDetectedChangedEventHandler(WebView sender, LoginPageDetectedChangedEventArgs args);

        public event LoginPageDetectedChangedEventHandler LoginPageDetectedChanged;

        private WebView _webView;
        public LoginAutoFill(WebView webView)
        {
            _webView = webView;

            webView.WebView2Runtime.CoreWebView2.NavigationCompleted += (s, a) => DetectLoginPage();
            webView.KeyDownListener.KeyDown += (s, a) => DetectLoginPage();
            webView.WebView2Runtime.CoreWebView2.FrameNavigationCompleted += (s, a) => DetectLoginPage();
            webView.WebView2Runtime.CoreWebView2.SourceChanged += (s, a) => DetectLoginPage();
        }


        private async void DetectLoginPage()
        {
            try
            {
                if (_webView != null)
                {
                    CoreWebView2 core = _webView.WebView2Runtime.CoreWebView2;

                    // Simple filter to not test every single page
                    string containsEmail = await core.ExecuteScriptAsync("document.querySelector('input[autocomplete=\"username\"]') != null;");
                    string containsPassword = await core.ExecuteScriptAsync("document.querySelector('input[type=\"password\"]') != null;");

                    if (containsEmail != "false" || containsPassword != "false")
                    {
                        List<Login> logins = Logins.Items.Where(p => p.Host.Contains(new Uri(_webView.WebView2Runtime.CoreWebView2.Source).Host)).ToList();
                        LoginPageDetectedChanged(_webView, new LoginPageDetectedChangedEventArgs(logins, true));
                        _webView.IsLoginPageDetected = true;
                        _webView.AvailableLoginsForPage = logins;
                        return;
                    }

                    var args = new LoginPageDetectedChangedEventArgs(new List<Login>(), false);
                    LoginPageDetectedChanged(_webView, args);
                    _webView.IsLoginPageDetected = false;
                    _webView.AvailableLoginsForPage = null;
                }
            }
            catch { }
        }

        public async void Autofill(Login login)
        {
            await _webView.WebView2Runtime.CoreWebView2.ExecuteScriptAsync(@"var login; var pass"); // has a lot of chances to fail but it's not an issue

            // Get the login (email) and password inputs
            await _webView.WebView2Runtime.CoreWebView2.ExecuteScriptAsync("login = document.querySelector('input[type=\"username\"]')");
            await _webView.WebView2Runtime.CoreWebView2.ExecuteScriptAsync("pass = document.querySelector('input[type=\"password\"]')");

            // Set the text of each input
            await _webView.WebView2Runtime.CoreWebView2.ExecuteScriptAsync($"login.value = \"{login.Username}\";");
            await _webView.WebView2Runtime.CoreWebView2.ExecuteScriptAsync($"pass.value = \"{login.Password}\";");
        }

    }
}
