using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace jobFindingAdmin
{
    public static class Crypt
    {
        public static string Hash(string value)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        public static string Encrypt(string plaintext)
        {
            string encryptionKey = "kXp2s5v8y/B?E(G+";
            byte[] clearBytes = Encoding.Unicode.GetBytes(plaintext);
            using(Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using(MemoryStream ms = new MemoryStream())
                {
                    using(CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    plaintext = Convert.ToBase64String(ms.ToArray());
                }
            }
            return plaintext;
        }

        public static string Decrypt(string ciphertext)
        {
            if (!string.IsNullOrEmpty(ciphertext))
            {
                string encryptionKey = "kXp2s5v8y/B?E(G+";
                ciphertext = ciphertext.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(ciphertext);
                using (Aes decryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    decryptor.Key = pdb.GetBytes(32);
                    decryptor.IV = pdb.GetBytes(16);
                    using(MemoryStream ms = new MemoryStream())
                    {
                        using(CryptoStream cs = new CryptoStream(ms, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        ciphertext = Encoding.Unicode.GetString(ms.ToArray());
                    }

                }
            }
            return ciphertext;
        }
    }
}