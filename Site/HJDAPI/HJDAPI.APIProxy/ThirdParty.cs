using HJDAPI.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class ThirdParty : BaseProxy
    {
        public static string LYTicketRefund(string param)
        {
            string url = APISiteUrl + "api/ThirdPary/ShopLogin";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return json;
        }



    }
}
