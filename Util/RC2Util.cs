using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GPATool.Util
{
    public class RC2Util
    {
        public static String Encrypt(String key, String text)
        {
            RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
            if (key.Length > 128)
            {
                key = key.Substring(0, 128);
            }
            else if (String.IsNullOrEmpty(key))
            {
                key = "default";
            }
            else if (key.Length < 8)
            {
                key = key.PadRight(8, ' ');
            }
            byte[] PrivateKey1 = Encoding.UTF8.GetBytes(key);
            byte[] PrivateKey2 = Encoding.UTF8.GetBytes(key);
            byte[] Data = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, rc2.CreateEncryptor(PrivateKey1, PrivateKey2), CryptoStreamMode.Write);
            cs.Write(Data, 0, Data.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public static String Decrypt(String key, String value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            try
            {
                RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
                if (key.Length > 128)
                {
                    key = key.Substring(0, 128);
                }
                else if (String.IsNullOrEmpty(key))
                {
                    key = "default";
                }
                else if (key.Length < 8)
                {
                    key = key.PadRight(8, ' ');
                }
                byte[] PrivateKey1 = Encoding.UTF8.GetBytes(key);
                byte[] PrivateKey2 = Encoding.UTF8.GetBytes(key);
                byte[] data = Convert.FromBase64String(value);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, rc2.CreateDecryptor(PrivateKey1, PrivateKey2), CryptoStreamMode.Write);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
