using System;
using System.Collections.Generic;
using Windows.Security.Credentials;

namespace Odyssey.TwoFactorsAuthentification.Helpers
{
    internal class CredencialsHelper
    {
        // https://learn.microsoft.com/en-us/windows/uwp/security/credential-locker
        // Modified code from GetCredentialFromLocker()
        public static IReadOnlyList<PasswordCredential> GetCredentialsFromLocker(string ressourceName)
        {
            var vault = new PasswordVault();

            IReadOnlyList<PasswordCredential> credentialList = null;

            try
            {
                credentialList = vault.FindAllByResource(ressourceName);
            }
            catch (Exception)
            {
                return null;
            }

            return credentialList;
        }
    }
}
