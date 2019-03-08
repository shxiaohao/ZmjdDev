using HJD.HotelManagementCenter.Domain.Fund;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
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
    public class Fund : BaseProxy
    {
        public UserFundEntity GetUserFundInfo(Int64 userId)
        {
            string url = APISiteUrl + "api/Fund/GetUserFundInfo";
            string postDataStr = string.Format("userId={0}", userId);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<UserFundEntity>(json);
        }

        public List<UserFundIncomeStatEntity> GetUserFundIncomeStat(Int64 userId)
        {
            string url = APISiteUrl + "api/Fund/GetUserFundIncomeStat";
            string postDataStr = string.Format("userId={0}", userId);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<UserFundIncomeStatEntity>>(json);
        }

        public List<UserFundExpendDetailEntity> GetUserFundExpendDetail(Int64 userId)
        {
            string url = APISiteUrl + "api/Fund/GetUserFundExpendDetail";
            string postDataStr = string.Format("userId={0}", userId);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<UserFundExpendDetailEntity>>(json);
        }

        public int DeductUserFund(Int64 userId, long orderId, decimal fund)
        {
            string url = APISiteUrl + "api/Access/DeductUserFund";
            string postDataStr = string.Format("userId={0}&orderId={1}&fund={2}", userId, orderId, fund);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int ReturnUserFund(Int64 userId, long orderId, decimal fund)
        {
            string url = APISiteUrl + "api/Access/ReturnUserFund";
            string postDataStr = string.Format("userId={0}&orderId={1}&fund={2}", userId, orderId, fund);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int AddUserFund(UserFundIncomeDetail detail)
        {
            string url = APISiteUrl + "api/Fund/AddUserFund";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, detail, ref cc);

            return JsonConvert.DeserializeObject<int>(json);
        }
    }
}
