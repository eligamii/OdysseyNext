﻿using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;

namespace Odyssey.FWebView.Helpers
{
    public class WebView2KeyDownHelpers
    {
        public class KeyDownListener // disabling accelerators key is required to capture Ctrl F for ex
        {
            public class KeyDownPressedEventArgs
            {
                public KeyDownPressedEventArgs(bool isControlKeyPressed, bool isAltKeyPressed, VirtualKey pressedKey)
                {
                    IsControlKeyPressed = isControlKeyPressed;
                    IsAltKeyPressed = isAltKeyPressed;
                    PressedKey = pressedKey;
                }

                public bool IsControlKeyPressed { get; set; }
                public bool IsAltKeyPressed { get; set; }
                public VirtualKey PressedKey { get; set; }
            }

            public delegate void KeyDownPressedEventHandler(object sender, KeyDownPressedEventArgs args);

            public event KeyDownPressedEventHandler KeyDown;



            private WebView2 sender;
            public KeyDownListener(WebView2 webView)
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

                        int rawKey = int.Parse(keys[0]);

                        VirtualKey pressedKey = ConvertToVirtualKey(rawKey);

                        KeyDown(sender, new KeyDownPressedEventArgs(isControlKeyPressed, isAltKeyPressed, pressedKey));
                    }
                    catch { }
                }
            }

            private VirtualKey ConvertToVirtualKey(int jsKey)  // WIP
            {
                if (jsKey >= 97 && jsKey <= 122) // Letter
                    return (VirtualKey)(jsKey + 64);
                else
                {
                    switch (jsKey)
                    {
                        case 32: return VirtualKey.Space;
                    }
                }

                return VirtualKey.None;
            }
        }
    }
}
