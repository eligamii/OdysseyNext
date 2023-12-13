using Windows.Storage;

namespace Odyssey.Data.Settings
{
    public static class Settings
    {
        public static ApplicationDataContainer Values { get; set; } = ApplicationData.Current.LocalSettings;

        public static void Init()
        {
            // Set default settings configuration
            // Individually test if each setting has a key to make adding settings without crash possible
            // Internal
            if (!Values.Values.ContainsKey("FirstLaunch")) FirstLaunch = true;
            if (!Values.Values.ContainsKey("Inititalized")) Inititalized = true;
            if (!Values.Values.ContainsKey("SuccessfullyClosed")) SuccessfullyClosed = true;

            // Personalization
            if (!Values.Values.ContainsKey("IsPaneLocked")) IsPaneLocked = true;
            if (!Values.Values.ContainsKey("CancelAppUriLaunchConfirmationDialog")) CancelAppUriLaunchConfirmationDialog = true;
            if (!Values.Values.ContainsKey("DynamicThemeEnabled")) IsDynamicThemeEnabled = true;
            if (!Values.Values.ContainsKey("IsDynamicThemeModeChangeEnabled")) IsDynamicThemeModeChangeEnabled = true;
            if (!Values.Values.ContainsKey("ThemePerformanceMode")) ThemePerformanceMode = 1;
            if (!Values.Values.ContainsKey("ThemeMode")) ThemeMode = 2;
            if (!Values.Values.ContainsKey("IsDevBarEnabled")) IsDevBarEnabled = false;

            // Dark reader
            if (!Values.Values.ContainsKey("IsDarkReaderEnabled")) IsDarkReaderEnabled = true;
            if (!Values.Values.ContainsKey("ForceDarkReader")) ForceDarkReader = false;

            // Features
            if (!Values.Values.ContainsKey("AutoPictureInPicture")) AutoPictureInPicture = true;
            if (!Values.Values.ContainsKey("PlaySoundsOnOnlyOneTab")) PlaySoundsOnOnlyOneTab = false;
            if (!Values.Values.ContainsKey("OpenTabOnStartup")) OpenTabOnStartup = false;
            if (!Values.Values.ContainsKey("IsSingleInstanceEnabled")) IsSingleInstanceEnabled = false;
            if (!Values.Values.ContainsKey("IsHisoryEncryptionEnabled")) IsHisoryEncryptionEnabled = false;

            // Misc
            if (!Values.Values.ContainsKey("AreExperimentalFeaturesEnabled")) AreExperimentalFeaturesEnabled = false;
            if (!Values.Values.ContainsKey("DisplayQACommandErrors")) DisplayQACommandErrors = true;

            // Ad blocker
            if (!Values.Values.ContainsKey("IsAdBlockerEnabled")) IsAdBlockerEnabled = true;
            if (!Values.Values.ContainsKey("AdBlockerType")) AdBlockerType = 0; // Experimental (adblock ro regex adblocker)
            if (!Values.Values.ContainsKey("IsEasylistFilterListEnabled")) IsEasylistFilterListEnabled = false;
            if (!Values.Values.ContainsKey("IsEasyprivacyFilterListEnabled")) IsEasyprivacyFilterListEnabled = false;
            if (!Values.Values.ContainsKey("IsSpam404FilterListEnabled")) IsSpam404FilterListEnabled = false;
        }

        public static bool? Inititalized
        {
            get { return (bool?)Values.Values["Inititalized"]; }
            set { Values.Values["Inititalized"] = value; }
        }

        public static bool DisplayQACommandErrors
        {
            get { return (bool)Values.Values["DisplayQACommandErrors"]; }
            set { Values.Values["DisplayQACommandErrors"] = value; }
        }

        public static bool IsEasylistFilterListEnabled
        {
            get { return (bool)Values.Values["IsEasylistFilterListEnabled"]; }
            set { Values.Values["IsEasylistFilterListEnabled"] = value; }
        }

        public static bool IsEasyprivacyFilterListEnabled
        {
            get { return (bool)Values.Values["IsEasyprivacyFilterListEnabled"]; }
            set { Values.Values["IsEasyprivacyFilterListEnabled"] = value; }
        }

        public static bool IsSpam404FilterListEnabled
        {
            get { return (bool)Values.Values["IsSpam404FilterListEnabled"]; }
            set { Values.Values["IsSpam404FilterListEnabled"] = value; }
        }

        public static bool IsDevBarEnabled
        {
            get { return (bool)Values.Values["IsDevBarEnabled"]; }
            set { Values.Values["IsDevBarEnabled"] = value; }
        }

        public static bool IsHisoryEncryptionEnabled
        {
            get { return (bool)Values.Values["IsHisoryEncryptionEnabled"]; }
            set { Values.Values["IsHisoryEncryptionEnabled"] = value; }
        }

        public static bool UseCustomAdBlockers
        {
            get { return (bool)Values.Values["UseCustomAdBlockers"]; }
            set { Values.Values["UseCustomAdBlockers"] = value; }
        }

        public static string CustomAdBlockerPath // paths separated by ','
        {
            get { return (string)Values.Values["CustomAdBlockerPath"]; }
            set { Values.Values["CustomAdBlockerPath"] = value; }
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

        public static bool IsSingleInstanceEnabled
        {
            get { return (bool)Values.Values["IsSingleInstanceEnabled"]; }
            set { Values.Values["IsSingleInstanceEnabled"] = value; }
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

        public static bool IsDynamicThemeEnabled
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

        public static bool IsAdBlockerEnabled
        {
            get { return (bool)Values.Values["IsAdBlockerEnabled"]; }
            set { Values.Values["IsAdBlockerEnabled"] = value; }
        }

        public static int AdBlockerType // 0 = host-based, 1 = AdBlock-Plus-filterlist-based
        {
            get { return (int)Values.Values["AdBlockerType"]; }
            set { Values.Values["AdBlockerType"] = value; }
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
