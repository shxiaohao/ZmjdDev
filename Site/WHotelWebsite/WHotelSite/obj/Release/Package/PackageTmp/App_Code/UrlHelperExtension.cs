using System;
using System.Collections.Generic;
using System.Linq;
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
            if (string.IsNullOrEmpty(interest))
            {
                return urlHelper.RouteUrl(sctype == 2 ? "酒店列表-城市及周边" : "酒店列表-城市", new { star, price, city, sort });
            }

            if (type == "theme")
            {
                return urlHelper.RouteUrl(sctype == 2 ? "主题-酒店列表-城市及周边" : "主题-酒店列表-城市", new { star, price, interest, city, sort });
            }

            return urlHelper.RouteUrl(sctype == 2 ? "景区-酒店列表-城市及周边" : "景区-酒店列表-城市", new { star, price, interest, city, sort });
        }
    }
}