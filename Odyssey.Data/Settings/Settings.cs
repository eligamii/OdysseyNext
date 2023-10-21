using Windows.Foundation.Collections;
using Windows.Storage;

namespace Odyssey.Data.Settings
{
    public static class Settings
    {
        public static ApplicationDataContainer Values { get; set; } = ApplicationData.Current.LocalSettings;

        public static bool? IsPaneLocked
        {
            get { return Values.Values["PaneLocked"] as bool?; }
            set { Values.Values["PaneLocked"] = value; }
        }

        public static bool? FirstLaunch
        {
            get { return Values.Values["FirstLaunch"] as bool?; }
            set { Values.Values["FirstLaunch"] = value; }
        }

        public static int SelectedSearchEngine
        {
            get { return (int)Values.Values["SelectedSearchEngine"]; }
            set { Values.Values["SelectedSearchEngine"] = value; }
        }


    }
}
