using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.Data.Settings;




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

            autoPIPToggleSwitch.IsOn = Settings.IsAutoPictureInPictureEnabled;
        }

        private void abBlockerToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            Settings.IsAdBlockerEnabled = toggleSwitch.IsOn;
        }

        private void autoPIPToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.IsAutoPictureInPictureEnabled = autoPIPToggleSwitch.IsOn;
        }
    }
}
