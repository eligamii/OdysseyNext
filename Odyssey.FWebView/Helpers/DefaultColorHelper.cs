using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Odyssey.FWebView.Helpers
{
    internal static class DefaultColorHelper
    {
        private static Bitmap ChangeColor(Bitmap bitmap, Color newColor)
        {
            Color actualColor;

            //make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    //get the pixel from the scrBitmap image
                    actualColor = bitmap.GetPixel(i, j);
                    // > 150 because.. Images edges can be of low pixel colr. if we set all pixel color to new then there will be no smoothness left.
                    if (actualColor.A > 150)
                        newBitmap.SetPixel(i, j, newColor);
                    else
                        newBitmap.SetPixel(i, j, actualColor);
                }
            }
            return newBitmap;
        }



        private static async Task<BitmapImage> Bitmap2BitmapImage(Bitmap bitmap)
        {
            BitmapImage image = new();
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            var stream = memoryStream.AsRandomAccessStream();

            await image.SetSourceAsync(stream);

            return image;
        }

        public static async Task<object> CreateDefaultIcon(WebView2 webView)
        {
            string defaultIconPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Odyssey.FWebView", "Assets", "square.png");
            var windowsColor = await WebView2AverageColorHelper.GetAverageColorFromWebView2Async(webView, 100, 100, 1);

            Color color = Color.FromArgb(windowsColor.A, windowsColor.R, windowsColor.G, windowsColor.B);
            Bitmap defaultIcon = (Bitmap)System.Drawing.Image.FromFile(defaultIconPath);

            MemoryStream memoryStream = new MemoryStream();
            ChangeColor(defaultIcon, color).Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            var stream = memoryStream.AsRandomAccessStream();

            return stream;
        }
    }
}
