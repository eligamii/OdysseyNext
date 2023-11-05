using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Views.Options;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Dialogs
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public static SettingsDialog Current { get; private set; }
        public SettingsDialog()
        {
            this.InitializeComponent();
            Current = this;

            OptionsViewFrame.Loaded += (s, a) => OptionsViewFrame.Navigate(typeof(GeneralPage), null, new SuppressNavigationTransitionInfo());
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsViewFrame.GoBack();
        }

        private void OptionsViewFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
