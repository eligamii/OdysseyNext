using CommunityToolkit.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Odyssey.Classes;
using Odyssey.Data.Settings;
using Odyssey.Dialogs;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Views.Options
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ApparancePage : Page
    {
        public ApparancePage()
        {
            this.InitializeComponent();
            dynamicThemeComboBox.IsOn = Settings.IsDynamicThemeEnabled;
            dynamicThemeModeComboBox.IsOn = Settings.IsDynamicThemeModeChangeEnabled;
            dynamicThemePerfComboBox.SelectedIndex = Settings.ThemePerformanceMode;

            customThemeExpander.IsEnabled = !dynamicThemeComboBox.IsOn;
        }

        private void dynamicThemeComboBox_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.IsDynamicThemeEnabled = dynamicThemeComboBox.IsOn;
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

            if (text.Length == 7)
            {
                UpdateTheme.UpdateThemeWith(text);
                Settings.CustomThemeColors = text;
            }
        }


        private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            var selection = sender.SelectionStart;
            char[] hexChars = { 'A', 'B', 'C', 'D', 'E', 'F' };

            if (sender.Text.Count() > 7)
            {
                sender.Text = sender.Text.Truncate(7);
            }

            if (sender.Text.ToUpper().All(c => (char.IsDigit(c) || hexChars.Contains(c)) && sender.Text.Count() == 6))
            {
                sender.Text = "#" + sender.Text.Truncate(6);
            }


            sender.Text = new string(sender.Text.ToUpper().Where(c => char.IsDigit(c) || hexChars.Contains(c) || c == '#').ToArray());

            sender.SelectionStart = selection;
        }

        private void CustomThemeButton_Click(object sender, RoutedEventArgs e)
        {
            string text = customThemeTextBox.Text;

            if (text.Length == 7)
            {
                UpdateTheme.UpdateThemeWith(text);
                Settings.CustomThemeColors = text;
            }
        }

        private async void CustomizeTitleBarButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsDialog.Current.Hide();

            var dialog = new CustomizeTitleBarItemsDialog();
            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
        }
    }
}
