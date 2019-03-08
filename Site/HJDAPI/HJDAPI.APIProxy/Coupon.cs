using HJD.AccountServices.Entity;
using HJD.CouponService.Contracts;
using HJD.CouponService.Contracts.Entity;
using HJD.HotelManagementCenter.Domain.Settlement;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Models;
using HJDAPI.Models.Coupon;
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
    public class Coupon : BaseProxy
    {
        #region 券照片更新
        /// <summary>
        ///  用户是否可以重新上传照片
        /// </summary>
        /// <param name="userid"></param> 
        public Int32 GetUserCanReUpdatePhotoListByUserID(long userid)
        {
             if (IsProductEvn)
            {
                return CouponAdapter.GetUserCanReUpdatePhotoListByUserID(userid);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/GetUserCanReUpdatePhotoListByUserID";
                CookieContainer cc = new CookieContainer();
                string postDataStr = string.Format("userId={0}", userid);
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<Int32>(json);
            }
        }

        /// <summary>
        /// 获取用户可以更新的券照片的ID
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="CouponID"></param>
        /// <param name="operatorUserID"></param>
        /// <returns></returns> 
        public bool SetUserCanReUpdatePhotoListByUserID(long userid, Int32 couponID, long operatorUserID)
        {
             if (IsProductEvn)
            {
                return CouponAdapter.SetUserCanReUpdatePhotoListByUserID(userid, couponID, operatorUserID);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/SetUserCanReUpdatePhotoListByUserID";
                CookieContainer cc = new CookieContainer();
                string postDataStr = string.Format("userId={0}&CouponID={1}&operatorUserID={2}", userid, couponID, operatorUserID);
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<bool>(json);                 
            }
        }
           

        /// <summary>
        /// 更新券信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public BaseResponse UpdateCoupnPhoto(UpdateCouponPhotoReqParms p)
        {
            if (IsProductEvn)
            {
                return CouponAdapter.UpdateCoupnPhoto(p);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/UpdateCoupnPhoto";
                CookieContainer cc = new CookieContainer(); 
                string json = HttpRequestHelper.PostJson(url, p, ref cc);
                return JsonConvert.DeserializeObject<BaseResponse>(json); 
            }
        }
        #endregion


        #region 使用红包
        public static int GetUserCanUseCashCouponAmount(long userid)
        {
            if (IsProductEvn)
            {
                return CouponAdapter.GetUserCanUseCashCouponAmount(userid);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/GetUserCanUseCashCouponAmount";
                CookieContainer cc = new CookieContainer();
                string postDataStr = string.Format("userId={0}", userid);
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<int>(json);
            }
        }

        //public static ReturnCouponResult SetOrderCoupon(long userid, long orderid, int couponAmount, bool isBudget)
        //{
        //    if (IsProductEvn)
        //    {
        //        return CouponAdapter.SetOrderCoupon(userid, orderid, couponAmount, isBudget);
        //    }
        //    else
        //    {
        //        string url = APISiteUrl + "api//SetOrderCoupon";
        //        CookieContainer cc = new CookieContainer();
        //        var xx = new { userid = userid, orderid = orderid, couponAmount = couponAmount, isBudget = isBudget };
        //        string json = HttpRequestHelper.PostJson(url, xx, ref cc);
        //        return JsonConvert.DeserializeObject<ReturnCouponResult>(json);
        //    }
        //}

        public static List<OriginCoupon> GetCashCouponList(long userId)
        {
            if (IsProductEvn)
            {
                return CouponAdapter.GetCashCouponList(userId);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/GetCashCouponList";
                CookieContainer cc = new CookieContainer();
                //var xx = new { userId = userId};
                //string json = HttpRequestHelper.PostJson(url, xx, ref cc);
                string postDataStr = string.Format("userId={0}", userId);
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<List<OriginCoupon>>(json);
            }
        }

        public static WalletResult GetWalletList(long userId)
        {
            string url = APISiteUrl + "api/Coupon/GetWalletList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}", userId);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<WalletResult>(json);
        }
        #endregion

        #region 生成红包
        //public OriginCouponResult GenerateOriginCoupon(long userId, int typeId, long sourceId)
        //{
        //    if (IsProductEvn)
        //    {
        //        //return CouponAdapter.SetOrderCoupon(userId, typeId, sourceId);
        //    }
        //    else
        //    {
        //        string url = APISiteUrl + "api/Coupon/SetOrderCoupon";
        //        CookieContainer cc = new CookieContainer();
        //        var xx = new { userId = userId, typeId = typeId, orderid = sourceId};
        //        string json = HttpRequestHelper.PostJson(url, xx, ref cc);
        //        //return JsonConvert.DeserializeObject<ReturnCouponResult>(json);
        //    }
        //    return new OriginCouponResult();
        //}

        public static AcquiredCoupon GetAcquiredCoupon(long userId, string guid, string phoneNo)
        {
            if (IsProductEvn)
            {
                return CouponAdapter.GetAcquiredCoupon(userId, guid, phoneNo);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/GetAcquiredCoupon";
                CookieContainer cc = new CookieContainer();
                CouponModelParam param = new CouponModelParam() { userId = userId, guid = guid, phoneNo = phoneNo };
                string json = HttpRequestHelper.PostJson(url, param, ref cc);
                return JsonConvert.DeserializeObject<AcquiredCoupon>(json);
            }
        }

        public static List<AcquiredCoupon> GetCurrentAcquiredCouponList(string guid)
        {
            string url = APISiteUrl + "api/Coupon/GetCurrentAcquiredCouponList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("guid={0}", guid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<AcquiredCoupon>>(json);
        }

        public static bool IsAllAcquired(string guid)
        {
            string url = APISiteUrl + "api/Coupon/IsAllAcquired";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("guid={0}", guid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public static bool GetProductAlbumSKUBySKUIDAndAlbumId(int skuid, int albumid)
        {
            string url = APISiteUrl + "api/Coupon/GetProductAlbumSKUBySKUIDAndAlbumId";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("skuid={0}&albumid={1}", skuid, albumid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public static OriginCouponResult UpdateCashCoupon(long id, string guid, int state)
        {
            if (IsProductEvn)
            {
                return CouponAdapter.UpdateCashCoupon(id, guid, state);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/UpdateCashCoupon";
                CookieContainer cc = new CookieContainer();
                CouponModelParam param = new CouponModelParam() { id = id, guid = guid, state = state };
                string json = HttpRequestHelper.PostJson(url, param, ref cc);
                return JsonConvert.DeserializeObject<OriginCouponResult>(json);
            }
        }

        public static OriginCouponResult UpdateCashCoupon20(long id, string guid, int state)
        {
            if (IsProductEvn)
            {
                return CouponAdapter.UpdateCashCoupon20(id, guid, state);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/UpdateCashCoupon20";
                CookieContainer cc = new CookieContainer();
                CouponModelParam param = new CouponModelParam() { id = id, guid = guid, state = state };
                string json = HttpRequestHelper.PostJson(url, param, ref cc);
                return JsonConvert.DeserializeObject<OriginCouponResult>(json);
            }
        }
        #endregion

        #region 使用券

        //        获取指定产品类型指定金额下，所有可用的券（订单确认页使用，暂时可以不分页）
        public List<UserCouponItemInfoEntity> GetCanUseCouponInfoListForOrder(OrderUserCouponRequestParams param)
        {
            string url = APISiteUrl + "api/Coupon/GetCanUseCouponInfoListForOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<List<UserCouponItemInfoEntity>>(json);
        }

        //获取指定产品类型指定金额下，所有不可用的券（订单确认页使用，暂时可以不分页）

        public List<UserCouponItemInfoEntity> GetCannotUseCouponInfoListForOrder(OrderUserCouponRequestParams param)
        {
            string url = APISiteUrl + "api/Coupon/GetCannotUseCouponInfoListForOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<List<UserCouponItemInfoEntity>>(json);
        }

        //获取指定产品类型指定金额下，默认最优惠的券（订单确认页需要默认选择一个券）        
        public UserCouponItemInfoEntity GetTheBestCouponInfoForOrder(OrderUserCouponRequestParams param)
        {
            string url = APISiteUrl + "api/Coupon/GetTheBestCouponInfoForOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<UserCouponItemInfoEntity>(json);
        }

        // 检测指定产品类型指定金额下，当前券ID是否满足条件（比如用户选好了一个券后，手动变更数量时，需要实时检测）     
        public CheckSelectedCashCouponInfoForOrderViewModel CheckSelectedCashCouponInfoForOrder(OrderUserCouponRequestParams param)
        {
            string url = APISiteUrl + "api/Coupon/CheckSelectedCashCouponInfoForOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<CheckSelectedCashCouponInfoForOrderViewModel>(json);
        }

        #endregion

        #region 预约

         public static  List<PDayItem> GetBookDateList(int skuid)
         {
             string url = APISiteUrl + "api/Coupon/GetBookDateList";
             CookieContainer cc = new CookieContainer();
             string postDataStr = string.Format("skuid={0}", skuid);
             string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
             return JsonConvert.DeserializeObject<List<PDayItem>>(json);
         }

         public static BaseResult SubmitBookInfo(BookPersonInfoEntity model)
         {
             string url = APISiteUrl + "api/Coupon/SubmitBookInfo";
             CookieContainer cc = new CookieContainer();
             string json = HttpRequestHelper.PostJson(url, model, ref cc);
             return JsonConvert.DeserializeObject<BaseResult>(json); 
         }

        #endregion


        public static GetRetailProductListResult GetRetailProductList(int pageSize, int start)
        {
            string url = APISiteUrl + "api/Coupon/GetRetailProductList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("pageSize={0}&start={1}", pageSize, start);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<GetRetailProductListResult>(json);
        }

        public static GetRetailProductListResult GetRetailerShopProductList(int pageSize, int start, int cid = 0)
        {
            string url = APISiteUrl + "api/Coupon/GetRetailerShopProductList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("pageSize={0}&start={1}&cid={2}", pageSize, start, cid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<GetRetailProductListResult>(json);
        }

        public static CouponActivityRetailDetailEntity GetRetailProductByID(int ID, long CID)
        {
            string url = APISiteUrl + "api/Coupon/GetRetailProductByID";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("ID={0}&CID={1}", ID, CID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CouponActivityRetailDetailEntity>(json);
        }

        public static string GetOriginGUID(long sourceId, int typeId)
        {
            if (IsProductEvn)
            {
                return CouponAdapter.GetOriginGUID(sourceId, typeId);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/GetOriginGUID";
                CookieContainer cc = new CookieContainer();
                CouponModelParam param = new CouponModelParam() { sourceId = sourceId, typeId = typeId };
                string json = HttpRequestHelper.PostJson(url, param, ref cc);
                return JsonConvert.DeserializeObject<string>(json);
            }
        }

        public static bool HasParticipateActivity(string guid, string phoneNum)
        {
            string url = APISiteUrl + "api/Coupon/HasParticipateActivity";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("guid={0}&phoneNum={1}", guid, phoneNum);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public static List<AcquiredCoupon> GetAcquiredCouponList(string guid)
        {
            string url = APISiteUrl + "api/Coupon/GetAcquiredCouponList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("guid={0}", guid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<AcquiredCoupon>>(json);
        }

        public static OriginCoupon GetCashCoupon(long id, string guid)
        {
            if (IsProductEvn)
            {
                return CouponAdapter.GetCashCoupon(id, guid);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/GetCashCoupon";
                CookieContainer cc = new CookieContainer();
                string postDataStr = string.Format("id={0}&guid={1}", id, guid);
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<OriginCoupon>(json);
            }
        }

        public static InspectorRewardResult GetInspectorRewardResult(long userid)
        {
            string url = APISiteUrl + "api/Coupon/GetInspectorRewardResult";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userid={0}", userid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<InspectorRewardResult>(json);
        }

        public static AcquiredCoupon GetAcquiredCouponById(long id)
        {
            if (IsProductEvn)
            {
                return CouponAdapter.GetAcquiredCouponById(id);
            }
            else
            {
                string url = APISiteUrl + "api/Coupon/GetAcquiredCouponById";
                CookieContainer cc = new CookieContainer();
                string postDataStr = string.Format("id={0}", id);
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<AcquiredCoupon>(json);
            }
        }

        public static CouponResult GetPersonalCoupon(GetInspectorHotelsListParam param)
        {
            string url = APISiteUrl + "api/Coupon/GetPersonalCoupon";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}&timeStamp={1}&sourceID={2}&requestType={3}&sign={4}", param.userid, param.TimeStamp, param.SourceID, param.RequestType, param.Sign);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CouponResult>(json);
        }

        public static PointResult GetPersonalPoint(GetInspectorHotelsListParam param)
        {
            string url = APISiteUrl + "api/Coupon/GetPersonalPoint";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}&timeStamp={1}&sourceID={2}&requestType={3}&sign={4}", param.userid, param.TimeStamp, param.SourceID, param.RequestType, param.Sign);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<PointResult>(json);
        }

        public static BaseResponse ConsumeUserPoints(ConsumeUserPointsParam param)
        {
            string url = APISiteUrl + "api/Coupon/ConsumeUserPoints";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<BaseResponse>(json);
        }

        public static OrderCancelResult GenPoint(PointsEntity pe)
        {
            string url = APISiteUrl + "api/Coupon/GenPoint";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, pe, ref cc);
            return JsonConvert.DeserializeObject<OrderCancelResult>(json);
        }

        public static CouponActivityDetailModel GetCouponActivityDetail(int activityID)
        {
            string url = APISiteUrl + "api/Coupon/GetCouponActivityDetail";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("activityID={0}", activityID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CouponActivityDetailModel>(json);
        }

        public static CouponActivityDetailModel GetGroupCouponActivityDetail(int activityID)
        {
            string url = APISiteUrl + "api/Coupon/GetGroupCouponActivityDetail";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("activityID={0}", activityID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CouponActivityDetailModel>(json);
        }

        public static PromotionCheckEntity CheckProductPromotionForCoupon(int skuid, int buynum, int userid, int sType)
        {
            string url = APISiteUrl + "api/Coupon/CheckProductPromotionForCoupon";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("skuid={0}&buynum={1}&userid={2}&sType={3}", skuid, buynum, userid, sType);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<PromotionCheckEntity>(json);
        }

        public static SKUCouponActivityDetailModel GetSKUCouponActivityDetail(int SKUID, long userid = 0, long couponOrderId = 0)
        {
            string url = APISiteUrl + "api/Coupon/GetSKUCouponActivityDetail";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("SKUID={0}&userid={1}&couponOrderId={2}", SKUID, userid, couponOrderId);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<SKUCouponActivityDetailModel>(json);
        }

        /// <summary>
        /// 获取指定SKUID的消费券产品列表
        /// </summary>
        /// <param name="skuids"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static List<SKUCouponActivityEntity> GetSKUCouponActivityListBySKUIds(string skuids, int userid = 0)
        {
            string url = APISiteUrl + "api/Coupon/GetSKUCouponActivityListBySKUIds";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("skuids={0}&userid={1}", skuids, userid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<SKUCouponActivityEntity>>(json);
        }

        /// <summary>
        /// 根据专辑ID获取专辑信息及SKUCouponActivity列表
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static SKUCouponActivityAlbumEntity GetProductAlbumSKUCouponActivityListByAlbumID(int albumId, int start, int count, int userid = 0, bool searchReceive = false,int sort = 0)
        {
            string url = APISiteUrl + "api/Coupon/GetProductAlbumSKUCouponActivityListByAlbumID";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("albumId={0}&start={1}&count={2}&userid={3}&searchReceive={4}&sort={5}", albumId, start, count, userid, searchReceive, sort);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<SKUCouponActivityAlbumEntity>(json);
        }

        /// <summary>
        /// 根据专辑ID获取专辑信息及SKUCouponActivity列表
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static SKUCouponActivityAlbumEntity GetProductSKUCouponList(int category = 14, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int userid = 0, int sort = 0, double locLat = 0, double locLng = 0)
        {
            string url = APISiteUrl + "api/Coupon/GetProductSKUCouponList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("category={0}&districtID={1}&lat={2}&lng={3}&geoScopeType={4}&start={5}&count={6}&userid={7}&sort={8}&locLat={9}&locLng={10}", category, districtID, lat, lng, geoScopeType, start, count, userid, sort, locLat, locLng);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<SKUCouponActivityAlbumEntity>(json);
        }

        public CouponOrderResult SubmitExchangeOrder(SubmitCouponOrderModel scom)
        {
            string url = APISiteUrl + "api/Coupon/SubmitExchangeOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, scom, ref cc);
            return JsonConvert.DeserializeObject<CouponOrderResult>(json);
        }

        public CouponOrderResult SubmitExchangeOrderForProduct(SubmitCouponOrderModel scom)
        {
            string url = APISiteUrl + "api/Coupon/SubmitExchangeOrderForProduct";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, scom, ref cc);
            return JsonConvert.DeserializeObject<CouponOrderResult>(json);
        }

        public CouponOrderResult SubmitGroupOrderForProduct(SubmitCouponOrderModel scom)
        {
            string url = APISiteUrl + "api/Coupon/SubmitGroupOrderForProduct";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, scom, ref cc);
            return JsonConvert.DeserializeObject<CouponOrderResult>(json);
        }
        public GroupSKUCouponActivityModel GetGroupSKUCouponActivityModel(int SKUID, long userId, int groupid, string openId = "")
        {
            string url = APISiteUrl + "api/Coupon/GetGroupSKUCouponActivityModel";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("SKUID={0}&userId={1}&groupid={2}&openId={3}", SKUID, userId, groupid, string.IsNullOrEmpty(openId) ? "" : openId);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<GroupSKUCouponActivityModel>(json);
        }
        public BuyCouponCheckNumResult IsExchangeCouponSoldOut(int activityID, long userID)
        {
            string url = APISiteUrl + "api/Coupon/IsExchangeCouponSoldOut";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("activityID={0}&userID={1}", activityID, userID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<BuyCouponCheckNumResult>(json);
        }

        //public RoomCouponOrderEntity GetOneRoomCouponOrderEntity(int couponOrderID)
        //{
        //    string url = APISiteUrl + "api/Coupon/GetOneRoomCouponOrderEntity";
        //    CookieContainer cc = new CookieContainer();
        //    string postDataStr = string.Format("couponOrderID={0}", couponOrderID);
        //    string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
        //    return JsonConvert.DeserializeObject<RoomCouponOrderEntity>(json);
        //}
        public static BaseResponse Add2RefundCoupon(int couponID, string userId)
        {
            string url = APISiteUrl + "api/Coupon/Add2RefundCoupon";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("couponID={0}&userId={1}", couponID, userId);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<BaseResponse>(json);
        }
        public static BaseResponse Add2RefundCoupon(int couponID, long userId = 0, long editorUserID = 0, string phoneNum = "", string remark = "")
        {
            string url = APISiteUrl + "api/Coupon/Add2RefundCoupon";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("couponID={0}&userId={1}&editorUserID={2}&phoneNum={3}&remark={4}", couponID, userId, editorUserID, phoneNum, remark);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<BaseResponse>(json);
        }

        public static KeyValueEntity BatchAdd2RefundCoupon(BatchAdd2RefundCouponEntity param)
        {
            string url = APISiteUrl + "api/Coupon/BatchAdd2RefundCoupon";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<KeyValueEntity>(json);
        }

        public RoomCouponOrderEntity GenCouponPayCompleteResult(int couponOrderID)
        {
            string url = APISiteUrl + "api/Coupon/GenCouponPayCompleteResult";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("couponOrderID={0}", couponOrderID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RoomCouponOrderEntity>(json);
        }

        public RoomCouponResult GetPersonalRoomCoupon(GetInspectorHotelsListParam param)
        {
            string url = APISiteUrl + "api/Coupon/GetPersonalRoomCoupon";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}&timeStamp={1}&sourceID={2}&requestType={3}&sign={4}", param.userid, param.TimeStamp, param.SourceID, param.RequestType, param.Sign);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RoomCouponResult>(json);
        }

        public int GetPersonalProductCouponCount(long userId)
        {
            string url = APISiteUrl + "api/Coupon/GetPersonalProductCouponCount";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}", userId);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public ProductCouponResult GetPersonalProductCoupon(long userId)
        {
            string url = APISiteUrl + "api/Coupon/GetPersonalProductCoupon";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}", userId);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ProductCouponResult>(json);
        }

        public int IsExchangeOrderCanPay(int couponOrderID)
        {
            string url = APISiteUrl + "api/Coupon/IsExchangeOrderCanPay";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("couponOrderID={0}", couponOrderID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public List<UserCouponDefineEntity> GetNewVIPGiftUserCouponList()
        {
            string url = APISiteUrl + "api/Coupon/GetNewVIPGiftUserCouponList";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            return JsonConvert.DeserializeObject<List<UserCouponDefineEntity>>(json);
        }
        public CommOrderEntity GetOneCommOrderEntity(int couponOrderID)
        {
            string url = APISiteUrl + "api/Coupon/GetOneCommOrderEntity";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("couponOrderID={0}", couponOrderID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CommOrderEntity>(json);
        }

        public RoomCouponActivityListModel GetSpeciallyCheapRoomCouponActivityList(int advID = 0, int groupNo = 0, int isVip = 0, int count = -1)
        {
            string url = APISiteUrl + "api/Coupon/GetSpeciallyCheapRoomCouponActivityList";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, string.Format("advID={0}&groupNo={1}&isVip={2}&count={3}", advID, groupNo, isVip, count), ref cc);
            return JsonConvert.DeserializeObject<RoomCouponActivityListModel>(json);
        }

        public int UpdateActivityManuSellNum4BG(int activityID, int type, int manualSellNum)
        {
            string url = APISiteUrl + "api/Coupon/UpdateActivityManuSellNum4BG";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("activityID={0}&type={1}&manualSellNum={2}", activityID, type, manualSellNum);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 检查当前用户是否属于【预约 & [品鉴师|候选|有预订记录] & 非会员】的用户
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int IsVipNoPayReserveUser(string userid)
        {
            string url = APISiteUrl + "api/Coupon/IsVipNoPayReserveUser";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("userid={0}", userid);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 提交兑换订单的参数
        /// </summary>
        /// <param name="submitParam"></param>
        /// <returns></returns>
        public static SubmitExchangeRoomOrderResult SubmitExchangeRoomOrder(SubmitExchangeRoomOrderParam submitParam)
        {
            string url = APISiteUrl + "api/Coupon/SubmitExchangeRoomOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, submitParam, ref cc);
            return JsonConvert.DeserializeObject<SubmitExchangeRoomOrderResult>(json);
        }

        public static ExchangeCouponPackageInfo GetCouponPackageInfo(int couponID)
        {
            string url = APISiteUrl + "api/Coupon/GetCouponPackageInfo";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("couponID={0}", couponID);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<ExchangeCouponPackageInfo>(json);
        }

        public ExchangeRoomOrderConfirmResult IsExchangeNeedAddMoney(SubmitExchangeRoomOrderParam submitParam)
        {
            string url = APISiteUrl + "api/Coupon/IsExchangeNeedAddMoney";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, submitParam, ref cc);
            return JsonConvert.DeserializeObject<ExchangeRoomOrderConfirmResult>(json);
        }

        public static bool MarkCouponPayOrder(long orderid, int SettlePrice, long curUserid)
        {
            string url = APISiteUrl + "api/Coupon/MarkCouponPayOrder";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("orderid={0}&SettlePrice={1}&curUserid={2}", orderid, SettlePrice, curUserid);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        /// <summary>
        /// 提交会员订单的参数
        /// </summary>
        /// <param name="submitParam"></param>
        /// <returns></returns>
        public static CouponOrderResult SubmitMemberOrder(SubmitCouponOrderModel submitParam)
        {
            string url = APISiteUrl + "api/Coupon/SubmitMemberOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, submitParam, ref cc);
            return JsonConvert.DeserializeObject<CouponOrderResult>(json);
        }

        /// <summary>
        /// 获取指定用户的现金券信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public GetUserCanUseCashCouponAmountResponse GetUserCanUseCashCouponAmountInfo(GetUserCanUseCashCouponAmountRequestParams p)
        {
            string url = APISiteUrl + "api/Coupon/GetUserCanUseCashCouponAmount";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, p, ref cc);
            return JsonConvert.DeserializeObject<GetUserCanUseCashCouponAmountResponse>(json);
        }

        /// <summary>
        /// 根据userid获取该用户的现金券信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetUserCanUseCashCouponAmountByUid(long userId)
        {
            string url = APISiteUrl + "api/Coupon/GetUserCanUseCashCouponAmountByUid";
            CookieContainer cc = new CookieContainer();
            string postData = string.Format("userId={0}", userId);
            string json = HttpRequestHelper.Get(url, postData, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public static List<RedShareEntity> GetRedShareEntityByGUID(string guid)
        {
            string url = APISiteUrl + "api/Coupon/GetRedShareEntityByGUID";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, string.Format("guid={0}", guid), ref cc);
            return JsonConvert.DeserializeObject<List<RedShareEntity>>(json);
        }
        //public int AddRedShare(RedShareEntity param)
        //{
        //    string url = APISiteUrl + "api/Coupon/AddRedShare";
        //    CookieContainer cc = new CookieContainer();
        //    string json = HttpRequestHelper.PostJson(url, param, ref cc);
        //    return JsonConvert.DeserializeObject<int>(json);
        //}

        //public string GetRedShareURL(string k)
        //{
        //    string url = APISiteUrl + "api/Coupon/GetRedShareURL";
        //    CookieContainer cc = new CookieContainer();
        //    string postData = string.Format("key={0}", k);
        //    string json = HttpRequestHelper.Get(url, postData, ref cc);
        //    return JsonConvert.DeserializeObject<string>(json);
        //}

        /// <summary>
        /// 获取分销订单列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static GetChannelMOrderListResult GetChannelMOrderList(int count, int start, int cid)
        {
            string url = APISiteUrl + "api/Coupon/GetChannelMOrderList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("count={0}&start={1}&cid={2}", count, start, cid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<GetChannelMOrderListResult>(json);
        }
        /// <summary>
        /// 获取提现记录列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static GetChannelSettleListResult GetChannelMWithDrawList(int count, int start, int cid)
        {
            string url = APISiteUrl + "api/Coupon/GetChannelMWithDrawList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("count={0}&start={1}&cid={2}", count, start, cid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<GetChannelSettleListResult>(json);
        }
        /// <summary>
        /// 获取分销订单详情
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ChannelMOrderDetailEntity GetChannelMOrderDetail(int id, int cid)
        {
            string url = APISiteUrl + "api/Coupon/GetChannelMOrderDetail";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, string.Format("id={0}&cid={1}", id, cid), ref cc);
            return JsonConvert.DeserializeObject<ChannelMOrderDetailEntity>(json);
        }


        /// <summary>
        /// 获取 我的 展示页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static ChannelMineModel GetChannelMMine(int cid, int aid)
        {
            string url = APISiteUrl + "api/Coupon/GetChannelMMine";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("cid={0}&aid={1}", cid, aid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ChannelMineModel>(json);
        }
        /// <summary>
        /// 获取入账记录列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static GetChannelMOrderListResult GetChannelMInAccountList(int count, int start, int cid)
        {
            string url = APISiteUrl + "api/Coupon/GetChannelMInAccountList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("count={0}&start={1}&cid={2}", count, start, cid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<GetChannelMOrderListResult>(json);
        }

        /// <summary>
        /// 获取体现方式列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static GetChannelMBankAccountListResult GetChannelBankAccountList(int count, int start, int cid)
        {
            string url = APISiteUrl + "api/Coupon/GetChannelBankAccountList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("count={0}&start={1}&cid={2}", count, start, cid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<GetChannelMBankAccountListResult>(json);
        }
        /// <summary>
        /// 获取提现方式详细
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static HJD.HotelManagementCenter.Domain.PayBankAccountLibEntity GetChannelBankAccount(int cid, int type)
        {
            string url = APISiteUrl + "api/Coupon/GetChannelBankAccount";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("cid={0}&type={1}", cid, type);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HJD.HotelManagementCenter.Domain.PayBankAccountLibEntity>(json);
        }
        /// <summary>
        /// 新增提现方式
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static OperationResult SaveBankAccount(HJD.HotelManagementCenter.Domain.PayBankAccountLibEntity model)
        {
            OperationResult info = null;
            string url = APISiteUrl + "api/Coupon/SaveBankAccount";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, model, ref cc);
            info = JsonConvert.DeserializeObject<OperationResult>(json);
            return info;

        }

        /// <summary>
        /// 获取提现金额
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static OperationResult GetCanRequire(int cid)
        {
            string url = APISiteUrl + "api/Coupon/GetCanRequire";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("cid={0}", cid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<OperationResult>(json);
        }

        /// <summary>
        /// 获取提现结果
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static OperationResult WithDrawBankAccount(int id, int cid)
        {
            string url = APISiteUrl + "api/Coupon/WithDrawBankAccount";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("id={0}&cid={0}", id, cid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<OperationResult>(json);
        }

        public static ShopSearchItemEntity GetSearchRetailerProductItemList()
        {
            string url = APISiteUrl + "api/Coupon/GetSearchRetailerProductItemList";
            CookieContainer cc = new CookieContainer();
            //string postDataStr = string.Format();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            return JsonConvert.DeserializeObject<ShopSearchItemEntity>(json);
        }
        public static ShopSearchItemEntity GetSearchRetailerProductShopItemList()
        {
            string url = APISiteUrl + "api/Coupon/GetSearchRetailerProductShopItemList";
            CookieContainer cc = new CookieContainer();
            //string postDataStr = string.Format();
            string json = HttpRequestHelper.Get(url, "", ref cc);
            return JsonConvert.DeserializeObject<ShopSearchItemEntity>(json);
        }

        public static GetRetailProductListResult GetSearchRetailerProductList(SearchProductRequestEntity param)
        {
            string url = APISiteUrl + "api/Coupon/GetSearchRetailerProductList";
            CookieContainer cc = new CookieContainer();
            //string postDataStr = string.Format();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<GetRetailProductListResult>(json);
        }
        public static GetRetailProductListResult GetSearchRetailerShopProductList(SearchProductRequestEntity param)
        {
            string url = APISiteUrl + "api/Coupon/GetSearchRetailerShopProductList";
            CookieContainer cc = new CookieContainer();
            //string postDataStr = string.Format();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<GetRetailProductListResult>(json);
        }

        public static BaseResponse PresentCashCoupon(PresentCashCouponParam param)
        {
            string url = APISiteUrl + "api/Coupon/PresentCashCoupon";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<BaseResponse>(json);
        }

   
        public static List<BookDetailItem> GetBookDetailByBookDateId(int skuid, int bookDateId)
        {
            string url = APISiteUrl + "api/Coupon/GetBookDetailByBookDateId";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("skuid={0}&bookDateId={1}", skuid, bookDateId);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<BookDetailItem>>(json);
        }
        public static HJD.HotelManagementCenter.Domain.TemplateDataEntity GetSKUTempSource(int skuid, int postionIndex = 2)
        {
            string url = APISiteUrl + "api/Coupon/GetSKUTempSource";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("skuid={0}&postionIndex={1}", skuid, postionIndex);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<HJD.HotelManagementCenter.Domain.TemplateDataEntity>(json);
        }

        public static List<BookUserDateInfoEntity> GetBookedUserInfoByExchangid(int exchangeid)
        {
            string url = APISiteUrl + "api/Coupon/GetBookedUserInfoByExchangid";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("exchangeid={0}", exchangeid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<BookUserDateInfoEntity>>(json);
        }

        public static List<BookUserDateInfoEntity> GetBookedUserInfoByExchangIds(string exchangeIds)
        {
            string url = APISiteUrl + "api/Coupon/GetBookedUserInfoByExchangIds";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("exchangeIds={0}", exchangeIds);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<BookUserDateInfoEntity>>(json);
        }

        public static List<BookUserDateInfoEntity> GetPostBookedUserInfoByExchangIds(ExchangeCouponsParam param)
        {
            string url = APISiteUrl + "api/Coupon/GetPostBookedUserInfoByExchangIds";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<List<BookUserDateInfoEntity>>(json);
        }

        public static BaseResult SubmitSetLike(string openid, int groupId, int activityID, long userid = 0)
        {
            string url = APISiteUrl + "api/Coupon/SubmitSetLike";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("openid={0}&groupId={1}&activityID={2}&userid={3}", openid, groupId, activityID, userid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<BaseResult>(json);
        }
        public static CommentTextAndUrl VIPDiscountDescription(decimal orderTotalPrice, decimal orderVipTotalPrice)
        {
            string url = APISiteUrl + "api/Coupon/VIPDiscountDescription";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("orderTotalPrice={0}&orderTotalPrice={1}", orderTotalPrice, orderVipTotalPrice);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CommentTextAndUrl>(json);
        }

        public static CommentTextAndUrl BecomeVIPDiscountDescription(decimal orderTotalPrice, decimal orderVipTotalPrice)
        {
            string url = APISiteUrl + "api/Coupon/BecomeVIPDiscountDescription";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("orderTotalPrice={0}&orderTotalPrice={1}", orderTotalPrice, orderVipTotalPrice);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CommentTextAndUrl>(json);
        }

        public static List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserId(int userId, UserCouponState state)
        {
            string url = APISiteUrl + "api/Coupon/GetUserCouponInfoListByUserId";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}&state={1}", userId, state);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<UserCouponItemInfoEntity>>(json);
        }
        public static List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserIdAndType(int userId, UserCouponState state, UserCouponType type)
        {
            string url = APISiteUrl + "api/Coupon/GetUserCouponInfoListByUserIdAndType";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}&state={1}&type={2}", userId, state, type);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<UserCouponItemInfoEntity>>(json);
        }

        public static int GetUserCouponInfoCountByUserIdAndType(int userId, UserCouponState state, UserCouponType type)
        {
            string url = APISiteUrl + "api/Coupon/GetUserCouponInfoCountByUserIdAndType";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}&state={1}&type={2}", userId, state, type);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public static int GetUserCouponInfoCountByUserId(int userId, UserCouponState state)
        {
            string url = APISiteUrl + "api/Coupon/GetUserCouponInfoCountByUserId";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}&state={1}", userId, state);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }


        public static int GetUserCouponLogByCouponItemID(int idx)
        {
            string url = APISiteUrl + "api/Coupon/GetUserCouponLogByCouponItemID";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("idx={0}", idx);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        #region   使用代金券

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static List<UserCouponItemInfoEntity> GetCanUseVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req)
        {
            string url = APISiteUrl + "api/Coupon/GetCanUseVoucherInfoListForOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, req, ref cc);
            return JsonConvert.DeserializeObject<List<UserCouponItemInfoEntity>>(json);
        }

        //获取指定产品类型指定金额下，所有不可用的券（订单确认页使用，暂时可以不分页）
        public static List<UserCouponItemInfoEntity> GetCannotUseVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req)
        {
            string url = APISiteUrl + "api/Coupon/GetCannotUseVoucherInfoListForOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, req, ref cc);
            return JsonConvert.DeserializeObject<List<UserCouponItemInfoEntity>>(json);
        }

        // 检测指定产品类型指定金额下，当前券ID是否满足条件（比如用户选好了一个券后，手动变更数量时，需要实时检测）
        public static CheckSelectedVoucherInfoForOrderViewModel CheckSelectedVoucherInfoForOrder(OrderVoucherRequestParams req)
        {
            string url = APISiteUrl + "api/Coupon/CheckSelectedVoucherInfoForOrder";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, req, ref cc);
            return JsonConvert.DeserializeObject<CheckSelectedVoucherInfoForOrderViewModel>(json);
        }

        #endregion


        public static RedRecordEntity GetRedRecordByGuidAndPhone(string key, string phoneNum, string openid)
        {
            string url = APISiteUrl + "api/Coupon/GetRedRecordByGuidAndPhone";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("key={0}&phoneNum={1}&openid={2}", key, phoneNum, openid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RedRecordEntity>(json);
        }

        public static RedActivityInfoEntity GetRedRecordByKey(string key, int count = 20, int start = 0)
        {
            string url = APISiteUrl + "api/Coupon/GetRedRecordByKey";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("key={0}&count={1}&start={2}", key, count, start);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RedActivityInfoEntity>(json);
        }

        public static RedOrderInfoEntity GetOrderRed(long userid, long orderId, HJDAPI.Common.Helpers.Enums.OrdersType OrderType, decimal totalPrice)
        {
            string url = APISiteUrl + "api/Coupon/GetOrderRed";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userid={0}&orderId={1}&OrderType={2}&totalPrice={3}", userid, orderId, OrderType, totalPrice);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RedOrderInfoEntity>(json);
        }
        public static RedRecordEntity GetUserRedRecord(int id)
        {
            string url = APISiteUrl + "api/Coupon/GetUserRedRecord";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("id={0}", id);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RedRecordEntity>(json);
        }

        public static OperationResult ReSendSMS(int couponid, int payid = 0, int thirdtype = 0)
        {
            string url = APISiteUrl + "api/Coupon/ReSendSMS";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("CouponID={0}&PayID={1}&ThirdType={2}", couponid, payid, thirdtype);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<OperationResult>(json);
        }

        public static string UpdateThirdPartyRefundCoupon(int couponid, int thirdstate, int refundstate)
        {
            string url = APISiteUrl + "api/Coupon/UpdateThirdPartyRefundCoupon";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("couponid={0}&thirdstate={1}&refundstate={2}", couponid, thirdstate, refundstate);
            return HttpRequestHelper.Get(url, postDataStr, ref cc).ToString();
        }

        public static int UpdateOperationState(ExchangeCouponEntity param)
        {
            string url = APISiteUrl + "api/Coupon/UpdateOperationState";
            CookieContainer cc = new CookieContainer();
            string res = HttpRequestHelper.PostJson(url, param, ref cc);
            return int.Parse(res);
        }
        public static int UpdateOperationState(int couponid, long updator, int operationstate)
        {
            string url = APISiteUrl + "api/Coupon/UpdateOperationState";
            CookieContainer cc = new CookieContainer();
            string param = string.Format("couponid={0}&updator={1}&operationstate={2}", couponid, updator, operationstate);
            return int.Parse(HttpRequestHelper.Get(url, param, ref cc).ToString());
        }

        /// <summary>
        /// 第三方门票下单失败, 更新券待处理状态 ， 如果直接退的，那么操作退券
        /// </summary>
        /// <param name="couponID"></param>
        /// <returns></returns>
        public static BaseResponse ThirdPartyCouponOrderFailed(int couponID, long updator)
        {
            string url = APISiteUrl + "api/Coupon/ThirdPartyCouponOrderFailed";
            CookieContainer cc = new CookieContainer();
            string param = string.Format("couponID={0}&updator={1}", couponID, updator);
            return JsonConvert.DeserializeObject<BaseResponse>(HttpRequestHelper.Get(url, param, ref cc));
        }

        public static int UpdateOperationRemark(int CouponID, string Remark)
        {
            string url = APISiteUrl + "api/Coupon/UpdateOperationRemark";
            CookieContainer cc = new CookieContainer();

            ExchangeCouponRequestParam ec = new ExchangeCouponRequestParam();
            ec.ID = CouponID;
            ec.OperationRemark = Remark;

            string res = HttpRequestHelper.PostJson(url, ec, ref cc);
            return int.Parse(res);
        }

        public static string CouponChangeUsed(int couponid)
        {
            string url = APISiteUrl + "api/Coupon/CouponChangeUsed";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("couponid={0}", couponid);
            return HttpRequestHelper.Get(url, postDataStr, ref cc).ToString();
        }

        public static SPUEntity GetSPUBySKUID(int skuid)
        {
            string url = APISiteUrl + "api/Coupon/GetSPUBySKUID";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("skuid={0}", skuid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<SPUEntity>(json);
        }

        public static CouponActivityEntity GetCouponActivityBySKUID(int skuid)
        {
            string url = APISiteUrl + "api/Coupon/GetCouponActivityBySKUID";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("skuid={0}", skuid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CouponActivityEntity>(json);
        }

        public static int InsertExchangeCoupon(ExchangeCouponEntity req)
        {
            string url = APISiteUrl + "api/Coupon/InsertExchangeCoupon";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, req, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }
        public static int UpdateExchangeCoupon(ExchangeCouponEntity req)
        {
            string url = APISiteUrl + "api/Coupon/UpdateExchangeCoupon";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, req, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public static RetailTeamDataEntity GetTeamRetailProductByCid(long cid)
        {
            string url = APISiteUrl + "api/Coupon/GetTeamRetailProductByCid";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("cid={0}", cid);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<RetailTeamDataEntity>(json);
        }
        /// <summary>
        /// 更新过期不退款券数据
        /// </summary>
        /// <returns></returns>
        public static bool UpdateExchangeCouponExpiredNotRefund()
        {
            return CouponAdapter.UpdateExchangeCouponExpiredNotRefund();
        }

        public static int ReceiveCouponDefine(long userId, string couponDefineIds)
        {
            string url = APISiteUrl + "api/Coupon/ReceiveCouponDefine";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}&couponDefineIds={1}", userId, couponDefineIds);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }
        public static List<UserCouponDefineEntity> GetCouponUserCouponList(string userId)
        {
            string url = APISiteUrl + "api/Coupon/GetCouponUserCouponList";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}", userId);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<UserCouponDefineEntity>>(json);
        }
        public static int GetUsedCoupon(long userId, int conponDefineID)
        {
            string url = APISiteUrl + "api/Coupon/GetUsedCoupon";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("userId={0}&conponDefineID={1}", userId, conponDefineID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

             /// <summary>
        /// 为周期卡生成新卡
        /// </summary>
        /// <param name="couponID">生成新周期卡的依据卡ID</param>
        /// <returns>新生成的券ID</returns> 

        public static int CreateNewCouponForCycleCoupon(int couponID)
        {
            string url = APISiteUrl + "api/Coupon/CreateNewCouponForCycleCoupon";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("couponID={0} ", couponID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }


        public CouponOrderResult SubmitStepGroupOrder(int depositSKUID, int tailSKUID, long userId, decimal price, long couponOrderID)
        {
            string url = APISiteUrl + "api/Coupon/SubmitStepGroupOrder";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("depositSKUID={0}&tailSKUID={1}&userId={2}&price={3}&couponOrderID={4}", depositSKUID, tailSKUID, userId, price, couponOrderID);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CouponOrderResult>(json);
        }

        public static void WxFWHRetailMessage(long cid, long orderId, string pageTitle, string orderTime, string price)
        {
            string url = APISiteUrl + "api/Coupon/WxFWHRetailMessage";
            CookieContainer cc = new CookieContainer();
            string postDataStr = string.Format("cid={0}&orderId={1}&pageTitle={2}&orderTime={3}&price={4}", cid, orderId, pageTitle, orderTime, price);
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
        }
    }
}