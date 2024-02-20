using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Odyssey.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;




namespace Odyssey.Controls.Flyouts
{
    public sealed partial class IconSelectorFlyout : Flyout
    {
        private static List<SymbolEx> symbols;
        private List<SymbolEx> filteredSymbols;

        public delegate void SymbolSelectedEventHandler(SymbolEx symbol);
        public event SymbolSelectedEventHandler SymbolSelected;

        public IconSelectorFlyout()
        {
            this.InitializeComponent();
            symbols = filteredSymbols = Enum.GetValues(typeof(SymbolEx)).OfType<SymbolEx>().ToList();

            SymbolSelected += (x) => { };
        }

        private void iconsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            iconsGridView.ItemsSource = filteredSymbols;
        }

        private void searchBox_TextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            filteredSymbols = symbols.Where(p => Enum.GetName(typeof(SymbolEx), p).ToLower().Contains(searchBox.Text.ToLower())).ToList();

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                searchBox.ItemsSource = filteredSymbols.Select(p => p.ToString()).Take(3);
            }

            if (filteredSymbols.Count() > 0)
            {
                iconsGridView.SelectedItem = filteredSymbols[0];
                iconsGridView.ScrollIntoView(iconsGridView.SelectedItem);
            }
        }

        private void iconsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count() != 0)
            {
                SymbolEx symbol = (SymbolEx)e.AddedItems[0];
                SymbolSelected(symbol);
            }
        }

        private void searchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                if (filteredSymbols.Count() > 0)
                {
                    iconsGridView.SelectedItem = filteredSymbols.Where(p => p.ToString() == args.ChosenSuggestion.ToString());
                    iconsGridView.ScrollIntoView(iconsGridView.SelectedItem);
                }
            }
        }
    }

    public class EnumToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is SymbolEx symbol)
            {
                return char.ConvertFromUtf32((int)value)[0];
            }

            return char.ConvertFromUtf32((int)SymbolEx.FavoriteStar)[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
