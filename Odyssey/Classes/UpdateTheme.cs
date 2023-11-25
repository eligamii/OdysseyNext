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

namespace Odyssey.Classes
{
    internal class UpdateTheme
    {
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

            MainWindow.Current.AppWindow.TitleBar.ButtonForegroundColor = MainWindow.Current.AppWindow.TitleBar.ButtonHoverForegroundColor = MainView.Current.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;

            var lightColor = ColorsHelper.LightEquivalent(nnColor, 0.2);

            MainWindow.Current.AppWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(150, lightColor.R, lightColor.G, lightColor.B);
        }
    }
}
