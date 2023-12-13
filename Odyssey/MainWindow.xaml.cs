using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Odyssey.Data.Main;
using Odyssey.Data.Settings;
using Odyssey.Views;
using Odyssey.Views.Pages;
using System;
using System.IO;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public static new MainWindow Current { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.Title = Shared.Helpers.ResourceString.GetString("Odyssey", "Main");
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Square44x44Logo.altform-lightunplated_targetsize-24.png");
            AppWindow.SetIcon(path);
            Init();
        }

        private void Init()
        {
            ExtendsContentIntoTitleBar = true;

            MinWidth = MinHeight = 500;

            // Change the size of the window to match with the UWP default window size
            Width = 1040;
            Height = 810;

            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

            AppWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(100, 255, 255, 255);
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonPressedBackgroundColor = Colors.Transparent;

            Frame rootFrame = new Frame();
            rootFrame.Navigate(typeof(MainView), null, new SuppressNavigationTransitionInfo());

            Content = rootFrame;

            // Make possible to access to MainWindow from anywhere
            Current = this;



            AppWindow.Closing += AppWindow_Closing;
        }

        private bool _close = false;
        public static bool ResetEngaged { get; set; } = false;
        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            if (!ResetEngaged)
            {
                args.Cancel = Settings.IsSingleInstanceEnabled;
                Settings.SuccessfullyClosed = true;
                QuickActions.Data.UserVariables.Save();

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

                    bool dark = Classes.UpdateTheme.IssystemDarkMode();
                    string color = dark ? "#202020" : "#F9F9F9";

                    if (Settings.IsDynamicThemeEnabled)
                    {
                        MainView.Current.RequestedTheme = Classes.UpdateTheme.IssystemDarkMode() ? Microsoft.UI.Xaml.ElementTheme.Dark : Microsoft.UI.Xaml.ElementTheme.Light;
                        Classes.UpdateTheme.UpdateThemeWith(color);
                    }

                    Tabs.Items.Clear();

                    AppWindow.Hide();
                }
            }
        }
    }
}
