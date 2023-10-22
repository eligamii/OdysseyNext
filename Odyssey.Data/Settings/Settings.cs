﻿using Windows.Foundation.Collections;
using Windows.Storage;

namespace Odyssey.Data.Settings
{
    public static class Settings
    {
        public static ApplicationDataContainer Values { get; set; } = ApplicationData.Current.LocalSettings;

        public static void Init()
        {
            if(Inititalized != true)
            {
                FirstLaunch = true;
                Inititalized = true;
                IsPaneLocked = true;
                CancelAppUriLaunchConfirmationDialog = false;
            }
        }

        public static bool? Inititalized
        {
            get { return (bool?)Values.Values["Inititalized"]; }
            set { Values.Values["Inititalized"] = value; }
        }

        public static bool IsPaneLocked
        {
            get { return (bool)Values.Values["IsPaneLocked"]; }
            set { Values.Values["IsPaneLocked"] = value; }
        }

        public static bool? FirstLaunch
        {
            get { return (bool?)Values.Values["FirstLaunch"]; }
            set { Values.Values["FirstLaunch"] = value; }
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
