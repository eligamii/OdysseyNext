using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.Data.Settings;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Views.Options
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GeneralPage : Page
    {
        public GeneralPage()
        {
            this.InitializeComponent();

            searchEngineComboBox.SelectedIndex = Settings.SelectedSearchEngine;
            searchSuggestionToggleSwitch.IsOn = Settings.AreSearchSuggestionsEnabled;
            darkModeToggleSwitch.IsOn = Settings.IsDarkReaderEnabled;
            forceDarkModeCheckBox.IsChecked = Settings.ForceDarkReader;

        }

        private void searchEngineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.SelectedSearchEngine = searchEngineComboBox.SelectedIndex;
        }

        private void darkModeToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.IsDarkReaderEnabled = darkModeToggleSwitch.IsOn;
        }

        private async void ForceDarkModeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100); 
            Settings.ForceDarkReader = forceDarkModeCheckBox.IsChecked == true;
        }

        private void searchSuggestionToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.AreSearchSuggestionsEnabled = searchSuggestionToggleSwitch.IsOn;
        }
    }
}
