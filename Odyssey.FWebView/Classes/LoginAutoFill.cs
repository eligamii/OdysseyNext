using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using Odyssey.Data.Main;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private bool _loginPage = false; 
        public LoginAutoFill(WebView webView)
        {
            _webView = webView;

            webView.CoreWebView2.NavigationCompleted += (s, a) => DetectLoginPage();
            webView.CoreWebView2.FrameNavigationCompleted += (s, a) => DetectLoginPage();
            webView.CoreWebView2.SourceChanged += (s, a) => DetectLoginPage();
            webView.CoreWebView2.NavigationStarting += (s, a) => TrySaveInfo();
            webView.CoreWebView2.FrameNavigationStarting += (s, a) => TrySaveInfo();

        }

        private async void TrySaveInfo()
        {
            if (_webView == null) return;
            await _webView.EnsureCoreWebView2Async();
            CoreWebView2 core = _webView.CoreWebView2;

            string login = await core.ExecuteScriptAsync("login.value");
            string pass = await core.ExecuteScriptAsync("pass.value");

            login = login.Substring(1, login.Length - 2);
            pass = pass.Substring(1, pass.Length - 2);



        }

        private async void DetectLoginPage()
        {
            try
            {
                if (_webView == null) return;

                await _webView.EnsureCoreWebView2Async();

                CoreWebView2 core = _webView.CoreWebView2;

                await core.ExecuteScriptAsync(@"var login; var pass"); // has a lot of chances to fail but it's not an issue

                string login = await core.ExecuteScriptAsync("login = document.querySelector('input[autocomplete=\"username\"]'); login");
                string pass = await core.ExecuteScriptAsync("pass = document.querySelector('input[type=\"password\"]'); pass");

                _loginPage = login == "null" && pass == "null";
            }
            catch { }
        }

        public async void Autofill(Login login)
        {
            await _webView.CoreWebView2.ExecuteScriptAsync(@"var login; var pass"); // has a lot of chances to fail but it's not an issue

            // Get the login (email) and password inputs
            await _webView.CoreWebView2.ExecuteScriptAsync("login = document.querySelector('input[type=\"username\"]')");
            await _webView.CoreWebView2.ExecuteScriptAsync("pass = document.querySelector('input[type=\"password\"]')");

            // Set the text of each input
            await _webView.CoreWebView2.ExecuteScriptAsync($"login.value = \"{login.Username}\";");
            await _webView.CoreWebView2.ExecuteScriptAsync($"pass.value = \"{login.Password}\";");
        }


        /*
         * if (containsEmail != "false" || containsPassword != "false")
                    {
                        List<Login> logins = Logins.Items.Where(p => p.Host.Contains(_webView.Source.Host)).ToList();
                        LoginPageDetectedChanged(_webView, new LoginPageDetectedChangedEventArgs(logins, true));
                        _webView.IsLoginPageDetected = true;
                        _webView.AvailableLoginsForPage = logins;
                        return;
                    }

                    var args = new LoginPageDetectedChangedEventArgs(new List<Login>(), false);
                    LoginPageDetectedChanged(_webView, args);
                    _webView.IsLoginPageDetected = false;
                    _webView.AvailableLoginsForPage = null;*/
    }
}
