using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Odyssey.Classes;
using Odyssey.Data.Main;
using Odyssey.Data.Settings;
using Odyssey.Views;
using Odyssey.Views.Pages;
using Windows.UI.WindowManagement;
using WinUIEx;




namespace Odyssey
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>


    public sealed partial class MainWindow : WindowEx
    {
        public static new MainWindow Current { get; set; }
        public bool IsActivated { get; set; } = false;
        public new Backdrop Backdrop { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.Title = Shared.Helpers.ResourceString.GetString("Odyssey", "Main");
            Backdrop = new(this);
            Init();
        }

        private void Init()
        {
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed; // To create custom caption buttons (will remove the default ones)

            MinWidth = MinHeight = 500;

            Width = Settings.Width;
            Height = Settings.Height;

            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

            AppWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(100, 255, 255, 255);
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonPressedBackgroundColor = Colors.Transparent;

            Frame rootFrame = new Frame();
            rootFrame.Navigate(typeof(MainView), null, new SuppressNavigationTransitionInfo());

            Content = rootFrame;

            // Make possible to access to MainWindow from anywhere
            Current = this;

            Hotkeys.Init();

            AppWindow.Closing += AppWindow_Closing;
            this.Activated += MainWindow_Activated;
            this.SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            if(this.WindowState == WindowState.Normal)
            {
                Settings.Width = args.Size.Width;
                Settings.Height = args.Size.Height;

            }
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
                IsActivated = false;
            else IsActivated = true;
        }

        public static bool ResetEngaged { get; set; } = false;
        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            if (!ResetEngaged)
            {
                args.Cancel = Settings.IsSingleInstanceEnabled;
                Settings.SuccessfullyClosed = true;
                QuickActions.Data.UserVariables.Save();
                Data.Main.TitleBarButtons.Save();

                if (!Settings.IsSingleInstanceEnabled)
                {
                    Close();
                }
                else
                {
                    // Put Odyssey in the default state
                    foreach (var tab in Tabs.Items)
                    {
                        if (tab.MainWebView != null) tab.MainWebView.Close();
                    }

                    foreach (var tab in Pins.Items)
                    {
                        if (tab.MainWebView != null) tab.MainWebView.Close();
                        tab.MainWebView = null;
                    }

                    foreach (var tab in Favorites.Items)
                    {
                        if (tab.MainWebView != null) tab.MainWebView.Close();
                        tab.MainWebView = null;
                    }

                    PaneView.Current.FavoriteGrid.SelectedItem =
                    PaneView.Current.PinsTabView.SelectedItem =
                    PaneView.Current.TabsView.SelectedItem = null;

                    MainView.Current.splitViewContentFrame.Navigate(typeof(HomePage), null, new SuppressNavigationTransitionInfo());

                    MainView.Current.documentTitle.Text = Shared.Helpers.ResourceString.GetString("Odyssey", "Main");

                    Tabs.Items.Clear();

                    AppWindow.Hide();
                }
            }
        }
    }
}
