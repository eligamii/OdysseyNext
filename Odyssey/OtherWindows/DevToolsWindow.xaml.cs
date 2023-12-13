using Microsoft.UI.Xaml.Media;
using Odyssey.Views.AdditionalDevTools;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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
