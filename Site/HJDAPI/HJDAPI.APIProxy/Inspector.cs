using HJD.HotelServices.Contracts;
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
    public class Inspector:BaseProxy
    {
        public ResultEntity CheckInspectorAccess(OperationResult item)
        {
            string url = APISiteUrl + "api/Inspector/CheckInspectorAccess";
            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, item, ref cc);
            return JsonConvert.DeserializeObject<ResultEntity>(json);
        }

        public ResultEntity BookInspectorHotel(BookInspectorHotelParam param)
        {
            string url = APISiteUrl + "api/Inspector/BookInspectorHotel";
            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<ResultEntity>(json);
        }

        public List<HotelVoucherAndInspectorHotel> GetHotelVoucherList(GetInspectorHotelsListParam param)
        {
            string url = APISiteUrl + "api/Inspector/GetHotelVoucherList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userid={0}&start={1}&counts={2}", param.userid,param.start,param.count);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<HotelVoucherAndInspectorHotel>>(json);
        }



        public bool HasBookedVoucherHotel(GetInspectorHotelByIdParam param)
        {
            string url = APISiteUrl + "api/Inspector/HasBookedVoucherHotel";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userid={0}&timeStamp={1}&sourceID={2}&requestType={3}&sign={4}&id={5}", param.userid, param.TimeStamp, param.SourceID, param.RequestType, param.Sign, param.id);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }


        public ResultEntity BookVoucherHotel(BookInspectorHotelParam param)
        {
            string url = APISiteUrl + "api/Inspector/BookVoucherHotel";
            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<ResultEntity>(json);
        }

        //[HttpGet]
        //public List<EvaluationerHotel> GetEvaluationerHotelList(long evaHotelID, long userID)
        //{
        //    return hotelService.GetEvaluationerHotelList(evaHotelID, userID);
        //}

        public InspectorHotelListResult GetInspectorHotelsList(GetInspectorHotelsListParam param)
        {
            string url = APISiteUrl + "api/Inspector/GetInspectorHotelsList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userid={0}&timeStamp={1}&sourceID={2}&requestType={3}&sign={4}", param.userid, param.TimeStamp, param.SourceID, param.RequestType, param.Sign);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<InspectorHotelListResult>(json);
        }

        public int IsInspector(GetInspectorHotelsListParam param)
        {
            string url = APISiteUrl + "api/Inspector/IsInspector";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userid={0}&timeStamp={1}&sourceID={2}&requestType={3}&sign={4}&identityCode={5}", param.userid, param.TimeStamp, param.SourceID, param.RequestType, param.Sign, param.identityCode);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public InspectorHotel GetInspectorHotelById(GetInspectorHotelByIdParam param)
        {
            string url = APISiteUrl + "api/Inspector/GetInspectorHotelById";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("id={0}&timeStamp={1}&sourceID={2}&requestType={3}&sign={4}", param.id, param.TimeStamp, param.SourceID, param.RequestType, param.Sign);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<InspectorHotel>(json);
        }

        public HotelVoucherEntity GetHotelVoucherById(GetInspectorHotelByIdParam param)
        {
            string url = APISiteUrl + "api/Inspector/GetHotelVoucherById";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("id={0}", param.id);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HotelVoucherEntity>(json);
        }

        public bool HasBookedInspectorHotel(GetInspectorHotelByIdParam param)
        {
            string url = APISiteUrl + "api/Inspector/HasBookedInspectorHotel";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userid={0}&timeStamp={1}&sourceID={2}&requestType={3}&sign={4}&id={5}", param.userid, param.TimeStamp, param.SourceID, param.RequestType, param.Sign, param.id);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public int GetAvailablePointByUserID(GetInspectorHotelByIdParam param)
        {
            string url = APISiteUrl + "api/Inspector/GetAvailablePointByUserID";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userid={0}&timeStamp={1}&sourceID={2}&requestType={3}&sign={4}&typeid={5}", param.userid, param.TimeStamp, param.SourceID, param.RequestType, param.Sign, param.typeid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int IsUserHasApplyInspector(InspectorApplyData data)
        {
            string url = APISiteUrl + "api/Inspector/IsUserHasApplyInspector";
            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, data, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public ResultEntity SubmitInspectorApplyData(InspectorApplyData item)
        {
            string url = APISiteUrl + "api/Inspector/SubmitInspectorApplyData";
            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, item, ref cc);
            return JsonConvert.DeserializeObject<ResultEntity>(json);
        }

        public List<UserTagOption> GetUserTagOptionList()
        {
            string url = APISiteUrl + "api/Inspector/GetUserTagOptionList";
            CookieContainer cc = new CookieContainer();
            //string postDataStr = string.Format("userid={0}&timeStamp={1}&sourceID={2}&requestType={3}&sign={4}&typeid={5}", param.userid, param.TimeStamp, param.SourceID, param.RequestType, param.Sign, param.typeid);
            string json = HttpRequestHelper.Get(url, "", ref cc);
            return JsonConvert.DeserializeObject<List<UserTagOption>>(json);
        }
    }
}