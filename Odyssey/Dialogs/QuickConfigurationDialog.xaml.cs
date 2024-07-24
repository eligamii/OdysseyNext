using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.Data.Settings;




namespace Odyssey.Dialogs
{
    public sealed partial class QuickConfigurationDialog : ContentDialog
    {
        public QuickConfigurationDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            Settings.FirstLaunch = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void SearchEngineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            int selectedIndex = comboBox.SelectedIndex;
            Settings.Values.Values["SelectedSearchEngine"] = selectedIndex;
        }
    }
}
