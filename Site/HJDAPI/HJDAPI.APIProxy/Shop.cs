using HJD.CouponService.Contracts.Entity;
using HJD.HotelManagementCenter.Domain;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using Newtonsoft.Json;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class Shop : BaseProxy
    {
        public static BusinessOperatorEntity ShopLogin(ShopLoginParam param)
        {
            string url = APISiteUrl + "api/Shop/ShopLogin";
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
        //public static CouponInfoEntity GetCouponInfoByExchangeNo(string exchangeNo)
        //{
        //    string url = APISiteUrl + "api/Shop/GetCouponInfoByExchangeNo";
        //    CookieContainer cc = new CookieContainer();
        //    string postData = string.Format("exchangeNo={0}", exchangeNo);
        //    string json = HttpRequestHelper.Get(url, postData, ref cc);
        //    return JsonConvert.DeserializeObject<CouponInfoEntity>(json);
        //}

        public static int UpdateExchangeState(ExchangeCouponEntity param)
        {
            string url = APISiteUrl + "api/Shop/UpdateExchangeState";
            CookieContainer cc = new CookieContainer();
            //string postData = string.Format("exchangeNo={0}&state={1}&id={2}&updator={3}", paramexchangeNo, state, id, updator);
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public static int AddUsedConsumerCouponInfo(UsedConsumerCouponInfoEntity param)
        {
            string url = APISiteUrl + "api/Shop/AddUsedConsumerCouponInfo";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }
        public static int AddEAPointsAfterCheck(UsedConsumerCouponInfoEntity param)
        {
            string url = APISiteUrl + "api/Shop/AddEAPointsAfterCheck";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }
        public static List<UsedCouponProductEntity> GetUsedCouponProductBySupplierId(int supplierId, string startTime = "2000-01-01", string endTime = "2100-01-01", int start = 0, int count = 100)
        {
            string url = APISiteUrl + "api/Shop/GetUsedCouponProductBySupplierId";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("supplierId={0}&startTime={1}&endTime={2}&start={3}&count={4}", supplierId, startTime, endTime, start, count);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<List<UsedCouponProductEntity>>(json);
        }
        public static List<UsedCouponProductEntity> GetUsedCouponProductByOperatorId(int operatorId, string startTime = "2000-01-01", string endTime = "2100-01-01", int start = 0, int count = 100)
        {
            string url = APISiteUrl + "api/Shop/GetUsedCouponProductByOperatorId";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("operatorId={0}&startTime={1}&endTime={2}&start={3}&count={4}", operatorId, startTime, endTime, start, count);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<List<UsedCouponProductEntity>>(json);
        }
        public static UsedProductCouponEntity GetSumCouponProductByOperatorId(int operatorId, DateTime startTime , DateTime endTime, int start = 0, int count = 100000)
        {
            string url = APISiteUrl + "api/Shop/GetSumCouponProductByOperatorId";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("operatorId={0}&startTime={1}&endTime={2}&start={3}&count={4}", operatorId, startTime, endTime, start, count);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<UsedProductCouponEntity>(json);
        }
        public static SupplierEntity GetSupplierById(long supplierId)
        {
            string url = APISiteUrl + "api/Shop/GetSupplierById";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("supplierId={0}", supplierId);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<SupplierEntity>(json);
        }

        public static ExchangeCouponEntity GetOneExchangeCouponInfo(int couponID, string exchangeNo)
        {
            string url = APISiteUrl + "api/Shop/GetOneExchangeCouponInfo";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("couponID={0}&exchangeNo={1}", couponID, exchangeNo);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<ExchangeCouponEntity>(json);
        }
        public static SKUInfoEntity GetSKUByID(int SKUID)
        {
            string url = APISiteUrl + "api/Shop/GetSKUByID";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("SKUID={0}", SKUID);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<SKUInfoEntity>(json);
        }
        public static UsedProductCouponEntity GetSumCouponProductBySupplierId(int supplierId, DateTime startTime, DateTime endTime, int start = 0, int count = 10000)
        {
            string url = APISiteUrl + "api/Shop/GetSumCouponProductBySupplierId";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("supplierId={0}&startTime={1}&endTime={2}&start={3}&count={4}", supplierId, startTime, endTime, start, count);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<UsedProductCouponEntity>(json);
        }
        public static BookNoUsedExchangeCouponEntity GetBookNoUsedExchangeCouponBySupplierId(int supplierId, DateTime startTime, DateTime endTime)
        {
            string url = APISiteUrl + "api/Shop/GetBookNoUsedExchangeCouponBySupplierId";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("supplierId={0}&startTime={1}&endTime={2}", supplierId, startTime, endTime);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<BookNoUsedExchangeCouponEntity>(json);
        }
        public static UsedConsumerCouponInfoEntity GetUsedCouponProductByExchangeNo(string exchangeNo)
        {
            string url = APISiteUrl + "api/Shop/GetUsedCouponProductByExchangeNo";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("exchangeNo={0}", exchangeNo);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<UsedConsumerCouponInfoEntity>(json);
        }

        public static RetailerShopEntity GetRetailerShopByCID(long CID)
        {
            string url = APISiteUrl + "api/Shop/GetRetailerShopByCID";
            string getDataStr = string.Format("CID={0}", CID);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, getDataStr, ref cc);
            return JsonConvert.DeserializeObject<RetailerShopEntity>(json);

        }

        public static int AddOrUpdateRetailerShop(RetailerShopEntity param)
        {
            string url = APISiteUrl + "api/Shop/AddOrUpdateRetailerShop";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }
        public static int UpdateAvatarUrl(long CID, string avatarUrl)
        {
            string url = APISiteUrl + "api/Shop/UpdateAvatarUrl";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("CID={0}&avatarUrl", CID,avatarUrl);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public static int UpdateRetailerShop(RetailerShopEntity param)
        {
            string url = APISiteUrl + "api/Shop/UpdateRetailerShop";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

    }
}
