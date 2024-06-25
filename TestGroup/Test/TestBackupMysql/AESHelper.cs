using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace TestBackupMysql
{
    internal class AESHelper
    {
        private static byte[] _salt = [0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76];


        public static async Task<bool> AES_Encrypt(string sourceFile, string destination, string password)
        {
            var result = false;
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, _salt);
            using (FileStream fs = new(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4 * 1024, FileOptions.Asynchronous))
            using (FileStream output = new(destination, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 1024, FileOptions.Asynchronous))
            using (Aes alg = Aes.Create())
            {
                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);
                using (CryptoStream cs = new CryptoStream(output, alg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    await fs.CopyToAsync(cs);
                    result = true;
                }
            }
            return result;
        }

        public static async Task<bool> AES_Decrypt(string sourceFile, string destination, string password)
        {
            var result = false;
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, _salt);
            using (FileStream fs = new(sourceFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 4 * 1024, FileOptions.Asynchronous))
            using (FileStream output = new(destination, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4*1024, FileOptions.Asynchronous))
            using (Aes alg = Aes.Create())
            {
                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);
                using (CryptoStream cs = new CryptoStream(fs, alg.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    await cs.CopyToAsync(output);
                    result = true;
                }
            }
            return result;
        }

        public static string AES_Encrypt(string input, string password)
        {
            byte[] clearBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] encryptedData = AES_Encrypt(clearBytes, password);
            return Convert.ToBase64String(encryptedData);
        }

        public static byte[] AES_Encrypt(byte[] input, string password)
        {
            return AES_Encrypt(input, Encoding.UTF8.GetBytes(password));
        }

        public static byte[] AES_Encrypt(byte[] input, byte[] password)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, _salt);
            return AES_Encrypt2(input, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        public static string AES_Decrypt(string input, string password)
        {
            byte[] cipherBytes = Convert.FromBase64String(input);
            byte[] decryptedData = AES_Decrypt(cipherBytes, password);
            return System.Text.Encoding.UTF8.GetString(decryptedData);
        }

        public static byte[] AES_Decrypt(byte[] input, string password)
        {
            return AES_Decrypt(input, Encoding.UTF8.GetBytes(password));
        }

        public static byte[] AES_Decrypt(byte[] input, byte[] password)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, _salt);
            return AES_Decrypt2(input, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        static byte[] AES_Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            byte[] encryptedData = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Rijndael alg = Rijndael.Create())
                {
                    alg.Key = Key;
                    alg.IV = IV;
                    using (CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearData, 0, clearData.Length);
                        cs.Close();
                    }
                    encryptedData = ms.ToArray();
                }
            }
            return encryptedData;
        }

        static byte[] AES_Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            byte[] decryptedData = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Rijndael alg = Rijndael.Create())
                {
                    alg.Key = Key;
                    alg.IV = IV;
                    using (CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherData, 0, cipherData.Length);
                        cs.Close();
                    }
                    decryptedData = ms.ToArray();
                }
            }
            return decryptedData;
        }


        static byte[] AES_Encrypt2(byte[] clearData, byte[] Key, byte[] IV)
        {
            byte[] encryptedData = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Aes alg = Aes.Create())
                {
                    alg.Key = Key;
                    alg.IV = IV;
                    using (CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearData, 0, clearData.Length);
                        cs.Close();
                    }
                    encryptedData = ms.ToArray();
                }
            }
            return encryptedData;
        }

        static byte[] AES_Decrypt2(byte[] cipherData, byte[] Key, byte[] IV)
        {
            byte[] decryptedData = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Aes alg = Aes.Create())
                {
                    alg.Key = Key;
                    alg.IV = IV;
                    using (CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherData, 0, cipherData.Length);
                        cs.Close();
                    }
                    decryptedData = ms.ToArray();
                }
            }
            return decryptedData;
        }
    }
}
