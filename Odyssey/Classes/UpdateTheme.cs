using Microsoft.UI;
using Microsoft.UI.Xaml;
using Odyssey.Helpers;
using Odyssey.Shared.Helpers;
using Odyssey.Views;
using System.Runtime.InteropServices;
using Windows.UI;

namespace Odyssey.Classes
{
    public class UpdateTheme
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool IssystemDarkMode();

        internal static void UpdateThemeWith(string color)
        {
            var sdc = System.Drawing.ColorTranslator.FromHtml(color);
            var nnColor = Color.FromArgb(140, sdc.R, sdc.G, sdc.B);

            bool isColorDark = ColorsHelper.IsColorDark(nnColor, 0.52);
            if (MainView.Current.ActualTheme == ElementTheme.Light)
            {
                var c = ColorsHelper.Lighten(nnColor, 0.9);
                nnColor = Color.FromArgb(240, c.R, c.G, c.B);
            }
            else
            {
                var c = ColorsHelper.Darken(nnColor, 0.3);
                nnColor = Color.FromArgb(240, c.R, c.G, c.B);
            }

            MainWindow.Current.Backdrop.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.4f : 0.9f;
            MainWindow.Current.Backdrop.TintColor = nnColor;

            MainView.Current.PaneAcrylicBrush.TintOpacity = MainWindow.Current.Backdrop.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.9f : 0.4f;
            MainView.Current.PaneAcrylicBrush.TintColor = MainWindow.Current.Backdrop.TintColor = Color.FromArgb(245, nnColor.R, nnColor.G, nnColor.B);
            MainView.Current.AppTitleBar.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Transparent);
            MainView.Current.AppTitleBar.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(20, 128, 128, 128));


            MainWindow.Current.AppWindow.TitleBar.ButtonForegroundColor = MainWindow.Current.AppWindow.TitleBar.ButtonHoverForegroundColor = MainView.Current.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;

            var lightColor = ColorsHelper.LightEquivalent(nnColor, 0.2);

            MainWindow.Current.AppWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(150, lightColor.R, lightColor.G, lightColor.B);
        }
    }
}
