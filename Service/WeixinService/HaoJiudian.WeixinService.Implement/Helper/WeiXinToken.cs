using HJD.WeixinService.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJD.WeixinService.Implement.Helper
{
    /// <summary>
    /// 周末酒店 订阅号
    /// </summary>
    public class WeiXinToken
    {
        public static string GetWeixinTokenByCode(WeiXinChannelCode code)
        {
            switch (code)
            {
                case WeiXinChannelCode.周末酒店订阅号:
                    return WeiXinToken.GetToken();
                case WeiXinChannelCode.周末酒店服务号:
                    return WeiXinServiceToken.GetToken();
                case WeiXinChannelCode.周末酒店服务号_皓颐:
                    return HaoYiServiceWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南服务号_皓颐:
                    return HaoYiServiceLiuwaWeiXinToken.GetTicket();
                case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                    return HaoYiServiceSZWeiXinToken.GetTicket();
                case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                    return HaoYiServiceShenZWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                    return HaoYiServiceLiuwaNJWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                    return HaoYiServiceLiuwaWXWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                    return HaoYiServiceLiuwaHZHWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南广州服务号_皓颐:
                    return HaoYiServiceLiuwaGZWeiXinToken.GetTicket();
                case WeiXinChannelCode.尚旅游订阅号:
                    return ShangLvYouWeiXinToken.GetToken();
            }

            return "";
        }

        private static string token = "";
        private static DateTime tokenExpiresTime = DateTime.Now;

        private static string ticket = "";
        private static DateTime ticketExpiresTime = DateTime.Now;

        public static string GetToken()
        {
            if (token == "" || tokenExpiresTime < DateTime.Now.AddSeconds(5))
            {
                GenToken();
            }
            return token;
        }

        public static string GenToken()
        {
            if (token == "" || tokenExpiresTime < DateTime.Now.AddSeconds(5))
            {
                string APPID = Configs.WeiXinAPPID;
                string WeiXinSecret = Configs.WeiXinSecret;
                string url = "https://api.weixin.qq.com/cgi-bin/token";
                string pars = string.Format("grant_type=client_credential&appid={0}&secret={1}", APPID, WeiXinSecret);

                string json = HttpHelper.HttpGet(url, pars);

                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                tokenExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                token = t.access_token;
            }

            return token;
        }

        public  static string GetTicket()
        {
            if (token == "" || tokenExpiresTime < DateTime.Now.AddSeconds(5))
            {
                string APPID = Configs.WeiXinAPPID;
                string WeiXinSecret = Configs.WeiXinSecret;
                string url = "https://api.weixin.qq.com/cgi-bin/token";
                string pars = string.Format("grant_type=client_credential&appid={0}&secret={1}", APPID, WeiXinSecret);

                string json = HttpHelper.HttpGet(url, pars);

                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                tokenExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                token = t.access_token;

                //此时过期或没有
                if (ticket == "" || ticketExpiresTime < DateTime.Now.AddSeconds(5))
                {
                    string url2 = "https://api.weixin.qq.com/cgi-bin/ticket/getticket";
                    string pars2 = string.Format("access_token={0}&type=jsapi", token);

                    string json2 = HttpHelper.HttpGet(url2, pars2);
                    //token返回值与ticket相同 公用一个类存值
                    TockenClass t2 = JsonConvert.DeserializeObject<TockenClass>(json2);

                    if (t2.errcode != "0")
                    {
                        return GetTicket();
                    }
                    else
                    {
                        ticketExpiresTime = DateTime.Now.AddSeconds(t2.expires_in);
                        ticket = t2.ticket;   
                    }
                }
            }
            else if (ticket == "")
            {
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket";
                string pars = string.Format("access_token={0}&type=jsapi", token);

                string json = HttpHelper.HttpGet(url, pars);
                //token返回值与ticket相同 公用一个类存值
                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                if (t.errcode != "0")
                {
                    return GetTicket();
                }
                else
                {
                    ticketExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                    ticket = t.ticket;
                }
            }
            return ticket;
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetToken();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 尚旅周末 服务号（尚旅主体）
    /// </summary>
    public class WeiXinServiceToken 
    {
        private static string APPID = "wx52970d8f53c19ba1";
        private static string WeiXinSecret = "6de917afedce0c7fd2057c682102ebf6";

        private static string token = "";
        private static DateTime tokenExpiresTime = DateTime.Now;

        private static string ticket = "";
        private static DateTime ticketExpiresTime = DateTime.Now;

        public static string GetToken()
        {
            if (token == "" || tokenExpiresTime < DateTime.Now.AddSeconds(5))
            {
                GenToken();
            }
            return token;
        }

        public static string GenToken()
        {
              string url = "https://api.weixin.qq.com/cgi-bin/token";
                string pars = string.Format("grant_type=client_credential&appid={0}&secret={1}", APPID, WeiXinSecret);

                string json = HttpHelper.HttpGet(url, pars);

                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                tokenExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                token = t.access_token;

            return token;
        }

        public static string GetTicket()
        {
            if (token == "" || tokenExpiresTime < DateTime.Now.AddSeconds(5))
            {
                string url = "https://api.weixin.qq.com/cgi-bin/token";
                string pars = string.Format("grant_type=client_credential&appid={0}&secret={1}", APPID, WeiXinSecret);

                string json = HttpHelper.HttpGet(url, pars);

                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                tokenExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                token = t.access_token;

                //此时过期或没有
                if (ticket == "" || ticketExpiresTime < DateTime.Now.AddSeconds(5))
                {
                    string url2 = "https://api.weixin.qq.com/cgi-bin/ticket/getticket";
                    string pars2 = string.Format("access_token={0}&type=jsapi", token);

                    string json2 = HttpHelper.HttpGet(url2, pars2);
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

                string json = HttpHelper.HttpGet(url, pars);
                //token返回值与ticket相同 公用一个类存值
                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                ticketExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                ticket = t.ticket;
            }
            return ticket;
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetToken();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 尚旅游 订阅号
    /// </summary>
    public class ShangLvYouWeiXinToken
    { 
        private static WeiXinTokenBase mBaseToken;
        private static string appID = "wxf03737350960d092";
        private static string weiXinSecret = "75d1dd82cc59c1c3ac98f6ee90baf67f";

        public static WeiXinTokenBase baseToken
        {
            get
            {
                if (mBaseToken == null)
                {
                    mBaseToken = new WeiXinTokenBase(appID, weiXinSecret);
                }
                return mBaseToken;
            }
        }

        public static string GetToken()
        { 
            return baseToken.GetToken();
        }

        public static string GenTicket()
        { 
            return baseToken.GenToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetToken();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 成都遛娃指南（原 尚旅游成都） 订阅号
    /// </summary>
    public class ShangLvYouChengduWeiXinToken
    {
        private static WeiXinTokenBase mBaseToken;
        private static string appID = "wx0d10c4a2a35069db";
        private static string weiXinSecret = "f1fc5dba4500dd8ebf2957512d2890a7";

        public static WeiXinTokenBase baseToken
        {
            get
            {
                if (mBaseToken == null)
                {
                    mBaseToken = new WeiXinTokenBase(appID, weiXinSecret);
                }
                return mBaseToken;
            }
        }

        public static string GetToken()
        {
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            return baseToken.GenToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetToken();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 北京遛娃指南（原 尚旅游北京） 订阅号
    /// </summary>
    public class ShangLvYouBeijingWeiXinToken
    {
        private static WeiXinTokenBase mBaseToken;
        private static string appID = "wxf3fed7645066cf16";
        private static string weiXinSecret = "1ce87625bd3c3c9c4a4a7139ec2a2c75";

        public static WeiXinTokenBase baseToken
        {
            get
            {
                if (mBaseToken == null)
                {
                    mBaseToken = new WeiXinTokenBase(appID, weiXinSecret);
                }
                return mBaseToken;
            }
        }

        public static string GetToken()
        {
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            return baseToken.GenToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetToken();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 周末酒店服务号 皓颐
    /// </summary>
    public class HaoYiServiceWeiXinToken
    { 
        private static WeiXinTokenBase baseToken;
        private static string appID = "wx8dc65a9d4caa40ee";
        private static string weiXinSecret = "c612bd2173bb928dc147a19e1be01e70";// "fe62d09172a4323ca1441629696b57e8";//"75d1dd82cc59c1c3ac98f6ee90baf67f";

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 遛娃指南苏州服务号 浩颐
    /// </summary>
    public class HaoYiServiceSZWeiXinToken
    {
        private static WeiXinTokenBase baseToken;
        private static string appID = "wx164d0cdfdd00576c";
        private static string weiXinSecret = "389a5565b5371c54511f9d951a4e720c";

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 遛娃指南成都服务号 浩颐
    /// </summary>
    public class HaoYiServiceCDWeiXinToken
    {
        private static WeiXinTokenBase baseToken;
        private static string appID = "wx138dce55bf5f9baf";
        private static string weiXinSecret = "c5e566a1b8a6469abd2fa78173ef0a91";

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 遛娃指南深圳服务号 浩颐
    /// </summary>
    public class HaoYiServiceShenZWeiXinToken
    {
        private static WeiXinTokenBase baseToken;
        private static string appID = "wxf8e1a7a628843fa2";
        private static string weiXinSecret = "c4e6c7140dcbc435155d40ac13cbc043";

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 遛娃指南服务号 浩颐
    /// </summary>
    public class HaoYiServiceLiuwaWeiXinToken
    {
        private static WeiXinTokenBase baseToken;
        private static string appID = "wx13a7741d0ed14bec";
        private static string weiXinSecret = "2b89af3b19a611257e21d68020d45834";

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GetToken()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 遛娃指南南京服务号 浩颐
    /// </summary>
    public class HaoYiServiceLiuwaNJWeiXinToken
    {
        private static WeiXinTokenBase baseToken;
        private static string appID = "wxc635831255e9164a";
        private static string weiXinSecret = "9a407f7c7260eb17290c46857a74377a";

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 遛娃指南无锡服务号 浩颐
    /// </summary>
    public class HaoYiServiceLiuwaWXWeiXinToken
    {
        private static WeiXinTokenBase baseToken;
        private static string appID = "wxfb9b38ab41c2e6c3";
        private static string weiXinSecret = "53e0135e14294baf4077b371ab9d8a22";

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            try
            {
                LogHelper.WriteLog(string.Format("UpdateWeixinMenu:{0}", json));
            }
            catch (Exception ex)
            {

            }

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 遛娃指南广州服务号 浩颐
    /// </summary>
    public class HaoYiServiceLiuwaGZWeiXinToken
    {
        private static WeiXinTokenBase baseToken;
        private static string appID = "wx14e5b6df72fbae1d";
        private static string weiXinSecret = "807508fb840a0e681e62b245f2490bbc";

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 遛娃指南杭州服务号 浩颐
    /// </summary>
    public class HaoYiServiceLiuwaHZHWeiXinToken
    {
        private static WeiXinTokenBase baseToken;
        private static string appID = "wxab7120b5890e1e78";
        private static string weiXinSecret = "d39c1366c071d3a7017bf0755637e176";

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 遛娃指南深圳订阅号 浩颐
    /// </summary>
    public class HaoYiLiuwaSHZHWeiXinToken
    {
        private static WeiXinTokenBase baseToken;
        private static string appID = "wx496ce681185a0b04";
        private static string weiXinSecret = "7c6a861e2fe37ecfedf8e0b45ef7940f";

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
        }
    }

    /// <summary>
    /// 遛娃指南 订阅号
    /// </summary>
    public class HaoYiLiuwaWeiXinToken
    {
        private static WeiXinTokenBase baseToken;
        private static string appID = "wxfba29a99236c4fdc";
        private static string weiXinSecret = "cf4e5c2ef8906e3e76461eee942f7fb2";
        //EncodingAESKey: 5hFJ1J6zzPtjiCwVBKqpd2uE5DXSvFLo6uyMl3GyVGs

        public static string GetTicket()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GetToken()
        {
            if (baseToken == null)
            {
                baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            }
            return baseToken.GetToken();
        }

        public static string GenTicket()
        {
            baseToken = new WeiXinTokenBase(appID, weiXinSecret);
            return baseToken.GetToken();
        }

        public static string UpdateMenu(string menuInfo)
        {
            //  string menuInfo = System.IO.File.ReadAllText(@"D:\Template\weixin\menu.txt");

            string token = GetTicket();

            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;

            CookieContainer cc = new CookieContainer();

            string json = HttpHelper.PostJson2(url, menuInfo, ref cc);

            return json + menuInfo;
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
}
