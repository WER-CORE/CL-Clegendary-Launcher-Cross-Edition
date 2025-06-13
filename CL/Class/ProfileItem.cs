using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CL.Class
{
    // Цей клас представляє елемент профілю користувача з властивостями для даних облікового запису та методами для шифрування і дешифрування.
    public class ProfileItem
    {
        public string NameAccount { get; set; }
        public string UUID { get; set; }
        public string AccessToken { get; set; }
        public string ImageUrl { get; set; }
        public bool OfficalAccount { get; set; }
        public static byte[] EncryptData(string plainText)
        {
            byte[] key = GetEncryptionKey();
            byte[] iv = new byte[16];

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                        }
                        return ms.ToArray();
                    }
                }
            }
        }
        public static string DecryptData(byte[] cipherText)
        {
            byte[] key = GetEncryptionKey();
            byte[] iv = new byte[16];

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream(cipherText))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
        public static byte[] GetEncryptionKey()
        {
            string base64Key = SettingsManager.Settings.EncryptedPassword;

            if (string.IsNullOrEmpty(base64Key) || Convert.FromBase64String(base64Key).Length != 16)
            {
                byte[] newKey = new byte[16];
                new Random().NextBytes(newKey);
                base64Key = Convert.ToBase64String(newKey);

                SettingsManager.Settings.EncryptedPassword = base64Key;
                SettingsManager.SaveSettings();

                return newKey;
            }
            return Convert.FromBase64String(base64Key);
        }
    }
}