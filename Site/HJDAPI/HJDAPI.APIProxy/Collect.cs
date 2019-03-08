using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using Newtonsoft.Json;
using PersonalServices.Contract;
using PersonalServices.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class Collect : BaseProxy
    {
        public static CollectOptionResult Add(CollectParamModel param)
        {
            string url = APISiteUrl + "api/Collect/Add";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            CollectOptionResult result = JsonConvert.DeserializeObject<CollectOptionResult>(json);
            return result;
        }

        //remove collect 支持取消收藏
        public static CollectOptionResult Remove(CollectParamModel param)
        {
            string url = APISiteUrl + "api/Collect/Remove";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            CollectOptionResult result = JsonConvert.DeserializeObject<CollectOptionResult>(json);
            return result;
        }

        //get collect 由搜索条件返回需要的收藏列表
        public static CollectHotelResult GetCollectHotelList(long UserID)
        {
            string url = APISiteUrl + "api/Collect/GetCollectHotelList";
            CookieContainer cc = new CookieContainer();
            CollectParamModel model = new CollectParamModel();
            model.UserID = UserID;
            string json = HttpRequestHelper.PostJson(url, model, ref cc);
            CollectHotelResult result = JsonConvert.DeserializeObject<CollectHotelResult>(json);
            return result;
        }

        //get collect 同步APP酒店收藏列表
        public static CollectHotelResult SyncCollectList(CollectParamModel param)
        {
            string url = APISiteUrl + "api/Collect/SyncCollectList";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            CollectHotelResult result = JsonConvert.DeserializeObject<CollectHotelResult>(json);
            return result;
        }
    }
}
