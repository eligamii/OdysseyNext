using ABI.Windows.Foundation;
using CommunityToolkit.WinUI.UI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Web.WebView2.Core;
using Odyssey.Classes;
using Odyssey.Controls;
using Odyssey.Controls.Tips;
using Odyssey.Data.Main;
using Odyssey.Data.Settings;
using Odyssey.Dialogs;
using Odyssey.FWebView;
using Odyssey.FWebView.Classes;
using Odyssey.Helpers;
using Odyssey.OtherWindows;
using Odyssey.QuickActions;
using Odyssey.TwoFactorsAuthentification;
using Odyssey.Views.Pages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using WinRT.Interop;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.UI.Xaml.Application;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        public static MainView Current { get; set; }
        public TitleBarDragRegions titleBarDragRegions;
        public string MainDocumentTitle { get { return documentTitle.Text; } 
            set 
            { 
                if(value != documentTitle.Text)
                {
                    documentTitle.Text = value;
                    titleBarDragRegions.SetDragRegionForTitleBars();
                } 
            }
        }
        public static WebView CurrentlySelectedWebView
        {
            get { return Current.splitViewContentFrame.Content as WebView; }
        }
        public MainView()
        {
            this.InitializeComponent();

            Loaded += MainView_Loaded;

            

            Current = this;
        }


        private void RestoreTabs()
        {
            // Get the instances of the app to prevent tabs from restoring on anther instances
            // when an instance has opened tabs (so when Settings.SuccessfullyClosed == false with an instance opened)
            var instances = Microsoft.Windows.AppLifecycle.AppInstance.GetInstances();

            if (Settings.SuccessfullyClosed == false && instances.Count == 1)
            {
                PaneView.Current.TabsView.ItemsSource = Tabs.Restore();
            }

            Settings.SuccessfullyClosed = false;
        }

        private async void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            titleBarDragRegions = new TitleBarDragRegions(
                new List<Grid>() { AppTitleBar, secondTitleBar },
                MainWindow.Current,
                new List<Type>() { typeof(ProgressBar), typeof(TextBlock), typeof(Microsoft.UI.Xaml.Shapes.Rectangle), typeof(Frame) },
                MainWindow.Current.Content as FrameworkElement,
                42);

            if (Settings.FirstLaunch != false)
            {
                QuickConfigurationDialog quickConfigurationDialog = new()
                {
                    XamlRoot = MainWindow.Current.Content.XamlRoot
                };

                await quickConfigurationDialog.ShowAsync();
            }

            // Use this instead of Systemackdrop = MicaBackdrop(); to be able to control the color of the Window
            MicaBackdropHelper.TrySetMicaBackdropTo(MainWindow.Current);

            WebView.XamlRoot = XamlRoot;
            DynamicTheme.PageToUpdateTheme = this;
            DynamicTheme.MicaController = MicaBackdropHelper.BackdropController;
            DynamicTheme.AppWindowTitleBar = MainWindow.Current.AppWindow.TitleBar;
            DynamicTheme.TitleBar = AppTitleBar;
            DynamicTheme.UpdateTheme = true;
            DynamicTheme.AcrylicBrush = PaneAcrylicBrush;
            SplitViewPaneFrame.Navigate(typeof(PaneView), null, new SuppressNavigationTransitionInfo());

            // Restore tabs after crash
            RestoreTabs();

            // Require these to not crash
            WebView.MainDownloadElement = moreButton;
            WebView.MainHistoryElement = moreButton;
            WebView.MainIconElement = new Microsoft.UI.Xaml.Controls.Image();
            WebView.MainProgressBar = progressBar;
            WebView.MainProgressElement = new ProgressBar();
            WebView.MainWebViewFrame = splitViewContentFrame;
            WebView.DocumentTextBlock = documentTitle;
            WebView.UrlTextBox = urlTextBox;

            QACommands.Frame = splitViewContentFrame;

            // Set the Quick actions command MainWindow to this window
            QACommands.MainWindow = MainWindow.Current;

            WebView.TotpLoginDetectedAction += () => SetTotpButtonVisibility();
            WebView.LoginPageDetectedAction += () => LoginDetectedChanged();

            // Check the internet connection every second for UI things
            CheckNetworkConnectionState();

            // Set the custom theme if dynamic theme is not enabled
            SetCustomTheme();

            this.ActualThemeChanged += (s, a) => SetCustomTheme();
            splitViewContentFrame.Navigate(typeof(HomePage));

            // Update the titlebar drag region based on the text of the documentTitle
            documentTitle.LayoutUpdated += DocumentTitle_LayoutUpdated;
        }

        private string _lastText = string.Empty;

        private void DocumentTitle_LayoutUpdated(object sender, object e)
        {
            string text = CurrentlySelectedWebView?.CoreWebView2?.DocumentTitle;
            if(_lastText != text)
            {
                titleBarDragRegions.SetDragRegionForTitleBars();
                _lastText = text;
            }
        }

        private bool lastConnectionState;

        private void SetCustomTheme()
        {
            if (!Settings.IsDynamicThemeEnabled)
            {
                string color = Settings.CustomThemeColors;
                if(color != null)
                {
                    if (Regex.IsMatch(color, @"#[0-9A-Z]{6}")) // To remove
                    {
                        UpdateTheme.UpdateThemeWith(color);
                    }
                }
            }
        }

        public void SetTotpButtonVisibility()
        {
            /*
            if (CurrentlySelectedWebView != null)
                _2faButton.Visibility = CurrentlySelectedWebView.IsTotpDetected ? Visibility.Visible : Visibility.Collapsed;
            else
                _2faButton.Visibility = Visibility.Collapsed;*/
        }

        public void LoginDetectedChanged()
        {
            if(CurrentlySelectedWebView != null)
            {
                loginButton.Visibility = CurrentlySelectedWebView.IsLoginPageDetected ? Visibility.Visible : Visibility.Collapsed;

                if (CurrentlySelectedWebView.IsLoginPageDetected)
                {
                    MenuFlyout menu = new();
                    foreach (var item in CurrentlySelectedWebView.AvailableLoginsForPage)
                    {
                        MenuFlyoutItem menuFlyoutItem = new();
                        menuFlyoutItem.Text = item.Username;

                        menuFlyoutItem.Click += (s, a) => CurrentlySelectedWebView.LoginAutoFill.Autofill(item);
                        menu.Items.Add(menuFlyoutItem);
                    }

                    loginButton.Flyout = menu;
                }
            }
        }

        private async void CheckNetworkConnectionState()
        {
            while (true)
            {
                await Task.Delay(250);

                bool isInternetAvailable = Shared.Helpers.NetworkHelper.IsInternetConnectionAvailable();
                if (lastConnectionState != isInternetAvailable)
                {
                    if (!isInternetAvailable)
                    {
                        //progressRing.Foreground = Application.Current.Resources["FocusStrokeColorOuterBrush"] as Brush;
                    }
                    else
                    {
                        //progressRing.Foreground = Application.Current.Resources["AccentFillColorDefaultBrush"] as Brush;
                    }

                    lastConnectionState = isInternetAvailable;
                }
            }
        }



        // Show pane button
        private void SplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            showPaneButton.Visibility = Visibility.Visible;
        }

        private void SplitView_PaneOpening(SplitView sender, object args)
        {
            if (showPaneButton != null)
            {
                showPaneButton.Visibility = Visibility.Collapsed;

            }
        }




        // Titlear buttons
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            SplitView.IsPaneOpen ^= true;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentlySelectedWebView != null)
            {
                if (CurrentlySelectedWebView.CanGoBack) CurrentlySelectedWebView.GoBack();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentlySelectedWebView != null)
            {
                if (CurrentlySelectedWebView.CanGoForward) CurrentlySelectedWebView.GoForward();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentlySelectedWebView != null)
            {
                CurrentlySelectedWebView.Reload();
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBar searchBar = new SearchBar();
            FlyoutShowOptions options = new FlyoutShowOptions();
            options.Placement = FlyoutPlacementMode.Bottom;
            options.Position = new Windows.Foundation.Point(splitViewContentFrame.ActualWidth / 2, 100);

            searchBar.ShowAt(splitViewContentFrame, options);
        }

        private void DownloadsButton_Click(object sender, RoutedEventArgs e)
        {
            FWebView.WebView.OpenDownloadDialog();
        }

        private void _2FAButton_Click(object sender, RoutedEventArgs e)
        {
            TwoFactorsAuthentification.TwoFactorsAuthentification.ShowFlyout(sender as FrameworkElement);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebView.OpenHistoryDialog(sender as FrameworkElement);
        }

        private void infoButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentlySelectedWebView != null)
            {
                //pageInfoTip.Open(CurrentlySelectedWebView);
            }
        }

        private void SplitView_PaneClosed(SplitView sender, object args)
        {
            if (titleBarDragRegions != null)
            {
                titleBarDragRegions.SetDragRegionForTitleBars();
            }
        }

        private void documentTitle_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SearchBar searchBar = new SearchBar(false);
            FlyoutShowOptions options = new FlyoutShowOptions();
            options.Placement = FlyoutPlacementMode.Bottom;
            options.Position = new Windows.Foundation.Point(splitViewContentFrame.ActualWidth / 2, 100);

            searchBar.ShowAt(splitViewContentFrame, options);
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentlySelectedWebView != null)
            {
                MainMenuFlyout mainMenuFlyout = new();
                mainMenuFlyout.ShowAt(infoRegionStckPanel);
            }
        }

        private void HistoryMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            WebView.OpenHistoryDialog(moreButton);
        }

        private void DownloadsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            WebView.OpenDownloadDialog(moreButton);
        }

        private void _2FAMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            TwoFactorsAuthentification.TwoFactorsAuthentification.ShowFlyout(moreButton);
        }

        private async void SettingsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsDialog settingsDialog = new()
            {
                XamlRoot = this.XamlRoot
            };

            await settingsDialog.ShowAsync();
        }

        private void DevToolsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            DevToolsWindow devToolsWindow = new();
            devToolsWindow.Activate();
        }

        TextBlock measureTextBlock = new();
        private async void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            string text = CurrentlySelectedWebView?.CoreWebView2?.DocumentTitle;
            documentTitle.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));

            try
            {
                if (documentTitle.DesiredSize.Width >= this.ActualWidth - 300)
                {
                    int lastLetter = (int)((this.ActualWidth - 300) / 15);
                    documentTitle.Text = text.Remove(lastLetter) + "...";
                }
                else
                {
                    measureTextBlock.Text = text;
                    measureTextBlock?.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));

                    await Task.Delay(10);

                    if (documentTitle.DesiredSize.Width < this.ActualWidth - 500)
                    {
                        documentTitle.Text = text;
                    }
                }
            } catch { }
        }

        private void ToggleMenuFlyoutItem_Click
            (object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            urlTextBox.Visibility = item.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void urlTextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var kind = await WebSearch.Helpers.WebSearchStringKindHelpers.GetStringKind(urlTextBox.Text);
                if (kind == WebSearch.Helpers.WebSearchStringKindHelpers.StringKind.Url)
                {
                    string finalUrl = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToUrl(urlTextBox.Text);
                    MainView.CurrentlySelectedWebView.CoreWebView2.Navigate(finalUrl);
                }
            }
        }
    }
}

