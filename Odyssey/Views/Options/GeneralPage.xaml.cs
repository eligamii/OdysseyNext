using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Data.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
    }
}
