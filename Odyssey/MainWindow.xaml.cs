using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Web.WebView2.Core;
using Odyssey.Data.Main;
using Odyssey.Helpers;
using Odyssey.Shared.DataTemplates.Data;
using Odyssey.Views;
using System;
using System.Threading.Tasks;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public static new MainWindow Current { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Data.Main.Data.Init();

            // Use this instead of Systemackdrop = Micaackdrop(); to be able to control the color of the Window
            MicaBackdropHelper.TrySetMicaackdropTo(this);

            ExtendsContentIntoTitleBar = true;

            MinWidth = MinHeight = 500;

            // Change the size of the window to match the UWP default window size
            Width = 1040; 
            Height = 810;

            Frame rootFrame = new Frame();
            rootFrame.Navigate(typeof(MainView), null, new SuppressNavigationTransitionInfo());

            Content = rootFrame;

            // Make possible to access to MainWindow from anywhere
            Current = this;
        }
    }
}
