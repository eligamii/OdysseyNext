<<<<<<< Updated upstream
﻿using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
=======
﻿extern alias webview;
using webview::Microsoft.Web.WebView2.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream

namespace Odyssey.FWebView
{
    internal class WebViewEx : WebView2Ex.UI.WebView2Ex
    {
        public static Window ParentWindow { get; set; }

=======
using Odyssey.Data.Settings;
using System.Drawing;
using Windows.Graphics.Printing.Workflow;
using System.IO;
using AriaSharp;
using Windows.Storage;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Core;

namespace Odyssey.FWebView
{
    public class WebView : WebView2Ex.UI.WebView2Ex
    {
>>>>>>> Stashed changes
        public TotpLoginDetection TotpLoginDetection { get; private set; }

        // To remove
        public static Action TotpLoginDetectedAction { get; set; }
        public static Action LoginPageDetectedAction { get; set; }

<<<<<<< Updated upstream
        public string InitialUrl { get; set; }
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        public static Image MainIconElement { get; set; } // The (titlebar) element which contain the favicon
=======
        public static Microsoft.UI.Xaml.Controls.Image MainIconElement { get; set; } // The (titlebar) element which contain the favicon
>>>>>>> Stashed changes
        public static FrameworkElement MainProgressElement { get; set; } // Will have its Visiblity property set to Collapsed based on if the webview is loading
        public static ProgressBar MainProgressBar { get; set; } // idem
        public static Frame MainWebViewFrame { get; set; }
        public static ListView TabsView { get; set; }

        private static DownloadsFlyout downloadsFlyout = new();
        private static HistoryFlyout historyFlyout = new();

        private FindFlyout findFlyout;

<<<<<<< Updated upstream
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
=======
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
            var stream = await this.WebView2Runtime.CoreWebView2.GetFaviconAsync(webview::Microsoft.Web.WebView2.Core.CoreWebView2FaviconImageFormat.Png);
            BitmapImage bitmapImage = new();
            await bitmapImage.SetSourceAsync((Windows.Storage.Streams.IRandomAccessStream)stream);

            return bitmapImage;
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

        Color? lastPixel = null;
        private async void UpdateThemeWithColorChange()
        {
            while (true)
            {
                await Task.Delay(1500);
                if (IsVisible)
                {
                    /*
                    Color pixel = await WebView2AverageColorHelper.GetFirstPixelColor(this);
                    if (pixel != lastPixel)
                    {
                        DynamicTheme.UpdateDynamicTheme(this);
                    }
                    else
                    {
                        lastPixel = pixel;
                    }*/

                }
            }
        }

        private void ScrollTimer_Tick(object sender, object e)
        {
            scrollTimer.Stop();

            // Checking if the user has finished scrolling for 2 seconds
            if (isScrolling && IsVisible)
            {
                //DynamicTheme.UpdateDynamicTheme(this);
                isScrolling = false;
            }
        }





        internal string initialUrl = "about:blank";


        private DispatcherTimer scrollTimer;
        private bool isScrolling = false;
        public static async Task<WebView> Create(string url = "about:blank")
        {
            WebView newWebView = new();
            newWebView.initialUrl = url;
            
            newWebView.WebView2Runtime = await WebView2Runtime.CreateAsync(
            await WebView2Environment.CreateAsync(),
            (nint)Window.AppWindow.Id.Value
            );

            await Task.Delay(1000);
>>>>>>> Stashed changes

            return newWebView;
        }

<<<<<<< Updated upstream
        private async void WebViewEx_Loaded(object sender, RoutedEventArgs e)
        {
=======
        public WebView() 
        {
            Loaded += WebViewEx_Loaded;
        }

        internal TaskCompletionSource<bool> task = new();
        private async void WebViewEx_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

            task.SetResult(true);

>>>>>>> Stashed changes
            WebView2Runtime = await WebView2Runtime.CreateAsync(
            await WebView2Environment.CreateAsync(),
            (nint)Window.AppWindow.Id.Value
            );

<<<<<<< Updated upstream
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
=======
            // CoreWebView2 is initialized

            foreach(var file in await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("Extensions", CreationCollisionOption.OpenIfExists)).GetFoldersAsync())
            {
                try
                {
                    await this.WebView2Runtime.CoreWebView2.Profile.AddBrowserExtensionAsync(file.Path);
                }catch { }
            }

            WebView2Runtime.CoreWebView2!.Navigate(initialUrl);

            var core = this.WebView2Runtime.CoreWebView2;

            core.DocumentTitleChanged += Core_DocumentTitleChanged;
            core.SourceChanged += Core_SourceChanged;
            core.NavigationStarting += Core_NavigationStarting;
            core.NewWindowRequested += Core_NewWindowRequested;
            core.FaviconChanged += Core_FaviconChanged;
            core.NavigationCompleted += Core_NavigationCompleted;

            // Scroll events (Uppate dynamic theme)
            scrollTimer = new DispatcherTimer();
            scrollTimer.Interval = TimeSpan.FromSeconds(2);
            scrollTimer.Tick += ScrollTimer_Tick;


