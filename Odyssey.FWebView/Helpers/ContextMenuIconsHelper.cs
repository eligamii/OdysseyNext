using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Windows.Management.Deployment;

namespace Odyssey.Helpers
{
    class ContextMenuIconsHelper
    {
        public static void SetIcon(object newItem, CoreWebView2ContextMenuItem current)
        {
            FontIcon fontIcon = new FontIcon() { FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons") };
            string glyph = "";

            switch (current.Name)
            {
                case "copyLinkLocation" or "copyImageLocation" or "copyLink":
                    glyph = "\uE167";
                    break;

                case "saveLinkAs" or "saveImageAs" or "saveMediaAs":
                    glyph = "\uE792";
                    break;

                case "openLinkInNewWindow":
                    glyph = "\uE8A7";
                    break;

                case "back":
                    glyph = "\uE112";
                    break;

                case "forward":
                    glyph = "\uE0AD";
                    break;

                case "reload":
                    glyph = "\uE149";
                    break;

                case "saveAs":
                    glyph = "\uE792";
                    break;

                case "print":
                    glyph = "\uE749";
                    break;

                case "share":
                    glyph = "\uE72D";
                    break;

                case "webSelect":
                    glyph = "\uEF20";
                    break;

                case "webCapture":
                    glyph = "\uE924";
                    break;

                case "inspectElement":
                    glyph = "\uE15E";
                    break;

                case "emoji":
                    glyph = "\uE11D";
                    break;

                case "undo":
                    glyph = "\uE10E";
                    break;

                case "redo":
                    glyph = "\uE10D";
                    break;

                case "cut":
                    glyph = "\uE16B";
                    break;

                case "copy" or "copyImage":
                    glyph = "\uE16F";
                    break;

                case "paste" or "pasteAndMatchStyle":
                    glyph = "\uE16D";
                    break;

                case "selectAll":
                    glyph = "\uE14E";
                    break;

                case "other":
                    if (current.Label.Contains("Voice"))
                    {
                        glyph = "\uE1D6";
                    }
                    else
                    {
                        glyph = "";
                    }
                    break;
            }

            fontIcon.Glyph = glyph;

            if (newItem.GetType() == typeof(MenuFlyoutSubItem))
            {
                ((MenuFlyoutSubItem)newItem).Icon = fontIcon;
            }
            else
            {
                ((MenuFlyoutItem)newItem).Icon = fontIcon;
            }
        }
    }
}
