using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Odyssey.Shared.Helpers;
using System;




namespace Odyssey.Views.QuickActionsDialog
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageQuickActionsPage : Page
    {
        public ManageQuickActionsPage()
        {
            this.InitializeComponent();

            quickActionListView.ItemsSource = Data.Main.QuickActions.Items;
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

            return char.ConvertFromUtf32((int)SymbolEx.FavoriteStar)[0]; // Return an empty string if the conversion fails
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (SymbolEx)char.ConvertToUtf32(((char)value).ToString(), 0);
        }
    }
}
