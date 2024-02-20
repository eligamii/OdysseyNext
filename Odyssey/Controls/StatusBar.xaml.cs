using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Odyssey.Helpers;
using Odyssey.Views;
using System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls
{
    public sealed partial class StatusBar : UserControl
    {
        DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(750) };
        public StatusBar()
        {
            this.InitializeComponent();

            // Use a timer to prevent the statusbar to collapse to fast
            timer.Tick += Timer_Tick;
        }

        private async void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            Opacity = 0;

            await Task.Delay(200);
            this.Visibility = Visibility.Collapsed;
        }

        public void SetText(string str)
        {
            bool empty = string.IsNullOrWhiteSpace(str);

            if (empty)
            {
                timer.Stop();
                timer.Start();

                this.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                this.Visibility = Visibility.Visible;

                var format = new CanvasTextFormat() { FontFamily = new("Segoe UI Variable"), FontSize = 12 };
                statusBarTextBlock.Text = TextHelpers.TrimTextToDesiredWidth(str, format, MainView.CurrentlySelectedWebView.ActualWidth / 3);

                timer.Stop();
                Opacity = 1;
            }

        }


        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // invert the alignement to prevent some weird scenarios 
            this.HorizontalAlignment = this.HorizontalAlignment == HorizontalAlignment.Left ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

    }
}
