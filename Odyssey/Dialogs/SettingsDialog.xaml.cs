using CommunityToolkit.WinUI.Controls;
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
            segmented.SelectionChanged += PageSelectionChanged;
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

        private void PageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Segmented segmented = sender as Segmented;
            int oldIndex = segmented.Items.IndexOf(e.RemovedItems[0]);
            int newIndex = segmented.Items.IndexOf(e.AddedItems[0]);

            SlideNavigationTransitionEffect effect = oldIndex > newIndex ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight;
            var animation = new SlideNavigationTransitionInfo() { Effect = effect };

            switch (newIndex)
            {
                case 0:
                    OptionsViewFrame.Navigate(typeof(GeneralPage), null, animation); break;
                case 1:
                    OptionsViewFrame.Navigate(typeof(ApparancePage), null, animation); break;
                case 2:
                    OptionsViewFrame.Navigate(typeof(FeaturesPage), null, animation); break;
                case 3:
                    OptionsViewFrame.Navigate(typeof(MiscPage), null, animation); break;


            }
        }
    }
}
