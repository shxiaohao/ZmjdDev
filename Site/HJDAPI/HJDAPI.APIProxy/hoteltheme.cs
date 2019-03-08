using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HJD.DestServices.Contract;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers;
using HJDAPI.Controllers.Adapter;
using Newtonsoft.Json;
using HJD.HotelServices.Contracts;
using HJDAPI.Models;


namespace HJDAPI.APIProxy
{
    public class hoteltheme
    {
        static bool IsProductEvn = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["APIProxy_IsProductEvn"]);
        static string APISiteUrl = System.Configuration.ConfigurationManager.AppSettings["APISiteUrl"];

        public List<HotelThemeEntity> GetAllHotelTheme()
        {            
            if (IsProductEvn)
                return ResourceAdapter.GetAllHotelTheme();
            else
            {
                string url = APISiteUrl + "api/hoteltheme/getallhoteltheme";

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, null, ref cc);
                return JsonConvert.DeserializeObject<List<HotelThemeEntity>>(json);
            }
        }

        public static BrowsingRecordResult GetBrowsingRecordList(BsicSearchParam param)
        {
            string url = APISiteUrl + "api/HotelTheme/GetBrowsingRecordList";
            string postDataStr = string.Format("start={0}&count={1}&userId={2}", param.start, param.count, param.userId);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<BrowsingRecordResult>(json);
        }
    }
}
