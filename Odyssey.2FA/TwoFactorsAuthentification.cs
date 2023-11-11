using Microsoft.UI.Xaml;
using Odyssey.Shared.Helpers;
using Odyssey.TwoFactorsAuthentification.Controls;
using Odyssey.TwoFactorsAuthentification.ViewModels;
using OtpNet;
using System;
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

        private static bool dataInitialized = false;

        // https://learn.microsoft.com/en-us/windows/uwp/security/credential-locker
        private static PasswordVault vault = new PasswordVault();
        public static void Init()
        {
            // Setup the login timer (will re-ask to enter Windows Hello password after 5min) 
            loginTimer = new DispatcherTimer();
            loginTimer.Interval = TimeSpan.FromMinutes(5);
            loginTimer.Tick += (s, a) => userAuthenticifated = false;

            Data.TwoFactAuthData.Load();
        }

        private static void InitData()
        {
            if (!dataInitialized)
            {
                // If the 2FA.json file was removed
                bool shouldRestore = Data.TwoFactAuthData.Items.Count == 0;

                var credencials = Helpers.CredencialsHelper.GetCredentialsFromLocker("Odyssey2FA");
                foreach (var item in credencials)
                {
                    if (shouldRestore)
                    {
                        // Create the items
                        TwoFactAuth twoFactAuth = new()
                        {
                            Name = item.UserName.Split("/").ElementAt(0)
                        };

                        twoFactAuth.Start(Base32Encoding.ToBytes(item.UserName.Split("/").ElementAt(1)));
                        Data.TwoFactAuthData.Items.Add(twoFactAuth);
                    }
                    else
                    {
                        TwoFactAuth twoFactAuth = Data.TwoFactAuthData.Items.Where(p => p.Name == item.UserName.Split("/").ElementAt(0)).ToList().First();

                        twoFactAuth.Start(Base32Encoding.ToBytes(item.UserName.Split("/").ElementAt(1)));
                    }
                    //vault.Remove(item);
                }
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
                    InitData();
                }
            }
            else
            {
                // Reset the timer
                loginTimer.Stop();
                loginTimer.Start();
                Two2FAFlyout flyout = new();
                flyout.ShowAt(element);
                InitData();
            }
        }

        public static void Add(string name, string secret)
        {
            // Remplacement needed as the password property cannot be read
            vault.Add(new PasswordCredential("Odyssey2FA", $"{name}/{secret}", "placeholder"));

            TwoFactAuth twoFactAuth = new()
            {
                Name = name
            };

            twoFactAuth.Start(Base32Encoding.ToBytes(secret));

            Data.TwoFactAuthData.Items.Add(twoFactAuth);
        }

    }
}
