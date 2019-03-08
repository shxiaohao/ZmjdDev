using HJD.AccessService.Contract.Model;
using HJD.WeixinServices.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace HJD.AccessService.Implement.Helper
{
    public class WeixinHelper
    {
        public static string FormatWeixinChatRecord(WeixinChatRecordEntity recordEntity)
        {
            var formatStr = "{0} | {1} | {2} | {3} | {4}";
            formatStr = string.Format(formatStr, 
                recordEntity.worker,
                recordEntity.openid,
                recordEntity.opercode,
                recordEntity.time,
                recordEntity.text);

            return formatStr;
        }

        public static WeixinChatRecordEntity GetBehaviorByString(string recordString)
        {
            var recordEntity = new WeixinChatRecordEntity();

            if (!string.IsNullOrEmpty(recordString))
            {
                try
                {
                    var rList = Regex.Split(recordString, " \\| ");
                    recordEntity.worker = rList[0];
                    recordEntity.openid = rList[1];
                    recordEntity.opercode = rList[2];
                    recordEntity.time = Convert.ToInt32(rList[3]);
                    recordEntity.text = rList[4];
                }
                catch (Exception ex)
                {
                    recordEntity.worker = "读取错误";
                    recordEntity.text = ex.Message;
                }
            }

            return recordEntity;
        }
    }

    public class WeixinApiHelper
    {
        #region 订阅号配置

        //订阅号配置
        public static string WeiXinAPPID = "wxb79a37b190594d96";
        public static string WeiXinSecret = "3750a02b64cfb850900053ea7f9469f1";

        public static string WeixinMchId = "1218698601";            //商户ID

        #endregion

        #region 服务号配置

        /// <summary>
        /// 服务号配置
        /// </summary>
        public static string WeixinServiceAppId = "wx52970d8f53c19ba1";
        public static string WeixinServiceSecret = "6de917afedce0c7fd2057c682102ebf6";  //AppSecret(应用密钥)6de917afedce0c7fd2057c682102ebf6

        public static string WeixinServiceMchId = "1306853401";    //商户ID

        public static string WeixinServiceApiSecret = "97cb75d342c4b05b84f3e7265c9d9c7c";   //商户API密钥

        public static string Snsapi_base = "snsapi_base";
        public static string Snsapi_userinfo = "snsapi_userinfo";

        #endregion

        /// <summary>
        /// 通过code换取网页授权access_token
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <param name="code"></param>
        public static WeixinAccessToken GetWeixinAccessToken(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, WeixinServiceAppId, WeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = DownloadHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);

            return obj;
        }

        /// <summary>
        /// 通过access_token拉取用户信息(需scope为 snsapi_userinfo)
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static WeixinUser GetWeixinUserInfo(string accessToken, string openid)
        {
            string url = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN";
            url = string.Format(url, accessToken, openid);
            CookieContainer cc = new CookieContainer();
            string json = DownloadHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinUser>(json);

            return obj;
        }

        /// <summary>
        /// 通过access_token拉取用户信息（包含是否关注当前订阅号的信息）(需scope为 snsapi_userinfo)
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static WeixinUser GetWeixinUserSubscribeInfo(string openid)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN";
            url = string.Format(url, WeiXinToken.GetToken(), openid);
            CookieContainer cc = new CookieContainer();
            var backJson = DownloadHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinUser>(backJson);

            return obj;
        }

        /// <summary>
        /// 获取微信下的所有关注用户
        /// </summary>
        /// <param name="nextopenid"></param>
        /// <returns></returns>
        public static WeixinSubscribeUser GetAllWeixinSubscribeUser(string token, string nextopenid)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid={1}";
            url = string.Format(url, token, nextopenid);
            CookieContainer cc = new CookieContainer();
            var backJson = DownloadHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinSubscribeUser>(backJson);

            return obj;
        }

        /// <summary>
        /// 获取微信下的图文素材数据
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 返回</param>
        /// <param name="count">返回素材的数量，取值在1到20之间</param>
        /// <returns></returns>
        public static WeixinMaterialListEntity GetAllWeixinMaterialList(string token, int offset, int count = 20)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}", token);
            var _param = new WeixinMaterialListParam { Type = "news", Offset = offset, Count = count };
            //string jsonStr = "{'type':'news','offset':" + offset + ",'count':" + count + "}";
            CookieContainer cc = new CookieContainer();
            var backJson = DownloadHelper.PostJson(url, _param, ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinMaterialListEntity>(backJson);

            return obj;
        }

        public static WeixinMaterialListEntity GetAllWeixinMaterial(string token, string media_id)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/material/get_material?access_token={0}", token);
            string jsonStr = "{'media_id':'" + media_id + "'}";
            CookieContainer cc = new CookieContainer();
            var backJson = DownloadHelper.PostJson(url, jsonStr, ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinMaterialListEntity>(backJson);

            return obj;
        }

        /// <summary>
        /// 传入回调url，生成返回微信静默授权url
        /// </summary>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        public static string GenSilenceAuthorUrl(string redirect_uri)
        {
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={2}&response_type=code&scope={1}&state=STATE#wechat_redirect", WeixinServiceAppId, Snsapi_base, redirect_uri);
        }

        /// <summary>
        /// 传入回调url，生成返回微信确认授权url
        /// </summary>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        public static string GenConfrimAuthorUrl(string redirect_uri)
        {
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={2}&response_type=code&scope={1}&state=STATE#wechat_redirect", WeixinServiceAppId, Snsapi_userinfo, redirect_uri);
        }

        #region 算法相关

        static int rep = 0;
        static readonly object _lock = new object();

        public static string GenerateRandomStr()
        {
            Random rand = new Random();
            int count = rand.Next(8, 30);
            return GenerateCheckCode(count);
        }

        /// <summary>
        /// 生成固定长度的随机字符串(数字加字母)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GenAssignLengthRandomStr(int count)
        {
            return GenerateCheckCode(count);
        }

        /// 生成随机字母字符串(数字字母混和)
        /// 待生成的位数
        /// 生成的字母字符串
        private static string GenerateCheckCode(int codeCount)
        {
            string str = string.Empty;
            lock (_lock)
            {
                long num2 = DateTime.Now.Ticks + rep;
                rep++;
                Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
                for (int i = 0; i < codeCount; i++)
                {
                    char ch;
                    int num = random.Next();
                    if ((num % 2) == 0)
                    {
                        ch = (char)(0x30 + ((ushort)(num % 10)));
                    }
                    else
                    {
                        ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                    }
                    str = str + ch.ToString();
                }
            }
            return str;
        }

        public static int getSecondCountSince19700101()
        {
            DateTime oldDate = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime currentDate = DateTime.Now;
            return (int)Math.Round((currentDate - oldDate).TotalSeconds, 0);
        }

        /// <summary>
        /// unix时间戳转换成日期
        /// </summary>
        /// <param name="unixTimeStamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(long timestamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timestamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow); 
        }

        #endregion
    }

    public class WeiXinToken
    {
        private static string token = "";
        private static DateTime tokenExpiresTime = DateTime.Now;

        private static string ticket = "";
        private static DateTime ticketExpiresTime = DateTime.Now;

        public static string GetToken()
        {
            if (token == "" || tokenExpiresTime < DateTime.Now.AddSeconds(5))
            {
                string url = "https://api.weixin.qq.com/cgi-bin/token";
                string pars = string.Format("grant_type=client_credential&appid={0}&secret={1}", WeixinApiHelper.WeiXinAPPID, WeixinApiHelper.WeiXinSecret);

                string json = DownloadHelper.HttpGet(url, pars);

                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                tokenExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                token = t.access_token;
            }
            return token;
        }

        public static string GetTicket()
        {
            if (token == "" || tokenExpiresTime < DateTime.Now.AddSeconds(5))
            {
                string url = "https://api.weixin.qq.com/cgi-bin/token";
                string pars = string.Format("grant_type=client_credential&appid={0}&secret={1}", WeixinApiHelper.WeiXinAPPID, WeixinApiHelper.WeiXinSecret);

                string json = DownloadHelper.HttpGet(url, pars);

                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                tokenExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                token = t.access_token;

                //此时过期或没有
                if (ticket == "" || ticketExpiresTime < DateTime.Now.AddSeconds(5))
                {
                    string url2 = "https://api.weixin.qq.com/cgi-bin/ticket/getticket";
                    string pars2 = string.Format("access_token={0}&type=jsapi", token);

                    string json2 = DownloadHelper.HttpGet(url2, pars2);
                    //token返回值与ticket相同 公用一个类存值
                    TockenClass t2 = JsonConvert.DeserializeObject<TockenClass>(json2);

                    ticketExpiresTime = DateTime.Now.AddSeconds(t2.expires_in);
                    ticket = t2.ticket;
                }
            }
            else if (ticket == "")
            {
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket";
                string pars = string.Format("access_token={0}&type=jsapi", token);

                string json = DownloadHelper.HttpGet(url, pars);
                //token返回值与ticket相同 公用一个类存值
                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                ticketExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                ticket = t.access_token;
            }
            return ticket;
        }
    }

    public class TockenClass
    {
        public string access_token { get; set; }
        public string ticket { get; set; }
        public int expires_in { get; set; }
        public string errcode { get; set; }
        public string errmsg { get; set; }
    }

    public class WeixinAccessToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("openid")]
        public string Openid { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("unionid")]
        public string Unionid { get; set; }
    }

    public class Data
    {

        [JsonProperty("openid")]
        public string[] Openid { get; set; }
    }

    public class WeixinSubscribeUser
    {

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("next_openid")]
        public string NextOpenid { get; set; }
    }

    public class MaterialNewsItem
    {

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("content_source_url")]
        public string ContentSourceUrl { get; set; }

        [JsonProperty("thumb_media_id")]
        public string ThumbMediaId { get; set; }

        [JsonProperty("show_cover_pic")]
        public int ShowCoverPic { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("thumb_url")]
        public string ThumbUrl { get; set; }
    }

    public class MaterialContent
    {

        [JsonProperty("news_item")]
        public MaterialNewsItem[] NewsItem { get; set; }

        [JsonProperty("create_time")]
        public int CreateTime { get; set; }

        [JsonProperty("update_time")]
        public int UpdateTime { get; set; }
    }

    public class MaterialItem
    {

        [JsonProperty("media_id")]
        public string MediaId { get; set; }

        [JsonProperty("content")]
        public MaterialContent Content { get; set; }

        [JsonProperty("update_time")]
        public int UpdateTime { get; set; }
    }

    public class WeixinMaterialListEntity
    {

        [JsonProperty("item")]
        public MaterialItem[] Item { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("item_count")]
        public int ItemCount { get; set; }
    }

    public class WeixinMaterialListParam
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public class WeixinMaterialModel
    {
        public int Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string WeiXinMediaID { get; set; }
        public string Digest { get; set; }
        public string ThumbMediaId { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        public string ContentSourceUrl { get; set; }
        public DateTime SourceUpdateTime { get; set; }
        public Int64 Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public Int64 Editor { get; set; }
        public DateTime UpdateTime { get; set; }
        public int WeixinAcountId { get; set; }
        public int CategoryId { get; set; }
        public int State { get; set; }
    }
}
