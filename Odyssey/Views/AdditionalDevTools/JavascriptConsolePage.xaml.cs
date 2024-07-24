using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.Data.Main;
using Odyssey.FWebView;
using Odyssey.Shared.ViewModels.Data;
using System;




namespace Odyssey.Views.AdditionalDevTools
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class JavascriptConsolePage : Page
    {
        public JavascriptConsolePage()
        {
            this.InitializeComponent();
        }

        private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainView.CurrentlySelectedWebView != null)
            {
                string js = editor.Editor.GetText(editor.Editor.TextLength);
                await MainView.CurrentlySelectedWebView.ExecuteScriptAsync(js);
                MainWindow.Current.Activate();
            }

        }

        private void editor_LinkClicked(object sender, Uri args)
        {
            WebView web = WebView.Create(args.ToString());

            Tab tab = new()
            {
                Url = args.ToString(),
                Title = args.ToString(),
                ToolTip = args.ToString(),
                MainWebView = web
            };

            web.LinkedTab = tab;

            Tabs.Items.Add(tab);
            PaneView.Current.TabsView.SelectedItem = tab;
        }
    }
}
