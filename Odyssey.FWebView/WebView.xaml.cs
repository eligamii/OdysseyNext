using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Odyssey.Aria2.Objects;
using Odyssey.Data.Main;
using Odyssey.Data.Settings;
using Odyssey.FWebView.Classes;
using Odyssey.FWebView.Controls;
using Odyssey.FWebView.Controls.Flyouts;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.WebSearch.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using static Odyssey.WebSearch.Helpers.WebSearchStringKindHelpers;

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
        public static new XamlRoot XamlRoot { get; set; }
        public static Image MainIconElement { get; set; } // The (titlebar) element which contain the favicon
        public static FrameworkElement MainProgressElement { get; set; } // Will have its Visiblity property set to Collapsed based on if the webview is loading
        public static ProgressRing MainProgressRing { get; set; }

        private static DownloadsFlyout downloadsFlyout = new();

        // Ojects to know if the user finished scrolling for 2s
        private DispatcherTimer scrollTimer;
        private bool isScrolling = false;
        public static WebView New(string url = "about:blank")
        {
            WebView newWebView = new();
            newWebView.Source = new Uri(url);

            return newWebView;
        }

        public bool IsVisible
        {
            get
            {
                try
                {
                    var parent = VisualTreeHelper.GetParent(this);
                    parent = VisualTreeHelper.GetParent(parent);

                    return parent.GetType() == typeof(Frame);
                } catch { return false;}
            }
        }

        public static void OpenDownloadDialog()
        {
            downloadsFlyout.ShowAt(MainDownloadElement);
        }

        private  async Task<BitmapImage> GetFaviconAsBitmapImageAsync()
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

            sender.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting; // Redirect any download to aria2

            sender.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged; // Set the icon of the linked tab

            // Scroll events (Uppate dynamic theme)
            scrollTimer = new DispatcherTimer();
            scrollTimer.Interval = TimeSpan.FromSeconds(2);
            scrollTimer.Tick += ScrollTimer_Tick;

            // Add extensions
            AdBlocker.AdBlocker blocker = new(sender.CoreWebView2);
        }

        private async void CoreWebView2_FaviconChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, object args)
        {
            // Getting the favicon from the webView
            var bitmapImage = await GetFaviconAsBitmapImageAsync();

            LinkedTab.ImageSource = bitmapImage;

            if(IsVisible && !IsPageLoading)
            {
                MainIconElement.Source = bitmapImage;
            }
        }

        private void CoreWebView2_DownloadStarting(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2DownloadStartingEventArgs args)
        {
            Aria2Download aria2Download = new(args.DownloadOperation.Uri);
            downloadsFlyout.DownloadItemsListView.Items.Add(aria2Download);
            args.Cancel = true;

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

            if(await GetStringKind(args.Uri) == StringKind.ExternalAppUri)
            {
                args.Cancel = true;
                AppUriLaunch.Launch(new Uri(args.Uri));
            }

            Tabs.Save();
            if(IsVisible)
            {
                MainIconElement.Source = null; // will hide the element
                MainProgressElement.Visibility = Visibility.Visible;
            }
           

        }

        private void CoreWebView2_ContextMenuRequested(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs args)
        {
            FWebViewContextMenu fWebViewContextMenu = new();
            fWebViewContextMenu.Show(this, args);
        }

        private void CoreWebView2_DocumentTitleChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, object args)
        {
            LinkedTab.Title = sender.DocumentTitle;
        }

        
    }
}
