using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using Odyssey.FWebView.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Shared.Helpers;
using Odyssey.Views;
using Windows.UI;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml.Media;
using Odyssey.FWebView;
using Odyssey.Helpers;

namespace Odyssey.Classes
{
    public class UpdateTheme
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool IssystemDarkMode();

        internal static void UpdateThemeWith(string color)
        {
            var sdc = System.Drawing.ColorTranslator.FromHtml(color);
            var nnColor = Color.FromArgb(140, sdc.R, sdc.G, sdc.B) ;

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

            FWebView.Classes.DynamicTheme.MicaController.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.4f : 0.9f;
            FWebView.Classes.DynamicTheme.MicaController.TintColor = nnColor;

            FWebView.Classes.DynamicTheme.AcrylicBrush.TintOpacity = MicaBackdropHelper.BackdropController.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.9f : 0.4f;
            FWebView.Classes.DynamicTheme.AcrylicBrush.TintColor = MicaBackdropHelper.BackdropController.TintColor = Color.FromArgb(245, nnColor.R, nnColor.G, nnColor.B);
            FWebView.Classes.DynamicTheme.TitleBar.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Transparent);
            FWebView.Classes.DynamicTheme.TitleBar.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(20, 128, 128, 128));


            MainWindow.Current.AppWindow.TitleBar.ButtonForegroundColor = MainWindow.Current.AppWindow.TitleBar.ButtonHoverForegroundColor = MainView.Current.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;

            var lightColor = ColorsHelper.LightEquivalent(nnColor, 0.2);

            MainWindow.Current.AppWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(150, lightColor.R, lightColor.G, lightColor.B);
        }
    }
}
