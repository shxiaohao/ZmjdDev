using HJD.AccountServices.Entity;
using HJD.WeixinService.Contract;
using HJDAPI.APIProxy;
using HJDAPI.Common.Helpers;
using HJDAPI.Common.Security;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using WHotelSite.Models;

namespace WHotelSite.Common
{
    public class WeiXinHelper 
    {
        #region 订阅号配置

        //订阅号配置
        public static string WeiXinAPPID = "wxb79a37b190594d96";
        public static string WeiXinSecret = "3750a02b64cfb850900053ea7f9469f1";

        public static string WeixinMchId = "1218698601";            //商户ID

        #endregion

        #region 服务号配置

        /// <summary>
        /// 尚旅周末（原 周末酒店服务号）[尚旅]配置
        /// </summary>
        public static string WeixinServiceAppId = "wx52970d8f53c19ba1";
        public static string WeixinServiceSecret = "6de917afedce0c7fd2057c682102ebf6";  //AppSecret(应用密钥)6de917afedce0c7fd2057c682102ebf6
        public static string WeixinServiceMchId = "1306853401";    //商户ID
        public static string WeixinServiceApiSecret = "97cb75d342c4b05b84f3e7265c9d9c7c";   //商户API密钥

        /// <summary>
        /// 周末酒店服务号[浩颐]
        /// </summary>
        public static string HaoYiWeixinServiceAppId = "wx8dc65a9d4caa40ee";
        public static string HaoYiWeixinServiceSecret = "c612bd2173bb928dc147a19e1be01e70";// "40487a5e1a6bc9c53197fea95485f97e";// "9555590c4ee66da08b61718fa1b5af56";// "d2fb41a2e53c489afb0446b2e993bb93";  //AppSecret(应用密钥)6de917afedce0c7fd2057c682102ebf6
        public static string HaoYiWeixinServiceMchId = "1429053402";    //商户ID
        public static string HaoYiWeixinServiceApiSecret = "c612bd2173bb928dc147a19e1be01e70";//"fe62d09172a4323ca1441629696b57e8";// "29696b57e82a4323ca14416fe62d0917";   //商户API密钥

        /// <summary>
        /// 周末酒店苏州服务号[浩颐]
        /// </summary>
        public static string HaoYiSZWeixinServiceAppId = "wx164d0cdfdd00576c";
        public static string HaoYiSZWeixinServiceSecret = "389a5565b5371c54511f9d951a4e720c";

        /// <summary>
        /// 周末酒店成都服务号[浩颐]
        /// </summary>
        public static string HaoYiCDWeixinServiceAppId = "wx138dce55bf5f9baf";
        public static string HaoYiCDWeixinServiceSecret = "c5e566a1b8a6469abd2fa78173ef0a91";

        /// <summary>
        /// 周末酒店深圳服务号[浩颐]
        /// </summary>
        public static string HaoYiShenZWeixinServiceAppId = "wxf8e1a7a628843fa2";
        public static string HaoYiShenZWeixinServiceSecret = "c4e6c7140dcbc435155d40ac13cbc043";

        /// <summary>
        /// 遛娃指南服务号[浩颐]
        /// </summary>
        public static string HaoYiLiuwaWeixinServiceAppId = "wx13a7741d0ed14bec";
        public static string HaoYiLiuwaWeixinServiceSecret = "2b89af3b19a611257e21d68020d45834";

        /// <summary>
        /// 遛娃指南南京服务号[浩颐]
        /// </summary>
        public static string HaoYiLiuwaNJWeixinServiceAppId = "wxc635831255e9164a";
        public static string HaoYiLiuwaNJWeixinServiceSecret = "9a407f7c7260eb17290c46857a74377a";

        /// <summary>
        /// 遛娃指南无锡服务号[浩颐]
        /// </summary>
        public static string HaoYiLiuwaWXWeixinServiceAppId = "wxfb9b38ab41c2e6c3";
        public static string HaoYiLiuwaWXWeixinServiceSecret = "53e0135e14294baf4077b371ab9d8a22";

        /// <summary>
        /// 遛娃指南杭州服务号[浩颐]
        /// </summary>
        public static string HaoYiLiuwaHZHWeixinServiceAppId = "wxab7120b5890e1e78";
        public static string HaoYiLiuwaHZHWeixinServiceSecret = "d39c1366c071d3a7017bf0755637e176";

