using Microsoft.UI.Xaml.Controls;
using Odyssey.Data.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Management.Deployment;
using Windows.System;

namespace Odyssey.FWebView.Classes
{
    public class AppUriLaunch
    {
        public static async void Launch(Uri uri)
        {
            var res = await Launcher.QueryUriSupportAsync(uri, LaunchQuerySupportType.Uri);
            
            if(res == LaunchQuerySupportStatus.Available)
            {
                if(!Settings.CancelAppUriLaunchConfirmationDialog)
                {
                    // Create and show a confirmation dialog
                    ContentDialog contentDialog = new();
                    contentDialog.Title = "Launch URI in external app";
                    contentDialog.Content = "Do you wand to launch the app assiociated with the Uri ?";
                    contentDialog.PrimaryButtonText = "Yes";
                    contentDialog.CloseButtonText = "Cancel";
                    contentDialog.SecondaryButtonText = "Never show again";

                    contentDialog.XamlRoot = WebView.XamlRoot;

                    var dialogResult = await contentDialog.ShowAsync();

                    // If the "yes" option is not selected, dont launch the app
                    if (dialogResult != ContentDialogResult.Primary && dialogResult != ContentDialogResult.Secondary)
                    {
                        return;
                    }
                    else if(dialogResult == ContentDialogResult.Secondary)
                    {
                        Settings.CancelAppUriLaunchConfirmationDialog = true;
                    }
                }

                await Launcher.LaunchUriAsync(uri);
            }
        }
    }
}
