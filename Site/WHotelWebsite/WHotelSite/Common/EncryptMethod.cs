using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WHotelSite.Common
{
    public class EncryptMethod
    {
        public static string GenMD5Key(string input)
        {
            return Encoding.UTF8.GetString(HJD.Framework.Encrypt.MD5.ComputeHash(Encoding.UTF8.GetBytes(input + Config.MD5KeyForOldOrder)));
        }

        public static string GenSignature4Pay(string merchantcode, string mobile, int amount, long orderId, string retUrl)
        {
            return Pay.Comm.Signature.GenSignature4Pay(merchantcode, mobile, amount, orderId, retUrl);
        }

        /// <summary>
        /// 将待加密字符串变成DES密串
        /// </summary>
        /// <param name="toEncryptStr"></param>
        /// <returns></returns>
        public static string GenDESEncryptStr(string toEncryptStr)
        {
            return HJDAPI.Common.Security.DES.Encrypt(toEncryptStr);
        }
    }
}