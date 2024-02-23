using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Odyssey.AdBlocker;
using Odyssey.Classes;
using Odyssey.Controls;
using Odyssey.Data.Main;
using Odyssey.Data.Settings;
using Odyssey.Dialogs;
using Odyssey.FWebView;
using Odyssey.Helpers;
using Odyssey.OtherWindows;
using Odyssey.QuickActions;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Views.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Devices;
using Type = System.Type;




namespace Odyssey.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        public static MainView Current { get; set; }
        public bool FocusModeEnabled { get; set; } = false;
        public TitleBarDragRegions titleBarDragRegions;
        public string MainDocumentTitle
        {
            get { return documentTitle.Text; }
            set
            {
                if (value != documentTitle.Text)
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

            if (Settings.SuccessfullyClosed == false && Settings.RestoreTabsAfterCrash && instances.Count == 1)
            {
                PaneView.Current.TabsView.ItemsSource = Tabs.Restore();

                switch(Settings.TabType)
                {
                    case 0: PaneView.Current.FavoriteGrid.SelectedIndex = Settings.TabIndex; break;
                    case 1: PaneView.Current.PinsTabView.SelectedIndex = Settings.TabIndex; break;
                    case 2: PaneView.Current.TabsView.SelectedIndex = Settings.TabIndex; break;
                }
            }

            Settings.SuccessfullyClosed = false;
        }

        DispatcherTimer focusModeCooldownTimer = new() { Interval = TimeSpan.FromSeconds(2) };
        private async void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            // Will block the loading of the MainPage 
            if (Settings.FirstLaunch != false)
            {
                QuickConfigurationDialog quickConfigurationDialog = new()
                {
                    XamlRoot = MainWindow.Current.Content.XamlRoot
                };

                await quickConfigurationDialog.ShowAsync();
            }


            // Automatically create titlebar drag regions for the window (see Odyssey.App.Helpers.TitleBarDragRegionsHelpers)
            titleBarDragRegions = new TitleBarDragRegions(
                new List<Grid>() { AppTitleBar, secondTitleBar },
                MainWindow.Current,
                new List<Type>() { typeof(ProgressBar), typeof(TextBlock), typeof(Microsoft.UI.Xaml.Shapes.Rectangle), typeof(Frame) },
                MainWindow.Current.Content as FrameworkElement,
                42);

            WebView.XamlRoot = XamlRoot;


            DiltillNETAdBlocker.Init();

            // Require these to not crash
            WebView.MainDownloadElement = moreButton;
            WebView.MainHistoryElement = moreButton;
            WebView.MainIconElement = new Image();
            WebView.MainProgressBar = progressBar;
            WebView.MainProgressElement = new ProgressBar();
            WebView.MainWebViewFrame = splitViewContentFrame;
            WebView.DocumentTextBlock = documentTitle;
            WebView.UrlTextBox = urlTextBox;

            QACommands.Frame = splitViewContentFrame;
            QACommands.ButtonsStackPanel = buttonsStackPanel;

            // Set the Quick actions command MainWindow to this window
            QACommands.MainWindow = MainWindow.Current;

            WebView.TotpLoginDetectedAction += SetTotpButtonVisibility;
            WebView.LoginPageDetectedAction += LoginDetectedChanged;

            this.ActualThemeChanged += (s, a) => SetCustomTheme();

            if (Settings.OpenTabAtStartup)
            {
                WebView webView = WebView.Create(SearchEngine.SelectedSearchEngine.Url);

                Tab tab = new()
                {
                    Url = SearchEngine.SelectedSearchEngine.Url,
                    Title = SearchEngine.SelectedSearchEngine.Name,
                    MainWebView = webView
                };

                webView.LinkedTab = tab;

                Tabs.Items.Add(tab);
                PaneView.Current.TabsView.SelectedItem = tab;
            }
            else if(Settings.SuccessfullyClosed == true)
            {
                splitViewContentFrame.Navigate(typeof(HomePage));
            }


            // Update the titlebar drag region based on the text of the documentTitle
            documentTitle.LayoutUpdated += DocumentTitle_LayoutUpdated;

            SplitView.DisplayMode = Settings.IsPaneLocked ? SplitViewDisplayMode.Inline : SplitViewDisplayMode.Overlay;
            SplitView.PaneBackground = Settings.IsPaneLocked ? new SolidColorBrush(Colors.Transparent) : PaneAcrylicBrush;


            WebView.CurrentlySelectedWebViewEventTriggered += WebView_CurrentlySelectedWebViewEventTriggered;


            titleBarDragRegions.SetDragRegionForTitleBars();

            stopwatch.Stop();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);

            // Use this instead of Systemackdrop = MicaBackdrop(); to be able to control the color of the Window
            SplitViewPaneFrame.Navigate(typeof(PaneView), null, new SuppressNavigationTransitionInfo());
            MainWindow.Current.Backdrop.SetBackdrop((BackdropKind)Settings.SystemBackdrop);

            Controls.TitleBarButtons.AddButtonsTo(buttonsStackPanel, false);

            // Set the custom theme if dynamic theme is not enabled
            if (!Settings.IsDynamicThemeEnabled)
                SetCustomTheme();

            // TODO: Remove this (this is for testing purposes)
            var extensions = await WebView.GetExtensionsAsync();

            foreach (var extension in extensions)
            {
                BitmapIcon bitmapIcon = new();
                bitmapIcon.UriSource = new(extension.MinQualityIcon);
                bitmapIcon.ShowAsMonochrome = false;

                MenuFlyoutItem item = new();
                item.Text = extension.DisplayName;
                item.Icon = bitmapIcon;
                item.Click += (s, a) => extension.OpenDefultPopup(moreButton);

                extensionFlyoutItem.Items.Add(item);
            }

            focusModeCooldownTimer.Tick += FocusModeCooldownTimer_Tick;

            // Restore tabs after crash
            RestoreTabs();
        }

        

        private bool _wasOpened;



        private void SplitView_PaneClosedOnFullScreen(SplitView sender, object args)
        {
            AppTitleBar.Visibility = Visibility.Collapsed;
        }

        private void SplitView_PaneOpened(SplitView sender, object args)
        {
            AppTitleBar.Visibility = Visibility.Visible;
        }

        private string _lastText = string.Empty;

        private void DocumentTitle_LayoutUpdated(object sender, object e)
        {

        }

        private bool lastConnectionState;

        private void SetCustomTheme()
        {
            if (!Settings.IsDynamicThemeEnabled)
            {
                string color = Settings.CustomThemeColors;
                if (color != null)
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
            if (CurrentlySelectedWebView != null)
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
        private bool _wasPaneManuallyOpened = false;
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            SplitView.IsPaneOpen ^= true;
            _wasPaneManuallyOpened = SplitView.IsPaneOpen;

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

            if (splitViewContentFrame.ActualWidth > 610)
            {
                options.Position = new Windows.Foundation.Point(splitViewContentFrame.ActualWidth / 2, 100);
            }
            else
            {
                options.Position = new Windows.Foundation.Point(this.ActualWidth / 2, 100);
            }

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
            if (CurrentlySelectedWebView != null)
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
            if (CurrentlySelectedWebView != null)
            {
                MainMenuFlyout mainMenuFlyout = new();
                FlyoutShowOptions options = new FlyoutShowOptions();
                options.Placement = FlyoutPlacementMode.Bottom;
                options.Position = new Windows.Foundation.Point(splitViewContentFrame.ActualWidth / 2, 4);

                mainMenuFlyout.ShowAt(splitViewContentFrame, options);
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
            if (CurrentlySelectedWebView != null)
            {
                if (CurrentlySelectedWebView.CoreWebView2?.DocumentTitle != null)
                {
                    try
                    {
                        string doc = Settings.ShowHostInsteadOfDocumentTitle ? CurrentlySelectedWebView.Source.Host : CurrentlySelectedWebView.CoreWebView2.DocumentTitle;
                        CanvasTextFormat format = new()
                        {
                            FontFamily = "Segoe UI Variable",
                            FontSize = (float)documentTitle.FontSize,
                            FontWeight = documentTitle.FontWeight,
                            FontStretch = documentTitle.FontStretch,
                            FontStyle = documentTitle.FontStyle
                        };

                        documentTitle.Text = TextHelpers.TrimTextToDesiredWidth(doc, format, e.NewSize.Width - (buttonsStackPanel.Children.Count() * 38 + 50) * 2);
                    }
                    catch { }
                }
            }
        }

        private void UrlBarToggleMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            urlTextBox.Visibility = item.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            Settings.IsDevBarEnabled = item.IsChecked;
        }

        private async void urlTextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string finalUrl = await WebSearch.Helpers.WebViewNavigateUrlHelper.ToWebView2Url(urlTextBox.Text);
                if (finalUrl != string.Empty)
                {
                    if (CurrentlySelectedWebView != null)
                    {
                        CurrentlySelectedWebView.CoreWebView2.Navigate(finalUrl);
                    }
                    else
                    {
                        FWebView.WebView webView = FWebView.WebView.Create(finalUrl);
                        Tab tab = new()
                        {
                            Title = urlTextBox.Text,
                            ToolTip = finalUrl,
                        };

                        tab.MainWebView = webView;
                        Tabs.Items.Add(tab);
                        try
                        {
                            await Task.Delay(100);
                            PaneView.Current.TabsView.SelectedItem = tab;
                        }
                        catch { }
                        webView.LinkedTab = tab;
                        splitViewContentFrame.Content = webView;
                    }
                }
            }
        }

        private void urlTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            urlTextBox.Visibility = Settings.IsDevBarEnabled ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UrlBarToggleMenuFlyoutItem_Loaded(object sender, RoutedEventArgs e)
        {
            ((ToggleMenuFlyoutItem)sender).IsChecked = Settings.IsDevBarEnabled;
        }

        private void LockPaneToggleMenuFlyoutItem_Loaded(object sender, RoutedEventArgs e)
        {
            ((ToggleMenuFlyoutItem)sender).IsChecked = Settings.IsPaneLocked;
        }

        private void LockPaneToggleMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            SplitView.DisplayMode = item.IsChecked ? SplitViewDisplayMode.Inline : SplitViewDisplayMode.Overlay;
            SplitView.PaneBackground = item.IsChecked ? new SolidColorBrush(Colors.Transparent) : PaneAcrylicBrush;
            Settings.IsPaneLocked = item.IsChecked;
        }

        private void wheelTabSwitchItems_PointerWheelChanged(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var pointer = e.GetCurrentPoint(this);
            int delta = pointer.Properties.MouseWheelDelta;
            int indexShift = delta > 0 ? -1 : 1;

            int index = PaneView.Current.TabsView.SelectedIndex;
            int maxIndex = PaneView.Current.TabsView.Items.Count - 1;

            if (index != -1)
            {
                int newIndex = index + indexShift;

                if (newIndex >= 0 && newIndex <= maxIndex)
                {
                    PaneView.Current.TabsView.SelectedIndex = newIndex;
                }
            }
        }

        private async void QuickActionsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            QuickActionsContentDialog contentDialog = new();
            contentDialog.XamlRoot = this.Content.XamlRoot;

            await contentDialog.ShowAsync();
        }

        private void HomeButton_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            MenuFlyout flyout = new();
            FlyoutShowOptions options = new()
            {
                Position = e.GetPosition(this)
            };

            foreach (Tab tab in Favorites.Items)
            {
                var bit = new BitmapIcon();

                ToggleMenuFlyoutItem item = new();
                item.Text = tab.Title;
                item.Icon = new SymbolIcon(Symbol.OutlineStar);
                item.IsChecked = CurrentlySelectedWebView == tab.MainWebView;
                item.Click += (s, a) =>
                {
                    if (item.IsChecked) PaneView.Current.FavoriteGrid.SelectedItem = tab;
                };

                flyout.Items.Add(item);
            }
            flyout.Items.Add(new MenuFlyoutSeparator());

            foreach (Tab tab in Pins.Items)
            {
                ToggleMenuFlyoutItem item = new();
                item.Text = tab.Title;
                item.Icon = new SymbolIcon(Symbol.Pin);
                item.IsChecked = CurrentlySelectedWebView == tab.MainWebView;
                item.Click += (s, a) =>
                {
                    if (item.IsChecked) PaneView.Current.PinsTabView.SelectedItem = tab;
                };

                flyout.Items.Add(item);
            }

            flyout.Items.Add(new MenuFlyoutSeparator());

            foreach (Tab tab in Tabs.Items)
            {
                ToggleMenuFlyoutItem item = new();
                item.Text = tab.Title;
                item.Icon = new SymbolIcon(Symbol.World);
                item.IsChecked = CurrentlySelectedWebView == tab.MainWebView;
                item.Click += (s, a) =>
                {
                    if (item.IsChecked) PaneView.Current.TabsView.SelectedItem = tab;
                };

                flyout.Items.Add(item);
            }

            flyout.ShowAt(this, options);
        }

        private async void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(2000);
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
        }

        private void RootGrid_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }

        private void showPaneButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (Settings.HoverToOpenPane && SplitView.DisplayMode == SplitViewDisplayMode.Overlay) SplitView.IsPaneOpen = true;
        }

        private void secondTitleBar_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PointerPoint pos = e.GetCurrentPoint(this);
            if (!_wasPaneManuallyOpened && Settings.HoverToOpenPane && SplitView.DisplayMode == SplitViewDisplayMode.Overlay && pos.Position.X > 5) SplitView.IsPaneOpen = false;
        }

        
        private void focusButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var element in AppTitleBar.Children.Where(p => p.GetType() != typeof(TextBox)))
                element.Visibility = Visibility.Visible;

            AppTitleBar.RowDefinitions[0].Height = new GridLength(42);
            titleBarDragRegions.SetTitleBarsHeight(42);

            focusButton.Visibility = Visibility.Collapsed;

            this.PointerEntered += (s, a) => focusModeCooldownTimer.Start(); // See this as AppTitleBar.PointerExited
            this.PointerExited += (s, a) => focusModeCooldownTimer.Stop(); // and this as AppTitleBar.PointerEntered

            focusModeCooldownTimer.Stop();

        }

      
        private void FocusModeCooldownTimer_Tick(object sender, object e)
        {
            if(FocusModeEnabled)
            {
                foreach (var element in AppTitleBar.Children.Where(p => p.GetType() != typeof(TextBox)))
                    element.Visibility = Visibility.Collapsed;

                AppTitleBar.RowDefinitions[0].Height = new GridLength(22);
                titleBarDragRegions.SetTitleBarsHeight(20);

                focusButton.Visibility = Visibility.Visible;
            }

            focusModeCooldownTimer.Stop();


        }
    }
}

