using Microsoft.UI.Xaml.Controls.Primitives;
using Odyssey.Controls;
using Odyssey.Data.Main;
using Odyssey.Helpers;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Views;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Odyssey.Classes
{
    internal static class Hotkeys
    {
        public static void Init()
        {
            SystemWideHotkeys.RegisterHotkeysForWindow(MainWindow.Current,
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_CONTROL, Key.VK_SPACE), // Open search bar
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_CONTROL, Key.VK_G), // Open pane
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_CONTROL, Key.VK_N), // New tab
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_CONTROL, Key.VK_W), // Close tab
                 new Hotkey(HOT_KEY_MODIFIERS.MOD_SHIFT, Key.VK_TAB) // Alt+Tab equivalent
            );

            SystemWideHotkeys.HotkeyTriggered += SystemWideHotkeys_HotkeyTriggered;
        }

        private static void SystemWideHotkeys_HotkeyTriggered(Key key)
        {
            if (MainWindow.Current.IsActivated) // As the hotkey can trigger even when the window is not focused / minimized
            {
                switch (key)
                {
                    case Key.VK_SPACE: // Space
                        SearchBar searchBar = new();
                        FlyoutShowOptions options = new();
                        options.Placement = FlyoutPlacementMode.Bottom;
                        options.Position = new Windows.Foundation.Point(MainView.Current.splitViewContentFrame.ActualWidth / 2, 100);
                        searchBar.ShowAt(MainView.Current.splitViewContentFrame, options);

                        break;

                    case Key.VK_G: // G
                        MainView.Current.SplitView.IsPaneOpen ^= true;
                        break;

                    case Key.VK_N: // N
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

                }
            }

        }
    }
}
