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
using HJDAPI.Models;

namespace HJDAPI.APIProxy
{
    public class App : BaseProxy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PreLoadAppData GetPreLoadAppData(PreLoadAppDataParam param)
        {
            string url = APISiteUrl + "api/app/GetPreLoadAppData";
            string postDataStr = string.Format("DistrictID={0}&DistrictName={1}&Lon={2}&Lat={3}&UserID={4}", param.DistrictID, param.DistrictName, param.Lon, param.Lat, param.UserID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<PreLoadAppData>(json);
        }

        /// <summary>
        /// 最新版的搜索（目前app、h5都在使用的）
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="hotelcount"></param>
        /// <param name="citycount"></param>
        /// <param name="playcount"></param>
        /// <param name="foodcount"></param>
        /// <returns></returns>
        public List<QuickSearchSuggestItem> Search(string keyword, int hotelcount = 20, int citycount = 0, int playcount = 0, int foodcount = 0)
        {
            string url = APISiteUrl + "api/Search/Search";
            string postDataStr = string.Format("keyword={0}&hotelcount={1}&citycount={2}&playcount={3}&foodcount={4}", keyword, hotelcount, citycount, playcount, foodcount);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<QuickSearchSuggestItem>>(json);
        }

    }
}
