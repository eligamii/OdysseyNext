using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.FWebView.Helpers;

namespace Odyssey.FWebView.Classes
{
    public static class DynamicTheme
    {
        public static MicaController MicaController { get; set; }
        public static Page PageToUpdateTheme { get; set; }
        public static AppWindowTitleBar AppWindowTitleBar { get; set; }
        public static bool UpdateTheme { get; set; } = true;

        public static async void UpdateDynamicTheme(WebView2 webView2)
        {
            // Get webView2 average color
            var color = await WebView2AverageColorHelper.GetWebView2AverageColorsAsync(webView2, 800, 800, 1);

            if (color != null)
            {
                var nnColor = (Windows.UI.Color)color;
                bool isColorDark = ColorsHelper.IsColorDark(nnColor);

                if (UpdateTheme) PageToUpdateTheme.RequestedTheme = isColorDark ? ElementTheme.Dark : ElementTheme.Light;

                MicaController.TintOpacity = ColorsHelper.IsColorGrayTint(nnColor) ? 0.4f : 0.9f;
                MicaController.TintColor = nnColor;

                AppWindowTitleBar.ButtonForegroundColor = AppWindowTitleBar.ButtonHoverForegroundColor = isColorDark ? Colors.White : Colors.Black;

                var lightColor = ColorsHelper.LightEquivalent(nnColor, 0.2);

                AppWindowTitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(150, lightColor.R, lightColor.G, lightColor.B);
            }
        }

    }
}
