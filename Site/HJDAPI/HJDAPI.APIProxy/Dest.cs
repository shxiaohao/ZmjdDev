using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HJD.DestServices.Contract;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers;
using Newtonsoft.Json;

namespace HJDAPI.APIProxy
{
    public class Dest : BaseProxy
    {

        public SimpleCityInfo GetCityInfoByName(string cityName, float lat, float lon)
        {

            if (IsProductEvn)
                return ResourceAdapter.GetCityInfoByName(cityName, lat, lon);
            else
            {
                string url = APISiteUrl + "api/dest/GetCityInfoByName";
                string postDataStr = string.Format("cityname={0}&lat={1}&lon={2}"
                  , cityName, lat, lon);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<SimpleCityInfo>(json);
            }
        }

        public List<SimpleCityInfo> GetAroundCityInfo(float lat, float lon)
        {

            if (IsProductEvn)
                return ResourceAdapter.GetAroundCityInfo(lat, lon);
            else
            {
                string url = APISiteUrl + "api/dest/GetAroundCityInfo";
                string postDataStr = string.Format("lat={0}&lon={1}", lat, lon);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<SimpleCityInfo>>(json);
            }
        }

        public static List<DistrictInfoEntity> GetDistrictInfo(List<int> ids)
        {

            if (IsProductEvn)
                return ResourceAdapter.GetDistrictInfo(ids);
            else
            {
                string url = APISiteUrl + "api/dest/GetDistrictInfo";
                string postDataStr = string.Format("ids={0}"
                  , string.Join(",", ids));

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<DistrictInfoEntity>>(json);
            }
        }

        public static List<DistrictZoneEntity> GetStrategyDistrictZoneList(int districtId)
        {

            if (IsProductEvn)
                return ResourceAdapter.GetStrategyDistrictZoneList(districtId);
            else
            {
                string url = APISiteUrl + "api/dest/GetStrategyDistrictZoneList";
                string postDataStr = string.Format("districtId={0}", districtId);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<DistrictZoneEntity>>(json);
            }
        }
    }
}
