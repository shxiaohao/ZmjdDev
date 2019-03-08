using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WHotelSite.Params.Hotel;

namespace WHotelSite.App_Code
{
    public static class UrlHelperExtension
    {
        public static string ListUrl(this UrlHelper urlHelper, int city, string interest, string star, string price, string sort, int sctype, string type = "theme")
        {
            sort = sort == "0" ? string.Empty : sort;
            star = star == "0" ? string.Empty : star;
            interest = interest == "0" ? string.Empty : interest;

            if (sctype == 3)
            {
                if (string.IsNullOrEmpty(interest))
                {
                    return urlHelper.RouteUrl("酒店列表-AroundCity", new { star, price, sort });
                }

                if (type == "theme")
                {
                    return urlHelper.RouteUrl("主题-酒店列表-AroundCity", new { star, price, interest, sort });
                }

                return urlHelper.RouteUrl("景区-酒店列表-AroundCity", new { star, price, interest, sort });
            }
            else
            {
                if (string.IsNullOrEmpty(interest))
                {
                    return urlHelper.RouteUrl("酒店列表-城市", new { star, price, city, sort });
                }

                if (type == "theme")
                {
                    return urlHelper.RouteUrl("主题-酒店列表-城市", new { star, price, interest, city, sort });
                }

                return urlHelper.RouteUrl("景区-酒店列表-城市", new { star, price, interest, city, sort });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isApp"></param>
        /// <param name="districtid"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string CityItemUrl(bool isApp, int districtid = 0, string title = "", int geoScopeType = -1)
        {
            //whotelapp://www.zmjiudian.com/strategy/place?districtid=15255&title=无锡
            //http://www.zmjiudian.com/strategy/place?districtid=15255&title=无锡

            var url = "";

            if (isApp)
	        {
                url = string.Format("whotelapp://www.zmjiudian.com/strategy/place?districtid={0}&title={1}", districtid, title);
                if (geoScopeType > 0)
                {
                    url += string.Format("&geoScopeType=" + geoScopeType);
                }
	        }
            else
            {
                url = string.Format("http://www.zmjiudian.com/city{0}", districtid);
            }

            return url;
        }

        public static string HomeDistrictBannerUrl(bool isApp, string whotelDistrictUrl)
        {
            if (isApp)
            {
                return whotelDistrictUrl;
            }

            return SplitWhotelDistrictUrlForWeb(whotelDistrictUrl);
        }

        public static string HomeDefBannerUrl(bool isApp, string bannerUrl)
        {
            if (isApp)
            {
                return bannerUrl;
            }

            var replaceUrl = HttpUtility.UrlDecode(bannerUrl);

            replaceUrl = replaceUrl.Replace("whotelapp://www.zmjiudian.com/gotopage?url=", "")
                .Replace("whotelapp://gotopage?url=", "")
                .Replace("whotelapp", "http")
                .Replace("{userid}", "0");

            return replaceUrl;
        }

        public static string SplitWhotelDistrictUrlForWeb(string whotelDistrictUrl)
        {
            var webUrl = whotelDistrictUrl;

            //whotelapp://www.zmjiudian.com/strategy/place?districtid=39085&title=台北
            if (!string.IsNullOrEmpty(whotelDistrictUrl))
	        {
                var baseUrl = "http://www.zmjiudian.com/city{0}";
                var splitKey = "districtid=";
                if (whotelDistrictUrl.ToLower().Trim().Contains("zoneid"))
                {
                    splitKey = "zoneid=";
                    baseUrl = "http://www.zmjiudian.com/region{0}";
                }

                var ulist = Regex.Split(whotelDistrictUrl.ToLower().Trim(), splitKey);
                if (ulist.Length > 1)
                {
                    var u1 = ulist[1];
                    var districtId = u1.Split('&')[0];
                    webUrl = string.Format(baseUrl, districtId);
                }
	        }

            return webUrl;
        }
    }
}