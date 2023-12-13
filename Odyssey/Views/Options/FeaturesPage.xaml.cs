using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.Data.Settings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Views.Options
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FeaturesPage : Page
    {
        public FeaturesPage()
        {
            this.InitializeComponent();
            abBlockerToggleSwitch.IsOn = Settings.IsAdBlockerEnabled;

            easylistItem.IsChecked = Settings.IsEasylistFilterListEnabled;
            easyprivacyItem.IsChecked = Settings.IsEasyprivacyFilterListEnabled;
            spam404Item.IsChecked = Settings.IsSpam404FilterListEnabled;
        }

        private void abBlockerToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            Settings.IsAdBlockerEnabled = toggleSwitch.IsOn;
        }
    }
}
