﻿extern alias webview;
using webview::Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;

namespace Odyssey.FWebView.Classes
{
    public class TotpLoginDetection
    {
        public class TotpLoginDetectedEventArgs
        {
            public TotpLoginDetectedEventArgs(string url, bool detected)
            {
                Url = url;
                Detected = detected;
            }
            public string Url { get; set; }
            public bool Detected { get; set; }
        }

        internal delegate void TotpLoginDetectedEventHandler(object sender, TotpLoginDetectedEventArgs args);

        internal event TotpLoginDetectedEventHandler TotpLoginDetected;



        private WebView webView;
        public TotpLoginDetection(WebView webView)
        {
            webView.WebView2Runtime.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            webView.WebView2Runtime.CoreWebView2.FrameNavigationCompleted += CoreWebView2_NavigationCompleted;

            this.webView = webView;

            Init(webView);
        }

        private async void Init(WebView webView)
        {
            // Create an input variable
            await webView.WebView2Runtime.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("var testInput");
        }

        private void CoreWebView2_NavigationCompleted(object sender, webview::Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            ContainTotp(webView.WebView2Runtime.CoreWebView2);
        }

        public async void ContainTotp(CoreWebView2 webView)
        {
            // Page that ask for OTP login are usually page with an input with the type 'tel' and autocomplete off
            // They can also have a input with 'otc' or 'totp' as input id/name

            // Prevent timing issues
            await Task.Delay(750);

            // Getting the input element
            await webView.ExecuteScriptAsync("testInput = document.querySelector('input[type=\"tel\"]')");

            // Test if the testInput JS variable = null
            string hasTelInput = await webView.ExecuteScriptAsync("testInput != null");

            if (bool.Parse(hasTelInput))
            {
                // Test if the autocomplete is turned off
                string autocompleteOff = await webView.ExecuteScriptAsync("testInput.autocomplete == \"off\"");

                if (bool.Parse(autocompleteOff))
                {
                    this.webView.IsTotpDetected = true;
                    TotpLoginDetected(webView, new TotpLoginDetectedEventArgs(webView.Source, true));

                    return;
                }
            }

            this.webView.IsTotpDetected = false;
            TotpLoginDetected(webView, new TotpLoginDetectedEventArgs(webView.Source, false));

        }
    }
}
