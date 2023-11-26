extern alias webview;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using webview::Microsoft.Web.WebView2.Core;

namespace Odyssey.FWebView.Classes
{
    internal class WebViewNativeToolTips
    {
        private Point lastPointerPos;
        private WebView webView;
        private List<string[]> elements = new();

        private List<ToolTip> toolTips = new();
        public WebViewNativeToolTips(WebView webView)
        {
            this.webView = webView;

            InitializeVariables();

            webView.WebView2Runtime.CoreWebView2.NavigationStarting += (s, a) =>
            {
                elements.Clear();

                foreach (var tip in toolTips)
                {
                    tip.IsOpen = false;
                }

                toolTips.Clear();
            };

            webView.WebView2Runtime.CoreWebView2.NavigationCompleted += (s, a) => GetElements();
            webView.WebView2Runtime.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            webView.PointerMoved += (s, a) => lastPointerPos = a.GetCurrentPoint(webView).Position;
        }

        private async void InitializeVariables()
        {
            await webView.WebView2Runtime.CoreWebView2.ExecuteScriptAsync("var links; var links2");
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            string message = args.WebMessageAsJson;

            string[] pairs = message.Split("{////}"); // impossible to have "{////}" on any website so used as a separator

            try
            {
                if (pairs[1] != "in\"" && pairs[1] != "out\"")
                {
                    elements.Add(pairs);
                }
                else if (pairs[1] == "in\"")
                {
                    var element = elements.Where(p => p[0] == pairs[0]).ToList().First();
                    ToolTip toolTip = new ToolTip();
                    toolTip.Content = element[1];

                    int left = int.Parse(element[2]);
                    int top = int.Parse(element[3].Replace("\"", "")); // as the base WebMessage is in the "message" (with quotes) format

                    ToolTipService.SetToolTip(webView, toolTip);
                    toolTip.PlacementRect = new Rect(left, top, 0, 0);
                    toolTip.IsOpen = true;

                    toolTips.Add(toolTip);
                }
                else if (pairs[1] == "out\"")
                {
                    foreach (var tip in toolTips)
                    {
                        tip.IsOpen = false;
                    }

                    toolTips.Clear();
                }
            }
            catch { }

        }


        public async void GetElements()
        {
            string script = @"document.querySelectorAll('a[title]').forEach(element => {  var rect = element.getBoundingClientRect(); window.chrome.webview.postMessage(element + ""{////}"" + element.title + ""{////}"" + rect.left + ""{////}"" + rect.top); element.title = """"; });";
            await webView.WebView2Runtime.CoreWebView2.ExecuteScriptAsync(script);

            string eventScript = @"links = document.querySelectorAll('a[title]');
links.forEach(link => {
    link.addEventListener('mouseover', (event) => {
        window.chrome.webview.postMessage(link + ""{////}in"");
    });
});";

            await webView.WebView2Runtime.CoreWebView2.ExecuteScriptAsync(eventScript);

            eventScript = @"links2 = document.querySelectorAll('a[title]');
links2.forEach(link => {
    link.addEventListener('mouseout', (event) => {
        window.chrome.webview.postMessage(link + ""{////}out"");
    });
});";
            await webView.WebView2Runtime.CoreWebView2.ExecuteScriptAsync(eventScript);
        }
    }
}
