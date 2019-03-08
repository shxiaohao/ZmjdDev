using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using HJD.DestServices.Contract;
using HJDAPI.APIProxy;
using HJDAPI.Models;
using HJD.HotelServices.Contracts;
using WHotelSite.Models;

using System.Web.Mvc;
using System.Text;
using System.Text.RegularExpressions;

using FiftyOne.Foundation.Mobile.Detection;

namespace WHotelSite
{
    public class Utils
    {
        public static DistrictInfoEntity GetDistrictInfo(int id)
        {
            foreach (DistrictInfoEntity t in Dest.GetDistrictInfo(new List<int> { id }))
            {
                if (t.DistrictID == id)
                {
                    return t;
                }
            }
            throw new Exception("无效的城市ID");
        }

        public static string BuildOTALink(string accessUrl, DateTime checkIn, DateTime checkOut)
        {
            accessUrl = accessUrl.Replace("{bYear}", checkIn.ToString("yyyy"));
            accessUrl = accessUrl.Replace("{bMonth}", checkIn.ToString("MM"));
            accessUrl = accessUrl.Replace("{bDay}", checkIn.ToString("dd"));
            accessUrl = accessUrl.Replace("{eYear}", checkOut.ToString("yyyy"));
            accessUrl = accessUrl.Replace("{eMonth}", checkOut.ToString("MM"));
            accessUrl = accessUrl.Replace("{eDay}", checkOut.ToString("dd"));
            accessUrl = accessUrl.Replace("{day}", (checkOut - checkIn).TotalDays.ToString("0"));
            return accessUrl;
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static DateTime ParseDate(string dateStr, DateTime fallback)
        {
            DateTime ret;
            if (DateTime.TryParse(dateStr, out ret))
            {
                return ret;
            }
            return fallback;
        }

        public static int GetChannelId(string userAgent,object SessionChannelID)
        {
            //如果是渠道带过来的，那么提交订单时记录渠道ID
            int ChannelID = -1;
            if (SessionChannelID != null)
            {
                int.TryParse(SessionChannelID.ToString(), out ChannelID);

                if (ChannelID == 0)
                {
                    try
                    {
                        string originStr = HJDAPI.Common.Security.DES.Decrypt(SessionChannelID.ToString());
                        int.TryParse(!string.IsNullOrWhiteSpace(originStr) ? originStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0] : "0", out ChannelID);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            
            if (ChannelID == -1)
            {
                ChannelID = userAgent.IndexOf(" MicroMessenger/") > 0 ? 1 : 0;
            }

            return ChannelID;
        }

        public static List<DistrictSuggestModel> GetDistrictSuggest()
        {
            var data = Hotel.GetZMJDCityData2();
            if (data == null)
            {
                return new List<DistrictSuggestModel>();
            }
            var hotList = Mapper.ConvertCityEntityListToSimpleDistrictModelList(data.HotArea);
            var districts = Mapper.ConvertCityEntityListToSimpleDistrictModelList(data.Citys);
            var gs = districts.GroupBy(p => p.FirstLetter).OrderBy(p => p.Key).ToList();
            var model = new List<DistrictSuggestModel>
            {
                new DistrictSuggestModel{ Group = "热门", List = hotList},
                new DistrictSuggestModel{ Group = "ABCDE",List = gs.Where(p=>"ABCD".IndexOf(p.Key,StringComparison.OrdinalIgnoreCase)>=0).SelectMany(p=>p).ToList()},
                new DistrictSuggestModel{ Group = "FGHIJ",List = gs.Where(p=>"FGHIJ".IndexOf(p.Key,StringComparison.OrdinalIgnoreCase)>=0).SelectMany(p=>p).ToList()},
                new DistrictSuggestModel{ Group = "KLMNO",List = gs.Where(p=>"KLMNO".IndexOf(p.Key,StringComparison.OrdinalIgnoreCase)>=0).SelectMany(p=>p).ToList()},
                new DistrictSuggestModel{ Group = "PQRST",List = gs.Where(p=>"PQRST".IndexOf(p.Key,StringComparison.OrdinalIgnoreCase)>=0).SelectMany(p=>p).ToList()},
                new DistrictSuggestModel{ Group = "UVWXYZ",List = gs.Where(p=>"UVWXYZ".IndexOf(p.Key,StringComparison.OrdinalIgnoreCase)>=0).SelectMany(p=>p).ToList()},
            };

            return model;
        }

        public static string JSONEncode(object obj)
        {
            return System.Web.Helpers.Json.Encode(obj);
        }

        public static string GetAbsoluteUrl(UrlHelper Url, string relativeUrl)
        {
             return GetAbsoluteUrl(Url.RequestContext.HttpContext.Request, relativeUrl);
        }

        public static string GetAbsoluteUrl(HttpRequestBase request, string relativeUrl)
        { 
            return string.Format("{0}://{1}{2}",
                (request.IsSecureConnection) ? "https" : "http",
                request.Headers["Host"],
                VirtualPathUtility.ToAbsolute(relativeUrl));
        }

        public static string BuildQueryString(IDictionary<string, string> dict, string charset)
        {
            StringBuilder sb = new StringBuilder();
            Encoding encoding = Encoding.GetEncoding(charset);
            foreach (KeyValuePair<string, string> p in dict) {
                if (sb.Length > 0) {
                    sb.Append("&");
                }
                sb.AppendFormat("{0}={1}", p.Key, HttpUtility.UrlEncode(p.Value, encoding));
            }
            return sb.ToString();
        }

        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str, Encoding.GetEncoding("utf-8"));
        }

        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str, Encoding.GetEncoding("utf-8"));
        }

