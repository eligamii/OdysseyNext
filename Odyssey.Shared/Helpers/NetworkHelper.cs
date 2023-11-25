namespace Odyssey.Shared.Helpers
{
    public static class NetworkHelper
    {
        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool IsInternetConnectionAvailable()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }
    }
}
