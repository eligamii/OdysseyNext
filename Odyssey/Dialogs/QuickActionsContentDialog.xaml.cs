using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Data.Settings;
using Odyssey.Shared.Helpers;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Dialogs
{
    public sealed partial class QuickActionsContentDialog : ContentDialog
    {
        private static List<char> icons = null;

        public QuickActionsContentDialog()
        {
            this.InitializeComponent();

            labelTextBox.TextChanged += (s, a) => EnableSave();
            iconBox.TextSubmitted += (s, a) => EnableSave();
            enableOnWhatWebsiteTextBox.TextChanged += (s, a) => EnableSave();
            showWhenBox.TextSubmitted += (s, a) => EnableSave();
            positionBox.TextSubmitted += (s, a) => EnableSave();
            commandTextBox.TextChanged += (s, a) => EnableSave();

            InitIcons();
        }

        private void EnableSave()
        {
            saveButton.IsEnabled = !string.IsNullOrWhiteSpace(labelTextBox.Text) &&
                                   !string.IsNullOrWhiteSpace(iconBox.SelectedItem.ToString()) &&
                                   !string.IsNullOrWhiteSpace(enableOnWhatWebsiteTextBox.Text) &&
                                   !string.IsNullOrWhiteSpace(showWhenBox.SelectedItem.ToString()) &&
                                   !string.IsNullOrWhiteSpace(positionBox.SelectedItem.ToString()) &&
                                   !string.IsNullOrWhiteSpace(commandTextBox.Text);
        }

        private async void InitIcons()
        {
            if(icons == null)
            {
                icons = new();

                await Task.Factory.StartNew(() =>
                {
                    foreach (SymbolEx enu in Enum.GetValues(typeof(SymbolEx)).AsParallel())
                    {
                        icons.Add(char.ConvertFromUtf32((int)enu)[0]);
                    }
                });

                
            }
            iconBox.ItemsSource = icons;

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
            int iconPoint = char.ConvertToUtf32(iconBox.SelectedItem.ToString(), 0);
            SymbolEx symbol = (SymbolEx)iconPoint;

            QuickAction action = new();
            action.Label = labelTextBox.Text;
            action.Icon = symbol;

            action.ShowOptions = new()
            {
                Position = (Shared.Enums.QuickActionShowPosition)positionBox.SelectedIndex,
                ShowCondition = (Shared.Enums.QuickActionShowCondition)showWhenBox.SelectedIndex,
                Type = Shared.Enums.QuickActionShowType.ContextMenuItem,
                UrlRegex = enableOnWhatWebsiteTextBox.Text
            };

            action.Command = commandTextBox.Text;
            Data.Main.QuickActions.Items.Add(action);

            Hide();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

    }
}
