using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Odyssey.Data.Settings;
using Odyssey.FWebView.Helpers;
using Odyssey.Shared.Helpers;
using Odyssey.Views;
using Windows.UI;

namespace Odyssey.Classes
{
    public static class DynamicTheme
    {

        public static async void UpdateDynamicThemeAsync(WebView2 webView2)
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
                        MainView.Current.RequestedTheme = isColorDark ? ElementTheme.Dark : ElementTheme.Light;

                        MainView.Current.PaneAcrylicBrush.TintOpacity = MainWindow.Current.Backdrop.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.6f : 0.4f;
                        MainWindow.Current.Backdrop.TintColor = MainWindow.Current.Backdrop.TintColor = Color.FromArgb(Settings.ColorAlpha, nnColor.R, nnColor.G, nnColor.B);
                        MainView.Current.AppTitleBar.Background = Settings.IsSolidTitleBarEnabled ? new SolidColorBrush(nnColor) : new SolidColorBrush(Colors.Transparent);
                        MainView.Current.AppTitleBar.BorderBrush = new SolidColorBrush(ColorsHelper.LightEquivalent(nnColor, 0.1));

                        MainWindow.Current.AppWindow.TitleBar.ButtonForegroundColor = MainWindow.Current.AppWindow.TitleBar.ButtonHoverForegroundColor = isColorDark ? Colors.White : Colors.Black;

                        var lightColor = ColorsHelper.LightEquivalent(nnColor, 0.1);

                        MainWindow.Current.AppWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(150, lightColor.R, lightColor.G, lightColor.B);
                    }
                    else
                    {
                        isColorDark = ColorsHelper.IsColorDark(nnColor, 0.52);
                        if (MainView.Current.ActualTheme == ElementTheme.Light)
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

                        MainView.Current.PaneAcrylicBrush.TintOpacity = MainWindow.Current.Backdrop.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.6f : 0.4f;
                        MainView.Current.PaneAcrylicBrush.TintColor = MainWindow.Current.Backdrop.TintColor = nnColor;
                        MainView.Current.AppTitleBar.Background = Settings.IsSolidTitleBarEnabled ? new SolidColorBrush(ColorsHelper.LightEquivalent(nnColor, 0.01)) : new SolidColorBrush(Colors.Transparent);

                        MainWindow.Current.AppWindow.TitleBar.ButtonForegroundColor = MainWindow.Current.AppWindow.TitleBar.ButtonHoverForegroundColor = MainView.Current.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;

                        var lightColor = ColorsHelper.LightEquivalent(nnColor, 0.2);

                        MainWindow.Current.AppWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(150, lightColor.R, lightColor.G, lightColor.B);
                    }
                }
            }
            catch { }
        }

    }
}
