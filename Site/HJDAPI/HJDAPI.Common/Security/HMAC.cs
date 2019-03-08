using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Common.Security
{
    public class HMAC
    {
        public static string GenHMACSecret(string key, string message, int cryptography = 1)
        {
            System.Text.Encoding encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);

            HMACMD5 hmacmd5 = new HMACMD5(keyByte);
            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);

            byte[] messageBytes = encoding.GetBytes(message);

            byte[] md5Bytes = hmacmd5.ComputeHash(messageBytes);
            var hmacmd5Str = ByteToString(md5Bytes);

            byte[] sha1Bytes = hmacsha1.ComputeHash(messageBytes);
            var hmacsha1Str = ByteToString(sha1Bytes);

            return hmacmd5Str + "  " + hmacsha1Str;
        }

        private static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }
    }
}
