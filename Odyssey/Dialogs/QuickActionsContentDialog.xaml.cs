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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Dialogs
{
    public sealed partial class QuickActionsContentDialog : ContentDialog
    {
        public QuickActionsContentDialog()
        {
            this.InitializeComponent();
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
    }
}
