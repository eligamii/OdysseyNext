using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Odyssey.Data.Main;
using Odyssey.Data.Settings;
using Odyssey.FWebView.Classes;
using Odyssey.FWebView.Controls;
using Odyssey.FWebView.Controls.Flyouts;
using Odyssey.FWebView.Helpers;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using static Odyssey.WebSearch.Helpers.WebSearchStringKindHelpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView
{
    public sealed partial class WebView : WebView2
    {
        public TotpLoginDetection TotpLoginDetection { get; private set; }

        // To remove
        public static Action TotpLoginDetectedAction { get; set; }
        public static Action LoginPageDetectedAction { get; set; }

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

        public static WebView SelectedWebView // Used for opening HistoryItems in the selected webview when available
        {
            get
            {
                if (MainWebViewFrame.Content != null)
                {
                    return MainWebViewFrame.Content as WebView;

                }
                else
                {
                    return null;
                }
            }
        }


        // Ojects to know if the user finished scrolling for 2s
        private DispatcherTimer scrollTimer;
        private bool isScrolling = false;
        public static WebView Create(string url = "about:blank")
        {
            WebView newWebView = new();
            newWebView.Source = new Uri(url);

            return newWebView;
        }


        public bool IsVisible
        {
            get
            {
                var parent = VisualTreeHelper.GetParent(this);
                if (parent != null)
                {
                    parent = VisualTreeHelper.GetParent(parent);
                    return parent.GetType() == typeof(Frame);
                }
                else
                {
                    return false;
                }
            }
        }

        public static void OpenDownloadDialog(FrameworkElement element = null)
        {
            FrameworkElement fElement = element == null ? MainDownloadElement : element;
            downloadsFlyout.ShowAt(fElement);
        }

        public static void OpenHistoryDialog(FrameworkElement element = null)
        {
            FrameworkElement fElement = element == null ? MainHistoryElement : element;
            historyFlyout.ShowAt(fElement);
        }

        private async Task<BitmapImage> GetFaviconAsBitmapImageAsync()
        {
            // Getting the favicon from the webView
            var stream = await this.CoreWebView2.GetFaviconAsync(CoreWebView2FaviconImageFormat.Png);
            BitmapImage bitmapImage = new();
            await bitmapImage.SetSourceAsync(stream);

            return bitmapImage;
        }

        public WebView()
        {
            this.InitializeComponent();
        }

        private async void WebView2_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            if (!IsLittleWeb)
            {
                sender.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged; // Update the text of the tabs

                sender.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged; // Update the 'url' value of the Tab objects
                sender.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
                sender.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged; // Set the icon of the linked tab
                sender.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted; // Update various UI things / save history

                // Scroll events (Uppate dynamic theme)
                scrollTimer = new DispatcherTimer();
                scrollTimer.Interval = TimeSpan.FromSeconds(2);
                scrollTimer.Tick += ScrollTimer_Tick;


                // Loading cycle
                sender.CoreWebView2.NavigationStarting += (s, a) => { if (IsVisible) MainProgressBar.Value = 0; };
                sender.CoreWebView2.SourceChanged += (s, a) => { if (IsVisible) MainProgressBar.Value = 1f/6f * 100f; };
                sender.CoreWebView2.ContentLoading += (s, a) => { if (IsVisible) MainProgressBar.Value = 1f/3f * 100f; };
                sender.CoreWebView2.HistoryChanged += (s, a) => { if (IsVisible) MainProgressBar.Value = 1f/2f * 100f; };
                sender.CoreWebView2.DOMContentLoaded += (s, a) => { if (IsVisible) MainProgressBar.Value = 7f/8f * 100f; };
                sender.CoreWebView2.NavigationCompleted += async (s, a) => { if (IsVisible) { MainProgressBar.Value = 100; await Task.Delay(1000); MainProgressBar.Value = 0; } };
            }

            sender.CoreWebView2.PermissionRequested += CoreWebView2_PermissionRequested;

            sender.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting; // Redirect the downloads to aria2
            sender.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested; // Custom context menus

            // Add extensions
            AdBlocker.AdBlocker blocker = new(sender.CoreWebView2);

            // Show native tooltips instead of Edge ones (disabled for now)
            //WebViewNativeToolTips tips = new(this);

            // Disable default keyboard accelerators keys to add custom find,... dialogs
            sender.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;

            // Save things only when private mode is disabled
            if (!IsPrivateModeEnabled)
            {
                sender.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged; // Save history
            }
            else
            {
                // [TODO]
            }

            findFlyout = new(this);

            // Using a custom KeyDown event as the default one doesnt work
            KeyDownListener = new(this);
            KeyDownListener.KeyDown += WebView_KeyDown;

            TotpLoginDetection = new(this);
            TotpLoginDetection.TotpLoginDetected += (s, a) => TotpLoginDetectedAction();

            LoginAutoFill = new(this);
            LoginAutoFill.LoginPageDetectedChanged += (s, a) => LoginPageDetectedAction();

            DarkReader darkReader = new(this);
            if (Settings.IsDarkReaderEnabled != false) // == null or true
            {
                darkReader.Auto(true);

                if (Settings.ForceDarkReader != false)
                {
                    darkReader.Enable();
                }
            }
            sender.CoreWebView2.Settings.AreDevToolsEnabled = true;
            // Add CDP event to get certificate info
            try // it seems to has some chances to fail and idk why
            {
                // It makes the app very unstable 
                /*
                
                await sender.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.enable", "{}");
                await sender.CoreWebView2.CallDevToolsProtocolMethodAsync("Security.enable", "{}");

                var res = sender.CoreWebView2.GetDevToolsProtocolEventReceiver("Security.visibleSecurityStateChanged");
                res.DevToolsProtocolEventReceived += DevToolsProtocolEventReceived;
                */
            }
            catch { }

            UpdateThemeWithColorChange();
        }

        Color? lastPixel = null; 
        private async void UpdateThemeWithColorChange()
        {
            while(true)
            {
                await Task.Delay(1500);
                if(IsVisible)
                {
                    Color pixel = await WebView2AverageColorHelper.GetFirstPixelColor(this);
                    if(pixel != lastPixel)
                    {
                        DynamicTheme.UpdateDynamicTheme(this);
                    }
                    else
                    {
                        lastPixel = pixel;
                    }

                }
            }
        }

        private void DevToolsProtocolEventReceived(CoreWebView2 sender, CoreWebView2DevToolsProtocolEventReceivedEventArgs args)
        {
            // memory access violation
            /*
            string json = args.ParameterObjectAsJson;
            SecurityInformation = JsonConvert.DeserializeObject<SecurityInformation>(json);
            */
        }

        private void CoreWebView2_PermissionRequested(CoreWebView2 sender, CoreWebView2PermissionRequestedEventArgs args)
        {
            // TODO
        }

        private void WebView_KeyDown(object sender, WebView2KeyDownHelpers.KeyDownListener.KeyDownPressedEventArgs args)
        {
            if (args.IsControlKeyPressed)
            {
                switch (args.PressedKey)
                {
                    case Windows.System.VirtualKey.F:
                        FindFlyout findFlyout = new(this);
                        findFlyout.PreferredPlacement = TeachingTipPlacementMode.Top;
                        findFlyout.XamlRoot = XamlRoot;
                        findFlyout.IsOpen = true;
                        break;
                }
            }
        }

        private async void CoreWebView2_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();

            WebView webView = new();
            await webView.EnsureCoreWebView2Async();

            webView.ParentTab = LinkedTab;
            args.NewWindow = webView.CoreWebView2;

            Tab tab = new()
            {
                MainWebView = webView,
                Title = webView.CoreWebView2.Source,
                Url = webView.CoreWebView2.Source,
                ToolTip = webView.CoreWebView2.Source
            };

            webView.LinkedTab = tab;

            Tabs.Items.Add(tab);
            TabsView.SelectedItem = tab;

            deferral.Complete();
        }

        private void CoreWebView2_HistoryChanged(CoreWebView2 sender, object args)
        {
            string lastItem = string.Empty;
            if (History.Items.Count != 0) lastItem = History.Items.First().Url;

            if (!string.IsNullOrWhiteSpace(sender.DocumentTitle) &&
               lastItem != sender.Source)
            {
                // Save history
                HistoryItem historyItem = new()
                {
                    Title = sender.DocumentTitle,
                    Url = sender.Source,
                    Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
                };

                History.Items.Insert(0, historyItem);
            }


        }

        private async void CoreWebView2_FaviconChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, object args)
        {
            // Getting the favicon from the webView
            var bitmapImage = await GetFaviconAsBitmapImageAsync();

            LinkedTab.ImageSource = bitmapImage;

            if (IsVisible && !IsPageLoading)
            {
                MainIconElement.Source = bitmapImage;
            }
        }

        private void CoreWebView2_DownloadStarting(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2DownloadStartingEventArgs args)
        {
            if (!args.DownloadOperation.Uri.StartsWith("blob"))
            {
                DownloadsFlyout.Items.Insert(0, new DonwloadItem { DownloadUrl = args.DownloadOperation.Uri });
                args.Cancel = true;
            }
            else // The file was downloaded in another location before (mega.nz downloads)
            {
                DownloadsFlyout.Items.Insert(0, new DonwloadItem { downloadOperation = args.DownloadOperation });
            }

            downloadsFlyout.ShowAt(MainDownloadElement);
        }





        // Dynamic themes events
        private void ScrollTimer_Tick(object sender, object e)
        {
            scrollTimer.Stop();

            // Checking if the user has finished scrolling for 2 seconds
            if (isScrolling && IsVisible)
            {
                DynamicTheme.UpdateDynamicTheme(this);
                isScrolling = false;
            }
        }

        private void WebView2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsVisible && !IsLittleWeb)
            {
                DynamicTheme.UpdateDynamicTheme(this);
            }
        }




        private async void CoreWebView2_NavigationCompleted(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            IsPageLoading = false;

            // Getting the favicon from the webView
            var bitmapImage = await GetFaviconAsBitmapImageAsync();

            LinkedTab.ImageSource = bitmapImage;

            if (IsVisible)
            {
                MainProgressElement.Visibility = Visibility.Collapsed;
                MainIconElement.Source = bitmapImage;
                DynamicTheme.UpdateDynamicTheme(this);
            }


            Tabs.Save();

            if (sender.Source.StartsWith("edge://flags"))
            {
                // TODO: restart every webview when restart button clicked
            }
        }

        private void CoreWebView2_SourceChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs args)
        {
            LinkedTab.Url = sender.Source;
        }

        private async void CoreWebView2_NavigationStarting(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            IsPageLoading = true;
            var kind = await GetStringKind(args.Uri);
            if (kind == StringKind.ExternalAppUri)
            {
                args.Cancel = true;
                AppUriLaunch.Launch(new Uri(args.Uri));
            }
            else if (kind == StringKind.OdysseyUrl)
            {
                if (Regex.IsMatch(args.Uri, ".*/downloads/{0,1}.*", RegexOptions.IgnoreCase))
                {
                    OpenDownloadDialog();
                    args.Cancel = true;
                }
                else if (Regex.IsMatch(args.Uri, ".*/history/{0,1}.*", RegexOptions.IgnoreCase))
                {
                    OpenHistoryDialog();
                    args.Cancel = true;
                }
            }

            Tabs.Save();
            if (IsVisible)
            {
                MainIconElement.Source = null; // will hide the element
                MainProgressElement.Visibility = Visibility.Visible;
                DynamicTheme.UpdateDynamicTheme(this);
            }


        }

        private void CoreWebView2_ContextMenuRequested(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs args)
        {
            QuickActions.Variables.ContextMenuArgs = args;

            FWebViewContextMenu fWebViewContextMenu = new();
            fWebViewContextMenu.Show(this, args);
        }

        private void CoreWebView2_DocumentTitleChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, object args)
        {
            LinkedTab.Title = sender.DocumentTitle;
            if(IsVisible)
            {
                DocumentTextBlock.Text = sender.DocumentTitle;
            }
        }


    }
}
