using Microsoft.UI.Xaml;
using Odyssey.OtherWindows;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }


        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var evt = AppInstance.GetActivatedEventArgs();
            ProtocolActivatedEventArgs protocolArgs = evt as ProtocolActivatedEventArgs;

            if (protocolArgs != null)
            {
                if(protocolArgs.Uri.ToString().StartsWith("http"))
                {
                    m_window = new LittleWebWindow(protocolArgs.Uri.ToString());
                    m_window.Activate();

                    return;
                }
            }


            m_window = new MainWindow();
            m_window.Activate();

        }

        private Window m_window;
    }
}
