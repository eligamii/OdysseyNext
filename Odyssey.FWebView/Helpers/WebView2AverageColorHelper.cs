<<<<<<< Updated upstream
﻿using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
=======
﻿extern alias webview;
using Microsoft.UI.Xaml.Controls;
using webview::Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
>>>>>>> Stashed changes
using Odyssey.Shared.Helpers;
using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Odyssey.FWebView.Helpers
{
    public class WebView2AverageColorHelper
    {
<<<<<<< Updated upstream
=======
        public class DeserializerClass
        {
            public string data { get; set; }
        }

        public static Bitmap Base64StringToBitmap(string base64String)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new(byteBuffer);
            memoryStream.Position = 0;

            Bitmap bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            return bmpReturn;
        }

        private static async Task<Bitmap> GetBitmap(CoreWebView2 webView, int width, int height)
        {
            dynamic clip = new JObject();
            clip.x = 0;
            clip.y = 0;
            clip.width = width;
            clip.height = height;
            clip.scale = 1;

            dynamic settings = new JObject();
            settings.format = "jpeg";
            settings.clip = clip;
            settings.quality = 1;
            settings.optimizeForSpeed = true;
            settings.fromSurface = false;
            settings.captureBeyondViewport = false;


            string p = settings.ToString(Newtonsoft.Json.Formatting.None);
            string data = await webView.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", p);
            var deserializedData = JsonSerializer.Deserialize<DeserializerClass>(data);

            var bmp = Base64StringToBitmap(deserializedData.data);
            return bmp;
        }


        public static async Task<Windows.UI.Color> GetFirstPixelColor(CoreWebView2 webView)
        {
            var bmp = await GetBitmap(webView, 1, 1);
            System.Drawing.Color clr = bmp.GetPixel(0, 0);
            return Windows.UI.Color.FromArgb(255, clr.R, clr.R, clr.B);
        }

>>>>>>> Stashed changes
        /// <summary>
        /// Get the average color of a part of a webview with the coordinate (0;0) the upper left corner of the webView
        /// </summary>
        /// <param name="webView">The webview</param>
        /// <param name="width">The width (>= 1) of the part to calculate</param>
        /// <param name="height">The height (>=1) of the part to calculate</param>
        /// <param name="step">(>=1) Greater value = less precision / better performance</param>
<<<<<<< Updated upstream
        internal async static Task<Windows.UI.Color?> GetWebView2AverageColorsAsync(WebView2 webView, uint width = 600, uint height = 1, int step = 1)
=======
        public static async Task<Windows.UI.Color> GetAverageColorFrom(CoreWebView2 webView, int width, int height, int step = 1)
>>>>>>> Stashed changes
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
                    try
                    {
                        totalBlue += pixels[i];
                        totalGreen += pixels[i + 1];
                        totalRed += pixels[i + 2];
                    } catch (IndexOutOfRangeException) { }
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
