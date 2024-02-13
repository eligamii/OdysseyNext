﻿using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Odyssey.Data.Settings;
using Odyssey.FWebView;
using Microsoft.UI.Xaml;
using System;
using Microsoft.Graphics.Canvas.Text;
using Odyssey.Helpers;
using System.Linq;
using Odyssey.FWebView.Helpers;
using Microsoft.UI.Xaml.Controls.Primitives;
using Odyssey.Controls;

namespace Odyssey.Views
{
    public sealed partial class MainView
    {
        private void WebView_CurrentlySelectedWebViewEventTriggered(CoreWebView2 sender, CurrentlySelectedWebViewEventTriggeredEventArgs args)
        {
            switch(args.EventType)
            {
                case EventType.FullScreen: FullScreenEvent(sender); return;
                case EventType.DocumentTitleChanged: DocumentTitleChangedEvent(sender); return;
                case EventType.SourceChanged: SourceChangedEvent(sender); return;
                case EventType.StatusBarTextChanged: StatusBarTextChanged(sender); return;
                case EventType.KeyDown: KeyDown(args.Args as WebView2KeyDownHelpers.KeyDownListener.KeyDownPressedEventArgs); return;
            }
        }


        private void KeyDown(WebView2KeyDownHelpers.KeyDownListener.KeyDownPressedEventArgs args)
        {
            switch(args.PressedKey)
            {
                case Windows.System.VirtualKey.Space:
                    if(args.IsAltKeyPressed)
                    {
                        SearchBar searchBar = new SearchBar();
                        FlyoutShowOptions options = new FlyoutShowOptions();
                        options.Placement = FlyoutPlacementMode.Bottom;
                        options.Position = new Windows.Foundation.Point(splitViewContentFrame.ActualWidth / 2, 100);
                        searchBar.ShowAt(splitViewContentFrame, options);
                    }
                    break;

            }
        }

        private void StatusBarTextChanged(CoreWebView2 sender)
        {
            statusBar.SetText(sender.StatusBarText);
        }

        private void SourceChangedEvent(CoreWebView2 sender)
        {
            if (Settings.ShowHostInsteadOfDocumentTitle)
            {

               string text = new Uri(sender.Source).Host;
                CanvasTextFormat format = new()
                {
                    FontFamily = "Segoe UI Variable",
                    FontSize = (float)documentTitle.FontSize,
                    FontWeight = documentTitle.FontWeight,
                    FontStretch = documentTitle.FontStretch,
                    FontStyle = documentTitle.FontStyle
                };

                documentTitle.Text = TextHelpers.TrimTextToDesiredWidth(text, format, this.ActualWidth - (buttonsStackPanel.Children.Count() * 38 + 50) * 2);
            
            }

            urlTextBox.Text = sender.Source;
        }

        private void DocumentTitleChangedEvent(CoreWebView2 sender)
        {
            if (!Settings.ShowHostInsteadOfDocumentTitle)
            {
                string text = sender.DocumentTitle;
                CanvasTextFormat format = new()
                {
                    FontFamily = "Segoe UI Variable",
                    FontSize = (float)documentTitle.FontSize,
                    FontWeight = documentTitle.FontWeight,
                    FontStretch = documentTitle.FontStretch,
                    FontStyle = documentTitle.FontStyle
                };

                documentTitle.Text = TextHelpers.TrimTextToDesiredWidth(text, format, this.ActualWidth - (buttonsStackPanel.Children.Count() * 38 + 50) * 2);
            }
        }

        private void FullScreenEvent(CoreWebView2 sender)
        {
            if (sender.ContainsFullScreenElement)
            {
                MainWindow.Current.PresenterKind = AppWindowPresenterKind.FullScreen;
                SplitView.DisplayMode = SplitViewDisplayMode.Inline;

                AppTitleBar.Visibility = Visibility.Collapsed;
                _wasOpened = SplitView.IsPaneOpen;
                SplitView.IsPaneOpen = false;

                SplitView.PaneOpened += SplitView_PaneOpened;
                SplitView.PaneClosed += SplitView_PaneClosedOnFullScreen;
            }
            else
            {
                SplitView.PaneOpened -= SplitView_PaneOpened;
                SplitView.PaneClosed -= SplitView_PaneClosedOnFullScreen;

                MainWindow.Current.PresenterKind = AppWindowPresenterKind.Default;
                SplitView.DisplayMode = Settings.IsPaneLocked ? SplitViewDisplayMode.Inline : SplitViewDisplayMode.Overlay;

                AppTitleBar.Visibility = Visibility.Visible;
                SplitView.IsPaneOpen = _wasOpened;

            }
        }
    }
}
