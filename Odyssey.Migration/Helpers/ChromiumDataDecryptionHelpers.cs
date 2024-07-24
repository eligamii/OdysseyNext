using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Odyssey.Migration.Helpers
{
    public class ChromiumDataDecryptionHelpers
    {
        public class os_crypt
        {
            public string encrypted_key { get; set; }
        }

        public class Key // Object made to easily get the decryption key stored as plain text in a json file...
        {
            public os_crypt os_crypt { get; set; }
        }

        public static string GetKeyString(string path)
        {
            // Get decryption key stored in the browser's 'Local State' json file
            string localState = File.ReadAllText(path);

            var obj = JsonSerializer.Deserialize<Key>(localState);

            return obj.os_crypt.encrypted_key;
        }

        public static byte[] GetKey(string key)
        {
            byte[] src = Convert.FromBase64String(key);
            byte[] encryptedKey = src.Skip(5).ToArray();

            byte[] decryptedKey = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);

            return decryptedKey;
        }


        public static void Prepare(byte[] encryptedData, out byte[] nonce, out byte[] ciphertextTag)
        {
            nonce = new byte[12];
            ciphertextTag = new byte[encryptedData.Length - 3 - nonce.Length];

            Array.Copy(encryptedData, 3, nonce, 0, nonce.Length);
            Array.Copy(encryptedData, 3 + nonce.Length, ciphertextTag, 0, ciphertextTag.Length);
        }

        public static string Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
        {
            string sR = string.Empty;
            try
            {

                GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
                AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);

                cipher.Init(false, parameters);
                byte[] plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
                int retLen = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
                cipher.DoFinal(plainBytes, retLen);

                sR = Encoding.UTF8.GetString(plainBytes).TrimEnd("\r\n\0".ToCharArray());
            }
            catch { }

            return sR;
        }

        public static string Decrypt(byte[] encryptedData, string browserLocalAppDataFolder)
        {
            string path = Path.Combine(browserLocalAppDataFolder, "Local State");
            string keyString = GetKeyString(path);
            var key = GetKey(keyString);

            byte[] nonce, ciphertextTag;
            Prepare(encryptedData, out nonce, out ciphertextTag);
            string decryptedData = Decrypt(ciphertextTag, key, nonce);

            return decryptedData;
        }
    }
}
