using CommunityToolkit.WinUI.Controls;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.Data.Settings;
using Odyssey.Shared.Helpers;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.Views.Options;
using Odyssey.Views.QuickActionsDialog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;




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
            if(e.RemovedItems.Count > 0)
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
