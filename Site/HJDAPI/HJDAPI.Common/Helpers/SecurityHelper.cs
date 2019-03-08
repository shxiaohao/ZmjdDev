using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HJDAPI.Common.Helpers
{
    public class SecurityHelper
    {
        public static string Md5(string encryptString)
        {
            var md5 = MD5.Create();
            byte[] byteOfPwd = Encoding.UTF8.GetBytes(encryptString);
            byte[] md5ByteOfPwd = md5.ComputeHash(byteOfPwd);
            string encrypte = null;
            for (int i = 0; i < md5ByteOfPwd.Length; i++)
            {
                encrypte += md5ByteOfPwd[i].ToString("x");
            }
            return encrypte;
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            var des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(encryptString);
            des.Key = Encoding.ASCII.GetBytes(encryptKey);
            des.IV = Encoding.ASCII.GetBytes(encryptKey);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            var des = new DESCryptoServiceProvider();
            var inputByteArray = new byte[decryptString.Length / 2];
            for (int x = 0; x < decryptString.Length / 2; x++)
            {
                int i = (Convert.ToInt32(decryptString.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = Encoding.ASCII.GetBytes(decryptKey);
            des.IV = Encoding.ASCII.GetBytes(decryptKey);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }
    }
}