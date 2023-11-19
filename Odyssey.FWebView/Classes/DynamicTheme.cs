using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Odyssey.Data.Settings;
using Odyssey.FWebView.Helpers;
using Odyssey.Shared.Helpers;
using Windows.UI;

namespace Odyssey.FWebView.Classes
{
    public static class DynamicTheme
    {
        public static Grid TitleBar { get; set; }
        public static MicaController MicaController { get; set; }
        public static Page PageToUpdateTheme { get; set; }
        public static AppWindowTitleBar AppWindowTitleBar { get; set; }
        public static bool UpdateTheme { get; set; } = true;

        public static async void UpdateDynamicTheme(WebView2 webView2)
        {
            // Get webView2 average color
            Color? color = null;

            if(Settings.DynamicThemeEnabled)
            {
                switch (Settings.ThemePerformanceMode)
                {
                    case 0: // 40000 pixels, quality mode
                        color = await WebView2AverageColorHelper.GetWebView2AverageColorsAsync(webView2, 400, 400, 4); break;

                    case 1: // 8000 pixels, default mode
                        color = await WebView2AverageColorHelper.GetWebView2AverageColorsAsync(webView2, 400, 80, 4); break;

                    case 2: // 2500 pixels, performace mode
                        color = await WebView2AverageColorHelper.GetWebView2AverageColorsAsync(webView2, 50, 50, 1); break;  
                }
            }

            if (color != null)
            {
                var nnColor = (Color)color;

                bool isColorDark = ColorsHelper.IsColorDark(nnColor);

                if (Settings.IsDynamicThemeModeChangeEnabled)
                {
                    PageToUpdateTheme.RequestedTheme = isColorDark ? ElementTheme.Dark : ElementTheme.Light;

                    MicaController.TintOpacity = 0.9f;
                    MicaController.TintColor = Color.FromArgb(245, nnColor.R, nnColor.G, nnColor.B);
                    TitleBar.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(nnColor);
                    TitleBar.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(ColorsHelper.LightEquivalent(nnColor, 0.1));

                    AppWindowTitleBar.ButtonForegroundColor = AppWindowTitleBar.ButtonHoverForegroundColor = isColorDark ? Colors.White : Colors.Black;

                    var lightColor = ColorsHelper.LightEquivalent(nnColor, 0.1);

                    AppWindowTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(150, lightColor.R, lightColor.G, lightColor.B);
                }
                else
                {
                    isColorDark = ColorsHelper.IsColorDark(nnColor, 0.52);
                    if (PageToUpdateTheme.ActualTheme == ElementTheme.Light)
                    {
                        if (isColorDark)
                        {
                            var c = ColorsHelper.Lighten(nnColor, 0.7);
                            nnColor = Color.FromArgb(255, c.R, c.G, c.B);
                        }
                    }
                    else
                    {
                        if (!isColorDark)
                        {
                            var c = ColorsHelper.Darken(nnColor, 0.3);
                            nnColor = Color.FromArgb(255, c.R, c.G, c.B);
                        }
                    }

                    MicaController.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.4f : 0.9f;
                    MicaController.TintColor = nnColor;
                    TitleBar.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(ColorsHelper.LightEquivalent(nnColor, 0.01));

                    AppWindowTitleBar.ButtonForegroundColor = AppWindowTitleBar.ButtonHoverForegroundColor = PageToUpdateTheme.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;

                    var lightColor = ColorsHelper.LightEquivalent(nnColor, 0.2);

                    AppWindowTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(150, lightColor.R, lightColor.G, lightColor.B);
                }
            }
        }

    }
}
