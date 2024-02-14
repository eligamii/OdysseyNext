using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Odyssey.Helpers;
using Windows.UI;
using WinRT; 

namespace Odyssey.Classes
{
    public enum BackdropKind
    {
        Mica, MicaAlt, Acrylic
    }


    public class Backdrop
    {
        BackdropKind Kind { get; set; } = BackdropKind.Mica;

        WindowsSystemDispatcherQueueHelper m_wsdqHelper; 
        object m_backdropController;
        SystemBackdropConfiguration m_configurationSource;

        public Color FallbackColor { get { return (Color)m_backdropController.GetType().GetProperty("FallbackColor").GetValue(m_backdropController); } set { m_backdropController.GetType().GetProperty("FallbackColor").SetValue(m_backdropController, value); } }
        public Color TintColor { get { return (Color)m_backdropController.GetType().GetProperty("TintColor").GetValue(m_backdropController); } set { m_backdropController.GetType().GetProperty("TintColor").SetValue(m_backdropController, value); } }
        public float LuminosityOpacity { get { return (float)m_backdropController.GetType().GetProperty("LuminosityOpacity").GetValue(m_backdropController); } set { m_backdropController.GetType().GetProperty("LuminosityOpacity").SetValue(m_backdropController, value); } }
        public float TintOpacity { get { return (float)m_backdropController.GetType().GetProperty("TintOpacity").GetValue(m_backdropController); } set { m_backdropController.GetType().GetProperty("TintOpacity").SetValue(m_backdropController, value); } }

        public Window TargetWindow { get; }

        public Backdrop(Window targetWindow, BackdropKind? kind = null)
        {
            TargetWindow = targetWindow;
            
            if (kind != null)
            {
                Kind = (BackdropKind)kind;
                SetBackdrop((BackdropKind)kind);
            }

            
        }



        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)TargetWindow.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = SystemBackdropTheme.Default; break;
            }
        }

        public void SetBackdrop(BackdropKind kind)
        {
            m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            // Create the policy object.
            m_configurationSource = new SystemBackdropConfiguration();

            // Initial configuration state.
            m_configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            TargetWindow.Activated += TargetWindow_Activated;
            TargetWindow.Closed += TargetWindow_Closed;
            ((FrameworkElement)TargetWindow.Content).ActualThemeChanged += Backdrop_ActualThemeChanged;

            // Enable the system backdrop.
            if (kind == BackdropKind.Acrylic)
            {
                m_backdropController = new DesktopAcrylicController();

                (m_backdropController as DesktopAcrylicController).AddSystemBackdropTarget(TargetWindow.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                (m_backdropController as DesktopAcrylicController).SetSystemBackdropConfiguration(m_configurationSource);
            }
            else
            {
                m_backdropController = new MicaController();
                (m_backdropController as MicaController).AddSystemBackdropTarget(TargetWindow.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                (m_backdropController as MicaController).SetSystemBackdropConfiguration(m_configurationSource);
                

                (m_backdropController as MicaController).Kind = (MicaKind)kind;
            }
        }

        private void Backdrop_ActualThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void TargetWindow_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed
            // so it doesn't try to use this closed window.
            if (m_backdropController != null)
            {
                // Get property by name as the m_backdropController can be MicaController or DesktopAcrylicController
                m_backdropController.GetType().GetMethod("Dispose").Invoke(m_backdropController, null);
                m_backdropController = null;
            }
            TargetWindow.Activated -= TargetWindow_Activated;
            m_configurationSource = null;
        }

        private void TargetWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }
    }
}
