using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.FWebView.Helpers
{
    internal class ColorsHelper
    {
        /// <summary>
        /// Returns true if a color is considered as dark
        /// </summary>
        /// <param name="color">The color</param>
        /// <param name="maxLightValue">The maximum light value for the color to be considered as dark (between 0 and 1)</param>
        /// <returns></returns>
        internal static bool IsColorDark(Windows.UI.Color color, double maxLightValue = 0.6)
        {
            RGBtoHSL(color.R, color.G, color.B, out double H, out double S, out double L);
            return L < maxLightValue;
        }

        internal static bool IsColorGrayTint(Windows.UI.Color color)
        {
            return color.R == color.G && color.G == color.B;
        }

        internal static double HueToRGB(double v1, double v2, double vH)
        {
            if (vH < 0) vH += 1;
            if (vH > 1) vH -= 1;
            if (6 * vH < 1) return v1 + (v2 - v1) * 6 * vH;
            if (2 * vH < 1) return v2;
            if (3 * vH < 2) return v1 + (v2 - v1) * (2 / 3.0 - vH) * 6;
            return v1;
        }

        internal static void RGBtoHSL(int R, int G, int B, out double H, out double S, out double L)
        {
            double r = R / 255.0;
            double g = G / 255.0;
            double b = B / 255.0;

            double cmax = Math.Max(Math.Max(r, g), b);
            double cmin = Math.Min(Math.Min(r, g), b);
            double delta = cmax - cmin;

            if (delta == 0)
            {
                H = 0;
            }
            else if (cmax == r)
            {
                H = 60 * (((g - b) / delta) % 6);
            }
            else if (cmax == g)
            {
                H = 60 * (((b - r) / delta) + 2);
            }
            else
            {
                H = 60 * (((r - g) / delta) + 4);
            }

            L = (cmax + cmin) / 2;

            if (delta == 0)
            {
                S = 0;
            }
            else
            {
                S = delta / (1 - Math.Abs(2 * L - 1));
            }

            H = Math.Round(H, 2);
            S = Math.Round(S, 2);
            L = Math.Round(L, 2);
        }

        internal static void HSLtoRGB(double H, double S, double L, out int R, out int G, out int B)
        {
            if (S == 0)
            {
                R = (int)(L * 255);
                G = (int)(L * 255);
                B = (int)(L * 255);
            }
            else
            {
                double temp1, temp2, temp3;
                if (L < 0.5)
                {
                    temp2 = L * (1 + S);
                }
                else
                {
                    temp2 = L + S - (L * S);
                }
                temp1 = 2 * L - temp2;
                temp3 = H / 360;

                R = (int)(255 * HueToRGB(temp1, temp2, temp3 + 1 / 3.0));
                G = (int)(255 * HueToRGB(temp1, temp2, temp3));
                B = (int)(255 * HueToRGB(temp1, temp2, temp3 - 1 / 3.0));
            }
        }

        internal static Color LightEquivalent(Windows.UI.Color color, double amount)
        {
            RGBtoHSL(color.R, color.G, color.B, out double H, out double S, out double L);

            double i;

            if (L > 0.5)
                i = amount * -1;
            else i = amount;

            L = Math.Min(1.0, L + i);
            int R, G, B;
            HSLtoRGB(H, S, L, out R, out G, out B);
            return Color.FromArgb(color.A, R, G, B);
        }
    }
}
