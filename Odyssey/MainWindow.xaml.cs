using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Odyssey.Data.Settings;
using Odyssey.QuickActions.Data;
using Odyssey.Views;
using System;
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

            Init();
        }

        private async void Init()
        {
            // Load default settings for the first run
            Settings.Init();

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

            UserVariables.Load();

            AppWindow.Closing += AppWindow_Closing;
        }

        private bool _close = false;
        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            if (!_close)
            {
                Settings.SuccessfullyClosed = true;
                QuickActions.Data.UserVariables.Save();
                _close = true;

                Close();
            }
        }
    }
}
