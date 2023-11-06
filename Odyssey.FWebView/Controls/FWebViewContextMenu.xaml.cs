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
    public sealed partial class FWebViewContextMenu : MenuFlyout
    {
        List<string> MoreList = new List<string>()
        {
            "webSelect",
            "webCapture",
            "other"

        };

        Microsoft.Windows.ApplicationModel.Resources.ResourceLoader resourceLoader = new();

        MenuFlyoutSubItem moreFlyout = new MenuFlyoutSubItem();



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
            PopulateContextMenu(args, menuList, this);
            //GetQuickActionItems(webView, args);

            var options = new FlyoutShowOptions() { Position = args.Location };

            this.ShowAt(webView, options);
        }

        /*
        private void GetQuickActionItems(WebView2 webView, CoreWebView2ContextMenuRequestedEventArgs args)

        {
            SymbolIconEx symbolIconEx = new SymbolIconEx();
            symbolIconEx.Symbol = SymbolEx.MusicSharing;

            bool haveItemsInTop = false;
            bool haveItemsInBottom = false;
            bool haveItemsInShowMoreOptions = false;

            foreach (var item in QuickActionItems.Items)
            {
                var menuItem = item.ReturnCorrespondingMenuFlyoutItem((FWebView)webView, args);

                if (menuItem != null)
                {
                    var showOptions = menuItem.QuickAction.ShowOptions;
                    switch (showOptions.Position)
                    {
                        case QuickActionItemShowPosition.Top:
                            this.Items.Insert(0, menuItem);
                            if (!haveItemsInTop) { this.Items.Insert(1, new MenuFlyoutSeparator()); haveItemsInTop = true; }
                            break;

                        case QuickActionItemShowPosition.BeforeInspectItem:
                            this.Items.Insert(this.Items.Count - 2, menuItem);
                            if (!haveItemsInBottom) { this.Items.Insert(this.Items.Count - 3, new MenuFlyoutSeparator()); haveItemsInBottom = true; }
                            break;

                        case QuickActionItemShowPosition.InShowMoreOptions:
                            moreFlyout.Items.Add(menuItem);
                            if (!haveItemsInShowMoreOptions) { moreFlyout.Items.Insert(1, new MenuFlyoutSeparator()); haveItemsInShowMoreOptions = true; }
                            break;
                    }
                }

            }
        }
        */

        private void PopulateContextMenu(CoreWebView2ContextMenuRequestedEventArgs args, IList<CoreWebView2ContextMenuItem> menuList, object menuFlyout)
        {
            if (menuFlyout.GetType() == typeof(FWebViewContextMenu))
            {
                moreFlyout = new MenuFlyoutSubItem() { Text = resourceLoader.GetString("ShowMoreOptions"), Icon = new FontIcon { FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons"), Glyph = "\uE10C" } };
            }

            IList<MenuFlyoutItemBase> itemList = new List<MenuFlyoutItemBase>();


            for (int i = 0; i < menuList.Count; i++)
            {

                CoreWebView2ContextMenuItem current = menuList[i];
                if (current.Kind == CoreWebView2ContextMenuItemKind.Separator)
                {
                    if(menuList.Count > 0)
                    {
                        if(menuList.Last().GetType() != typeof(MenuFlyoutSeparator))
                        {
                            MenuFlyoutSeparator sep = new MenuFlyoutSeparator();
                            itemList.Add(sep);
                        }
                    }
                    continue;
                }

                if (current.Kind == CoreWebView2ContextMenuItemKind.Submenu)
                {
                    MenuFlyoutSubItem newItem = new MenuFlyoutSubItem();
                    // The accessibility key is the key after the & in the label
                    // Replace with '_' so it is underlined in the label
                    newItem.Text = current.Label.Replace("&", "");
                    newItem.IsEnabled = current.IsEnabled;

                    ContextMenuIconsHelper.SetIcon(newItem, current);

                    if (MoreList.Contains(current.Name)) moreFlyout.Items.Add(newItem);
                    else itemList.Add(newItem);

                }
                else
                {

                    if (current.Kind == CoreWebView2ContextMenuItemKind.CheckBox
                    || current.Kind == CoreWebView2ContextMenuItemKind.Radio)
                    {
                        ToggleMenuFlyoutItem newItem = new ToggleMenuFlyoutItem();
                        // The accessibility key is the key after the & in the label
                        // Replace with '_' so it is underlined in the label
                        newItem.Text = current.Label.Replace("&", "");
                        newItem.IsEnabled = current.IsEnabled;

                        newItem.IsChecked = current.IsChecked;

                        newItem.Click += (s, ex) =>
                        {
                            args.SelectedCommandId = current.CommandId;
                        };
                        if (MoreList.Contains(current.Name)) moreFlyout.Items.Add(newItem);
                        else itemList.Add(newItem);
                    }
                    else
                    {
                        MenuFlyoutItem newItem = new MenuFlyoutItem();
                        // The accessibility key is the key after the & in the label
                        // Replace with '_' so it is underlined in the label
                        newItem.Text = current.Label.Replace("&", "");
                        newItem.IsEnabled = current.IsEnabled;

                        newItem.Click += (s, ex) =>
                        {
                            args.SelectedCommandId = current.CommandId;
                        };
                        ContextMenuIconsHelper.SetIcon(newItem, current);
                        if (MoreList.Contains(current.Name))
                            moreFlyout.Items.Add(newItem);
                        else itemList.Add(newItem);
                    }

                }

                if (current.Name == "inspectElement" && moreFlyout.Items.Count != 0)
                {
                    itemList.Insert(itemList.Count - 1, moreFlyout);
                }

                if (menuFlyout.GetType() == typeof(FWebViewContextMenu))
                {
                    var item = (FWebViewContextMenu)menuFlyout;
                    foreach (var menuItem in itemList)
                    {
                        item.Items.Add(menuItem);
                    }
                }
                else
                {
                    var item = (MenuFlyoutSubItem)menuFlyout;
                    foreach (var menuItem in itemList)
                    {
                        item.Items.Add(menuItem);
                    }
                }

                itemList.Clear();

            }


        }
    }
}
