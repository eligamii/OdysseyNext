using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView.Controls
{
    public sealed partial class DownloadItem : ListViewItem
    {
        private class DownloadData : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private bool isProgressActive = false;
            private bool isProgressIntermeinate = true;

            private double progressValue = 0;
            private string title = string.Empty;
            private string subtitle = string.Empty;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public bool IsProgressActive
            {
                get
                {
                    return isProgressActive;
                }
                set
                {
                    if (value != isProgressActive)
                    {
                        isProgressActive = value;
                        OnPropertyChanged();
                    }
                }
            }
            public bool IsProgressIntermeinate
            {
                get
                {
                    return isProgressIntermeinate;
                }
                set
                {
                    if (value != isProgressIntermeinate)
                    {
                        isProgressIntermeinate = value;
                        OnPropertyChanged();
                    }

                }
            }

            public double ProgressValue
            {
                get
                {
                    return progressValue;
                }
                set
                {
                    if (value != progressValue)
                    {
                        progressValue = value;
                        OnPropertyChanged();
                    }
                }
            }
            public string Title
            {
                get
                {
                    return title;
                }
                set
                {
                    if (value != title)
                    {
                        title = value;
                        OnPropertyChanged();
                    }
                }
            }
            public string Subtitle
            {
                get
                {
                    return subtitle;
                }
                set
                {
                    if (value != subtitle)
                    {
                        subtitle = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        DownloadData downloadData = new();
        public DownloadItem(Process process)
        {
            this.InitializeComponent();
            //process.OutputDataReceived += Process_OutputDataReceived; // Get the download info

            this.DataContext = downloadData;
        }

        /*
        private async void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                downloadData.Title = "Downloading...";
            });
            Aria2Download progress = Aria2Download.ToAria2DownloadProgress(e.Data);
            
            if(progress.ResultPath == string.Empty)
            {
                downloadData.IsProgressActive = true;
                if (progress.Progress == 0) downloadData.IsProgressIntermeinate = true;

                downloadData.ProgressValue = progress.Progress * 100;

                downloadData.Title = "Downloading...";
                downloadData.Subtitle = $"{progress.TotalDownloadedDataSize}/{progress.TotalFileSize} - {progress.DownloadSpeed}/s";
            }
            else
            {
                FileInfo fileInfo = new(progress.ResultPath);

                downloadData.IsProgressActive = false;
                downloadData.Title = fileInfo.Name;
                downloadData.Subtitle = fileInfo.FullName;
            }
        }
        */

        private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

        }

        private void DeleteMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowInFolderMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
