// <copyright file="Helper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="AutomationTestingProgram.Helper" />.
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// The Cleanse.
        /// </summary>
        /// <param name="text">The text<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string Cleanse(string text)
        {
            char[] characterToTrim = { '\n', '\t', '\b', '\r', '\f' };
            text = text.Trim();
            text = text.Trim(characterToTrim);
            return text;
        }

        // reference: https://www.codeproject.com/Articles/769741/Csharp-AES-bits-Encryption-Library-with-Salt

        /// <summary>
        /// Encrypt a string given a password.
        /// </summary>
        /// <param name="text">String to be encrypted.</param>
        /// <param name="password">Password to encrypt the string with.</param>
        /// <returns>Base64 encoded string.</returns>
        public static string EncryptString(string text, string password)
        {
            byte[] baPwd = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            byte[] baPwdHash = SHA256Managed.Create().ComputeHash(baPwd);

            byte[] baText = Encoding.UTF8.GetBytes(text);

            byte[] baSalt = GetRandomBytes();
            byte[] baEncrypted = new byte[baSalt.Length + baText.Length];

            // Combine Salt + Text
            for (int i = 0; i < baSalt.Length; i++)
            {
                baEncrypted[i] = baSalt[i];
            }

            for (int i = 0; i < baText.Length; i++)
            {
                baEncrypted[i + baSalt.Length] = baText[i];
            }

            baEncrypted = AES_Encrypt(baEncrypted, baPwdHash);

            string result = Convert.ToBase64String(baEncrypted);
            return result;
        }

        /// <summary>
        /// Decrypts a string given the password.
        /// </summary>
        /// <param name="text">The encyrpted string to be decrypted.</param>
        /// <param name="password">Password to decrypt string.</param>
        /// <returns>Decrypted string.</returns>
        public static string DecryptString(string text, string password)
        {
            byte[] baPwd = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            byte[] baPwdHash = SHA256Managed.Create().ComputeHash(baPwd);

            byte[] baText = Convert.FromBase64String(text);

            byte[] baDecrypted = AES_Decrypt(baText, baPwdHash);

            // Remove salt
            int saltLength = GetSaltLength();
            byte[] baResult = new byte[baDecrypted.Length - saltLength];
            for (int i = 0; i < baResult.Length; i++)
            {
                baResult[i] = baDecrypted[i + saltLength];
            }

            string result = Encoding.UTF8.GetString(baResult);
            return result;
        }

        private static byte[] GetRandomBytes()
        {
            int saltLength = GetSaltLength();
            byte[] ba = new byte[saltLength];
            RNGCryptoServiceProvider.Create().GetBytes(ba);
            return ba;
        }

        private static int GetSaltLength()
        {
            return 8;
        }

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aES = new RijndaelManaged())
                {
                    aES.KeySize = 256;
                    aES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aES.Key = key.GetBytes(aES.KeySize / 8);
                    aES.IV = key.GetBytes(aES.BlockSize / 8);

                    aES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aES = new RijndaelManaged())
                {
                    aES.KeySize = 256;
                    aES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aES.Key = key.GetBytes(aES.KeySize / 8);
                    aES.IV = key.GetBytes(aES.BlockSize / 8);

                    aES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        try
                        {
                            cs.Close();
                        }
                        catch
                        {
                            Logger.Info("Error with helper function AES Decrypt");
                        }
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}
