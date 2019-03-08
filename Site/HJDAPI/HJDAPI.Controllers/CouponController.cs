using HJD.AccountServices.Entity;
using HJD.CouponService.Contracts;
using HJD.CouponService.Contracts.Entity;
using HJD.Framework.Interface;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Common.Security;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJDAPI.Common.DistributedLock;
using Newtonsoft.Json;
using HJD.AccountServices.Contracts;
using HJD.Framework.WCF;
using HJD.DestServices.Contract;
using System.Diagnostics;
using ProductService.Contracts.Entity;
using System.Text.RegularExpressions;
using HJDAPI.Controllers.Biz;
using HJD.HotelManagementCenter.Domain.Settlement;
using HJD.HotelManagementCenter.Domain.ThirdParty.Ctrip;
using HJD.HotelManagementCenter.Domain.ThirdParty;
using HJD.WeixinService.Contract;
using HJDAPI.Models.Coupon;
using HJDAPI.Controllers.Cache;
using HJD.WeixinServices.Contracts;
using AutoMapper;
using HJDAPI.Models.Tags;

namespace HJDAPI.Controllers
{
    public class CouponController : BaseApiController
    {


        public static IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");
        public static IDestService destService = ServiceProxyFactory.Create<IDestService>("BasicHttpBinding_IDestService");
        public static IHotelService hotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
        public static ICouponService couponService = ServiceProxyFactory.Create<ICouponService>("ICouponService");
        public static HJD.HotelManagementCenter.IServices.IThirdPartyService thirdpartyService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.IThirdPartyService>("IThirdPartyService");

        static string GroupSuccessMsg600 = @"恭喜您拼团成功！券号为{0}。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;
        static string GroupSuccessMsg200 = @"恭喜您拼团成功！您以超值拼团价买到：{0}酒店的{1}房券1张，券号为{2}。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;
        static string GroupFailMsg = @"抱歉，因团购人数未达标，您的拼团{0}失败！退款¥{1}将在1-5个工作日原路退至您的支付账户中，请注意查收。" + Configs.SMSSuffix;

        static string QRCodeMsg = "您已成功购买{0}产品， 请在使用当日用微信在现场扫二维码入场。 如需服务，请在“周末酒店服务号”或“周末酒店”APP的“顾问”频道留言，或致电客服4000-021-702。";
        static string GroupSuccessQRCodeMsg = "恭喜您拼团成功！您已成功购买{0}产品， 请在使用当日用微信在现场扫二维码入场。 如需服务，请在“周末酒店服务号”或“周末酒店”APP的“顾问”频道留言，或致电客服4000-021-702。";


        #region 现金券接口

        //        获取指定产品类型指定金额下，所有可用的券（订单确认页使用，暂时可以不分页）




        //获取指定产品类型指定金额下，所有不可用的券（订单确认页使用，暂时可以不分页）
        //获取指定产品类型指定金额下，默认最优惠的券（订单确认页需要默认选择一个券）


        #endregion

        #region   活动


        /// <summary>
        /// 老VIP 送券活动
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public bool SendVIPCouponActitity(long userid)
        {
            foreach (string v in CommAdapter.GetCommDicKeyValueWidthCache(12000, 1))
            {
                bool s = couponService.SendOneUserCouponItemOnlyOneTime(userid, int.Parse(v), 0);
                if (s == false) //如果已送过，就不需要
                    break;
            }
            return true;
        }

        /// <summary>
        /// 获取活动券送列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<UserCouponDefineEntity> GetVIPCouponGiftUserCouponList()
        {
            return CacheAdapter.LocalCache10Min.GetData<List<UserCouponDefineEntity>>("GetVIPCouponGiftUserCouponList", () =>
            {
                List<UserCouponDefineEntity> list = new List<UserCouponDefineEntity>();
                foreach (string v in CommAdapter.GetCommDicKeyValueWidthCache(12000, 1))
                {
                    list.Add(couponService.GetUserCouponDefineByID(int.Parse(v)));
                }
                return list;
            });
        }

        /// <summary>
        /// 获取活动券送列表
        /// </summary>
        /// <param name="type">活动类型</param>
        /// <returns></returns>
        [HttpGet]
        public List<UserCouponDefineEntity> GetActivityRedCashCouponUserCouponList(HJDAPI.Common.Helpers.Enums.ActivityRedCashCouponType type)
        {
            int cmmDictCategory = 0;
            int cmmDictKey = 0;

            switch (type)
            {
                case Enums.ActivityRedCashCouponType.redcashcouponlistnewuser:
                    cmmDictCategory = 12000;
                    cmmDictKey = 2;
                    break;
            }

            return CacheAdapter.LocalCache10Min.GetData<List<UserCouponDefineEntity>>(string.Format("GetActivityRedCashCouponUserCouponList{0}{1}", cmmDictCategory, cmmDictKey), () =>
            {
                List<UserCouponDefineEntity> list = new List<UserCouponDefineEntity>();
                foreach (string v in CommAdapter.GetCommDicKeyValueWidthCache(cmmDictCategory, cmmDictKey))
                {
                    list.Add(couponService.GetUserCouponDefineByID(int.Parse(v)));
                }
                return list;
            });
        }

        /// <summary>
        /// 根据红包券id获取红包集合
        /// </summary>
        /// <param name="cashCouponIds">红包id，多个用逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        public List<UserCouponDefineEntity> GetRedCashCouponList(string cashCouponIds)
        {
            List<UserCouponDefineEntity> list = new List<UserCouponDefineEntity>();
            foreach (string id in cashCouponIds.Split(','))
            {
                list.Add(couponService.GetUserCouponDefineByID(int.Parse(id)));
            }
            return list;
            //return new List<UserCouponDefineEntity>();
        }
        [HttpGet]
        public string Add2RefundCouponTest(string couponids = "")
        {
            string errorString = "";
            //string coupon = "455262,455261,455260,455269,455270,455250,455257,455258,455256,455255,455263,455259,455264,455268,455267,455266,455265,455252,455251";


            //string coupon = "455541,455385,455542,455459,455380,455537,455379,455538,455308,455384,455392,455391,455390,455389,455393,455396,455404,455403,455370,455376,455371,455375,455386,455383,455382,455381,455388,455387,455399,455400,455394,455401,455395,455406,455405,455402,455369,455374,455378,455377,455372,455373";
            if (!string.IsNullOrEmpty(couponids))
            {
                string coupon = couponids;// "345112,345115,345040,345033,345035,345034,345038,345111,345110,345106,345107,345039,345057,345116,345117,345114,345113,345036,345108,345109,345037,345058,345059";
                foreach (var item in coupon.Split(','))
                {
                    int couponID = Convert.ToInt32(item);
                    BaseResponse result = Add2RefundCoupon(couponID, 4687819, 4687819, "15317569036", "duoduo让退--给分销商充券，分销商不要了");
                    if (result.Success != 0)
                    {
                        errorString = errorString + item;
                    }
                }
            }

            return errorString;
        }

        //[HttpGet]
        //public string BatchAdd2RefundCouponTest(string couponids = "",long userId = 0,string remark="")
        //{
        //    string errorString = "";
        //    //string coupon = "455262,455261,455260,455269,455270,455250,455257,455258,455256,455255,455263,455259,455264,455268,455267,455266,455265,455252,455251";
        //    //string coupon = "455541,455385,455542,455459,455380,455537,455379,455538,455308,455384,455392,455391,455390,455389,455393,455396,455404,455403,455370,455376,455371,455375,455386,455383,455382,455381,455388,455387,455399,455400,455394,455401,455395,455406,455405,455402,455369,455374,455378,455377,455372,455373";
        //    if (!string.IsNullOrEmpty(couponids))
        //    {
        //        string coupon = couponids;// "345112,345115,345040,345033,345035,345034,345038,345111,345110,345106,345107,345039,345057,345116,345117,345114,345113,345036,345108,345109,345037,345058,345059";
        //        foreach (var item in coupon.Split(','))
        //        {
        //            int couponID = Convert.ToInt32(item);
        //            BaseResponse result = Add2RefundCoupon(couponID, userId, userId, "多多让退款", remark);
        //            if (result.Success != 0)
        //            {
        //                errorString = errorString + ","+ item;
        //            }
        //        }
        //    }
        //    return errorString;
        //}


        /// <summary>
        /// 批量退消费券
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public KeyValueEntity BatchAdd2RefundCoupon(BatchAdd2RefundCouponEntity param)
        {
            KeyValueEntity result = new KeyValueEntity() { Key = "成功" };
            string resultString = "";
            int totalCount = param.ExchangeCouponIDs.Count;
            int successCount = 0;
            int errorCount = 0;
            Log.WriteLog("param.UserID：" + param.UserID);
            foreach (var item in param.ExchangeCouponIDs)
            {
                int couponID = Convert.ToInt32(item);
                BaseResponse br = Add2RefundCoupon(couponID, param.UserID, param.UserID, "", param.Remark);
                if (br.Success != 0)
                {
                    resultString = resultString + "," + item + "--" + br.Message;
                    errorCount++;
                }
                else
                {
                    successCount++;
                }
            }
            result.Value = string.Format("共{0}条；退款成功{1}条；失败{2}条。{3}", totalCount, successCount, errorCount, resultString);
            return result;
        }
        /// <summary>
        /// 获取活动券送列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<UserCouponDefineEntity> GetCouponUserCouponListBySKUAlbumIds(string albumIds, long userId)
        {
            List<UserCouponDefineEntity> list = new List<UserCouponDefineEntity>();

            List<ProductAlbumSKUEntity> productAlbumSkuList = ProductAdapter.GetProductAlbumSkuByAlbumIds(albumIds);
            string skuids = string.Join(",", productAlbumSkuList.Select(_ => _.SKUID).ToList());
            list = CouponAdapter.GetCouponDefineByIntVals(skuids, 1, userId);
            //string couponCondationIs = string.Join(",", userCouponCondationList.Select(_ => _.CouponDefineID).ToList());
            //list = CouponAdapter.GetUserCouponDefineListByIds(couponCondationIs, userId);
            return list;
        }

        /// <summary>
        /// 判断用户是否领取过活动券
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public bool IsUserHasGetNewVIPGift(long userid)
        {
            int couponDefineID = int.Parse(CommAdapter.GetCommDicKeyValueWidthCache(12000, 1).First());
            return couponService.GetUserCouponItemByUserIdAndCouponDefineId(userid, couponDefineID) > 0;
        }


        #endregion


        #region 使用红包

        /// <summary>
        /// 获取用户可用现金券金额
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public GetUserCanUseCashCouponAmountResponse GetUserCanUseCashCouponAmount(GetUserCanUseCashCouponAmountRequestParams p)
        {
            GetUserCanUseCashCouponAmountResponse r = new GetUserCanUseCashCouponAmountResponse();

            if (p.userID == 0)
            {
                r.UserCanUseCashCouponAmount = 0;
            }
            else
            {
                r.UserCanUseCashCouponAmount = CouponAdapter.GetUserCanUseCashCouponAmount(p.userID);

                if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
                {
                    r.UserCanUseCashCouponAmount = CouponAdapter.GetUserCanUseCashCouponAmount(p.userID);
                    if (p.packagePayType == HotelServiceEnums.PackagePayType.FrontPay)
                    {
                        r.UserCanUseHousingFundAmount = 0;
                    }
                    else
                    {
                        r.UserCanUseHousingFundAmount = (int)FundAdapter.GetUserFundInfo(p.userID).TotalFund;
                    }
                }
                else
                {
                    r.SignError();
                }
            }

            return r;
        }

        [HttpGet]
        public int GetUserCanUseCashCouponAmountByUid(long userId)
        {
            return CouponAdapter.GetUserCanUseCashCouponAmount(userId);
        }

        [HttpPost]
        public ReturnCouponResult SetOrderCoupon(CouponModelParam param)
        {
            return CouponAdapter.SetOrderCoupon(param.userId, param.sourceId, param.couponAmount, param.isBudget);
        }

        /// <summary>
        /// 获得现金列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<OriginCoupon> GetCashCouponList(long userId)
        {
            List<OriginCoupon> list = CouponAdapter.GetCashCouponList(userId);
            return list != null ? list : new List<OriginCoupon>();
        }

        [HttpGet]
        public WalletResult GetWalletList(long userId)
        {
            //List<OriginCoupon> cashList = CouponAdapter.GetCashCouponList(userId);
            ////现金钱包列表必须是已领取的红包
            //List<OriginCoupon> cashList2 = cashList != null ? (from p in cashList where p.State == 1 select p).ToList<OriginCoupon>():new List<OriginCoupon>();
            List<AcquiredCoupon> couponList = CouponAdapter.GetAcquiredCouponList2(userId, null);
            List<AcquiredCoupon> couponList2 = couponList != null ? (from p in couponList where !string.IsNullOrEmpty(p.PhoneNo) select p).ToList<AcquiredCoupon>() : new List<AcquiredCoupon>();

            int sumCanUse = (from p in couponList2 where p.RestMoney > 0 select (int)p.RestMoney).Sum<int>(i => i);
            int sumUsed = (from p in couponList2 where (p.TotalMoney - p.RestMoney - p.ExpiredMoney) > 0 select (int)(p.TotalMoney - p.RestMoney - p.ExpiredMoney)).Sum<int>(i => i);

            return new WalletResult()
            {
                UsedCouponAmount = sumUsed,
                CanUseCouponAmount = sumCanUse,
                couponList = couponList2
            };
        }
        #endregion

        #region 生成红包
        [HttpPost]
        public OriginCouponResult GenerateOriginCoupon(CouponModelParam param)
        {
            int totalMoney = 0;
            int cashMoney = 0;
            string moneyStr = "";
            return CouponAdapter.GenerateOriginCoupon(param.userId, param.typeId, param.sourceId, totalMoney, cashMoney, moneyStr);
        }
        [HttpPost]
        public AcquiredCoupon GetAcquiredCoupon(CouponModelParam param)
        {
            if (param.userId == 0)
            {
                User_Info info = AccountAdapter.GetOrRegistPhoneUser(param.phoneNo, 0);
                param.userId = info.UserId;
            }
            return CouponAdapter.GetAcquiredCoupon(param.userId, param.guid, param.phoneNo);
        }
        [HttpGet]
        public List<AcquiredCoupon> GetCurrentAcquiredCouponList(string guid)
        {
            List<AcquiredCoupon> couponList = CouponAdapter.GetAcquiredCouponList(guid);
            List<AcquiredCoupon> finalList = null;
            if (couponList != null && couponList.Count != 0)
            {
                finalList = couponList.FindAll(i => !string.IsNullOrEmpty(i.PhoneNo));
                if (finalList != null)
                {
                    finalList.ForEach(i => i.PhoneNo = DescriptionHelper.maskPhoneNum(i.PhoneNo));
                }
            }
            return finalList != null ? finalList : new List<AcquiredCoupon>();
        }
        [HttpGet]
        public bool IsAllAcquired(string guid)
        {
            List<AcquiredCoupon> couponList = CouponAdapter.GetAcquiredCouponList(guid);
            if (couponList != null && couponList.Count != 0)
            {
                return couponList.All<AcquiredCoupon>(i => !string.IsNullOrEmpty(i.PhoneNo));
            }
            else
            {
                return false;
            }
        }
        [HttpGet]
        public AcquiredCoupon GetAcquiredCouponById(long id)
        {
            return CouponAdapter.GetAcquiredCouponById(id);
        }
        /// <summary>
        /// 订单分享 现金返现 更新状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public OriginCouponResult UpdateCashCoupon(CouponModelParam param)
        {
            return CouponAdapter.UpdateCashCoupon(param.id, param.guid, param.state);
        }
        /// <summary>
        /// 订单分享 送现金券 更新状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public OriginCouponResult UpdateCashCoupon20(CouponModelParam param)
        {
            return CouponAdapter.UpdateCashCoupon20(param.id, param.guid, param.state);
        }
        #endregion

        #region 获取微信js-SDK 使用注册信息

        /// <summary>
        /// 周末酒店订阅号
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        public WeixinConfig GetWeixinConfig(WeixinConfig config2)
        {
            return WeiXinAdapter.GetWeixinApiConfig(config2, 1);
        }

        /// <summary>
        /// 尚旅游订阅号
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        public WeixinConfig GetWeixinConfigSly(WeixinConfig config2)
        {
            return WeiXinAdapter.GetWeixinApiConfig(config2, 3);
        }

        /// <summary>
        /// 尚旅游成都订阅号
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        public WeixinConfig GetWeixinConfigSlycd(WeixinConfig config2)
        {
            //临时处理：尚旅游成都也适用zmjiudian的weixin jsapi，因为尚旅游成都订阅号免费住活动用 2017.05.16 haoy
            return WeiXinAdapter.GetWeixinApiConfig(config2, 1);

            //return WeiXinAdapter.GetWeixinApiConfig(config2, 4);
        }

        /// <summary>
        /// 美味至尚订阅号
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        public WeixinConfig GetWeixinConfigMwzs(WeixinConfig config2)
        {
            return WeiXinAdapter.GetWeixinApiConfig(config2, 5);
        }

        #endregion

        [HttpPost]
        public string GetOriginGUID(CouponModelParam param)
        {
            return CouponAdapter.GetOriginGUID(param.sourceId, param.typeId);
        }

        [HttpGet]
        public List<UserCouponDefineEntity> GetNewVIPGiftUserCouponList()
        {
            return CouponAdapter.GetNewVIPGiftUserCouponList();
        }

        [HttpGet]
        public bool HasParticipateActivity(string guid, string phoneNum)
        {
            List<AcquiredCoupon> couponList = CouponAdapter.GetAcquiredCouponList(guid);
            if (couponList != null && couponList.Count != 0)
            {
                return couponList.Exists(i => !string.IsNullOrEmpty(i.PhoneNo) && i.PhoneNo.Equals(phoneNum));
            }
            else
            {
                return false;
            }
        }


        [HttpGet]
        public List<AcquiredCoupon> GetAcquiredCouponList(string guid)
        {
            List<AcquiredCoupon> couponList = CouponAdapter.GetAcquiredCouponList(guid);
            return couponList;
        }

        [HttpGet]
        public OriginCoupon GetCashCoupon(long id, string guid)
        {
            return CouponAdapter.GetCashCoupon(id, guid);
        }

        [HttpGet]
        public InspectorRewardResult GetInspectorRewardResult(long userid)
        {
            List<InspectorRewardItem> items = CouponAdapter.GetInspectorRewardItemList(userid);
            InspectorRewardResult result = new InspectorRewardResult();
            if (items != null && items.Count != 0)
            {
                result.Items = items;
                result.TotalCommission = items.Sum<InspectorRewardItem>(i => i.Commission);
            }
            return result;
        }

        [HttpPost]
        public OriginCouponResult GetShareCommentCoupon(ShareCommentCouponParam param)
        {
            return CouponAdapter.GenerateOriginCouponEx(param.userId, param.sourceId, CouponActivityCode.sharecomment, param.phoneNo);
        }

        [HttpGet]
        public CouponResult GetPersonalCoupon(long userId, long timeStamp, int sourceID, string requestType, string sign)
        {
            try
            {
                List<AcquiredCoupon> couponlist = CouponAdapter.couponSvc.GetAcquireCouponRecordByUserID(userId);
                if (couponlist != null && couponlist.Count != 0)
                {
                    BindObjectName4CouponList(couponlist);

                    List<AcquiredCoupon> unExpiredList = couponlist.FindAll(i => i.RestMoney > 0 && i.ExpiredTime >= DateTime.Now);
                    int canUseCoupon = 0;
                    if (unExpiredList != null && unExpiredList.Count != 0)
                    {
                        canUseCoupon = (int)unExpiredList.Sum(i => i.RestMoney);
                    }

                    List<UseCouponRecordEntity> recordList = CouponAdapter.couponSvc.GetUseCouponRecordByUserID(userId);
                    foreach (UseCouponRecordEntity temp in recordList)
                    {
                        temp.TypeName = "支付抵扣";
                    }
                    return new CouponResult()
                    {
                        CanUseCoupon = canUseCoupon,
                        couponList = couponlist,
                        couponRecordList = recordList
                    };
                }
                else
                {
                    return new CouponResult()
                    {
                        CanUseCoupon = 0,
                        couponList = new List<AcquiredCoupon>(),
                        couponRecordList = new List<UseCouponRecordEntity>()
                    };
                }
            }
            catch (Exception err)
            {
                Log.WriteLog("GetPersonalCoupon:" + err.Message);
                return new CouponResult();
            }
        }

        /// <summary>
        /// key and value
        /// </summary>
        /// <param name="couponList"></param>
        /// <returns></returns>
        private void BindObjectName4CouponList(List<AcquiredCoupon> couponList)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (couponList != null && couponList.Count != 0)
            {
                foreach (var c in couponList.GroupBy(i => i.TypeCode))
                {
                    string typeCode = c.Select(i => i.TypeCode).First();
                    if (string.IsNullOrEmpty(typeCode))
                    {
                        continue;
                    }
                    List<long> sourceIds = c.Select(i => i.SourceID).Distinct().ToList();
                    List<SourceIDAndObjectNameEntity> commentObjectNameList = HotelController.HotelService.GetObjectNamesByTypeCode(sourceIds, typeCode, 0);

                    foreach (SourceIDAndObjectNameEntity temp in commentObjectNameList)
                    {
                        string key = typeCode + temp.SourceID;
                        dic.Add(key, temp.ObjectName);
                    }
                }
                foreach (AcquiredCoupon ac in couponList)
                {
                    string key = ac.TypeCode + ac.SourceID;
                    if (dic.ContainsKey(key))
                    {
                        ac.ObjectName = dic[key];
                    }
                }
            }
            //return dic;
        }

        //private Dictionary<string, List<SourceIDAndObjectNameEntity>> ReturnCodeAndSourceIDAndOnjectNameListDic(List<UseCouponRecordEntity> couponList)
        //{

        //}

        [HttpGet]
        public PointResult GetPersonalPoint(long userId, long timeStamp, int sourceID, string requestType, string sign)
        {
            return PointsAdapter.GetPersonalPoint(userId);
        }

        [HttpPost]
        public BaseResponse ConsumeUserPoints(ConsumeUserPointsParam param)
        {
            BaseResponse r = new BaseResponse();
            Signature.CheckSignature(r, param);
            if (r.IsSuccess)
            {
                int result = PointsAdapter.ConsumeUserPoints(param);
                if (result != 0)
                {
                    r.Success = BaseResponse.ResponseSuccessState.Points_ConsumFailed;//
                    r.Message = "扣积分失败";
                }
            }

            return r;
        }



        [HttpPost]
        public BaseResponse PresentCashCoupon(PresentCashCouponParam param)
        {
            BaseResponse r = new BaseResponse();
            Signature.CheckSignature(r, param);
            if (r.IsSuccess)
            {
                OriginCouponResult result = CouponAdapter.GenerateOriginCoupon2(param.userID, param.typeID, param.phoneNo);
                if (result.Success != 0)
                {
                    r.Success = BaseResponse.ResponseSuccessState.CashCoupon_PresentFailed;//
                    r.Message = "赠送现金券失败";
                }
            }

            return r;
        }

        /// <summary>
        /// 根据活动类型 指定积分数量 给指定用户发积分
        /// </summary>
        /// <param name="pe"></param>
        /// <returns></returns>
        [HttpPost]
        public OrderCancelResult GenPoint(PointsEntity pe)
        {
            if (pe.UserID == 0 && !string.IsNullOrEmpty(pe.PhoneNo))
            {
                pe.UserID = AccountAdapter.GetOrRegistPhoneUser(pe.PhoneNo, 0).UserId;
            }

            if (pe == null || pe.UserID == 0 || pe.TypeID == 0 || pe.BusinessID == 0)
            {
                return new OrderCancelResult() { success = 1, Message = "参数不完整，插入积分失败" };
            }

            HotelAdapter.HotelService.InsertOrUpdatePoints(pe);
            return new OrderCancelResult() { success = 0, Message = "插入积分成功" };
        }

        #region 房券活动 一、获取活动页面内容;二、支付 形成券单

        /// <summary>
        /// 获得秒杀活动页面内容 Model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public CouponActivityDetailModel GetCouponActivityDetail(int activityID)
        {
            CouponActivityDetailModel model = CouponAdapter.GetCouponActivityDetail(activityID, false);//10min缓存
            var result = ProductCache.CouponSellInfoMem(activityID);//1min缓存
            if (result != null)
            {
                model.activity.SellNum = result.SellNum;//更新已卖出量（算上了后台设置的限制量）
            }

            return model;
        }


        /// <summary>
        /// 为周期卡生成新卡
        /// </summary>
        /// <param name="couponID">生成新周期卡的依据卡ID</param>
        /// <returns>新生成的券ID</returns> 
        [HttpGet]
        public int CreateNewCouponForCycleCoupon(int couponID)
        {
            return CouponAdapter.CreateNewCouponForCycleCoupon(couponID);
        }


        /// <summary>
        /// 通过SKUID获取SKU信息以及相关券活动信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SKUCouponActivityDetailModel GetSKUCouponActivityDetail(int SKUID, long userid = 0, long couponOrderId = 0)
        {
            SKUCouponActivityDetailModel model = CouponAdapter.GetSKUCouponActivityDetail(SKUID, true);

            if (model != null && model.activity != null)
            {

                #region 插入浏览记录
                if (userid > 0)
                {
                    int terminalType = _ContextBasicInfo.IsWeb ? 1 : _ContextBasicInfo.IsIOS ? 2 : _ContextBasicInfo.IsAndroid ? 3 : 4;
                    string clientIP = terminalType == 2 || terminalType == 3 ? HttpRequestHelper.GetClientIp(this.ControllerContext.Request) : "";

                    HotelAdapter.InsertBrowsingRecord(new BrowsingRecordEntity()
                    {
                        BusinessID = SKUID,
                        BusinessType = (int)HJDAPI.Controllers.EnumHelper.BrowsingRecordType.sku,
                        Visitor = userid,
                        TerminalType = terminalType,
                        ClientIP = clientIP,
                        SessionID = "",
                        OpenID = "",
                        AppVer = _ContextBasicInfo.AppVer
                    });
                }
                #endregion


                if (model.activity != null && model.activity.Notice != null & model.activity.Notice != "")
                {
                    model.activity.NoticeList = model.activity.Notice.Split('_').ToList();
                }
                if (model.activity != null && model.activity.PackageInfo != null & model.activity.PackageInfo != "")
                {
                    model.activity.PackageInfoList = model.activity.PackageInfo.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
                }
                //var result = CouponAdapter.CouponSellInfoMem(model.activity.ID);//1min缓存
                //if (result != null)
                //{
                //    model.activity.SellNum = result.SellNum;//更新已卖出量（算上了后台设置的限制量）
                //}
                //过滤SKUList
                try
                {
                    if (model.SKUInfo != null && model.SKUInfo.SKUList != null && model.SKUInfo.SKUList.Count > 0)
                    {
                        model.SKUInfo.SKUList = model.SKUInfo.SKUList.Where(_ => !_.HasTag(ProductServiceEnums.ProductTagType.IsOffLine)).ToList();
                    }
                }
                catch (Exception)
                {

                }
                if (userid > 0)
                {
                    model.BoughtCount = CouponAdapter.GetExchangeCouponListByUserIDAndSKU(userid, model.SKUInfo.SKU.ID).Where(_ => _.State != 4 && _.State != 40 && _.State != 5).Count();
                }

                try
                {
                    int showState = 1;//现金券前段展示状态 0 不展示 1展示
                    model.CouponInfo = CouponAdapter.GetCanCouponDefineByBizID(SKUID, 1, userid, showState);

                    model.SKUInfo.SPU.SKUList = new List<SKUEntity>();//
                }
                catch (Exception e)
                {
                    Log.WriteLog("产品关联红包报错 ERROR：" + e);
                }

                CouponAdapter.DealStepGroupSKUPrice(model, couponOrderId);
                model.activity.SellNum = (model.activity.SellNum + model.activity.ManuSellNum) > 0 ? model.activity.SellNum + model.activity.ManuSellNum : 0;
            }
            model = model == null ? new SKUCouponActivityDetailModel() : model;

            SetSKUActivityInfo(model);

            return model;
        }

        /// <summary>
        /// 根据SKUID返回活动信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        private static void SetSKUActivityInfo(SKUCouponActivityDetailModel pinfo)
        {
            int curActivityID = 1; // 1: 积分送里程 
            if (IsSKUInActivity(curActivityID, pinfo))
            {
                GenActivityJson(curActivityID, pinfo);
            }
        }

        /// <summary>
        /// 判断SKU是否在活动中
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="pinfo"></param>
        /// <returns></returns>
        private static bool IsSKUInActivity(int activityID, SKUCouponActivityDetailModel pinfo)
        {
            bool result = false;
            switch (activityID)
            {
                case 1:
                    result = GetCachedAlbumsSKUIDs(new List<int> { 26, 27, 28 }).Contains(pinfo.SKUInfo.SKU.ID);
                    break;
            }

            return result;
        }

        private static SortedSet<int> GetCachedAlbumsSKUIDs(List<int> albumsIdList)
        {
            SortedSet<int> skuids = new SortedSet<int>();
            ProductAdapter.GetProductAlbumSkuByAlbumIds(string.Join(",", albumsIdList)).ForEach(_ => skuids.Add(_.SKUID));
            return skuids;
            //return CacheAdapter.LocalCache10Min.GetData<SortedSet<int>>(string.Format("GetCachedAlbumsSKUIDs:{0}", string.Join("_", albumsIdList)), () =>
            //{
            //    SortedSet<int> skuids = new SortedSet<int>();
            //    ProductAdapter.GetProductAlbumSkuByAlbumIds(string.Join(",", albumsIdList)).ForEach(_ => skuids.Add(_.SKUID));
            //    return skuids;
            //});
        }




        /// <summary>
        /// 生成活动JSON
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="pinfo"></param>
        /// <returns></returns>
        private static void GenActivityJson(int activityID, SKUCouponActivityDetailModel pinfo)
        {
            switch (activityID)
            {
                case 1:
                    string strPrice = pinfo.SKUInfo.SKU.VIPPrice.ToString();
                    pinfo.ActiviyInfo = @"{""activeID"":1, ""activeIcon"":""http://whfront.b0.upaiyun.com/www/img/Active/productalbumactive/active-detail-tip-1.png"",""activeTip"":""购买本产品可获赠航空里程"",""activeLink"":""/Active/ProductAlbumActive?_newpage=1""}";
                    break;
            }
        }

        /// <summary>
        /// 获取SKU、团购活动信息
        /// </summary>
        /// <param name="SKUID"></param>
        /// <param name="userId"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpGet]
        public GroupSKUCouponActivityModel GetGroupSKUCouponActivityModel(int SKUID, long userId, int groupid, string openId = "")
        {
            GroupSKUCouponActivityModel model = CouponAdapter.GetGroupSKUCouponActivity(SKUID, userId, groupid, openId: openId);//10min缓存
            if (model != null && model.activity != null)
            {


                #region 插入浏览记录
                if (userId > 0)
                {
                    int terminalType = _ContextBasicInfo.IsWeb ? 1 : _ContextBasicInfo.IsIOS ? 2 : _ContextBasicInfo.IsAndroid ? 3 : 4;
                    string clientIP = terminalType == 2 || terminalType == 3 ? HttpRequestHelper.GetClientIp(this.ControllerContext.Request) : "";

                    HotelAdapter.InsertBrowsingRecord(new BrowsingRecordEntity()
                    {
                        BusinessID = SKUID,
                        BusinessType = (int)HJDAPI.Controllers.EnumHelper.BrowsingRecordType.sku,
                        Visitor = userId,
                        TerminalType = terminalType,
                        ClientIP = clientIP,
                        SessionID = "",
                        OpenID = "",
                        AppVer = _ContextBasicInfo.AppVer
                    });
                }
                #endregion
                PackageEntity p = hotelService.GetOnePackageEntity(Convert.ToInt32(model.activity.SourceID));
                model.activity.HotelID = p.HotelID;
                if (model.activity != null && model.activity.Notice != null & model.activity.Notice != "")
                {
                    model.activity.NoticeList = model.activity.Notice.Split('_').ToList();
                }
                if (model.activity != null && model.activity.PackageInfo != null & model.activity.PackageInfo != "")
                {
                    model.activity.PackageInfoList = model.activity.PackageInfo.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                if (model.activity != null && model.activity.PackageInfo != null & model.activity.PackageInfo != "")
                {
                    model.activity.UseDecriptionList = model.activity.UseDecription.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                model.activity.SellNum = (model.activity.SellNum + model.activity.ManuSellNum) > 0 ? model.activity.SellNum + model.activity.ManuSellNum : 0;
                //try
                //{
                //    //实际卖出的量
                //    model.activity.SellNum = CouponAdapter.GetExchangeCouponEntityListByActivity(model.activity.ID).Where(_ => _.State == 2 || _.State == 3).Count();
                //    model.activity.SellNum = model.activity.SellNum + model.activity.ManuSellNum;
                //}
                //catch (Exception e)
                //{
                //    Log.WriteLog("实际卖出的量 error:" + model.activity.ID + "   " + e);
                //}


                try
                {
                    model.CouponInfo = CouponAdapter.GetCanCouponDefineByBizID(SKUID, 1, userId);
                    //List<HJD.CouponService.Contracts.Entity.UserCouponUseCondationEntity> couponUseCondationList = CouponAdapter.GetCouponUseCondationByInVal(SKUID, 1);
                    //if (couponUseCondationList != null && couponUseCondationList.Count > 0)
                    //{
                    //    model.CouponInfo = new PackageAndProductCouponDefineEntity();
                    //    string ids = string.Join(",", couponUseCondationList.Select(_ => _.CouponDefineID).ToList());
                    //    List<HJD.CouponService.Contracts.Entity.UserCouponDefineEntity> userCouponDefine = CouponAdapter.GetUserCouponDefineListByIds(ids,userId);
                    //    decimal sumDiscountAmount = userCouponDefine.Sum(_ => _.DiscountAmount);
                    //    model.CouponInfo.Icon = "http://whfront.b0.upaiyun.com/app/img/coupon/product/product-coupon-icon-1.png";
                    //    model.CouponInfo.Link = "";
                    //    model.CouponInfo.Tip = "立即领取￥" + sumDiscountAmount + "红包，尽享优惠";
                    //    model.CouponInfo.CouponDefineList = userCouponDefine;
                    //}
                }
                catch (Exception e)
                {
                    Log.WriteLog("产品关联红包报错 ERROR：" + e);
                }

            }
            return model == null ? new GroupSKUCouponActivityModel() : model;

        }

        /// <summary>
        /// 获取指定SKUID的消费券产品列表
        /// </summary>
        /// <param name="skuids">String:可包含多个SKUID，英文逗号间隔</param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public List<SKUCouponActivityEntity> GetSKUCouponActivityListBySKUIds(string skuids, int userid = 0)
        {
            var list = CouponAdapter.GetSKUCouponActivityListBySKUIds(skuids, userid);
            return list;
        }

        /// <summary>
        /// 根据专辑ID获取专辑信息及SKUCouponActivity列表
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public SKUCouponActivityAlbumEntity GetProductAlbumSKUCouponActivityListByAlbumID(int albumId, int start, int count, int userid = 0, bool searchReceive = false, int districtID = 0, int commTagID = 0, int sort = 0)
        {
            SKUCouponActivityAlbumEntity SKUCouponActivityAlbum = new SKUCouponActivityAlbumEntity();
            SKUCouponActivityAlbum = CouponAdapter.GetProductAlbumSKUCouponActivityListByAlbumID(albumId, start, count, userid, searchReceive, districtID, commTagID, sort);
            return SKUCouponActivityAlbum;
        }


        public bool GetProductAlbumSKUBySKUIDAndAlbumId(int skuid, int albumid)
        {
            return CouponAdapter.GetProductAlbumSKUBySKUIDAndAlbumId(skuid, albumid);
        }
        /// <summary>
        /// 【小程序】根据专辑ID获取专辑信息及SKUCouponActivity列表
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public SKUCouponActivityAlbumEntity GetProductAlbumSKUCouponActivityListByAlbumIDForWxapp(int albumId, int start, int count, int userid = 0, bool searchReceive = false, int sort = 0, int districtID = 0)
        {
            SKUCouponActivityAlbumEntity SKUCouponActivityAlbum = new SKUCouponActivityAlbumEntity();
            SKUCouponActivityAlbum = CouponAdapter.GetProductAlbumSKUCouponActivityListByAlbumID(albumId, start, count, userid, searchReceive, districtID, sort: sort);
            return SKUCouponActivityAlbum;
        }

        /// <summary>
        /// 【遛娃小程序】 首页列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SKUCouponActivityAlbumEntity GetProductAlbumSKUCouponByAlbumForWxapp()
        {
            return new SKUCouponActivityAlbumEntity();
        }
        [HttpGet]
        public ProductAlbumSumEntity GetProductAlbumSummary(int albumId)
        {
            ProductAlbumSumEntity albumSum = new ProductAlbumSumEntity();
            albumSum = CouponAdapter.GetProductAlbumSummary(albumId);
            return albumSum;
        }

        /// <summary>
        /// 根据产品分类，获取消费券
        /// </summary>
        /// <param name="category">产品分类对应的 母id（parentid）</param>
        /// <param name="districtID"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="geoScopeType">3：周边 0：指定地区</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="userid"></param>
        /// <param name="sort">排序 1：按销量排序 2：价格由高至低 3：价格由低至高 4：距离由近及远 5：距离由远及近</param>
        /// <param name="payType">payType 0：货币支付 1：积分支付</param>
        /// <returns></returns>
        [HttpGet]
        public SKUCouponActivityAlbumEntity GetProductSKUCouponList(int category = 14, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int userid = 0, int sort = 0, int payType = 0, double locLat = 0, double locLng = 0)
        {
            return CouponAdapter.GetProductSKUCouponList(category, districtID, lat, lng, geoScopeType, start, count, userid, sort, payType, locLat, locLng);
        }
        /// <summary>
        /// 根据产品所属分类标签ProductTagID，获取消费券
        /// </summary>
        /// <param name="ID">产品分类ID</param>
        /// <param name="districtID"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="geoScopeType">3：周边 0：指定地区</param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="userid"></param>
        /// <param name="sort">排序 1：按销量排序 2：价格由高至低 3：价格由低至高 4：距离由近及远 5：距离由远及近</param>
        /// <param name="payType">payType 0：货币支付 1：积分支付</param>
        /// <returns></returns>
        [HttpGet]
        public SKUCouponActivityAlbumEntity GetProductSKUCouponListByID(int ID = 0, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int userid = 0, int sort = 0, int payType = 0, double locLat = 0, double locLng = 0)
        {
            return CouponAdapter.GetProductSKUCouponListByID(ID, districtID, lat, lng, geoScopeType, start, count, userid, sort, payType, locLat, locLng);
        }
        /// <summary>
        /// 获取多人成团性质的房券售卖
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        [HttpGet]
        public CouponActivityDetailModel GetGroupCouponActivityDetail(int activityID)
        {
            CouponActivityDetailModel model = CouponAdapter.GetCouponActivityDetail(activityID, true);//10min缓存
            var result = ProductCache.CouponSellInfoMem(activityID);//1min缓存
            if (result != null)
            {
                model.activity.SellNum = result.SellNum;//更新已卖出量（算上了后台设置的限制量）
            }

            return model;
        }

        /// <summary>
        /// 【消费券产品】根据指定用户的购买产品相关参数，检测当前享受的Promotion优惠策略
        /// </summary>
        /// <param name="skuid">SKUID</param>
        /// <param name="buynum">购买数量</param>
        /// <param name="userid">UserId</param>
        /// <param name="sType">购买场景（wap/web/app/weixin/wxapp）</param>
        /// <param name="usType">购买阶段</param>
        /// <returns>当前条件下可享受的优惠信息实体</returns>
        [HttpGet]
        public PromotionCheckEntity CheckProductPromotionForCoupon(int skuid, int buynum, long userid, ProductServiceEnums.SceneType sType)
        {
            var obj = ProductAdapter.CheckProductPromotionForCoupon(skuid, buynum, userid, sType);

            return obj;
        }

        /// <summary>
        /// 【参数指定产品类型】根据指定用户的购买产品相关参数，检测当前享受的Promotion优惠策略
        /// </summary>
        /// <param name="productType">产品类型（房券/消费券/常规酒店产品）</param>
        /// <param name="objId">产品ID（目前可以是SKUID/CouponId）</param>
        /// <param name="buynum">购买数量</param>
        /// <param name="userid">UserId</param>
        /// <param name="sType">购买场景（wap/web/app/weixin/wxapp）</param>
        /// <param name="usType">购买阶段：买前/买中/买后</param>
        /// <returns></returns>
        [HttpGet]
        public PromotionCheckEntity CheckProductPromotion(ProductServiceEnums.ProductType productType, int objId, int buynum, long userid, ProductServiceEnums.SceneType sType, ProductServiceEnums.PromotionUseSceneType usType = ProductServiceEnums.PromotionUseSceneType.Shopping)
        {
            var obj = ProductAdapter.CheckProductPromotion(productType, objId, buynum, userid, sType, usType);

            return obj;
        }

        /// <summary>
        /// 获取房券订单列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="timeStamp"></param>
        /// <param name="sourceID"></param>
        /// <param name="requestType"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpGet]
        public RoomCouponResult GetPersonalRoomCoupon(long userId, long timeStamp, int sourceID, string requestType, string sign)
        {
            if (userId == 0)
            {
                return new RoomCouponResult();
            }
            List<ExchangeCouponEntity> exchangelist = CouponAdapter.GetExchangeCouponEntityListByUser(userId, 0).FindAll(_ => _.ActivityType != (int)(Enums.CouponType.VIP) && _.ActivityType != (int)(Enums.CouponType.ProductCoupon) && ((_.State == 2 && _.PayType != 0) || _.State == 3 || _.State == 1 || _.State == 5 || _.State == 4 || _.State == 8));//去掉已取消和支付成功且超时的券状态  //增加显示已取消状态的条件 || _.State == 4 haoy 2016.12.27
            List<RoomCouponOrderEntity> roomCouponList = new List<RoomCouponOrderEntity>();


            //string skuids = string.Join(",", exchangelist.Select(_ => _.SKUID).Distinct()).ToString();
            //List<ProductPropertyInfoEntity> ppiList = ProductAdapter.GetProductPropertyInfoBySKU(skuids);

            if (exchangelist != null && exchangelist.Count != 0)
            {
                foreach (var exchangeGroup in exchangelist.Where(_ => _.State == (int)Enums.CouponState.submit).GroupBy(_ => new { _.ActivityID, _.PayID, key = _.PayID }).OrderByDescending(_ => _.Key.key)) //不同的优惠也需要拆分显示
                {
                    List<ExchangeCouponEntity> tempCouponList = exchangeGroup.ToList();
                    ExchangeCouponEntity tempCoupon = tempCouponList[0];

                    ////20181017 可以拿掉 (tempCoupon.State == 2 || tempCoupon.State == 6) && tempCoupon.RefundState > 0 ? 7 :   //新增“退款中”中的状态
                    //int realState = (tempCoupon.State == 2 || tempCoupon.State == 6) && tempCoupon.RefundState > 0 ? 7 : tempCoupon.State;

                    int realState = tempCoupon.State;
                    if (tempCoupon.ExpireTime <= DateTime.Now.Date.AddDays(-1) && (realState == 2 || realState == 6))
                    {
                        realState = 8;//已过期（特殊状态） 出现在不能退的券客户未消费或者系统自动退款服务没有及时退款(显示的一种中间状态)
                    }
                    SKUEntity sku = new SKUEntity();
                    CouponOrderStepGroupEntity couponOderStepGroup = new CouponOrderStepGroupEntity();
                    try
                    {
                        sku = ProductAdapter.GetSKUEXEntityByID(tempCoupon.SKUID);

                        //获取skuproperty
                        ProductPropertyInfoEntity ppi = ProductAdapter.GetProductPropertyInfoBySKU(sku.ID.ToString()).First();
                        //未支付的大团购给非定金产品不显示
                        if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 2)
                        {
                            int orderCount = exchangelist.Where(_ => _.CouponOrderId == tempCoupon.CouponOrderId).Count();
                            //判断是否是 非定金单买的产品
                            if (orderCount == 1)
                            {
                                StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPUID);
                                couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                couponOderStepGroup.TailSKUID = sku.ID;
                                couponOderStepGroup.DepositSKUID = 0;
                                couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                couponOderStepGroup.Price = sku.Price;
                                couponOderStepGroup.TotalPrice = sku.Price;
                                couponOderStepGroup.IsPayFinish = false;
                                couponOderStepGroup.BookPosition = 0;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 1)
                        {
                            StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPUID);
                            couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                            couponOderStepGroup.TailSKUID = 0;
                            couponOderStepGroup.DepositSKUID = sku.ID;
                            couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                            couponOderStepGroup.StepGroupState = stg.StepGroupState;
                            couponOderStepGroup.Price = 0;
                            couponOderStepGroup.TotalPrice = 0;
                            couponOderStepGroup.IsPayFinish = false;
                            couponOderStepGroup.BookPosition = 0;
                        }
                        else
                        {
                            couponOderStepGroup = null;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog("GetSKUEntityByID 1:" + tempCoupon.SKUID.ToString() + e);
                    }


                    GroupPurchaseEntity purchase = ProductAdapter.GetGroupPurchaseEntity(tempCoupon.GroupId).FirstOrDefault();
                    //券的类型和套餐类型填一个值 增加一个待退款的中间状态
                    roomCouponList.Add(new RoomCouponOrderEntity()
                    {
                        CouponOrderID = tempCoupon.PayID,
                        CouponPrice = tempCoupon.Price,
                        TotalPrice = tempCouponList.Sum(_ => _.Price),
                        TotalPoints = tempCouponList.Sum(_ => _.Points),
                        HotelName = tempCoupon.ObjectName,
                        PackageName = tempCoupon.CouponName,
                        ExchangeCouponList = CouponAdapter.TransExchangeCouponEntity2ExchangeCouponModel(tempCouponList),
                        State = realState,
                        CreateTime = tempCoupon.CreateTime,
                        GroupId = tempCoupon.GroupId,
                        GroupPurchase = purchase,
                        CouponOrderStepGroup = couponOderStepGroup
                    });
                }
                foreach (var item in exchangelist.Where(_ => _.State != (int)Enums.CouponState.submit).OrderByDescending(_ => _.ID))
                {

                    //20181017   (item.State == 2 || item.State == 6) && item.RefundState > 0 ? 7 :  //新增“退款中” 的状态
                    //int realState = (item.State == 2 || item.State == 6) && item.RefundState > 0 ? 7 : item.State;
                    int realState = item.State;
                    if (item.ExpireTime <= DateTime.Now.Date.AddDays(-1) && (realState == 2 || realState == 6))
                    {
                        realState = 8;//已过期（特殊状态） 出现在不能退的券客户未消费或者系统自动退款服务没有及时退款(显示的一种中间状态)
                    }
                    SKUInfoEntity sku = new SKUInfoEntity();
                    CouponOrderStepGroupEntity couponOderStepGroup = new CouponOrderStepGroupEntity();
                    try
                    {
                        sku = ProductAdapter.GetSKUInfoByID(item.SKUID);

                        string skuids = string.Join(",", sku.SKUList.Select(_ => _.ID)).ToString();
                        //获取skuproperty
                        List<ProductPropertyInfoEntity> ppiList = ProductAdapter.GetProductPropertyInfoBySKU(skuids);
                        ProductPropertyInfoEntity ppi = ppiList.Where(_ => _.SKUID == item.SKUID).First();

                        #region 大团购
                        //判断是否是大团购
                        if (ppi.PropertyType == 6)
                        {
                            #region 定金产品
                            if (ppi.PropertyOptionTargetID == 1)
                            {
                                StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPU.ID);
                                ProductPropertyInfoEntity ppi2 = ppiList.Where(_ => _.PropertyType == 6 && _.PropertyOptionTargetID == 2).First();

                                SKUEntity tailsku = sku.SKUList.Where(_ => _.ID == ppi2.SKUID).First();
                                //定金 已支付
                                if (item.State == 2)
                                {
                                    couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                    couponOderStepGroup.TailSKUID = tailsku.ID;
                                    couponOderStepGroup.DepositSKUID = item.SKUID;
                                    couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                    couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                    couponOderStepGroup.Price = tailsku.Price - sku.SKU.MarketPrice;//支付价格-膨胀金价格（MarketPrice是大团购的膨胀金）
                                    couponOderStepGroup.TotalPrice = tailsku.Price;// 当前sku的售卖价
                                    couponOderStepGroup.IsPayFinish = false;
                                    couponOderStepGroup.BookPosition = tailsku.BookPosition;
                                }
                                else if (item.State == 3)//已支付 定金尾款
                                {
                                    continue;
                                }
                                else if (item.State != 2 && item.State != 3)//未支付或已取消、已退款等状态
                                {
                                    couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                    couponOderStepGroup.TailSKUID = 0;
                                    couponOderStepGroup.DepositSKUID = item.SKUID;
                                    couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                    couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                    couponOderStepGroup.Price = 0;//Price,尾款金额
                                    couponOderStepGroup.TotalPrice = 0;// 总支付金额
                                    couponOderStepGroup.IsPayFinish = false;
                                    couponOderStepGroup.BookPosition = 0;

                                }
                            }
                            #endregion
                            #region 尾款产品
                            else if (ppi.PropertyOptionTargetID == 2)
                            {
                                ProductPropertyInfoEntity ppi2 = ppiList.Where(_ => _.PropertyType == 6 && _.PropertyOptionTargetID == 1).First();
                                SKUEntity depositsku = sku.SKUList.Where(_ => _.ID == ppi2.SKUID).First();
                                StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPU.ID);

                                int orderCount = exchangelist.Where(_ => _.CouponOrderId == item.CouponOrderId).Count();
                                //判断是否是单买  
                                if (orderCount == 1)
                                {
                                    couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                    couponOderStepGroup.TailSKUID = item.SKUID;
                                    couponOderStepGroup.DepositSKUID = depositsku.ID;
                                    couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                    couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                    couponOderStepGroup.Price = item.OriPrice;
                                    couponOderStepGroup.TotalPrice = item.OriPrice;
                                    couponOderStepGroup.IsPayFinish = true;
                                    couponOderStepGroup.BookPosition = sku.SKU.BookPosition;
                                }
                                else
                                {
                                    //订单列表 只有已消费、已支付、已退款的产品才会显示在订单列表
                                    if (item.State == 2 || item.State == 3 || item.State == 5)
                                    {
                                        couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                        couponOderStepGroup.TailSKUID = item.SKUID;
                                        couponOderStepGroup.DepositSKUID = depositsku.ID;
                                        couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                        couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                        couponOderStepGroup.Price = item.OriPrice;
                                        if (item.State == 5 || realState == 7)
                                        {
                                            couponOderStepGroup.TotalPrice = item.OriPrice;//退款状态只显示实际支付尾款价格
                                        }
                                        else
                                        {
                                            couponOderStepGroup.TotalPrice = item.OriPrice + depositsku.VIPPrice;
                                        }
                                        couponOderStepGroup.IsPayFinish = true;
                                        couponOderStepGroup.BookPosition = sku.SKU.BookPosition;
                                    }
                                    else if (item.State != 2 && item.State != 3 && item.State != 5)
                                    {
                                        continue;
                                    }
                                }
                            }
                            #endregion

                        }
                        else
                        {
                            couponOderStepGroup = null;
                        }
                        #endregion



                        #region  注释大团购 订单列表
                        //List<ExchangeCouponEntity> stepGroupOrderList = couponService.GetExchangCouponByCouponOrderID(item.CouponOrderId);
                        ////单买状态
                        //if (stepGroupOrderList.Count == 1 && ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 2)
                        //{
                        //    ProductPropertyInfoEntity ppi2 = ppiList.Where(_ => _.PropertyType == 6 && _.PropertyOptionTargetID == 1).First();
                        //    SKUEntity depositsku = sku.SKUList.Where(_ => _.ID == ppi2.SKUID).First();
                        //    StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPU.ID);
                        //    couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                        //    couponOderStepGroup.TailSKUID = item.SKUID;
                        //    couponOderStepGroup.DepositSKUID = depositsku.ID;
                        //    couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                        //    couponOderStepGroup.StepGroupState = stg.StepGroupState;
                        //    couponOderStepGroup.Price = item.OriPrice;
                        //    couponOderStepGroup.TotalPrice = item.OriPrice;
                        //    couponOderStepGroup.IsPayFinish = true;
                        //    couponOderStepGroup.BookPosition = sku.SKU.BookPosition;
                        //}
                        //else
                        //{
                        //    //判断是否是大团购
                        //    if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 1 && item.State == 2)//该产品是大团购的定金产品，并且状态是已支付。---未支付尾款
                        //    {
                        //        ExchangeCouponEntity stepGroupOrder = couponService.GetExchangCouponByCouponOrderID(item.CouponOrderId).Where(_ => _.State == 1).FirstOrDefault();
                        //        StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPU.ID);
                        //        ProductPropertyInfoEntity ppi2 = ppiList.Where(_ => _.PropertyType == 6 && _.PropertyOptionTargetID == 2).First();

                        //        SKUEntity tailsku = sku.SKUList.Where(_ => _.ID == ppi2.SKUID).First();
                        //        couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                        //        couponOderStepGroup.TailSKUID = tailsku.ID;
                        //        couponOderStepGroup.DepositSKUID = item.SKUID;
                        //        couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                        //        couponOderStepGroup.StepGroupState = stg.StepGroupState;
                        //        couponOderStepGroup.Price = tailsku.Price - sku.SKU.MarketPrice;//支付价格-膨胀金价格（MarketPrice是大团购的膨胀金）
                        //        couponOderStepGroup.TotalPrice = tailsku.Price;// 当前sku的售卖价
                        //        couponOderStepGroup.IsPayFinish = false;
                        //        couponOderStepGroup.BookPosition = tailsku.BookPosition;
                        //    }
                        //    else if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 1 && item.State == 3)//该产品是大团购产品，并且状态是已核销。---已支付尾款，不在订单列表显示
                        //    {
                        //        continue;
                        //    }
                        //    else if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 1 && (item.State != 2 && item.State != 3))
                        //    {
                        //        StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPU.ID);
                        //        couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                        //        couponOderStepGroup.TailSKUID = 0;
                        //        couponOderStepGroup.DepositSKUID = item.SKUID;
                        //        couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                        //        couponOderStepGroup.StepGroupState = stg.StepGroupState;
                        //        couponOderStepGroup.Price = 0;//Price,尾款金额
                        //        couponOderStepGroup.TotalPrice = 0;// 总支付金额
                        //        couponOderStepGroup.IsPayFinish = false;
                        //        couponOderStepGroup.BookPosition = 0;

                        //    }
                        //    else if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 2 && (item.State == 2 || item.State == 3 || item.State == 5))//该产品是大团购非定金产品，并且状态是已支付或核销。---已支付尾款
                        //    {
                        //        ProductPropertyInfoEntity ppi2 = ppiList.Where(_ => _.PropertyType == 6 && _.PropertyOptionTargetID == 1).First();
                        //        SKUEntity depositsku = sku.SKUList.Where(_ => _.ID == ppi2.SKUID).First();
                        //        //ExchangeCouponEntity stepGroupOrder = couponService.GetExchangCouponByCouponOrderID(item.CouponOrderId).Where(_ => _.State == 1).FirstOrDefault();
                        //        StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPU.ID);
                        //        couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                        //        couponOderStepGroup.TailSKUID = item.SKUID;
                        //        couponOderStepGroup.DepositSKUID = depositsku.ID;
                        //        couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                        //        couponOderStepGroup.StepGroupState = stg.StepGroupState;
                        //        couponOderStepGroup.Price = item.OriPrice;
                        //        if (item.State == 5 || realState == 7)
                        //        {
                        //            couponOderStepGroup.TotalPrice = item.OriPrice;//退款状态只显示实际支付尾款价格
                        //        }
                        //        else
                        //        {
                        //            couponOderStepGroup.TotalPrice = item.OriPrice + depositsku.VIPPrice;
                        //        }
                        //        couponOderStepGroup.IsPayFinish = true;
                        //        couponOderStepGroup.BookPosition = sku.SKU.BookPosition;
                        //    }
                        //    else if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 2 && (item.State != 2 || item.State != 3 || item.State != 5))//该产品是大团购非定金产品，并且状态是已支付或核销。---未支付尾款，非定金不在订单列表显示
                        //    {
                        //        continue;
                        //    }
                        //    else
                        //    {
                        //        couponOderStepGroup = null;
                        //    }
                        //} 
                        #endregion
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog("GetSKUEntityByID 2:" + item.SKUID.ToString() + e);
                    }
                    GroupPurchaseEntity purchase = ProductAdapter.GetGroupPurchaseEntity(item.GroupId).FirstOrDefault();
                    //券的类型和套餐类型填一个值 增加一个待退款的中间状态
                    roomCouponList.Add(new RoomCouponOrderEntity()
                    {
                        CouponOrderID = item.PayID,
                        CouponPrice = item.Price,
                        TotalPrice = item.Price,
                        TotalPoints = item.Points,
                        HotelName = item.ObjectName,
                        PackageName = item.CouponName,
                        ExchangeCouponList = CouponAdapter.TransExchangeCouponEntity2ExchangeCouponModel(new List<ExchangeCouponEntity> { item }),
                        State = realState,
                        CreateTime = item.CreateTime,
                        GroupId = item.GroupId,
                        GroupPurchase = purchase,
                        CouponOrderStepGroup = couponOderStepGroup
                    });
                }
            }
            return new RoomCouponResult() { couponList = roomCouponList };

        }

        /// <summary>
        /// 消费券数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public int GetPersonalProductCouponCount(long userId)
        {
            int count = CouponAdapter.GetExchangeCouponEntityListByUser(userId, 0).
                       FindAll(_ => _.ActivityType == 600 && _.State == 2 && _.PayType != 0 && _.RefundState == 0).Count;// 只显示状态可用的数量|| _.State == 3 || _.State == 1 || _.State == 5 || _.State == 4) yyb 2017 04 10
            return count;
        }


        /// <summary>
        ///  用户是否可以重新上传照片
        /// </summary>
        /// <param name="userid"></param>
        [HttpGet]
        public Int32 GetUserCanReUpdatePhotoListByUserID(long userid)
        {
            return CouponAdapter.GetUserCanReUpdatePhotoListByUserID(userid);
        }

        /// <summary>
        /// 获取用户可以更新的券照片的ID
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="CouponID"></param>
        /// <param name="operatorUserID"></param>
        /// <returns></returns>
        [HttpGet]
        public bool SetUserCanReUpdatePhotoListByUserID(long userid, Int32 CouponID, long operatorUserID)
        {
            return CouponAdapter.SetUserCanReUpdatePhotoListByUserID(userid, CouponID, operatorUserID);
        }


        /// <summary>
        /// 更新券信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public BaseResponse UpdateCoupnPhoto(UpdateCouponPhotoReqParms p)
        {
            return CouponAdapter.UpdateCoupnPhoto(p);
        }

        /// <summary>
        /// 消费券详情
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public ProductCouponResult GetPersonalProductCoupon(long userId)
        {
            List<ExchangeCouponEntity> exchangelist = CouponAdapter.GetExchangeCouponEntityListByUser(userId, 0).FindAll(_ => _.ActivityType == (int)(Enums.CouponType.ProductCoupon) && ((_.State == (int)Enums.CouponState.paied && _.PayType != 0) || _.State == (int)Enums.CouponState.consumed || _.State == (int)Enums.CouponState.submit || _.State == (int)Enums.CouponState.cancel || _.State == (int)Enums.CouponState.refund || _.State == (int)Enums.CouponState.expired));//去掉已取消和支付成功且超时的券状态  //增加显示已取消状态的条件 || _.State == 4 haoy 2016.12.27
            List<ProductCouponOrderResult> productCouponOrderList = new List<ProductCouponOrderResult>();

            //string skuids = string.Join(",", exchangelist.Select(_ => _.SKUID).Distinct()).ToString();
            ////获取skuproperty
            //List<ProductPropertyInfoEntity> ppiList = ProductAdapter.GetProductPropertyInfoBySKU(skuids);

            if (exchangelist != null && exchangelist.Count != 0)
            {
                foreach (var exchangeGroup in exchangelist.Where(_ => _.State == (int)Enums.CouponState.submit).GroupBy(_ => new { _.ActivityID, _.PayID, sn = _.PayID }).OrderByDescending(_ => _.Key.sn)) //不同的优惠也需要拆分显示
                {
                    List<ExchangeCouponEntity> tempCouponList = exchangeGroup.ToList();
                    ExchangeCouponEntity tempCoupon = tempCouponList[0];

                    //20181017 可以拿掉 (tempCoupon.State == 2 || tempCoupon.State == 6) && tempCoupon.RefundState > 0 ? 7 :  //新增“退款中” 的状态
                    //int realState = (tempCoupon.State == 2 || tempCoupon.State == 6) && tempCoupon.RefundState > 0 ? 7 : tempCoupon.State;
                    int realState = tempCoupon.State;
                    if (tempCoupon.ExpireTime <= DateTime.Now.Date.AddDays(-1) && (realState == 2 || realState == 6))
                    {
                        realState = 8;//已过期（特殊状态） 出现在不能退的券客户未消费或者系统自动退款服务没有及时退款(显示的一种中间状态)
                    }
                    SKUEntity sku = new SKUEntity();
                    CouponOrderStepGroupEntity couponOderStepGroup = new CouponOrderStepGroupEntity();
                    try
                    {
                        sku = ProductAdapter.GetSKUEXEntityByID(tempCoupon.SKUID);

                        ////获取skuproperty
                        ProductPropertyInfoEntity ppi = ProductAdapter.GetProductPropertyInfoBySKU(sku.ID.ToString()).First();

                        //ProductPropertyInfoEntity ppi = ppiList.Where(_ => _.SKUID == tempCoupon.SKUID).FirstOrDefault();

                        if (ppi != null && ppi.PropertyType == 6)
                        {
                            if (ppi.PropertyOptionTargetID == 1)
                            {
                                StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPUID);
                                couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                couponOderStepGroup.TailSKUID = 0;
                                couponOderStepGroup.DepositSKUID = sku.ID;
                                couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                couponOderStepGroup.Price = 0;
                                couponOderStepGroup.TotalPrice = 0;
                                couponOderStepGroup.IsPayFinish = false;
                                couponOderStepGroup.BookPosition = 0;
                            }
                            else if (ppi.PropertyOptionTargetID == 2)
                            {
                                int orderCount = exchangelist.Where(_ => _.CouponOrderId == tempCoupon.CouponOrderId).Count();
                                if (orderCount == 1)
                                {
                                    StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPUID);
                                    couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                    couponOderStepGroup.TailSKUID = sku.ID;
                                    couponOderStepGroup.DepositSKUID = 0;
                                    couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                    couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                    couponOderStepGroup.Price = sku.Price;
                                    couponOderStepGroup.TotalPrice = sku.Price;
                                    couponOderStepGroup.IsPayFinish = false;
                                    couponOderStepGroup.BookPosition = 0;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            couponOderStepGroup = null;
                        }


                        #region 注释大团购
                        ////未支付的大团购给非定金产品不显示
                        //if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 2)
                        //{
                        //    int orderCount = exchangelist.Where(_ => _.CouponOrderId == tempCoupon.CouponOrderId).Count();
                        //    //List<ExchangeCouponEntity> stepGroupOrderList = couponService.GetExchangCouponByCouponOrderID(tempCoupon.CouponOrderId);
                        //    if (orderCount == 1)
                        //    {
                        //        StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPUID);
                        //        couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                        //        couponOderStepGroup.TailSKUID = sku.ID;
                        //        couponOderStepGroup.DepositSKUID = 0;
                        //        couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                        //        couponOderStepGroup.StepGroupState = stg.StepGroupState;
                        //        couponOderStepGroup.Price = sku.Price;
                        //        couponOderStepGroup.TotalPrice = sku.Price;
                        //        couponOderStepGroup.IsPayFinish = false;
                        //        couponOderStepGroup.BookPosition = 0;
                        //    }
                        //    else
                        //    {
                        //        continue;
                        //    }
                        //}
                        //else if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 1)
                        //{
                        //    StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPUID);
                        //    couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                        //    couponOderStepGroup.TailSKUID = 0;
                        //    couponOderStepGroup.DepositSKUID = sku.ID;
                        //    couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                        //    couponOderStepGroup.StepGroupState = stg.StepGroupState;
                        //    couponOderStepGroup.Price = 0;
                        //    couponOderStepGroup.TotalPrice = 0;
                        //    couponOderStepGroup.IsPayFinish = false;
                        //    couponOderStepGroup.BookPosition = 0;
                        //}
                        //else
                        //{
                        //    couponOderStepGroup = null;
                        //} 
                        #endregion
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog("GetSKUEntityByID 3:" + tempCoupon.SKUID.ToString() + e);
                    }


                    GroupPurchaseEntity purchase = ProductAdapter.GetGroupPurchaseEntity(tempCoupon.GroupId).FirstOrDefault();
                    //券的类型和套餐类型填一个值 增加一个待退款的中间状态
                    productCouponOrderList.Add(new ProductCouponOrderResult()
                    {
                        CouponOrderID = exchangeGroup.Key.PayID,
                        CouponPrice = tempCoupon.Price,
                        TotalPrice = tempCouponList.Sum(_ => _.Price),
                        TotalPoints = tempCouponList.Sum(_ => _.Points),
                        HotelName = tempCoupon.ObjectName,
                        PackageName = tempCoupon.CouponName,
                        ExchangeCouponList = CouponAdapter.TransExchangeCouponEntity2ExchangeCouponModel(tempCouponList),
                        State = realState,
                        CreateTime = tempCoupon.CreateTime,
                        ExpireTime = Convert.ToDateTime(tempCoupon.ExpireTime),
                        SkuName = sku.Name != null ? sku.Name : "",
                        SkuID = tempCoupon.SKUID,
                        Notice = string.IsNullOrWhiteSpace(tempCoupon.Notice) ? new List<string>() : tempCoupon.Notice.Trim().Split('_').ToList(),
                        DicProperties = StringHelper.ParseStringToDic(tempCoupon.Properties),
                        GroupId = tempCoupon.GroupId,
                        GroupPurchase = purchase,
                        CouponOrderStepGroup = couponOderStepGroup
                    });
                }
                foreach (var item in exchangelist.Where(_ => _.State != (int)Enums.CouponState.submit).OrderByDescending(_ => _.ID))
                {

                    //20181017 可以拿掉 (item.State == 2 || item.State == 6) && item.RefundState > 0 ? 7 :  //新增“退款中” 的状态
                    //int realState = (item.State == 2 || item.State == 6) && item.RefundState > 0 ? 7 : item.State;
                    int realState = item.State;

                    if (item.ExpireTime <= DateTime.Now.Date.AddDays(-1) && (realState == 2 || realState == 6))
                    {
                        realState = 8;//已过期（特殊状态） 出现在不能退的券客户未消费或者系统自动退款服务没有及时退款(显示的一种中间状态)
                    }
                    SKUInfoEntity sku = new SKUInfoEntity();
                    CouponOrderStepGroupEntity couponOderStepGroup = new CouponOrderStepGroupEntity();
                    try
                    {
                        sku = ProductAdapter.GetSKUInfoByID(item.SKUID);

                        string skuids = string.Join(",", sku.SKUList.Select(_ => _.ID)).ToString();
                        //获取skuproperty
                        List<ProductPropertyInfoEntity> ppiList = ProductAdapter.GetProductPropertyInfoBySKU(skuids);
                        ProductPropertyInfoEntity ppi = ppiList.Where(_ => _.SKUID == item.SKUID).FirstOrDefault();

                        #region 大团购

                        if (ppi != null && ppi.PropertyType == 6)
                        {
                            #region 定金产品
                            if (ppi.PropertyOptionTargetID == 1)
                            {
                                StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPU.ID);
                                ProductPropertyInfoEntity ppi2 = ppiList.Where(_ => _.PropertyType == 6 && _.PropertyOptionTargetID == 2).First();

                                if (item.State == 2)
                                {
                                    SKUEntity tailsku = sku.SKUList.Where(_ => _.ID == ppi2.SKUID).First();
                                    couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                    couponOderStepGroup.TailSKUID = tailsku.ID;
                                    couponOderStepGroup.DepositSKUID = item.SKUID;
                                    couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                    couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                    couponOderStepGroup.Price = tailsku.Price - sku.SKU.MarketPrice;//支付价格-膨胀金价格（MarketPrice是大团购的膨胀金）
                                    couponOderStepGroup.TotalPrice = tailsku.Price;// 当前sku的售卖价
                                    couponOderStepGroup.IsPayFinish = false;
                                    couponOderStepGroup.BookPosition = tailsku.BookPosition;
                                }
                                else if (item.State == 3)
                                {
                                    continue;
                                }
                                else if (item.State != 2 && item.State != 3)
                                {
                                    couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                    couponOderStepGroup.TailSKUID = 0;
                                    couponOderStepGroup.DepositSKUID = item.SKUID;
                                    couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                    couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                    couponOderStepGroup.Price = 0;//Price,尾款金额
                                    couponOderStepGroup.TotalPrice = 0;// 总支付金额
                                    couponOderStepGroup.IsPayFinish = false;
                                    couponOderStepGroup.BookPosition = 0;
                                }
                            }
                            #endregion
                            #region 非定金
                            else if (ppi.PropertyOptionTargetID == 2)
                            {
                                ProductPropertyInfoEntity ppi2 = ppiList.Where(_ => _.PropertyType == 6 && _.PropertyOptionTargetID == 1).First();
                                SKUEntity depositsku = sku.SKUList.Where(_ => _.ID == ppi2.SKUID).First();
                                StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPU.ID);

                                int orderCount = exchangelist.Where(_ => _.CouponOrderId == item.CouponOrderId).Count();
                                //判断单买，非单买的价格前段展示需要减掉膨胀金额
                                if (orderCount == 1)
                                {
                                    couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                    couponOderStepGroup.TailSKUID = item.SKUID;
                                    couponOderStepGroup.DepositSKUID = depositsku.ID;
                                    couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                    couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                    couponOderStepGroup.Price = item.OriPrice;
                                    couponOderStepGroup.TotalPrice = item.OriPrice;
                                    couponOderStepGroup.IsPayFinish = true;
                                    couponOderStepGroup.BookPosition = sku.SKU.BookPosition;
                                }
                                else
                                {
                                    //非定金产品 订单列表页 只展示已支付、已消费、已退款的产品
                                    if (item.State == 2 || item.State == 3 || item.State == 5)
                                    {
                                        couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                                        couponOderStepGroup.TailSKUID = item.SKUID;
                                        couponOderStepGroup.DepositSKUID = depositsku.ID;
                                        couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                                        couponOderStepGroup.StepGroupState = stg.StepGroupState;
                                        couponOderStepGroup.Price = item.OriPrice;
                                        if (item.State == 5 || realState == 7)
                                        {
                                            couponOderStepGroup.TotalPrice = item.OriPrice;//退款状态只显示实际支付尾款价格
                                        }
                                        else
                                        {
                                            couponOderStepGroup.TotalPrice = item.OriPrice + depositsku.VIPPrice;
                                        }
                                        couponOderStepGroup.IsPayFinish = true;
                                        couponOderStepGroup.BookPosition = sku.SKU.BookPosition;
                                    }
                                    else if (item.State != 2 && item.State != 3 && item.State != 5)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                        #endregion
                        else
                        {
                            couponOderStepGroup = null;
                        }
                        #endregion


                    }
                    catch (Exception e)
                    {
                        Log.WriteLog("GetSKUEntityByID 4:" + item.SKUID.ToString() + e);
                    }
                    GroupPurchaseEntity purchase = ProductAdapter.GetGroupPurchaseEntity(item.GroupId).FirstOrDefault();
                    //券的类型和套餐类型填一个值 增加一个待退款的中间状态
                    productCouponOrderList.Add(new ProductCouponOrderResult()
                    {
                        CouponOrderID = item.PayID,
                        CouponPrice = item.Price,
                        TotalPrice = item.Price,
                        TotalPoints = item.Points,
                        HotelName = item.ObjectName,
                        PackageName = item.CouponName,
                        ExchangeCouponList = CouponAdapter.TransExchangeCouponEntity2ExchangeCouponModel(new List<ExchangeCouponEntity> { item }),
                        State = realState,
                        CreateTime = item.CreateTime,
                        ExpireTime = Convert.ToDateTime(item.ExpireTime),
                        //SkuName = sku.Name != null ? sku.Name : "",
                        SkuName = (sku != null && sku.SKU != null) ? sku.SKU.Name : "",
                        SkuID = item.SKUID,
                        CategoryParentId = (sku != null && sku.Category != null) ? sku.Category.ParentID : 0,
                        SPUTempType = (sku != null && sku.Category != null) ? sku.Category.SPUTempType : 0,
                        Notice = string.IsNullOrWhiteSpace(item.Notice) ? new List<string>() : item.Notice.Trim().Split('_').ToList(),
                        DicProperties = StringHelper.ParseStringToDic(item.Properties),
                        GroupId = item.GroupId,
                        GroupPurchase = purchase,
                        CouponOrderStepGroup = couponOderStepGroup
                    });
                }
            }
            return new ProductCouponResult() { productCouponList = productCouponOrderList };
        }

        /// <summary>
        /// 根据userid和供应商id获取可以当天使用的订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="supplierId">供应商id</param>
        /// <returns></returns>
        [HttpGet]
        public List<CouponOrderEntity> GetUserOrderListBySupplierId(long userId, int supplierId)
        {
            List<CouponOrderEntity> OrderListItemEntity = new List<CouponOrderEntity>();
            List<ExchangeCouponEntity> couponOrderList = couponService.GetExchangeCouponListByUserIDSupplierId(userId, supplierId).Where(_ => _.RefundState == 0 && _.ExchangeMethod != 2
                && (_.BookPosition == 0 || (_.BookPosition > 0 && _.BookDate.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))) && Convert.ToDateTime(_.ExpireTime).AddDays(1) > DateTime.Now).ToList();//Convert.ToDateTime(_.ExpireTime).AddDays(1) 是因为数据库中存的是00：00：00

            var couponOrderListState2 = couponOrderList.Where(_ => _.State == 2).OrderByDescending(_ => _.ID).ToList();
            var couponOrderListState3 = couponOrderList.Where(_ => _.State == 3).OrderByDescending(_ => _.UpdateTime).ToList();

            //当天可使用的订单
            foreach (var item in couponOrderListState2)
            {
                CouponOrderEntity model = new CouponOrderEntity();
                model.OrderTypeIcon = OrderHelper.GetIcon(item.ParentID);
                model.ExchangeCouponID = item.ID;
                model.ExchangeNo = item.ExchangeNo;
                model.ExpireTime = Convert.ToDateTime(item.ExpireTime);
                model.OrderState = item.State;
                model.PageTitle = item.PageTitle;
                model.Price = item.Price;
                model.SKUID = item.SKUID;
                model.SKUName = item.SKUName;
                model.StartTime = item.StartTime;
                model.Points = item.Points;
                OrderListItemEntity.Add(model);
            }
            //追加当天核销的数据
            if (couponOrderListState3.Count() > 0)
            {
                string exchangenos = string.Join(",", couponOrderListState3.Select(_ => _.ExchangeNo));
                List<UsedConsumerCouponInfoEntity> usedConsumerlist = couponService.GetUsedCouponListByExchangeNos(exchangenos);
                foreach (var item in couponOrderListState3)
                {
                    int useConsumerCount = usedConsumerlist.Where(_ => _.ExchangeNo == item.ExchangeNo && _.CreateTime.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd")).Count();
                    if (useConsumerCount > 0)
                    {
                        CouponOrderEntity model = new CouponOrderEntity();
                        model.OrderTypeIcon = OrderHelper.GetIcon(item.ParentID);
                        model.ExchangeCouponID = item.ID;
                        model.ExchangeNo = item.ExchangeNo;
                        model.ExpireTime = Convert.ToDateTime(item.ExpireTime);
                        model.OrderState = item.State;
                        model.PageTitle = item.PageTitle;
                        model.Price = item.Price;
                        model.SKUID = item.SKUID;
                        model.SKUName = item.SKUName;
                        model.StartTime = item.StartTime;
                        model.Points = item.Points;
                        OrderListItemEntity.Add(model);
                    }
                }
            }

            return OrderListItemEntity;
            //return 
        }

        [HttpGet]
        public List<BookUserDateInfoEntity> GetBookedUserInfoByExchangid(int exchangeid)
        {
            return CouponAdapter.GetBookedUserInfoByExchangid(exchangeid);
        }

        [HttpGet]
        public List<BookUserDateInfoEntity> GetBookedUserInfoByExchangIds(string exchangeIds)
        {
            return CouponAdapter.GetBookedUserInfoByExchangIds(exchangeIds);
        }

        [HttpPost]
        public List<BookUserDateInfoEntity> GetPostBookedUserInfoByExchangIds(ExchangeCouponsParam param)
        {
            if (param != null && param.ExchangeIds != null)
            {
                string exchangeIds = param.ExchangeIds;
                return CouponAdapter.GetBookedUserInfoByExchangIds(exchangeIds);
            }
            else
            {
                return new List<BookUserDateInfoEntity>();
            }
        }

        [HttpGet]
        public int IsExchangeOrderCanPay(int couponOrderID)
        {
            return (int)CouponAdapter.IsExchangeOrderCanPay(couponOrderID).Success;
        }

        /// <summary>
        /// 支付成功后 更新兑换券的状态 ??? 有兑换券支付订单号 能否更新对应的兑换券
        /// 如果更新参数PayID不等于0 则将PayID一致的元素设置为支付完成
        /// </summary>
        /// <param name="state"></param>
        /// <param name="exchangeNo"></param>
        /// <returns></returns>
        [HttpPost]
        public int UpdateExchangeCoupon(ExchangeCouponEntity ece)
        {
            return CouponAdapter.UpdateExchangeCoupon(ece);
        }


        /// <summary>
        /// 插如新的兑换券记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public int InsertExchangeCoupon(ExchangeCouponEntity ece)
        {
            return CouponAdapter.InsertExchangeCoupon(ece);
        }

        /// <summary>
        /// 【小程序】提交房券
        /// </summary>
        /// <param name="scom"></param>
        /// <returns></returns>
        [HttpPost]
        public CouponOrderResult SubmitExchangeOrderForWxapp(SubmitCouponOrderForWxapp scom)
        {
            var param = new SubmitCouponOrderModel();
            param.ActivityID = scom.ActivityID;
            param.ActivityType = scom.ActivityType;
            param.UserID = scom.UserID;
            param.OrderItems = new List<ProductAndNum> { new ProductAndNum { SourceID = scom.SourceID, Number = scom.Number, Price = scom.Price, Type = scom.Type } };
            param.AddInfo = new ExchangeCouponAddInfoItem();
            param.SceneType = Convert.ToInt32(ProductServiceEnums.SceneType.WXAPP);

            return SubmitExchangeOrder(param);
        }

        /// <summary>
        /// 抢购/房券订单提交
        /// </summary>
        /// <param name="scom"></param>
        /// <returns></returns>
        [HttpPost]
        public CouponOrderResult SubmitExchangeOrder(SubmitCouponOrderModel scom)
        {
            if (scom.Flag > 0 && scom.OrderItems[0].Price == 0)
            {
                scom.OrderItems[0].SourceID = 62;
                scom.OrderItems[0].Type = 0;
                scom.OrderItems[0].Number = scom.Flag > 100 ? 1 : scom.Flag;
                scom.OrderItems[0].Price = 998;
            }

            if (scom.ActivityID == 0 || scom.UserID == 0 || scom.ActivityType == 0 || scom.OrderItems == null ||
                scom.OrderItems.Count == 0) return new CouponOrderResult() { Success = 1, Message = "关键参数丢失,请联系我们" };


            if (!CouponAdapter.AcquireLock(scom.ActivityID))
            {
                return new CouponOrderResult() { Success = 100, Message = "由于闪购人数超额，当前无法提交订单，稍后请再试。" };//"当前有很多用户提交,请稍后重新尝试！" };
            }

            var submitSum = scom.OrderItems != null ? scom.OrderItems.Sum(_ => _.Number) : 0;
            CouponOrderResult checkResult = CheckCouponSubmitNumberByCache(scom);

            CouponAdapter.ReleaseLock(scom.ActivityID);//验证完毕后 可以释放锁

            if (checkResult.Success != 0)
            {
                return checkResult;
            }

            if (scom.Flag != 110)
            {
                try
                {
                    List<int> exchangeCouponIdList = new List<int>();
                    var model = CouponAdapter.GetCouponActivityDetail(scom.ActivityID, false);//10min缓存
                    if (model.activity.IsValid == false) //前端不展示
                    {
                        return new CouponOrderResult() { Success = 101, Message = "房券不可售，请联系客服反馈问题，谢谢！" };//"当前有很多用户提交,请稍后重新尝试！" };       
                    }
                    else
                    {
                        var maxBuyNum = model.activity.SingleBuyNum;
                        var currentNum = ProductCache.GetCouponUserLockNumFromMemcache(scom.ActivityID, scom.UserID);
                        if (maxBuyNum < currentNum.HasLockedNum + submitSum && !IsTestPhone(scom.PhoneNo))
                        {
                            return new CouponOrderResult() { Success = 5, Message = "您累计提交的数量已经超过了最大购买数量" };//双重验证 避免缓存未及时更新导致验证漏掉
                        }

                        Decimal totalPrice = 0;
                        var priceOriginStr = model.activity.Price;
                        if (!string.IsNullOrWhiteSpace(priceOriginStr))
                        {
                            string[] priceStr = priceOriginStr.Split(",".ToCharArray(),
                                StringSplitOptions.RemoveEmptyEntries);
                            if (priceStr.Length > 0)
                            {
                                Decimal sellPrice = int.Parse(priceStr[0]);
                                sellPrice = sellPrice == 0 ? int.Parse(priceStr[1]) : sellPrice;
                                totalPrice += scom.OrderItems[0].Number * sellPrice;

                                scom.OrderItems[0].Price = sellPrice; //设置价格
                            }
                        }

                        User_Info ui = AccountAdapter.GetUserInfoByUserId(scom.UserID);

                        //提交订单ID
                        int couponOrderPayId = 0;
                        if (totalPrice > 0)
                        {
                            CommOrderEntity coe = new CommOrderEntity()
                            {
                                CustomID = scom.ActivityID,
                                IDX = 0,
                                Name = model.activity.Type == 400 ? "周末酒店VIP会员" : string.Format("{0}({1})", model.package.HotelName, model.package.PackageName),
                                OpNotice = null,
                                PhoneNum = string.IsNullOrWhiteSpace(scom.PhoneNo) ? ui.MobileAccount : scom.PhoneNo,
                                Price = totalPrice,
                                TypeID = scom.ActivityType
                            };
                            couponOrderPayId = CouponAdapter.InsertCommOrders(coe); //新建支付订单记录
                        }


                        //购买的SKU是否为大团购定金
                        bool curSKUIsStepGroupPrePay = false;
                        //获取sku属性
                        List<ProductPropertyInfoEntity> propertyInfo = ProductAdapter.GetProductPropertyInfoBySKU(scom.SKUID.ToString());
                        if (propertyInfo != null && propertyInfo.Count > 0)
                        {
                            ProductPropertyInfoEntity ppi = propertyInfo.First();
                            //判断是否是大团购 非大团购订单，一次提交多份只生成一个couponorderid,但大团购定金订单会生成多个CouponOrderID
                            if (ppi.PropertyType != 6 && scom.CouponOrderID == 0)
                            {
                                curSKUIsStepGroupPrePay = true;
                            }
                        }

                        //插入券记录
                        if (couponOrderPayId > 0)
                        {
                            //如果是支付大团购尾款，那么scom.CouponOrderID > 0， 尾款订单需要用这个CouponOrderID 
                            long firstCouponOrderId = scom.CouponOrderID > 0 ? scom.CouponOrderID : couponService.GetNextId(NextIdType.OrderID);
                            List<long> couponOrderIDList = new List<long>();

                            var SPUInfo = ProductAdapter.GetSPUBySKUID(scom.SKUID);
                            ExchangeCouponAddInfoItem addInfo = scom.AddInfo;
                            int CustomerType = (int)AccountAdapter.GetCustomerType(scom.UserID);
                            foreach (var productAndNum in scom.OrderItems)
                            {
                                for (int i = 0; i < productAndNum.Number; i++)
                                {
                                    long couponOrderID = (curSKUIsStepGroupPrePay == false ? firstCouponOrderId : couponService.GetNextId(NextIdType.OrderID)); //  如果是大团购定金，那么每份生成一个独立的couponorderid。
                                    couponOrderIDList.Add(couponOrderID);
                                    int couponId = CouponAdapter.InsertExchangeCoupon(new ExchangeCouponEntity()
                                    {
                                        PayID = couponOrderPayId,
                                        UserID = scom.UserID,
                                        CID = scom.CID,
                                        SKUID = scom.SKUID,
                                        PhoneNum =
                                            string.IsNullOrWhiteSpace(scom.PhoneNo) ? ui.MobileAccount : scom.PhoneNo,
                                        ActivityID = scom.ActivityID,
                                        ActivityType = scom.ActivityType,
                                        ExchangeNo = null,
                                        Price = productAndNum.Price,
                                        Type = productAndNum.Type,
                                        State = 1,
                                        CustomerType = CustomerType,
                                        InnerBuyGroup = i + 1,
                                        AddInfo = addInfo == null
                                            ? "||"
                                            : string.Format("{0}|{1}|{2}",
                                                string.IsNullOrWhiteSpace(addInfo.TrueName)
                                                    ? ""
                                                    : addInfo.TrueName,
                                                string.IsNullOrWhiteSpace(addInfo.PersonnelStructure)
                                                    ? ""
                                                    : addInfo.PersonnelStructure,
                                                string.IsNullOrWhiteSpace(addInfo.ChildrenAge)
                                                    ? ""
                                                    : addInfo.ChildrenAge),
                                        SupplierID = SPUInfo == null ? 0 : SPUInfo.SupplierID,
                                        CouponOrderId = couponOrderID
                                    });

                                    exchangeCouponIdList.Add(couponId);

                                }
                            }

                            //更新Redis缓存
                            couponOrderIDList.Distinct().ToList().ForEach(couponOrderID => OrderHelper.AddOrderToRedis(couponOrderID));

                        }
                        else
                        {
                            return new CouponOrderResult()
                            {
                                Success = 8,
                                Message = "房券订单提交失败",
                                OrderID = 0,
                                UserID = scom.UserID
                            };
                        }


                        return new CouponOrderResult()
                        {
                            Success = 0,
                            Message = "房券订单提交成功",
                            OrderID = couponOrderPayId,
                            UserID = scom.UserID,
                            PayChannels = OrderAdapter.genOrderPayChannels(HJDAPI.Common.Helpers.Enums.CustomerType.general, -1, scom.PhoneNo),
                            Amount = totalPrice,
                            GoodsInfo = model.activity.Type == 400 ? "周末酒店VIP会员" : string.Format("{0}({1})", model.package.HotelName, model.package.PackageBrief),
                            ExchangeIdList = exchangeCouponIdList
                        };
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("执行SubmitExchangeOrder发生错误，详细信息为:" + ex.Message + "\r\n" + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                return new CouponOrderResult() { Success = scom.Flag, Message = "测试并发直接返回" };
            }

        }

        private bool IsTestPhone(string p)
        {
            return p.StartsWith("100123123");
        }



        /// <summary>
        /// 【小程序】提交消费券
        /// </summary>
        /// <param name="scom"></param>
        /// <returns></returns>
        [HttpPost]
        public CouponOrderResult SubmitProductExchangeOrderForWxapp(SubmitCouponOrderForWxapp scom)
        {
            var param = new SubmitCouponOrderModel();
            param.SKUID = scom.SKUID;
            param.ActivityID = scom.ActivityID;
            param.ActivityType = scom.ActivityType;
            param.UserID = scom.UserID;
            param.OrderItems = new List<ProductAndNum> { new ProductAndNum { SourceID = scom.SourceID, Number = scom.Number, Price = scom.Price, Type = scom.Type } };
            param.AddInfo = new ExchangeCouponAddInfoItem();
            param.SceneType = Convert.ToInt32(ProductServiceEnums.SceneType.WXAPP);
            param.IsGroupActivity = scom.IsGroupActivity;
            param.GroupId = scom.GroupId;
            param.OpenId = scom.OpenId == null ? "" : scom.OpenId;
            param.UseCashCouponInfo.CashCouponID = scom.CashCouponID;
            param.UseCashCouponInfo.UseCashAmount = scom.CashCouponAmount;

            param.TemplateData = scom.TemplateData;
            param.TravelId = scom.TravelId;

            return SubmitExchangeOrderForProduct(param);
        }

        /// <summary>
        /// 消费券订单提交
        /// </summary>
        /// <param name="scom"></param>
        /// <returns></returns>
        [HttpPost]
        public CouponOrderResult SubmitExchangeOrderForProduct(SubmitCouponOrderModel scom)
        {
            if (scom.ActivityID == 0 || scom.SKUID == 0 || scom.UserID == 0 || scom.ActivityType == 0 || scom.OrderItems == null ||
                scom.OrderItems.Count == 0)
                return new CouponOrderResult() { Success = 1, Message = "关键参数丢失,请联系我们" };

            TimeLog log = new TimeLog(string.Format("SubmitExchangeOrderForProduct:ActivityID{0} SKUID{1} UserID{2}  ", scom.ActivityID, scom.SKUID, scom.UserID)
, 1000, null);


            if (!CouponAdapter.AcquireLock(scom.ActivityID))
            {
                return new CouponOrderResult() { Success = 100, Message = "由于购买人数超额，当前无法提交订单，稍后请再试。" };//"当前有很多用户提交,请稍后重新尝试！" };
            }
            log.AddLog("AcquireLock");

            //验证完毕后 可以释放锁  
            CouponOrderResult checkResult = CheckCouponSubmitNumberByCache(scom);
            CouponAdapter.ReleaseLock(scom.ActivityID);

            log.AddLog("CheckCouponSubmitNumberByCache");
            if (checkResult.Success != 0)
            {
                return checkResult;
            }
            var submitSum = scom.OrderItems.Sum(_ => _.Number);

            #region 判断是否参与过拼团
            List<ProductPropertyInfoEntity> propertyInfo = ProductAdapter.GetProductPropertyInfoBySKU(scom.SKUID.ToString());

            //判断是否是拼团
            int num = propertyInfo.FindAll(_ => _.PropertyType == (int)Enums.PropertyType.拼团 && _.PropertyOptionTargetID == 2).Count();

            //CouponAdapter.GetExchangeCouponListByUserSKU()

            //判断有没有参与过此产品的拼团记录(取进行中和已成功的数据)
            List<ExchangeCouponEntity> exList = CouponAdapter.GetExchangeCouponListByUserIDAndSKU(scom.UserID, scom.SKUID);
            //int count = ProductAdapter.GetGroupPurchaseCountBySkuAndUserid(scom.SKUID, scom.UserID).FindAll(_ => _.State == 0 || _.State == 1).Count;

            int count = exList.FindAll(_ => _.State == 2 && _.RefundState == 0).Count;
            if (num > 0 && count > 0)
            {
                return new CouponOrderResult() { Success = 1, Message = "该团购价限购买一次，拼团中或拼团成功暂不能重复参与。" };
            }
            #endregion
            if (scom.Flag != 110)
            {
                try
                {
                    List<int> exchangeCouponIdList = new List<int>();
                    //获取消费券的信息
                    var productModel = CouponAdapter.GetSKUCouponActivityDetail(scom.SKUID);
                    log.AddLog("GetSKUCouponActivityDetail");
                    var SKUInfo = productModel.SKUInfo;
                    var SPUInfo = ProductAdapter.GetSPUBySKUID(scom.SKUID);
                    //单人最大购买数量
                    var maxBuyNum = productModel.activity.SingleBuyNum;

                    //双重验证 避免缓存未及时更新导致验证漏掉
                    var currentNum = ProductCache.GetCouponUserLockNumFromMemcache(scom.ActivityID, scom.UserID);
                    log.AddLog("GetCurrentUserNumAndLockNum");
                    //if (maxBuyNum < currentNum.HasLockedNum + submitSum)
                    //{
                    //    return new CouponOrderResult() { Success = 5, Message = "您累计提交的数量已经超过了最大购买数量" };
                    //}

                    //当前购买用户信息
                    User_Info ui = AccountAdapter.GetUserInfoByUserId(scom.UserID);
                    log.AddLog("GetUserInfoByUserId");

                    //获取用户是否VIP
                    var isVip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(scom.UserID));
                    log.AddLog("IsVIPCustomer");


                    ProductAndNum productAndNum = scom.OrderItems[0];


                    #region 设置价格(根据用户身份&Markdown规则计算结算价)

                    var productCheckPrice = ProductAdapter.CheckProductPromotionForCoupon(scom.SKUID, productAndNum.Number, scom.UserID, (ProductServiceEnums.SceneType)scom.SceneType);
                    List<PromotionCheckItemPriceEntity> priceList = isVip ? productCheckPrice.SellVIPPriceItemList : productCheckPrice.SellPriceItemList;
                    decimal totalPrice = priceList.Sum(_ => _.Price);
                    decimal totalPoints = productCheckPrice.SellPoints;
                    decimal totalDiscount = scom.UseCashCouponInfo.UseCashAmount;
                    decimal totalUserUseHousingFundAmount = scom.UserUseHousingFundAmount;
                    decimal totalVoucherAmount = scom.UseVoucherInfo.UseVoucherAmount;

                    decimal totalRetailPrice = scom.OrderItems.Sum(_ => _.Number * _.RetailPrice);
                    decimal unionRetailPrice = scom.OrderItems.First().RetailPrice;
                    List<PromotionCheckItemPointsEntity> pointsList = productCheckPrice.SellPointsItemList;

                    log.AddLog("设置价格");

                    #endregion

                    #region check points

                    if (totalPoints > 0)
                    {
                        GetInspectorHotelsListParam lp = new GetInspectorHotelsListParam { userid = scom.UserID };

                        PointResult userPointsInfo = PointsAdapter.GetPersonalPoint(scom.UserID);

                        if (userPointsInfo.CanUsePoints < totalPoints)
                        {
                            return new CouponOrderResult() { Success = 200, Message = "抱歉，当前账户积分不足。" };
                        }
                    }

                    #endregion


                    //提交订单ID
                    int couponOrderPayId = 0;
                    Decimal commOrderPrice = totalRetailPrice > 0 ? totalRetailPrice : (totalPrice - totalDiscount - totalUserUseHousingFundAmount - totalVoucherAmount);

                    decimal deductiblePrice = 0;

                    //购买的SKU是否为大团购定金
                    bool curSKUIsStepGroupPrePay = false;


                    try
                    {
                        ProductPropertyInfoEntity ppi = propertyInfo.First();
                        //判断是否是大团购 非大团购订单，一次提交多份只生成一个couponorderid。
                        if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 1)
                        {
                            curSKUIsStepGroupPrePay = true;
                        }

                        //判断当前SKU是否是大团购产品 非定金产品
                        if (ppi.PropertyType == 6 && ppi.PropertyOptionTargetID == 2 && scom.CouponOrderID > 0)
                        {
                            SKUEntity depositSKU = productModel.SKUInfo.SKUList.Where(_ => _.IsDepositSKU == true).First();
                            deductiblePrice = depositSKU.MarketPrice;

                            commOrderPrice = commOrderPrice - deductiblePrice;
                            //List<ProductPropertyInfoEntity> propertyList = ProductAdapter.GetProductPropertyInfoBySKU(string.Join(",", productModel.SKUInfo.SKUList.Select(_ => _.ID)).ToString());
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    CommOrderEntity coe = new CommOrderEntity()
                    {
                        CustomID = scom.ActivityID,
                        IDX = 0,
                        Name = string.Format("{0}({1})", productModel.activity.PageTitle, productModel.SKUInfo.SKU.Name),
                        OpNotice = null,
                        PhoneNum = string.IsNullOrWhiteSpace(scom.PhoneNo) ? ui.MobileAccount : scom.PhoneNo,
                        Price = commOrderPrice,
                        Points = totalPoints,
                        TypeID = scom.ActivityType
                    };
                    couponOrderPayId = CouponAdapter.InsertCommOrders(coe); //新建支付订单记录

                    log.AddLog("提交订单ID");

                    //插入券记录
                    if (couponOrderPayId > 0)
                    {
                        #region 插入拼团

                        int groupid = 0;
                        if (num > 0 && count == 0)
                        {
                            //取消未支付状态的拼团
                            List<ExchangeCouponEntity> nopayList = exList.Where(_ => _.State == 40).ToList();
                            foreach (ExchangeCouponEntity item in nopayList)
                            {
                                GroupPurchaseEntity gp = ProductAdapter.GetGroupPurchaseEntity(item.GroupId).FirstOrDefault();
                                if (gp.State == (int)Enums.GroupState.未支付)
                                {
                                    ProductAdapter.UpdateGroupPurchase(gp.ID, (int)Enums.GroupState.已取消);
                                }
                            }

                            //sku属性选项是拼团 并且参数是0  则插入团购表
                            if (scom.GroupId == 0)
                            {
                                CouponActivityEntity couponActivityModel = CouponAdapter.GetCouponActivityEntity(scom.ActivityID);
                                //添加团购
                                GroupPurchaseEntity groupmodel = new GroupPurchaseEntity();
                                groupmodel.SKUID = scom.SKUID;
                                groupmodel.State = (int)Enums.GroupState.未支付;
                                groupmodel.CreatTime = System.DateTime.Now;
                                groupmodel.EndTime = DateTime.Now.AddHours(couponActivityModel.GroupDay) > couponActivityModel.ExpireTime ?
                                                         couponActivityModel.ExpireTime : DateTime.Now.AddHours(couponActivityModel.GroupDay);//拼团结束时间大于过期时间，则取过期时间


                                groupmodel.UserID = scom.UserID;
                                groupmodel.OpenId = string.IsNullOrEmpty(scom.OpenId) ? "" : scom.OpenId;
                                groupid = ProductAdapter.AddGroupPurchase(groupmodel);
                            }
                            else if (scom.GroupId > 0)
                            {
                                groupid = scom.GroupId;
                                GroupPurchaseEntity group = ProductAdapter.GetGroupPurchaseEntity(scom.GroupId).FirstOrDefault();
                                if (group.ID > 0 && (group.State == (int)Enums.GroupState.失败 || group.State == (int)Enums.GroupState.未支付 || group.State == (int)Enums.GroupState.已取消))
                                {
                                    log.AddLog("团购失败  groupid：" + scom.GroupId + "。 userid：" + scom.UserID);
                                    return new CouponOrderResult()
                                    {
                                        Success = 8,
                                        Message = "订单提交失败,该团不存在",
                                        OrderID = 0,
                                        UserID = scom.UserID
                                    };
                                }
                            }
                        }
                        #endregion
                        int CustomerType = (int)AccountAdapter.GetCustomerType(scom.UserID);

                        ExchangeCouponAddInfoItem addInfo = scom.AddInfo;

                        List<Decimal> useCashCouponAmountList = CommMethods.AvgAmount(scom.UseCashCouponInfo.UseCashAmount, productAndNum.Number);
                        List<Decimal> useVoucherAmountList = CommMethods.AvgAmount(scom.UseVoucherInfo.UseVoucherAmount, productAndNum.Number);
                        List<Decimal> UserUseHousingFundAmountList = CommMethods.AvgAmount(scom.UserUseHousingFundAmount, productAndNum.Number);

                        //如果是支付大团购尾款，那么scom.CouponOrderID > 0， 尾款订单需要用这个CouponOrderID 
                        long firstCouponOrderId = scom.CouponOrderID > 0 ? scom.CouponOrderID : couponService.GetNextId(NextIdType.OrderID);

                        List<long> couponOrderIDList = new List<long>();

                        for (int i = 0; i < productAndNum.Number; i++)
                        {
                            long couponOrderID = (curSKUIsStepGroupPrePay == false ? firstCouponOrderId : couponService.GetNextId(NextIdType.OrderID)); //  如果是大团购定金，那么每份生成一个独立的couponorderid。
                            couponOrderIDList.Add(couponOrderID);

                            var OriPrice = priceList[i].OriPrice;
                            var Price = priceList[i].Price;
                            if (deductiblePrice > 0)
                            {
                                OriPrice = OriPrice - deductiblePrice;
                                Price = Price - deductiblePrice;
                            }
                            ExchangeCouponEntity ec = new ExchangeCouponEntity()
                            {
                                PayID = couponOrderPayId,
                                UserID = scom.UserID,
                                CID = scom.CID,
                                SKUID = scom.SKUID,
                                CashCouponID = scom.UseCashCouponInfo.CashCouponID,
                                CashCouponAmount = useCashCouponAmountList[i],
                                VoucherIDs = string.Join(",", scom.UseVoucherInfo.UseVoucherIDList),
                                VoucherAmount = useVoucherAmountList[i],
                                UserUseHousingFundAmount = UserUseHousingFundAmountList[i],
                                PhoneNum =
                                    string.IsNullOrWhiteSpace(scom.PhoneNo) ? ui.MobileAccount : scom.PhoneNo,
                                ActivityID = scom.ActivityID,
                                ActivityType = scom.ActivityType,
                                ExchangeNo = null,
                                Points = pointsList[i].Points,
                                Price = unionRetailPrice > 0 ? unionRetailPrice : Price,//priceList[i].Price,
                                OriPrice = OriPrice,//priceList[i].OriPrice,
                                PromotionID = priceList[i].PromotionID,
                                SettlePrice = SKUInfo.SKU.PayForSupplier,
                                Type = productAndNum.Type,
                                State = (int)ExchangeCouponState.submit,
                                GroupId = groupid,
                                CustomerType = CustomerType,
                                InnerBuyGroup = i + 1,
                                OperationState = SKUInfo.SKU.FollowOperation == 2 ? 1 : SKUInfo.SKU.FollowOperation,// 原第三方下单 的后续操作为2。现在已没有这个状态
                                TraveIDs = string.Join(",", scom.TravelId == null ? new List<int>() : scom.TravelId),//出行人id
                                AddInfo = addInfo == null
                                    ? "||"
                                    : string.Format("{0}|{1}|{2}",
                                        string.IsNullOrWhiteSpace(addInfo.TrueName)
                                            ? ""
                                            : addInfo.TrueName,
                                        string.IsNullOrWhiteSpace(addInfo.PersonnelStructure)
                                            ? ""
                                            : addInfo.PersonnelStructure,
                                        string.IsNullOrWhiteSpace(addInfo.ChildrenAge)
                                            ? ""
                                            : addInfo.ChildrenAge),
                                FromWeixinUid = scom.FromWeixinUid,
                                PhotoUrl = scom.PhotoUrl,
                                SupplierID = SPUInfo == null ? 0 : SPUInfo.SupplierID,
                                CouponOrderId = couponOrderID
                            };

                            int ecID = CouponAdapter.InsertExchangeCoupon(ec);
                            exchangeCouponIdList.Add(ecID);

                            if (scom.TravelId != null)
                            {
                                List<TravelPersonEntity> travelList = HotelAdapter.GetTravelPersonByIDS(string.Join(",", scom.TravelId));
                                foreach (var travel in travelList)
                                {
                                    CouponOrderPersonEntity cop = new CouponOrderPersonEntity();
                                    cop.TravelPersonName = travel.TravelPersonName;
                                    cop.TravelPersonCardType = travel.IDType;
                                    cop.TravelPersonCardNo = travel.IDNumber;
                                    cop.ExchangeID = ecID;
                                    CouponAdapter.AddCouponOrderPerson(cop);
                                }
                            }

                            //下单时需要填写 信息
                            if (SKUInfo.SKU.WriteOtherPostion == 1 && scom.TemplateData != null)
                            {
                                scom.TemplateData.BizId = ecID;
                                Addtemplate(scom.TemplateData);
                            }
                        }

                        //更新Redis缓存
                        couponOrderIDList.Distinct().ToList().ForEach(couponOrderID => OrderHelper.AddOrderToRedis(couponOrderID));

                    }
                    else
                    {
                        return new CouponOrderResult()
                        {
                            Success = 8,
                            Message = "订单提交失败",
                            OrderID = 0,
                            UserID = scom.UserID
                        };
                    }
                    log.AddLog("插入券记录");

                    log.Finish();

                    return new CouponOrderResult()
                    {
                        Success = 0,
                        Message = "订单提交成功",
                        OrderID = couponOrderPayId,
                        UserID = scom.UserID,
                        PayChannels = OrderAdapter.genOrderPayChannels(HJDAPI.Common.Helpers.Enums.CustomerType.general, -1, scom.PhoneNo),
                        Amount = (int)totalPrice,
                        GoodsInfo = string.Format("{0}({1})", productModel.activity.PageTitle, SKUInfo.SKU.Name),
                        ExchangeIdList = exchangeCouponIdList
                    };
                }
                catch (Exception ex)
                {
                    Log.WriteLog("执行SubmitExchangeOrderForProduct发生错误，详细信息为:" + ex.Message + "\r\n" + ex.StackTrace);
                    throw;
                }

            }
            else
            {
                return new CouponOrderResult() { Success = scom.Flag, Message = "测试并发直接返回" };
            }
        }

        [HttpGet]
        public long GetNextId()
        {
            return couponService.GetNextId(NextIdType.OrderID);
        }

        #region 大团购支付尾款

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skuIdTag1">定金skuid</param>
        /// <param name="skuIdTag2">尾款skuid</param>
        /// <param name="userId"></param>
        /// <param name="CID"></param>
        /// <param name="price"></param>
        /// <param name="couponOrderID"></param>
        /// <returns></returns>
        [HttpGet]
        public CouponOrderResult SubmitStepGroupOrder(int depositSKUID, int tailSKUID, long userId, decimal price, long couponOrderID)
        {
            CouponOrderResult exCoupon = null;

            List<int> exchangeCouponIdList = new List<int>();
            //SubmitCouponOrderModel submitParam = new SubmitCouponOrderModel();
            if (userId > 0 && depositSKUID > 0 && tailSKUID > 0 && couponOrderID > 0)
            {
                SKUCouponActivityDetailModel skuInfo = CouponAdapter.GenSKUCouponActivityDetail(depositSKUID);

                SKUEntity sku1 = skuInfo.SKUInfo.SKUList.Where(_ => _.ID == depositSKUID).First();
                SKUEntity sku2 = skuInfo.SKUInfo.SKUList.Where(_ => _.ID == tailSKUID).First();

                ProductPropertyEntity pdp = skuInfo.SKUInfo.SPU.PropertyList.First();
                //判断是否是大团购
                if (pdp.Type == 6)
                {
                    //产品是否可售状态
                    if (skuInfo.activityOpenState == 1)
                    {
                        //sku2.Price 正常售价，sku1.Price 定金可抵扣的金额
                        if (price == sku2.Price - sku1.MarketPrice)
                        {
                            List<ExchangeCouponEntity> exList = CouponAdapter.GetExchangCouponByCouponOrderID(couponOrderID).Where(_ => _.State == 1 || _.State == 2).ToList();
                            if (exList.Count == 1)
                            {
                                ExchangeCouponEntity ex = exList.First();
                                if (ex.State == 2)
                                {
                                    //提交订单ID
                                    int couponOrderId = 0;
                                    Decimal commOrderPrice = price;
                                    CommOrderEntity coe = new CommOrderEntity()
                                    {
                                        CustomID = ex.ActivityID,
                                        IDX = 0,
                                        Name = string.Format("{0}({1})", skuInfo.activity.PageTitle, skuInfo.SKUInfo.SKU.Name),
                                        OpNotice = null,
                                        PhoneNum = ex.PhoneNum,
                                        Price = commOrderPrice,
                                        Points = 0,
                                        TypeID = ex.ActivityType
                                    };
                                    couponOrderId = CouponAdapter.InsertCommOrders(coe); //新建支付订单记录

                                    ExchangeCouponEntity ec = new ExchangeCouponEntity()
                                    {
                                        PayID = couponOrderId,
                                        UserID = userId,
                                        CID = ex.CID,
                                        SKUID = tailSKUID,
                                        CashCouponID = depositSKUID,
                                        CashCouponAmount = sku1.MarketPrice,//大团购的价格，在这当作抵扣金额
                                        VoucherIDs = "",
                                        VoucherAmount = 0,
                                        UserUseHousingFundAmount = sku1.VIPPrice, //大团购的定金
                                        PhoneNum = ex.PhoneNum,
                                        ActivityID = ex.ActivityID,
                                        ActivityType = ex.ActivityType,
                                        ExchangeNo = null,
                                        Points = 0,
                                        Price = sku2.Price,//正常售卖价
                                        OriPrice = price,//减去定金抵扣价
                                        PromotionID = ex.PromotionID,
                                        SettlePrice = ex.SettlePrice,
                                        Type = ex.Type,
                                        State = (int)ExchangeCouponState.submit,
                                        GroupId = ex.GroupId,
                                        CustomerType = ex.CustomerType,
                                        InnerBuyGroup = ex.InnerBuyGroup,
                                        OperationState = sku2.FollowOperation == 2 ? 1 : sku2.FollowOperation,// 原第三方下单 的后续操作为2。现在已没有这个状态
                                        TraveIDs = ex.TraveIDs,//出行人id
                                        AddInfo = ex.AddInfo,
                                        FromWeixinUid = ex.FromWeixinUid,
                                        PhotoUrl = ex.PhotoUrl,
                                        SupplierID = ex.SupplierID,
                                        CouponOrderId = couponOrderID
                                    };

                                    int ecID = CouponAdapter.InsertExchangeCoupon(ec);
                                    exchangeCouponIdList.Add(ecID);

                                    //更新Redis缓存
                                    OrderHelper.AddOrderToRedis(ec.CouponOrderId);

                                    exCoupon = new CouponOrderResult()
                                    {
                                        Success = 0,
                                        Message = "订单提交成功",
                                        OrderID = couponOrderId,
                                        UserID = userId,
                                        PayChannels = OrderAdapter.genOrderPayChannels(HJDAPI.Common.Helpers.Enums.CustomerType.general, -1, ex.PhoneNum),
                                        Amount = (int)price,
                                        GoodsInfo = string.Format("{0}({1})", skuInfo.activity.PageTitle, sku2.Name),
                                        ExchangeIdList = exchangeCouponIdList
                                    };
                                }
                            }
                            else if (exList.Count == 2)//大团购如果存在两个相同的couponOrderID，则认为已经创建尾款订单
                            {
                                ExchangeCouponEntity ex = exList.Where(_ => _.State == 1).First();
                                List<int> idlist = new List<int>();
                                idlist.Add(ex.ID);
                                exCoupon = new CouponOrderResult()
                                {
                                    Success = 0,
                                    Message = "订单提交成功",
                                    OrderID = ex.PayID,
                                    UserID = userId,
                                    PayChannels = OrderAdapter.genOrderPayChannels(HJDAPI.Common.Helpers.Enums.CustomerType.general, -1, ex.PhoneNum),
                                    Amount = (int)ex.OriPrice,
                                    GoodsInfo = string.Format("{0}({1})", skuInfo.activity.PageTitle, sku2.Name),
                                    ExchangeIdList = idlist
                                };
                            }
                            else
                            {
                                exCoupon = new CouponOrderResult()
                                {
                                    Success = -4,
                                    Message = "订单提交失败",
                                    OrderID = 0,
                                    UserID = userId,
                                    PayChannels = new List<string>(),
                                    Amount = (int)0,
                                    GoodsInfo = "",
                                    ExchangeIdList = new List<int>()
                                };
                            }
                        }
                        else
                        {
                            //价格不一致
                            exCoupon = new CouponOrderResult()
                            {
                                Success = -3,
                                Message = "价格有误，订单提交失败",
                                OrderID = 0,
                                UserID = userId,
                                PayChannels = new List<string>(),
                                Amount = (int)0,
                                GoodsInfo = "",
                                ExchangeIdList = new List<int>()
                            };
                        }
                    }
                    else if (skuInfo.activityOpenState == 0)
                    {
                        exCoupon = new CouponOrderResult()
                        {
                            Success = -1,
                            Message = "已售完，订单提交失败",
                            OrderID = 0,
                            UserID = userId,
                            PayChannels = new List<string>(),
                            Amount = (int)0,
                            GoodsInfo = "",
                            ExchangeIdList = new List<int>()
                        };
                        //
                    }
                    else if (skuInfo.activityOpenState == 2)
                    {
                        //已结束
                        exCoupon = new CouponOrderResult()
                        {
                            Success = -2,
                            Message = "已结束，订单提交失败",
                            OrderID = 0,
                            UserID = userId,
                            PayChannels = new List<string>(),
                            Amount = (int)0,
                            GoodsInfo = "",
                            ExchangeIdList = new List<int>()
                        };
                    }
                }

            }
            else
            {
                //已结束
                exCoupon = new CouponOrderResult()
                {
                    Success = -2,
                    Message = "关键参数丢失，订单提交失败",
                    OrderID = 0,
                    UserID = userId,
                    PayChannels = new List<string>(),
                    Amount = (int)0,
                    GoodsInfo = "",
                    ExchangeIdList = new List<int>()
                };
            }

            return exCoupon;
        }

        #endregion



        /// <summary>
        /// 团购房券
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public CouponOrderResult SubmitGroupOrderForProduct(SubmitCouponOrderModel scom)
        {
            scom.IsGroupActivity = true;
            return SubmitExchangeOrderForProduct(scom);
        }

        /// <summary>
        /// 提交集赞
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResult SubmitSetLike(string openid, int groupId, int activityID, long userid = 0)
        {
            BaseResult result = new BaseResult();
            result.Message = "关键参数丢失";
            result.RetCode = "0";
            if (!string.IsNullOrEmpty(openid) && groupId > 0)
            {
                GroupPurchaseEntity GroupPurchaseEntity = ProductAdapter.GetGroupPurchaseEntity(groupId).FirstOrDefault();
                if (GroupPurchaseEntity == null || GroupPurchaseEntity.EndTime < System.DateTime.Now)
                {
                    var _msg = "好友助力时间已过，助力未成功。";
                    if (GroupPurchaseEntity != null && GroupPurchaseEntity.SKUID > 0)
                    {
                        var _link = string.Format("http://www.zmjiudian.com/coupon/group/product/{0}/0", GroupPurchaseEntity.SKUID);
                        _msg = string.Format("好友助力时间已过，助力未成功。我们为你准备了免费的礼品，点击领取→ {0}", _link);
                    }
                    result.Message = _msg;
                    result.RetCode = "0";
                }
                else
                {
                    List<GroupPurchaseEntity> GroupPurchaseList = ProductAdapter.GetGroupPurchaseByOpenID(openid, GroupPurchaseEntity.SKUID);
                    bool IsExist = GroupPurchaseList.Exists(_ => _.State == 0 || _.State == 1);
                    if (IsExist)
                    {
                        result.Message = "你已参与过该活动，不能重复参与";
                        result.RetCode = "0";
                    }
                    else
                    {
                        GroupPurchaseDetailEntity detailmodel = new GroupPurchaseDetailEntity();
                        detailmodel.IsSponsor = false;
                        detailmodel.UserId = userid;
                        detailmodel.JoinTime = System.DateTime.Now;
                        detailmodel.GroupId = groupId;
                        detailmodel.OpenId = openid;
                        ProductAdapter.AddGroupPurchaseDetail(detailmodel);

                        result.Message = "你已成功参与";
                        result.RetCode = "1";

                        int groupNum = ProductAdapter.GetGroupPurchaseDetailCountByGroupId(groupId);
                        //根据activeid获取成团数量
                        CouponActivityEntity couponActivity = ProductCache.GetOneCouponActivity(activityID);
                        if (groupNum == couponActivity.GroupCount || (groupNum > couponActivity.GroupCount && GroupPurchaseEntity.State == 0))
                        {

                            ProductAdapter.UpdateGroupPurchase(groupId, (int)Enums.GroupState.成功);
                            AddGroupProductPromotionProductByGroupID(groupId);//成功后根据团ID给已提交的团订单补发促销产品
                            try
                            {
                                ExchangeCouponEntity ex = CouponAdapter.GetExchangeCouponEntityListByGroupId(groupId, 0).FirstOrDefault();
                                if (ex != null && ex.PhoneNum != "")
                                {
                                    string couponType = "消费券";
                                    if (ex.ActivityType == 200)
                                    {
                                        couponType = "房券";
                                    }
                                    //如果自定义短信,发自定义短信内容
                                    SKUEntity skuEntity = ProductAdapter.GetSKUEXEntityByID(ex.SKUID);
                                    if (skuEntity != null && skuEntity.SMSType == 2)
                                    {
                                        SMServiceController.SendSMS(ex.PhoneNum, skuEntity.SMSConten);
                                    }
                                    else
                                    {
                                        if (ex.WeixinAcountId == 11)
                                        {
                                            SMServiceController.SendSMS(ex.PhoneNum, "恭喜您的拼团成功！请在遛娃指南服务号，我的>>我的订单>>" + couponType + "内查看详情及使用方式。");
                                        }
                                        else
                                        {
                                            SMServiceController.SendSMS(ex.PhoneNum, "恭喜您的拼团成功！请在周末酒店APP或周末酒店微信服务号，我的>>我的订单>>" + couponType + "内查看详情及使用方式。");
                                        }
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLog("短信发送异常" + ex.Message);
                            }
                        }
                    }
                }
            }


            return result;
        }

        /// <summary>
        /// 检查当前SKU下集赞产品。集赞人数已达标 团状态还是进行中的团  对该团进行拼团成功
        /// </summary>
        /// <param name="skuid"></param>
        /// <returns></returns>
        [HttpGet]
        public string CheckSetLikeGroupState(int skuid)
        {
            string result = "";
            int i = 0;
            List<GroupPurchaseEntity> groupList = ProductAdapter.GetGroupPurchaseContainCABySKUID(skuid).Where(_ => _.State != 1).ToList();
            //如果自定义短信,发自定义短信内容
            SKUEntity skuEntity = ProductAdapter.GetSKUEXEntityByID(skuid);
            foreach (GroupPurchaseEntity item in groupList)
            {
                if (item.GroupSuccessCount <= item.GroupPeopleCount)
                {
                    ProductAdapter.UpdateGroupPurchase(item.ID, (int)Enums.GroupState.成功);
                    AddGroupProductPromotionProductByGroupID(item.ID);//成功后根据团ID给已提交的团订单补发促销产品
                    try
                    {
                        ExchangeCouponEntity ex = CouponAdapter.GetExchangeCouponEntityListByGroupId(item.ID, 0).FirstOrDefault();
                        if (ex != null && ex.PhoneNum != "")
                        {
                            string couponType = "消费券";
                            if (ex.ActivityType == 200)
                            {
                                couponType = "房券";
                            }
                            if (skuEntity != null && skuEntity.SMSType == 2)
                            {
                                SMServiceController.SendSMS(ex.PhoneNum, skuEntity.SMSConten);
                            }
                            else
                            {
                                if (ex.WeixinAcountId == 11)
                                {
                                    SMServiceController.SendSMS(ex.PhoneNum, "恭喜您的拼团成功！请在遛娃指南服务号，我的>>我的订单>>" + couponType + "内查看详情及使用方式。");
                                }
                                else
                                {
                                    SMServiceController.SendSMS(ex.PhoneNum, "恭喜您的拼团成功！请在周末酒店APP或周末酒店微信服务号，我的>>我的订单>>" + couponType + "内查看详情及使用方式。");
                                }
                            }

                        }
                        i++;
                        result += "," + item.ID;
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("短信发送异常" + ex.Message);
                    }
                }
            }
            return "批量处理" + i + "条。团id（groupid）：" + result;
        }

        /// <summary>
        /// 检测券的状态
        /// </summary>
        /// <returns></returns>
        public CouponOrderResult CheckCouponSubmitNumberByCache(SubmitCouponOrderModel scom, TimeLog parentLog = null)
        {
            TimeLog log = new TimeLog("CheckCouponSubmitNumberByCache", 500, parentLog);
            CouponOrderResult ret = new CouponOrderResult() { Success = 0, Message = "可以继续往下走" };
            int submitSum = 0;
            if (scom == null || scom.ActivityID == 0 || scom.ActivityType == 0 || scom.UserID == 0 ||
                scom.OrderItems == null || !scom.OrderItems.Any())
            {
                ret = new CouponOrderResult() { Success = 1, Message = "关键参数丢失,请联系我们" };
            }
            else
            {
                //本次用户提交购买数
                submitSum = scom.OrderItems.Sum(_ => _.Number);

                var skuInfoEntity = ProductAdapter.GetSKUItemByID(scom.SKUID);

                var couponInfo = ProductCache.CouponSellInfoMem(scom.ActivityID);//活动其他数量  

                if (couponInfo == null)
                {
                    ret = new CouponOrderResult() { Success = 2, Message = "参数错误,没有找到活动数据" };
                }
                else if (couponInfo.EffectiveTime > DateTime.Now)
                {
                    ret = new CouponOrderResult() { Success = 18, Message = "活动尚未开始，请耐心等候" };
                }
                else if (couponInfo.SaleEndDate < DateTime.Now && scom.IsGroupActivity == false)//团购 活动过期后 小团未过期仍可以提交
                {
                    ret = new CouponOrderResult() { Success = 19, Message = "活动已结束" };
                }
                else if (couponInfo.SellNum + couponInfo.ManuSellNum >= couponInfo.TotalNum)
                {
                    ret = new CouponOrderResult() { Success = 3, Message = "本次活动已经全部售完，谢谢您的关注" };
                }

                else if (submitSum < skuInfoEntity.MinBuyCount)
                {
                    ret = new CouponOrderResult() { Success = 18, Message = "您本次提交的数量低于最小购买数量" };
                }
                else if (submitSum > skuInfoEntity.MaxBuyCount)
                {
                    ret = new CouponOrderResult() { Success = 12, Message = "您本次提交的数量高于最大购买数量" };
                }
                else
                {

                    var couponLockedResult = ProductCache.GetCouponLockNumFromMemcache(scom.ActivityID);

                    var userLockedInfo = ProductCache.GetCouponUserLockNumFromMemcache(scom.ActivityID, scom.UserID);//个人提交数量
                    var userAddLockedNum = submitSum - userLockedInfo.NotPaiedCount;
                    int payingCount = couponLockedResult.ActivityLockNum - couponInfo.SellNum - couponInfo.ManuSellNum;
                    if (couponLockedResult.ActivityLockNum - userLockedInfo.NotPaiedCount >= couponInfo.TotalNum)
                    {
                        ret = new CouponOrderResult() { Success = 4, Message = string.Format("本次活动剩余数量已被锁定，有{0}份订单等待客人付款中,稍后再来碰碰运气吧", payingCount) };
                    }
                    else if (couponLockedResult.ActivityLockNum + userAddLockedNum > couponInfo.TotalNum)
                    {
                        int restCanBuyCount = couponInfo.TotalNum - couponLockedResult.ActivityLockNum;
                        ret = new CouponOrderResult()
                        {
                            Success = 4,
                            Message =
                                string.Join("", new List<string>{
                                    "现在本次活动剩余可购数量只有" + restCanBuyCount.ToString() + "份了。",
                                    payingCount>0? string.Format("有{0}份订单在待客人付款中,稍后再来碰碰运气吧",payingCount):""})
                        };
                    }
                    else
                    {
                        log.AddLog("GetCurrentUserNumAndLockNumFromMemcache");
                        if (couponInfo.MaxBuyNum < userLockedInfo.HasLockedNum + userAddLockedNum && !IsTestPhone(scom.PhoneNo == null ? "" : scom.PhoneNo))
                        {
                            ret = new CouponOrderResult() { Success = 5, Message = "您累计提交的数量已经超过了最大购买数量" };
                        }
                        else
                        {
                            couponLockedResult.ActivityLockNum += userAddLockedNum; //按提交的数量加到锁的量
                            ProductCache.SetCouponLockNum(scom.ActivityID, couponLockedResult);//设置活动锁定数量
                            log.AddLog("SetCurrentUserNumAndLockNum");

                            userLockedInfo.HasLockedNum += userAddLockedNum; //按提交的数量加到已购买数量上
                            userLockedInfo.NotPaiedCount = submitSum; //
                            ProductCache.SetCurrentUserLockNum(scom.ActivityID, scom.UserID, userLockedInfo);//设置个人提交数量
                            log.AddLog("SetCurrentUserNumAndLockNum");

                            CouponAdapter.UpdateCouponSellState(scom.ActivityID);
                            try
                            {
                                List<long> canceledOrderIDList = CouponAdapter.CancelUnPayExchangeCouponOrderByActivityIDAndUserID(scom.UserID, scom.ActivityID, scom.SKUID);
                                if (canceledOrderIDList.Count > 0)
                                {
                                    CouponAdapter.CancelUserOrderRel(scom.UserID, canceledOrderIDList);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.WriteLog("CheckCouponSubmitNumberByCache ERROR：" + e);
                            }
                        }
                    }
                }

            }
            log.Finish();
            return ret;
        }

        /// <summary>
        /// 【小程序】提交购买VIP
        /// </summary>
        /// <param name="scom"></param>
        /// <returns></returns>
        [HttpPost]
        public CouponOrderResult SubmitMemberOrderForWxapp(SubmitCouponOrderForWxapp scom)
        {
            var param = new SubmitCouponOrderModel();
            param.ActivityID = scom.ActivityID;
            param.ActivityType = scom.ActivityType;
            param.UserID = scom.UserID;
            param.RealName = scom.RealName;
            param.PhoneNo = scom.PhoneNo;
            param.CID = scom.CID;
            param.OrderItems = new List<ProductAndNum> { new ProductAndNum { SourceID = scom.SourceID, Number = scom.Number, Price = scom.Price, Type = scom.Type } };
            param.AddInfo = new ExchangeCouponAddInfoItem();

            return SubmitMemberOrder(param);
        }

        /// <summary>
        /// 购买VIP
        /// </summary>
        /// <param name="scom"></param>
        /// <returns></returns>
        [HttpPost]
        public CouponOrderResult SubmitMemberOrder(SubmitCouponOrderModel scom)
        {
            CouponOrderResult result = CheckSubmitMemberOrder(scom);

            //  Log.WriteLog("SubmitMemberOrder:" + scom.IsVIPInvatation + ":" + scom.PhoneNo +":" + scom.ActivityID);

            if (result.Success == 0) //检测成功
            {
                try
                {
                    var model = CouponAdapter.GetCouponActivityDetail(scom.ActivityID, false);//10min缓存

                    Decimal totalPrice = 0;
                    string[] priceStr = model.activity.Price.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    decimal sellPrice = int.Parse(priceStr[0]);
                    sellPrice = sellPrice == 0 ? Decimal.Parse(priceStr[1]) : sellPrice;

                    if (CouponAdapter.IsVIP199UpGrade2VIP599(scom.ActivityID, scom.UserID))
                    {
                        sellPrice = 400;//如果是VIP199升级到VIP599，那么只收400元
                    }

                    if (CouponAdapter.IsVIPRenew(scom.ActivityID, scom.UserID))
                    {
                        sellPrice = (int)(sellPrice * 0.88M);//如果是续费则88折
                    }

                    totalPrice += scom.OrderItems[0].Number * sellPrice;
                    scom.OrderItems[0].Price = sellPrice; //设置价格

                    User_Info ui = AccountAdapter.GetUserInfoByUserId(scom.UserID);

                    //提交订单ID
                    int couponOrderId = 0;
                    if (totalPrice > 0)
                    {
                        CommOrderEntity coe = new CommOrderEntity()
                        {
                            CustomID = scom.ActivityID,
                            IDX = 0,
                            Name = model.activity.PageTitle,// "周末酒店VIP会员",   //TODO 
                            OpNotice = null,
                            PhoneNum = string.IsNullOrWhiteSpace(scom.PhoneNo) ? ui.MobileAccount : scom.PhoneNo,
                            Price = totalPrice,
                            TypeID = scom.ActivityType
                        };
                        couponOrderId = CouponAdapter.InsertCommOrders(coe); //新建支付订单记录
                    }

                    int SKUID = CheckVIPSKUID(scom.SKUID);

                    //插入券记录
                    if (couponOrderId == 0)
                    {
                        result = new CouponOrderResult()
                        {
                            Success = 8,
                            Message = "会员订单提交失败",
                            OrderID = 0,
                            UserID = scom.UserID
                        };
                    }
                    else
                    {
                        int CustomerType = (int)AccountAdapter.GetCustomerType(scom.UserID);

                        if (scom.SKUID > 0)
                        {
                            //获取sku属性
                            List<ProductPropertyInfoEntity> propertyInfo = ProductAdapter.GetProductPropertyInfoBySKU(scom.SKUID.ToString());
                            ProductPropertyInfoEntity ppi = propertyInfo.First();
                            //判断是否是大团购 非大团购订单，一次提交多份只生成一个couponorderid。
                            if (ppi.PropertyType != 6 && scom.CouponOrderID == 0)
                            {
                                scom.CouponOrderID = couponService.GetNextId(NextIdType.OrderID);
                            }
                        }

                        ExchangeCouponAddInfoItem addInfo = scom.AddInfo;
                        foreach (var productAndNum in scom.OrderItems)
                        {
                            for (int i = 0; i < productAndNum.Number; i++)
                            {
                                CouponAdapter.InsertExchangeCoupon(new ExchangeCouponEntity()
                                {
                                    PayID = couponOrderId,
                                    UserID = scom.UserID,
                                    CID = scom.CID,
                                    SKUID = SKUID,
                                    IsVIPInvatation = scom.IsVIPInvatation,
                                    PhoneNum =
                                        string.IsNullOrWhiteSpace(scom.PhoneNo) ? ui.MobileAccount : scom.PhoneNo,
                                    ActivityID = scom.ActivityID,
                                    ActivityType = scom.ActivityType,
                                    ExchangeNo = null,
                                    Price = productAndNum.Price,
                                    Type = productAndNum.Type,
                                    State = 1,
                                    CustomerType = CustomerType,
                                    InnerBuyGroup = i + 1,
                                    AddInfo = addInfo == null
                                        ? "||"
                                        : string.Format("{0}|{1}|{2}",
                                            string.IsNullOrWhiteSpace(addInfo.TrueName)
                                                ? ""
                                                : addInfo.TrueName,
                                            string.IsNullOrWhiteSpace(addInfo.PersonnelStructure)
                                                ? ""
                                                : addInfo.PersonnelStructure,
                                            string.IsNullOrWhiteSpace(addInfo.ChildrenAge)
                                                ? ""
                                                : addInfo.ChildrenAge),
                                    CouponOrderId = scom.CouponOrderID > 0 ? scom.CouponOrderID : couponService.GetNextId(NextIdType.OrderID)
                                });
                            }
                        }

                        UpdateUserRealName(scom.UserID, scom.RealName);

                        result = new CouponOrderResult()
                        {
                            Success = 0,
                            Message = "会员订单提交成功",
                            OrderID = couponOrderId,
                            UserID = scom.UserID,
                            PayChannels = OrderAdapter.genOrderPayChannels(HJDAPI.Common.Helpers.Enums.CustomerType.vip, -1, scom.PhoneNo),
                            Amount = totalPrice,
                            GoodsInfo = model.activity.PageTitle
                        };
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("执行SubmitMemberOrder发生错误，详细信息为:" + ex.Message + "\r\n" + ex.StackTrace);
                    throw;
                }
            }

            return result;
        }


        /// <summary>
        /// 检测买VIP送的SKU是否合法。
        /// 只有SPU的类型名为"买VIP送产品"的SKU才合法
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int CheckVIPSKUID(int skuid)
        {
            if (skuid > 0)
            {
                var skuinfo = ProductAdapter.GetSKUInfoByID(skuid);
                if (skuinfo.Category.Name != "买VIP送产品")
                {
                    skuid = 0;
                }
            }

            return skuid;
        }

        private static void UpdateUserRealName(long UserID, string RealName)
        {
            //更新用户的RealName
            SP_UpdateMemberProfileParam s = new SP_UpdateMemberProfileParam();
            s.RealName = RealName;
            s.UserID = UserID;
            try
            {
                int _updateRealName = AccService.UpdateUserInfoExRealName(s);
            }
            catch (Exception ex)
            {
                Log.WriteLog("执行UpdateMemberProfile发生错误，详细信息为:【" + UserID + "】【" + RealName + "】" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }


        private CouponOrderResult CheckSubmitMemberOrder(SubmitCouponOrderModel scom)
        {
            CouponOrderResult result = new CouponOrderResult() { Success = 0, Message = "检测通过" };
            if (scom.ActivityID == 0 || scom.UserID == 0 || scom.ActivityType == 0 || scom.OrderItems == null ||
                scom.OrderItems.Count == 0)
            {
                result = new CouponOrderResult() { Success = 1, Message = "关键参数丢失,请联系我们" };
            }
            else
            {
                var errMessage = CouponAdapter.CanBuyMemberCouponSecond(scom.UserID, scom.ActivityID);
                if (errMessage.Length > 0)
                {
                    result = new CouponOrderResult() { Success = 200, Message = errMessage };
                }
                else if (!CouponAdapter.AcquireLock(scom.ActivityID))
                {
                    result = new CouponOrderResult() { Success = 100, Message = "当前有很多用户提交,请稍后重新尝试！" };
                }
                else
                {

                    var submitSum = 0;
                    CouponOrderResult checkResult = CheckMemberSubmitNumberByCached(scom, out submitSum);
                    CouponAdapter.ReleaseLock(scom.ActivityID);//验证完毕后 可以释放锁

                    if (checkResult.Success != 0)
                    {
                        result = checkResult;
                    }
                    else if (scom.Flag == 110)
                    {
                        result = new CouponOrderResult() { Success = scom.Flag, Message = "测试并发直接返回" };
                    }
                    else
                    {
                        try
                        {
                            var model = CouponAdapter.GetCouponActivityDetail(scom.ActivityID, false);//10min缓存

                            var currentNum = ProductCache.GetCouponUserLockNumFromMemcache(scom.ActivityID, scom.UserID);
                            if (currentNum.OrderID > 0 && currentNum.PayType == 0)
                            {
                                result = new CouponOrderResult() { Success = 5, Message = "已经提交成功，等待用户支付", OrderID = currentNum.OrderID, PayChannels = OrderAdapter.genOrderPayChannels(HJDAPI.Common.Helpers.Enums.CustomerType.vip, -1, scom.PhoneNo) };
                            }
                            else
                            {
                                var priceOriginStr = model.activity.Price;
                                if (string.IsNullOrWhiteSpace(priceOriginStr)
                                    || priceOriginStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length == 0
                                    )
                                {
                                    result = new CouponOrderResult() { Success = 8, Message = "活动价格定义不正确！", OrderID = currentNum.OrderID, PayChannels = OrderAdapter.genOrderPayChannels(HJDAPI.Common.Helpers.Enums.CustomerType.vip, -1, scom.PhoneNo) };
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            result = new CouponOrderResult() { Success = 300, Message = e.Message };
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 检测会员售卖的状态
        /// </summary>
        /// <returns></returns>
        private CouponOrderResult CheckMemberSubmitNumberByCached(SubmitCouponOrderModel scom, out int submitSum)
        {
            submitSum = 0;
            if (scom == null || scom.ActivityID == 0 || scom.ActivityType == 0 || scom.UserID == 0 ||
                scom.OrderItems == null || !scom.OrderItems.Any())
            {
                return new CouponOrderResult() { Success = 1, Message = "关键参数丢失,请联系我们" };
            }
            else
            {
                submitSum = scom.OrderItems.Sum(_ => _.Number);

                var result = ProductCache.CouponSellInfoMem(scom.ActivityID);//活动其他数量                

                if (result == null)
                {
                    return new CouponOrderResult() { Success = 2, Message = "参数错误,没有找到活动数据" };
                }
                if (result.EffectiveTime > DateTime.Now)
                {
                    return new CouponOrderResult() { Success = 18, Message = "活动尚未开始，请耐心等候" };
                }
                if (result.TotalNum <= result.SellNum)
                {
                    return new CouponOrderResult() { Success = 3, Message = "本次会员名额已经全部售完，谢谢您的关注" };
                }
                //else if (result.TotalNum <= result.SellNum + userResult.ActivityLockNum)
                //{
                //    return new CouponOrderResult() { Success = 4, Message = "本次活动剩余数量已被锁定，等待客人付款中,稍后再来碰碰运气吧" };
                //}
                //else
                //{
                //    var userResult20 = CouponAdapter.GetCurrentUserNumAndLockNumFromMemcache(scom.ActivityID, scom.UserID);//个人提交数量
                //    if (result.MaxBuyNum < userResult20.UserBuyNum + submitSum)
                //    {
                //        if (userResult20.PayType > 0)
                //        {
                //            return new CouponOrderResult() { Success = 6, Message = "您已购买过会员", OrderID = userResult20.OrderID };
                //        }
                //        else if (userResult20.OrderID > 0)
                //        {
                //            return new CouponOrderResult() { Success = 5, Message = "已经提交成功，等待用户支付", OrderID = userResult20.OrderID, PayChannels = OrderAdapter.genOrderPayChannels(HJDAPI.Common.Helpers.Enums.CustomerType.vip, -1) };
                //        }
                //    }
                //    else
                //    {
                //        userResult.ActivityLockNum += submitSum; //按提交的数量加到锁的量
                //        CouponAdapter.SetCurrentUserNumAndLockNum(scom.ActivityID, userResult);//设置活动锁定数量

                //        userResult20.UserBuyNum += submitSum; //按提交的数量加到已购买数量上
                //        CouponAdapter.SetCurrentUserNumAndLockNum(scom.ActivityID, userResult20, scom.UserID);//设置个人提交数量
                //    }
                //}
            }
            return new CouponOrderResult() { Success = 0, Message = "可以继续往下走" };
        }

        /// <summary>
        /// 券是否已卖完或剩余已被锁定
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        [HttpGet]
        public BuyCouponCheckNumResult IsExchangeCouponSoldOut(int activityID, long userID)
        {
            return CouponAdapter.GenCouponSealInfo(activityID, userID);
        }

        //[HttpGet]
        //public RoomCouponOrderEntity GetOneCouponOrderEntity(int couponOrderID)
        //{
        //    return CouponAdapter.GetOneRoomCouponOrderEntity(couponOrderID);
        //}

        [HttpGet]
        public CommOrderEntity GetOneCommOrderEntity(int couponOrderID)
        {
            return CouponAdapter.GetOneCommOrderEntity(couponOrderID);
        }

        /// <summary>
        /// 房券购买完成调用的接口
        /// </summary>
        /// <param name="couponOrderID"></param>
        /// <param name="useMemcached"></param>
        /// <returns></returns>
        [HttpGet]
        public RoomCouponOrderEntity GenCouponPayCompleteResult(int couponOrderID, bool useMemcached = false)
        {
            var result = CouponAdapter.GetOneRoomCouponOrderEntityByPayID(couponOrderID);

            //if(result.ExchangeCouponList!=null)

            //过滤掉壳产品券码
            // result.ExchangeCouponList = result.ExchangeCouponList.Where(_ => _.IsPackage == false).ToList();

            return result;
        }


        [HttpGet]
        public RoomCouponOrderEntity AfterCouponPayComplete(int payOrderID, long timestamp, int sourceid, string Sign)
        {

            string url = this.Request.RequestUri.AbsoluteUri.Replace("&_newtitle=1", "").Replace("&_newpage=1", "").Replace("&_newpage=0", "");
            Log.WriteLog(string.Format("AfterCouponPayComplete:payOrderID:{0} timestamp:{1} sourceid:{2} Sign:{3} url:{4} ", payOrderID, timestamp, sourceid, Sign, url));
            string url_Sign = Sign;//url请求的sign 
            int sublength = url.LastIndexOf("&");
            string url_RequestType = sublength > 0 ? url.Substring(0, sublength) : url;

            string sscode = HJDAPI.Common.Security.Signature.GenSignature(timestamp, sourceid, Configs.MD5Key, url_RequestType);

            string final = url_Sign.Replace(" ", "+");

            if (!(sscode == final))
            {
                Log.WriteLog(string.Format("验证失败:payOrderID:{0} timestamp:{1} sourceid:{2} Sign:{3} sscode:{4} final:{5}", payOrderID, timestamp, sourceid, Sign, sscode, final));

                return new RoomCouponOrderEntity();
                //  m.message = "验证失败！";
            }
            else
            {

                HJD.CouponService.Contracts.Entity.CommOrderEntity commOrder = CouponAdapter.GetOneCommOrderEntity(payOrderID);
                if (commOrder.TypeID == (int)Enums.CommOrderType.InvocePay)
                {

                    Log.WriteLog(payOrderID + "   comm  " + JsonConvert.SerializeObject(commOrder));
                    User_Info userEntity = AccountAdapter.GetUserInfoByMobile(commOrder.PhoneNum);
                    RoomCouponOrderEntity model = new RoomCouponOrderEntity();
                    DealInvoiceInfo(commOrder.CustomID);

                    var param = new ConsumeUserPointsParam { userID = userEntity.UserId, businessID = payOrderID, requiredPoints = (int)commOrder.Points, typeID = HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.InvoicePointsCost };
                    PointsAdapter.ConsumeUserPoints(param);
                    return model;
                }
                else
                {
                    RoomCouponOrderEntity coe = null;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("GenCouponPayCompleteResult:" + payOrderID.ToString());
                    try
                    {

                        ExchangeCouponModel couponOrder = new ExchangeCouponModel();
                        coe = CouponAdapter.GetOneRoomCouponOrderEntityByPayID(payOrderID);

                        Log.WriteLog("GenCouponPayCompleteResult：payOrderID：" + payOrderID + " State:" + coe.State);
                        if ((coe.State == 1))
                        {
                            sb.AppendLine("coe.State == " + coe.State.ToString());
                            //判断是不是已支付
                            var rightCouponList = coe.ExchangeCouponList.FindAll(_ => _.PayType != 0);

                            if (rightCouponList.Count > 0)
                            {
                                couponOrder = rightCouponList.First();
                                int couponID = couponOrder.ID;
                                string phoneNum = couponOrder.PhoneNum;
                                int activityID = couponOrder.ActivityID;
                                long userID = couponOrder.UserID;
                                int SKUID = couponOrder.SKUID;
                                long CID = coe.CID;
                                int GroupId = couponOrder.GroupId;
                                int buyNum = 0;

                                sb.AppendLine("rightCouponList.Count > 0");
                                bool canBuy = true;
                                #region 判断用户是否有购买首单权限

                                bool bIsCouponListNeedVIPFirstPay = CouponAdapter.IsCouponListNeedVIPFirstPay(coe.ExchangeCouponList);

                                ProductPropertyInfoEntity property = ProductAdapter.GetProductPropertyInfoBySKU(SKUID.ToString()).First();

                                if (bIsCouponListNeedVIPFirstPay)
                                {
                                    //如果手拉手发团，可以购买首单
                                    if (!CouponAdapter.CanJumpOverCanBuyVIPFirstBuyPackage(coe.ActivityType, activityID, userID, GroupId))
                                    {
                                        bool bUseHasBuyVIPFirstPriviledge = AccountAdapter.HasUserPriviledge(coe.UserId, PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage);

                                        if (bUseHasBuyVIPFirstPriviledge == false)
                                        {
                                            Log.WriteLog("购买失败，并退款：payOrderID：" + payOrderID);
                                            canBuy = false;
                                            coe.PayResultMessage = "抱歉，此专享优惠套餐每位VIP会员限购一套哦，您之前已经购买过啦~";
                                            CouponAdapter.TransExchangeCouponModel2ExchangeCouponEntity(coe.ExchangeCouponList).ForEach(item =>
                                            {
                                                item.State = (int)ExchangeCouponState.paied;
                                                item.ExchangeNo = (-item.ID).ToString();
                                                CouponAdapter.UpdateExchangeState(item);
                                                Add2RefundCoupon(item.ID, item.UserID);
                                            });
                                        }
                                    }
                                }

                                canBuy = CheckCouponAndFund(rightCouponList);

                                #endregion
                                if (canBuy)
                                {
                                    bool hasEnoughThirdCoupon = true;
                                    #region 判断SKU券码类型
                                    //SKUEntity skuEntity = ProductAdapter.GetSKUEntityByID(SKUID);
                                    //switch (skuEntity.ExchangeNoType)
                                    //{
                                    //    case 0:
                                    //    case 2:
                                    //    case 3:
                                    //        //随机生成字符串,同时检测是否存在相同的券号。如果存在则替换,不存在则使用
                                    //        CouponAdapter.GenIntCouponRandomNo(6, rightCouponList);
                                    //        sb.AppendLine("GenIntCouponRandomNo");
                                    //        break;
                                    //    case 1:
                                    //        hasEnoughThirdCoupon = CouponAdapter.GetTopSupplierCouponBySupplierID(coe.SupplierID, rightCouponList);
                                    //        Log.WriteLog("rightCouponList：" + hasEnoughThirdCoupon + " SupplierID:" + coe.SupplierID);
                                    //        break;
                                    //    default:
                                    //        CouponAdapter.GenIntCouponRandomNo(6, rightCouponList);
                                    //        sb.AppendLine("GenIntCouponRandomNo");
                                    //        break;
                                    //}
                                    #endregion


                                    ////bool hasEnoughThirdCoupon = true;
                                    ////有供应商，则用第三方的券码
                                    //if (coe.SupplierID > 0)
                                    //{
                                    //    hasEnoughThirdCoupon = CouponAdapter.GetTopSupplierCouponBySupplierID(coe.SupplierID, rightCouponList);
                                    //    Log.WriteLog("rightCouponList：" + hasEnoughThirdCoupon + " SupplierID:" + coe.SupplierID);
                                    //}
                                    //else
                                    //{
                                    //    //随机生成字符串,同时检测是否存在相同的券号。如果存在则替换,不存在则使用
                                    //    CouponAdapter.GenIntCouponRandomNo(6, rightCouponList);
                                    //    sb.AppendLine("GenIntCouponRandomNo");
                                    //}
                                    int supplierCount = rightCouponList.Where(_ => _.ExchangeNoType == 1).Count();
                                    foreach (var exchangeCoupon in rightCouponList)
                                    {
                                        if (hasEnoughThirdCoupon == false) break;
                                        if (exchangeCoupon.ExchangeNoType == 1)
                                        {
                                            SPUEntity spumodel = ProductAdapter.GetSPUBySKUID(exchangeCoupon.SKUID);
                                            hasEnoughThirdCoupon = CouponAdapter.GetSupplierCouponBySupplierID(spumodel.SupplierID, supplierCount, exchangeCoupon);
                                        }
                                        else
                                        {
                                            exchangeCoupon.ExchangeNo = CouponAdapter.CheckExistExchagnNo(6, exchangeCoupon);
                                            sb.AppendLine("CheckExistExchagnNo");
                                        }
                                    }

                                    if (hasEnoughThirdCoupon == false)//没有足够的第三方券则进入超时支付状态
                                    {
                                        string phone = rightCouponList.First().PhoneNum;
                                        //更新券状态及生成券码
                                        foreach (ExchangeCouponEntity item in rightCouponList)
                                        {
                                            ExchangeCouponEntity ece = new ExchangeCouponEntity()
                                            {
                                                PayID = payOrderID,
                                                ID = item.ID,
                                                ExchangeNo = null,
                                                ExchangeTargetID = 0,
                                                ActivityID = 0,
                                                State = (int)ExchangeCouponState.paiedOverTime,
                                                CancelTime = null,
                                                ExchangeTime = null
                                            };//根据支付ID更新活动已售出量和券的状态
                                            CouponAdapter.UpdateExchangeCoupon(ece);
                                        }
                                        SMSAdapter.SendSMS(phone, "您已购买成功，券码将稍后发送到您手机。" + Configs.SMSSuffix);
                                    }
                                    else
                                    {
                                        ////随机生成字符串,同时检测是否存在相同的券号。如果存在则替换,不存在则使用
                                        //CouponAdapter.GenCouponRandomNo(8, rightCouponList);
                                        //sb.AppendLine("GenCouponRandomNo");

                                        List<int> promotionIDList = new List<int>();
                                        List<int> SKUIDList = new List<int>();


                                        //更新券状态及生成券码
                                        foreach (ExchangeCouponEntity item in rightCouponList)
                                        {
                                            ExchangeCouponEntity ece = new ExchangeCouponEntity()
                                            {
                                                PayID = payOrderID,
                                                ID = item.ID,
                                                ExchangeNo = item.ExchangeNo,
                                                ExchangeTargetID = 0,
                                                ActivityID = 0,
                                                State = (int)ExchangeCouponState.paied,
                                                CancelTime = null,
                                                ExchangeTime = null
                                            };//根据支付ID更新活动已售出量和券的状态
                                            CouponAdapter.UpdateExchangeCoupon(ece);

                                            #region 更新大团购定金状态
                                            try
                                            {
                                                if (property.PropertyType == 6 && property.PropertyOptionTargetID == 2)
                                                {
                                                    List<ExchangeCouponEntity> upExList = CouponAdapter.GetExchangCouponByCouponOrderID(item.CouponOrderId).Where(_ => _.State == 2).ToList();
                                                    ExchangeCouponEntity upEx = upExList.Where(_ => _.SKUID != item.SKUID).First();

                                                    ExchangeCouponEntity upEx1 = new ExchangeCouponEntity()
                                                    {
                                                        PayID = payOrderID,
                                                        ID = upEx.ID,
                                                        ExchangeNo = upEx.ExchangeNo,
                                                        ExchangeTargetID = upEx.ExchangeTargetID,
                                                        ActivityID = upEx.ActivityID,
                                                        State = (int)ExchangeCouponState.exchanged,
                                                        CancelTime = null,
                                                        ExchangeTime = null
                                                    };
                                                    CouponAdapter.UpdateExchangeCoupon(upEx1);
                                                }
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            #endregion



                                            //如果需要积分，那么扣除用户积分
                                            if (item.Points > 0)
                                            {
                                                var param = new ConsumeUserPointsParam { userID = item.UserID, businessID = item.ID, requiredPoints = (int)item.Points, typeID = HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.Coupon };
                                                PointsAdapter.ConsumeUserPoints(param);
                                            }


                                            buyNum++;
                                            if (item.PromotionID > 0)
                                            {
                                                promotionIDList.Add(item.PromotionID);
                                            }
                                            if (item.SKUID > 0)
                                            {
                                                SKUIDList.Add(item.SKUID);
                                            }

                                            if (item.UserUseHousingFundAmount > 0)
                                            {
                                                FundAdapter.ReduceUserFund(new HJD.HotelManagementCenter.Domain.Fund.UserFundExpendDetailEntity
                                                {
                                                    CreateTime = DateTime.Now,
                                                    Fund = item.UserUseHousingFundAmount,
                                                    TypeId = (int)HJD.HotelManagementCenter.Domain.FundType.OrderDeduct,
                                                    Label = "支付消费券",
                                                    OriginAmount = item.Price,
                                                    OriginOrderId = item.ID,
                                                    UserId = item.UserID
                                                });
                                            }

                                            if (item.CashCouponID > 0)
                                            {
                                                var couponItem = CouponAdapter.GetUserCouponItemInfoByID(item.CashCouponID);
                                                if (couponItem.UserCouponType == (int)UserCouponType.DiscountUnconditional)
                                                {
                                                    //记录券使用
                                                    RequestResultEntity resp = CouponAdapter.UseUserCouponInfoItem(
                                                        new UseCashCouponItem
                                                        {
                                                            CashCouponID = couponItem.IDX,
                                                            CashCouponType = couponItem.UserCouponType,
                                                            UseCashAmount = item.CashCouponAmount,
                                                            OrderID = item.ID,
                                                            OrderType = (int)CashCouponOrderSorceType.sku
                                                        });
                                                }
                                            }

                                        }


                                        if (rightCouponList.First().CashCouponID > 0)
                                        {

                                            var couponItem = CouponAdapter.GetUserCouponItemInfoByID(rightCouponList.First().CashCouponID);

                                            if (couponItem.UserCouponType == (int)UserCouponType.DiscountOverPrice)
                                            {
                                                //记录券使用
                                                RequestResultEntity resp = CouponAdapter.UseUserCouponInfoItem(
                                                    new UseCashCouponItem
                                                    {
                                                        CashCouponID = couponItem.IDX,
                                                        CashCouponType = couponItem.UserCouponType,
                                                        UseCashAmount = couponItem.DiscountAmount,
                                                        OrderID = rightCouponList.First().ID,
                                                        OrderType = (int)CashCouponOrderSorceType.sku
                                                    });
                                            }
                                        }

                                        if (rightCouponList.First().VoucherAmount > 0)
                                        {
                                            foreach (var CashCouponID in CommMethods.TranStrIDsToList(rightCouponList.First().VoucherIDs))
                                            {
                                                var couponItem = CouponAdapter.GetUserCouponItemInfoByID(CashCouponID);
                                                //记录代金券使用
                                                RequestResultEntity resp = CouponAdapter.UseUserCouponInfoItem(
                                                    new UseCashCouponItem
                                                    {
                                                        CashCouponID = couponItem.IDX,
                                                        CashCouponType = couponItem.UserCouponType,
                                                        UseCashAmount = couponItem.DiscountAmount,
                                                        OrderID = rightCouponList.First().ID,
                                                        OrderType = (int)CashCouponOrderSorceType.sku
                                                    });
                                            }
                                        }


                                        if (coe.GroupId != 0)
                                        {
                                            GenGroupPayCompleteSMS(coe, rightCouponList);
                                        }
                                        else if (property.PropertyType == 6 && property.PropertyOptionTargetID == 1)
                                        {
                                            GenStepGroupSMS(coe, rightCouponList, 1);
                                        }
                                        else if (property.PropertyType == 6 && property.PropertyOptionTargetID == 2)
                                        {
                                            GenStepGroupSMS(coe, rightCouponList, 2);
                                        }
                                        else
                                        {
                                            GenCouponPaySMS(coe, rightCouponList, buyNum);
                                        }

                                        ProductServiceEnums.ProductType productType = ProductServiceEnums.ProductType.RoomCoupon;
                                        int objID = activityID;
                                        if (rightCouponList.First().SKUID > 0)  //如果有SKUID，那么以卖产品的方式处理后继低优惠
                                        {
                                            productType = ProductServiceEnums.ProductType.ProductCoupon;
                                            objID = rightCouponList.First().SKUID;
                                        }

                                        ///开始处理Promotion
                                        if (coe.GroupId > 0 || coe.ActivityType == 400 && coe.CouponPrice == 400) //如果是VIP升级则不参与送产品活动  //手拉手成团后再送赠券
                                        {

                                        }
                                        else
                                        {
                                            GenCouponPromotProducts(couponID, phoneNum, userID, CID, buyNum, productType, objID, couponOrder.PayID, couponOrder.SKUID, couponOrder.CouponOrderId);
                                        }

                                        //  如果有Promotion, 需要检查是否需要去除首套低优惠权限
                                        // 如果是手拉手发团用户，那么不扣VIP首套权限   
                                        if (!CouponAdapter.IsGroupSponserAndCanBuyVIPFirstBuyPackage(activityID, userID, GroupId))
                                        {
                                            if (promotionIDList.Count > 0)
                                            {
                                                ProductAdapter.CheckPromotionAndRemoveUserBuyFirstPackagePriviledge(userID, promotionIDList);
                                            }

                                            if (SKUIDList.Count > 0)
                                            {
                                                ProductAdapter.CheckSKUAndRemoveUserBuyFirstPackagePriviledge(userID, SKUIDList);
                                            }
                                        }

                                        ////处理临时券活动行为
                                        DealTempCouponPromition(couponOrder.PhoneNum, couponOrder.ActivityID, couponOrder.UserID, coe.CID);

                                        coe.IsNewRegistUser = AccService.GetUserInfoByMobile(couponOrder.PhoneNum).IsTemporaryPWD;

                                        //券完成后，检查如果是全员分销产品&有分享来源，则去触发红包奖励机制 2018.1.23 haoy
                                        try
                                        {
                                            var _sku = ProductAdapter.GetSKUEXEntityByID(rightCouponList[0].SKUID);
                                            if (_sku.IsRetail && couponOrder.FromWeixinUid > 0)
                                            {
                                                CouponAdapter.GenCouponOrderRedPackReward(couponOrder.ID, rightCouponList.First().PageTitle, (int)((_sku.ManualCommission > 0 ? _sku.ManualCommission : _sku.AutoCommission) * 100), couponOrder.FromWeixinUid);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteLog(string.Format("【全员分销】GenCouponOrderRedPackReward:Error {0}", ex.Message + " @ " + ex.StackTrace));
                                        }
                                    }
                                }

                                var userLockedInfo = ProductCache.GetCouponUserLockNumFromMemcache(coe.ActivityID, coe.UserId);//个人提交数量 
                                userLockedInfo.NotPaiedCount = 0; //
                                ProductCache.SetCurrentUserLockNum(coe.ActivityID, coe.UserId, userLockedInfo);//设置个人提交数量 

                                var couponSellInfo = ProductCache.CouponSellInfoMem(coe.ActivityID);
                                couponSellInfo.SellNum += buyNum;
                                couponSellInfo.SellNum = couponSellInfo.SellNum > couponSellInfo.TotalNum ? couponSellInfo.TotalNum : couponSellInfo.SellNum;
                                ProductCache.SetCouponSellInfoMem(coe.ActivityID, couponSellInfo);

                                //更新大团购sku价格  状态 判断是否成团
                                CouponActivityEntity ca = CouponAdapter.GetCouponActivityEntity(activityID);
                                var bStepGroupStateChanged = ProductAdapter.StepGroupUpdateSKUPriceBySKUID(SKUID, ca.SellNum + ca.ManuSellNum);
                                if (bStepGroupStateChanged)
                                {
                                    ProductCache.RemoveStepGroupCahceWithSKUID(SKUID);
                                }

                                //更新前置预约的预约状态
                                foreach (ExchangeCouponEntity item in coe.ExchangeCouponList)
                                {
                                    ProductAdapter.UpdateCouponOrderBookUserDateByCouponId(item.ID);
                                }

                            }

                        }
                        else if (coe.State == 4 || coe.State == 40) //已取消状态下的订单,发短信告知客户原因并立即全额退款,更新券的状态为6-已支付(超时取消)
                        {
                            sb.AppendLine("coe.State == 4");
                            //发送通知短信参数
                            int activityID = 0;
                            long userID = 0;
                            string phoneNum = "";
                            foreach (var item in coe.ExchangeCouponList)
                            {
                                if (activityID == 0)
                                {
                                    phoneNum = item.PhoneNum;
                                    activityID = item.ActivityID;
                                    userID = item.UserID;
                                }

                                //更新单券状态
                                ExchangeCouponEntity param = new ExchangeCouponEntity() { PayID = 0, ID = item.ID, ExchangeNo = null, ExchangeTargetID = 0, ActivityID = 0, State = (int)ExchangeCouponState.paiedOverTime, CancelTime = null, ExchangeTime = null };//根据券的状态为6(已支付成功且已经超时)
                                CouponAdapter.UpdateExchangeCoupon(param);


                                //提交自动退款记录  无论券可退不可退 此处都应可退
                                CouponAdapter.CouponRefund(item.ID); //插入退款记录 退款成功后 房券的状态改为5（已退款）  如果还是6说明插入带退款记录失败 便于后期手动加入待退款列表
                            }

                            string couponTypeName = "";
                            switch (coe.ActivityType)
                            {
                                case 400:
                                    coe.PayResultMessage = "由于支付时间已超过10分钟，会员没有购买成功";
                                    couponTypeName = "会员";
                                    break;
                                case 600:
                                    coe.PayResultMessage = "由于支付时间已超过10分钟，消费券没有购买成功";
                                    couponTypeName = "消费券";
                                    break;
                                default:
                                    coe.PayResultMessage = "由于支付时间已超过10分钟，房券没有购买成功";
                                    couponTypeName = "房券";
                                    break;
                            }

                            try
                            {

                                SMServiceController.SendSMS(phoneNum, string.Format(CouponAdapter.buyCouponTimeOutTemplate200, coe.TotalPrice, couponTypeName));
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLog("短信发送异常" + ex.Message);
                            }

                        }


                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("complete 房券出现异常" + ex.Message + ex.StackTrace);
                    }

                    // Log.WriteLog(sb.ToString());
                    // 更新Redis缓存
                    var payRelCouponOrderIDList = CouponAdapter.GetOnePayRelCouponOrderList(payOrderID).Select(_ => _.CouponOrderId).ToList();
                    OrderHelper.AddOrderToRedisWithIDList(payRelCouponOrderIDList);
                    // 订单状态改变，清缓存
                    var payRelCouponIDList = CouponAdapter.GetOnePayRelCouponOrderList(payOrderID).Select(_ => (long)_.ID).ToList();
                    ProductCache.RemoveUserDetailOrderCacheWithOrderIDList(payRelCouponIDList);

                    //更新活动缓存售卖数
                    ProductCache.RemoveCouponSellNumCache(coe.ActivityID);

                    return coe;
                }
            }
        }

        /// <summary>
        /// 处理发票状态
        /// </summary>
        /// <param name="invoiceId">发票id</param>
        [HttpGet]
        public void DealInvoiceInfo(int invoiceId)
        {
            InvoiceEntity invoice = SettlementAdapter.GetInvoiceByID(invoiceId);
            if (invoice != null && invoice.ID > 0 && invoice.State == 3)
            {
                invoice.State = 0;
                Log.WriteLog("DealInvoiceInfo " + JsonConvert.SerializeObject(invoice));
                SettlementAdapter.UpdateInvoice(invoice);


                SMServiceController.SendSMS(invoice.TelPhone, "您的订单已经申请开具增值税普通发票纸质发票，将于5-10个工作日快递给您。" + Configs.SMSSuffix);
            }
        }
        /// <summary>
        /// 一次支付完成后生成赠券
        /// </summary>
        /// <param name="couponID">购买券的ID</param>
        /// <param name="phoneNum"></param>
        /// <param name="userID"></param>
        /// <param name="CID"></param>
        /// <param name="buyNum">购买数量，可能会购买多份，那需要按多份赠送券</param>
        /// <param name="productType">产品类型，券或以前的房券</param>
        /// <param name="objID">SKUID或房券活动ID</param>
        /// <param name="PayID">支付的ID</param>
        /// <param name="PromotionID"></param>
        /// <param name="couponOrderId"></param>
        /// <param name="skuID"></param>
        private static void GenCouponPromotProducts(int couponID, string phoneNum, long userID, long CID, int buyNum, ProductServiceEnums.ProductType productType, int objID, int PayID, int skuID, long couponOrderId)
        {
            PromotionCheckEntity promotionListForAfterPay = ProductAdapter.CheckProductPromotion(productType, objID, buyNum, userID, ProductServiceEnums.SceneType.APP, ProductServiceEnums.PromotionUseSceneType.PayComplate);
            if (promotionListForAfterPay.PromotionRuleList != null)
            {
                int CustomerType = (int)AccountAdapter.GetCustomerType(userID);

                PromotionOrderInfo order = new PromotionOrderInfo { UserID = userID, CID = CID, orderid = couponID, PhoneNum = phoneNum, PayID = PayID, SKUID = skuID, CustomerType = CustomerType, CouponOrderId = couponOrderId };

                var promList = promotionListForAfterPay.PromotionRuleList.Where(r => r.Valid == true).ToList();
                if (promList.Count > 0)
                {
                    promList.ForEach(rule =>
                                    {
                                        for (int i = 0; i < buyNum; i++)  //支持每份都送活动，那么就不支持按单送
                                        {
                                            order.InnerBuyGroup = i + 1; //购买时的分组，解决一次购买多份，每份都会送产品，在退时，判断一组中送的产品是否已使用，如果已使用则不能退， 从1开始记，区别于单买
                                            Promotion.DoPromotionForPaied(rule, order);
                                        }
                                    });
                    CouponAdapter.AdjustPriceForPackageSKU(PayID);
                }
            }


        }

        /// <summary>
        /// 检查券使用的现金券和住基金是否可用
        /// </summary>
        /// <param name="rightCouponList"></param>
        /// <returns></returns>
        private bool CheckCouponAndFund(List<ExchangeCouponModel> rightCouponList)
        {
            bool canBuy = true;

            decimal fundAmount = rightCouponList.Sum(_ => _.UserUseHousingFundAmount);
            decimal cashCouponAmount = rightCouponList.Sum(_ => _.CashCouponAmount);
            decimal voucherAmount = rightCouponList.Sum(_ => _.VoucherAmount);
            long userID = rightCouponList.First().UserID;

            string logMsg = "";

            if (fundAmount > 0)
            {
                decimal totalFund = FundAdapter.GetUserFundInfo(userID).TotalFund;

                if (totalFund < fundAmount)
                {
                    canBuy = false;
                    logMsg = string.Format("券订单{0}使用住基金{1}元，但用户只有{2}元！", rightCouponList.First().ID, fundAmount, totalFund);

                }

            }

            if (cashCouponAmount > 0)
            {
                var CashCouponID = rightCouponList.First().CashCouponID;

                var couponItem = CouponAdapter.GetUserCouponItemInfoByID(CashCouponID);

                if (couponItem.State != (int)UserCouponState.log)
                {
                    canBuy = false;
                    logMsg = string.Format("券订单{0}使用现金券{1}，但现金券状态为{1}！", rightCouponList.First().ID, CashCouponID, couponItem.State);

                }
                else if (couponItem.UserCouponType == (int)UserCouponType.DiscountUnconditional && couponItem.RestAmount < cashCouponAmount)
                {
                    canBuy = false;
                    logMsg = string.Format("券订单{0}使用现金券({1}){2}元,但现金券可用剩余金额为{3}！", rightCouponList.First().ID, CashCouponID, cashCouponAmount, couponItem.RestAmount);

                }
            }

            if (voucherAmount > 0)
            {
                var VoucherIDs = rightCouponList.First().VoucherIDs;

                foreach (var CashCouponID in CommMethods.TranStrIDsToList(VoucherIDs))
                {
                    var couponItem = CouponAdapter.GetUserCouponItemInfoByID(CashCouponID);
                    if (couponItem.State != (int)UserCouponState.log)
                    {
                        canBuy = false;
                        logMsg = string.Format("券订单{0}使用现金券{1}，但现金券状态为{1}！", rightCouponList.First().ID, CashCouponID, couponItem.State);

                    }
                }
            }

            if (canBuy == false)
            {
                Log.WriteLog("CheckCouponAndFund:" + logMsg);
            }

            return canBuy;
        }



        /// <summary>
        /// 处理团购券信息
        /// </summary>
        /// <param name="coe"></param>
        /// <param name="rightCouponList"></param>
        private void GenGroupPayCompleteSMS(RoomCouponOrderEntity coe, List<ExchangeCouponModel> rightCouponList)
        {
            coe.GroupPurchase = ProductAdapter.GetGroupPurchaseEntity(coe.GroupId).First();
            ExchangeCouponModel firstCoupon = rightCouponList.First();

            //如果团购状态为 未支付状态改为进行中
            GroupPurchaseDetailEntity detailmodel = new GroupPurchaseDetailEntity();
            //只有发起团，才会出现未支付状态
            if (coe.GroupPurchase.State == (int)Enums.GroupState.未支付 || coe.GroupPurchase.State == (int)Enums.GroupState.已取消)
            {

                string kt = "您已开团，现在邀请更多朋友来参团吧。拼团进度可在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看。" + Configs.SMSSuffix;
                SKUInfoEntity skuInfo = new SKUInfoEntity();
                if (firstCoupon.SKUID > 0)
                {
                    skuInfo = ProductAdapter.GetSKUInfoByID(firstCoupon.SKUID);
                }
                if (skuInfo.Category.ID == 21)
                {
                    kt = "您已发起助力拼团，快邀请更多朋友来助力吧。拼团进度可在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看。" + Configs.SMSSuffix;
                    switch (firstCoupon.WeixinAcountId)
                    {
                        case 8:
                            {
                                kt = "您已发起助力拼团，快邀请更多朋友来助力吧。拼团进度可在微信“遛娃指南苏州服务号”的“我的>>我的订单”中查看。如需服务，请致电4000-021-702或关注“遛娃指南苏州服务号”微信进行在线咨询。";
                                break;
                            }
                        case 11:
                            {
                                kt = "您已发起助力拼团，快邀请更多朋友来助力吧。拼团进度可在微信“遛娃指南服务号”的“我的>>我的订单”中查看。如需服务，请致电4000-021-702或关注“遛娃指南服务号”微信进行在线咨询。";
                                break;
                            }
                    }

                    #region 服务号信息推送

                    //助力拼团发起后，推送一个周末酒店服务号信息
                    try
                    {
                        if (coe.GroupPurchase != null && !string.IsNullOrEmpty(coe.GroupPurchase.OpenId))
                        {
                            //Log.WriteLog(string.Format("【助力推送】服务号信息推送:coe.GroupPurchase.OpenId {0}", coe.GroupPurchase.OpenId));

                            var weixinAcount = firstCoupon.WeixinAcountId > 0 ? firstCoupon.WeixinAcountId : 7;
                            var weixinAcountName = "周末酒店";

                            //Log.WriteLog(string.Format("【助力推送】服务号信息推送:weixinAcount {0}", weixinAcount));

                            //团购订单状态提醒
                            var tempId = "8Ctx7mex5dalgAE_cu0GC12v2fwPs8Szx2vn_1Ri2oM";
                            switch ((WeiXinChannelCode)weixinAcount)
                            {
                                case WeiXinChannelCode.周末酒店服务号_皓颐:
                                    weixinAcountName = "周末酒店";
                                    tempId = "8Ctx7mex5dalgAE_cu0GC12v2fwPs8Szx2vn_1Ri2oM";
                                    break;
                                case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                                    weixinAcountName = "遛娃指南";
                                    tempId = "Vjtz_ENJc8bJIkdVz3tGLCcjgXaxFjsyODkGwEo-bPc";
                                    break;
                                case WeiXinChannelCode.周末酒店成都服务号_皓颐:
                                    weixinAcountName = "周末酒店";
                                    tempId = "1tY-ArY11_vaNMGQ_P1z0cATZBv_7rkSV6ngaUIwLDY";
                                    break;
                                case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                                    weixinAcountName = "周末酒店";
                                    tempId = "leGThrNm9C1j_RsgaoVqwso1hyD7utCyRaTCkcNmmKE";
                                    break;
                                case WeiXinChannelCode.遛娃指南服务号_皓颐:
                                    weixinAcountName = "遛娃指南";
                                    tempId = "M_ZSUolM0yuKdZKHYakAO8J_KkEcQPqx58fv_hooSgw";
                                    break;
                                case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                                    weixinAcountName = "遛娃指南";
                                    tempId = "WYbP8jXvU-wh1LsQETtRgf2oJ99Vp1hpq99F8L6_rQk";
                                    break;
                                case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                                    weixinAcountName = "遛娃指南";
                                    //tempId = "KmQWuI4mkf2gUuGR79PnWD4v2sBcVHlcpVMHxr30ap4";
                                    tempId = "kxA6PDXdHY-JZX9s643sKZcEeU6_tKPJU7Y5Pd6LSYQ";
                                    break;
                                case WeiXinChannelCode.遛娃指南广州服务号_皓颐:
                                    weixinAcountName = "遛娃指南";
                                    tempId = "kLAbgRrvtQpkNmmd0V94MoatJUA-GXBU1VUgkSuHoV4";
                                    break;
                                case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                                    weixinAcountName = "遛娃指南";
                                    tempId = "AFB7Q8HvXIUaDpWntXkAXxns8U2-3HCSqAmEL2SjLT0";
                                    break;
                            }

                            //点击文字
                            var clickTxt = ">>点击去邀请好友";

                            //团状态url
                            var groupTreeUrl = string.Format("http://www.zmjiudian.com/coupon/group/tree/{0}/{1}", coe.SKUID, coe.GroupId);

                            //title
                            var first = "你已成功发起助力拼团，快邀请朋友来助力吧！";

                            //做换行处理
                            first = string.Format(@"{0}\r\n", first);
                            clickTxt = string.Format(@"\r\n{0}", clickTxt);

                            //data list
                            var dataList = new List<string>();
                            dataList.Add(weixinAcountName);
                            dataList.Add(firstCoupon.PageTitle);

                            //整理成微信模板消息对象
                            var _wxTempEntity = new WeixinTemplateMessageEntity
                            {
                                WeixinAccount = (WeiXinChannelCode)weixinAcount,
                                ToOpenId = coe.GroupPurchase.OpenId,
                                TemplateId = tempId,
                                TemplateUrl = groupTreeUrl,
                                DataFirst = first,
                                DataRemark = clickTxt,
                                DataKeywords = dataList
                            };

                            //执行发送模板消息
                            WeiXinAdapter.SendTemplateMessage(_wxTempEntity);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(string.Format("SendTemplateMessage:Error {0}", ex.Message + " @ " + ex.StackTrace));
                    }

                    #endregion
                }
                else
                {
                    kt = "您已开团，现在邀请更多朋友来参团吧。拼团进度可在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看。" + Configs.SMSSuffix;
                    if (firstCoupon.ActivityType == 600)
                    {
                        kt = "您已开团，现在邀请更多朋友来参团吧。拼团进度可在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看。" + Configs.SMSSuffix;
                    }

                    try
                    {
                        //判断开团人的CID是否大于0。大于0 发微信服务号提示。
                        if (firstCoupon.CID > 100000 && firstCoupon.ActivityType != 400)
                        {
                            WxFWHRetailMessage(firstCoupon.CID, (long)firstCoupon.ID, firstCoupon.PageTitle, firstCoupon.CreateTime.ToString("yyyy年MM月dd日 HH:mm"), firstCoupon.Price.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog("服务号 推送分销订单失败：" + e);
                    }

                }

                //string kt = "您已开团，现在邀请更多朋友来参团吧。拼团进度可在周末酒店APP>>我的>>钱包>>房券内查看，同样信息也可在周末酒店公众号（zmjiudian）内查看。";
                //if (firstCoupon.ActivityType == 600)
                //{
                //    kt = "您已开团，现在邀请更多朋友来参团吧。拼团进度可在周末酒店APP>>我的>>钱包>>消费券内查看，同样信息也可在周末酒店公众号（zmjiudian）内查看。";
                //}
                SMServiceController.SendSMS(firstCoupon.PhoneNum, kt);

                ProductAdapter.UpdateGroupPurchase(coe.GroupId, (int)Enums.GroupState.进行中);
                detailmodel.IsSponsor = true;
            }
            else
            {
                string ct = "您已参团，现在邀请更多朋友来参团吧。拼团进度可在周末酒店APP>>我的>>我的订单>>房券内查看，同样信息也可在周末酒店公众号（zmjiudian）内查看。";
                if (firstCoupon.ActivityType == 600)
                {
                    ct = "您已参团，现在邀请更多朋友来参团吧。拼团进度可在周末酒店APP>>我的>>我的订单>>消费券内查看，同样信息也可在周末酒店公众号（zmjiudian）内查看。";
                }
                SMServiceController.SendSMS(firstCoupon.PhoneNum, ct);
                detailmodel.IsSponsor = false;
            }
            detailmodel.UserId = coe.UserId;
            detailmodel.JoinTime = System.DateTime.Now;
            detailmodel.GroupId = coe.GroupId;
            detailmodel.OpenId = coe.GroupPurchase.OpenId;
            ProductAdapter.AddGroupPurchaseDetail(detailmodel);

            //根据groupid  获取拼团参加人数
            int groupNum = ProductAdapter.GetGroupPurchaseDetailCountByGroupId(coe.GroupId);
            //根据activeid获取成团数量
            CouponActivityEntity couponActivity = ProductCache.GetOneCouponActivity(coe.ActivityID);
            if (groupNum >= couponActivity.GroupCount)
            {

                if (groupNum == couponActivity.GroupCount)//购买人数等于成团数，更新拼团状态
                {
                    ProductAdapter.UpdateGroupPurchase(coe.GroupId, (int)Enums.GroupState.成功);
                    //发送拼团成功短信
                    GroupSuccessSendMsg(coe.GroupId, coe.HotelName, coe.PackageName, coe.ActivityType, couponActivity.ExchangeMethod, couponActivity.PageTitle);
                    AddGroupProductPromotionProductByGroupID(coe.GroupId);

                }
                if (groupNum > couponActivity.GroupCount)//拼团成功后  加入团 发送短信
                {
                    string exchangeno = string.Join(",", rightCouponList.Select(_ => _.ExchangeNo));
                    //var activityEntity = ProductCache.GetOneCouponActivity(firstCoupon.ActivityID);
                    var sms = "";
                    if (coe.ActivityType == 200)
                    {
                        sms = string.Format(GroupSuccessMsg200, coe.HotelName, coe.PackageName, exchangeno);
                    }
                    else if (coe.ActivityType == 600)
                    {
                        sms = string.Format(GroupSuccessMsg600, exchangeno);
                    }
                    //var sms = string.Format(GroupSuccessMsg200, coe.HotelName, coe.PackageName, exchangeno);
                    try
                    {
                        SKUEntity skuEntity = ProductAdapter.GetSKUEXEntityByID(firstCoupon.SKUID);
                        if ((firstCoupon.CID > 0 && firstCoupon.ExchangeMethod == 6) || skuEntity.SMSType != 2)
                        {
                            SMServiceController.SendSMS(firstCoupon.PhoneNum, sms);
                        }
                        else if (skuEntity.SMSType == 2)
                        {
                            SMServiceController.SendSMS(firstCoupon.PhoneNum, skuEntity.SMSConten);
                        }
                        else if (couponActivity.ExchangeMethod == 5)
                        {
                            SMServiceController.SendSMS(firstCoupon.PhoneNum, string.Format(QRCodeMsg, couponActivity.PageTitle));
                        }
                        else
                        {
                            SMServiceController.SendSMS(firstCoupon.PhoneNum, sms);
                        }

                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("短信发送异常" + ex.Message);
                    }

                    List<ExchangeCouponEntity> couponList = coe.ExchangeCouponList.Select(_ => (ExchangeCouponEntity)_).ToList();
                    AddGroupProductPromotionProduct(couponList);
                }
            }
            coe.GroupPurchase.GroupPeople = ProductAdapter.GetGroupPurchaseDetailByGroupId(coe.GroupId);
            coe.GroupPurchase.GroupPeople.ForEach(m => m.AvatarUrl = (string.IsNullOrWhiteSpace(m.AvatarUrl) ? DescriptionHelper.defaultAvatar :
            PhotoAdapter.GenHotelPicUrl(m.AvatarUrl, Enums.AppPhotoSize.jupiter)));
            int groupCount = coe.GroupPurchase.GroupPeople.Count;
            coe.GroupPurchase.GroupShortageCount = couponActivity.GroupCount - groupCount;

        }

        /// <summary>
        /// 分销订单推送
        /// </summary>
        /// <param name="orderId">订单id</param>
        /// <param name="pageTitle">分销名称</param>
        /// <param name="orderTime">下单时间 yyyy年MM月dd日 HH:mm</param>
        /// <param name="price">下单金额</param>
        [HttpGet]
        public void WxFWHRetailMessage(long cid, long orderId, string pageTitle, string orderTime, string price)
        {
            WeixinUser wxUserInfo = WeiXinAdapter.GetOpenidByUid(cid, "shanglvzhoumo");
            //周末酒店服务号
            string tempId = "57GrHgxxc7FYIQoaHMQs0UquzMGN_JtpfflahNQ5rMc";
            //data list
            var dataList = new List<string>();
            dataList.Add(orderId.ToString());
            dataList.Add(pageTitle);
            dataList.Add(orderTime);
            dataList.Add(price + "元");
            dataList.Add("度假伙伴");

            //整理成微信模板消息对象
            var _wxTempEntity = new WeixinTemplateMessageEntity
            {
                WeixinAccount = WeiXinChannelCode.周末酒店服务号,
                ToOpenId = wxUserInfo.Openid,
                TemplateId = tempId,
                TemplateUrl = "http://partner.zmjiudian.com/channel/MOrderTypeList",
                DataFirst = "恭喜你有新的分销订单啦！",
                DataRemark = ">>进入店铺了解更多佣金详情",
                DataKeywords = dataList
            };

            //执行发送模板消息
            WeiXinAdapter.SendTemplateMessage(_wxTempEntity);
        }

        /// <summary>
        /// 成功后根据团ID给已提交的团订单补发促销产品
        /// </summary>
        /// <param name="GroupId">团ID</param>
        [HttpGet]
        public int AddGroupProductPromotionProductByGroupID(int GroupId)
        {
            List<ExchangeCouponEntity> couponList = CouponAdapter.GetExchangeCouponEntityListByGroupId(GroupId).Where(_ => _.State == 2).ToList();

            AddGroupProductPromotionProduct(couponList);
            return couponList.Count;
        }

        /// <summary>
        /// 批量领取现金券
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="cashCouponIds">现金券id</param>
        /// <returns></returns>
        [HttpGet]
        public bool SendCashCoupon(long userId, string cashCouponIds)
        {
            List<int> cashCouponIdList = new List<int>();
            foreach (string cashCouponId in cashCouponIds.Split(','))
            {
                cashCouponIdList.Add(int.Parse(cashCouponId));
            }

            return CouponAdapter.SendCashCoupon(userId, cashCouponIdList);
        }

        /// <summary>
        /// 检查用户是否已领取过指定的现金券
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cashCouponIds"></param>
        /// <returns></returns>
        [HttpGet]
        public bool CheckUserCouponItemByUserIdAndCouponDefineIdlist(long userId, string cashCouponIds)
        {
            List<int> cashCouponIdList = cashCouponIds.Split(',').ToList().Select(_ => Convert.ToInt32(_)).ToList();
            List<UserCouponItemInfoEntity> userCouponItemList = CouponAdapter.GetUserCouponItemByUserIdAndCouponDefineIdlist(userId, cashCouponIdList);
            List<int> userCouponItemIdList = userCouponItemList.Select(_ => _.CouponDefineID).Distinct().ToList();
            if (cashCouponIdList.All(userCouponItemIdList.Contains) && cashCouponIdList.Count == userCouponItemIdList.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void AddGroupProductPromotionProduct(List<ExchangeCouponEntity> couponList)
        {
            ProductServiceEnums.ProductType productType = ProductServiceEnums.ProductType.ProductCoupon;

            foreach (var oneUserCouponList in couponList.GroupBy(_ => _.UserID))
            {
                var firstCoupon = oneUserCouponList.First();

                GenCouponPromotProducts(firstCoupon.ID, firstCoupon.PhoneNum, firstCoupon.UserID, firstCoupon.CID, oneUserCouponList.Count(), productType, firstCoupon.SKUID, firstCoupon.PayID, firstCoupon.SKUID, firstCoupon.CouponOrderId);

                OrderHelper.AddOrderToRedis(firstCoupon.CouponOrderId);
            }
        }

        private void GenStepGroupSMS(RoomCouponOrderEntity coe, List<ExchangeCouponModel> rightCouponList, int tagId)
        {
            var couponOrder = rightCouponList.First();
            string phoneNum = couponOrder.PhoneNum;
            int activityID = couponOrder.ActivityID;
            int skuid = couponOrder.SKUID;
            var activityEntity = ProductCache.GetOneCouponActivity(activityID);
            var sms = string.Format("您支付的产品{0}定金￥{1}已收到，点击http://www.zmjiudian.com/coupon/stepgroup/product/{2}查看活动进程。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。如需服务，请致电4000-021-702或关注“周末酒店服务号”微信进行在线咨询。"
                , activityEntity.PageTitle, couponOrder.OriPrice, skuid);
            if (tagId == 1)
            { }
            else
            {
                sms = string.Format("您定购的产品{0}已成团，请到微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中支付尾款。如需服务，请致电4000-021-702或关注“周末酒店服务号”回复“服务”。"
                  , activityEntity.PageTitle);
            }
            try
            {
                if (activityEntity.ExchangeMethod == 5)
                {
                    SMServiceController.SendSMS(phoneNum, string.Format(QRCodeMsg, activityEntity.PageTitle));
                }
                else
                {
                    SMServiceController.SendSMS(phoneNum, sms);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("短信发送异常" + ex.Message);
            }
        }

        private void GenCouponPaySMS(RoomCouponOrderEntity coe, List<ExchangeCouponModel> rightCouponList, int buyNum)
        {
            var couponOrder = rightCouponList.First();
            var bSendSMS = CommMethods.CheckSendSMSByPayType(couponOrder.PayType);
            int couponID = couponOrder.ID;
            string phoneNum = couponOrder.PhoneNum;
            int activityID = couponOrder.ActivityID;
            int activityType = couponOrder.ActivityType;
            long userID = couponOrder.UserID;
            long CID = coe.CID;

            //更新过房券状态后 不再从数据库取房券记录。直接设置房券状态为2 已支付，便于页面据此状态判断是否购买成功
            coe.State = 2;
            rightCouponList.ForEach(_ => _.State = 2);
            //供应商大于0 表示走第三方券码
            if (couponOrder.ExchangeNoType == 1)
            {

                SPUEntity spumodel = ProductAdapter.GetSPUBySKUID(couponOrder.SKUID);
                var supplierModel = SupplierAdapter.GetSupplierById(spumodel.SupplierID);
                if (!string.IsNullOrWhiteSpace(supplierModel.SMSTemplate))
                {
                    var smstemplate = supplierModel.SMSTemplate.Replace("{couponNO}", string.Join("、", rightCouponList.Select(_ => _.ExchangeNo.Trim())));
                    if (bSendSMS)
                    {
                        SMServiceController.SendSMS(phoneNum, smstemplate);
                    }
                }
                Log.WriteLog("GenCouponPay " + phoneNum + "supplierModel.SMSTemplate：" + supplierModel.SMSTemplate);
            }
            else
            {
                var activityEntity = ProductCache.GetOneCouponActivity(activityID);
                var sku = ProductAdapter.GetSKUEXEntityByID(rightCouponList[0].SKUID);
                switch (coe.ActivityType)
                {
                    case 400://ToDo 插入会员角色记录 
                        CreateVIPRole(userID, phoneNum, activityID, CID);
                        AccountEnums.ChangeType ChangeType = AccountEnums.ChangeType.VIP;
                        if (activityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599)
                        {
                            if (couponOrder.Price == 400)
                            {
                                ChangeType = AccountEnums.ChangeType.VIP599Update;
                            }
                            else
                            {
                                ChangeType = AccountEnums.ChangeType.VIP599;
                            }
                        }

                        AccountAdapter.AddUserChannelRelHistory(new UserChannelRelHistoryEntity { CreateTime = DateTime.Now, CID = CID, ChangeType = (int)ChangeType, UserID = userID, RelBizInfo = couponOrder.ID.ToString() });
                        DealVIPTempActivity(couponOrder);
                        coe.PayResultMessage = "周末酒店会员购买成功!";

                        if (coe.CouponPrice > 0) //用钱购买的VIP才给积分
                        {
                            PresentPointsForVIP(userID, ChangeType, coe.ExchangeCouponList.First().ID);
                        }
                        Log.WriteLog("CID " + (coe.CID > 0) + " ToDateTime " + (Convert.ToDateTime("2017-09-25 22:00:00") > System.DateTime.Now));
                        if (coe.CID > 0 && Convert.ToDateTime("2017-09-25 22:00:00") > System.DateTime.Now)
                        {
                            User_Info ui = AccountAdapter.GetUserInfoByUserId(coe.CID);
                            if (ui != null)
                            {

                                List<ExchangeCouponEntity> exchangeList = CouponAdapter.GetExchangeCouponEntityByCID(coe.CID).Where(_ => _.ActivityType == 400 && _.State == 2 && _.CreateTime < Convert.ToDateTime("2017-09-25 22:00:00")).ToList();
                                string smsVip = "您已成功邀请" + exchangeList.Count + "位好友成为VIP会员，每成功一位您将获得一套联票，上不封顶！活动截止9月25日晚上22点，之后请关注“周末酒店服务号”回复“邀请奖励”，领取奖励联票。";
                                if (bSendSMS)
                                {
                                    SMServiceController.SendSMS(ui.MobileAccount, smsVip);
                                }
                            }
                        }
                        break;
                    case 500:
                        if (activityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.TTGY) //&& CID == 160)
                        {
                            var sms1 = "度假礼包购买成功！礼包所含权益领取请查看：http://dm12.me/6k15";// string.Format(buyCouponSuccessTemplate, coe.HotelName, coe.PackageName, rightCouponList.Count, string.Join("和", rightCouponList.Select(_ => _.ExchangeNo.Trim())));//插入sqlserver的数据记录 自带尾空格 使用时除去
                            try
                            {
                                if (bSendSMS)
                                {
                                    if ((couponOrder.CID > 0 && couponOrder.ExchangeMethod == 6) || sku.SMSType != 2)
                                    {
                                        SMServiceController.SendSMS(phoneNum, sku.SMSConten);
                                    }
                                    else
                                    {
                                        SMServiceController.SendSMS(phoneNum, sms1);
                                    }
                                    //if (sku.SMSType == 2 && (couponOrder.CID != 0 && couponOrder.ExchangeMethod != 6))
                                    //{
                                    //    SMServiceController.SendSMS(phoneNum, sku.SMSConten);
                                    //}
                                    //else
                                    //{
                                    //    SMServiceController.SendSMS(phoneNum, sms1);
                                    //}
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLog("短信发送异常" + ex.Message);
                            }
                        }
                        else
                        {

                            try
                            {
                                if (bSendSMS)
                                {
                                    if ((couponOrder.CID > 0 && couponOrder.ExchangeMethod == 6) || sku.SMSType != 2)
                                    {
                                        string sms500 = string.Format(CouponAdapter.buyCouponSuccessTemplate500, activityEntity.PageTitle, sku.Name, rightCouponList.Count, string.Join("、", rightCouponList.Select(_ => _.ExchangeNo.Trim())));
                                        SMServiceController.SendSMS(phoneNum, sms500);
                                    }
                                    else
                                    {
                                        SMServiceController.SendSMS(phoneNum, sku.SMSConten);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLog("短信发送异常" + ex.Message);
                            }

                        }
                        coe.PayResultMessage = "您已成功购买房券";
                        break;
                    case 600: //消费券
                        //var activityEntity = ProductCache.GetOneCouponActivity(activityID);
                        var payType = rightCouponList[0].PayType;
                        string smsTemplate = CouponAdapter.buyCouponSuccessTemplate600;
                        var sms600 = string.Empty;

                        if (rightCouponList[0].Points > 0)
                        {
                            smsTemplate = CouponAdapter.buyCouponSuccessTemplate600Points;
                        }
                        else if (rightCouponList[0].Price == 0 && rightCouponList[0].Points == 0)
                        {
                            smsTemplate = CouponAdapter.buyCouponSuccessTemplate600Points_Free;
                        }
                        sms600 = string.Format(smsTemplate, activityEntity.PageTitle, sku.Name, rightCouponList.Count, string.Join("、", rightCouponList.Select(_ => _.ExchangeNo.Trim())));//插入sqlserver的数据记录 自带尾空格 使用时除去

                        try
                        {
                            //如果是壳产品 购买成功短信中不要出现 券号为XXXX 的内容
                            var promotionRuleList = ProductAdapter.GetPromotionRuleListBySKU(sku.ID);
                            if (promotionRuleList.Count > 0)
                            {
                                smsTemplate = CouponAdapter.buyCouponSuccessPromotionTemplate600;
                                sms600 = string.Format(smsTemplate, activityEntity.PageTitle, sku.Name, rightCouponList.Count);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog("判断壳产品异常" + ex.Message);
                        }
                        try
                        {
                            if (bSendSMS)
                            {
                                #region zcf 当消费券产品选择购买后预约并且使用系统短信的时候，需要用以下短信模板
                                if (sku.BookPosition == 2 && sku.SMSType == 0)
                                {
                                    var smsSystem = string.Format(CouponAdapter.buyCouponSuccessTemplate600_SystemInfo, sku.Name, string.Join("、", rightCouponList.Select(_ => _.ExchangeNo.Trim())));
                                    SMServiceController.SendSMS(phoneNum, smsSystem);
                                    Log.WriteLog("测试系统短信模板：" + smsSystem);
                                }
                                #endregion
                                else if ((couponOrder.CID > 0 && couponOrder.ExchangeMethod == 6) || sku.SMSType != 2)
                                {
                                    SMServiceController.SendSMS(phoneNum, sms600);
                                }
                                else if (sku.SMSType == 2)
                                {
                                    SMServiceController.SendSMS(phoneNum, sku.SMSConten);
                                }
                                else if (activityEntity.ExchangeMethod == 5)
                                {
                                    SMServiceController.SendSMS(phoneNum, string.Format(QRCodeMsg, activityEntity.PageTitle));
                                }
                                //else
                                //{
                                //    SMServiceController.SendSMS(phoneNum, sku.SMSConten);
                                //}
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog("短信发送异常" + ex.Message);
                        }

                        coe.PayResultMessage = "您已成功购买消费券";
                        break;
                    default:
                        string sms = string.Format(CouponAdapter.buyCouponSuccessTemplate200, coe.HotelName, coe.PackageName, rightCouponList.Count, string.Join("、", rightCouponList.Select(_ => _.ExchangeNo.Trim())));//插入sqlserver的数据记录 自带尾空格 使用时除去

                        //sms = GenPointsCouponSMS(rightCouponList[0], activityID, rightCouponList, sms);


                        try
                        {
                            if (bSendSMS)
                            {
                                if ((couponOrder.CID > 0 && couponOrder.ExchangeMethod == 6) || sku.SMSType != 2)
                                {
                                    SMServiceController.SendSMS(phoneNum, sms);
                                }
                                else if (sku.SMSType == 2)
                                {
                                    SMServiceController.SendSMS(phoneNum, sku.SMSConten);
                                }
                                else if (activityEntity.ExchangeMethod == 5)
                                {
                                    SMServiceController.SendSMS(phoneNum, string.Format(QRCodeMsg, activityEntity.PageTitle));
                                }
                                else
                                {
                                    SMServiceController.SendSMS(phoneNum, sms);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog("短信发送异常" + ex.Message);
                        }

                        coe.PayResultMessage = "您已成功购买房券";
                        break;
                }
            }

            try
            {
                //判断CID是否大于0。大于0 发微信服务号提示。
                if (couponOrder.CID > 100000 && couponOrder.ActivityType != 400)
                {
                    WxFWHRetailMessage(couponOrder.CID, (long)couponOrder.ID, couponOrder.PageTitle, couponOrder.CreateTime.ToString("yyyy年MM月dd日 HH:mm"), couponOrder.Price.ToString());
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("单买 服务号 推送分销订单失败：" + e);
            }

        }

        [HttpGet]
        public SPUEntity GetSPUBySKUID(int skuid)
        {
            SPUEntity spumodel = ProductAdapter.GetSPUBySKUID(skuid);
            return spumodel;
        }

        [HttpGet]
        public OperationResult ReSendSMS(int CouponID, int PayID, int ThirdType)//0：失败   1：成功     -1：暂未获取到携程短信内容   -2：暂未生成第三方订单
        {
            OperationResult result = new OperationResult();
            int smsRes = 0;
            switch (ThirdType)
            {
                case 0://本地短信
                    smsRes = SendCoupon600(CouponID, PayID);
                    break;
                case 1://驴妈妈短信
                    smsRes = SendLmmCoupon(CouponID);
                    break;
                case 2://携程短信
                    smsRes = SendCtripCoupon(CouponID);
                    break;
                case 3://同程短信
                    smsRes = SendTtripCoupon(CouponID);
                    break;
                case 4://自我游短信
                    smsRes = SendZwyCoupon(CouponID);
                    break;
                default:
                    break;
            }
            result.Success = true;
            result.Message = GetTipMessage(smsRes);
            return result;
        }

        public int SendCoupon600(int CouponID, int PayID)
        {
            RoomCouponOrderEntity coe = new RoomCouponOrderEntity();
            List<ExchangeCouponEntity> couponlist = new List<ExchangeCouponEntity>();
            int activityID = 0;
            bool bSendSMS = false;
            SKUEntity sku = new SKUEntity();
            string smsTemplate = CouponAdapter.buyCouponSuccessTemplate600;
            int Counts = 0;
            var sms600 = string.Empty;
            string content = string.Empty;
            string phoneNum = string.Empty;
            if (PayID > 0)
            {
                coe = CouponAdapter.GetOneRoomCouponOrderEntityByPayID(PayID);
                var rightCouponList = coe.ExchangeCouponList.FindAll(_ => _.PayType != 0);
                var couponOrder = rightCouponList.FirstOrDefault();
                phoneNum = couponOrder.PhoneNum;
                bSendSMS = CommMethods.CheckSendSMSByPayType(couponOrder.PayType);
                activityID = couponOrder.ActivityID;

                sku = ProductAdapter.GetSKUEXEntityByID(couponOrder.SKUID);
                int payType = couponOrder.PayType;

                if (rightCouponList[0].Points > 0)
                {
                    smsTemplate = CouponAdapter.buyCouponSuccessTemplate600Points;
                }
                else if (rightCouponList[0].Price == 0 && rightCouponList[0].Points == 0)
                {
                    smsTemplate = CouponAdapter.buyCouponSuccessTemplate600Points_Free;
                }
                Counts = rightCouponList.Count;
                content = string.Join("、", rightCouponList.Select(_ => _.ExchangeNo.Trim()));
            }
            else
            {
                couponlist = CouponAdapter.GetExchangeCouponEntityListByIDList(new List<int>() { CouponID });
                var couponOrder = couponlist.FirstOrDefault();
                activityID = couponOrder.ActivityID;
                phoneNum = couponOrder.PhoneNum;
                bSendSMS = CommMethods.CheckSendSMSByPayType(couponOrder.PayType);
                sku = ProductAdapter.GetSKUEXEntityByID(couponOrder.SKUID);
                int payType = couponOrder.PayType;
                if (couponOrder.Points > 0)
                {
                    smsTemplate = CouponAdapter.buyCouponSuccessTemplate600Points;
                }
                else if (couponOrder.Price == 0 && couponOrder.Points == 0)
                {
                    smsTemplate = CouponAdapter.buyCouponSuccessTemplate600Points_Free;
                }
                Counts = couponlist.Count;
                content = string.Join("、", couponlist.Select(_ => _.ExchangeNo.Trim()));
            }
            var activityEntity = ProductCache.GetOneCouponActivity(activityID);
            sms600 = string.Format(smsTemplate, activityEntity.PageTitle, sku.Name, Counts, content);

            try
            {
                if (sku.SMSType == 2)
                {
                    SMServiceController.SendSMS(phoneNum, sku.SMSConten);
                }
                else
                {
                    if (bSendSMS)
                    {
                        SMServiceController.SendSMS(phoneNum, sms600);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("短信发送异常" + ex.Message);
                return 0;
            }
            return 1;
        }

        public int SendLmmCoupon(int CouponID)
        {
            string orderid = CouponAdapter.GetThirdCouponOrderID(CouponID, 1);
            if (!string.IsNullOrEmpty(orderid))
            {
                thirdpartyService.resendCode((long)CouponID, long.Parse(orderid));
                return 1;//成功
            }
            else
            {
                return -2;//失败
            }
        }

        public int SendCtripCoupon(int CouponID)
        {
            StringBuilder smsContent = new StringBuilder();
            string thirdorderid = couponService.GetThirdCouponOrderID(CouponID, 2);
            if (!string.IsNullOrEmpty(thirdorderid))
            {
                List<ThirdPartyOrderSMSEntity> smslist = thirdpartyService.GetThirdPartySMSByCouponID(CouponID, 2);
                string thirdsms = string.Empty;
                if (smslist.Count > 0)
                {
                    thirdsms = smslist.FirstOrDefault().SMSContent;
                }
                else
                {
                    thirdsms = thirdpartyService.GetTicketSMSContent(long.Parse(thirdorderid));

                    ThirdPartyOrderSMSEntity ordersms = new ThirdPartyOrderSMSEntity();
                    ordersms.ThirdType = 2;
                    ordersms.ThirdOrderID = thirdorderid;
                    ordersms.SMSContent = thirdsms;
                    ordersms.CreateTime = DateTime.Now;
                    ordersms.CouponID = CouponID;
                    thirdpartyService.AddThirdPartyOrderSMS(ordersms);
                }
                CtripSmsContent model = JsonConvert.DeserializeObject<CtripSmsContent>(thirdsms);
                if (model.VoucherOrderItemInfoList.Count > 0)
                {
                    OrderVoucherItemType sms = model.VoucherOrderItemInfoList[0];
                    Log.WriteLog("TestSMS:" + sms.ResourceName + sms.Quantity.ToString() + (string.IsNullOrEmpty(sms.VendorVoucher) ? "" : sms.VendorVoucher) + sms.ProductUseMsg);
                    smsContent.AppendFormat("您已成功预订{0}， 份数:{1}，携程订单号:{4}， {2}， {3}  此单由周末酒店与携程网联合售卖", sms.ResourceName, sms.Quantity.ToString(), (string.IsNullOrEmpty(sms.VendorVoucher) ? "" : sms.VendorVoucher), sms.ProductUseMsg, thirdorderid);
                    List<ExchangeCouponEntity> ec = CouponAdapter.GetExchangeCouponEntityListByIDList(new List<int>() { CouponID });
                    SMServiceController.SendSMS(ec.FirstOrDefault().PhoneNum, smsContent.ToString());
                }
                else
                {
                    return -1;//"暂未获取到第三方短信内容";
                }
            }
            else
            {
                return -2;//"暂未生成第三方订单，请稍后再试";
            }
            return 1;//成功
        }
        public int SendTtripCoupon(int CouponID)
        {
            string orderid = CouponAdapter.GetThirdCouponOrderID(CouponID, 3);
            List<ExchangeCouponEntity> ec = CouponAdapter.GetExchangeCouponEntityListByIDList(new List<int>() { CouponID });
            if (!string.IsNullOrEmpty(orderid))
            {
                List<string> orderlist = new List<string>();
                orderlist.Add(orderid);
                thirdpartyService.TtripSMSResend(orderlist, ec.FirstOrDefault().PhoneNum);
                return 1;//成功
            }
            else
            {
                return -2;//失败
            }
        }

        /// <summary>
        /// 自我游重发短信
        /// </summary>
        /// <param name="CouponID"></param>
        /// <returns></returns>
        public int SendZwyCoupon(int CouponID)
        {

            string orderid = CouponAdapter.GetThirdCouponOrderID(CouponID, 4);
            List<ExchangeCouponEntity> ec = CouponAdapter.GetExchangeCouponEntityListByIDList(new List<int>() { CouponID });
            if (!string.IsNullOrEmpty(orderid))
            {
                thirdpartyService.ZwyMsgResend(orderid, ec.FirstOrDefault().PhoneNum);
                return 1;//成功
            }
            else
            {
                return -2;//失败
            }


        }




        public string GetTipMessage(int smsRes)
        {
            string message = string.Empty;
            switch (smsRes)
            {
                case 1:
                    message = "发送成功！";
                    break;
                case 0:
                    message = "发送失败！";
                    break;
                case -1:
                    message = "发送失败.暂未获取到携程短信内容";
                    break;
                case -2:
                    message = "发送失败.暂未生成第三方订单";
                    break;
                default:
                    message = "未知状态";
                    break;
            }
            return message;
        }

        /// <summary>
        /// 购买VIP送积分：新VIP：500 铂金：1000  升级：500 //   新VIP：1000 铂金：2000  升级：1000 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="ChangeType"></param>
        private void PresentPointsForVIP(long userID, AccountEnums.ChangeType changeType, int couponID)
        {
            int Points = 500;
            HotelServiceEnums.ConsumeUserPointsBizType pointBizType = HotelServiceEnums.ConsumeUserPointsBizType.NewVIP;
            switch (changeType)
            {
                case AccountEnums.ChangeType.VIP:
                    Points = 500;
                    break;
                case AccountEnums.ChangeType.VIP599Update:
                    Points = 500;
                    pointBizType = HotelServiceEnums.ConsumeUserPointsBizType.UpgradeVIP;
                    break;
                case AccountEnums.ChangeType.VIP599:
                    Points = 1000;
                    break;
            }

            //先取消     PointsAdapter.PresentPoints(couponID, pointBizType, userID, Points);
        }

        /// <summary>
        /// 处理邀请VIP送LOGO活动
        /// </summary>
        /// <param name="coe"></param>
        private void DealVIPTempActivity(ExchangeCouponModel coe)
        {
            //处理邀请VIP送LOGO活动
            if (DateTime.Now < DateTime.Parse("2017-7-11"))
            {
                if (coe.IsVIPInvatation)
                {
                    string sms = string.Format(
                           "亲爱的用户，你邀请的{0}***{1}已成功加入周末酒店VIP会员计划，恭喜你获得奖品-乐高玩具，周末酒店客服将在活动结束之后联系您领取奖品。奖品可累加，继续邀请朋友吧！活动时间截止2017年7月10日。",
              coe.PhoneNum.Substring(0, 3), coe.PhoneNum.Substring(coe.PhoneNum.Length - 4));

                    string phone = AccountAdapter.GetUserInfoByUserId(coe.CID).MobileAccount;

                    SMSAdapter.SendSMS(phone, sms);
                }
            }
            //处理买VIP送东东 
            if (coe.SKUID > 0 && coe.Price == 400) //指定的CID  //升级铂金不参与VIP送产品活动
            {
                //如果SKU为需要首单权限，那么购买的VIP将使用掉VIP首单权限
                var sku = ProductAdapter.GetSKUInfoByID(coe.SKUID);
                if (sku.SKU.HasTag(ProductServiceEnums.ProductTagType.NeedVIPFirstPayPriviledge))
                {
                    AccountAdapter.InsertOrDeleteUserPrivilegeRel(coe.UserID, (int)PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage, false, HJD.HotelManagementCenter.Domain.OpLogBizType.CIDVIPPromotion, coe.SKUID, " DealVIPTempActivity");
                }

            }

        }


        /// <summary>
        ///   处理临时券活动行为
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="activityID"></param>
        /// <param name="userID"></param>
        /// <param name="CID"></param>
        private static void DealTempCouponPromition(string phoneNum, int activityID, long userID, long CID)
        {
            if (activityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.TTGY)// && CID == 160)
            {
                ExchangeCouponEntity ec = new ExchangeCouponEntity()
                {
                    PayID = 0,
                    UserID = userID,
                    CID = CID,
                    PhoneNum = phoneNum,
                    ActivityID = (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR,
                    ActivityType = 400,
                    ExchangeNo = phoneNum,
                    InnerBuyGroup = 1,
                    Price = 0,
                    Type = 0,
                    State = 2,
                    AddInfo = "||"
                };

                List<UserRole> rl = AccountAdapter.GetUserRole(userID);

                if (rl.Count > 0)
                {
                    if (rl.Where(r => r.RoleID == (int)HJDAPI.Controllers.AccountAdapter.UserRoleEnum.VIP6M).Count() == 1)  //如果是6MVIP 
                    {
                        CreateVIPRole(userID, phoneNum, (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR, CID); // 发会员身份
                        CouponAdapter.InsertExchangeCoupon(ec);
                        Send200Coupon(phoneNum, userID);
                    }
                    else //如果是其它VIP，那么只发券
                    {
                        Send200Coupon(phoneNum, userID);
                    }
                }
                else
                {
                    CreateVIPRole(userID, phoneNum, (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR, CID); // 发会员身份
                    CouponAdapter.InsertExchangeCoupon(ec);
                    Send200Coupon(phoneNum, userID);
                }
            }
        }
        private static void Send200Coupon(string phoneNum, long userID)
        {

            CouponAdapter.GiveCouponByActivityType(userID, CouponActivityCode.cashcoupon200, phoneNum); //发200元现金券

            MessageAdapter.InsertSysMessage(new MessageService.Contract.SysMessageEnitity()
            {
                state = 0,
                businessID = 131,//数据库SysNotice表中对应的ID
                businessType = 100,
                receiver = userID,
                sendNickName = "",
                CreateTime = DateTime.Now
            });

            MessageAdapter.InsertSysMessage(new MessageService.Contract.SysMessageEnitity()
            {
                state = 0,
                businessID = 134,//数据库SysNotice表中对应的ID
                businessType = 100,
                receiver = userID,
                sendNickName = "",
                CreateTime = DateTime.Now
            });
            SMServiceController.SendNotice(131, userID.ToString(), "您已获得200元现金券，请在我的>>钱包>>现金券内查看");
            SMServiceController.SendNotice(134, userID.ToString(), "您已获得星级酒店房券一张，请在我的>>钱包>>房券中查看并兑换使用");
        }
        //return isNewRegistUser;




        private static void CreateVIPRole(long userID, string phoneNum, int activityID, long CID)
        {
            DealVIPTempPromotion(userID, phoneNum, activityID, CID);

            //判断是不是升级
            bool isUpdateToVIP = DealVIPUpdate(userID, activityID);
            bool isReNewVIP = isUpdateToVIP ? false : CouponAdapter.IsVIPRenew(activityID, userID);

            DealNewVIPRoleAndPrivilege(userID, activityID, CID, isUpdateToVIP);

            if (isReNewVIP) //如果是续费，那么不需要增加角色、权限，但需要 发现金券和延长新VIP时间及关闭原有VIP订单
            {
                //  CouponAdapter.PresentNewVIPGift(userID);   //取消，这会重复发 现金券
                CouponAdapter.ReNewVIPAfterPayment(userID);
            }


            var smsMsg400 = CouponAdapter.GetVIPConfirmSMS();
            try
            {
                SMServiceController.SendSMS(phoneNum, smsMsg400);
            }
            catch (Exception ex)
            {
                Log.WriteLog("短信发送异常" + ex.Message);
            }

            try
            {
                if (!AccountAdapter.NotRegistMobileAccount(phoneNum))
                {
                    int r = new Random().Next(100000, 999999);
                    string passWord = r.ToString();
                    string phone = phoneNum;
                    AccountAdapter.RegisterPhoneUser(phone, passWord, 2, 0, "", true, CID: CID);
                    string registMsg = string.Format(@"你的临时密码为：{0}。登录周末酒店APP，可在设置页面更改密码。点击下载周末酒店APP：http://app.zmjiudian.com", passWord);
                    SMServiceController.SendSMS(phone, registMsg);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("未注册用户购买vip注册异常" + ex.Message);
            }

        }

        /// <summary>
        /// 临时活动
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="phoneNum"></param>
        private static void DealVIPTempPromotion(long userID, string phoneNum, int activityID, long CID)
        {
            DateTime startTime = DateTime.Parse("2017-03-13");
            DateTime endTime = DateTime.Parse("2017-03-23");
            if (endTime >= System.DateTime.Now && startTime <= System.DateTime.Now)
            {
                var userRoleList = AccountAdapter.GetUserRoleRelByUserId(userID);

                if (userRoleList != null
                    && userRoleList.Count > 0
                    && userRoleList.Where(r => r.RoleID == (int)HJDAPI.Controllers.AccountAdapter.UserRoleEnum.VIP6M).Count() > 0)
                {
                    if (CID == 160) //如果是天天果园，则不送
                    {

                    }
                    else
                    {
                        CouponAdapter.GiveCouponByActivityType(userID, CouponActivityCode.upvip, phoneNum);
                        Log.WriteLog(string.Format("限时活动赠送现金券：userid：{0}，fund：{1}，时间：{2}", userID, phoneNum, DateTime.Now));

                        MessageAdapter.InsertSysMessage(new MessageService.Contract.SysMessageEnitity()
                        {
                            state = 0,
                            businessID = 119,//数据库SysNotice表中对应的ID
                            businessType = 100,
                            receiver = userID,
                            sendNickName = "",
                            CreateTime = DateTime.Now
                        });

                        SMServiceController.SendNotice(119, userID.ToString(), "升级VIP成功，您获赠的100元现金券已到账");
                    }
                }
            }
        }

        /// <summary>
        /// 处理购买VIP后的用户角色和权限
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="activityID"></param>
        /// <param name="CID"></param>
        private static void DealNewVIPRoleAndPrivilege(long userID, int activityID, long CID, bool isUpdateToVIP)
        {
            ChannelInfoEntity channel = ChannelAdapter.GetChannelInfo(CID);

            HJDAPI.Controllers.AccountAdapter.UserRoleEnum VIPRole = HJDAPI.Controllers.AccountAdapter.UserRoleEnum.VIP199NR;
            if (activityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599)
                VIPRole = HJDAPI.Controllers.AccountAdapter.UserRoleEnum.VIP599;

            AccountAdapter.UserRoleRelInsert(userID, (int)VIPRole);

            if (isUpdateToVIP == false)  //升级不过VIP首单权限
            {
                if (channel.CanBuyFisrtVIPPackage) //如果渠道允许购买首套VIP套餐
                {
                    AccountAdapter.InsertOrDeleteUserPrivilegeRel(userID, (int)PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage, true, HJD.HotelManagementCenter.Domain.OpLogBizType.BuyVIP, CID, "DealNewVIPRoleAndPrivilege");
                }
            }
        }

        /// <summary>
        /// 判断并处理用户VIP升级
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="aid"></param>
        private static bool DealVIPUpdate(long userID, int aid)
        {
            bool isUpdateToVIP = false;
            if (aid == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599)
            {
                var userRoleList = AccountAdapter.GetUserRoleRelByUserId(userID);
                if (userRoleList != null
                    && userRoleList.Count > 0
                    && userRoleList.Where(r => r.RoleID == (int)HJDAPI.Controllers.AccountAdapter.UserRoleEnum.VIP199
                    || r.RoleID == (int)HJDAPI.Controllers.AccountAdapter.UserRoleEnum.VIP199NR
                    ).Count() > 0)
                {
                    AccountAdapter.UserRoleRelDelete(userID, (int)HJDAPI.Controllers.AccountAdapter.UserRoleEnum.VIP199);
                    AccountAdapter.UserRoleRelDelete(userID, (int)HJDAPI.Controllers.AccountAdapter.UserRoleEnum.VIP199NR);
                    List<ExchangeCouponEntity> clist = CouponAdapter.GetExchangeCouponEntityListByUser(userID, 400);
                    if (clist != null)
                    {
                        var tclist = clist.Where(c => c.State == 2 && (
                            c.ActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199
                            || c.ActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR));
                        if (tclist.Count() > 0)
                        {
                            isUpdateToVIP = true;
                            foreach (var vip199Item in tclist)
                            {
                                vip199Item.State = (int)ExchangeCouponState.cancel;
                                CouponAdapter.UpdateExchangeCoupon(new ExchangeCouponEntity { PayID = 0, ID = vip199Item.ID, State = (int)ExchangeCouponState.cancel });
                            }
                        }
                    }
                }

            }

            return isUpdateToVIP;
        }

        [HttpGet]
        public int CouponRefund(int couponID = 0)
        {
            Log.WriteLog("CouponRefund  couponID:" + couponID.ToString());//记录看一下是否还有调用这个接口的地方 ，如果没有了，那就取消
            return CouponAdapter.CouponRefund(couponID);
        }


        [HttpGet]
        public int RefundOverdueCoupons()
        {

            List<ExchangeCouponEntity> eceList = CouponAdapter.GetWaitingRefundCouponList();
            if (eceList != null && eceList.Count != 0)
            {
                foreach (var ece in eceList)
                {
                    Add2RefundCoupon(ece.ID, ece.UserID, ece.UserID, ece.PhoneNum, "过期自动退");
                }
            }

            return eceList.Count;
        }

        [HttpGet]
        public RoomCouponActivityListModel GetSpeciallyCheapRoomCouponActivityList(int advID = 0, int groupNo = 0, int isVip = 0)
        {
            return CouponAdapter.GetSpeciallyCheapRoomCouponActivityListModel(advID, groupNo, isVip);
        }

        /// <summary>
        /// 小程序 专用
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="count"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public RoomCouponActivityListModel GetCouponActivityByAlbumSKU(int albumId, int count, int type)
        {
            return CouponAdapter.GetCouponActivityByAlbumSKU(albumId, type, count);
        }


        /// <summary>
        /// BG后台更新活动详情数据 重新设置缓存
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public int UpdateActivityManuSellNum4BG(int activityID, int type = 0, int manualSellNum = 0)
        {
            try
            {
                if (ProductCache.HasCouponSellInfoMem(activityID))//如果不存在，也就不需要后继的主动更新，以免在批量退款时产生不必要的动作
                {
                    ProductCache.RemoveCouponSellInfoMem(activityID);
                    ProductCache.RemoveCouponSellNumCache(activityID);
                    ProductCache.RemoveCouponLockNumCache(activityID);


                    //更新活动内容缓存
                    CouponActivityDetailModel model = CouponAdapter.GenCouponActivityDetail(activityID, true);
                    CouponAdapter.SetCacheActivityDetail(activityID, model);

                    CouponAdapter.UpdateSKUCouponCacheByActivityID(activityID);
                }
                ProductCache.RemoveStepGroupCahceWithSKUID(ProductCache.GetCouponActivitySKUSPURelCache(activityID).SKUID);


            }
            catch (Exception e)
            {
                Log.WriteLog("UpdateActivityManuSellNum4BG :" + e);
            }
            return 0;
        }

        /// <summary>
        /// 检查当前用户是否属于【预约 & [品鉴师|候选|有预订记录] & 非会员】的用户
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public int IsVipNoPayReserveUser(string userid)
        {
            return CouponAdapter.IsVipNoPayReserveUser(userid);
        }

        #endregion

        #region 后台房券兑换订单 修改订单状态
        /// <summary>
        /// 标记订单为使用预购券
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpGet]
        public bool MarkCouponPayOrder(long orderid, int SettlePrice, long curUserid)
        {
            return CouponAdapter.MarkCouponPayOrder(orderid, SettlePrice, curUserid);
        }
        #endregion

        #region 房券订单设置获取价格

        /// <summary>
        /// 提交兑换订单的参数
        /// </summary>
        /// <param name="submitParam"></param>
        /// <returns></returns>
        [HttpPost]
        public SubmitExchangeRoomOrderResult SubmitExchangeRoomOrder(SubmitExchangeRoomOrderParam submitParam)
        {
            return CouponAdapter.SubmitExchangeRoomOrder(submitParam);
        }

        /// <summary>
        /// 获取房券关联套餐的一些信息
        /// </summary>
        /// <param name="couponID"></param>
        /// <returns></returns>
        [HttpGet]
        public ExchangeCouponPackageInfo GetCouponPackageInfo(int couponID)
        {
            ExchangeCouponPackageInfo result = new ExchangeCouponPackageInfo();
            ExchangeCouponEntity rightCoupon = CouponAdapter.GetOneExchangeCouponInfoByCouponID(couponID);
            //首先券要存在 且 可以在App端兑换
            if (rightCoupon != null && rightCoupon.ID > 0 && rightCoupon.ExchangeMethod != 2)
            {
                int packageID = (int)rightCoupon.SourceID;//房券套餐ID
                int hotelID = rightCoupon.ObjectID;//酒店ID

                PackageInfoEntity pie = new PackageInfoEntity();

                if (hotelID > 0)
                {
                    pie = HotelAdapter.HotelService.GetHotelPackages(hotelID, packageID).First();

                    if (pie.Room.Options.Length == 0)
                    {
                        pie.Room.DefaultOption = "无烟";
                        pie.Room.Options = "无烟,吸烟";
                    }
                }
                CouponActivityEntity activityEntity = ProductCache.GetOneCouponActivity(rightCoupon.ActivityID, false);
                string[] priceArray = activityEntity.Price.Split(",，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);//价格信息

                if (priceArray != null && priceArray.Length != 0)
                {
                    int weekdayPrice = int.Parse(priceArray[0]);
                    int weekendPrice = priceArray.Length > 1 ? int.Parse(priceArray[1]) : 0;

                    IEnumerable<CouponRateEntity> couponRates = CouponAdapter.GetCouponRateList(rightCoupon.ActivityID);

                    result = new ExchangeCouponPackageInfo()
                    {
                        exchangeInfo = rightCoupon,
                        packageInfo = pie,
                        exchangeDateType = weekdayPrice * weekendPrice > 0 ? 0 : weekdayPrice > 0 ? 1 : weekendPrice > 0 ? 2 : 3,
                        canExchangeDates = CouponAdapter.genCanExchangeRoomDates(couponRates, activityEntity.ExpireTime),
                        isAllowMultiRoom = activityEntity.IsAllowMultiRoom
                    };
                }

            }

            return result;
        }

        /// <summary>
        /// 验证是否需要补差价
        ///如果需要消耗用户多张券，这里也需要进行检查提示
        /// </summary>
        /// <param name="submitParam"></param>
        /// <returns></returns>
        [HttpPost]
        public ExchangeRoomOrderConfirmResult IsExchangeNeedAddMoney(SubmitExchangeRoomOrderParam submitParam)
        {
            return CouponAdapter.IsExchangeNeedAddMoney(submitParam);
        }

        #endregion

        [HttpGet]
        public string ExceptionCollectTest()
        {
            throw new NullReferenceException()
            {

            };
        }

        #region 铂韬同步产品数据生成

        /// <summary>
        /// 
        /// </summary>
        /// <param name="merchantcode"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpGet]
        public ThirdPartyProductDataResult AvailableProductData([FromUri]ThirdPartyRequestProductDataParam param)
        {
            var result = new ThirdPartyProductDataResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(param.merchantcode)
                    && !string.IsNullOrWhiteSpace(param.sign)
                    && !string.IsNullOrWhiteSpace(param.noncestr)
                    && !string.IsNullOrWhiteSpace(param.starttime)
                    && !string.IsNullOrWhiteSpace(param.extrefnumber))
                {
                    Dictionary<string, string> dicKeyVal = new Dictionary<string, string>();
                    dicKeyVal.Add("merchantcode", param.merchantcode);
                    dicKeyVal.Add("starttime", param.starttime);
                    dicKeyVal.Add("timestamp", param.timestamp.ToString());
                    dicKeyVal.Add("noncestr", param.noncestr);
                    dicKeyVal.Add("extrefnumber", param.extrefnumber);

                    string resultSign = "";
                    if (Signature.VerifySignFromBoTao(dicKeyVal, true, param.sign, out resultSign))
                    {
                        var startTime = DateTime.Parse(param.starttime);
                        startTime = startTime < new DateTime(2000, 1, 1, 0, 0, 0) ? DateTime.Now : startTime;

                        int totalCount = 0;
                        var originActivityList = CouponAdapter.couponSvc.GetCouponActivityList(
                            new HJD.CouponService.Contracts.Entity.CouponActivityQueryParam()
                            {
                                stateArray = new int[] { 0, 1 },
                                PageSize = 20,
                                PageIndex = 1,
                                activityTypeArray = new int[] { 200 },
                                merchantCode = HJD.CouponService.Contracts.Entity.CouponActivityMerchant.bohuijinrong,
                                lastEditTime = startTime
                            }, out totalCount);

                        var activityIds = originActivityList.FindAll(_ => _.UpdateTime >= startTime).Select(_ => _.ID);
                        var tempCouponDetailList = new List<CouponActivityDetailModel>();
                        if (activityIds != null && activityIds.Count() != 0)
                        {
                            foreach (var activityID in activityIds)
                            {
                                var couponActivityDetail = GetCouponActivityDetail(activityID);
                                tempCouponDetailList.Add(couponActivityDetail);
                            }

                            var hotelList = HotelAdapter.GetHotelBasicInfoList(tempCouponDetailList.Select(_ => _.package.HotelID).Distinct());
                            var hotelIdAndCityNameDic = new Dictionary<int, string>();
                            foreach (var item in hotelList)
                            {
                                var districtName = HotelAdapter.GetHotelAssociatedZoneName(item.HotelID, item.DistrictID);
                                hotelIdAndCityNameDic.Add(item.HotelID, districtName);
                            }

                            var couponList = tempCouponDetailList.Select(_ => new ThirdPartyProductItem()
                            {
                                Address = hotelIdAndCityNameDic[_.package.HotelID],
                                AvailableCount = _.activity.TotalNum - _.activity.SellNum,
                                CityName = hotelIdAndCityNameDic[_.package.HotelID],
                                DetailUrl = string.Format("{1}/coupon/shop/{0}", _.activity.ID, Configs.WWWURL),
                                DiscountPrice = int.Parse(_.activity.Price.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First(p => p != "0")),
                                ExpirationDate = _.activity.ExpireTime,
                                HotelName = _.package.HotelName,
                                IsDel = _.activity.State == 0,
                                LastEditTime = _.activity.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss"),
                                MarketPrice = _.activity.MarketPrice,
                                PackageBrief = _.package.PackageBrief,
                                PackageName = _.package.PackageName,
                                PicUrl = _.activity.PicPath,
                                ProdcutDetail = _.activity.Description,
                                ProductCode = "coupon_" + _.activity.ID,
                                ProductType = ThirdPartyProductType.roomcoupon,
                                TotalCount = _.activity.TotalNum
                            });

                            result.Items.AddRange(couponList);//插入爆款产品
                        }

                        //Log.WriteLog("start同步非爆款产品");

                        result.Items.AddRange(HotelAdapter.GetThirdPartyProductItemList4Botao(startTime));//插入非爆款产品
                        //result.Items.Add(HotelAdapter.GenThirdPartyProductItem("http://p1.zmjiudian.com/115BN0N0OD_theme", string.Format("{0}/hotel/627383", Configs.WWWURL), "莫干山栖居酒店", "亲子采茶套餐A", "莫干山", "筏头乡庙前村10号", 100, 100, new DateTime(2016, 6, 30, 0, 0, 0), 0, 0, ThirdPartyProductType.hotel, "幽静大床房+2大1小早餐+双晚餐+水果+特色高山茶+儿童活动中心+游泳", "环境好	酒店环境非常优美，设置很高端，风景还不错，早上看日出很美，比较适合小资们一起去玩。", "hotel_627383", DateTime.Now, true));

                        result.ErrorInfo.result = true;
                        result.ErrorInfo.data = "";
                        result.ErrorInfo.errorcode = "0";
                        result.ErrorInfo.errormsg = "Success";
                    }
                    else
                    {
                        result.ErrorInfo.result = false;
                        result.ErrorInfo.data = "";
                        result.ErrorInfo.errorcode = "10002";
                        result.ErrorInfo.errormsg = "签名验证失败";
                    }
                }
                else
                {
                    result.ErrorInfo.result = false;
                    result.ErrorInfo.data = "";
                    result.ErrorInfo.errorcode = "10001";
                    result.ErrorInfo.errormsg = "参数错误";
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("同步产品出现异常" + ex.Message + ex.StackTrace);
            }
            return result;
        }

        #endregion

        #region 处理已退款会员 解除角色
        /// <summary>
        /// 处理退款列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public int CancelMemberRole4Batch()
        {
            var memberList = CouponAdapter.GetNeedCancelMemberUserList();
            return AccountAdapter.UpdateUserRoleRel4ManyUserId(memberList, 12, false);
        }
        #endregion

        #region 微信小程序闪购数据相关接口

        /// <summary>
        /// 【微信小程序专用】查询指定groupNo下的所有房券列表
        /// </summary>
        /// <param name="advID"></param>
        /// <param name="groupNo">123123超值团 1000微信VIP专享套餐转换</param>
        /// <param name="isVip"></param>
        /// <returns></returns>
        [HttpGet]
        public RoomCouponActivityListModel GetAllRoomCouponListForWxapp(int advID = 0, string groupNo = "123123,1000", int isVip = 0, int districtId = 0)
        {
            //首先获取完整列表
            var _listModel = CouponAdapter.GetAllRoomCouponListForWxapp(advID, groupNo, isVip);

            //如果传了districtId，则做districtId的筛选
            if (districtId > 0 && _listModel != null && _listModel.Items != null)
            {
                return new RoomCouponActivityListModel { TopPicUrl = _listModel.TopPicUrl, Items = _listModel.Items.Where(_ => _.DistrictId == districtId).ToList() };
            }

            return _listModel;
        }

        //【微信小程序专用】搜索指定groupNo下房券的城市与酒店信息
        /// <summary>
        /// 【微信小程序专用】搜索指定groupNo下房券的城市与酒店信息
        /// </summary>
        /// <param name="advID">广告ID</param>
        /// <param name="groupNo"></param>
        /// <param name="isVip"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]
        public List<QuickSearchSuggestItem> SearchRoomCouponAndDistrictForWxapp(int advID = 0, string groupNo = "123123,1000", int isVip = 0, string keyword = "")
        {
            List<QuickSearchSuggestItem> result = new List<QuickSearchSuggestItem>();
            //首先获取完整列表
            RoomCouponActivityListModel _listModel = CouponAdapter.GetAllRoomCouponListForWxapp(advID, groupNo, isVip);
            List<RoomCouponActivityListItem> dItems = _listModel.Items.FindAll(_ => _.DistrictName.Contains(keyword.Trim())).ToList();
            if (dItems.Count > 0)
            {
                foreach (var d in dItems)
                {
                    if (result.Where(_ => _.Id == d.DistrictId).Count() == 0)
                    {
                        QuickSearchSuggestItem item = new QuickSearchSuggestItem()
                        {
                            EName = d.DistrictEName == null ? "" : d.DistrictEName,
                            Icon = "",
                            Id = d.DistrictId,
                            Name = d.DistrictName,
                            ParentName = "",
                            Tag = "",
                            Type = "D",
                            ActionUrl = ""
                        };
                        result.Add(item);
                    }
                }
            }
            List<RoomCouponActivityListItem> hItems = _listModel.Items.Where(_ => _.HotelName.Contains(keyword.Trim())).ToList();
            if (hItems.Count > 0)
            {
                foreach (var h in hItems)
                {
                    QuickSearchSuggestItem item = new QuickSearchSuggestItem()
                    {
                        EName = "",
                        Icon = "",
                        Id = h.ActivityID,
                        Name = h.HotelName,
                        ParentName = "",
                        Tag = "",
                        Type = "H",
                        ActionUrl = ""
                    };
                    result.Add(item);
                }
            }

            return result;
        }

        //【微信小程序专用】获取指定groupNo下房券的所有的目的地信息
        [HttpGet]
        public List<DistrictInfoForWxappEntity> GetRoomCouponDistrictInfoForWxapp(int advID = 0, string groupNo = "123123,1000", int isVip = 0, int count = 0)
        {
            RoomCouponActivityListModel _listModel = CouponAdapter.GetAllRoomCouponListForWxapp(advID, groupNo, isVip);
            List<int> disIds = new List<int>();
            if (count == 0)
            {
                disIds = _listModel.Items.Select(_ => _.DistrictId).Distinct().OrderByDescending(_ => _listModel.Items.Count(p => p.DistrictId == _)).ToList();
            }
            else
            {
                disIds = _listModel.Items.Select(_ => _.DistrictId).Distinct().OrderByDescending(_ => _listModel.Items.Count(p => p.DistrictId == _)).Take(count).ToList();
            }
            var dis = CouponAdapter.GetRoomCouponDistrictInfoForWxapp(disIds, _listModel, count);
            return dis.OrderByDescending(_ => _listModel.Items.Count(p => p.DistrictId == _.DistrictId)).ToList();
        }
        /// <summary>
        ///【微信小程序专用】获取所有酒店下的目的地信息
        /// </summary>
        /// <param name="geoScopeType">geoScopeType:3周边，else 所有</param>
        /// <returns></returns>
        [HttpGet]
        public List<DistrictInfoForWxappEntity> GetHotelPackageDistrictInfoForWxapp(double lat = 0, double lng = 0, int count = 0, bool inChina = true, int geoScopeType = 3)
        {
            List<SimpleHotelEntity> hotelList = hotelService.GetHotelPackageDistrictInfo(lat, lng, geoScopeType);
            //Log.WriteLog("HotelId：" + string.Join(",", hotelList.Select(_ => _.HotelId)));
            List<int> disIds = hotelList.Select(_ => _.DistrictID).ToList();
            //Log.WriteLog("disids：" + string.Join(",", disIds));
            //List<DistrictInfoForWxappEntity> dis = CouponAdapter.GetHotelDistrictInfo(disIds, hotelList, lat, lng, geoScopeType, inChina).Where(_ => _.InChina == inChina).ToList();
            List<DistrictInfoForWxappEntity> dis = CouponAdapter.GetHotelDistrictInfo(disIds, hotelList, lat, lng, geoScopeType, inChina);
            if (count == 0)
            {
                dis = dis.OrderByDescending(_ => hotelList.Where(p => p.DistrictID == _.DistrictId).Sum(c => c.PackageCount)).ToList();
                //dis = dis.OrderByDescending(_ => hotelList.Count(p => p.DistrictID == _.DistrictId)).ToList();
            }
            else
            {
                dis = dis.OrderByDescending(_ => hotelList.Where(p => p.DistrictID == _.DistrictId).Sum(c => c.PackageCount)).Take(count).ToList();
                //dis = dis.OrderByDescending(_ => hotelList.Count(p => p.DistrictID == _.DistrictId)).Take(count).ToList();
            }
            return dis == null ? new List<DistrictInfoForWxappEntity>() : dis;
        }

        /// <summary>
        /// 【微信小程序专用】获取目的地下的所有套餐
        /// </summary>
        /// <param name="districtId">目的地id</param>
        /// <param name="checkIn">入住时间</param>
        /// <param name="checkOut">结束时间</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public RecommendHotelResult GetHotelPackageListForWxapp(int districtId, string checkIn, string checkOut, int pageIndex, int pageSize)
        {
            var result = new RecommendHotelResult()
            {
                HotelList = new List<RecommendHotelItem>(),
                HotelTotalCount = 0,
                HotelBlockTitle = ""
            };
            DateTime CheckIn = string.IsNullOrEmpty(checkIn) ? DateTime.Now : Convert.ToDateTime(checkIn);
            DateTime CheckOut = string.IsNullOrEmpty(checkOut) ? DateTime.Now : Convert.ToDateTime(checkOut);

            TimeSpan ts = CheckOut.Subtract(CheckIn);
            int totalCount = 0;
            List<PackageItemEntity> packageList = HotelAdapter.GetHotelPackageByDistrictId(districtId, CheckIn, CheckOut, pageIndex, pageSize, out totalCount);
            var regex = new Regex("_.*$", RegexOptions.Compiled | RegexOptions.RightToLeft);
            var hotelList = new List<RecommendHotelItem>();
            foreach (var _ in packageList)
            {

                var _item = new RecommendHotelItem();
                _item.HotelID = _.HotelID;
                _item.HotelName = _.HotelName;

                _item.HotelPicUrl = "";
                if (_.PicUrls != null && _.PicUrls.Count > 0)
                {
                    _item.HotelPicUrl = regex.Replace(_.PicUrls.First(), "_theme");
                }

                _item.HotelPrice = 0;
                _item.VIPPrice = 0;
                _item.TotalHotelPrice = 0;
                _item.TotalVIPPrice = 0;
                _item.NotVIPPrice = 0;

                if (_.PackagePrice != null && _.PackagePrice.Count > 0)
                {
                    if (_.PackagePrice.Exists(j => j.Type == 0))
                    {
                        var _singlePrice = _.PackagePrice.First(j => j.Type == 0).Price;

                        _item.HotelPrice = _singlePrice;
                        //_item.TotalHotelPrice = (_.DayLimitMin > 1 ? PriceAdapter.GetManyDaysPackagePrice(_.HotelID, _.PackageID, _singlePrice, _.DayLimitMin, (int)curUserID, false) : _singlePrice * _.DayLimitMin);
                    }

                    if (_.PackagePrice.Exists(j => j.Type == -1))
                    {
                        var _singlePrice = _.PackagePrice.First(j => j.Type == -1).Price;

                        _item.VIPPrice = _singlePrice;
                        //_item.TotalVIPPrice = (_.DayLimitMin > 1 ? PriceAdapter.GetManyDaysPackagePrice(_.HotelID, _.PackageID, _singlePrice, _.DayLimitMin, (int)curUserID, true) : _singlePrice * _.DayLimitMin);
                    }
                }

                _item.DayLimitMin = 0;
                _item.ADDescription = "";
                _item.MarketPrice = 0;
                _item.PackageBrief = _.PackageBrief;
                _item.PackageName = _.PackageName;
                _item.PID = _.PackageID;
                _item.RecommendPicUrl = "";
                _item.DistrictId = _.DistrictId;
                _item.DistrictName = "";
                _item.DistrictEName = "";
                _item.ProvinceName = "";
                _item.InChina = true;
                _item.CustomerType = 0;
                _item.ForVIPFirstBuy = _.ForVIPFirstBuy;
                _item.PackageOrderCount = _.OrderCount;

                hotelList.Add(_item);
            }
            result.HotelList = hotelList.OrderByDescending(_ => _.PackageOrderCount).ThenByDescending(l => (l.HotelPrice - l.VIPPrice)).ThenBy(p => p.VIPPrice).ToList();
            result.HotelTotalCount = totalCount;

            return result;
        }

        //【微信小程序专用】搜索可售套餐城市与酒店信息
        /// <summary>
        /// 【微信小程序专用】搜索可售套餐城市与酒店信息
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]
        public List<QuickSearchSuggestItem> SearchHotelAndDistrictForWxapp(string keyword = "")
        {
            List<QuickSearchSuggestItem> result = new List<QuickSearchSuggestItem>();
            //首先获取完整列表
            List<SimpleHotelEntity> hotelList = hotelService.GetHotelPackageDistrictInfo().Where(_ => _.DistrictName != null).ToList();
            List<SimpleHotelEntity> dItems = hotelList.FindAll(_ => _.DistrictName.Contains(keyword.Trim())).ToList();
            if (dItems.Count > 0)
            {
                foreach (var d in dItems)
                {
                    if (result.Where(_ => _.Id == d.DistrictID).Count() == 0)
                    {
                        QuickSearchSuggestItem item = new QuickSearchSuggestItem()
                        {
                            EName = d.DistrictEName == null ? "" : d.DistrictEName,
                            Icon = "",
                            Id = d.DistrictID,
                            Name = d.DistrictName,
                            ParentName = "",
                            Tag = "",
                            Type = "D",
                            ActionUrl = ""
                        };
                        result.Add(item);
                    }
                }
            }
            List<SimpleHotelEntity> hItems = hotelList.Where(_ => _.HotelName.Contains(keyword.Trim())).ToList();
            if (hItems.Count > 0)
            {
                foreach (var h in hItems)
                {
                    QuickSearchSuggestItem item = new QuickSearchSuggestItem()
                    {
                        EName = "",
                        Icon = "",
                        Id = h.HotelId,
                        Name = h.HotelName,
                        ParentName = "",
                        Tag = "",
                        Type = "H",
                        ActionUrl = ""
                    };
                    result.Add(item);
                }
            }

            return result;
        }
        #endregion


        /// <summary>
        /// 消费券部分退款
        /// </summary>
        /// <param name="couponID"></param>
        /// <param name="activityID"></param>
        /// <param name="phoneNum"></param>
        /// <param name="RefundPrice">部分退款金额</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse Add2PartRefundCoupon(int couponID, decimal partRefundPrice, long userId = 0, long editorUserID = 0, string phoneNum = "", string remark = "")
        {
            BaseResponse response = new BaseResponse { Message = "操作成功,等待退款完成", Success = 0 };
            try
            {
                if (editorUserID == 0) editorUserID = userId;
                if (phoneNum == "")
                {
                    phoneNum = AccountAdapter.GetUserInfoByUserId(userId).MobileAccount;
                }

                //验证券的状态是否为已支付未消费
                var userCoupons = CouponAdapter.GetOnePayExchangeCouponEntityByID(couponID);

                //增加状态40（支付完成后未收到第三方回调，再次提交订单造成）
                var targetCoupon = userCoupons.FirstOrDefault(_ => _.ID == couponID);
                if (targetCoupon == null)
                {
                    response.Success = BaseResponse.ResponseSuccessState.Coupon_NoInfo;// 1;
                    response.Message = "没有找到相应的券信息";
                }
                else
                {
                    // 增加如下退款规则：一次支付中，如果有0元支付的产品存在，那么，所有的产品都应该处于末消费状态。如果可以退，那么对0元的产品直接状态变为已取消，对于VIP，则删除VIP
                    var OneUserCoupons = userCoupons.Where(r => r.UserID == targetCoupon.UserID);

                    var refundPrice = targetCoupon.Price - targetCoupon.CashCouponAmount - targetCoupon.UserUseHousingFundAmount - targetCoupon.VoucherAmount;

                    refundPrice = partRefundPrice < refundPrice ? partRefundPrice : refundPrice;


                    //插入待退款列表 初始状态为1
                    CouponAdapter.AddRefundCoupon(new RefundCouponsEntity() { CouponID = couponID, State = 1, Creator = editorUserID, Updator = editorUserID, Remark = remark, Price = refundPrice });
                }
            }
            catch (Exception err)
            {
                Log.WriteLog(string.Format(" Add2PartRefundCoupon: couponID{0} userId{1} editorUserID{2} phoneNum{3}remark:{4} err:{5}", couponID, userId, editorUserID, phoneNum, remark, err.Message + err.StackTrace));
            }
            response.Message = string.Format("{0}:{1}", couponID, response.Message);
            return response;
        }


        [HttpGet]
        /// <summary>
        /// 第三方门票下单失败后，如果产品是下单失败直接退款的，那么直接退掉
        /// </summary>
        /// <param name="couponID">下单失败的券ID</param>
        /// <returns></returns>
        public BaseResponse ThirdPartyCouponOrderFailed(int couponID, long updator)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(couponID.ToString());
            BaseResponse response = new BaseResponse { Message = "操作成功,等待退款完成", Success = 0 };
            try
            {
                int operationState = 1;//待处理
                UpdateOperationState(couponID, updator, operationState);

                var orderInfo = CouponAdapter.GetOneExchangeCouponInfoByCouponID(couponID);
                sb.AppendLine(orderInfo.State.ToString());
                if (orderInfo.State == (int)ExchangeCouponState.paied)
                {
                    var sku = ProductAdapter.GetSKUEXEntityByID(orderInfo.SKUID);
                    sb.AppendLine(sku.IsRefundOnFail.ToString());
                    sb.AppendLine(orderInfo.PhoneNum);
                    if (sku.IsRefundOnFail == true)
                    {
                        string sms = string.Format("您预定的{0}消费券，经系统反馈出票失败，您支付的款项已经原路退回，退款周期1-3个工作日到账。如需服务，请致电4000-021-702(9:00-21:00)或关注“周末酒店服务号”微信（9：00-24：00）进行在线咨询。", sku.Name);
                        SMSAdapter.SendSMS(orderInfo.PhoneNum, sms);
                        response = Add2RefundCoupon(orderInfo.ID, userId: orderInfo.UserID, phoneNum: orderInfo.PhoneNum, remark: "第三方API下单失败后直接取消订单");
                    }
                }
            }
            catch (Exception err)
            {
                sb.AppendLine(err.Message + err.StackTrace);
            }
            Log.WriteLog("ThirdPartyCouponOrderFailed:" + sb.ToString());
            return response;
        }


        /// <summary>
        /// 消费券退款
        /// </summary>
        /// <param name="couponID"></param>
        /// <param name="activityID"></param>
        /// <param name="phoneNum"></param>
        /// <param name="partRefundPrice">部分退款金额，如果是倒退，那么为-1</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse Add2RefundCoupon(int couponID, long userId = 0, long editorUserID = 0, string phoneNum = "", string remark = "")
        {
            BaseResponse response = new BaseResponse { Message = "操作成功,等待退款完成", Success = 0 };


            try
            {
                if (editorUserID == 0) editorUserID = userId;
                if (phoneNum == "")
                {
                    phoneNum = AccountAdapter.GetUserInfoByUserId(userId).MobileAccount;
                }

                //验证券的状态是否为已支付未消费
                var userCoupons = CouponAdapter.GetOnePayExchangeCouponEntityByID(couponID);

                //增加状态40（支付完成后未收到第三方回调，再次提交订单造成）   --2017-12-26  增加已消费的可以退款（这个需求很奇怪） 
                var targetCoupon = userCoupons.FirstOrDefault(_ => (_.State == 2 || _.State == 6 || _.State == 8 || _.State == 40 || (_.State == 3 && _.ActivityType != 200)) && _.ID == couponID);
                if (targetCoupon == null)
                {
                    response.Success = BaseResponse.ResponseSuccessState.Coupon_NoInfo;// 1;
                    response.Message = "没有找到相应的券信息";
                }
                else if (targetCoupon.Price == 0 && targetCoupon.Points == 0 && targetCoupon.PromotionID > 0 && userCoupons.Count > 1)
                {
                    response.Success = BaseResponse.ResponseSuccessState.Coupon_GiftCanBeRefundSolely;// 4;
                    response.Message = "赠送券不能单独退款。";
                }
                else if (IsRefundVIPAndUsedCoupon(targetCoupon))
                {
                    response.Success = BaseResponse.ResponseSuccessState.VIP_UsedCouponOnRefund;// 4;
                    response.Message = "新VIP现金券礼已使用，不能取消VIP。";
                }
                else if (targetCoupon.PromotionID > 0)
                {
                    response.Success = BaseResponse.ResponseSuccessState.Coupon_SubCouponCannotRefund;// 4;
                    response.Message = "子产品不能退款。";
                }
                else
                {
                    // 取消这些规则： 增加如下退款规则：一次支付中，如果有0元支付的产品存在，那么，所有的产品都应该处于末消费状态。如果可以退，那么对0元的产品直接状态变为已取消，对于VIP，则删除VIP               
                    var OneUserCoupons = userCoupons.Where(r => r.UserID == targetCoupon.UserID);
                    var OnePayCoupons = OneUserCoupons.Where(r => r.PayID == targetCoupon.PayID);
                    var OneInnerPayGroupCoupons = OnePayCoupons.Where(r => r.InnerBuyGroup == targetCoupon.InnerBuyGroup);
                    var SubCoupons = OneInnerPayGroupCoupons.Where(r => r.PromotionID > 0);
                    var UsedCoupons = OneInnerPayGroupCoupons.Where(r => r.State == 3);

                    var refundPrice = targetCoupon.Price - targetCoupon.CashCouponAmount - targetCoupon.UserUseHousingFundAmount - targetCoupon.VoucherAmount;



                    if (SubCoupons.Count() > 0 && OneInnerPayGroupCoupons.Where(r => (r.State != 2 && r.State != 40)).Count() > 0)
                    {
                        response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.RelCouponHasUsed;// 2;
                        response.Message = "关联券码已使用，该券码不可单独作取消";
                    }
                    else  //可以退了
                    {
                        if (SubCoupons.Count() > 0)  //退赠券
                        {
                            SubCoupons.ToList().ForEach(c =>
                            {
                                c.State = (int)ExchangeCouponState.refund;
                                c.UpdateTime = DateTime.Now;
                                CouponAdapter.UpdateExchangeCoupon(c);

                                //目前购买券会送VIP，但对于退券时，希望所有的购买券都退了后才退VIP。所以这里需要增加一下退券规则
                                if (c.ActivityType == (int)MemberType.会员)
                                {
                                    bool isTheLastVIPCoupon = OnePayCoupons.Where(_ => _.ActivityType == 400 && _.State == (int)ExchangeCouponState.paied).Count() == 0;
                                    if (isTheLastVIPCoupon)
                                    {
                                        CancelUserRoleByActivityID(c.UserID, c.ActivityID);
                                    }
                                }
                                ProductCache.RemoveUserDetailOrderCache(c.ID);
                            });

                        }

                        //退已消费的 或已过期的 券  把券状态改为2
                        if (targetCoupon.State == 3 || targetCoupon.State == 8)
                        {
                            targetCoupon.UpdateTime = DateTime.Now;
                            targetCoupon.Updator = editorUserID;
                            CouponAdapter.UpdateExchangeCoupon(targetCoupon);
                        }
                        else
                        {
                            targetCoupon.State = (int)ExchangeCouponState.refunding;
                            targetCoupon.UpdateTime = DateTime.Now;
                            targetCoupon.Updator = editorUserID;
                            CouponAdapter.UpdateExchangeCoupon(targetCoupon);
                        }


                        //插入待退款列表 初始状态为1
                        var couponRefundState = 1;
                        if (targetCoupon.ThirdPartyType > 0)
                        {
                            var thirdOrder = thirdpartyService.GetThirdPartyOrderRelByCouponID(targetCoupon.ID);
                            if (thirdOrder != null && thirdOrder.BizID > 0 && !string.IsNullOrEmpty(thirdOrder.ThirdOrderID))
                            {
                                couponRefundState = 10;
                            }
                            else
                            {
                                thirdpartyService.UpdateThirdPartyOrderRelState(targetCoupon.ID, 5);//没有下单成功就更改state为取消
                            }
                        }
                        var refundCouponID = CouponAdapter.AddRefundCoupon(new RefundCouponsEntity()
                        {
                            CouponID = couponID,
                            State = couponRefundState,
                            Creator = editorUserID,
                            Updator = editorUserID,
                            Remark = remark,
                            Price = -1
                        });

                        if (targetCoupon.PayType == 50 ||
                            targetCoupon.PayType == 51 ||
                            targetCoupon.PayType == 6 || targetCoupon.PayType == 30 || refundPrice == 0 || IsFailedGroup(targetCoupon.GroupId))//分销商记账的方式 或退款金额为0 或 失败的团 直接退款  不需进入确认流程
                        {
                            UpdateRefundNotOrderState(refundCouponID, editorUserID);
                        }

                        //如果是升级铂金用户退款，那么需要将相应的金牌购买的VIP也退掉
                        if (targetCoupon.ActivityType == (int)MemberType.会员)
                        {
                            if (targetCoupon.Price == 400) //升级用户
                            {
                                var upgradeFrom199VIP = userCoupons.Where(c => c.ActivityType == (int)MemberType.会员 && c.Price == 199).OrderByDescending(_ => _.ID).First();
                                //插入待退款列表 初始状态为1
                                CouponAdapter.AddRefundCoupon(new RefundCouponsEntity() { CouponID = upgradeFrom199VIP.ID, State = 1, Creator = editorUserID, Updator = editorUserID, Remark = remark, Price = -1 });
                            }
                        }


                        //退还用户VIP首套权限
                        // 如果是手拉手发团用户，那么不退VIP首套权限            
                        if (!CouponAdapter.IsGroupSponserAndCanBuyVIPFirstBuyPackage(targetCoupon.ActivityID, targetCoupon.UserID, targetCoupon.GroupId))
                        {
                            if (targetCoupon.PromotionID > 0 && targetCoupon.State != 40)
                            {
                                ProductAdapter.CheckPromotionAndAddUserBuyFirstPackagePriviledge(targetCoupon.UserID, targetCoupon.PromotionID);
                            }
                            if (targetCoupon.SKUID > 0 && targetCoupon.State != 40)
                            {
                                ProductAdapter.CheckSKUAndAddUserBuyFirstPackagePriviledge(targetCoupon.UserID, targetCoupon.SKUID);
                            }
                        }

                        //退积分
                        if (targetCoupon.Points > 0)
                        {
                            PointsAdapter.RefundUserPoints(targetCoupon.UserID, HotelServiceEnums.ConsumeUserPointsBizType.Coupon, targetCoupon.ID);
                        }

                        //退住基金
                        if (targetCoupon.UserUseHousingFundAmount > 0)
                        {

                            FundAdapter.AddUserFund(new HJD.HotelManagementCenter.Domain.Fund.UserFundIncomeDetail
                            {
                                CreateTime = DateTime.Now,
                                Fund = targetCoupon.UserUseHousingFundAmount,
                                TypeId = (int)HJD.HotelManagementCenter.Domain.FundType.OrderRefund,
                                UserId = targetCoupon.UserID,
                                OriCreateTime = targetCoupon.CreateTime,
                                OriginAmount = refundPrice,
                                OriginOrderId = targetCoupon.ID
                            });

                        }

                        //退现金券
                        if (targetCoupon.CashCouponID > 0)
                        {
                            var item = CouponAdapter.GetUserCouponItemInfoByID(targetCoupon.CashCouponID);
                            if (item.UserCouponType == (int)UserCouponType.DiscountUnconditional)
                            {
                                CouponAdapter.CancelUseUserCouponInfoItem(
                                    new UseCashCouponItem
                                    {
                                        CashCouponID = item.IDX,
                                        CashCouponType = item.UserCouponType,
                                        OrderID = targetCoupon.ID,
                                        OrderType = (int)CashCouponOrderSorceType.sku,
                                        UseCashAmount = targetCoupon.CashCouponAmount
                                    });
                            }
                            else
                            {
                                //满减的只在所有券都退了时才会现金券
                                var NotThisInnerPayGroupCoupons = OnePayCoupons.Where(r => r.InnerBuyGroup != targetCoupon.InnerBuyGroup && r.RefundState == (int)CouponRefundState.NoRefund);
                                if (NotThisInnerPayGroupCoupons.Count() == 0)
                                {
                                    CouponAdapter.CancelUseUserCouponInfoItem(
                                  new UseCashCouponItem
                                  {
                                      CashCouponID = item.IDX,
                                      CashCouponType = item.UserCouponType,
                                      OrderID = targetCoupon.ID,
                                      OrderType = (int)CashCouponOrderSorceType.sku,
                                      UseCashAmount = item.DiscountAmount
                                  });
                                }
                            }

                        }

                        //退代金券
                        if (targetCoupon.VoucherAmount > 0)
                        {

                            //代金券只在所有券都退了时才会退
                            var NotThisInnerPayGroupCoupons = OnePayCoupons.Where(r => r.InnerBuyGroup != targetCoupon.InnerBuyGroup && r.RefundState == (int)CouponRefundState.NoRefund);
                            if (NotThisInnerPayGroupCoupons.Count() == 0)
                            {
                                foreach (var CashCouponID in CommMethods.TranStrIDsToList(targetCoupon.VoucherIDs))
                                {
                                    var item = CouponAdapter.GetUserCouponItemInfoByID(CashCouponID);

                                    CouponAdapter.CancelUseUserCouponInfoItem(
                                  new UseCashCouponItem
                                  {
                                      CashCouponID = item.IDX,
                                      CashCouponType = item.UserCouponType,
                                      OrderID = targetCoupon.ID,
                                      OrderType = (int)CashCouponOrderSorceType.sku,
                                      UseCashAmount = item.DiscountAmount
                                  });
                                }
                            }
                        }

                        //解除占用预约
                        if (targetCoupon.SKUID > 0)
                        {
                            BookUserDateInfoEntity bookUserInfo = GetBookedUserInfoByExchangid(targetCoupon.ID).Where(_ => _.State == 0).FirstOrDefault();
                            if (bookUserInfo != null && bookUserInfo.ID > 0)
                            {
                                CancelBookInfo(bookUserInfo.ID);
                            }

                        }

                        CouponActivityEntity cae = ProductCache.GetOneCouponActivity(targetCoupon.ActivityID);

                        PackageEntity packge = HotelAdapter.GetOnePackageEntity((int)cae.SourceID);

                        cae.HotelName = "";
                        if (cae.SourceID > 0)
                        {
                            var hotelInfo = HotelAdapter.GetHotelBasicInfoList(new List<int> { (int)packge.HotelID }).FirstOrDefault();
                            cae.HotelName = hotelInfo != null ? hotelInfo.HotelName : "";
                        }

                        SendCouponRefundSMS(targetCoupon, refundPrice, cae);

                        //清理前端缓存，以便释放库存
                        UpdateActivityManuSellNum4BG(targetCoupon.ActivityID, targetCoupon.ActivityType, 0);

                        ProductCache.RemoveUserDetailOrderCache(couponID);

                        ProductCache.RemoveCouponUserLockNumCache(targetCoupon.ActivityID, targetCoupon.UserID);


                        //供应商扣款
                        SettlementAdapter.UpdateSettlementForSupplierProductOnCouponRefund(OneInnerPayGroupCoupons.Select(_ => _.ID).ToList());

                        //删除已核销的数据记录
                        DeleteUsedConsumerCouponInfoByExchangeNo(targetCoupon.ExchangeNo);
                    }
                }
            }
            catch (Exception err)
            {
                response = new BaseResponse { Message = "操作成功,等待退款完成", Success = BaseResponse.ResponseSuccessState.Failed };
                Log.WriteLog(string.Format(" Add2RefundCoupon: couponID{0} userId{1} editorUserID{2} phoneNum{3}remark:{4} err:{5}", couponID, userId, editorUserID, phoneNum, remark, err.Message + err.StackTrace));
            }

            return response;
        }

        /// <summary>
        /// 判断是不是失败的团
        /// </summary>
        /// <param name="groupID">团ID</param>
        /// <returns>如果GrupID=0，返回false</returns>
        private bool IsFailedGroup(int groupID)
        {
            if (groupID == 0)
            {
                return false;
            }
            else
            {
                return ProductAdapter.GetGroupPurchaseEntityByGroupID(groupID).State == (int)Enums.GroupState.失败;
            }
        }



        public int UpdateRefundNotOrderState(int refundCouponIDX, long userid, int state = 3)
        {
            long userID = userid;
            string addboolLog = string.Format("确认退款:{0} ", refundCouponIDX);
            HJD.HotelManagementCenter.Domain.BizOpLogEntity bizLog = new HJD.HotelManagementCenter.Domain.BizOpLogEntity
            {
                OperatorUserID = userID,
                OpDateTime = System.DateTime.Now,
                OpType = (int)HJD.HotelManagementCenter.Domain.OpLogBizOpType.ConfirmRefund,
                BizID = refundCouponIDX,
                BizType = HJD.HotelManagementCenter.Domain.OpLogBizType.ExchangeCouponOrder,
                OpContent = "UpdateRefundNotOrderState Log：" + addboolLog
            };

            RefundCouponsEntity refundCoupon = CouponAdapter.GetRefundCouponByRefundCouponIDX(refundCouponIDX);


            int couponID = refundCoupon.CouponID;

            if (state == 2)//手动退，refund新增退款记录
            {
                RefundCouponsEntity rce = new RefundCouponsEntity();
                rce.RefundCouponIDX = refundCoupon.RefundCouponIDX;
                rce.RefundState = 2;
                rce.Creator = userID;
                rce.Updator = userID;
                CouponAdapter.Insert2Refund(rce);
            }

            List<ExchangeCouponEntity> coupon = CouponAdapter.GetExchangeCouponEntityListByIDList(couponID);
            RefundCouponsEntity rc = new RefundCouponsEntity()
            {
                RefundCouponIDX = refundCouponIDX,
                CouponID = couponID,
                State = state,
                RefundState = 6,
                Creator = userID,
                Updator = userID
            };

            if (refundCoupon.RefundType == 1)  //部分退款，不需要处理券状态，直接退钱
            {
                return CouponAdapter.UpdateRefundCouponForPartRefund(rc);
            }
            else
            {
                //更新后台房券状态（支付宝支付:已经退款成功;其他的则为已确认)
                return CouponAdapter.UpdateRefundCoupon(rc);
            }

        }

        /// <summary>
        /// 发券退款短信
        /// </summary>
        /// <param name="targetCoupon"></param>
        /// <param name="refundPrice"></param>
        /// <param name="cae"></param>
        private void SendCouponRefundSMS(ExchangeCouponEntity targetCoupon, decimal refundPrice, CouponActivityEntity cae)
        {
            //支付方式为“分销商记账”或“分销商记帐-无短信”的订单，在退款时，不需要发短信。  
            if (CommMethods.CheckSendSMSByPayType(targetCoupon.PayType))
            {
                //不发短信
                return;
            }
            else
            {
                string sms = "";

                ProductPropertyInfoEntity property = ProductAdapter.GetProductPropertyInfoBySKU(targetCoupon.SKUID.ToString()).First();

                if (targetCoupon.GroupId > 0) //手拉手
                {
                    GroupPurchaseEntity group = ProductAdapter.GetGroupPurchaseEntity(targetCoupon.GroupId).First();
                    //团购状态失败时 不发取消短信。团购状态成功，需要发取消短信
                    if (group.State == 1)
                    {
                        switch (targetCoupon.ActivityType)
                        {
                            case (int)MemberType.会员:
                                sms = "取消：您购买的VIP会员资格已取消，您支付的款项将于1-5个工作日退至您的支付帐号，请注意查收。" + Configs.SMSSuffix;
                                CancelUserRoleByActivityID(targetCoupon.UserID, targetCoupon.ActivityID);
                                break;
                            case (int)CouponActivityType.消费券:
                                if (targetCoupon.State == 40)
                                {
                                    sms = "你的订单已被取消并发起退款申请，申请审核通过后退款将在1-7个工作日到账。退款原因：没有权限或权限已被使用。" + Configs.SMSSuffix;
                                }
                                else
                                {
                                    sms = string.Format("取消：您购买的{0}消费券已经取消，退款￥{1}在审核完成后将退至您的支付账户中，到账周期1-5个工作日，请注意查收。" + Configs.SMSSuffix,
                                        cae.PageTitle, refundPrice);

                                }
                                break;
                            default:
                                if (targetCoupon.State == 40)
                                {
                                    sms = "你的订单已被取消并发起退款申请，申请审核通过后退款将在1-7个工作日到账。退款原因：没有权限或权限已被使用。" + Configs.SMSSuffix;
                                }
                                else
                                {
                                    string smsTemplate = "取消：您抢购的{0}酒店的房券已经取消，退款￥{1}已经退至您的支付账户中，到账周期1-5个工作日，请注意查收。" + Configs.SMSSuffix;
                                    sms = string.Format(smsTemplate, cae.HotelName, refundPrice);
                                }

                                break;
                        }

                    }
                }
                else if (property.PropertyType == 6)
                {
                    sms = string.Format("抱歉，您定购的产品{0}由于到期未成团，此次团购活动已取消，您支付的定金将在1-5个工作日退回至您的支付帐号，如需服务，请致电4000-021-702或关注“周末酒店服务号”回复“服务”。", cae.PageTitle);
                }
                else
                {
                    switch (targetCoupon.ActivityType)
                    {
                        case (int)MemberType.会员:
                            sms = "取消：您购买的VIP会员资格已取消，您支付的款项将于1-5个工作日退至您的支付帐号，请注意查收。" + Configs.SMSSuffix;
                            CancelUserRoleByActivityID(targetCoupon.UserID, targetCoupon.ActivityID);
                            break;
                        case (int)CouponActivityType.消费券:
                            if (targetCoupon.State == 40)
                            {
                                sms = "你的订单已被取消并发起退款申请，申请审核通过后退款将在1-7个工作日到账。退款原因：没有权限或权限已被使用。" + Configs.SMSSuffix;
                            }
                            else
                            {
                                sms = string.Format("取消：您购买的{0}消费券已经取消，退款￥{1}在审核完成后将退至您的支付账户中，到账周期1-5个工作日，请注意查收。" + Configs.SMSSuffix,
                                    cae.PageTitle, refundPrice);

                                //sms = GenPointsRefundSMS(targetCoupon, cae, sms);
                            }
                            break;
                        default:
                            if (targetCoupon.State == 40)
                            {
                                sms = "你的订单已被取消并发起退款申请，申请审核通过后退款将在1-7个工作日到账。退款原因：没有权限或权限已被使用。" + Configs.SMSSuffix;
                            }
                            else
                            {
                                string smsTemplate = "取消：您抢购的{0}酒店的房券已经取消，退款￥{1}已经退至您的支付账户中，到账周期1-5个工作日，请注意查收。" + Configs.SMSSuffix;
                                sms = string.Format(smsTemplate, cae.HotelName, refundPrice);

                                //sms = GenPointsRefundSMS(targetCoupon, cae, sms);
                            }
                            break;
                    }

                }

                if (sms.Length > 0)
                {

                    SMSAdapter.SendSMS(targetCoupon.PhoneNum, sms);
                }
            }
        }

        /// <summary>
        /// 检查是不是VIP购买券，如果是则要看一下新VIP礼券是否已使用，如果已使用则不能退，如果没有使用则直接取消券
        /// </summary>
        /// <param name="targetCoupon"></param>
        /// <returns></returns>
        private bool IsRefundVIPAndUsedCoupon(ExchangeCouponEntity targetCoupon)
        {
            bool result = false;
            if (targetCoupon.ActivityType == 400)
            {
                var list = CouponAdapter.GetNewVIPGiftUserCouponInfoListByUserId(targetCoupon.UserID).Where(_ => _.State != (int)UserCouponState.canceled
                    && _.StartDate > targetCoupon.CreateTime.AddDays(-31)  //是本次购买VIP获得的券。 -30天，是因为可能会是30内续费的
                    ).ToList();
                if (list.Where(_ => _.DiscountAmount != _.RestAmount || _.State != (int)UserCouponState.log).Count() > 0)
                {
                    result = true;
                }
                else if (list.Count() > 0)
                {
                    CouponAdapter.UpdateUserCoupoinItemByIDs(list.Select(_ => _.IDX).ToList(), (int)UserCouponState.canceled);
                }
            }

            return result;
        }

        //private static string GenPointsRefundSMS(ExchangeCouponEntity targetCoupon, CouponActivityEntity cae, string sms)
        //{
        //    if (targetCoupon.Points > 0) //积分兑换
        //    {
        //        return "";  //不发短信
        //        var sku = ProductAdapter.GetSKUEntityByID(targetCoupon.SKUID);
        //        sms = string.Format(CouponAdapter.refundCouponTemplate600Points,
        //              cae.PageTitle, sku.Name, (int)targetCoupon.Points
        //            );
        //    }
        //    else if (targetCoupon.Points == 0 && targetCoupon.Price == 0) //免费
        //    {
        //        return "";  //不发短信
        //        var sku = ProductAdapter.GetSKUEntityByID(targetCoupon.SKUID);
        //        sms = string.Format(CouponAdapter.refundCouponTemplate600Points_Free,
        //             cae.PageTitle, sku.Name
        //         );
        //    }
        //    return sms;
        //}

        private static string GenPointsCouponSMS(ExchangeCouponEntity targetCoupon, int activityID, List<ExchangeCouponModel> rightCouponList, string sms)
        {
            return "";  //不发短信

            if (targetCoupon.Points > 0) //积分兑换
            {
                var sku = ProductAdapter.GetSKUEXEntityByID(targetCoupon.SKUID);
                var cae = ProductCache.GetOneCouponActivity(activityID);

                sms = string.Format(CouponAdapter.buyCouponSuccessTemplate200Points,
                      cae.PageTitle, sku.Name, rightCouponList.Count, string.Join("、", rightCouponList.Select(_ => _.ExchangeNo.Trim())));
            }
            else if (targetCoupon.Points == 0 && targetCoupon.Price == 0) //免费
            {
                var sku = ProductAdapter.GetSKUEXEntityByID(targetCoupon.SKUID);
                var cae = ProductCache.GetOneCouponActivity(activityID);
                sms = string.Format(CouponAdapter.buyCouponSuccessTemplate200Points_Free,
                     cae.PageTitle, sku.Name, rightCouponList.Count, string.Join("、", rightCouponList.Select(_ => _.ExchangeNo.Trim())));
            }
            return sms;
        }


        private void CancelUserRoleByActivityID(long couponUserID, int ActivityID)
        {
            AccountAdapter.DeleteUserRight_UserPrivilegeRel(couponUserID, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage);
            HJDAPI.Controllers.AccountAdapter.UserRoleEnum userRole = TransActivityIDToUserRole(ActivityID);
            AccountAdapter.DeleteUserRight_UserRoleRel(couponUserID, userRole);
        }

        private AccountAdapter.UserRoleEnum TransActivityIDToUserRole(int activityID)
        {
            AccountAdapter.UserRoleEnum userRole = AccountAdapter.UserRoleEnum.VIP199NR;
            switch (activityID)
            {
                case (int)CouponAdapter.ActivityID.VIP199:
                case (int)CouponAdapter.ActivityID.VIP199_EVER:
                    userRole = AccountAdapter.UserRoleEnum.VIP199;
                    break;
                case (int)CouponAdapter.ActivityID.TTGY:
                case (int)CouponAdapter.ActivityID.VIP199_NR:
                    userRole = AccountAdapter.UserRoleEnum.VIP199NR;
                    break;
                case (int)CouponAdapter.ActivityID.VIP599:
                    userRole = AccountAdapter.UserRoleEnum.VIP599;
                    break;

            }
            return userRole;
        }

        /// <summary>
        /// 检查拼团和集赞拼团是否成功，如果拼团失败  进行退款操作
        /// </summary>
        [HttpGet]
        public void GroupFailRefund()
        {

            //团相应活动ID，用于清理活动SellNum缓存
            List<int> RelActivityIDList = new List<int>();
            //查出进行中 并且 过期的拼团
            List<GroupPurchaseEntity> groupFailList = ProductAdapter.GetGroupPurchaseByStateAndEndTime(0, DateTime.Now);
            //更新团状态 为失败 state=2
            foreach (GroupPurchaseEntity item in groupFailList)
            {
                Log.WriteLog("GroupFailRefund groupid：" + item.ID);
                //更新团购为失败状态  state=2
                ProductAdapter.UpdateGroupPurchase(item.ID, 2);
                List<ExchangeCouponEntity> couponList = CouponAdapter.GetExchangeCouponEntityListByGroupId(item.ID).Where(_ => _.State == 2).ToList();
                RelActivityIDList.AddRange(couponList.Select(_ => _.ActivityID).ToList());
                //进行退款
                foreach (var coupon in couponList)
                {
                    try
                    {
                        Log.WriteLog("Add2RefundCoupon groupid" + coupon.ID + "  userid：" + coupon.UserID);
                        string sms = string.Format(GroupFailMsg, coupon.PageTitle, coupon.Price);
                        Add2RefundCoupon(coupon.ID, coupon.UserID, remark: "拼团失败自动退");
                        Log.WriteLog("GroupFailRefund sms：" + coupon.PhoneNum + "  sms：" + sms);
                        SMServiceController.SendSMS(coupon.PhoneNum, sms);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("团购失败 短信发送异常" + ex.Message);
                    }
                    try
                    {
                        var skuInfo = ProductAdapter.GetSKUInfoByID(coupon.SKUID);
                        if (skuInfo.SKU.ExchangeNoType == 1)//第三方提供券码由我们发放
                        {
                            SupplierCouponEntity supplierCouponEntity = new SupplierCouponEntity();
                            supplierCouponEntity.CouponCode = coupon.ExchangeNo;
                            supplierCouponEntity.State = 0;
                            supplierCouponEntity.SupplierId = skuInfo.SPU.SupplierID;
                            CouponAdapter.UpdateSupplierCouponState(supplierCouponEntity, coupon.ID);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(string.Format("CouponCode={0},团购失败修改第三方券码为未使用异常：{1}", coupon.ExchangeNo, ex.Message + ex.StackTrace));
                    }
                }

            }
            RelActivityIDList.Distinct().ToList().ForEach(activityID => ProductCache.RemoveCouponSellNumCache(activityID));
        }

        /// <summary>
        /// 自动拼团成功
        /// </summary>
        [HttpGet]
        public void AutoSuccessGroup()
        {

            List<GroupPurchaseEntity> autoGroupList = ProductAdapter.GetAutoGroupByTime();
            foreach (GroupPurchaseEntity item in autoGroupList)
            {
                DateTime dt = item.EndTime.AddHours(-2);
                if (dt < DateTime.Now && DateTime.Now < item.EndTime)
                {
                    Log.WriteLog("系统自动拼团成功 groupid：" + item.ID);

                    List<ExchangeCouponEntity> couponLlist = CouponAdapter.GetExchangeCouponEntityListByGroupId(item.ID).Where(_ => _.State == 2).ToList();

                    ExchangeCouponEntity coupon = couponLlist.FirstOrDefault();

                    //根据activeid获取成团数量
                    CouponActivityEntity couponActivity = ProductCache.GetOneCouponActivity(coupon.ActivityID);

                    int groupNum = ProductAdapter.GetGroupPurchaseDetailCountByGroupId(item.ID);


                    List<User_Info> ulist = AccountAdapter.GetTestUserList();

                    int lackNum = couponActivity.GroupCount - groupNum;
                    for (int i = 0; i < lackNum; i++)
                    {
                        GroupPurchaseDetailEntity detailmodel = new GroupPurchaseDetailEntity();
                        detailmodel.IsSponsor = false;
                        detailmodel.UserId = ulist[i].UserId;
                        detailmodel.JoinTime = System.DateTime.Now;
                        detailmodel.GroupId = item.ID;
                        detailmodel.OpenId = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
                        ProductAdapter.AddGroupPurchaseDetail(detailmodel);
                    }
                    //更新团
                    int editid = ProductAdapter.UpdateGroupPurchase(item.ID, 1);

                    //if (editid > 0)
                    //{
                    //处理赠送的消费券
                    ProductServiceEnums.ProductType productType = ProductServiceEnums.ProductType.ProductCoupon;
                    foreach (var oneUserCouponList in couponLlist.GroupBy(_ => _.UserID))
                    {
                        try
                        {
                            SMServiceController.SendSMS(coupon.PhoneNum, "恭喜您的拼团成功！请在遛娃指南服务号，我的>>我的订单>>消费券内查看详情及使用方式。");
                            ExchangeCouponEntity oneCoupon = oneUserCouponList.First();
                            Log.WriteLog(string.Format("处理自动拼团：{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}-{9}", oneCoupon.ID, oneCoupon.PhoneNum, oneCoupon.UserID, oneCoupon.CID, oneUserCouponList.Count(), productType, oneCoupon.SKUID, oneCoupon.PayID, oneCoupon.SKUID, oneCoupon.CouponOrderId));
                            GenCouponPromotProducts(oneCoupon.ID, oneCoupon.PhoneNum, oneCoupon.UserID, oneCoupon.CID, oneUserCouponList.Count(), productType, oneCoupon.SKUID, oneCoupon.PayID, oneCoupon.SKUID, oneCoupon.CouponOrderId);

                            // 更新Redis缓存
                            var payRelCouponOrderIDList = CouponAdapter.GetOnePayRelCouponOrderList(oneCoupon.PayID).Select(_ => _.CouponOrderId).ToList();
                            OrderHelper.AddOrderToRedisWithIDList(payRelCouponOrderIDList);
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog("短信发送异常" + ex.Message);
                        }
                    }
                    //}
                }
            }
        }
        /// <summary>
        /// 拼团成功发送短信
        /// </summary>
        /// <param name="groupid">团购id</param>
        /// <param name="hotelName">酒店名称</param>
        /// <param name="packageName">套餐名称</param>
        /// <param name="exchangeMethod">兑换类型</param>
        public void GroupSuccessSendMsg(int groupid, string hotelName, string packageName, int type, int exchangeMethod = 0, string pageTitle = "")
        {
            List<ExchangeCouponEntity> couponList = CouponAdapter.GetExchangeCouponEntityListByGroupId(groupid).Where(_ => _.State == 2).ToList();
            //IEnumerable<IGrouping<int,ExchangeCouponEntity>> groupList= couponList.GroupBy(_ => _.PayID);
            foreach (var couponitem in couponList.GroupBy(_ => _.PayID))
            {
                ExchangeCouponEntity coupon = couponitem.First();
                string exchangeNoList = string.Join("、", (couponitem.Select(_ => _.ExchangeNo).ToList()));
                string sms = "";
                if (type == 600)
                {
                    sms = string.Format(GroupSuccessMsg600, exchangeNoList);//coupon.PageTitle,
                }
                else
                {
                    sms = string.Format(GroupSuccessMsg200, hotelName, packageName, exchangeNoList);
                }
                try
                {
                    //如果自定义短信,发自定义短信内容
                    SKUEntity skuEntity = ProductAdapter.GetSKUEXEntityByID(coupon.SKUID);
                    if ((coupon.CID > 0 && coupon.ExchangeMethod == 6) || skuEntity.SMSType != 2)
                    {
                        SMServiceController.SendSMS(coupon.PhoneNum, sms);
                    }
                    if (skuEntity != null && skuEntity.SMSType == 2)
                    {
                        SMServiceController.SendSMS(coupon.PhoneNum, skuEntity.SMSConten);
                    }
                    else if (exchangeMethod == 5)
                    {

                        SMServiceController.SendSMS(coupon.PhoneNum, string.Format(GroupSuccessQRCodeMsg, pageTitle));
                    }
                    else
                    {
                        SMServiceController.SendSMS(coupon.PhoneNum, sms);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("团购成功 短信发送异常" + ex.Message);
                }

            }
        }
        ///// <summary>
        ///// 新增红包
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public int AddRedShare(RedShareEntity param)
        //{
        //    param.RedUrl = CouponAdapter.GetRedShareURL(param.GUID);
        //    return CouponAdapter.AddRedShare(param);
        //}


        //[HttpPost]
        //public string GetRedShareURL(string k)
        //{
        //    return CouponAdapter.GetRedShareURL(k);
        //}

        public List<RedShareEntity> GetRedShareEntityByGUID(string guid)
        {
            guid = string.IsNullOrWhiteSpace(guid) ? "" : guid;
            return CouponAdapter.GetRedShareEntityByGUID(guid);
        }

        [HttpGet]
        public GetRetailProductListResult GetRetailProductList(int pageSize, int start)
        {
            GetRetailProductListResult m = new GetRetailProductListResult();
            int totalCount = 0;
            m.list = CouponAdapter.GetRetailProductList(pageSize, start, out totalCount);

            m.pageSize = pageSize;
            m.start = start;
            m.totalCount = totalCount;
            return m;
        }
        /// <summary>
        /// 店铺商品展示
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        [HttpGet]
        public GetRetailProductListResult GetRetailerShopProductList(int pageSize, int start, int cid = 0)
        {
            GetRetailProductListResult m = new GetRetailProductListResult();
            int totalCount = 0;
            m.list = CouponAdapter.GetRetailProductList(pageSize, start, out totalCount);

            m.pageSize = pageSize;
            m.start = start;
            m.totalCount = totalCount;
            return m;
        }

        /// <summary>
        /// 分销产品详情页
        /// </summary>
        /// <param name="ID">SKUID</param>
        /// <param name="CID"></param>
        /// <returns></returns>
        [HttpGet]
        public CouponActivityRetailDetailEntity GetRetailProductByID(int ID, long CID)
        {
            return CouponAdapter.GetRetailProductByID(ID, CID);
        }

        [HttpGet]
        public GetChannelMOrderListResult GetChannelMOrderList(int count, int start, int cid)
        {
            GetChannelMOrderListResult m = new GetChannelMOrderListResult();
            int totalCount = 0;
            m.list = CouponAdapter.GetChannelMOrderList(cid, count, start, out totalCount);

            m.pageSize = count;
            m.start = start;
            m.totalCount = totalCount;
            return m;
        }
        [HttpGet]
        public GetChannelMOrderListResult GetChannelMOrderListByCidAndState(int count, int start, int cid, int state, DateTime starttime, DateTime endtime)
        {
            GetChannelMOrderListResult m = new GetChannelMOrderListResult();
            int totalCount = 0;
            m.list = CouponAdapter.GetChannelMOrderListByCidAndState(cid, count, start, state, starttime, endtime, out totalCount);

            m.pageSize = count;
            m.start = start;
            m.totalCount = totalCount;
            return m;
        }
        [HttpGet]
        public GetChannelSettleListResult GetChannelMWithDrawList(int count, int start, int cid)
        {
            GetChannelSettleListResult m = new GetChannelSettleListResult();
            int totalCount = 0;
            m.list = CouponAdapter.GetChannelMWithDrawList(cid, count, start, out totalCount);
            m.pageSize = count;
            m.start = start;
            m.totalCount = totalCount;
            return m;
        }
        [HttpGet]
        public ChannelMOrderDetailEntity GetChannelMOrderDetail(int id, int cid)
        {
            return CouponAdapter.GetChannelMOrderDetail(id, cid);
        }
        [HttpGet]
        public RetailTeamDataEntity GetTeamRetailProductByCid(long cid)
        {
            RetailTeamDataEntity result = new RetailTeamDataEntity();
            List<HJD.HotelManagementCenter.Domain.RetailerInvateEntity> retailList = CouponAdapter.GetRetailerInvateByRefereesUserId(cid);
            result.TeamMemberCount = retailList.Count;
            result.RetailDetailList = CouponAdapter.GetTeamRetailOrderStatistics(cid);
            //result.RetailDetailList = new List<HJD.HotelManagementCenter.Domain.RetailOrderStatistics>();
            //List<SettlementForChannelEntity> teamLearerReward = CouponAdapter.GetTeamLeaderRewardByCid(cid);
            //string orderIds = string.Join(",", teamLearerReward.Select(_ => _.OrderID).ToList());
            //List<SettlementForChannelEntity> teamMember = CouponAdapter.GetTeamMemberRetailOrder(orderIds);
            //var groupList = teamMember.GroupBy(_ => _.CID).ToList();
            //foreach (var item in groupList)
            //{
            //    try
            //    {
            //        long userid = (long)item.Key;
            //        HJD.HotelManagementCenter.Domain.RetailOrderStatistics model = new HJD.HotelManagementCenter.Domain.RetailOrderStatistics();
            //        model.RetailerName = retailList.Where(_ => _.UserID == userid).Count() > 0 ? retailList.Where(_ => _.UserID == userid).First().Name : "";
            //        model.TotalOrderPrice = teamMember.Where(_ => (long)_.CID == userid && (_.RewardType == 2 || _.RewardType == 0)).Sum(t => t.OrderPrice);
            //        model.TotalReward = teamMember.Where(_ => (long)_.CID == cid && _.RewardType == 1).Sum(t => t.Reward);
            //        model.RetailerPhoneNum = retailList.Where(_ => _.UserID == userid).Count() > 0 ? retailList.Where(_ => _.UserID == userid).First().PhoneNum : "";
            //        result.RetailDetailList.Add(model);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.WriteLog("GetTeamRetailProductByCid " + ex);
            //    }
            //}
            result.TotalTeamOrderPrice = result.RetailDetailList.Sum(_ => _.TotalOrderPrice);
            result.TotalTeamReward = result.RetailDetailList.Sum(_ => _.TotalReward);
            return result;

        }

        [HttpGet]
        public List<HJD.HotelManagementCenter.Domain.RetailOrderStatistics> GetRetailRankings(int start, int count, int beforeday)
        {
            DateTime startTime = Convert.ToDateTime(DateTime.Now.AddDays(-beforeday).ToShortDateString());
            List<HJD.HotelManagementCenter.Domain.RetailOrderStatistics> list = CouponAdapter.GetRetailRankings(start, count, startTime);
            foreach (var item in list)
            {
                if (item.RetailerPhoneNum != null && item.RetailerPhoneNum.Length > 7)
                {
                    item.RetailerPhoneNum = item.RetailerPhoneNum.Substring(0, 3) + "****" + item.RetailerPhoneNum.Substring(7);
                }
            }
            return list;
        }

        [HttpGet]
        public ChannelMineModel GetChannelMMine(int cid, int aid)
        {
            return CouponAdapter.GetChannelMMine(cid, aid);
        }
        [HttpGet]
        public GetChannelMOrderListResult GetChannelMInAccountList(int count, int start, int cid)
        {
            GetChannelMOrderListResult m = new GetChannelMOrderListResult();
            int totalCount = 0;
            m.list = CouponAdapter.GetChannelMInAccountList(cid, count, start, out totalCount);

            m.pageSize = count;
            m.start = start;
            m.totalCount = totalCount;
            return m;
        }

        /// <summary>
        /// 入账记录接口
        /// </summary>
        /// <param name="count"></param>
        /// <param name="start"></param>
        /// <param name="cid"></param>
        /// <param name="state">0：待入账 1待提现 2提现中 3 已打款</param>
        /// <returns></returns>
        [HttpGet]
        public AccountRecordInfoEntity GetChannelAccountRecord(int cid, int start, int count, int state)
        {
            AccountRecordInfoEntity m = new AccountRecordInfoEntity();
            int totalCount = 0;
            m.list = CouponAdapter.GetChannelAccountRecord(cid, start, count, state, out totalCount);
            m.totalCount = totalCount;
            return m;
        }

        [HttpGet]
        public GetChannelMBankAccountListResult GetChannelBankAccountList(int cid)
        {
            GetChannelMBankAccountListResult m = new GetChannelMBankAccountListResult();
            m.list = CouponAdapter.GetChannelBankAccountList(cid);
            return m;
        }
        [HttpGet]
        public HJD.HotelManagementCenter.Domain.PayBankAccountLibEntity GetChannelBankAccount(int cid, int type)
        {
            HJD.HotelManagementCenter.Domain.PayBankAccountLibEntity m = CouponAdapter.GetChannelBankAccount(cid, type);
            return m;
        }
        [HttpPost]
        public OperationResult SaveBankAccount(HJD.HotelManagementCenter.Domain.PayBankAccountLibEntity model)
        {
            var result = new OperationResult();
            result = CouponAdapter.SaveBankAccount(model);
            return result;
        }

        [HttpGet]
        public OperationResult GetCanRequire(int cid)
        {
            var result = new OperationResult();
            result = CouponAdapter.GetCanRequire(cid);
            return result;
        }
        [HttpGet]
        public OperationResult WithDrawBankAccount(int id, int cid)
        {
            var result = new OperationResult();
            result = CouponAdapter.WithDrawBankAccount(id, cid);
            return result;
        }

        /// <summary>
        /// 包含酒店套餐的搜索条件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public ShopSearchItemEntity GetSearchRetailItemList(int type)
        {
            return CouponAdapter.GetSearchRetailItemList(type);
        }

        /// <summary>
        /// 根据分销产品类型获取排序条件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public List<ShopSearchEntity> GetSearchRetailSortList(int type, bool isRetailShop = false)
        {
            return CouponAdapter.GetSearchRetailSortList(type, isRetailShop);
        }

        /// <summary>
        /// 获取分销产品筛选条件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<ShopSearchEntity> GetSearchRetailScreenList()
        {
            return CouponAdapter.GetSearchRetailScreenList();
        }


        [HttpGet]
        public ShopSearchItemEntity GetSearchRetailerProductItemList()
        {
            return CouponAdapter.GetSearchRetailerProductItemList();
        }
        [HttpGet]
        public ShopSearchItemEntity GetSearchRetailerProductShopItemList()
        {
            return CouponAdapter.GetSearchRetailerProductShopItemList();
        }

        [HttpGet]
        public SearchEntity GetSearchItem(int catorgoryParentId, int payType)
        {
            return CouponAdapter.GetSearchItem(catorgoryParentId, payType);
        }

        [HttpGet]
        public SearchEntity GetDistinctSPUDistrictIDByAlbum(int albumId)
        {
            return CouponAdapter.GetDistinctSPUDistrictIDByAlbum(albumId);
        }

        /// <summary>
        /// 根据专辑id获取省市联级
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<GroupDistrictEntity> GetGroupDistinctSPUDistrictIDByAlbum(int albumId)
        {
            return CouponAdapter.GetGroupDistinctSPUDistrictIDByAlbum(albumId);
        }

        /// <summary>
        /// 根据专辑id获取关联的标签列表
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<CommTagEntity> GetTagListByAlbum(int albumId)
        {
            return CacheAdapter.LocalCache30Min.GetData<List<CommTagEntity>>("GetTagListByAlbum:" + albumId.ToString(), () =>
         {
             var TagCategoryAlbumRelList = CommAdapter.GetObjRelList(HJD.HotelManagementCenter.Domain.ObjRelTypeEnum.TagCategory_Album_Rel, albumId, HJD.HotelManagementCenter.Domain.ObjIDTypeEnum.ObjTwoID);

             var TagCategoryRelList = CommAdapter.GetObjRelList(HJD.HotelManagementCenter.Domain.ObjRelTypeEnum.Tag_TagCategory_Rel, TagCategoryAlbumRelList.First().Obj1ID, HJD.HotelManagementCenter.Domain.ObjIDTypeEnum.ObjTwoID);

             var TagItemList = CommAdapter.GetTagItemList(TagCategoryRelList.Select(_ => (Int32)_.Obj1ID).ToList());

             return Mapper.Map<List<CommTagEntity>>(TagItemList);
             //            return TagItemList.Select(_=>Mapper.Map<CommTagEntity>(_)).ToList();
         });
        }

        /// <summary>
        /// 分销产品 获取省市联级
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<GroupDistrictEntity> GetRetailGroupDistinct()
        {
            return CouponAdapter.GetRetailGroupDistinct();
        }

        [HttpGet]
        public SearchEntity GetDistinctSPUDistrictIDByProductTagID(int productTagID)
        {
            return CouponAdapter.GetDistinctSPUDistrictIDByProductTagID(productTagID);
        }
        /// <summary>
        ///  分销商商品
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        [HttpPost]
        public GetRetailProductListResult GetSearchRetailerProductList(SearchProductRequestEntity param)
        {
            GetRetailProductListResult m = new GetRetailProductListResult();
            int totalCount = 0;
            param.Screen.ProductType = (param.Screen.ProductType == null || param.Screen.ProductType.Count == 0) ? new List<int>() { 200, 500, 600 } : param.Screen.ProductType;
            param.SearchWord = string.IsNullOrWhiteSpace(param.SearchWord) ? "" : param.SearchWord;
            m.list = CouponAdapter.GetSearchProductList(param, out totalCount);

            m.totalCount = totalCount;
            return m;
        }

        /// <summary>
        ///  分销商商品
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        [HttpPost]
        public GetRetailProductListResult GetSearchRetailerProductListByCategory(SearchProductRequestEntity param)
        {
            GetRetailProductListResult m = new GetRetailProductListResult();
            int totalCount = 0;
            param.Screen.ProductType = (param.Screen.ProductType == null || param.Screen.ProductType.Count == 0) ? new List<int>() { 14, 15, 20 } : param.Screen.ProductType;
            param.SearchWord = string.IsNullOrWhiteSpace(param.SearchWord) ? "" : param.SearchWord;
            m.list = CouponAdapter.GetSearchProductListByCategory(param, out totalCount);
            m.ParamType = param.Screen.ProductType;
            m.totalCount = totalCount;
            return m;
        }


        /// <summary>
        ///  搜索分销产品
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        [HttpGet]
        public GetRetailProductListResult GetSearchRetailerProductList(string keyWord = "")
        {
            GetRetailProductListResult m = new GetRetailProductListResult();
            SearchParamsForCoupon param = new SearchParamsForCoupon();
            param.count = 10000;
            param.needHighlight = false;
            param.OnlyDistributable = true;
            param.keyword = keyWord;
            List<QuickSearchSuggestItem> listData = SearchAdapter.SearchCoupon(param);

            List<int> skuList = listData.Select(_ => _.Id).ToList();

            List<CouponActivityRetailEntity> pullList = new List<CouponActivityRetailEntity>();

            List<CouponActivityRetailEntity> list = CouponAdapter.GetSearchRetailerProductList(skuList);
            foreach (QuickSearchSuggestItem item in listData)
            {
                foreach (CouponActivityRetailEntity it in list)
                {
                    if (item.Id == it.ID)
                    {
                        pullList.Add(it);
                        break;
                    }
                }
            }

            m.list = pullList;// CouponAdapter.GetSearchRetailerProductList(skuList);
            m.totalCount = m.list.Count();
            return m;
        }

        /// <summary>
        /// 店铺商品展示
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        [HttpPost]
        public GetRetailProductListResult GetSearchRetailerShopProductList(SearchProductRequestEntity param)
        {
            GetRetailProductListResult m = new GetRetailProductListResult();
            int totalCount = 0;

            param.Screen.ProductType = (param.Screen.ProductType == null || param.Screen.ProductType.Count == 0) ? new List<int>() { 200, 600 } : param.Screen.ProductType;
            param.SearchWord = string.IsNullOrWhiteSpace(param.SearchWord) ? "" : param.SearchWord;
            m.list = CouponAdapter.GetSearchProductList(param, out totalCount);

            m.totalCount = totalCount;
            return m;
        }

        [HttpPost]
        public BaseResult SubmitBookInfo(BookPersonInfoEntity model)
        {
            BaseResult result = new BaseResult();
            result.RetCode = "1";
            result.Message = "预约成功";
            try
            {
                if (model.BookDateId == 0)
                {
                    result.RetCode = "-1";
                    result.Message = "预约失败";
                    Log.WriteLog("预约报错 缺少关键信息 model.BookDateId  ：" + model.BookDateId);
                }
                else
                {
                    foreach (int exchangeId in model.ExchangCouponIds)
                    {
                        int bookCount = ProductAdapter.GetBookUserDateInfoByExchangID(exchangeId).Count;
                        if (bookCount > 0)
                        {
                            result.RetCode = "2";
                            result.Message = "您已预约过，无需再次预约";
                        }
                        else
                        {
                            SKUEntity skuModel = ProductAdapter.GetSKUEXEntityByID(model.skuid);
                            bool isCanBook = ProductAdapter.IsCanBookItem(model.BookDateId, model.BookDetailId, (skuModel.StockCount * model.ExchangCouponIds.Count));
                            if (isCanBook)
                            {
                                BookUserDateInfoEntity m = new BookUserDateInfoEntity();

                                m.BookDateId = model.BookDateId;
                                m.BookDetailId = model.BookDetailId;
                                m.ExchangCouponId = exchangeId;//model.ExchangCouponId;

                                ExchangeCouponEntity exEntity = new ExchangeCouponEntity();
                                //如果前置预约需要查询订单状态
                                if (skuModel.BookPosition == 1)
                                {
                                    exEntity = CouponAdapter.GetExchangCouponByCouponId(exchangeId);
                                }

                                m.State = (skuModel.BookPosition == 1 && exEntity.State != 2) ? 2 : 0;//购买前预约的状态改为2，防止提交未支付的订单占用预量
                                m.TravelIDs = "";// string.Join(",", model.TravelId == null ? new List<int>() : model.TravelId);
                                ProductAdapter.AddBookUserDateInfo(m);

                                //如果 证件id不为空 则更新exchangecoupon表的出行人信息
                                if (!string.IsNullOrWhiteSpace(string.Join(",", model.TravelId == null ? new List<int>() : model.TravelId)))
                                {
                                    ExchangeCouponEntity entity = new ExchangeCouponEntity();

                                    entity.Updator = model.UserID;
                                    entity.TraveIDs = string.Join(",", model.TravelId == null ? new List<int>() : model.TravelId);
                                    entity.ID = exchangeId;// model.ExchangCouponId;
                                    CouponAdapter.UpdateExchangeCouponTravelIDs(entity);

                                    //先把之前的订单出行人更新为不可用,然后再插入新的联系人信息
                                    List<CouponOrderPersonEntity> orderPersonList = CouponAdapter.GetCouponOrderPersonByOrderId(exchangeId);
                                    if (orderPersonList != null && orderPersonList.Count > 0)
                                    {
                                        foreach (CouponOrderPersonEntity cop in orderPersonList)
                                        {
                                            cop.State = 1;
                                            CouponAdapter.UpdateCouponOrderPerson(cop);
                                        }
                                    }

                                    //插入新的联系人信息
                                    List<TravelPersonEntity> travelList = HotelAdapter.GetTravelPersonByIDS(string.Join(",", model.TravelId));
                                    foreach (var travel in travelList)
                                    {
                                        CouponOrderPersonEntity cop = new CouponOrderPersonEntity();
                                        cop.TravelPersonName = travel.TravelPersonName;
                                        cop.TravelPersonCardType = travel.IDType;
                                        cop.TravelPersonCardNo = travel.IDNumber;
                                        cop.ExchangeID = exchangeId;
                                        CouponAdapter.AddCouponOrderPerson(cop);
                                    }
                                }

                                if (model.TemplateData != null)
                                {
                                    model.TemplateData.BizId = exchangeId;
                                    Addtemplate(model.TemplateData);
                                }
                                List<int> exchangeIds = new List<int>();
                                exchangeIds.Add(exchangeId);//model.ExchangCouponId
                                List<ExchangeCouponEntity> ExList = CouponAdapter.GetExchangeCouponEntityListByIDList(exchangeIds).Where(_ => _.State == 2).ToList();
                                foreach (ExchangeCouponEntity item in ExList)
                                {
                                    if (item.OperationState == 4)
                                    {
                                        //更新ExchangeCoupone 状态
                                        ExchangeCouponEntity exchangeCoupon = new ExchangeCouponEntity();
                                        exchangeCoupon.ExchangeNo = item.ExchangeNo;
                                        exchangeCoupon.ID = item.ID;
                                        exchangeCoupon.State = item.State;
                                        exchangeCoupon.OperationState = skuModel.FollowOperation == 4 ? 1 : 0;
                                        CouponAdapter.UpdateExchangeState(exchangeCoupon);
                                    }
                                    else
                                    if (item.BookPosition == 2)
                                    {
                                        //更新ExchangeCoupone 状态
                                        ExchangeCouponEntity exchangeCoupon = new ExchangeCouponEntity();
                                        exchangeCoupon.ExchangeNo = item.ExchangeNo;
                                        exchangeCoupon.State = item.State;
                                        exchangeCoupon.ID = item.ID;
                                        exchangeCoupon.OperationState = skuModel.FollowOperation == 4 ? 1 : 0;
                                        CouponAdapter.UpdateExchangeState(exchangeCoupon);
                                    }
                                }
                            }
                            else
                            {
                                result.RetCode = "0";
                                result.Message = "该场次预约名额不足，请重新选择";
                            }
                        }
                        //预约后更新用户订单列表缓存
                        ProductCache.RemoveUserDetailOrderCache(exchangeId);

                    }
                }
            }
            catch (Exception e)
            {
                result.RetCode = "0";
                result.Message = "预约失败";
                Log.WriteLog("预约报错 error--- ：" + e);
            }

            return result;
        }

        public void Addtemplate(TemplateDataEntity model)
        {
            HJD.HotelManagementCenter.Domain.TemplateDataEntity tempDate = new HJD.HotelManagementCenter.Domain.TemplateDataEntity();
            tempDate.BizId = model.BizId;
            tempDate.BizType = (int)HJD.HotelManagementCenter.Domain.TempLateDataType.ExchangeCouponID;
            tempDate.CreateTime = DateTime.Now;
            tempDate.Description = model.Description;
            tempDate.TemplateID = model.TemplateID;
            tempDate.TemplateItem = model.TemplateItem;
            HotelAdapter.InserOrUpdateTemplateData(tempDate);
        }
        [HttpGet]
        public BaseResult CancelBookInfo(int id)
        {
            BookUserDateInfoEntity model = ProductAdapter.GetBookedUserInfoById(id);
            BaseResult result = new BaseResult();
            if (model != null)
            {
                model.State = 1;
                ProductAdapter.UpdateBookUserDateInfo(model);

                List<int> exchangeIds = new List<int>();
                exchangeIds.Add(model.ExchangCouponId);
                List<ExchangeCouponEntity> ExList = CouponAdapter.GetExchangeCouponEntityListByIDList(exchangeIds).Where(_ => _.State == 2).ToList();
                foreach (ExchangeCouponEntity item in ExList)
                {
                    try
                    {
                        SKUEntity skuModel = ProductAdapter.GetSKUEXEntityByID(item.SKUID);
                        if (skuModel.FollowOperation == 4)
                        {
                            //更新ExchangeCoupone 状态
                            ExchangeCouponEntity exchangeCoupon = new ExchangeCouponEntity();
                            exchangeCoupon.ID = item.ID;
                            exchangeCoupon.ExchangeNo = item.ExchangeNo;
                            exchangeCoupon.State = item.State;
                            exchangeCoupon.OperationState = 4;//4：预约后进入待处理状态
                            CouponAdapter.UpdateExchangeState(exchangeCoupon);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog("CancelBookInfo ERROR ：" + e);
                    }
                }

                //预约取消时也需要清理订单缓存，以便在订单列表中看到是更新的数据
                ProductCache.RemoveUserDetailOrderCache(model.ExchangCouponId);

                result.RetCode = "1";
                result.Message = "取消预约成功";
            }
            else
            {
                result.RetCode = "0";
                result.Message = "取消预约失败";
            }
            return result;
        }

        [HttpGet]
        public List<PDayItem> GetBookDateList(int skuid)
        {
            List<BookDayItem> bDayList = new List<BookDayItem>();
            List<PDayItem> pDayItemList = new List<PDayItem>();
            var SKUModel = ProductAdapter.GetSKUItemByID(skuid);
            if (SKUModel.BookNameId > 0 && SKUModel.StockCount > 0)
            {
                //bDayList = ProductAdapter.GetBookDateList(SKUModel.BookNameId, SKUModel.StockCount);
                bDayList = ProductAdapter.GetBookDateListBySql(SKUModel.BookNameId, SKUModel.StockCount);
                DateTime beforeBook = Convert.ToDateTime(System.DateTime.Now.AddDays(SKUModel.BeforeBookDay).ToString("yyyy-MM-dd " + SKUModel.BeforeBookDayHour + ":" + SKUModel.BeforeBookDayMinute + ":00"));
                foreach (var item in bDayList)
                {
                    PDayItem pday = new PDayItem();
                    pday.Day = item.BookDay;
                    if (SKUModel.BeforeBookDay == 0 && SKUModel.BeforeBookDayHour == 0 && SKUModel.BeforeBookDayMinute == 0)
                    {
                        pday.SellState = item.State ? 1 : 0;
                    }
                    else if (beforeBook.ToString("yyyy-MM-dd") == item.BookDay.ToString("yyyy-MM-dd"))
                    {
                        item.BookDay = Convert.ToDateTime(item.BookDay.ToString("yyyy-MM-dd " + System.DateTime.Now.Hour + ": " + System.DateTime.Now.Minute + ":00"));
                        pday.SellState = (item.State && item.BookDay < beforeBook) ? 1 : 0;
                    }
                    else
                    {
                        pday.SellState = (item.State && item.BookDay > beforeBook) ? 1 : 0;
                    }
                    pday.ID = item.ID;
                    pDayItemList.Add(pday);
                }
            }
            return pDayItemList;
        }

        [HttpGet]
        public List<BookDetailItem> GetBookDetailByBookDateId(int skuid, int bookDateId)
        {
            List<BookDetailItem> bookDetaiList = new List<BookDetailItem>();
            var SKUModel = ProductAdapter.GetSKUItemByID(skuid);
            if (SKUModel.BookNameId > 0 && SKUModel.StockCount > 0)
            {
                bookDetaiList = ProductAdapter.GetBookDetailItemList(SKUModel.BookNameId, bookDateId, SKUModel.StockCount);
            }
            return bookDetaiList == null ? new List<BookDetailItem>() : bookDetaiList;
        }

        /// <summary>
        /// 获取模板信息
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="postionIndex">1 提交时填写信息 2：预约是填写信息</param>
        /// <returns></returns>
        [HttpGet]
        public HJD.HotelManagementCenter.Domain.TemplateDataEntity GetSKUTempSource(int skuid, int postionIndex = 2)
        {
            var SKUModel = ProductAdapter.GetSKUItemByID(skuid);
            HJD.HotelManagementCenter.Domain.TemplateDataEntity tempData = new HJD.HotelManagementCenter.Domain.TemplateDataEntity();
            if (SKUModel.WriteOtherPostion == postionIndex && SKUModel.WriteOtherPostion > 0)
            {
                HJD.HotelManagementCenter.Domain.TemplateSourceEntity tempSource = HotelAdapter.GetTempSourceById(SKUModel.TempSourceID);
                tempData = new HJD.HotelManagementCenter.Domain.TemplateDataEntity();
                if (tempSource != null && tempSource.ID > 0)
                {
                    tempData.TemplateID = tempSource.ID;
                    tempData.TemplateItem = tempSource.TemplateContent;
                    tempData.BizType = 3;
                }
            }
            return tempData;
        }

        [HttpGet]
        public CommentTextAndUrl VIPDiscountDescription(decimal orderTotalPrice, decimal orderVipTotalPrice)
        {
            CommentTextAndUrl result = new CommentTextAndUrl();

            if (_ContextBasicInfo.IsThanVer6_0)
            {
                return result;
            }
            else
            {
                int leveAmount = Convert.ToInt32(200 - orderVipTotalPrice) > 0 ? (Convert.ToInt32(200 - orderVipTotalPrice) + 300) : 300;
                var appType = AppType.ToLower();
                if (orderVipTotalPrice > 200)
                {
                    int DiscountAmount = Convert.ToInt32(orderTotalPrice - orderVipTotalPrice + 200);

                    string desc = "<div style=\"font-size:1em;color:#474747;text-align:center;\">现在成为VIP会员</span><br/>只需<span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">¥199</span>，此订单立减<span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">¥" + DiscountAmount + "</span><br/>并立获<br/><span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">¥" + leveAmount + "</span>现金券可抵扣未来消费<br/>享受平台<b>会员价优惠</b><br/>享受价值<b>万元免费福利</b>";
                    //android  暂时无法实现居中   20171027  Zb------
                    if (appType.Contains("android"))
                    {
                        desc = "<div align=center>现在成为VIP会员，只需<font color=#fe8000>¥199</font>，此订单立减<font color=#fe8000>¥" + DiscountAmount + "</font>，并立获<font color=#fe8000>¥" + leveAmount + "</font>现金券可抵扣未来消费，享受平台<b>会员价优惠</b>，享受价值<b>万元免费福利</b></div>";
                    }
                    result.Description = desc;
                }
                else
                {
                    int getCouponAmount = Convert.ToInt32(500 - orderVipTotalPrice);
                    string desc = "<div style=\"font-size:1em;color:#474747;text-align:center;\">现在成为VIP会员</span><br/>只需<span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">¥199</span>,此订单<span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">免费</span><br/>并立获<br/><span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">¥" + leveAmount + "</span>现金券可抵扣未来消费<br/>享受平台<b>会员价优惠</b><br/>享受价值<b>万元免费福利</b>";
                    if (appType.Contains("android"))
                    {
                        desc = "<div align=center>现在成为VIP会员，只需<font color=#fe8000>¥199</font>，此订单<font color=#fe8000>免费</font>，并立获<font color=#fe8000>¥" + leveAmount + "</font>现金券可抵扣未来消费，享受平台<b>会员价优惠</b>，享受价值<b>万元免费福利</b></div>";
                    }
                    result.Description = desc;
                }
                result.ActionUrl = "http://www.zmjiudian.com/Coupon/VipAreaInfo?userid={userid}&_newpage=1&_isoneoff=1";
                result.Text = "成为VIP";

                return result;
            }
        }

        [HttpGet]
        public BecomVIPTip BecomeVIPDiscountDescription(decimal orderTotalPrice, decimal orderVipTotalPrice)
        {
            BecomVIPTip result = new BecomVIPTip();
            result.TipTitle = "此为VIP专享价哦~你还不是VIP会员？";
            int leveAmount = Convert.ToInt32(200 - orderVipTotalPrice) > 0 ? (Convert.ToInt32(200 - orderVipTotalPrice) + 300) : 300;
            var appType = AppType.ToLower();
            if (orderVipTotalPrice > 200)
            {
                //int DiscountAmount = Convert.ToInt32(orderTotalPrice - orderVipTotalPrice + 200);

                string desc = "<div style=\"font-size:1em;color:#474747;text-align:center;\">现在成为VIP会员</span><br/>只需<span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">¥199</span>，首单会员价<span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">再减¥200</span><br/><br/>并立获<br/><span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">¥" + leveAmount + "</span>现金券可抵扣未来消费<br/>享受平台<b>会员价优惠</b><br/>享受价值<b>万元免费福利</b>";

                if (appType.Contains("android"))
                {
                    desc = "<div align=center>现在成为VIP会员，只需<font color=#fe8000>¥199</font>，首单会员价<font color=#fe8000>再减¥200</font>，并立获<font color=#fe8000>¥" + leveAmount + "</font>现金券可抵扣未来消费，享受平台<b>会员价优惠</b>，享受价值<b>万元免费福利</b></div>";
                }
                result.Description = desc;
            }
            else
            {
                int getCouponAmount = Convert.ToInt32(500 - orderVipTotalPrice);
                string desc = "<div style=\"font-size:1em;color:#474747;text-align:center;\">现在成为VIP会员</span><br/>只需<span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">¥199</span>,此订单<span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">免费</span><br/><br/>并立获<br/><span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">¥" + leveAmount + "</span>现金券可抵扣未来消费<br/>享受平台<b>会员价优惠</b><br/>享受价值<b>万元免费福利</b>";
                if (appType.Contains("android"))
                {
                    desc = "<div align=center>现在成为VIP会员，只需<font color=#fe8000>¥199</font>，此订单<font color=#fe8000>免费</font>，并立获<font color=#fe8000>¥" + leveAmount + "</font>现金券可抵扣未来消费，享受平台<b>会员价优惠</b>，享受价值<b>万元免费福利</b></div>";
                }
                result.Description = desc;
            }
            result.ActionUrl = "http://www.zmjiudian.com/Coupon/VipAreaInfo?userid={userid}&_newpage=1&_isoneoff=1";
            result.Text = "成为VIP";

            return result;
        }

        [HttpGet]
        public CommentTextAndUrl BecomeVIPTips(decimal orderTotalPrice, decimal orderVipTotalPrice)
        {
            CommentTextAndUrl result = new CommentTextAndUrl();
            int DiscountAmount = 0;
            if (orderVipTotalPrice >= 200)
            {
                DiscountAmount = Convert.ToInt32(orderTotalPrice - orderVipTotalPrice + 200);
            }
            else
            {
                DiscountAmount = Convert.ToInt32(orderTotalPrice);
            }
            string desc = "付¥199<span style=\"color:#3E9EC0;text-decoration:underline; \">成为VIP</span>立减<span style=\"color:#fe8000;font-size:1em;\" color=\"color:#fe8000;\">¥" + DiscountAmount + "</span>，并享万元福利优惠";
            result.Description = desc;
            result.ActionUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1";
            result.Text = "成为VIP";

            return result;
        }

        [HttpGet]
        public List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserId(int userId, UserCouponState state)
        {
            List<UserCouponItemInfoEntity> cashCouponList = CouponAdapter.GetUserCouponInfoListByUserId(userId, state);
            //foreach (var item in cashCouponList)
            //{
            //    item.IsShowExpireTips = (item.ExpiredDate - System.DateTime.Now).Days > 30 ? false : true;
            //    item.CashCouponName = CouponAdapter.GetCashCouponTypeName((UserCouponType)item.UserCouponType, item.RequireAmount, item.DiscountAmount);
            //}
            return cashCouponList;
        }


        [HttpGet]
        public List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserIdAndType(int userId, UserCouponState state, UserCouponType type)
        {
            List<UserCouponItemInfoEntity> cashCouponList = CouponAdapter.GetUserCouponInfoListByUserIdAndType(userId, state, type);
            //foreach (var item in cashCouponList)
            //{
            //    item.IsShowExpireTips = (item.ExpiredDate - System.DateTime.Now).Days > 30 ? false : true;
            //    item.CashCouponName = CouponAdapter.GetCashCouponTypeName((UserCouponType)item.UserCouponType, item.RequireAmount, item.DiscountAmount);
            //}
            return cashCouponList;
        }

        [HttpGet]
        public int GetUserCouponInfoCountByUserId(int userId, UserCouponState state)
        {
            return CouponAdapter.GetUserCouponInfoCountByUserId(userId, state);
        }

        [HttpGet]
        public int GetUserCouponInfoCountByUserIdAndType(int userId, UserCouponState state, UserCouponType type)
        {
            return CouponAdapter.GetUserCouponInfoCountByUserIdAndType(userId, state, type);
        }

        [HttpPost]
        public RequestResultEntity UseUserCouponInfoItem(UseCashCouponItem param)
        {
            RequestResultEntity requestResult = CouponAdapter.UseUserCouponInfoItem(param);
            #region 注释
            //BaseResult result = new BaseResult();
            //UserCouponItemInfoEntity userCouponItemInfo = CouponAdapter.GetUserCouponItemInfoByID(param.CashCouponID, (UserCouponType)param.CashCouponType);
            //if (userCouponItemInfo.State == 0)
            //{
            //    if (userCouponItemInfo.RestAmount >= param.UseCashAmount)
            //    {
            //        UserCouponItemEntity model = CouponAdapter.GetUserCouponItemByID(param.CashCouponID);
            //        //立减
            //        if (param.CashCouponType == (int)UserCouponType.DiscountUnconditional)
            //        {
            //            model.RestAmount = model.RestAmount - param.UseCashAmount;
            //            if (model.RestAmount == 0)
            //            {
            //                model.State = (int)UserCouponState.used;
            //            }
            //            CouponAdapter.UpdateUserCouponItem(model);
            //        }
            //        else if (param.CashCouponType == (int)UserCouponType.DiscountOverPrice)
            //        {
            //            model.State = (int)UserCouponState.used;
            //            CouponAdapter.UpdateUserCouponItem(model);
            //        }
            //        // 写入使用明细表

            //        UserCouponConsumeLogEntity logModel = new UserCouponConsumeLogEntity();
            //        logModel.ConsumeAmount = param.UseCashAmount;
            //        logModel.CreateTime = System.DateTime.Now;
            //        logModel.OrderID = param.OrderID;
            //        logModel.OrderType = param.OrderType;
            //        logModel.UserCouponItemID = param.CashCouponID;
            //        logModel.ConsumeType = 0;
            //        CouponAdapter.AddUserCouponConsumeLog(logModel);
            //        result.Message = "已抵扣";
            //        result.RetCode = "0";
            //        return result;
            //    }
            //    else
            //    {
            //        result.Message = "抵扣金额大于券剩余金额";
            //        result.RetCode = "1";
            //        return result;
            //    }
            //}
            //else if (userCouponItemInfo.State == 1)
            //{
            //    result.Message = "该券已使用";
            //    result.RetCode = "1";
            //    return result;
            //}
            //else if (userCouponItemInfo.State == 2)
            //{
            //    result.Message = "该券已过期";
            //    result.RetCode = "2";
            //    return result;
            //}
            //else if (userCouponItemInfo.State == 3)
            //{ 
            //    result.Message="该券已取消";
            //    result.RetCode = "3";
            //    return result;
            //} 
            #endregion

            return requestResult;
        }


        [HttpPost]
        public RequestResultEntity CancelUseUserCouponInfoItem(UseCashCouponItem param)
        {
            return CouponAdapter.CancelUseUserCouponInfoItem(param);
            #region 注释
            //BaseResult result = new BaseResult();
            //UserCouponItemEntity model = CouponAdapter.GetUserCouponItemByID(param.CashCouponID);
            //if (model.IDX > 0)
            //{
            //    model.State = (int)UserCouponState.log;
            //    if (param.CashCouponType == (int)UserCouponType.DiscountUnconditional)
            //    {
            //        model.RestAmount = param.UseCashAmount;
            //    }
            //    CouponAdapter.UpdateUserCouponItem(model);

            //    // 写入使用明细表

            //    UserCouponConsumeLogEntity logModel = new UserCouponConsumeLogEntity();
            //    logModel.ConsumeAmount = param.UseCashAmount;
            //    logModel.CreateTime = System.DateTime.Now;
            //    logModel.OrderID = param.OrderID;
            //    logModel.OrderType = param.OrderType;
            //    logModel.UserCouponItemID = param.CashCouponID;
            //    logModel.ConsumeType = 1;
            //    CouponAdapter.AddUserCouponConsumeLog(logModel);
            //    result.Message = "取消成功";
            //    result.RetCode = "0";
            //    return result;
            //}
            //else
            //{
            //    result.Message = "未找到券";
            //    result.RetCode = "1";
            //    return result;
            //} 
            #endregion
        }


        public List<UserCouponConsumeLogEntity> GetUserCouponLogByCouponItemID(int idx)
        {
            return CouponAdapter.GetUserCouponLogByCouponItemID(idx);
        }

        #region   使用代金券
        [HttpPost]
        public List<UserCouponItemInfoEntity> GetCanUseVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req)
        {
            return CouponAdapter.GetCanUseVoucherInfoListForOrder(req);
        }
        //获取指定产品类型指定金额下，所有不可用的券（订单确认页使用，暂时可以不分页）
        [HttpPost]
        public List<UserCouponItemInfoEntity> GetCannotUseVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req)
        {
            return CouponAdapter.GetCannotUseVoucherInfoListForOrder(req);
        }
        [HttpPost]
        // 检测指定产品类型指定金额下，当前券ID是否满足条件（比如用户选好了一个券后，手动变更数量时，需要实时检测）
        public CheckSelectedVoucherInfoForOrderViewModel CheckSelectedVoucherInfoForOrder(OrderVoucherRequestParams req)
        {
            return CouponAdapter.CheckSelectedVoucherInfoForOrder(req);
        }

        #endregion

        #region 使用现金券

        [HttpPost]
        //        获取指定产品类型指定金额下，所有可用的券（订单确认页使用，暂时可以不分页）
        public List<UserCouponItemInfoEntity> GetCanUseCouponInfoListForOrder(OrderUserCouponRequestParams req)
        {
            return CouponAdapter.GetCanUseCouponInfoListForOrder(req);
        }

        [HttpPost]
        //获取指定产品类型指定金额下，所有不可用的券（订单确认页使用，暂时可以不分页）

        public List<UserCouponItemInfoEntity> GetCannotUseCouponInfoListForOrder(OrderUserCouponRequestParams req)
        {
            return CouponAdapter.GetCannotUseCouponInfoListForOrder(req);
        }

        [HttpPost]
        //获取指定产品类型指定金额下，默认最优惠的券（订单确认页需要默认选择一个券）        
        public TheBestCouponInfoForOrderViewModel GetTheBestCouponInfoForOrder(OrderUserCouponRequestParams req)
        {
            return CouponAdapter.GetTheBestCouponInfoForOrder(req);
        }


        [HttpPost]
        // 检测指定产品类型指定金额下，当前券ID是否满足条件（比如用户选好了一个券后，手动变更数量时，需要实时检测）
        public CheckSelectedCashCouponInfoForOrderViewModel CheckSelectedCashCouponInfoForOrder(OrderUserCouponRequestParams req)
        {
            return CouponAdapter.CheckSelectedCashCouponInfoForOrder(req);
        }
        #endregion

        /// <summary>
        /// 领取&获得红包
        /// </summary>
        /// <param name="key"></param>
        /// <param name="phoneNum"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        [HttpGet]
        public RedRecordEntity GetRedRecordByGuidAndPhone(string key, string phoneNum, string openid)
        {
            //不是周末酒店的 用户则注册
            User_Info info = AccountAdapter.GetOrRegistPhoneUser(phoneNum, 0);

            try
            {
                //微信环境下，如果当前已经登录，则绑定
                if (!string.IsNullOrEmpty(openid) && info != null && info.UserId > 0)
                {
                    //weixinUserInfo.Openid = "oHGzlw-sdix9G__-S4IzfTsYRqC8";

                    var _userChannelRel = new UserChannelRelEntity
                    {
                        Channel = "weixinservice_haoyi",
                        Code = openid,
                        UserId = info.UserId,
                        Tag = "",
                        CreateTime = DateTime.Now
                    };

                    //先查询
                    var _rel = AccountAdapter.GetOneUserChannelRel(_userChannelRel);
                    if (_rel != null && _rel.UserId > 0)
                    {

                    }
                    else
                    {

                        var _add = AccountAdapter.AddUserChannelRel(_userChannelRel);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            RedActivityEntity redActivity = CouponAdapter.GetRedActivityByGUID(key);
            int totalCount = 0;
            List<RedRecordEntity> redRecordList = CouponAdapter.GetRedRecordByActivityId(redActivity.ID, 100, 0, out totalCount);

            RedRecordEntity redRecord = new RedRecordEntity();

            //修复  分享到微信 后 不返回app导致 没有发放红包
            List<RedRecordEntity> noRealGetRed = redRecordList.Where(_ => _.State == 0).ToList();
            foreach (RedRecordEntity item in noRealGetRed)
            {
                ReturnRedResult(item.ID);
            }
            redRecord = CouponAdapter.GetRedRecordByRedActivityIdAndPhone(redActivity.ID, phoneNum);

            if (redActivity.Count > totalCount && redActivity.ID > 0)
            {

                if (redRecord.ID > 0)
                {
                    redRecord.IsShowResult = redActivity.IsShowResult;
                    return redRecord;
                }
                else
                {
                    List<RedPoolDetailEntity> redPoolDetailList = CouponAdapter.GetRedDetailByPoolId(redActivity.RedPoolId);

                    //随机生成一个红包 
                    Random rd = new Random();
                    int i = rd.Next(redPoolDetailList.Count);
                    RedPoolDetailEntity rpdModel = redPoolDetailList[i];
                    try
                    {
                        int redCoredId = AddRedRecord(rpdModel.ID, openid, phoneNum, redActivity.ID, info.UserId, rpdModel.BizType, 0, 0);
                        int result = ReturnRedResult(redCoredId);
                        if (result == 0)
                        {
                            Log.WriteLog("GetRedRecordByGuidAndPhone  userid  ：" + rpdModel.ID + "  Phone ： " + phoneNum + "   rpdModel： " + rpdModel.ID);
                        }
                        else
                        {
                            redRecord = CouponAdapter.GetRedRecordByRedActivityIdAndPhone(redActivity.ID, phoneNum);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog("GetRedRecordByGuidAndPhone  userid  ：" + info.UserId + "  Phone ： " + phoneNum + "   rpdModel： " + rpdModel.ID + " redActivity.ID：" + redActivity.ID + "  rpdModel.BizType  ： " + rpdModel.BizType + " ERROR" + e);
                    }
                }
                //房券  取PageTitle
                if (redRecord.BizType == 3)
                {
                    CouponActivityEntity activity = CouponAdapter.GetCouponActivityBySKUID(redRecord.CouponId);
                    redRecord.CouponTypeName = activity.PageTitle;
                }
            }
            redRecord.IsShowResult = redActivity.IsShowResult;
            return redRecord;
        }

        [HttpGet]
        public CouponActivityEntity GetCouponActivityBySKUID(int skuid)
        {
            CouponActivityEntity activity = CouponAdapter.GetCouponActivityBySKUID(skuid);
            return activity;
        }

        [HttpGet]
        public RedActivityInfoEntity GetRedRecordByKey(string key, int count = 20, int start = 0)
        {
            RedActivityInfoEntity redInfo = new RedActivityInfoEntity();
            List<RedRecordEntity> redRecordList = new List<RedRecordEntity>();
            RedActivityEntity redActivity = CouponAdapter.GetRedActivityByGUID(key);
            int totalCount = 0;
            if (redActivity.ID > 0 && redActivity.IsShowResult == true)
            {
                redRecordList = CouponAdapter.GetRedRecordByActivityId(redActivity.ID, count, start, out totalCount);
                redInfo.TotalCount = totalCount;
                redRecordList.ForEach(m => m.AvatarUrl = (string.IsNullOrWhiteSpace(m.AvatarUrl) ? DescriptionHelper.defaultAvatar : m.AvatarUrl));
            }
            redInfo.RedRecordList = redRecordList;

            if (redRecordList.Count >= redActivity.Count)
            {
                redInfo.RedState = 1; //已领完
            }
            if (System.DateTime.Now > redActivity.ExpireTime)
            {
                redInfo.RedState = 2;//已过期
            }

            return redInfo;
        }


        [HttpGet]
        public RedOrderInfoEntity GetOrderRed(long userid, long orderId, HJDAPI.Common.Helpers.Enums.OrdersType OrderType, decimal totalPrice)
        {
            RedOrderInfoEntity redOrder = new RedOrderInfoEntity();
            redOrder.ButtonText = "分享领红包";
            redOrder.Description = "分享成功后在“我的”-“钱包”里查看已获得的红包";
            redOrder.ResultTitle = "";
            redOrder.SmallPicture = "http://whfront.b0.upaiyun.com/app/img/coupon/redcoupon/red-coupon-sml.png";
            redOrder.BigPicture = "http://whfront.b0.upaiyun.com/app/img/coupon/redcoupon/red-coupon-big.png";
            redOrder.RedShare.photoUrl = "http://whfront.b0.upaiyun.com/app/img/coupon/redcoupon/redcoupon-share-icon.png";
            redOrder.RedShare.Content = "度假休闲，又好又划算！";
            redOrder.RedShare.title = "周末酒店APP送你一个大礼包";
            redOrder.RedState = 0;
            redOrder.RedShare.shareLink = "";
            redOrder.RedShare.returnUrl = "";
            redOrder.RedShare.returnApiUrl = "";

            ///app 必须要这个字段   by---zbing
            redOrder.RedShare.notHotelNameTitle = "";

            string phoneNum = "";
            long userID = 0;

            RedActivityType redActivityType = (RedActivityType)((int)OrderType);
            List<RedActivityEntity> redList = CouponAdapter.GetRedActivityByBizIDAndBizType(orderId, redActivityType);
            if (redList.Count > 0)
            {

                if (OrderType == HJDAPI.Common.Helpers.Enums.OrdersType.ExchangeOrder)
                {
                    List<ExchangeCouponEntity> exchangeList = CouponAdapter.GetExchangeCouponEntityByPayID(Convert.ToInt32(orderId));
                    int count = exchangeList.Where(_ => _.State == 2 || _.State == 3).Count();
                    if (count > 0)
                    {
                        ExchangeCouponEntity exchangeCoupon = exchangeList.First();
                        if (exchangeCoupon.PayType != 6 && exchangeCoupon.PayType != 7 && (exchangeCoupon.PayType != 25 || exchangeCoupon.Price != 0) && exchangeCoupon.PayType != 26 && exchangeCoupon.GroupId == 0)
                        {
                            phoneNum = exchangeCoupon.PhoneNum;
                            userID = exchangeCoupon.UserID;
                        }
                        else
                        {
                            redOrder.RedState = 0;
                            return redOrder;
                        }
                    }
                }
                else if (OrderType == HJDAPI.Common.Helpers.Enums.OrdersType.HotelOrder)
                {
                    HJD.HotelPrice.Contract.DataContract.Order.PackageOrderInfo orderInfo = OrderAdapter.GetPackageOrderInfo(orderId, userid);
                    if (((orderInfo.State == 10 && orderInfo.OrderPayType != 8) || orderInfo.State == 12 || orderInfo.State == 31 || orderInfo.State == 32) && (orderInfo.OrderPayType != 6 && orderInfo.OrderPayType != 7 && orderInfo.OrderPayType != 25 && orderInfo.OrderPayType != 26))
                    {
                        User_Info userInfo = AccountAdapter.GetUserInfoByUserId(orderInfo.UserID);
                        phoneNum = userInfo.MobileAccount;
                        userID = orderInfo.UserID;
                    }
                    else
                    {
                        redOrder.RedState = 0;
                        return redOrder;
                    }
                }

                RedActivityEntity ra = redList.OrderByDescending(_ => _.ID).First();
                int totalCount = 0;
                List<RedRecordEntity> redRecordList = CouponAdapter.GetRedRecordByActivityId(ra.ID, 10, 0, out totalCount);
                List<RedRecordEntity> redRecord = redRecordList.Where(_ => _.State == 1).ToList();
                if (redRecord.Count > 0)
                {
                    redOrder.RedState = 0;
                }
                else if (redRecordList.Count > 0)
                {
                    redOrder.RedState = 1;
                    RedRecordEntity rr = redRecordList.OrderByDescending(_ => _.ID).First();

                    GenShareModel(redOrder, rr.ID);

                }
                else
                {
                    redOrder.RedState = 1;
                    RedPoolDetailEntity redPoolDetail = GenRedPoolDetail(ra.RedPoolId, totalPrice);

                    int redRecordId = AddRedRecord(redPoolDetail.ID, "", phoneNum, ra.ID, userid, redPoolDetail.BizType, 0, 0);

                    GenShareModel(redOrder, redRecordId);
                }
            }
            else
            {

                if (OrderType == HJDAPI.Common.Helpers.Enums.OrdersType.ExchangeOrder)
                {
                    List<ExchangeCouponEntity> exchangeList = CouponAdapter.GetExchangeCouponEntityByPayID(Convert.ToInt32(orderId));
                    int count = exchangeList.Where(_ => _.State == 2 || _.State == 3).Count();
                    if (count > 0)
                    {
                        ExchangeCouponEntity exchangeCoupon = exchangeList.First();
                        if (exchangeCoupon.PayType != 6 && exchangeCoupon.PayType != 7 && (exchangeCoupon.PayType != 25 || exchangeCoupon.Price != 0) && exchangeCoupon.PayType != 26 && exchangeCoupon.GroupId == 0)
                        {
                            phoneNum = exchangeCoupon.PhoneNum;
                            userID = exchangeCoupon.UserID;
                            redOrder.RedState = 1;
                            GenRedOrderUrl(redOrder, totalPrice, orderId, (int)OrderType, userID, phoneNum, "");
                        }
                    }
                }
                else if (OrderType == HJDAPI.Common.Helpers.Enums.OrdersType.HotelOrder)
                {
                    HJD.HotelPrice.Contract.DataContract.Order.PackageOrderInfo orderInfo = OrderAdapter.GetPackageOrderInfo(orderId, userid);
                    if (((orderInfo.State == 10 && orderInfo.OrderPayType != 8) || orderInfo.State == 12 || orderInfo.State == 31 || orderInfo.State == 32) && (orderInfo.OrderPayType != 6 && orderInfo.OrderPayType != 7 && orderInfo.OrderPayType != 25 && orderInfo.OrderPayType != 26))
                    {
                        User_Info userInfo = AccountAdapter.GetUserInfoByUserId(orderInfo.UserID);
                        redOrder.RedState = 1;
                        GenRedOrderUrl(redOrder, totalPrice, orderId, (int)OrderType, orderInfo.UserID, userInfo.MobileAccount, "");
                    }
                }
            }

            return redOrder;
        }

        public void GenRedOrderUrl(RedOrderInfoEntity redOrder, decimal totalPrice, long orderId, int orderType, long userid, string phoneNum, string openID)
        {
            List<RedPoolEntity> redPoolList = CouponAdapter.GetRedPoolByOrderPrice(totalPrice);
            if (redPoolList.Count > 0)
            {
                RedPoolEntity redPool = redPoolList.OrderByDescending(_ => _.ID).First();
                RedActivityEntity param = new RedActivityEntity();
                param.Name = "订单红包  订单ID " + orderId;
                param.GUID = System.Guid.NewGuid().ToString();
                param.CreateTime = System.DateTime.Now;
                param.Creator = userid;
                param.BizType = orderType;
                param.BizID = orderId;
                param.RedPoolId = redPool.ID;
                param.Count = 10;
                param.ExpireTime = Convert.ToDateTime(System.DateTime.Now.AddDays(10).ToString("yyyy-MM-dd 23:59:59"));
                param.IsShowResult = true;
                int id = CouponAdapter.AddRedActivity(param);

                RedPoolDetailEntity redPoolDetail = GenRedPoolDetail(redPool.ID, totalPrice);

                int redRecordId = AddRedRecord(redPoolDetail.ID, openID, phoneNum, id, userid, redPoolDetail.BizType, 0, 0);
                //redOrder.ResultTitle = "恭喜你获得¥" + Convert.ToInt32(redPoolDetail.DiscountAmount) + "红包";
                //redOrder.Description = "分享成功后在“我的”-“ 钱包”-“现金券”里查看已获得的红包";

                //redOrder.RedShare.shareLink = Configs.WWWURL + "/Coupon/RedCashCoupon?key=" + param.GUID;
                ////redOrder.RedShare.returnUrl = Configs.APIURL + "/api/coupon/ReturnRedResult?redRecordId=" + redRecordId;
                //redOrder.RedShare.returnUrl = Configs.APIURL + "/Coupon/RedCashCoupon?key=" + param.GUID + "& redRecordId=" + redRecordId;
                //redOrder.RedShare.returnApiUrl = Configs.APIURL + "/api/coupon/ReturnRedResult?redRecordId=" + redRecordId;
                GenShareModel(redOrder, redRecordId);
            }
        }

        public void GenShareModel(RedOrderInfoEntity redOrder, int redRecordId)
        {
            RedRecordEntity redRecord = CouponAdapter.GetRedRecordById(redRecordId);

            redOrder.ResultTitle = "恭喜你获得¥" + Convert.ToInt32(redRecord.DiscountAmount) + "红包";
            if (redRecord.BizType == (int)RedRecordType.DiscountOver || redRecord.BizType == (int)RedRecordType.DiscountUnconditional)
            {
                redOrder.Description = "分享成功后在“我的”-“ 钱包”-“现金券”里查看已获得的红包";
            }
            else if (redRecord.BizType == (int)RedRecordType.DiscountVoucher)
            {
                redOrder.Description = "分享成功后在“我的”-“ 钱包”-“代金券”里查看已获得的红包";
            }
            else if (redRecord.BizType == (int)RedRecordType.ExchangeCoupon)
            {
                redOrder.Description = "分享成功后在“我的”-“钱包”里查看已获得的红包";
            }

            redOrder.RedShare.shareLink = Configs.WWWURL + "/Coupon/RedCashCoupon?key=" + redRecord.GUID;
            redOrder.RedShare.returnUrl = Configs.WWWURL + "/Coupon/RedCashCoupon?key=" + redRecord.GUID + "&redRecordId=" + redRecordId;
            redOrder.RedShare.returnApiUrl = Configs.APIURL + "/api/coupon/ReturnRedResult?redRecordId=" + redRecordId;
        }

        public RedPoolDetailEntity GenRedPoolDetail(int redPoolId, decimal totalPrice)
        {
            List<RedPoolDetailEntity> redPoolDetailList = CouponAdapter.GetRedDetailByPoolId(redPoolId);

            RedPoolDetailEntity redPoolDetail = new RedPoolDetailEntity();
            if (redPoolDetailList.Count > 0)
            {
                if (totalPrice <= 100)
                {
                    redPoolDetail = redPoolDetailList.Where(_ => _.DiscountAmount == 5).ToList().FirstOrDefault();
                }
                else if (totalPrice <= 500)
                {
                    redPoolDetail = redPoolDetailList.Where(_ => _.DiscountAmount == 10).ToList().FirstOrDefault();
                }
                else if (totalPrice <= 1000)
                {
                    redPoolDetail = redPoolDetailList.Where(_ => _.DiscountAmount == 20).ToList().FirstOrDefault();
                }
                else if (totalPrice <= 10000)
                {
                    redPoolDetail = redPoolDetailList.Where(_ => _.DiscountAmount == 30).ToList().FirstOrDefault();
                }
                else if (totalPrice > 10000)
                {
                    redPoolDetail = redPoolDetailList.Where(_ => _.DiscountAmount == 50).ToList().FirstOrDefault();
                }
            }
            return redPoolDetail == null ? new RedPoolDetailEntity() : redPoolDetail;
        }

        public int AddRedRecord(int redPoolDetailID, string openId, string phoneNum, int redActivityId, long userId, int orderType, int state, int bizID)
        {
            RedRecordEntity model = new RedRecordEntity();
            model.OpenId = openId;
            model.PhoneNum = phoneNum;
            model.RedActivityID = redActivityId;
            model.RedPoolDetailID = redPoolDetailID;
            model.BizID = bizID;
            model.BizType = orderType;
            model.State = state;
            model.UserId = userId;
            model.CreateTime = System.DateTime.Now;
            return CouponAdapter.AddRedRecord(model);
        }

        [HttpGet]
        public int ReturnRedResult(int redRecordId)
        {
            RedRecordEntity redRecordModel = CouponAdapter.GetRedRecordById(redRecordId);
            if (redRecordModel.State == 0)
            {
                RedPoolDetailEntity rpdModel = CouponAdapter.GetRedPoolDetailByID(redRecordModel.RedPoolDetailID);

                int couponItemId = 0;
                //判断红包类型  插入领取表
                if (rpdModel.BizType == 0 || rpdModel.BizType == 1)
                {
                    //插入积分表
                    UserCouponItemEntity UserCouponItem = new UserCouponItemEntity();
                    UserCouponItem.CouponDefineID = rpdModel.BizID;
                    couponItemId = CouponAdapter.SendOneUserCouponItem(redRecordModel.UserId, rpdModel.BizID, 100000);


                }
                else if (rpdModel.BizType == 3)
                {
                    CouponActivityEntity activity = CouponAdapter.GetCouponActivityBySKUID(rpdModel.BizID);
                    ExchangeCouponEntity ec = new ExchangeCouponEntity();
                    ec.UserID = redRecordModel.UserId;
                    ec.Type = 0;
                    ec.SKUID = rpdModel.BizID;
                    ec.PhoneNum = redRecordModel.PhoneNum;
                    ec.ActivityID = activity.ID;
                    ec.ActivityType = activity.Type;
                    ec.PayID = 1;
                    couponItemId = CouponAdapter.InsertExchangeCoupon(ec);
                }

                redRecordModel.State = 1;
                redRecordModel.BizID = couponItemId;
                return CouponAdapter.updateRedRecord(redRecordModel);
            }
            else
            {
                return 0;
            }
        }

        [HttpGet]
        public int ReceiveCouponDefine(long userId, string couponDefineIds)
        {
            int result = 1;
            try
            {
                if (userId > 0)
                {
                    foreach (string couponDefineId in couponDefineIds.Split(',').ToList())
                    {
                        CouponAdapter.SendOneUserCouponItem(userId, int.Parse(couponDefineId), 100000);
                    }
                }
            }
            catch (Exception e)
            {
                result = 0;
            }
            return result;
        }

        [HttpGet]
        public RedRecordEntity GetUserRedRecord(int id)
        {
            RedRecordEntity redRecord = CouponAdapter.GetRedRecordById(id);
            return redRecord;
        }

        [HttpPost]
        public int UpdateOperationState(ExchangeCouponEntity ece)
        {
            return CouponAdapter.UpdateOperationState(ece);
        }

        [HttpGet]
        public int UpdateOperationState(int couponid, long updator, int operationstate)
        {
            ExchangeCouponEntity ec = new ExchangeCouponEntity();
            ec.ID = couponid;
            ec.Updator = updator;
            ec.OperationState = operationstate;
            return CouponAdapter.UpdateOperationState(ec);
        }
        [HttpGet]
        public int UpdateThirdPartyRefundCoupon(int couponid, int thirdstate, int refundstate)
        {
            return CouponAdapter.UpdateThirdPartyRefundCoupon(couponid, thirdstate, refundstate);
        }

        [HttpPost]
        public int UpdateOperationRemark(ExchangeCouponRequestParam param)
        {
            return CouponAdapter.UpdateOperationRemark(param.ID, param.OperationRemark);
        }

        [HttpGet]
        public int CouponChangeUsed(int couponid)
        {
            return CouponAdapter.CouponChangeUsed(couponid);
        }
        /// <summary>
        /// 更新过期不退款券数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public bool UpdateExchangeCouponExpiredNotRefund()
        {
            return CouponAdapter.UpdateExchangeCouponExpiredNotRefund();
        }

        #region App6.1版本的钱包相关接口提供

        [HttpGet]
        public List<MenuItemEntity> GetWalletMenuList(long userId)
        {
            var menuList = new List<MenuItemEntity>();

            var WalletMenuUrl = Configs.WalletMenuUrl;

            //积分
            var pointItemEntity = new MenuItemEntity();
            pointItemEntity.ShowName = "积分";
            pointItemEntity.ShowCount = 0;
            pointItemEntity.Icon = "http://whfront.b0.upaiyun.com/app/img/me/icon-point.png";
            pointItemEntity.Type = 0;
            pointItemEntity.ActionUrl = GetUserWalletUrl(userId, "point", newtitle: true, newpage: true);

            //住基金
            var fundItemEntity = new MenuItemEntity();
            fundItemEntity.ShowName = "住基金";
            fundItemEntity.ShowCount = 0;
            fundItemEntity.Icon = "http://whfront.b0.upaiyun.com/app/img/me/icon-zhujijin.png";
            fundItemEntity.Type = 0;
            fundItemEntity.ActionUrl = GetUserWalletUrl(userId, "fund", newtitle: true, newpage: true);

            //现金券
            var cashCouponItemEntity = new MenuItemEntity();
            cashCouponItemEntity.ShowName = "现金券";
            cashCouponItemEntity.ShowCount = 0;
            cashCouponItemEntity.Icon = "http://whfront.b0.upaiyun.com/app/img/me/icon-coupon-manjian.png";
            cashCouponItemEntity.Type = 0;
            cashCouponItemEntity.ActionUrl = string.Format("{1}coupon/WalletCashCoupon?userid={0}&select=0&orderid=0&issection=0&_newpage=1", userId, WalletMenuUrl);

            //代金券
            var voucherItemEntity = new MenuItemEntity();
            voucherItemEntity.ShowName = "代金券";
            voucherItemEntity.ShowCount = 0;
            voucherItemEntity.Icon = "http://whfront.b0.upaiyun.com/app/img/me/icon-coupon-daijin.png";
            voucherItemEntity.Type = 0;
            voucherItemEntity.ActionUrl = string.Format("{1}coupon/WalletVoucher?userid={0}&select=0&orderid=0&issection=0&_newpage=1", userId, WalletMenuUrl);

            //航空里程
            var airItemEntity = new MenuItemEntity();
            airItemEntity.ShowName = "航空里程";
            airItemEntity.ShowCount = 0;
            airItemEntity.Icon = "http://whfront.b0.upaiyun.com/app/img/me/icon-air.png";
            airItemEntity.Type = 0;
            airItemEntity.ActionUrl = GetUserWalletUrl(userId, "airmiles", newtitle: false, newpage: true);//string.Format("{1}personal/wallet/{0}/airmiles?&_newpage=1", userId, WalletMenuUrl);

            //add to list
            menuList.Add(pointItemEntity);
            menuList.Add(fundItemEntity);
            menuList.Add(cashCouponItemEntity);
            menuList.Add(voucherItemEntity);
            menuList.Add(airItemEntity);

            return menuList;
        }

        /// <summary>
        /// 获取钱包下的各个功能的URL
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tag"></param>
        /// <param name="mode"></param>
        /// <param name="newtitle">url是否要追加 _newtitle=1 参数</param>
        /// <param name="newpage">url是否要追加 _newpage=1 参数</param>
        /// <param name="dorpdown">url是否要追加 _dorpdown=1 参数</param>
        /// <returns></returns>
        public string GetUserWalletUrl(long userId, string tag, string mode = "", bool newtitle = false, bool newpage = false, bool dorpdown = false)
        {
            Int64 url_TimeStamp = Signature.GenTimeStamp();
            int url_SourceID = 100;
            //string webSiteUrl = "http://www.zmjiudian.com/";


            var walletMenuUrl = Configs.WalletMenuUrl;
            //webSiteUrl = "http://192.168.1.22:8081/";
            //webSiteUrl = "http://localhost:8780/"; 
            //webSiteUrl = "http://www.tst.zmjd001.com/";
            string url_RequestType = String.Format("{4}personal/wallet/{0}/{3}{5}?TimeStamp={1}&SourceID={2}", userId, url_TimeStamp, url_SourceID, tag, walletMenuUrl, (string.IsNullOrEmpty(mode) ? "" : "/" + mode));
            string MD5Key = Configs.MD5Key;
            string Sign = Signature.GenSignature(url_TimeStamp, url_SourceID, MD5Key, url_RequestType);
            string UserInfoUrl = String.Format("{0}&Sign={1}{2}{3}{4}", url_RequestType, Sign, (newtitle ? "&_newtitle=1" : ""), (newpage ? "&_newpage=1" : ""), (dorpdown ? "&_dorpdown=1" : ""));
            return UserInfoUrl;
        }

        /// <summary>
        /// 支付金额为0 的订单直接支付
        /// </summary>
        /// <param name="payid">支付id</param>
        /// <returns></returns>
        [HttpGet]
        public bool FreePriceCouponForWXAPP(int payid)
        {
            HJD.CouponService.Contracts.Entity.CommOrderEntity comm = CouponAdapter.GetOneCommOrderEntity(payid);
            if (comm.Price == 0)
            {
                string zmjdPayUrl = Configs.ZMJDPayUrl;
                Int64 url_TimeStamp = Signature.GenTimeStamp();
                int url_SourceID = -1111;
                var freeUrl = string.Format("{0}Pay/FreePayFinish?orderid={1}&CID=-1111&Price={2}&TimeStamp={3}", zmjdPayUrl, payid, 0, url_TimeStamp);
                string MD5Key = Configs.MD5Key;
                string Sign = Signature.GenSignature(url_TimeStamp, url_SourceID, MD5Key, freeUrl);
                string freePayUrl = String.Format("{0}&Sign={1}", freeUrl, Sign);
                Log.WriteLog("FreePriceCouponForWXAPP" + freePayUrl);
                HttpHelper.Get(freePayUrl, "utf-8");
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        [HttpGet]
        public AlbumSkuListEntity GetSKUAlbumListByAlbumId(int albumid, int start, int count)
        {
            AlbumSkuListEntity result = CouponAdapter.GetSKUAlbumEntityListByAlbumId(albumid, start, count);
            return result;
        }

        [HttpGet]
        public int GetUsedCoupon(long userid, int conponDefineID)
        {
            return CouponAdapter.GetUserCouponItemByUserId(userid, conponDefineID);
        }

        /// <summary>
        /// 检查大团购是否成功
        /// </summary>
        [HttpGet]
        public void RefundAndUpdateStepGroupState()
        {
            //获取进行中的大团
            List<StepGroupEntity> StepGroupingList = ProductAdapter.GetStepGroupByState(0);
            foreach (StepGroupEntity item in StepGroupingList)
            {
                //大团购失败团时间加10分钟，防止刚好过期时支付
                if (item.DepositEndTime.AddMinutes(10) <= DateTime.Now)
                {
                    List<GradientDiscountEntity> result = new List<GradientDiscountEntity>();
                    foreach (string dlist in item.GradientPrice.Replace("；", ";").Split(';').ToList())
                    {
                        List<string> gkeyvalue = dlist.Replace("：", ":").Split(':').ToList();
                        if (gkeyvalue.Count == 2)
                        {
                            GradientDiscountEntity gradien = new GradientDiscountEntity();
                            gradien.GroupCount = int.Parse(gkeyvalue[0]);
                            gradien.Price = decimal.Parse(gkeyvalue[1]);
                            result.Add(gradien);
                        }
                    }

                    GradientDiscountEntity gradient = result.OrderBy(_ => _.GroupCount).FirstOrDefault();

                    SPUInfoEntity spuinfo = ProductAdapter.GetSPUInfoByID(item.SPUID);
                    //获取定金对应的sku信息
                    SKUEntity sku = spuinfo.SPU.SKUList.Where(_ => _.IsDepositSKU == true).First();
                    CouponActivityEntity ac = couponService.GetCouponActivityBySKUID(sku.ID);
                    if (gradient != null && gradient.GroupCount > ac.SellNum)
                    {
                        item.StepGroupState = 2;//状态更新为失败
                        ProductAdapter.UpdateStepGroup(item);

                        List<ExchangeCouponIDEntity> exchangeList = CouponAdapter.GetExchangeCouponIDListBySKUID(sku.ID);
                        foreach (var ex in exchangeList.Where(_ => _.State == 2))
                        {
                            Add2RefundCoupon(ex.ID, ex.UserID, ex.UserID, "", "大团购过期成团失败自动退");
                        }

                        ProductCache.RemoveStepGroupCahceWithSKUID(sku.ID);
                    }

                }
            }

        }

        /// <summary>
        /// 获取券订单详情
        /// </summary>
        /// <param name="id">券订单id</param>
        /// <returns></returns>
        [HttpGet]
        public ProductCouponOrderResult GetExchangeCouponOrderDetail(int id)
        {
            ProductCouponOrderResult result = new ProductCouponOrderResult();
            //默认订单数量为1
            List<ExchangeCouponEntity> exchangeList = new List<ExchangeCouponEntity>();

            //获取券订单信息
            ExchangeCouponEntity exchangeCoupon = CouponAdapter.GetExchangeCouponEntityListByIDList(id).First();

            // 未支付的订单需要根据payid获取提交的订单数和订单金额
            if (exchangeCoupon.State == 1)
            {
                //根据payId获取未支付的订单
                List<ExchangeCouponEntity> noPayOrderList = CouponAdapter.GetExchangeCouponEntityByPayID(exchangeCoupon.PayID);

                exchangeCoupon.OrderCount = noPayOrderList.Count;

                // 得到总价  前端合并支付 
                exchangeCoupon.VoucherAmount = noPayOrderList.Sum(_ => _.VoucherAmount);
                exchangeCoupon.UserUseHousingFundAmount = noPayOrderList.Sum(_ => _.UserUseHousingFundAmount);
                exchangeCoupon.CashCouponAmount = noPayOrderList.Sum(_ => _.CashCouponAmount);
                exchangeCoupon.Price = noPayOrderList.Sum(_ => _.Price);
                exchangeCoupon.OriPrice = noPayOrderList.Sum(_ => _.OriPrice);
            }
            //20181017  可以拿掉 (exchangeCoupon.State == 2 || exchangeCoupon.State == 6) && exchangeCoupon.RefundState > 0 ? 7 :  //新增“退款中” 的状态
            //int realState = (exchangeCoupon.State == 2 || exchangeCoupon.State == 6) && exchangeCoupon.RefundState > 0 ? 7 : exchangeCoupon.State;
            int realState = exchangeCoupon.State;
            if (exchangeCoupon.ExpireTime <= DateTime.Now.Date.AddDays(-1) && (realState == 2 || realState == 6))
            {
                realState = 8;//已过期（特殊状态） 出现在不能退的券客户未消费或者系统自动退款服务没有及时退款(显示的一种中间状态)
            }
            SKUInfoEntity sku = new SKUInfoEntity();
            CouponOrderStepGroupEntity couponOderStepGroup = null;

            if (exchangeCoupon.SKUID > 0)
            {
                sku = ProductAdapter.GetSKUInfoByID(exchangeCoupon.SKUID);

                result.OrderIcon = OrderHelper.GetIcon(sku.Category.ParentID);

                result.IsPackage = sku.SKU.IsPackage;
                string skuids = string.Join(",", sku.SKUList.Select(_ => _.ID));
                List<ProductPropertyInfoEntity> ppiList = ProductAdapter.GetProductPropertyInfoBySKU(skuids);
                //获取sku属性
                ProductPropertyInfoEntity ppi = ppiList.Where(_ => _.SKUID == exchangeCoupon.SKUID).First();

                //最终支付金额
                exchangeCoupon.FinishPayPrice = exchangeCoupon.Price - exchangeCoupon.CashCouponAmount - exchangeCoupon.VoucherAmount - exchangeCoupon.UserUseHousingFundAmount;
                if (ppi != null && ppi.PropertyType == 6)
                {
                    result.ExchangeCouopnOrderType = 6;
                    StepGroupEntity stg = ProductAdapter.GetStepGroupBySPUID(sku.SPU.ID);
                    //判断 定金产品
                    if (ppi.PropertyOptionTargetID == 1)
                    {
                        couponOderStepGroup = new CouponOrderStepGroupEntity();
                        ProductPropertyInfoEntity ppi2 = ppiList.Where(_ => _.PropertyType == 6 && _.PropertyOptionTargetID == 2).First();

                        SKUEntity tailsku = sku.SKUList.Where(_ => _.ID == ppi2.SKUID).First();
                        couponOderStepGroup.TailMoneyEndTime = stg.TailMoneyEndTime;
                        couponOderStepGroup.TailSKUID = tailsku.ID;
                        couponOderStepGroup.DepositSKUID = exchangeCoupon.SKUID;
                        couponOderStepGroup.TailMoneyStartTime = stg.TailMoneyStartTime;
                        couponOderStepGroup.StepGroupState = stg.StepGroupState;
                        couponOderStepGroup.Price = tailsku.Price - sku.SKU.MarketPrice;//支付价格-膨胀金价格（MarketPrice是大团购的膨胀金）
                        couponOderStepGroup.TotalPrice = tailsku.Price;// 当前sku的售卖价
                        couponOderStepGroup.IsPayFinish = false;
                        couponOderStepGroup.BookPosition = tailsku.BookPosition;

                        //定金标识
                        exchangeCoupon.IsDepositOrder = true;

                    }
                    else if (ppi.PropertyOptionTargetID == 2 && exchangeCoupon.CouponOrderId > 0)
                    {
                        //获取大团购 定金sku
                        ProductPropertyInfoEntity depositSKU = ppiList.Where(_ => _.PropertyType == 6 && _.PropertyOptionTargetID == 1).FirstOrDefault();

                        var li = CouponAdapter.GetExchangCouponByCouponOrderID(exchangeCoupon.CouponOrderId);

                        //获取定金对应的订单 找不到则是直接购买订单
                        ExchangeCouponEntity depositOrder = CouponAdapter.GetExchangCouponByCouponOrderID(exchangeCoupon.CouponOrderId).Where(_ => _.State == 3 && _.SKUID == depositSKU.SKUID).FirstOrDefault();

                        if (depositOrder != null)
                        {
                            //大团购通过支付定金 膨胀金额
                            exchangeCoupon.ExpansionAmount = sku.SKUList.Where(_ => _.ID == depositSKU.SKUID).First().MarketPrice;

                            //大团购定金的最终支付价
                            depositOrder.FinishPayPrice = depositOrder.Price - depositOrder.CashCouponAmount - depositOrder.VoucherAmount - depositOrder.UserUseHousingFundAmount;


                            //大团购定金的最终支付价
                            exchangeCoupon.FinishPayPrice = exchangeCoupon.Price - exchangeCoupon.CashCouponAmount - exchangeCoupon.VoucherAmount - exchangeCoupon.UserUseHousingFundAmount;

                            //exchangcoupon 大团购的价格存的是减去膨胀金的金额，所以在这要加上膨胀金
                            exchangeCoupon.Price = exchangeCoupon.Price + exchangeCoupon.ExpansionAmount;

                            //标记为定金产品订单
                            depositOrder.IsDepositOrder = true;
                            exchangeList.Add(depositOrder);
                        }
                    }
                }
            }

            exchangeList.Add(exchangeCoupon);
            if (exchangeCoupon.CashCouponID > 0)
            {
                UserCouponItemInfoEntity usercoupon = CouponAdapter.GetUserCouponItemInfoByID(exchangeCoupon.CashCouponID);
                if (usercoupon != null)
                {
                    exchangeList.ForEach(_ => _.CashCouponAmountName = usercoupon.Name);
                }
            }

            GroupPurchaseEntity purchase = ProductAdapter.GetGroupPurchaseEntity(exchangeCoupon.GroupId).FirstOrDefault();

            List<TravelPersonEntity> travelList = HotelAdapter.GetTravelPersonByIDS(exchangeCoupon.TraveIDs);

            result.CouponOrderID = exchangeCoupon.PayID;
            result.CouponPrice = exchangeCoupon.Price;
            result.TotalPrice = exchangeCoupon.Price;
            result.TotalPoints = exchangeCoupon.Points;
            result.PageTitle = exchangeCoupon.PageTitle;
            result.ExchangeCouponList = CouponAdapter.TransExchangeCouponEntity2ExchangeCouponModel(exchangeList);
            result.State = realState;
            result.CreateTime = exchangeCoupon.CreateTime;
            result.ExpireTime = Convert.ToDateTime(exchangeCoupon.ExpireTime);
            result.StartTime = Convert.ToDateTime(exchangeCoupon.StartTime);
            result.SkuName = (sku != null && sku.SKU != null) ? sku.SKU.Name : "";
            result.SkuID = exchangeCoupon.SKUID;
            result.CategoryParentId = (sku != null && sku.Category != null) ? sku.Category.ParentID : 0;
            result.CategoryId = (sku != null && sku.Category != null) ? sku.Category.ID : 0;
            result.SPUTempType = (sku != null && sku.Category != null) ? sku.Category.SPUTempType : 0;
            result.Notice = string.IsNullOrWhiteSpace(exchangeCoupon.Notice) ? new List<string>() : exchangeCoupon.Notice.Trim().Split('_').ToList();
            result.PackageInfoList = string.IsNullOrWhiteSpace(exchangeCoupon.PackageInfo) ? new List<string>() : exchangeCoupon.PackageInfo.Trim().Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
            result.DicProperties = StringHelper.ParseStringToDic(exchangeCoupon.Properties);
            result.GroupId = exchangeCoupon.GroupId;
            result.GroupPurchase = purchase;
            result.CouponOrderStepGroup = couponOderStepGroup;
            result.TemplateData = CouponAdapter.GetTemplateData(sku.SKU.TempSourceID, exchangeCoupon.ID, (int)HJD.HotelManagementCenter.Domain.TempLateDataType.ExchangeCouponID);
            result.TravelPerson = new List<TravelPersonEntity>();
            //20181111
            //如果订单出行人表中存在出行人信息，则不根据订单表中出行人的id获取出行人信息。之前做法订单表中存的是出行人id，若修改出行人信息，提交的信息也会随之修改，导致信息不正确
            List<CouponOrderPersonEntity> orderPersonList = CouponAdapter.GetCouponOrderPersonByOrderId(id).Where(_ => _.State == 0).ToList();
            if (orderPersonList != null && orderPersonList.Count > 0)
            {
                foreach (CouponOrderPersonEntity cop in orderPersonList)
                {
                    TravelPersonEntity tp = new TravelPersonEntity();
                    tp.CardTypeName = "";
                    tp.IDType = cop.TravelPersonCardType;
                    tp.IDNumber = cop.TravelPersonCardNo;
                    tp.TravelPersonName = cop.TravelPersonName;
                    result.TravelPerson.Add(tp);
                }
            }
            else
            {
                result.TravelPerson = exchangeCoupon.TraveIDs != null ? HotelAdapter.GetTravelPersonByIDS(exchangeCoupon.TraveIDs) : new List<TravelPersonEntity>();
            }

;
            return result;
        }

        /// <summary>
        /// 直接用券码预约模块  根据券码获取消费券信息 
        /// </summary>
        /// <param name="exchangeNo"></param>
        [HttpGet]
        public ExchangeNoBookEntity GetExchangeNoBookExchangeInfoByExchangeNo(string exchangeNo)
        {
            ExchangeNoBookEntity result = new ExchangeNoBookEntity();
            ExchangeCouponEntity exchangeCoupon = CouponAdapter.GetOneExchangeCouponInfoByExchangeNo(exchangeNo);
            if (exchangeCoupon != null && exchangeCoupon.ID > 0)
            {
                if (exchangeCoupon.State == 2)
                {
                    //判断是否是分销记账 分销记账无短信
                    if (exchangeCoupon.PayType == (int)HJD.HotelManagementCenter.Domain.PayType.Retailer || exchangeCoupon.PayType == (int)HJD.HotelManagementCenter.Domain.PayType.Retailer_NoSMS)
                    {
                        SKUEntity skuModel = ProductAdapter.GetSKUEXEntityByID(exchangeCoupon.SKUID);

                        result.UserID = exchangeCoupon.UserID;
                        result.CouponID = exchangeCoupon.ID;
                        result.SKUID = skuModel.ID;
                        result.SKUName = skuModel.Name;
                        result.PageTitle = exchangeCoupon.ObjectName;
                        result.ExpireTime = exchangeCoupon.ExpireTime == null ? "" : Convert.ToDateTime(exchangeCoupon.ExpireTime).ToString("yyyy/MM/dd");

                        if (skuModel.BookPosition == 2)
                        {
                            List<BookUserDateInfoEntity> bookList = GetBookedUserInfoByExchangid(exchangeCoupon.ID);
                            int bookcount = bookList.Where(_ => _.State == 0).Count();
                            if (bookcount > 0)
                            {
                                BookUserDateInfoEntity bookmodel = bookList.First();
                                result.BookPlayNum = bookmodel.PlayNumName;
                                result.BookTime = bookmodel.BookDay.ToString("yyyy/MM/dd");
                                result.BookState = 0;
                                result.BookID = bookmodel.ID;
                            }
                            else
                            {

                                result.BookState = 1;
                            }

                        }
                        else
                        {
                            //券码状态不对，请核对券码。
                            result.State = 4;
                            result.StateName = "券码预约状态不对，请核对券码";
                        }

                    }
                    else
                    {
                        //券码渠道信息不对，请核对券码
                        result.State = 3;
                        result.StateName = "券码渠道信息不对，请核对券码";
                    }
                }
                else
                {
                    //券码状态不对，请核对券码。
                    result.State = 2;
                    result.StateName = "券码状态不对，请核对券码";
                }
            }
            else
            {
                //没有查到对应的券信息，请核对券码。
                result.State = 1;
                result.StateName = "没有查到对应的券信息，请核对券码";
            }
            return result;
        }

        /// <summary>
        /// 检查是否成功购买过 该sku
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public bool CheckSkuGetStateByUserId(int skuId, long userId)
        {
            bool result = false;
            List<ExchangeCouponEntity> exlist = CouponAdapter.GetExchangeCouponByUserIdAndSKUID(userId, skuId);
            foreach (ExchangeCouponEntity item in exlist)
            {
                if ((item.State == 2 || item.State == 3) && item.RefundState == 0 && item.GroupId > 0)
                {
                    GroupPurchaseEntity group = ProductAdapter.GetGroupPurchaseEntity(item.GroupId).First();
                    if (group.State == 1)
                    {
                        result = true;
                    }
                }
                else if ((item.State == 2 || item.State == 3) && item.RefundState == 0)
                {
                    result = true;
                }
            }
            return result;
        }


        /// <summary>
        /// 核销消费券
        /// </summary>
        /// <param name="exchangeNo">券码</param>
        /// <param name="supplierId">供应商id</param>
        /// <param name="curUserID">操作人id</param>
        /// <returns></returns>
        [HttpGet]
        public WriteOffExchangeCouponResponse WriteOffExchangeCoupon(string exchangeNo, int supplierId, long curUserID)
        {
            WriteOffExchangeCouponResponse result = new WriteOffExchangeCouponResponse();
            if (!string.IsNullOrEmpty(exchangeNo))
            {
                ExchangeCouponEntity exchange = CouponAdapter.GetOneExchangeCouponInfoByExchangeNo(exchangeNo);
                if (exchange.State == 2)
                {
                    UsedConsumerCouponInfoEntity ucc = CouponAdapter.GetUsedCouponProductByExchangeNo(exchange.ExchangeNo);
                    if (ucc == null || ucc.ID == 0)
                    {
                        try
                        {
                            SPUEntity spuModel = ProductAdapter.GetSPUBySKUID(exchange.SKUID);
                            if (supplierId == spuModel.SupplierID)
                            {
                                SKUEntity skuModel = ProductAdapter.GetSKUItemByID(exchange.SKUID);

                                var supplierModel = SupplierAdapter.GetSupplierById(supplierId);

                                //ProductAdapter.GetProductCategoryList


                                //更新ExchangeCoupone 状态
                                ExchangeCouponEntity exchangeCoupon = new ExchangeCouponEntity();
                                exchangeCoupon.ExchangeNo = exchangeNo;
                                exchangeCoupon.State = 3;//消费状态为3
                                CouponAdapter.UpdateExchangeState(exchangeCoupon);

                                //记录到消费券表
                                UsedConsumerCouponInfoEntity model = new UsedConsumerCouponInfoEntity();
                                model.ExchangeNo = exchangeNo;
                                model.OperatorId = (int)curUserID;
                                model.SupplierId = supplierId;
                                model.CreateTime = System.DateTime.Now;
                                CouponAdapter.AddUsedConsumerCouponInfo(model);
                                result.State = 1;
                                result.StateName = "核销成功";
                                result.Price = exchange.Price;
                                result.SKUID = exchange.SKUID;
                                result.SKUName = skuModel.Name;
                                result.WriteOffTime = DateTime.Now;
                                result.SupplierName = supplierModel.Name;

                                CouponActivityEntity ca = CouponAdapter.GetCouponActivityBySKUID(exchange.SKUID);
                                //周期卡处理：在有效期内核销了一张卡就发一张新卡，金额、结算都为0 ， promotionID = -1(用以标识生成的周期卡，不记在销售数量中)
                                if (spuModel.ProductCategoryID == 27 && ca.ExpireTime >= DateTime.Now)
                                {
                                    CreateNewCouponForCycleCoupon(exchange.ID);
                                }
                            }
                            else
                            {
                                result.StateName = "券码供应商不一致";
                                result.State = 0;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("消费券码：" + exchangeNo + "   供应商ID：" + supplierId + "  userID：" + curUserID + "   error:" + e);
                            result.State = -3;
                            result.StateName = "核销出错！";
                        }
                    }
                    else
                    {
                        Log.WriteLog("消费券码：" + exchangeNo + "   供应商ID：" + supplierId + "  userID：" + curUserID);
                        result.State = -2;
                        result.StateName = "核销出错！";
                    }
                }
                else if (exchange.State == 3)
                {
                    result.State = 3;
                    result.StateName = "已核销，消费券不可用";
                }
                else
                {
                    result.StateName = "消费券不可用";
                    result.State = -1;
                }

                ProductCache.RemoveUserDetailOrderCache(exchange.ID);
            }

            return result;
        }

        /// <summary>
        /// 检查用户积分是否足够支付运费
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="points">需要积分</param>
        /// <returns></returns>
        [HttpGet]
        public bool CheckUserPointsCanPayShipping(long userId, int points)
        {
            List<PointsEntity> listPoints = PointsAdapter.GetUserPointsList(userId);
            int canTotalPoints = listPoints.Sum(_ => _.LeavePoint);
            return canTotalPoints > points;
        }

        /// <summary>
        ///  删除已核销 操作退款的券码记录  并备份到新表中
        /// </summary>
        /// <param name="exchangeNo">券码</param>
        [HttpGet]
        public void DeleteUsedConsumerCouponInfoByExchangeNo(string exchangeNo)
        {
            couponService.DeleteUsedConsumerCouponInfoByExchangeNo(exchangeNo);
        }

    }
}