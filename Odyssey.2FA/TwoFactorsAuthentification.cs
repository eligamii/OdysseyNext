using Microsoft.UI.Xaml;
using Odyssey.Shared.Helpers;
using Odyssey.Shared.ViewModels.Data;
using Odyssey.TwoFactorsAuthentification.Controls;
using Odyssey.TwoFactorsAuthentification.ViewModels;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.TwoFactorsAuthentification
{
    public static class TwoFactorsAuthentification
    {
        public static XamlRoot XamlRoot { get; set; }

        private static DispatcherTimer loginTimer;
        private static bool userAuthenticifated = false;

        internal static ObservableCollection<TwoFactAuth> Items { get; set; } = new();
        public static void Init()
        {
            // Setup the login timer (will re-ask to enter Windows Hello password after 5min) 
            loginTimer = new DispatcherTimer();
            loginTimer.Interval = TimeSpan.FromMinutes(5);
            loginTimer.Tick += (s, a) => userAuthenticifated = false;

            InitData();
        }

        private static async void InitData()
        {
            var items = await Data.Main.TwoFactorsAuthentification.Load();

            foreach(var item in items)
            {
                TwoFactAuth twoFactAuth = new(item);
                twoFactAuth.Start();

                Items.Add(twoFactAuth);
            }
        }

        public static async void ShowFlyout(FrameworkElement element)
        {
            if (!userAuthenticifated)
            {
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync(ResourceString.GetString("AccessTo2FA", "Dialogs"));
                if (consentResult.Equals(UserConsentVerificationResult.Verified))
                {
                    userAuthenticifated = true;

                    // Reset the timer
                    loginTimer.Stop();
                    loginTimer.Start();
                    Two2FAFlyout flyout = new();
                    flyout.ShowAt(element);
                }
            }
            else
            {
                // Reset the timer
                loginTimer.Stop();
                loginTimer.Start();
                Two2FAFlyout flyout = new();
                flyout.ShowAt(element);
            }
        }

        public static void Add(string name, string secret)
        {
            // Remplacement needed as the password property cannot be read
            TwoFactorAuthItem item = new()
            {
                Name = name,
                Secret = Base32Encoding.ToBytes(secret)
            };

            TwoFactAuth twoFactAuth = new(item);
            twoFactAuth.Start();

            Items.Add(twoFactAuth);
            Data.Main.TwoFactorsAuthentification.Items.Add(item);
        }

    }
}
