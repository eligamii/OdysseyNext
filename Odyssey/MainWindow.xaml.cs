using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Odyssey.Data.Main;
using Odyssey.Data.Settings;
using Odyssey.QuickActions.Data;
using Odyssey.Views;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static new MainWindow Current { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            // Load default settings for the first run
            Settings.Init();

            this.ExtendsContentIntoTitleBar = false; // weird behaviors happens when using it with WebView2Ex and DragRegionHelpers

            AppWindow.Title = "Odyssey Experimental";
            /*
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

            AppWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(100, 255, 255, 255);
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonPressedBackgroundColor = Colors.Transparent;*/

            Frame rootFrame = new Frame();
            rootFrame.Navigate(typeof(MainView), null, new SuppressNavigationTransitionInfo());

            Content = rootFrame;

            // Make possible to access to MainWindow from anywhere
            Current = this;

            WebView2Ex.UI.WebView2Ex.Window = this;

            UserVariables.Load();

            AppWindow.Closing += AppWindow_Closing;
        }

        private bool _close = false;
        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
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
                foreach (var tab in Tabs.Items)
                {
                    if (tab.MainWebView != null) tab.MainWebView.Close();
                }

                Tabs.Items.Clear();

                AppWindow.Hide();
            }
        }
    }
}
