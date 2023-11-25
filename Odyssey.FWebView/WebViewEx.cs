using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Odyssey.FWebView.Classes;
using Odyssey.FWebView.Controls.Flyouts;
using Odyssey.FWebView.Helpers;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebView2Ex;

namespace Odyssey.FWebView
{
    internal class WebViewEx : WebView2Ex.UI.WebView2Ex
    {
        public static Window ParentWindow { get; set; }

        public TotpLoginDetection TotpLoginDetection { get; private set; }

        // To remove
        public static Action TotpLoginDetectedAction { get; set; }
        public static Action LoginPageDetectedAction { get; set; }

        public string InitialUrl { get; set; }
        public bool IsLoginPageDetected { get; set; }
        public List<Login> AvailableLoginsForPage { get; set; }
        public bool IsTotpDetected { get; set; } = false;
        public bool IsSelected { get; set; } = true; // For splitview
        public Tab LinkedTab { get; set; } = new(); // Initialize to prevent issues with LittleWeb
        public Tab ParentTab { get; set; } = null; // used when a webView is assiociated to another (login, etc)
        public Favorite LinkedFavorite { get; set; } = null;
        public bool IsPageLoading { get; private set; } = true;
        public bool IsLittleWeb { get; set; } = false;
        public bool IsPrivateModeEnabled { get; set; } = false;
        public DarkReader DarkReader { get; set; }
        public SecurityInformation SecurityInformation { get; set; } = null;
        public WebView2KeyDownHelpers.KeyDownListener KeyDownListener { get; set; }
        public LoginAutoFill LoginAutoFill { get; set; }
        public static TextBlock DocumentTextBlock { get; set; }
        public static FrameworkElement MainDownloadElement { get; set; } // The element used to show the DownloadsFlyout
        public static FrameworkElement MainHistoryElement { get; set; }
        public static new XamlRoot XamlRoot { get; set; }
        public static Image MainIconElement { get; set; } // The (titlebar) element which contain the favicon
        public static FrameworkElement MainProgressElement { get; set; } // Will have its Visiblity property set to Collapsed based on if the webview is loading
        public static ProgressBar MainProgressBar { get; set; } // idem
        public static Frame MainWebViewFrame { get; set; }
        public static ListView TabsView { get; set; }

        private static DownloadsFlyout downloadsFlyout = new();
        private static HistoryFlyout historyFlyout = new();

        private FindFlyout findFlyout;

        private DispatcherTimer scrollTimer;
        private bool isScrolling = false;

        public WebViewEx()
        {
            Loaded += WebViewEx_Loaded;
        }

        public static WebViewEx Create(string url = "about:blank")
        {
            WebViewEx newWebView = new();
            newWebView.InitialUrl = url;

            return newWebView;
        }

        private async void WebViewEx_Loaded(object sender, RoutedEventArgs e)
        {
            WebView2Runtime = await WebView2Runtime.CreateAsync(
            await WebView2Environment.CreateAsync(),
            (nint)Window.AppWindow.Id.Value
            );

            // Assuming the CoreWebView2Initialized event happens here
            WebView2Runtime.CoreWebView2!.Navigate(InitialUrl);
            
            if(!IsLittleWeb)
            {
                var core = WebView2Runtime.CoreWebView2;

                core.DocumentTitleChanged += Core_DocumentTitleChanged;
                core.SourceChanged += Core_SourceChanged;
                core.NavigationStarting += Core_NavigationStarting;
                core.NavigationCompleted += Core_NavigationCompleted;
                core.NewWindowRequested += Core_NewWindowRequested;
                core.FaviconChanged += Core_FaviconChanged;

                // Scroll events (Uppate dynamic theme)
                scrollTimer = new DispatcherTimer();
                scrollTimer.Interval = TimeSpan.FromSeconds(2);
                scrollTimer.Tick += ScrollTimer_Tick; ;

            }

            UpdateThemeWithColorChange();
        }

        private void ScrollTimer_Tick(object sender, object e)
        {
            throw new NotImplementedException();
        }

        private void Core_FaviconChanged(object sender, object e)
        {
            throw new NotImplementedException();
        }

        private void Core_NewWindowRequested(object sender, object e)
        {
            throw new NotImplementedException();
        }

        private void Core_NavigationCompleted(object sender, object e)
        {
            throw new NotImplementedException();
        }

        private void Core_NavigationStarting(object sender, object e)
        {
            throw new NotImplementedException();
        }

        private void Core_SourceChanged(object sender, object e)
        {
            throw new NotImplementedException();
        }

        private void Core_DocumentTitleChanged(object sender, object e)
        {
            throw new NotImplementedException();
        }
    }
}
