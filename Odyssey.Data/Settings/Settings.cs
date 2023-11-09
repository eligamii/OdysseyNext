using Windows.Storage;

namespace Odyssey.Data.Settings
{
    public static class Settings
    {
        public static ApplicationDataContainer Values { get; set; } = ApplicationData.Current.LocalSettings;

        public static void Init()
        {
            if (Inititalized != true)
            {
                FirstLaunch = true;
                Inititalized = true;
                IsPaneLocked = true;
                CancelAppUriLaunchConfirmationDialog = false;
                SuccessfullyClosed = true;
                IsDarkReaderEnabled = true;
                ForceDarkReader = false;
                AutoPictureInPicture = true;
                PlaySoundsOnOnlyOneTab = false;
                OpenTabOnStartup = false;
                AreExperimentalFeaturesEnabled = false;
                ThemeMode = 2; // System / Dynamic theme change
                DynamicThemeEnabled = true;
                IsDynamicThemeModeChangeEnabled = true; // if false, the color will be converted to a darker/lighter one
                ThemePerformanceMode = 1;
            }
        }

        public static bool? Inititalized
        {
            get { return (bool?)Values.Values["Inititalized"]; }
            set { Values.Values["Inititalized"] = value; }
        }

        public static bool AutoPictureInPicture 
        {
            get { return (bool)Values.Values["AutoPictureInPicture"]; }
            set { Values.Values["AutoPictureInPicture"] = value; }
        }

        public static bool PlaySoundsOnOnlyOneTab
        {
            get { return (bool)Values.Values["PlaySoundsOnOnlyOneTab"]; }
            set { Values.Values["PlaySoundsOnOnlyOneTab "] = value; }
        }

        public static bool IsPaneLocked
        {
            get { return (bool)Values.Values["IsPaneLocked"]; }
            set { Values.Values["IsPaneLocked"] = value; }
        }

        public static bool OpenTabOnStartup
        {
            get { return (bool)Values.Values["OpenTabOnStartup"]; }
            set { Values.Values["OpenTabOnStartup"] = value; }
        }

        public static bool AreExperimentalFeaturesEnabled // Native tips, find popup...
        {
            get { return (bool)Values.Values["AreExperimentalFeaturesEnabled"]; }
            set { Values.Values["AreExperimentalFeaturesEnabled"] = value; }
        }

        public static string CustomThemeColors // #ffffff format (hex)
        {
            get { return (string)Values.Values["CustomThemeColors"]; }
            set { Values.Values["CustomThemeColors"] = value; }
        }

        public static int ThemeMode // 0 = light, 1 = dark, 2 = system
        {
            get { return (int)Values.Values["ThemeMode"]; }
            set { Values.Values["ThemeMode"] = value; }
        }

        public static int ThemePerformanceMode //  2 = performance, 1 = default, 0 = quality
        {
            get { return (int)Values.Values["ThemePerformanceMode"]; }
            set { Values.Values["ThemePerformanceMode"] = value; }
        }

        public static bool DynamicThemeEnabled
        {
            get { return (bool)Values.Values["DynamicThemeEnabled"]; }
            set { Values.Values["DynamicThemeEnabled"] = value; }
        }

        public static bool IsDynamicThemeModeChangeEnabled // Dark mode when the dynamic theme is dark and vice versa
        {
            get { return (bool)Values.Values["IsDynamicThemeModeChangeEnabled"]; }
            set { Values.Values["IsDynamicThemeModeChangeEnabled"] = value; }
        }

        public static bool IsDarkReaderEnabled
        {
            get { return (bool)Values.Values["IsDarkReaderEnabled"]; }
            set { Values.Values["IsDarkReaderEnabled"] = value; }
        }
        public static bool ForceDarkReader
        {
            get { return (bool)Values.Values["ForceDarkReader"]; }
            set { Values.Values["ForceDarkReader"] = value; }
        }

        public static bool FirstLaunch
        {
            get { return (bool)Values.Values["FirstLaunch"]; }
            set { Values.Values["FirstLaunch"] = value; }
        }

        public static bool SuccessfullyClosed
        {
            get { return (bool)Values.Values["SuccessfullyClosed"]; }
            set { Values.Values["SuccessfullyClosed"] = value; }
        }

        public static int SelectedSearchEngine
        {
            get { return (int)Values.Values["SelectedSearchEngine"]; }
            set { Values.Values["SelectedSearchEngine"] = value; }
        }

        public static bool CancelAppUriLaunchConfirmationDialog
        {
            get { return (bool)Values.Values["CancelAppUriLaunchConfirmationDialog"]; }
            set { Values.Values["CancelAppUriLaunchConfirmationDialog"] = value; }
        }


    }
}