        /// <summary>
        /// 遛娃指南广州服务号[浩颐]
        /// </summary>
        public static string HaoYiLiuwaGZWeixinServiceAppId = "wx14e5b6df72fbae1d";
        public static string HaoYiLiuwaGZWeixinServiceSecret = "807508fb840a0e681e62b245f2490bbc";

        public static string Snsapi_base = "snsapi_base";
        public static string Snsapi_userinfo = "snsapi_userinfo";

        #endregion

        /// <summary>
        /// 【周末酒店酒店服务号 尚旅】通过code换取网页授权access_token
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <param name="code"></param>
        public static WeixinAccessToken GetWeixinAccessToken(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, WeixinServiceAppId, WeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);

            return obj;
        }

        /// <summary>
        /// 【周末酒店酒店服务号 浩颐】通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static WeixinAccessToken GetWeixinAccessTokenForHaoYi(string code)
        {
           string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, HaoYiWeixinServiceAppId, HaoYiWeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);
          //  LogHelper.WriteLog("GetWeixinAccessTokenForHaoYi:" + url + " " + json );
          
            return obj;
        }

        /// <summary>
        /// 【周末酒店酒店苏州服务号】通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static WeixinAccessToken GetWeixinAccessTokenForHaoYiSZ(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, HaoYiSZWeixinServiceAppId, HaoYiSZWeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);
          //  LogHelper.WriteLog("GetWeixinAccessTokenForHaoYiSZ:" + url + " " + json);

            return obj;
        }

        /// <summary>
        /// 【周末酒店酒店成都服务号】通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static WeixinAccessToken GetWeixinAccessTokenForHaoYiCD(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, HaoYiCDWeixinServiceAppId, HaoYiCDWeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);
           // LogHelper.WriteLog("GetWeixinAccessTokenForHaoYiCD:" + url + " " + json);

            return obj;
        }

        /// <summary>
        /// 【周末酒店酒店深圳服务号】通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static WeixinAccessToken GetWeixinAccessTokenForHaoYiShenZ(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, HaoYiShenZWeixinServiceAppId, HaoYiShenZWeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);
          //  LogHelper.WriteLog("GetWeixinAccessTokenForHaoYiShenZ:" + url + " " + json);

            return obj;
        }

        /// <summary>
        /// 【遛娃指南服务号】通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static WeixinAccessToken GetWeixinAccessTokenForHaoYiLiuwa(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, HaoYiLiuwaWeixinServiceAppId, HaoYiLiuwaWeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);
        //    LogHelper.WriteLog("GetWeixinAccessTokenForHaoYiLiuwa:" + url + " " + json);

            return obj;
        }

        /// <summary>
        /// 【遛娃指南南京服务号】通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static WeixinAccessToken GetWeixinAccessTokenForHaoYiLiuwaNJ(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, HaoYiLiuwaNJWeixinServiceAppId, HaoYiLiuwaNJWeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);
       //     LogHelper.WriteLog("GetWeixinAccessTokenForHaoYiLiuwaNJ:" + url + " " + json);

            return obj;
        }

        /// <summary>
        /// 【遛娃指南无锡服务号】通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static WeixinAccessToken GetWeixinAccessTokenForHaoYiLiuwaWX(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, HaoYiLiuwaWXWeixinServiceAppId, HaoYiLiuwaWXWeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);
            //     LogHelper.WriteLog("GetWeixinAccessTokenForHaoYiLiuwaWX:" + url + " " + json);

            return obj;
        }

        /// <summary>
        /// 【遛娃指南广州服务号】通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static WeixinAccessToken GetWeixinAccessTokenForHaoYiLiuwaGZH(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, HaoYiLiuwaGZWeixinServiceAppId, HaoYiLiuwaGZWeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);
       //     LogHelper.WriteLog("GetWeixinAccessTokenForHaoYiLiuwaGZ:" + url + " " + json);

            return obj;
        }

        /// <summary>
        /// 【遛娃指南杭州服务号】通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static WeixinAccessToken GetWeixinAccessTokenForHaoYiLiuwaHZH(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, HaoYiLiuwaHZHWeixinServiceAppId, HaoYiLiuwaHZHWeixinServiceSecret, code);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinAccessToken>(json);
            //     LogHelper.WriteLog("GetWeixinAccessTokenForHaoYiLiuwaHZH:" + url + " " + json);

            return obj;
        }

        /// <summary>
        /// 通过access_token拉取用户信息(需scope为 snsapi_userinfo)
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static WeixinUserInfo GetWeixinUserInfo(string accessToken, string openid)
        {
            string url = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN";
            url = string.Format(url, accessToken, openid);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinUserInfo>(json);

            return obj;
        }

        /// <summary>
        /// 通过access_token拉取用户信息（包含是否关注当前订阅号的信息）(需scope为 snsapi_userinfo)
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static WeixinUserInfo GetWeixinUserSubscribeInfo(string openid, ref string backJson)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN";
            url = string.Format(url, WeiXinToken.GetToken(), openid);
            CookieContainer cc = new CookieContainer();
            backJson = HttpRequestHelper.Get(url, "", ref cc);
            var obj = JsonConvert.DeserializeObject<WeixinUserInfo>(backJson);

            return obj;
        }

        /// <summary>
        /// 【周末酒店酒店服务号 尚旅】传入回调url，生成返回微信静默授权url
        /// </summary>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        public static string GenSilenceAuthorUrl(string redirect_uri)
        {
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={2}&response_type=code&scope={1}&state=STATE#wechat_redirect", WeixinServiceAppId, Snsapi_base, redirect_uri);
        }

        /// <summary>
        /// 【周末酒店酒店服务号 浩颐】传入回调url，生成返回微信静默授权url
        /// </summary>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        public static string GenSilenceAuthorUrlForHaoYi(string redirect_uri)
        {
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={2}&response_type=code&scope={1}&state=STATE#wechat_redirect", HaoYiWeixinServiceAppId, Snsapi_base, redirect_uri);
        }

        /// <summary>
        /// 获取指定服务号的确认授权URL
        /// </summary>
        /// <param name="acountCode"></param>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        public static string GenConfrimAuthorUrlByWeixinAcount(WeiXinChannelCode acountCode, string redirect_uri)
        {
            var appid = "";
            switch (acountCode)
            {
                case WeiXinChannelCode.周末酒店服务号:
                    appid = WeixinServiceAppId;
                    break;
                case WeiXinChannelCode.周末酒店服务号_皓颐:
                    appid = HaoYiWeixinServiceAppId;
                    break;
                case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                    appid = HaoYiSZWeixinServiceAppId;
                    break;
                case WeiXinChannelCode.周末酒店成都服务号_皓颐:
                    appid = HaoYiCDWeixinServiceAppId;
                    break;
                case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                    appid = HaoYiShenZWeixinServiceAppId;
                    break;
                case WeiXinChannelCode.遛娃指南服务号_皓颐:
                    appid = HaoYiLiuwaWeixinServiceAppId;
                    break;
                case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                    appid = HaoYiLiuwaNJWeixinServiceAppId;
                    break;
                case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                    appid = HaoYiLiuwaWXWeixinServiceAppId;
                    break;
                case WeiXinChannelCode.遛娃指南广州服务号_皓颐:
                    appid = HaoYiLiuwaGZWeixinServiceAppId;
                    break;
                case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                    appid = HaoYiLiuwaHZHWeixinServiceAppId;
                    break;
            }

            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={2}&response_type=code&scope={1}&state=STATE#wechat_redirect", appid, Snsapi_userinfo, redirect_uri);
        }

        /// <summary>
        /// 【周末酒店酒店服务号 尚旅】传入回调url，生成返回微信确认授权url
        /// </summary>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        public static string GenConfrimAuthorUrl(string redirect_uri)
        {
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={2}&response_type=code&scope={1}&state=STATE#wechat_redirect", WeixinServiceAppId, Snsapi_userinfo, redirect_uri);
        }

        /// <summary>
        /// 【周末酒店酒店服务号 浩颐】传入回调url，生成返回微信确认授权url
        /// </summary>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        public static string GenConfrimAuthorUrlForHaoYi(string redirect_uri)
        {
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={2}&response_type=code&scope={1}&state=STATE#wechat_redirect", HaoYiWeixinServiceAppId, Snsapi_userinfo, redirect_uri);
        }

        /// <summary>
        /// 【周末酒店酒店苏州服务号】传入回调url，生成返回微信确认授权url
        /// </summary>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        public static string GenConfrimAuthorUrlForHaoYiSZ(string redirect_uri)
        {
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={2}&response_type=code&scope={1}&state=STATE#wechat_redirect", HaoYiSZWeixinServiceAppId, Snsapi_userinfo, redirect_uri);
        }

        #region 绑定微信账号与zmjd账号

        /// <summary>
        /// 绑定指定uid和微信unionid
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="unionid"></param>
        public static void BindingWxAndUid(long uid, string unionid)
        {
            if (uid > 0 && !string.IsNullOrEmpty(unionid))
            {
                var _userChannelRel = new UserChannelRelEntity
                {
                    Channel = "weixin",
                    Code = unionid,
                    UserId = uid,
                    Tag = "",
                    CreateTime = DateTime.Now
                };

                //先查询
                var _rel = account.GetOneUserChannelRel(_userChannelRel);
                if (_rel != null && _rel.UserId > 0)
                {

                }
                else
                {
                    var _add = account.AddUserChannelRel(_userChannelRel);

                    #region 检查CID关联(只对新关联的用户做处理即可，已经绑定过微信id的用户，基本在注册的时候已经处理该逻辑 2018.06.26 haoy)

                    //如果用户先在微信成为A的粉丝，然后第一次在APP注册（拿不到unionid），然后又在微信里购买，那么要在这个时候做CID绑定 2018.06.25
                    try
                    {
                        //有粉丝关联
                        UserFansRel fansInfo = account.GetOneFansRelByUnionid(unionid);
                        if (fansInfo != null && fansInfo.UserID > 0)
                        {
                            //检查当前用户是否有CID关联   
                            var _addCidRel = new account().AddUserChannelRelHistory(new UserChannelRelHistoryEntity
                            {
                                ChangeType = (int)AccountEnums.ChangeType.Fans,
                                CID = fansInfo.UserID,
                                CreateTime = DateTime.Now,
                                UserID = uid,
                                RelBizInfo = ""
                            });
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    #endregion
                }
            }
        }

        #endregion

        #region 支付相关

        /// <summary>
        /// 微信统一下单
        /// </summary>
        /// <param name="body"></param>
        /// <param name="attach"></param>
        /// <param name="out_trade_no"></param>
        /// <param name="total_fee"></param>
        /// <param name="spbill_create_ip"></param>
        /// <param name="notify_url"></param>
        /// <param name="trade_type"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static WeixinPrePayResult GetWeixinUnifiedOrder(string body, string attach, string out_trade_no, int total_fee, string spbill_create_ip, string notify_url, string trade_type, string openid)
        {
            string APPID = WeixinServiceAppId;
            string WeiXinSecret = WeixinServiceSecret;
            string mch_id = WeixinServiceMchId;   // "1218698601";
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

            string nonce_str = GenerateRandomStr();
            string sign = Signature.WeixinPayUnifiedorderSignature(APPID, WeixinServiceApiSecret, mch_id, nonce_str, body, attach, out_trade_no, total_fee, spbill_create_ip, notify_url, trade_type, openid);
            
            //StringBuilder strBuilder = new StringBuilder();
            //strBuilder.Append("<xml>");
            //strBuilder.Append("<appid>" + APPID + "</appid>");
            //strBuilder.Append("<mch_id>" + "1218698601"  + "</mch_id>");
            //strBuilder.Append("<nonce_str>" + noncestr + "</nonce_str>");
            //strBuilder.Append("<sign>" + sign + "</sign>");
            //strBuilder.Append("<body>" + body + "</body>");
            //strBuilder.Append("<out_trade_no>" + out_trade_no + "</out_trade_no>");
            //strBuilder.Append("<total_fee>" + total_fee.ToString() + "</total_fee>");
            //strBuilder.Append("<spbill_create_ip>" + spbill_create_ip + "</spbill_create_ip>");
            //strBuilder.Append("<notify_url>" + notify_url + "</notify_url>");
            //strBuilder.Append("<trade_type>" + trade_type + "</trade_type>");
            //strBuilder.Append("<openid>" + openid + "</openid>");
            //strBuilder.Append("</xml>");
            //string ss = strBuilder.ToString();

            XmlDocument responseXML = new XmlDocument();
            var root = responseXML.AppendChild(responseXML.CreateElement("xml"));

            //公众号
            var node = responseXML.CreateElement("appid");
            node.InnerText = APPID;
            root.AppendChild(node);

            //商户号
            node = responseXML.CreateElement("mch_id");
            node.InnerText = mch_id;
            root.AppendChild(node);

            //随机字符串
            node = responseXML.CreateElement("nonce_str");
            node.InnerText = nonce_str;
            root.AppendChild(node);

            //商品描述
            node = responseXML.CreateElement("body");
            node.InnerText = body;
            root.AppendChild(node);

            //签名
            node = responseXML.CreateElement("sign");
            node.InnerText = sign;
            root.AppendChild(node);

            //商户订单号
            node = responseXML.CreateElement("out_trade_no");
            node.InnerText = out_trade_no;
            root.AppendChild(node);

            //总金额
            node = responseXML.CreateElement("total_fee");
            node.InnerText = total_fee.ToString();
            root.AppendChild(node);

            //终端IP
            node = responseXML.CreateElement("spbill_create_ip");
            node.InnerText = spbill_create_ip;
            root.AppendChild(node);

            //通知地址
            node = responseXML.CreateElement("notify_url");
            node.InnerText = notify_url;
            root.AppendChild(node);

            //交易类型 
            node = responseXML.CreateElement("trade_type");
            node.InnerText = trade_type;
            root.AppendChild(node);

            //用户标识 jssdk必传
            node = responseXML.CreateElement("openid");
            node.InnerText = openid;
            root.AppendChild(node);

            string ss = responseXML.OuterXml;

            string result = HttpRequestHelper.PostXml(url, ss);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(result);

            XmlElement rootElement = xmlDoc.DocumentElement;
            XmlNode flag1 = rootElement.SelectSingleNode("return_code");

            string return_code = GetXMLNodeValue(flag1);
            if (!return_code.Equals("SUCCESS"))
            {
                XmlNode flag = rootElement.SelectSingleNode("return_msg");
                string return_msg = GetXMLNodeValue(flag);

                return new WeixinPrePayResult() { return_code = return_code, return_msg = return_msg };
            }

            XmlNode flag2 = rootElement.SelectSingleNode("result_code");
            string result_code = GetXMLNodeValue(flag2);
            if (!result_code.Equals("SUCCESS"))
            {
                XmlNode err_codeNode = rootElement.SelectSingleNode("err_code");
                XmlNode err_code_desNode = rootElement.SelectSingleNode("err_code_des");
                string err_code = GetXMLNodeValue(err_codeNode);
                string err_code_des = GetXMLNodeValue(err_code_desNode);

                return new WeixinPrePayResult() { result_code = result_code, err_code = err_code, err_code_des = err_code_des };
            }

            if (return_code.Equals("SUCCESS") && result_code.Equals("SUCCESS"))
            {
                XmlNode prepay_idNode = rootElement.SelectSingleNode("prepay_id");
                string prepay_id = GetXMLNodeValue(prepay_idNode);
                return new WeixinPrePayResult() { prepay_id = prepay_id };
            }

            return new WeixinPrePayResult();
        }

        public static string GetXMLNodeValue(XmlNode rootElement, string nodeName)
        {
            try
            {
                XmlNode flag2 = rootElement.SelectSingleNode(nodeName);

                return GetXMLNodeValue(flag2);
            }
            catch
            {
                return "";
            }
        }

        public static string GetXMLNodeValue(XmlNode node)
        {
            if (node != null)
            {
                try
                {
                    XmlCDataSection cdata = (XmlCDataSection)node.FirstChild;
                    if (cdata != null)
                    {
                        return cdata.InnerText;
                    }
                    else
                    {
                        return node.InnerText;
                    }
                }
                catch
                {
                    return node.InnerText;
                }
            }
            else
            {
                return "";
            }
        }

        #endregion

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
                string pars = string.Format("grant_type=client_credential&appid={0}&secret={1}", WeiXinHelper.WeiXinAPPID, WeiXinHelper.WeiXinSecret);

                string json = HttpHelper.HttpGet(url, pars);

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
                string pars = string.Format("grant_type=client_credential&appid={0}&secret={1}", WeiXinHelper.WeiXinAPPID, WeiXinHelper.WeiXinSecret);

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
}