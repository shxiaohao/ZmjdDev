using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers;
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
    public class Interest : BaseProxy
    {
        public static InterestModel2 QueryInterest20(int districtid, float lat, float lng, int distance = 300000)
        {
            if (IsProductEvn)
                return ResourceAdapter.QueryInterest20(districtid, lat, lng, distance);
            else
            {
                string url = APISiteUrl + "api/Interest/QueryInterest20";
                string postDataStr = string.Format("districtid={0}&lat={1}&lng={2}&distance={3}"
                  , districtid, lat, lng, distance);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<InterestModel2>(json);
            }
        }

        public static List<InterestEntity2> GetAllInterest()
        {
            if (IsProductEvn)
                return ResourceAdapter.GetAllInterest();
            else
            {
                string url = APISiteUrl + "api/Interest/GetAllInterest";
                string postDataStr ="";

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<InterestEntity2>>(json);
            }
        }

    }
}
