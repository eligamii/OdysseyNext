using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;

namespace Odyssey.FWebView.Helpers
{
    internal class WebView2KeyDownHelpers
    {
        internal class KeyDownListener // disabling accelerators key is required to capture Ctrl F for ex
        {
            internal class KeyDownPressedEventArgs
            {
                internal KeyDownPressedEventArgs(bool isControlKeyPressed, bool isAltKeyPressed, VirtualKey pressedKey)
                {
                    IsControlKeyPressed = isControlKeyPressed;
                    IsAltKeyPressed = isAltKeyPressed;
                    PressedKey = pressedKey;
                }

                internal bool IsControlKeyPressed { get; set; }
                internal bool IsAltKeyPressed { get; set; }
                internal VirtualKey PressedKey { get; set; }
            }

            internal delegate void KeyDownPressedEventHandler(object sender, KeyDownPressedEventArgs args);

            internal event KeyDownPressedEventHandler KeyDown;



            private WebView2 sender;
            internal KeyDownListener(WebView2 webView)
            {
                sender = webView;
                Init(webView);
            }

            private async void Init(WebView2 webView)
            {
                await webView.EnsureCoreWebView2Async();
                await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.addEventListener(\"keypress\" ,onkeypress); function onkeypress(evt) {window.chrome.webview.postMessage(\"[KEYPRESS]\" + evt.keyCode + \" \" + evt.ctrlKey + \" \" + evt.altKey);}");

                webView.WebMessageReceived += WebView_WebMessageReceived;

            }

            private void WebView_WebMessageReceived(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
            {
                string message = args.WebMessageAsJson.Replace("\"", "");

                // Ensure that the message is sent because of a keypress event
                if (message.StartsWith("[KEYPRESS]"))
                {
                    message = message.Replace("[KEYPRESS]", "");
                    string[] keys = message.Split(' '); // 0 = key code, 1 = ctrl (bool), 2 = alt (bool)

                    try
                    {
                        bool isControlKeyPressed = bool.Parse(keys[1]);
                        bool isAltKeyPressed = bool.Parse(keys[2]);

                        VirtualKey pressedKey = (VirtualKey)(int.Parse(keys[0]) + 64);

                        KeyDown(sender, new KeyDownPressedEventArgs(isControlKeyPressed, isAltKeyPressed, pressedKey));
                    }
                    catch { }
                }
            }
        }
    }
}
