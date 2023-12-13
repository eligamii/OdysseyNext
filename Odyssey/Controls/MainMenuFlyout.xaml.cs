using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Odyssey.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls
{
    public sealed partial class MainMenuFlyout : Flyout
    {
        public MainMenuFlyout()
        {
            this.InitializeComponent();
            Opening += MainMenuFlyout_Opening;
        }

        private void MainMenuFlyout_Opening(object sender, object e)
        {
            if (MainView.CurrentlySelectedWebView != null)
            {
                var info = MainView.CurrentlySelectedWebView.SecurityInformation;
            }
        }

        private void urlTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {

            }
        }

        private void urlTextBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void urlTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
