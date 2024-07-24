using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Odyssey.Controls;
using Odyssey.Data.Main;
using Odyssey.FWebView.Controls.Flyouts;
using Odyssey.Helpers;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Views;
using System.Linq;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Odyssey.Classes
{
    internal static class Hotkeys
    {
        private static bool _focusMode = false;
        public static void Init()
        {
            SystemWideHotkeys.RegisterHotkeysForWindow(MainWindow.Current,
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_CONTROL, Key.VK_SPACE), // Open search bar
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_CONTROL, Key.VK_G), // Open pane
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_CONTROL, Key.VK_N), // New tab
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_CONTROL, Key.VK_W), // Close tab
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_SHIFT, Key.VK_TAB), // Alt+Tab equivalent
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_CONTROL, Key.VK_K), // Toggle focus mode
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_CONTROL, Key.VK_U)
            );

            SystemWideHotkeys.HotkeyTriggered += SystemWideHotkeys_HotkeyTriggered;
        }

        private static void SystemWideHotkeys_HotkeyTriggered(Key key)
        {
            if (MainWindow.Current.IsActivated) // As the hotkey can trigger even when the window is not focused / minimized
            {
                switch (key)
                {
                    case Key.VK_SPACE:
                        SearchBar searchBar = new();
                        FlyoutShowOptions options = new();
                        options.Placement = FlyoutPlacementMode.Bottom;
                        options.Position = new Windows.Foundation.Point(MainView.Current.splitViewContentFrame.ActualWidth / 2, 100);
                        searchBar.ShowAt(MainView.Current.splitViewContentFrame, options);

                        break;

                    case Key.VK_G:
                        MainView.Current.SplitView.IsPaneOpen ^= true;
                        break;

                    case Key.VK_N:
                        SearchBar newTabSearchBar = new(true);
                        FlyoutShowOptions options_2 = new();
                        options_2.Placement = FlyoutPlacementMode.Bottom;
                        options_2.Position = new Windows.Foundation.Point(MainView.Current.splitViewContentFrame.ActualWidth / 2, 100);
                        newTabSearchBar.ShowAt(MainView.Current.splitViewContentFrame, options_2);
                        break;

                    case Key.VK_W:
                        if (MainView.CurrentlySelectedWebView != null)
                        {
                            Tab tab = MainView.CurrentlySelectedWebView.LinkedTab;
                            if (tab.GetType() != typeof(Favorite) && tab.GetType() != typeof(Pin))
                            {
                                tab.MainWebView.Close();
                                tab.MainWebView = null;
                                Tabs.Items.Remove(tab);
                            }
                        }
                        break;

                    case Key.VK_K:
                        if(!MainView.Current.FocusModeEnabled)
                        {
                            foreach (var element in MainView.Current.AppTitleBar.Children.Where(p => p.GetType() != typeof(TextBox)))
                                element.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;

                            MainView.Current.AppTitleBar.RowDefinitions[0].Height = new Microsoft.UI.Xaml.GridLength(22);
                            MainView.Current.titleBarDragRegions.SetTitleBarsHeight(20);

                            MainView.Current.focusButton.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                            MainView.Current.FocusModeEnabled = true;
                        }
                        else
                        {
                            foreach (var element in MainView.Current.AppTitleBar.Children.Where(p => p.GetType() != typeof(TextBox)))
                                element.Visibility = Microsoft.UI.Xaml.Visibility.Visible;

                            MainView.Current.AppTitleBar.RowDefinitions[0].Height = new Microsoft.UI.Xaml.GridLength(42);
                            MainView.Current.titleBarDragRegions.SetTitleBarsHeight(42);

                            MainView.Current.focusButton.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                            MainView.Current.FocusModeEnabled = false;
                        }
                        break;


                    case Key.VK_U:
                        FindFlyout findFlyout = new(MainView.CurrentlySelectedWebView);
                        findFlyout.PreferredPlacement = TeachingTipPlacementMode.Top;
                        findFlyout.IsOpen = true;
                        break;

                }
            }

        }
    }
}
