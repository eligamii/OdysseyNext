using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.WebUI;
using WinRT.Interop;

namespace Odyssey.FWebView.Helpers
{
    public class WebView2AverageColorHelper
    {
        /// <summary>
        /// Get the average color of a part of a webview with the coordinate (0;0) the upper left corner of the webView
        /// </summary>
        /// <param name="webView">The webview</param>
        /// <param name="width">The width (>= 1) of the part to calculate</param>
        /// <param name="height">The height (>=1) of the part to calculate</param>
        /// <param name="step">(>=1) Greater value = less precision / better performance</param>
        internal async static Task<Windows.UI.Color?> GetWebView2AverageColorsAsync(WebView2 webView, uint width = 600, uint height = 1, int step = 1)
        {
            try
            {
                await webView.EnsureCoreWebView2Async();

                // Capture the WebView content
                var captureStream = new InMemoryRandomAccessStream();
                await webView.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, captureStream);

                // Create a BitmapDecoder and get the captured image as a SoftwareBitmap
                var decoder = await BitmapDecoder.CreateAsync(captureStream);

                var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                // Save the bitmap to a file (only way i found to convert this into a stream)
                var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("captured_image.png", CreationCollisionOption.GenerateUniqueName);

                using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                    encoder.SetSoftwareBitmap(softwareBitmap);
                    await encoder.FlushAsync();
                }


                var transform = new BitmapTransform
                {
                    Bounds = new BitmapBounds
                    {
                        X = 0,
                        Y = 0,
                        Width = width,
                        Height = height
                    }
                };

                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    transform,
                    ExifOrientationMode.RespectExifOrientation,
                    ColorManagementMode.ColorManageToSRgb
                );

                byte[] pixels = pixelData.DetachPixelData();
                int totalRed = 0, totalGreen = 0, totalBlue = 0;

                for (int i = 0; i < pixels.Length; i += step)
                {
                    totalBlue += pixels[i];
                    totalGreen += pixels[i + 1];
                    totalRed += pixels[i + 2];
                }

                int pixelCount = pixels.Length / 4;  // Each pixel has 4 bytes (RGBA)

                byte averageRed = (byte)(totalRed / pixelCount);
                byte averageGreen = (byte)(totalGreen / pixelCount);
                byte averageBlue = (byte)(totalBlue / pixelCount);

                Windows.UI.Color averageColor = Windows.UI.Color.FromArgb(255, averageRed, averageGreen, averageBlue);

                ColorsHelper.RGBtoHSL(averageColor.R, averageColor.G, averageColor.B, out double H, out double S, out double L);

                await file.DeleteAsync();

                if (averageColor == Windows.UI.Color.FromArgb(255, 0, 0, 0))
                {
                    averageColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);
                }
                else if (averageColor == Windows.UI.Color.FromArgb(255, 255, 255, 255))
                {
                    averageColor = Windows.UI.Color.FromArgb(255, 243, 243, 243);
                }

                return averageColor;
            }
            catch { }

            return null;
        }
    }
}