            // Loading cycle
            core.NavigationStarting += (s, a) => { if (IsVisible) MainProgressBar.Value = 0; };
            core.SourceChanged += (s, a) => { if (IsVisible) MainProgressBar.Value = 1f / 6f * 100f; };
            core.ContentLoading += (s, a) => { if (IsVisible) MainProgressBar.Value = 1f / 3f * 100f; };
            core.HistoryChanged += (s, a) => { if (IsVisible) MainProgressBar.Value = 1f / 2f * 100f; };
            core.DOMContentLoaded += (s, a) => { if (IsVisible) MainProgressBar.Value = 7f / 8f * 100f; };
            core.NavigationCompleted += async (s, a) => { if (IsVisible) { MainProgressBar.Value = 100; await Task.Delay(1000); MainProgressBar.Value = 0; } };

            core.PermissionRequested += Core_PermissionRequested;

            core.DownloadStarting += Core_DownloadStarting;// Redirect the downloads to aria2
            core.ContextMenuRequested += Core_ContextMenuRequested; // Custom context menus

            // Save things only when private mode is disabled
            if (!IsPrivateModeEnabled)
            {
                core.HistoryChanged += Core_HistoryChanged; // Save history
            }
            else
            {
                // [TODO]
            }


            UpdateThemeWithColorChange();
        }

        private void Core_HistoryChanged(object sender, object e)
        {
            throw new NotImplementedException();
        }

        private void Core_ContextMenuRequested(object sender, webview::Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void Core_DownloadStarting(object sender, webview::Microsoft.Web.WebView2.Core.CoreWebView2DownloadStartingEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileInfo resultPath = new(e.DownloadOperation.ResultFilePath);

            if(resultPath.Extension == ".crx") // Extension
            {
                e.Cancel = true; // Will use Aria2 instead

                string extensionId = resultPath.Name.Split("_")[0];
                string url = $"https://clients2.google.com/service/update2/crx?response=redirect&os=linux&arch=x64&os_arch=x86_64&nacl_arch=x86-64&prod=chromium&prodchannel=unknown&prodversion=91.0.4442.4&lang=en-US&acceptformat=crx2,crx3&x=id%3D{extensionId}%26installsource%3Dondemand%26uc";

                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DownloadedExtensions", CreationCollisionOption.OpenIfExists);
                var extensionsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Extensions", CreationCollisionOption.OpenIfExists);

                var extensionFolder = await extensionsFolder.CreateFolderAsync(extensionId, CreationCollisionOption.OpenIfExists);

                AriaDownloadOperation download = AriaSharpDownloader.Download(url, folder.Path, extensionId);

                download.DownloadProgressChanged += async (dl, args) =>
                {
                    if (args.Status == AriaDownloadOperation.Status.Completed)
                    {
                        string file = Path.Combine(folder.Path, extensionId);
                        ExtractZipFile(file, string.Empty, extensionFolder.Path);

                        await Task.Delay(5000);

                        this.WebView2Runtime.CoreWebView2.Profile.AddBrowserExtensionAsync(extensionFolder.Path);
                    }
                };

            }

        }

        public void ExtractZipFile(string archivePath, string password, string outFolder)
        {

            using (var fsInput = File.OpenRead(archivePath))
            using (var zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(fsInput))
            {

                if (!String.IsNullOrEmpty(password))
                {
                    // AES encrypted entries are handled automatically
                    zf.Password = password;
                }

                foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        // Ignore directories
                        continue;
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:
                    //entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here
                    // to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    // Manipulate the output filename here as desired.
                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    // 4K is optimum
                    var buffer = new byte[4096];

                    // Unzip file in buffered chunks. This is just as fast as unpacking
                    // to a buffer the full size of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (var zipStream = zf.GetInputStream(zipEntry))
                    using (Stream fsOutput = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, fsOutput, buffer);
                    }
                }
            }
        }

        private void Core_PermissionRequested(object sender, webview::Microsoft.Web.WebView2.Core.CoreWebView2PermissionRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Core_NavigationCompleted(object sender, webview::Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
>>>>>>> Stashed changes
        {
            throw new NotImplementedException();
        }

        private void Core_FaviconChanged(object sender, object e)
        {
            throw new NotImplementedException();
        }

<<<<<<< Updated upstream
        private void Core_NewWindowRequested(object sender, object e)
=======
        private void Core_NewWindowRequested(object sender, webview::Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
>>>>>>> Stashed changes
        {
            throw new NotImplementedException();
        }

<<<<<<< Updated upstream
        private void Core_NavigationCompleted(object sender, object e)
=======
        private void Core_NavigationStarting(object sender, webview::Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
>>>>>>> Stashed changes
        {
            throw new NotImplementedException();
        }

<<<<<<< Updated upstream
        private void Core_NavigationStarting(object sender, object e)
        {
            throw new NotImplementedException();
        }

        private void Core_SourceChanged(object sender, object e)
=======
        private void Core_SourceChanged(object sender, webview::Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
>>>>>>> Stashed changes
        {
            throw new NotImplementedException();
        }

        private void Core_DocumentTitleChanged(object sender, object e)
        {
            throw new NotImplementedException();
        }
    }
}
