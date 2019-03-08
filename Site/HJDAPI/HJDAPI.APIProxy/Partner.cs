using HJD.HotelManagementCenter.Domain;
using HJDAPI.Common.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class Partner : BaseProxy
    {
        public static RetailerInvateState GetRetailerInvateState(long userID)
        {
            string url = APISiteUrl + "api/Partner/GetRetailerInvateState";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userID={0}", userID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RetailerInvateState>(json);
        }


        public static RetailerInvateEntity GetRetailerInvate(long userID)
        {
            string url = APISiteUrl + "api/Partner/GetRetailerInvate";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userID={0}", userID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RetailerInvateEntity>(json);
        }


        public static bool AddRetailerInvateinfo(RetailerInvateEntity param)
        {
            string url = APISiteUrl + "api/Partner/AddRetailerInvateinfo";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }
    }
}
