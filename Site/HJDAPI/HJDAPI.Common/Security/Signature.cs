using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace HJDAPI.Common.Security
{
    public class Signature
    {
        private static string MD5Key = ConfigurationManager.AppSettings["MD5Key"];

        private static string From_BoTao_secret = "16B188DB-1490-4C01-AAA7-9BEAB91EB85F";//铂汇金融产品给我们的MD5密钥
        private static string To_BoTao_secret = "e23f34g43h54i65j67k78wa3ds12zx4c";//给铂汇金融产品准备的MD5密钥

        public static bool IsRightSignature(Int64 TimeStamp, long  sourceID, string MD5Key, string RequestType, string sign)
        {
            string sResult = GenSignature(TimeStamp, sourceID, MD5Key, RequestType);
            return sResult == sign;
        }

        public static void CheckSignature(BaseResponse r, BaseParam p)
        {
            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign) == false)
            {
                r.SignError();
            }
        }

        public static bool IsRightSignature(Int64 TimeStamp, long sourceID, string RequestType, string sign)
        {
            if (IsInTimeWindow(TimeStamp))
            {
                sign = sign.Replace(" ", "+");//wwb 直接把base64字符串中的空格  转成+号即可

                string sResult = GenSignature(TimeStamp, sourceID, MD5Key, RequestType);

                return sResult == sign;
            }
            else
            {
                return false;
            }
        }

        public static bool IsInTimeWindow(Int64 TimeStamp)
        {
           return Math.Abs(TimeStamp - GenTimeStamp()) < 300;  //临时取消签名时间限止

          //  return true;
        }


        /// <summary>
        /// 签名，需要先给 SourceID 和 RequestType 赋值
        /// </summary>
        /// <param name="param"></param>
        public static void SignBaseParam(BaseParam param, string RequestType, long   SourceID)
        {
            param.TimeStamp = GenTimeStamp();
            param.RequestType = RequestType;
            param.SourceID = SourceID;
            param.Sign = Signature.GenSignature(param.TimeStamp, param.SourceID, MD5Key, param.RequestType);
        }


        
        public static string GenSignature(Int64 TimeStamp, long sourceID, string MD5Key, string RequestType)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string toSign = (TimeStamp.ToString() + sourceID.ToString() + MD5Key + RequestType).ToUpper();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(toSign);
            //加密Byte[]数组
            byte[] result = md5.ComputeHash(data);
            //将加密后的数组转化为字段
            //return  System.Text.Encoding.Unicode.GetString(result);
            return Convert.ToBase64String(result);
        }
         

        public static Int64 GenTimeStamp()
        {
            return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
        }

        public static string GetTestSignature(int sourceID, string secretKey, string data, string requestType)
        {
            long timeStamp = GenSince1970_01_01_00_00_00Seconds();
            string sign = GenSignature(timeStamp, sourceID, secretKey, requestType);
            string EncodeData = DES.Decrypt(data, secretKey);

            return string.Format("http://api.zmjiudian.com/api/app/TestSecurity?TimeStamp={0}&SourceID={1}&Data={2}&RequestType={3}&sign={4}", timeStamp, sourceID, EncodeData, requestType, sign);
        }

        public static string WeixinSDKSignature(string noncestr, string jsapi_ticket, long timestamp, string url)
        {
            string str1 = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsapi_ticket, noncestr, timestamp, url);
            //Membership.
            //return FormsAuthentication.HashPasswordForStoringInConfigFile(str1, "SHA1");
            return "";
        }

        public static string WeixinPayUnifiedorderSignature(string appid, string appsecret, string mch_id, string noncestr, string body, string attach, string out_trade_no, int total_fee, string spbill_create_ip, string notify_url, string trade_type, string openid)
        {
            string s1 = string.Format("appid={0}&body={1}&mch_id={2}&nonce_str={3}&notify_url={4}&openid={5}&out_trade_no={6}&spbill_create_ip={7}&total_fee={8}&trade_type={9}", appid, body, mch_id, noncestr, notify_url, openid, out_trade_no, spbill_create_ip, total_fee, trade_type);
            string s2 = string.Format("{0}&key={1}", s1, appsecret);//key是啥？ToDo
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s2);
            //加密Byte[]数组
            byte[] result = md5.ComputeHash(data);
            StringBuilder buf = new StringBuilder();
            foreach (byte b in result)
            {
                buf.Append(b.ToString("X2"));//这样就对了
            }
            return buf.ToString().ToUpper();
        }

        public static string WeixinPaySignSignature(string appId, string appsecret, string noncestr, string package, string signType, int timeStamp)
        {
            string s1 = string.Format("appId={0}&nonceStr={1}&package={2}&signType={3}&timeStamp={4}", appId, noncestr, package, signType, timeStamp);
            string s2 = string.Format("{0}&key={1}", s1, appsecret);//key是啥？ToDo
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s2);
            //加密Byte[]数组
            byte[] result = md5.ComputeHash(data);
            StringBuilder buf = new StringBuilder();
            foreach (byte b in result)
            {
                buf.Append(b.ToString("X2"));//这样就对了
            }
            return buf.ToString().ToUpper();
        }

        /// <summary>
        /// 将md5值(byte[])转成字符串   
        /// </summary>
        /// <param name="needSignatureStr"></param>
        /// <param name="toLower">true则小写 false则大写 null则无变化</param>
        /// <returns></returns>
        public static string GenMD5HexString(string needSignatureStr, bool? toLower = true)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(needSignatureStr);
            //加密Byte[]数组
            byte[] result = md5.ComputeHash(data);
            return GenBytes2Hex(result, toLower);
        }

        public static string GenBytes2Hex(byte[] bytes, bool? toLower = true)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in bytes)
            {
                if (toLower == null || !toLower.HasValue)
                {
                    sb.Append(i.ToString("x2"));
                }
                else if (!toLower.Value)
                {
                    sb.Append(i.ToString("x2").ToUpper());
                }
                else
                {
                    sb.Append(i.ToString("x2").ToLower());
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// umeng发送消息或通知加密方法
        /// </summary>
        /// <param name="http_method">默认POST</param>
        /// <param name="url">不包括?及之后的querystring</param>
        /// <param name="post_body">post参数</param>
        /// <param name="app_master_secret">umeng后台申请的</param>
        /// <returns>返回空字符串代表 缺少关键的加密参数</returns>
        public static string GenUmengSendMessageSign(string http_method, string url, string post_body, string appmastersecret)
        {
            if (string.IsNullOrWhiteSpace(http_method) || string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(post_body) || string.IsNullOrWhiteSpace(appmastersecret))
            {
                return "";
            }
            string concatStr = string.Format("{0}{1}{2}{3}", http_method.ToUpper(), url, post_body, appmastersecret);
            return Signature.GenMD5HexString(concatStr, true);
        }

        /// <summary>
        /// 整型数字(当前协调世界时距离1970年一月一日零时零分零秒的秒数)
        /// </summary>
        /// <returns></returns>
        public static long GenSince1970_01_01_00_00_00Seconds()
        {
            return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
        }

        /// <summary>
        /// 按参数key的ASCII字典序升序排列 大小字母ASCII码小于小写字母 A65 a97
        /// 无Value的key 不参与加密
        /// </summary>
        /// <param name="dic">参数键值对字典</param>
        /// <returns></returns>
        public static string SortKeyNameByASCIIDic(Dictionary<string, string> dic, bool sortAsc = true)
        {
            string sortedStr = "";
            StringBuilder sb = new StringBuilder();
            if (dic != null && dic.Keys != null && dic.Keys.Count != 0)
            {
                var newDic = sortAsc ? dic.OrderBy(_ => _.Key).ToDictionary(r => r.Key, r => r.Value) :
                    dic.OrderByDescending(_ => _.Key).ToDictionary(r => r.Key, r => r.Value);
                foreach (var item in newDic)
                {
                    //无Value的key 不参与加密
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        if (item.Value.Contains("{"))
                        {
                            sb.Append(item.Key + "=" + item.Value);
                        }
                        else
                        {
                            sb.AppendFormat(string.Format("{0}={1}&", item.Key, item.Value));
                        }
                    }
                }
                sortedStr = sb.ToString().Trim('&');//去除末尾&字符
            }
            return sortedStr;
        }

        public static bool VerifySignFromBoTao(Dictionary<string, string> dic, bool sortAsc, string sign, out string resultSign)
        {
            string stringA = SortKeyNameByASCIIDic(dic, sortAsc);
            string toSignTempStr = stringA + "&key=" + To_BoTao_secret;

            resultSign = GenMD5HexString(toSignTempStr, false);
            return resultSign == sign.Trim();
        }

        public static string CalculateBoTaoSign(Dictionary<string, string> dic, bool sortAsc, string apikey = null)
        {
            string stringA = SortKeyNameByASCIIDic(dic, sortAsc);
            string toSignTempStr = stringA + "&key=" + (string.IsNullOrWhiteSpace(apikey) ? From_BoTao_secret : apikey);

            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(toSignTempStr);
            //加密Byte[]数组
            byte[] result = md5.ComputeHash(data, 0, toSignTempStr.Length);
            return GenBytes2Hex(result, false);
        }
    }
}