using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Odyssey.Data.Main;
using Odyssey.FWebView;
using Odyssey.QuickActions;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Views;

namespace Odyssey.Controls
{
    public static class TitleBarButtons
    {

        public class UITitleBarButton : Button
        {
            public int Id { get; set; }
            public new string Command { get; set; }

            public UITitleBarButton(int id)
            {
                Background = new SolidColorBrush(Colors.Transparent);
                BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                FontFamily = new FontFamily("Segoe Fluent Icons");
                Id = id;
            }

            public UITitleBarButton()
            {
                Background = new SolidColorBrush(Colors.Transparent);
                BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                FontFamily = new FontFamily("Segoe Fluent Icons");
            }
        }

        public static void AddButtonsTo(Panel panel, bool placeholder)
        {
            foreach (var b in Data.Main.TitleBarButtons.Items)
            {
                var button = CreateButton(placeholder, b.Id, b.Icon, b.Command);
                panel.Children.Insert(0, button);
            }
        }

        private static UITitleBarButton CreateButton(bool placeholder, int id, string icon, string command = null)
        {
            UITitleBarButton button = new();
            if (!placeholder) button.Click += Button_Click;
            button.Content = icon;
            button.Id = id;
            button.Command = command;

            return button;
        }

        private static void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as UITitleBarButton;
            int id = button.Id;

            switch (id)
            {
                case -1: Command(button.Command); break;

                case 0: Back(); break;
                case 1: Forward(); break;
                case 2: Refresh(); break;
                case 3: Search(); break;
                case 4: History(); break;
                case 5: Downloads(); break;
                case 6: Favorite(); break;
                case 7: Pin(); break;
                case 8: NewTab(); break;
                case 9: TogglePane(); break;
            }
        }

        private static void TogglePane()
        {
            MainView.Current.SplitView.IsPaneOpen ^= true;
        }

        private static void NewTab()
        {
            SearchBar searchBar = new(true);
            FlyoutShowOptions options = new();
            options.Placement = FlyoutPlacementMode.Bottom;

            options.Position = new Windows.Foundation.Point(MainView.Current.splitViewContentFrame.ActualWidth / 2, 100);

            searchBar.ShowAt(MainView.Current.splitViewContentFrame, options);
        }

        private static void Back()
        {
            if (MainView.CurrentlySelectedWebView != null)
            {
                if (MainView.CurrentlySelectedWebView.CanGoBack) MainView.CurrentlySelectedWebView.GoBack();
            }
        }

        private static void Forward()
        {
            if (MainView.CurrentlySelectedWebView != null)
            {
                if (MainView.CurrentlySelectedWebView.CanGoForward) MainView.CurrentlySelectedWebView.GoForward();
            }
        }

        private static void Refresh()
        {
            if (MainView.CurrentlySelectedWebView != null)
            {
                MainView.CurrentlySelectedWebView.Reload();
            }
        }

        private static void Search()
        {
            SearchBar searchBar = new();
            FlyoutShowOptions options = new();
            options.Placement = FlyoutPlacementMode.Bottom;

            options.Position = new(MainView.Current.splitViewContentFrame.ActualWidth / 2, 100);

            searchBar.ShowAt(MainView.Current.splitViewContentFrame, options);
        }

        private static void History()
        {
            WebView.OpenHistoryDialog();
        }

        private static void Downloads()
        {
            WebView.OpenDownloadDialog();
        }

        private static void Favorite()
        {
            if (PaneView.Current.TabsView.SelectedIndex != -1)
            {
                Tab itam = PaneView.Current.TabsView.SelectedItem as Tab;
                Favorite favorite = new()
                {
                    Url = itam.Url,
                    ImageSource = itam.ImageSource
                };

                if (itam.MainWebView != null)
                {
                    favorite.MainWebView = itam.MainWebView;
                }

                Favorites.Items.Add(favorite);
            }
        }

        private static void Pin()
        {
            if (PaneView.Current.TabsView.SelectedIndex == -1)
            {
                Tab item = PaneView.Current.TabsView.SelectedItem as Tab;

                Pin pin = new Pin()
                {
                    MainWebView = item.MainWebView,
                    Title = item.Title,
                    ImageSource = item.ImageSource,
                    Url = item.Url
                };
                if (pin.MainWebView is not null) ((FWebView.WebView)pin.MainWebView).LinkedTab = pin;

                Pins.Items.Add(pin);
                PaneView.Current.PinsTabView.ItemsSource = Pins.Items;
                PaneView.Current.TabsView.SelectedItem = pin;

                Tabs.Items.Remove(item);
            }


        }
        private static async void Command(string cmd)
        {
            await QACommands.Execute(cmd);
        }
    }
}
