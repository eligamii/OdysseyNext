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
        }

        private void dynamicThemeComboBox_Toggled(object sender, RoutedEventArgs e)
        {

        }

        private void dynamicThemeModeComboBox_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.IsDynamicThemeModeChangeEnabled = dynamicThemeModeComboBox.IsOn;
        }

        private void dynamicThemePerfComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.ThemePerformanceMode = dynamicThemePerfComboBox.SelectedIndex;
        }
    }
}
