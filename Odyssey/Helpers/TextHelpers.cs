using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Windows.Foundation;

namespace Odyssey.Helpers
{
    public static class TextHelpers
    {
        public static Size MeasureText(string text, CanvasTextFormat textFormat, float limitedToWidth = float.PositiveInfinity, float limitedToHeight = 0.0f)
        {
            var device = CanvasDevice.GetSharedDevice();

            var layout = new CanvasTextLayout(device, text, textFormat, 8192f, limitedToHeight);

            var width = layout.DrawBounds.Width;
            var height = layout.DrawBounds.Height;

            return new Size(width, height);
        }

        public static string TrimTextToDesiredWidth(string text, CanvasTextFormat textFormat, double desiredWidth)
        {
            string modifiedText = text;
            int count = 0;

            while (MeasureText(modifiedText, textFormat).Width > desiredWidth)
            {
                count++;
                modifiedText = text.Remove(text.Length - count, count) + "...";
            }

            return modifiedText;
        }
    }
}
