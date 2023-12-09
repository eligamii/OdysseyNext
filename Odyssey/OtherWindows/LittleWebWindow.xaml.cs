using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Odyssey.Data.Settings;
using Odyssey.Helpers;
using Odyssey.Views;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.OtherWindows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LittleWebWindow : WindowEx
    {
        public static new LittleWebWindow Current { get; private set; }
        public LittleWebWindow(string url)
        {
            this.InitializeComponent();

            // Load default settings for the first run
            Settings.Init();

            // Load data          
            //Downloads.Aria2.Init();
            AdBlocker.AdBlocker.Init();

            // Use this instead of Systemackdrop = Micaackdrop(); to be able to control the color of the Window
            MicaBackdropHelper.TrySetMicaBackdropTo(this);

            ExtendsContentIntoTitleBar = true;

            MinWidth = MinHeight = 500;

            Width = 810;
            Height = 810;

            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

            AppWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(100, 255, 255, 255);
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonPressedBackgroundColor = Colors.Transparent;

            Frame rootFrame = new Frame();
            rootFrame.Navigate(typeof(LittleWebView), url, new SuppressNavigationTransitionInfo());

            Content = rootFrame;

            // Start the 2FA service
            TwoFactorsAuthentification.TwoFactorsAuthentification.Init();

            Current = this;
        }
    }
}
