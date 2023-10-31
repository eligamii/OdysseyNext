using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using Odyssey.Downloads.Data;
using Odyssey.Downloads.Objects;
using Odyssey.Data.Main;
using Odyssey.FWebView.Classes;
using Odyssey.FWebView.Controls;
using Odyssey.FWebView.Controls.Flyouts;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Odyssey.WebSearch.Helpers.WebSearchStringKindHelpers;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView
{
    public sealed partial class WebView : WebView2
    {
        public bool IsSelected { get; set; } = true; // For splitview
        public Tab LinkedTab { get; set; }
        public Tab ParentTab { get; set; } = null; // used when a webView is assiociated to another (login, etc)
        public Favorite LinkedFavorite { get; set; } = null;
        public bool IsPageLoading { get; private set; } = true;

        public static FrameworkElement MainDownloadElement { get; set; } // The element used to show the DownloadsFlyout
        public static FrameworkElement MainHistoryElement { get; set; }
        public static new XamlRoot XamlRoot { get; set; }
        public static Microsoft.UI.Xaml.Controls.Image MainIconElement { get; set; } // The (titlebar) element which contain the favicon
        public static FrameworkElement MainProgressElement { get; set; } // Will have its Visiblity property set to Collapsed based on if the webview is loading
        public static ProgressRing MainProgressRing { get; set; } // idem
        public static Frame MainWebViewFrame { get; set; }
        public static ListView TabsView { get; set; }

        private static DownloadsFlyout downloadsFlyout = new();
        private static HistoryFlyout historyFlyout = new();

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

        public static void OpenDownloadDialog()
        {
            downloadsFlyout.ShowAt(MainDownloadElement);
        }

        public static void OpenHistoryDialog()
        {
            historyFlyout.ShowAt(MainHistoryElement);
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

        private void WebView2_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged; // Update the text of the tabs
            sender.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested; // Custom context menus

            sender.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged; // Update the 'url' value of the Tab objects
            sender.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            sender.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted; // Update various UI things / save history

            sender.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting; // Redirect the downloads to aria2

            sender.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged; // Set the icon of the linked tab

            sender.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged; // Save history

            sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

            // Scroll events (Uppate dynamic theme)
            scrollTimer = new DispatcherTimer();
            scrollTimer.Interval = TimeSpan.FromSeconds(2);
            scrollTimer.Tick += ScrollTimer_Tick;

            // Add extensions
            AdBlocker.AdBlocker blocker = new(sender.CoreWebView2);

            // Show native tooltips instead of Edge ones
            //WebViewNativeToolTips tips = new(this);


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
            if (!string.IsNullOrWhiteSpace(sender.DocumentTitle) &&
               History.Items.First().Url != sender.Source)
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
            if(!args.DownloadOperation.Uri.StartsWith("blob"))
            {
                Downloads.Objects.DownloadItem aria2Download = new(args.DownloadOperation.Uri);
                Downloads.Data.Downloads.Items.Insert(0, aria2Download);
                args.Cancel = true;
            }
            else // The file was downloaded in another location before (mega.nz downloads)
            {
                Downloads.Objects.DownloadItem aria2Download = new(args.DownloadOperation);
                Downloads.Data.Downloads.Items.Insert(0, aria2Download);
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
            if (IsVisible)
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
        }


    }
}