        public static bool IsMobile()
        {
            bool? ret = null;
            if (ret == null) {
                HttpRequest request = HttpContext.Current.Request;
                ret = request.Browser["IsMobile"] == "True";
                if (!ret.Value && !string.IsNullOrEmpty(request.UserAgent))
                {
                    ret = "Android,iPhone,SymbianOS,Windows Phone,iPad,iPod".Split(',').ToList().Exists(_ => request.UserAgent.Contains(_));
                }
            }
            return ret.Value;
        }

        public static string GetFitPicUrl(string picUrl)
        {
            return IsMobile() ? picUrl : picUrl.TrimEnd('s');
        }

        public static string ClientType()
        {
            return IsMobile() ? "wap" : "www";
        }

        public static string ChineseNumber(int n)
        {
            return (n >=0 && n < 10) ? ("零一二三四五六七八九").Substring(n, 1) : n.ToString("0");
        }

        public static Dictionary<string, object> MakeCalendarOptions(List<HJD.HotelServices.Contracts.PDayItem> dayItems, int min, int max)
        {
            if (dayItems == null) {
                return null;
            }
            DateTime? start = null;
            DateTime? end = null;
            List<String> grayDays = new List<String>();
            List<String> priceList = new List<String>();
            List<String> vipPriceList = new List<String>();
            List<String> idList = new List<String>();
            foreach (PDayItem item in dayItems) {
                if (start == null) {
                    start = item.Day;
                }
                end = item.Day;
                if (item.SellState != 1) {
                    grayDays.Add(Utils.FormatDate(item.Day));
                }

                priceList.Add(item.SellPrice.ToString());
                vipPriceList.Add(item.VipPrice.ToString());

                idList.Add(item.ID.ToString());
            }
            if (start == null) {
                return null;
            }
            return new Dictionary<string, object> {
                {"start", Utils.FormatDate(start.Value)},
                {"end", Utils.FormatDate(end.Value)},
                {"grayDays", grayDays},
                {"priceList", priceList},
                {"vipPriceList", vipPriceList},
                {"idList", idList},
                {"limit",new Dictionary<string, object>{{"min",min},{"max",max}}},
                {"limitMin", min},
                {"limitMax", max},
            };
        }

        /// <summary>
        /// TODO: tell web from wap
        /// TODO: definition is not perfect
        /// 0 微信
        /// 1 web
        /// 2 ios app
        /// 3 android app
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="_contextBasicInfo"></param>
        /// <returns></returns>
        public static string GetTerminalName(string userAgent, ContextBasicInfo _contextBasicInfo)
        {
            userAgent = userAgent.ToLower();
            bool isApp = userAgent.IndexOf(" weekendhotel/") > -1;
            bool isAndroid = userAgent.IndexOf("android") > -1;
            if (isApp)
            {
                return isAndroid ? "Android" : "iOS";
            }
            else if (_contextBasicInfo.IsApp)
            {
                return _contextBasicInfo.IsAndroid ? "Android" : "iOS";
            }

            bool isWeixin = userAgent.IndexOf("micromessenger") > -1;
            if (isWeixin)
            {
                return "Weixin";
            }
            bool isWebBrowser = userAgent.IndexOf("mozilla") > -1;
            if(isWebBrowser){
                return "WWW";
            }
            else
            {
                if (IsMobile())
                {
                    return "Wap";
                }
                else 
                {
                    return "WWW/未知设备";   
                }
            }
        }

        // TODO: tell web from wap
        // TODO: definition is not perfect
        // 0 wap
        // 1 web
        // 2 ios app
        // 3 android app
        // 4 weixin
        // 5 weixin mini app (微信小程序里会传5，再有其它类型从6开始)
        public static int GetTerminalId(string userAgent)
        {
            userAgent = userAgent.ToLower();
            bool isApp = userAgent.IndexOf(" weekendhotel/") > -1;
            bool isAndroid = userAgent.IndexOf("android") > -1;
            if (isApp)
            {
                return isAndroid ? 3 : 2;
            }
            bool isWeixin = userAgent.IndexOf("micromessenger") > -1;
            if (isWeixin)
            {
                return 4;
            }
            bool isWebBrowser = userAgent.IndexOf("mozilla") > -1;
            if (isWebBrowser)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}