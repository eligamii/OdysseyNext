using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Odyssey.Views.QuickActionsDialog;
using System.Collections.Generic;




namespace Odyssey.Dialogs
{
    public sealed partial class QuickActionsContentDialog : ContentDialog
    {
        private static List<char> icons = null;

        public QuickActionsContentDialog()
        {
            this.InitializeComponent();

            rootFrame.Loaded += (s, a) => rootFrame.Navigate(typeof(CreateQuickActionPage));

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (rootFrame.CanGoBack) rootFrame.GoBack();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Hide();

        private void segmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                Segmented segmented = sender as Segmented;
                int oldIndex = segmented.Items.IndexOf(e.RemovedItems[0]);
                int newIndex = segmented.Items.IndexOf(e.AddedItems[0]);

                SlideNavigationTransitionEffect effect = oldIndex > newIndex ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight;
                var animation = new SlideNavigationTransitionInfo() { Effect = effect };

                switch (newIndex)
                {
                    case 0:
                        rootFrame.Navigate(typeof(ManageQuickActionsPage), null, animation); break;
                    case 1:
                        rootFrame.Navigate(typeof(CreateQuickActionPage), null, animation); break;

                }
            }
        }
    }
}
