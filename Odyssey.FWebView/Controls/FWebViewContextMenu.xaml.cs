using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Web.WebView2.Core;
using Odyssey.Helpers;
using Odyssey.Shared.Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.VoiceCommands;

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

        private WebView2 webView;
        public void Show(WebView2 webView, CoreWebView2ContextMenuRequestedEventArgs args)
        {

            this.webView = webView;
            CoreWebView2 core = webView.CoreWebView2;

            IList<CoreWebView2ContextMenuItem> menuList = args.MenuItems;
            var deferral = args.GetDeferral();
            args.Handled = true;

            this.Closed += (s, ex) => deferral.Complete();
            PopulateContextMenu(args);
            PopulateWithQuickActions(args);
            
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
                else if(item.GetType() == typeof(AppBarButton))
                {
                    if(SecondaryCommands.Where(p => p.GetType() == typeof(AppBarButton)).Where(p => ((AppBarButton)p).Label == ((AppBarButton)item).Label).Count() >= 2)
                    {
                        ((AppBarButton)item).Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                    }
                }
            }

            this.ShowAt(webView, options);

        }

        

        private void PopulateWithQuickActions(CoreWebView2ContextMenuRequestedEventArgs args)
        {
            foreach(var item in Data.Main.QuickActions.Items)
            {
                if(item.CondiditonsAreMet(args))
                {
                    AppBarButton button = new();
                    button.Label = item.Label;
                    button.Icon = new SymbolIconEx(item.Icon);
                    button.Click += async (s, a) => await QuickActions.QACommands.Execute(item.Command);

                    switch(item.ShowOptions.Position)
                    {
                        case Shared.Enums.QuickActionShowPosition.PrimaryItems:
                            PrimaryCommands.Insert(0, button);
                            break;

                        case Shared.Enums.QuickActionShowPosition.Top:
                            SecondaryCommands.Insert(0, button);
                            break;

                        case Shared.Enums.QuickActionShowPosition.BeforeInspectItem:
                            SecondaryCommands.Insert(SecondaryCommands.Count - 2, button);
                            break;
                    }
                }
            }
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
                Debug.WriteLine(webView2contextMenuItem.Label + " " + webView2contextMenuItem.CommandId);

                switch (webView2contextMenuItem.Kind)
                {
                    case CoreWebView2ContextMenuItemKind.Separator:
                        newContextMenuItem = new AppBarSeparator();
                        break;

                    case CoreWebView2ContextMenuItemKind.CheckBox or CoreWebView2ContextMenuItemKind.Radio:
                        newContextMenuItem = new AppBarToggleButton();
                        ((AppBarToggleButton)newContextMenuItem).IsEnabled = webView2contextMenuItem.IsEnabled;
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
                        ((AppBarButton)newContextMenuItem).IsEnabled = webView2contextMenuItem.IsEnabled;
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
                        ((AppBarButton)newContextMenuItem).IsEnabled = webView2contextMenuItem.IsEnabled;
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
                    if (((AppBarButton)newContextMenuItem).IsEnabled && webView2contextMenuItem.Label != string.Empty)
                    {
                        // Fix the forward button with a test
                        if(webView.CanGoForward && webView2contextMenuItem.Name == "forward")
                        {
                            this.PrimaryCommands.Add((ICommandBarElement)newContextMenuItem);
                            ((AppBarButton)newContextMenuItem).Click += (s, a) => this.Hide();
                        }
                        else if(webView2contextMenuItem.Name != "forward")
                        {
                            this.PrimaryCommands.Add((ICommandBarElement)newContextMenuItem);
                            ((AppBarButton)newContextMenuItem).Click += (s, a) => this.Hide();
                        }
                    }
                }
                else
                {
                    this.SecondaryCommands.Add((ICommandBarElement)newContextMenuItem);
                }
            }
        }
    }
}
