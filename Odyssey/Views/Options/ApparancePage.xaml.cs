using CommunityToolkit.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Classes;
using Odyssey.Data.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Views.Options
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ApparanceView : Page
    {
        public ApparanceView()
        {
            this.InitializeComponent();
            dynamicThemeComboBox.IsOn = Settings.DynamicThemeEnabled;
            dynamicThemeModeComboBox.IsOn = Settings.IsDynamicThemeModeChangeEnabled;
            dynamicThemePerfComboBox.SelectedIndex = Settings.ThemePerformanceMode;

            customThemeExpander.IsEnabled = !dynamicThemeComboBox.IsOn;
        }

        private void dynamicThemeComboBox_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.DynamicThemeEnabled = dynamicThemeComboBox.IsOn;
            customThemeExpander.IsEnabled = !dynamicThemeComboBox.IsOn;
        }

        private void dynamicThemeModeComboBox_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.IsDynamicThemeModeChangeEnabled = dynamicThemeModeComboBox.IsOn;
            
        }

        private void dynamicThemePerfComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.ThemePerformanceMode = dynamicThemePerfComboBox.SelectedIndex;
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            string text = ((TextBox)sender).Text;

            if(e.Key == Windows.System.VirtualKey.Enter && text.Length == 7)
            {
                UpdateTheme.UpdateThemeWith(text);
            }
        }


        private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            var selection = sender.SelectionStart;
            char[] hexChars = { 'A', 'B', 'C', 'D', 'E', 'F' };

            if(sender.Text.Count() > 7)
            {
                sender.Text =  sender.Text.Truncate(7);
            }

            if (sender.Text.ToUpper().All(c => (char.IsDigit(c) || hexChars.Contains(c)) && sender.Text.Count() >= 6))
            {
                sender.Text = "#" + sender.Text.Truncate(6);
            }


            sender.Text = new string(sender.Text.ToUpper().Where(c => char.IsDigit(c) || hexChars.Contains(c) || c == '#').ToArray());

            sender.SelectionStart = selection;
        }
    }
}
