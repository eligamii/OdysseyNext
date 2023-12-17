using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Web.WebView2.Core;
using Odyssey.Helpers;
using System.Collections.Generic;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView.Controls
{
    public sealed partial class FWebViewContextMenu : CommandBarFlyout
    {
        private readonly List<string> primaryList = new List<string>()
        {
            "back",
            "forward",
            "reload",
            "copy",
            "paste",
            "redo",
            "undo",
            "cut"
        };



        public FWebViewContextMenu()
        {
            this.InitializeComponent();
        }


        public void Show(WebView2 webView, CoreWebView2ContextMenuRequestedEventArgs args)
        {
            CoreWebView2 core = webView.CoreWebView2;

            IList<CoreWebView2ContextMenuItem> menuList = args.MenuItems;
            var deferral = args.GetDeferral();
            args.Handled = true;

            this.Closed += (s, ex) => deferral.Complete();
            PopulateContextMenu(args);
            
            var options = new FlyoutShowOptions() { Position = args.Location, Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft };
            this.AlwaysExpanded = true;

            // Clean the CommandBarFlyout from unwanted separators
            foreach(var item in SecondaryCommands)
            {
                if(item.GetType() == typeof(AppBarSeparator))
                {
                    if(SecondaryCommands.IndexOf(item) == 0 ||
                       SecondaryCommands.IndexOf(item) == SecondaryCommands.Count - 1)
                    {
                        ((AppBarSeparator)item).Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                    }
                    else if(SecondaryCommands.ElementAt(SecondaryCommands.IndexOf(item) - 1).GetType() == typeof(AppBarSeparator))
                    {
                        ((AppBarSeparator)item).Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                    }
                }
            }

            this.ShowAt(webView, options);
        }

        

        private List<MenuFlyoutItemBase> PopulateSubContextMenus(CoreWebView2ContextMenuRequestedEventArgs args, CoreWebView2ContextMenuItem current, object parent)
        {
            List<MenuFlyoutItemBase> newItem = new();

            foreach(var child in current.Children)
            {
                object newContextMenuItem = null;

                switch (child.Kind)
                {
                    case CoreWebView2ContextMenuItemKind.Separator:
                        newContextMenuItem = new MenuFlyoutSeparator();
                        break;

                    case CoreWebView2ContextMenuItemKind.CheckBox or CoreWebView2ContextMenuItemKind.Radio:
                        newContextMenuItem = new ToggleMenuFlyoutItem();
                        ((ToggleMenuFlyoutItem)newContextMenuItem).Text = child.Label.Replace("&", "");
                        ((ToggleMenuFlyoutItem)newContextMenuItem).KeyboardAcceleratorTextOverride = child.ShortcutKeyDescription;
                        ((ToggleMenuFlyoutItem)newContextMenuItem).IsChecked = child.IsChecked;
                        ((ToggleMenuFlyoutItem)newContextMenuItem).Click += (s, ex) =>
                        {
                            args.SelectedCommandId = child.CommandId;
                            this.Hide();
                        };
                        break;

                    default:
                        newContextMenuItem = new MenuFlyoutItem();
                        ((MenuFlyoutItem)newContextMenuItem).Text = child.Label.Replace("&", "");
                        ((MenuFlyoutItem)newContextMenuItem).KeyboardAcceleratorTextOverride = child.ShortcutKeyDescription;
                        ((MenuFlyoutItem)newContextMenuItem).Click += (s, ex) =>
                        {
                            args.SelectedCommandId = child.CommandId;
                            this.Hide();
                        };
                        break;
                }

                newItem.Add((MenuFlyoutItemBase)newContextMenuItem);
            }

            return newItem;
        }

        private void PopulateContextMenu(CoreWebView2ContextMenuRequestedEventArgs args)
        {
            foreach (var webView2contextMenuItem in args.MenuItems)
            {
                // The item that will be added
                object newContextMenuItem = null;

                switch (webView2contextMenuItem.Kind)
                {
                    case CoreWebView2ContextMenuItemKind.Separator:
                        newContextMenuItem = new AppBarSeparator();
                        break;

                    case CoreWebView2ContextMenuItemKind.CheckBox or CoreWebView2ContextMenuItemKind.Radio:
                        newContextMenuItem = new AppBarToggleButton();
                        ((AppBarToggleButton)newContextMenuItem).Label = webView2contextMenuItem.Label.Replace("&", "");
                        ((AppBarToggleButton)newContextMenuItem).KeyboardAcceleratorTextOverride = webView2contextMenuItem.ShortcutKeyDescription;
                        ((AppBarToggleButton)newContextMenuItem).IsChecked = webView2contextMenuItem.IsChecked;
                        ((AppBarToggleButton)newContextMenuItem).Click += (s, ex) =>
                        {
                            args.SelectedCommandId = webView2contextMenuItem.CommandId;
                        };
                        break;

                    case CoreWebView2ContextMenuItemKind.Submenu:
                        newContextMenuItem = new AppBarButton();
                        ((AppBarButton)newContextMenuItem).Label = webView2contextMenuItem.Label.Replace("&", "");
                        ((AppBarButton)newContextMenuItem).KeyboardAcceleratorTextOverride = webView2contextMenuItem.ShortcutKeyDescription;
                        var flyout = new MenuFlyout();
                        foreach(var item in PopulateSubContextMenus(args, webView2contextMenuItem, this))
                        {
                            flyout.Items.Add(item);
                        }

                        ((AppBarButton)newContextMenuItem).Flyout = flyout;
                        

                        break;

                    default:
                        newContextMenuItem = new AppBarButton();
                        ((AppBarButton)newContextMenuItem).Label = webView2contextMenuItem.Label.Replace("&", "");
                        ((AppBarButton)newContextMenuItem).KeyboardAcceleratorTextOverride = webView2contextMenuItem.ShortcutKeyDescription;
                        ((AppBarButton)newContextMenuItem).Click += (s, ex) =>
                        {
                            args.SelectedCommandId = webView2contextMenuItem.CommandId;
                        };
                        break;
                }

                ContextMenuIconsHelper.SetIcon(newContextMenuItem, webView2contextMenuItem);



                if (primaryList.Contains(webView2contextMenuItem.Name))
                {
                    this.PrimaryCommands.Add((ICommandBarElement)newContextMenuItem);
                    ((AppBarButton)newContextMenuItem).Click += (s, a) => this.Hide();
                }
                else
                {
                    this.SecondaryCommands.Add((ICommandBarElement)newContextMenuItem);
                }
            }
        }
    }
}
