using HJD.HotelManagementCenter.Domain;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class Channel : BaseProxy
    {
        public static BusinessOperatorEntity ChannelLogin(ChannelLoginParam param)
        {
            string url = APISiteUrl + "api/Shop/ChannelLogin";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<BusinessOperatorEntity>(json);
        }
        public static BusinessOperatorEntity PhoneNumChannelLogin(ChannelLoginParam param)
        {
            string url = APISiteUrl + "api/Shop/PhoneNumChannelLogin";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<BusinessOperatorEntity>(json);
        }
        public static int ExistsOperateName(string operatorName, int id)
        {
            string url = APISiteUrl + "api/Shop/ExistsOperateName";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("operatorName={0}&id={1}", operatorName, id);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public static bool  RemoveChannelInfoCache(long CID)
        {
            string url = APISiteUrl + "api/Channel/RemoveChannelInfoCache";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("CID={0}", CID);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }
    }
}
