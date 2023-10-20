using Windows.Foundation.Collections;
using Windows.Storage;

namespace Odyssey.Data.Settings
{
    public static class Settings
    {
        private static IPropertySet values = ApplicationData.Current.LocalSettings.Values;

        public static bool? IsPaneLocked
        {
            get { return values["PaneLocked"] as bool?; }
            set { values["PaneLocked"] = value; }
        }


    }
}
