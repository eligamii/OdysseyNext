using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Controls
{
    public sealed partial class CaptionButtons : UserControl
    {
        public CaptionButtons()
        {
            this.InitializeComponent();
            Loaded += CaptionButtons_Loaded;
        }

        private void CaptionButtons_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.WindowStateChanged += Current_WindowStateChanged;
        }

        private void Current_WindowStateChanged(object sender, WindowState e)
        {
             maximizeButton.Content = e == WindowState.Maximized ? "\uE923" : "\uE922";
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.Minimize();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Current.WindowState != WindowState.Maximized)
            {
                MainWindow.Current.Maximize();
            }         
            else
            {
                MainWindow.Current.Restore();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.Close();
        }
    }
}
