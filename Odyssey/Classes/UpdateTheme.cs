using Microsoft.UI;
using Microsoft.UI.Xaml;
using Odyssey.Shared.Helpers;
using Odyssey.Views;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.UI;

namespace Odyssey.Classes
{
    public class UpdateTheme
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool IssystemDarkMode();

        internal static void UpdateThemeWith(string colorString)
        {
            var color = colorString.Split(',').Select(byte.Parse).ToArray();
            var nnColor = Color.FromArgb(color[0], color[1], color[2], color[3]);

            MainWindow.Current.Backdrop.SetBackdrop(BackdropKind.Acrylic);

            MainWindow.Current.Backdrop.TintOpacity = color[0] / 255;
            MainWindow.Current.Backdrop.TintColor = nnColor;

            MainView.Current.PaneAcrylicBrush.TintOpacity = MainWindow.Current.Backdrop.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.9f : 0.4f;
            MainView.Current.PaneAcrylicBrush.TintColor = MainWindow.Current.Backdrop.TintColor = Color.FromArgb(nnColor.A, nnColor.R, nnColor.G, nnColor.B);
            MainView.Current.AppTitleBar.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Transparent);
            MainView.Current.AppTitleBar.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(20, 128, 128, 128));


            MainWindow.Current.AppWindow.TitleBar.ButtonForegroundColor = MainWindow.Current.AppWindow.TitleBar.ButtonHoverForegroundColor = MainView.Current.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;

            var lightColor = ColorsHelper.LightEquivalent(nnColor, 0.2);

            MainWindow.Current.AppWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(150, lightColor.R, lightColor.G, lightColor.B);
        }
    }
}
