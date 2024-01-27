using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Data.Main;
using Odyssey.Shared.Helpers;




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
