using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;

namespace Odyssey.Helpers
{
    class ContextMenuIconsHelper
    {
        // Set as much icons manually as possible (FontIcon) for better quality and to match with the actual fluent icons
        // See the "redo" icon in Edge for example to see why
        public static async void SetIcon(object newItem, CoreWebView2ContextMenuItem current)
        {
            FontIcon fontIcon = new FontIcon() { FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons") };
            string glyph = "";
            ImageIcon icon = null;

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

                default:
                    if(current.Icon != null)
                    {
                        BitmapImage image = new();
                        await image.SetSourceAsync(current.Icon);

                        icon = new();
                        icon.Source = image;
                    }
                    else
                    {
                        icon = null;
                    }
                    break;
            }

            fontIcon.Glyph = glyph;

            if(newItem != null)
            {
                if (newItem.GetType() == typeof(AppBarButton) || newItem.GetType() == typeof(AppBarToggleButton))
                {
                    ((AppBarButton)newItem).Icon = glyph == string.Empty ? icon : fontIcon;
                }
                else if (newItem.GetType() != typeof(AppBarSeparator))
                {
                    ((MenuFlyoutItem)newItem).Icon = glyph == string.Empty ? icon : fontIcon;
                }
            }
        }
    }
}
