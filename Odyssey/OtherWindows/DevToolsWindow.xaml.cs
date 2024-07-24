using Microsoft.UI.Xaml.Media;
using Odyssey.Views.AdditionalDevTools;
using WinUIEx;




namespace Odyssey.OtherWindows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DevToolsWindow : WindowEx
    {
        public DevToolsWindow()
        {
            this.InitializeComponent();
            this.SystemBackdrop = new MicaBackdrop();
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(appTitleBar);

            contentFrame.Loaded += (s, a) =>
            {
                contentFrame.Navigate(typeof(JavascriptConsolePage));
            };

        }
    }
}
