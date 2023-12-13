using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Odyssey.FWebView.Helpers
{
    public class WebView2AverageColorHelper
    {
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

        private static async Task<Bitmap> GetBitmap(WebView2 webView, int width, int height)
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
            string data = await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", p);
            var deserializedData = JsonSerializer.Deserialize<DeserializerClass>(data);

            var bmp = Base64StringToBitmap(deserializedData.data);
            return bmp;
        }


        public static async Task<Windows.UI.Color> GetFirstPixelColor(WebView2 webView)
        {
            var bmp = await GetBitmap(webView, 1, 1);
            System.Drawing.Color clr = bmp.GetPixel(0, 0);
            return Windows.UI.Color.FromArgb(255, clr.R, clr.R, clr.B);
        }

        /// <summary>
        /// Get the average color of a part of a webview with the coordinate (0;0) the upper left corner of the webView
        /// </summary>
        /// <param name="webView">The webview</param>
        /// <param name="width">The width (>= 1) of the part to calculate</param>
        /// <param name="height">The height (>=1) of the part to calculate</param>
        /// <param name="step">(>=1) Greater value = less precision / better performance</param>
        public static async Task<Windows.UI.Color> GetAverageColorFrom(WebView2 webView, int width, int height, int step = 1)
        {
            // Capture an image
            var bmp = await GetBitmap(webView, width, height);

            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0; // The total number of pixel with the color calculated

            for (int x = 0; x < width; x += step)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color clr = bmp.GetPixel(x, y);
                    r += clr.R;
                    g += clr.G;
                    b += clr.B;
                    total++;
                }
            }

            //Calculate the average color
            r /= total;
            g /= total;
            b /= total;

            return Windows.UI.Color.FromArgb(255, (byte)r, (byte)g, (byte)b);

        }



    }

}
