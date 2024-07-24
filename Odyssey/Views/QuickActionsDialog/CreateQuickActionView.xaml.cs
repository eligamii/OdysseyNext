using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.Data.Settings;
using Odyssey.Shared.Helpers;
using Odyssey.Shared.ViewModels.Data;




namespace Odyssey.Views.QuickActionsDialog
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateQuickActionPage : Page
    {
        SymbolEx? selectedSymbol = null;
        public CreateQuickActionPage()
        {
            this.InitializeComponent();
            labelTextBox.TextChanged += (s, a) => EnableSave();
            enableOnWhatWebsiteTextBox.TextChanged += (s, a) => EnableSave();
            showWhenBox.TextSubmitted += (s, a) => EnableSave();
            positionBox.TextSubmitted += (s, a) => EnableSave();
            commandTextBox.TextChanged += (s, a) => EnableSave();

        }

        private void EnableSave()
        {
            saveButton.IsEnabled = !string.IsNullOrWhiteSpace(labelTextBox.Text) &&
                                   !string.IsNullOrWhiteSpace(enableOnWhatWebsiteTextBox.Text) &&
                                   !string.IsNullOrWhiteSpace(showWhenBox.SelectedItem?.ToString()) &&
                                   !string.IsNullOrWhiteSpace(positionBox.SelectedItem?.ToString()) &&
                                   !string.IsNullOrWhiteSpace(commandTextBox.Text) &&
                                   selectedSymbol != null;
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            QuickAction action = new();
            action.Label = "Recherche rapide";
            action.Icon = SymbolEx.Search;

            action.ShowOptions = new()
            {
                Position = Shared.Enums.QuickActionShowPosition.Top,
                ShowCondition = Shared.Enums.QuickActionShowCondition.HasSelection,
                Type = Shared.Enums.QuickActionShowType.ContextMenuItem,
                UrlRegex = ".*"
            };

            string searchUrl = SearchEngine.ToSearchEngineObject((SearchEngines)Settings.SelectedSearchEngine).SearchUrl;
            action.Command = $"$flyout content:\"{searchUrl}<selectiontext>\" pos:<pointerpos>";
            Data.Main.QuickActions.Items.Add(action);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {


            QuickAction action = new();
            action.Label = labelTextBox.Text;
            action.Icon = (SymbolEx)selectedSymbol;

            action.ShowOptions = new()
            {
                Position = (Shared.Enums.QuickActionShowPosition)positionBox.SelectedIndex,
                ShowCondition = (Shared.Enums.QuickActionShowCondition)showWhenBox.SelectedIndex,

                UrlRegex = enableOnWhatWebsiteTextBox.Text
            };

            action.Command = commandTextBox.Text;
            Data.Main.QuickActions.Items.Add(action);


        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void iconBox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IconSelectorFlyout_SymbolSelected(SymbolEx symbol)
        {
            selectedSymbol = symbol;
        }
    }
}
