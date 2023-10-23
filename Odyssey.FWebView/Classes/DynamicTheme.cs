using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Odyssey.FWebView.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.FWebView.Classes
{
    public static class DynamicTheme
    {
        public static MicaController MicaController { get; set; }
        public static Page PageToUpdateTheme { get; set; }
        public static bool UpdateTheme { get; set; } = true;

        public static async void UpdateDynamicTheme(WebView2 webView2)
        {
            // Get webView2 colors info
            var color = await WebView2AverageColorHelper.GetWebView2AverageColorsAsync(webView2);
            
            if(color != null)
            {
                bool isColorDark = ColorsHelper.IsColorDark((Windows.UI.Color)color);

                if (UpdateTheme) PageToUpdateTheme.RequestedTheme = isColorDark ? ElementTheme.Dark : ElementTheme.Light;

                MicaController.TintOpacity = ColorsHelper.IsColorGrayTint((Windows.UI.Color)color) ? 0.4f : 0.9f;
                MicaController.TintColor = (Windows.UI.Color)color;
            }
        }

    }
}
