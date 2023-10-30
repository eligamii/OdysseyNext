using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OtpNet;
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.TwoFactorsAuthentification.Dialogs
{
    public sealed partial class AddDialog : ContentDialog
    {
        public AddDialog()
        {
            this.InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            secretBox.Text = secretBox.Text.Replace(" ", "");
            secretBox.Text = secretBox.Text.Replace("-", "");

            if (secretBox.Text.Length >= 16)
            {
                TwoFactorsAuthentification.Add(nameBox.Text, secretBox.Text);
            }

            Hide();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void CopyGeneratedCodeButton_Click(object sender, RoutedEventArgs e)
        {
            secretBox.Text = secretBox.Text.Replace(" ", "");
            secretBox.Text = secretBox.Text.Replace("-", "");

            if (secretBox.Text.Length >= 16)
            {
                Totp totp = new(Base32Encoding.ToBytes(secretBox.Text));
                string code = totp.ComputeTotp();

                var package = new DataPackage();
                package.SetText(code);
                Clipboard.SetContent(package);
            }
        }
    }
}
