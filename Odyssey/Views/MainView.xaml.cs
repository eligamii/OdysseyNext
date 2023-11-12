using CommunityToolkit.WinUI.UI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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
using Odyssey.QuickActions;
using Odyssey.Views.Pages;
using System;
using System.Collections.Generic;
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
        public static FWebView.WebView CurrentlySelectedWebView
        {
            get { return Current.splitViewContentFrame.Content as FWebView.WebView; }
        }
        public MainView()
        {
            this.InitializeComponent();

            Loaded += MainView_Loaded;

            AppTitleBar.Loaded += AppTitleBar_Loaded;
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;

            Current = this;
        }


        private async void RestoreTabs()
        {
            await Data.Main.Data.Init();

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
            FWebView.Classes.DynamicTheme.PageToUpdateTheme = this;
            FWebView.Classes.DynamicTheme.MicaController = MicaBackdropHelper.BackdropController;
            FWebView.Classes.DynamicTheme.AppWindowTitleBar = MainWindow.Current.AppWindow.TitleBar;
            FWebView.Classes.DynamicTheme.UpdateTheme = true;

            // Start the 2FA service
            TwoFactorsAuthentification.TwoFactorsAuthentification.Init();

            // Load data          
            Downloads.Aria2.Init();
            AdBlocker.AdBlocker.Init();

            // Restore tabs after crash
            RestoreTabs();

            SplitViewPaneFrame.Navigate(typeof(PaneView), null, new SuppressNavigationTransitionInfo());

            // Require these to not crash
            WebView.MainDownloadElement = downloadButton;
            WebView.MainHistoryElement = historyButton;
            WebView.MainIconElement = Favicon;
            WebView.MainProgressElement = progressRing;
            WebView.MainWebViewFrame = splitViewContentFrame;

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
        }

        private bool lastConnectionState;

        private void SetCustomTheme()
        {
            if (!Settings.DynamicThemeEnabled)
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
            if (CurrentlySelectedWebView != null)
                _2faButton.Visibility = CurrentlySelectedWebView.IsTotpDetected ? Visibility.Visible : Visibility.Collapsed;
            else
                _2faButton.Visibility = Visibility.Collapsed;
        }

        public void LoginDetectedChanged()
        {
            loginButton.Visibility = CurrentlySelectedWebView.IsLoginPageDetected ? Visibility.Visible : Visibility.Collapsed;

            if(CurrentlySelectedWebView.IsLoginPageDetected)
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
                        progressRing.Foreground = Application.Current.Resources["FocusStrokeColorOuterBrush"] as Brush;
                    }
                    else
                    {
                        progressRing.Foreground = Application.Current.Resources["AccentFillColorDefaultBrush"] as Brush;
                    }

                    lastConnectionState = isInternetAvailable;
                }
            }
        }


        // Titlebar drag regions
        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetDragRegionForCustomTitleBar(MainWindow.Current.AppWindow);
        }

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            SetDragRegionForCustomTitleBar(MainWindow.Current.AppWindow);
        }


        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

        private double GetScaleAdjustment()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(MainWindow.Current);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            // Get DPI.
            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
            if (result != 0)
            {
                throw new System.Exception("Could not get DPI for monitor.");
            }

            uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorPercent / 100.0;
        }

        private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
        {
            try
            {
                // Check to see if customization is supported.
                // The method returns true on Windows 10 since Windows App SDK 1.2, and on all versions of
                // Windows App SDK on Windows 11.
                if (AppWindowTitleBar.IsCustomizationSupported()
                    && appWindow.TitleBar.ExtendsContentIntoTitleBar)
                {
                    double scaleAdjustment = GetScaleAdjustment();

                    RightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);
                    LeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

                    List<Windows.Graphics.RectInt32> dragRectsList = new();

                    Windows.Graphics.RectInt32 dragRectL;
                    dragRectL.X = (int)((LeftPaddingColumn.ActualWidth) * scaleAdjustment);
                    dragRectL.Y = 0;
                    dragRectL.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
                    dragRectL.Width = (int)((0) * scaleAdjustment);
                    dragRectsList.Add(dragRectL);

                    Windows.Graphics.RectInt32 dragRectR;
                    dragRectR.X = (int)((LeftPaddingColumn.ActualWidth
                                        + ControlsColumn.ActualWidth) * scaleAdjustment);
                    dragRectR.Y = 0;
                    dragRectR.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
                    dragRectR.Width = (int)(RightDragColumn.ActualWidth * scaleAdjustment);
                    dragRectsList.Add(dragRectR);

                    Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

                    appWindow.TitleBar.SetDragRectangles(dragRects);
                }
            }
            catch { }
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
            options.Position = new Point(splitViewContentFrame.ActualWidth / 2, 100);

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
                pageInfoTip.Open(CurrentlySelectedWebView);
            }
        }
    }
}
