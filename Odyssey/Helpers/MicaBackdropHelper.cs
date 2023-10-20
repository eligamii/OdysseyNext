using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Windowing;
using Microsoft.UI.Composition;

namespace Odyssey.Helpers
{
    public class MicaBackdropHelper
    {
        static WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See below for implementation.
        public static MicaController BackdropController;
        static SystemBackdropConfiguration m_configurationSource;

        static Window this_window;



        public static bool TrySetMicaackdropTo(Window window)
        {
            if (MicaController.IsSupported())
            {
                this_window = window;
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Create the policy object.
                m_configurationSource = new SystemBackdropConfiguration();
                window.Activated += Window_Activated;
                window.Closed += Window_Closed;
                ((FrameworkElement)window.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                BackdropController = new MicaController();


                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                BackdropController.AddSystemBackdropTarget(window as ICompositionSupportsSystemBackdrop);
                BackdropController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            Microsoft.UI.WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(myWndId);

            AppWindowTitleBar titleBar = appWindow.TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;

            titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(100, 255, 255, 255);
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonPressedBackgroundColor = Colors.Transparent;

            return false; // Mica is not supported on this system
        }

        private static void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private static void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed
            // so it doesn't try to use this closed window.
            if (BackdropController != null)
            {
                BackdropController.Dispose();
                BackdropController = null;
            }
            this_window.Activated -= Window_Activated;
            m_configurationSource = null;
        }

        private static void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private static void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)this_window.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }
        }
    }
}
