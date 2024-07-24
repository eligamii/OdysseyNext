using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.Shared.Helpers;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using static Odyssey.Controls.TitleBarButtons;




namespace Odyssey.Dialogs
{
    public sealed partial class CustomizeTitleBarItemsDialog : ContentDialog
    {
        public CustomizeTitleBarItemsDialog()
        {
            this.InitializeComponent();
            Loaded += CustomizeTitleBarItemsDialog_Loaded;
        }

        private void CustomizeTitleBarItemsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            AddButtonsTo(buttonsStackPanel, true);
            foreach (Button button in buttonsStackPanel.Children) button.Click += ButtonsStackPanel_RightTapped;
            _availableButtons = defaultTitleBarButtons.Where(p => !Data.Main.TitleBarButtons.Items.Any(q => q.Id == p.Id)).ToList();

            availableItems.ItemsSource = _availableButtons;
        }

        private void ButtonsStackPanel_RightTapped(object sender, object e)
        {
            MenuFlyout flyout = new();

            MenuFlyoutItem leftItem = new() { Icon = new Shared.Helpers.SymbolIconEx(Shared.Helpers.SymbolEx.Back) };
            leftItem.Click += LeftItem_Click;
            leftItem.Tag = sender;
            flyout.Items.Add(leftItem);


            MenuFlyoutItem rightItem = new() { Icon = new Shared.Helpers.SymbolIconEx(Shared.Helpers.SymbolEx.Forward) };
            rightItem.Click += RightItem_Click;
            rightItem.Tag = sender;
            flyout.Items.Add(rightItem);

            MenuFlyoutItem delItem = new() { Icon = new Shared.Helpers.SymbolIconEx(Shared.Helpers.SymbolEx.Delete) };
            delItem.Click += DelItem_Click; ;
            delItem.Tag = sender;
            flyout.Items.Add(delItem);

            flyout.ShowAt(sender as FrameworkElement);
        }

        private void DelItem_Click(object sender, RoutedEventArgs e)
        {
            var flyoutItem = sender as MenuFlyoutItem;
            Button b = flyoutItem.Tag as Button;

            int bIndex = buttonsStackPanel.Children.IndexOf(b);
            int tbIndex = buttonsStackPanel.Children.Count() - 1 - bIndex;

            TitleBarButton button = Data.Main.TitleBarButtons.Items.ElementAt(tbIndex);

            buttonsStackPanel.Children.RemoveAt(bIndex);
            Data.Main.TitleBarButtons.Items.RemoveAt(tbIndex);

            _availableButtons.Insert(0, button);
            availableItems.ItemsSource = null;
            availableItems.ItemsSource = _availableButtons;
        }

        private void RightItem_Click(object sender, RoutedEventArgs e)
        {
            var flyoutItem = sender as MenuFlyoutItem;
            Button b = flyoutItem.Tag as Button;

            int bIndex = buttonsStackPanel.Children.IndexOf(b);
            int tbIndex = buttonsStackPanel.Children.Count() - bIndex - 1;


            if (tbIndex > -1 && bIndex < buttonsStackPanel.Children.Count() - 1)
            {
                buttonsStackPanel.Children.Move((uint)bIndex, (uint)bIndex + 1);
                TitleBarButton button = Data.Main.TitleBarButtons.Items.ElementAt(tbIndex);
                Data.Main.TitleBarButtons.Items.Remove(button); Data.Main.TitleBarButtons.Items.Insert(tbIndex - 1, button);
            }



        }

        private void LeftItem_Click(object sender, RoutedEventArgs e)
        {
            var flyoutItem = sender as MenuFlyoutItem;
            Button b = flyoutItem.Tag as Button;

            int bIndex = buttonsStackPanel.Children.IndexOf(b);
            int tbIndex = buttonsStackPanel.Children.Count() - bIndex - 1;


            if (tbIndex < buttonsStackPanel.Children.Count())
            {
                buttonsStackPanel.Children.Move((uint)bIndex, (uint)bIndex - 1);
                TitleBarButton button = Data.Main.TitleBarButtons.Items.ElementAt(tbIndex);
                Data.Main.TitleBarButtons.Items.Remove(button);

                if (tbIndex == buttonsStackPanel.Children.Count - 1)
                {
                    Data.Main.TitleBarButtons.Items.Insert(tbIndex, button);
                }
                else
                {
                    Data.Main.TitleBarButtons.Items.Insert(tbIndex + 1, button);
                }
            }
        }

        private void availableItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            TitleBarButton button = e.ClickedItem as TitleBarButton;
            UITitleBarButton uITitleBarButton = new(button.Id);
            uITitleBarButton.Click += ButtonsStackPanel_RightTapped;
            uITitleBarButton.Content = button.Icon;

            buttonsStackPanel.Children.Insert(0, uITitleBarButton);
            Data.Main.TitleBarButtons.Items.Add(button);
            Data.Main.TitleBarButtons.Save();

            _availableButtons.Remove(button);
            availableItems.ItemsSource = null;
            availableItems.ItemsSource = _availableButtons;
        }


        private List<TitleBarButton> _availableButtons;
        private static List<TitleBarButton> defaultTitleBarButtons = new()
        {
            new TitleBarButton() {Title = "Back", Icon = "\uE72B", Id = 0},
            new TitleBarButton() {Title = "Forward", Icon = "\uE72A", Id = 1},
            new TitleBarButton() {Title = "Refresh", Icon = "\uE72C", Id = 2},
            new TitleBarButton() {Title = "Search", Icon = "\uE721", Id = 3},
            new TitleBarButton() {Title = "History", Icon = "\uE81C", Id = 4},
            new TitleBarButton() {Title = "Download", Icon = "\uE896", Id = 5},
            new TitleBarButton() {Title = "Favorite", Icon = "\uE734", Id = 6},
            new TitleBarButton() {Title = "Pin", Icon = "\uE718", Id = 7},
            new TitleBarButton() {Title = "New tab", Icon = "\uE710", Id = 8},
            new TitleBarButton() {Title = "Toggle pane", Icon = "\uE80F", Id = 9}
        };

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; MainView.Current.buttonsStackPanel.Children.Count() > 1; i++)
            {
                MainView.Current.buttonsStackPanel.Children.RemoveAt(0);
            }

            AddButtonsTo(MainView.Current.buttonsStackPanel, false);
            Data.Main.TitleBarButtons.Save();

            MainView.Current.titleBarDragRegions.SetDragRegionForTitleBars();

            Hide();
        }

        private SymbolEx _symbol;
        private void iconSelectorFlyout_SymbolSelected(Shared.Helpers.SymbolEx symbol) => _symbol = symbol;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TitleBarButton button = new();
            button.Id = -1;
            button.Title = labelTextBox.Text;
            button.Command = commandTextBox.Text;
            button.Icon = SymbolIconEx.SymbolExToString(_symbol);

            UITitleBarButton uITitleBarButton = new(button.Id);
            uITitleBarButton.Command = button.Command;
            uITitleBarButton.Click += ButtonsStackPanel_RightTapped;
            uITitleBarButton.Content = button.Icon;

            buttonsStackPanel.Children.Insert(0, uITitleBarButton);
            Data.Main.TitleBarButtons.Items.Add(button);
            Data.Main.TitleBarButtons.Save();

        }
    }
}
