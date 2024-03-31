using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.Migration;


namespace Odyssey.Dialogs
{
    public sealed partial class MigrateDataContentDialog : ContentDialog
    {
        public MigrateDataContentDialog()
        {
            this.InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void MigrateButton_Click(object sender, RoutedEventArgs e)
        {
            var data = Migration.Migration.Migrate(selectionView.SelectedIndex switch
            {
                0 => Browser.Arc,
                1 => Browser.Chrome,
                2 => Browser.Edge,
                3 => Browser.Opera,
                _ => new Browser()
            });

            if (data is null)
            {
                infoText.Text = "The selected browser is not installed.";
                return;
            }
            else infoText.Text = string.Empty;

            string s = "t";
        }
    }
}
