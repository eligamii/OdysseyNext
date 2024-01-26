using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Odyssey.Data.Settings;
using Odyssey.FWebView.Helpers;
using Odyssey.Shared.Helpers;
using System.Threading.Tasks;
using Windows.UI;

namespace Odyssey.FWebView.Classes
{
    public static class DynamicTheme // This is a very bad class only made for Odyssey
    {
        public static AcrylicBrush AcrylicBrush { get; set; }
        public static Grid TitleBar { get; set; }
        public static MicaController MicaController { get; set; }
        public static Page PageToUpdateTheme { get; set; }
        public static AppWindowTitleBar AppWindowTitleBar { get; set; }
        public static bool UpdateTheme { get; set; } = true;

        public static async Task UpdateDynamicThemeAsync(WebView2 webView2)
        {
            // Get webView2 average color
            Color? color = null;

            try
            {
                if (Settings.IsDynamicThemeEnabled)
                {
                    switch (Settings.ThemePerformanceMode)
                    {
                        case 0: // Depend of the size of the webview (min = 20225 pixels or (min window size - locked pane size) / 4)px, quality mode
                            color = await WebView2AverageColorHelper.GetAverageColorFromWebView2Async(webView2, (int)webView2.ActualWidth, 100, 4); break;

                        case 1: // Depend of the size of the webview (min = 2022 pixels), default mode
                            color = await WebView2AverageColorHelper.GetAverageColorFromWebView2Async(webView2, (int)webView2.ActualWidth, 10, 4); break;

                        case 2: // 100 pixels, performace mode
                            color = await WebView2AverageColorHelper.GetAverageColorFromWebView2Async(webView2, 10, 10, 1); break;
                    }
                }

                if (color != null)
                {
                    var nnColor = (Color)color;

                    bool isColorDark = ColorsHelper.IsColorDark(nnColor);

                    if (Settings.IsDynamicThemeModeChangeEnabled)
                    {
                        PageToUpdateTheme.RequestedTheme = isColorDark ? ElementTheme.Dark : ElementTheme.Light;

                        AcrylicBrush.TintOpacity = MicaController.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.9f : 0.4f;
                        AcrylicBrush.TintColor = MicaController.TintColor = Color.FromArgb(245, nnColor.R, nnColor.G, nnColor.B);
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

                        AcrylicBrush.TintOpacity = MicaController.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.4f : 0.9f;
                        AcrylicBrush.TintColor = MicaController.TintColor = nnColor;
                        TitleBar.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(ColorsHelper.LightEquivalent(nnColor, 0.01));

                        AppWindowTitleBar.ButtonForegroundColor = AppWindowTitleBar.ButtonHoverForegroundColor = PageToUpdateTheme.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;

                        var lightColor = ColorsHelper.LightEquivalent(nnColor, 0.2);

                        AppWindowTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(150, lightColor.R, lightColor.G, lightColor.B);
                    }
                }
            }
            catch { }
        }

    }
}
