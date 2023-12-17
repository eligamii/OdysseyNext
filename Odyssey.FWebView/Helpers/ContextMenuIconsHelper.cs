using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;

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
            }

            if(glyph != string.Empty)
            {
                fontIcon.Glyph = glyph;

                if (newItem != null)
                {
                    if (newItem.GetType() == typeof(AppBarButton) || newItem.GetType() == typeof(AppBarToggleButton))
                    {
                        ((AppBarButton)newItem).Icon = fontIcon;
                    }
                    else if (newItem.GetType() != typeof(AppBarSeparator))
                    {
                        ((MenuFlyoutItem)newItem).Icon = fontIcon;
                    }
                }
            }
            else
            {
                SetIconById(newItem, current);
            }
        }

        private static void SetIconById(object newItem, CoreWebView2ContextMenuItem current)
        {
            FontIcon fontIcon = new FontIcon() { FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons") };
            string glyph = "";

            switch(current.CommandId)
            {
                case 50211: // Voice
                    glyph = "\uE720";
                    break;

                case 41005: // Spell check
                    glyph = "\uF87B";
                    break;

                case 41120: // Writing direction
                    glyph = "\uE8AB";
                    break;

                case 41000 or 41001 or 41002: // Spell check suggestions
                    glyph = "\uE73E";
                    break;

                case 50123: // Copy video screen
                    glyph = "\uEE71";
                    break;

                case 50124: // Open video in new tab
                    glyph = "\uE8A7";
                    break;

                case 50125: // PiP
                    glyph = "\uE8A7";
                    break;

                case 50131: // Media controls
                    glyph = "\uE90F";
                    break;

                case 50130: // Repeat
                    glyph = "\uE8EE";
                    break;

            }

            fontIcon.Glyph = glyph;

            if (newItem != null)
            {
                if (newItem.GetType() == typeof(AppBarButton))
                {
                    ((AppBarButton)newItem).Icon = fontIcon;
                }
                else if (newItem.GetType() == typeof(AppBarToggleButton))
                {
                    ((AppBarToggleButton)newItem).Icon = fontIcon;
                }
                else if (newItem.GetType() != typeof(AppBarSeparator))
                {
                    ((MenuFlyoutItem)newItem).Icon = fontIcon;
                }
            }
        }
    }
}
