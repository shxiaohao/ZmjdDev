using HJD.CouponService.Contracts;
using HJD.CouponService.Contracts.Entity;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HJDAPI.Common.DistributedLock;
using HJD.HotelManagementCenter.IServices;
using HJD.HotelManagementCenter.Domain.Fund;
using HJDAPI.Controllers.Common;
using ProductService.Contracts.Entity;
using ProductService.Contracts;
using HJDAPI.Controllers.Biz;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.Domain.Settlement;
using HJD.AccountServices.Entity;
using HJD.WeixinServices.Contract;
using HJD.WeixinServices.Contracts;
using HJDAPI.Common.Security;
using Newtonsoft.Json;
using HJDAPI.Controllers.Cache;

namespace HJDAPI.Controllers.Adapter
{
    public class CouponAdapter
    {


        public static ICouponService couponSvc = ServiceProxyFactory.Create<ICouponService>("ICouponService");
        static IWeixinService weixinService = ServiceProxyFactory.Create<IWeixinService>("IWeixinService");
        static HJD.ADServices.Contract.IADService ADService = ServiceProxyFactory.Create<HJD.ADServices.Contract.IADService>("IADService");
        public static HJD.HotelManagementCenter.IServices.IOrderService OrderService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.IOrderService>("IOrderService");
        public static HJD.HotelPrice.Contract.IHotelPriceService PriceService = ServiceProxyFactory.Create<HJD.HotelPrice.Contract.IHotelPriceService>("BasicHttpBinding_IHotelPriceService");
        public static HJD.HotelManagementCenter.IServices.ISettlementService SettleService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.ISettlementService>("ISettlementService");
        public static HJD.HotelManagementCenter.IServices.IHotelService HMC_HotelService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.IHotelService>("HMC_IHotelService");
        // public static IFundService FundService = ServiceProxyFactory.Create<IFundService>("IFundService");
        private static IProductService productSvc = ServiceProxyFactory.Create<IProductService>("basicHttpBinding_IProductService");
        static IRetailerService retailerService = ServiceProxyFactory.Create<IRetailerService>("HMC_IRetailerService");

        public static ICacheProvider LocalCache100Min = CacheManagerFactory.Create("DynamicCacheForType2");//抢券活动缓存 100分钟
        public static ICacheProvider LocalCache10Min = CacheManagerFactory.Create("DynamicCacheFor10Min");//抢券活动缓存 十分钟
        public static ICacheProvider LocalCache1Min = CacheManagerFactory.Create("DynamicCacheFor1Min");//抢券活动缓存 1分钟


        static readonly Object _obj = new object();
        const string topPicUrl = "116BSu50"; //116BRmT0 2016双十一头图      //"115VTQ60";//"115DTEz06X";//特价优惠抢购列表的置顶图

        const string GetCouponActivityDetailCache = "GetCouponActivityDetail_Ver1";
        const string GetSKUCouponActivityDetailCache = "GetSKUCouponActivityDetail_Ver1";
        const string GetSKUCouponActivityListBySKUIdsCache = "GetSKUCouponActivityListBySKUIds_Ver1";



        public static string buyCouponSuccessTemplate200 = @"您已成功购买{0}的{1}超值度假房券{2}张，券号为{3}。房券详情请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;
        public static string buyCouponTimeOutTemplate200 = @"您支付的{0}元已经收到，由于支付时间已超过10分钟，{1}没有购买成功。我们将全额退款至您的支付帐户，到账周期1-5个工作日。如需{1}，请重新提交购买。" + Configs.SMSSuffix;
        public static string buyCouponSuccessTemplate200ForPresent = @"恭喜您获赠{0}的{1}超值度假房券{2}张，券号为{3}。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;
        public static string buyCouponSuccessTemplate200Points_Free = @"您已成功领取{0}-{1}{2}张，券号为{3}。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;
        public static string buyCouponSuccessTemplate200Points = @"您已成功兑换{0}-{1}{2}张，券号为{3}。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;


        public static string buyCouponSuccessTemplate500 = @"您已成功购买{0}-{1}超值度假房券{2}张，券号为{3}。房券详情请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;
        /// <summary>
        /// 消费券短信模板
        /// </summary>
        public static string buyCouponSuccessTemplate600 = @"您已成功购买{0}-{1}{2}张，券号为{3}。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;
        /// <summary>
        /// 当消费券产品选择购买后预约并且使用系统短信的时候，需要用以下短信模板
        /// </summary>
        public static string buyCouponSuccessTemplate600_SystemInfo = @"您已成功购{0}，券码：{1}，此产品需提前至平台预约，预约成功后请按预约日期前往使用。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;

        /// <summary>
        /// 消费券壳产品短信模板
        /// </summary>
        public static string buyCouponSuccessPromotionTemplate600 = @"您已成功购买{0}-{1}{2}张。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;
        public static string buyCouponTimeOutTemplate600 = @"您支付的{0}元已经收到，由于支付时间已超过10分钟，消费券没有购买成功。我们将全额退款至您的支付帐户，到账周期1-5个工作日。如还需要消费券，请重新提交购买。" + Configs.SMSSuffix;
        public static string buyCouponSuccessTemplate600ForPresent = @"恭喜您获赠{0}-{1}{2}张，券号为{3}。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;

        public static string buyCouponSuccessTemplate600Points_Free = @"您已成功领取{0}-{1}{2}张，券号为{3}。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;
        public static string buyCouponSuccessTemplate600Points = @"您已成功兑换{0}-{1}{2}张，券号为{3}。请在微信“周末酒店服务号”或“周末酒店”APP的“我的订单”中查看订单详情。" + Configs.SMSSuffix;
        public static string refundCouponTemplate600Points_Free = " 您领取的{0}-{1}已经取消。" + Configs.SMSSuffix;
        public static string refundCouponTemplate600Points = " 您兑换的{0}-{1}已经取消，积分{2}分已经退至您的积分账户中，请注意查收。" + Configs.SMSSuffix;

        static readonly object _lock = new object();

        public static bool UpdOldVIPtoNewVIP(long userId)
        {
            return couponSvc.UpdOldVIPtoNewVIP(userId);
        }

        public static bool Del6MVIP(long userId)
        {
            return couponSvc.Del6MVIP(userId);
        }

        public enum ActivityID
        {
            VIP199_EVER = 100250,
            VIP599 = 100398,
            VIP199 = 100399,
            VIP3M = 100433,//	金牌3月
            VIP6M = 100434,	//金牌6月
            VIP199_NR = 100929,//金牌不可退
            TTGY = 100944  //天天果园

        }
        public static Int32 AdjustPriceForPackageSKU(int payid)
        {
            return couponSvc.AdjustPriceForPackageSKU(payid);
        }


        public static OriginCouponResult GenerateOriginCoupon2(long userId, CouponActivityCode typeId, string phoneNo)
        {
            return couponSvc.GenerateOriginCoupon2(userId, (int)typeId, phoneNo);
        }

        #region 使用红包

        public static int GetUserCanUseCashCouponAmount(long userid)
        {
            return couponSvc.GetUserCouponSum(userid);
        }

        public static ReturnCouponResult SetOrderCoupon(long userid, long orderid, int couponAmount, bool isBudget)
        {
            return couponSvc.SetOrderCoupon(userid, orderid, couponAmount, isBudget);
        }

        /// <summary>
        /// 获得现金列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<OriginCoupon> GetCashCouponList(long userId)
        {
            return couponSvc.GetCashCouponList(userId);
        }

        /// <summary>
        /// 获得我的现金券列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<AcquiredCoupon> GetAcquiredCouponList2(long userId, bool? isExpired)
        {
            return couponSvc.GetAcquiredCouponList2(userId, isExpired);
        }

        #endregion

        #region 生成红包
        public static string GetRedShareURL(string key)
        {
            var acUrl = string.Format("{0}/Coupon/Share?key={1}", Configs.WWWURL, key);

            //更换域名
            acUrl = AccessAdapter.GenShortUrl(0, acUrl);

            return acUrl;
        }

        public static OriginCouponResult GenerateOriginCoupon(long userId, int typeId, long sourceId, int totalMoney, int cashMoney, string moneyStr)
        {
            return new OriginCouponResult();//暂停发红包
            //  return couponSvc.GenerateOriginCoupon(userId, typeId, sourceId, totalMoney * 100, cashMoney * 100, moneyStr);
        }

        public static AcquiredCoupon GetAcquiredCoupon(long userId, string guid, string phoneNo)
        {
            return couponSvc.GetAcquiredCoupon(userId, guid, phoneNo);
        }

        public static List<AcquiredCoupon> GetAcquiredCouponList(string guid)
        {
            return couponSvc.GetAcquiredCouponList(guid);
        }

        /// <summary>
        /// 分享订单 现金返现
        /// </summary>
        /// <returns></returns>
        public static OriginCouponResult UpdateCashCoupon(long id, string guid, int state)
        {
            //TODO 小胖 调用订单，更新订单的活动返现状态为已申请
            OriginCouponResult r = null;
            if (state == 1)
            {
                OriginCoupon oc = couponSvc.GetCashCoupon(id, guid);
                if (oc != null)
                {
                    long orderID = oc.SourceId.HasValue ? (long)oc.SourceId : 0;
                    if (orderID != 0)
                    {
                        OrderAdapter.OrderActiveRebateShared(orderID);
                    }
                }
                r = couponSvc.UpdateCashCoupon(id, guid, CashCouponState.Acquired);
            }
            else if (state == 3)
            {
                r = couponSvc.UpdateCashCoupon(id, guid, CashCouponState.Cancel);
            }
            else
            {
                r = new OriginCouponResult() { Message = "此状态值仅限后台更新", Success = 1 };
            }
            return r;
        }

        /// <summary>
        /// 订单返现 发送现金券
        /// </summary>
        /// <param name="id"></param>
        /// <param name="guid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static OriginCouponResult UpdateCashCoupon20(long id, string guid, int state)
        {
            //TODO 小胖 调用订单，更新订单的活动返现状态为已申请
            OriginCouponResult r = null;
            if (state == 1)
            {
                OriginCoupon oc = couponSvc.GetCashCoupon(id, guid);
                if (oc != null)
                {
                    long orderID = oc.SourceId.HasValue ? (long)oc.SourceId : 0;
                    if (orderID != 0)
                    {
                        OrderAdapter.OrderActiveRebateShared(orderID);
                    }
                    if (oc.State != 1)
                    {
                        HJD.AccountServices.Entity.MemberProfileInfo info = AccountAdapter.GetCurrentUserInfo(oc.UserId);
                        r = couponSvc.UpdateCashCouponAndGenACouponRecord(oc.ID, CashCouponState.Acquired, oc.UserId, info.MobileAccount, (int)Math.Round(oc.TotalMoney / 10, 0));
                    }
                    else
                    {
                        r = new OriginCouponResult() { Message = "抱歉，您已经领取过现金券", Success = 3 };
                    }
                }
                else
                {
                    r = new OriginCouponResult() { Message = "没有找到OriginCoupon记录", Success = 2 };
                }
            }
            else if (state == 3)
            {
                r = couponSvc.UpdateCashCoupon(id, guid, CashCouponState.Cancel);
            }
            else
            {
                r = new OriginCouponResult() { Message = "此状态值仅限后台更新", Success = 1 };
            }
            return r;
        }
        #endregion



        #region UserCanUpdateCouponPhoto

        /// <summary>
        ///  用户是否可以重新上传照片
        /// </summary>
        /// <param name="userid"></param>
        public static Int32 GetUserCanReUpdatePhotoListByUserID(long userid)
        {
            object obj = MemCacheHelper.memcached30min.Get(GenUserCanReUpdatePhotoListByUserIDCacheKey(userid));
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        private static string GenUserCanReUpdatePhotoListByUserIDCacheKey(long userid)
        {
            return "UserCanReUpdatePhotoListByUserIDCacheKey:" + userid.ToString();
        }

        public static bool SetUserCanReUpdatePhotoListByUserID(long userid, Int32 CouponID, long operatorUserID)
        {
            MemCacheHelper.memcached30min.Set(GenUserCanReUpdatePhotoListByUserIDCacheKey(userid), CouponID);

            couponSvc.AddCouponOrderOperation(new CouponOrderOperationEntity
            {
                CreateTime = DateTime.Now,
                Creator = operatorUserID,
                ExchangeID = CouponID,
                OperationState = 0,
                Remark = "允许用户更新券照片"
            });

            return true;
        }


        /// <summary>
        /// 更新券信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static BaseResponse UpdateCoupnPhoto(UpdateCouponPhotoReqParms p)
        {
            BaseResponse resp = new BaseResponse();
            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                var coupon = CouponAdapter.GetExchangeCouponEntityListByIDList(p.CouponID).FirstOrDefault();

                couponSvc.AddCouponOrderOperation(new CouponOrderOperationEntity
                {
                    Remark = string.Format("更新照片:{0}->{1}", coupon.PhotoUrl, p.PhotoUrl),
                    OperationState = 0,
                    ExchangeID = p.CouponID,
                    Creator = coupon.UserID,
                    CreateTime = DateTime.Now
                });

                couponSvc.UpdateExchangeCouponForPhotoUrl(coupon.ID, p.PhotoUrl);

                MemCacheHelper.memcached30min.Remove(GenUserCanReUpdatePhotoListByUserIDCacheKey(coupon.UserID));
            }
            else
            {
                resp.SignError();
            }
            return resp;
        }

        #endregion


        public static SubmitExchangeRoomOrderResult SubmitExchangeRoomOrder(SubmitExchangeRoomOrderParam submitParam)
        {


            SubmitExchangeRoomOrderResult submitResult = new SubmitExchangeRoomOrderResult()
            {
                Success = 0,
                Message = "",
                HotelName = "",
                OrderID = 0,
                UserID = submitParam.userID
            };
            try
            {

                if (string.IsNullOrWhiteSpace(submitParam.exchangeNo) || string.IsNullOrWhiteSpace(submitParam.contactPhone) || submitParam.packageID == 0 || submitParam.checkIn == DateTime.MinValue || submitParam.nightCount == 0 || submitParam.userID == 0)
                {
                    submitResult = new SubmitExchangeRoomOrderResult()
                    {
                        Success = 1,
                        Message = "缺少关键参数，无法兑换酒店",
                        HotelName = "",
                        OrderID = 0,
                        UserID = submitParam.userID
                    };
                }
                else
                {



                    //获得券的相关信息 平日券(0)还是周末券(6)
                    //平日券还有周末券的验证
                    //平日券周末用的差价计算
                    ExchangeCouponEntity rightCoupon = CouponAdapter.GetOneExchangeCouponInfoByExchangeNo(submitParam.exchangeNo);
                    //验证券状态
                    if (rightCoupon.State == 2 && rightCoupon.StartTime <= DateTime.Now && rightCoupon.ExpireTime >= DateTime.Now)
                    {

                        submitResult = CheckSubmitExchangeRoomOrderStepOne(submitParam, submitResult, rightCoupon);

                        if (submitResult.Success == 0)
                        {

                            #region 套餐的验证过程
                            lock (_lock) { 

                            PackageInfoEntity packageInfo = GetPackageInfo(submitParam, rightCoupon);

                            submitResult = CheckSubmitExchangeRoomOrderStep2(submitParam, submitResult, packageInfo);

                                if (submitResult.Success == 0)
                                {


                                    HJD.HotelPrice.Contract.DataContract.Order.OrderPackageEntity package = new HJD.HotelPrice.Contract.DataContract.Order.OrderPackageEntity();
                                    package.CheckIn = submitParam.checkIn.Date;
                                    package.NightCount = submitParam.nightCount;
                                    package.Contact = submitParam.contact;
                                    package.ContactPhone = submitParam.contactPhone;
                                    package.PID = submitParam.packageID;
                                    package.RoomCount = submitParam.roomCount;
                                    package.Note = submitParam.note;
                                    //package

                                    HJD.HotelPrice.Contract.DataContract.Order.OrderMainEntity main = new HJD.HotelPrice.Contract.DataContract.Order.OrderMainEntity();
                                    main.Amount = packageInfo.Price * package.RoomCount; //rightCoupon.Price;//这里是券的实际购买价格
                                    main.Type = HJD.HotelPrice.Contract.OrderType.Package; //房券套餐 待确定 应该都是周末酒店订单套餐 类型为1
                                    main.HotelID = submitParam.hotelID;
                                    main.TerminalID = submitParam.terminalID;
                                    main.ChannelID = Convert.ToInt32(rightCoupon.CID);
                                    main.UserID = submitParam.userID;

                                    HJD.HotelPrice.Contract.DataContract.Order.OrderEntity orderEntity = new HJD.HotelPrice.Contract.DataContract.Order.OrderEntity();
                                    orderEntity.main = main;
                                    orderEntity.package = package;

                                    //出行人
                                    if (submitParam.travelId != null)
                                    {
                                        orderEntity.TravelId = submitParam.travelId;
                                    }

                                    HJD.HotelPrice.Contract.DataContract.Order.OrderSubmitResult result = OrderAdapter.SubmitOrder(orderEntity, "4.2");//调用通用的订单生成接口

                                    if (result.OrderID > 0)
                                    {
                                        List<ExchangeCouponEntity> UsedExchangeCouponList = new List<ExchangeCouponEntity> { rightCoupon };

                                        int useSouponCount = submitParam.roomCount * submitParam.nightCount / rightCoupon.NightRoomCount;
                                        if (useSouponCount > 1)//（若需要兑换得券数量大于1，则通过UserID 查询未使用，且未退款得券，取一条，加入列表，在取得过程中，并没有准确验证，该券是否能够兑换此套餐。guojia----注释）
                                        {
                                            UsedExchangeCouponList.AddRange(CouponAdapter.GetExchangeCouponListByUserIDSourceID(rightCoupon.UserID, rightCoupon.SourceID, rightCoupon.SKUID)
                                                .Where(_ => _.ExchangeNo != submitParam.exchangeNo)
                                                .Take(useSouponCount - 1));

                                        }

                                        useSouponCount = UsedExchangeCouponList.Count;
                                        decimal couponPrice = rightCoupon.Price * useSouponCount;

                                        var orderInfo = PriceService.GetOrderInfo(result.OrderID);


                                        UsedExchangeCouponList.ForEach(exchangeCoupon =>
                                        {
                                        //订单提交成功 则兑换房券信息
                                        CouponAdapter.BindOrderAndExchangeCoupon(result.OrderID, exchangeCoupon.ExchangeNo, submitParam.contactPhone, submitParam.userID);
                                            ProductCache.RemoveUserDetailOrderCache(exchangeCoupon.ID);
                                        });

                                        //房券兑换    把exchangeNo保存到客服说明字段中
                                        try
                                        {
                                            OrderService.UpdateOrderOperate_OrderOpNotice(orderInfo.ID, "兑换券码：" + string.Join(",", UsedExchangeCouponList));
                                        }
                                        catch (Exception e)
                                        {
                                            Log.WriteLog("更新客服备注报错：订单orderid " + result.OrderID + "   error  : " + e);
                                        }


                                        //设置订单(已线下支付)状态 兑换房券记录
                                        CouponAdapter.MarkCouponPayOrder(result.OrderID, (int)(couponPrice * 100), rightCoupon.UserID);//用户自己操作的兑换

                                        CouponActivityEntity activityEntity = ProductCache.GetOneCouponActivity(rightCoupon.ActivityID, false);

                                        DateTime checkInDate = submitParam.checkIn.Date;
                                        DateTime checkOutDate = submitParam.checkIn.AddDays(submitParam.nightCount).Date;

                                        Enums.CustomerType customerType = (Enums.CustomerType)rightCoupon.CustomerType;

                                        decimal packagePrice = CalCouponPackagePrice(submitParam, rightCoupon, packageInfo, checkInDate, checkOutDate, customerType);


                                        //如果需要补汇款
                                        decimal needAddPayAmount = packagePrice - couponPrice;
                                        string url = "";
                                        Log.WriteLog("needAddPayAmount:" + needAddPayAmount.ToString());
                                        if (needAddPayAmount > 0)
                                        {
                                            PackageOrderInfo20 pOrderInfo20 = OrderAdapter.GetPackageOrderInfo20(result.OrderID, rightCoupon.UserID);
                                            int addPayID = OrderAdapter.AddOrderAddPay(pOrderInfo20.ID, (int)(needAddPayAmount * 100), "房券兑换补差价", rightCoupon.UserID);
                                            string IDXs = addPayID.ToString();
                                            url = string.Format("{0}/API/OrderAPI/MergeCreateAddPayOrder?IDXs={1}&payChannelType={2}&OperatorID={3}&OrderID={4}", Configs.BGURL, IDXs, (int)PayChannelType.online, submitParam.userID, result.OrderID);
                                            HttpHelper.Get(url, "utf-8");

                                            submitResult = new SubmitExchangeRoomOrderResult()
                                            {
                                                Success = 0,
                                                Message = "您已提交兑换申请,兑换成功后还需工作人员与酒店确认房态。",
                                                HotelName = rightCoupon.ObjectName,
                                                OrderID = result.OrderID,
                                                UserID = submitParam.userID
                                            };
                                        }

                                        //  Log.WriteLog(string.Format("SubmitExchangeRoomOrder:{0}  packagePrice {1}  couponPrice {2} url:{3} ", submitParam.contactPhone, packagePrice, couponPrice, url));
                                    }
                                    else
                                    {
                                        submitResult = new SubmitExchangeRoomOrderResult()
                                        {
                                            Success = 100,
                                            Message = result.ErrorMessage,
                                            HotelName = "",
                                            OrderID = 0,
                                            UserID = submitParam.userID
                                        };
                                    }
                                }
                            }
                            #endregion
                        }

                    }
                    else
                    {
                        submitResult = new SubmitExchangeRoomOrderResult()
                        {
                            Success = 1,
                            Message = "兑换失败，请刷新页面稍后兑换",
                            HotelName = "",
                            OrderID = 0,
                            UserID = submitParam.userID
                        };
                    }
                }
            }
            catch (Exception err)
            {
                Log.WriteLog("SubmitExchangeRoomOrder:" + err.Message + err.StackTrace);
                submitResult = new SubmitExchangeRoomOrderResult()
                {
                    Success = 100,
                    Message = err.Message,
                    HotelName = "",
                    OrderID = 0,
                    UserID = submitParam.userID
                };
            }

            return submitResult;
        }

        /// <summary>
        /// 计算券兑换的套餐的价值。 团券、抢购券是在券上定义的价格，专辑是在所选的套餐上定义的价格
        /// </summary>
        /// <param name="submitParam"></param>
        /// <param name="rightCoupon"></param>
        /// <param name="packageInfo"></param>
        /// <param name="checkInDate"></param>
        /// <param name="checkOutDate"></param>
        /// <param name="packagePrice"></param>
        /// <returns></returns>
        private static int CalCouponPackagePrice(SubmitExchangeRoomOrderParam submitParam, ExchangeCouponEntity rightCoupon, PackageInfoEntity packageInfo, DateTime checkInDate, DateTime checkOutDate, Enums.CustomerType customerType = Enums.CustomerType.general)
        {
            int packagePrice = 0;
            if (rightCoupon.ActivityType == (int)CouponActivityType.专辑房券)
            {
                packagePrice = packageInfo.Price * submitParam.roomCount;
            }
            else
            {
                packagePrice = CouponAdapter.GenExchangeRoomOrderAmountWithCoupunRate(rightCoupon.ActivityID, checkInDate, checkOutDate, submitParam.roomCount, customerType);
            }

            return packagePrice;
        }

        private static SubmitExchangeRoomOrderResult CheckSubmitExchangeRoomOrderStep2(SubmitExchangeRoomOrderParam submitParam, SubmitExchangeRoomOrderResult submitResult, PackageInfoEntity packageInfo)
        {
            if (packageInfo == null || packageInfo.packageBase == null)
            {
                submitResult = new SubmitExchangeRoomOrderResult()
                {
                    Success = 7,
                    Message = "套餐已下线，请选择其他",
                    HotelName = "",
                    OrderID = 0,
                    UserID = submitParam.userID
                };
            }
            else if (packageInfo.packageBase.PackageCount == 0)
            {
                submitResult = new SubmitExchangeRoomOrderResult()
                {
                    Success = 8,
                    Message = "套餐已售韾，请选择其他",
                    HotelName = "",
                    OrderID = 0,
                    UserID = submitParam.userID
                };
            }
            else if (packageInfo.packageBase.PackageCount < (submitParam.roomCount <= 0 ? 1 : submitParam.roomCount))
            {
                submitResult = new SubmitExchangeRoomOrderResult()
                {
                    Success = 8,
                    Message = "套餐目前仅剩" + packageInfo.packageBase.PackageCount + "套, 请重新选择",
                    HotelName = "",
                    OrderID = 0,
                    UserID = submitParam.userID
                };
            }


            int dayLimitMin = packageInfo.packageBase.DayLimitMin;
            int dayLimitMax = packageInfo.packageBase.DayLimitMax;

            if (dayLimitMin != 0 || dayLimitMax != 0)
            {
                if (dayLimitMin > 0 && submitParam.nightCount < dayLimitMin)
                {
                    submitResult = new SubmitExchangeRoomOrderResult()
                    {
                        Success = 8,
                        Message = string.Format("套餐要求最少订{0}天，但目前只订了{1}天，请重新选择", dayLimitMin, submitParam.nightCount),
                        HotelName = "",
                        OrderID = 0,
                        UserID = submitParam.userID
                    };
                }
                else if (dayLimitMax > 0 && submitParam.nightCount > dayLimitMax)
                {
                    submitResult = new SubmitExchangeRoomOrderResult()
                    {
                        Success = 8,
                        Message = string.Format("套餐要求最多订{0}天，但目前订了{1}天，请重新选择", dayLimitMin, submitParam.nightCount),
                        HotelName = "",
                        OrderID = 0,
                        UserID = submitParam.userID
                    };
                }
            }

            int CustomBuyMin = packageInfo.packageBase.CustomBuyMin;
            int CustomBuyMax = packageInfo.packageBase.CustomBuyMax;
            if (CustomBuyMin != 0 || CustomBuyMax != 0)
            {
                if (CustomBuyMin > 0 && submitParam.roomCount < CustomBuyMin)
                {
                    submitResult = new SubmitExchangeRoomOrderResult()
                    {
                        Success = 8,
                        Message = string.Format("套餐要求最少订{0}套，但目前只订了{1}套，请重新选择", CustomBuyMin, submitParam.roomCount),
                        HotelName = "",
                        OrderID = 0,
                        UserID = submitParam.userID
                    };
                }
                else if (CustomBuyMax > 0 && submitParam.roomCount > CustomBuyMax)
                {
                    submitResult = new SubmitExchangeRoomOrderResult()
                    {
                        Success = 8,
                        Message = string.Format("套餐要求最多订{0}套，但目前订了{1}套，请重新选择", CustomBuyMax, submitParam.roomCount),
                        HotelName = "",
                        OrderID = 0,
                        UserID = submitParam.userID
                    };
                }
            }


            int roomNightLimitMin = packageInfo.packageBase.RoomNightLimitMin;
            int roomNightLimitMax = packageInfo.packageBase.RoomNightLimitMax;
            if (roomNightLimitMin != 0 || roomNightLimitMax != 0)
            {
                if (roomNightLimitMin > 0 && submitParam.roomCount * submitParam.nightCount < roomNightLimitMin)
                {
                    submitResult = new SubmitExchangeRoomOrderResult()
                    {
                        Success = 8,
                        Message = string.Format("套餐要求最少订{0}间夜，但目前只订了{1}间夜，请重新选择", roomNightLimitMin, submitParam.roomCount * submitParam.nightCount),
                        HotelName = "",
                        OrderID = 0,
                        UserID = submitParam.userID
                    };
                }
                else if (roomNightLimitMax > 0 && submitParam.roomCount * submitParam.nightCount > roomNightLimitMax)
                {
                    submitResult = new SubmitExchangeRoomOrderResult()
                    {
                        Success = 8,
                        Message = string.Format("套餐要求最多订{0}间夜，但目前订了{1}间夜，请重新选择", roomNightLimitMax, submitParam.roomCount * submitParam.nightCount),
                        HotelName = "",
                        OrderID = 0,
                        UserID = submitParam.userID
                    };
                }
            }


            return submitResult;
        }

        private static SubmitExchangeRoomOrderResult CheckSubmitExchangeRoomOrderStepOne(SubmitExchangeRoomOrderParam submitParam, SubmitExchangeRoomOrderResult submitResult, ExchangeCouponEntity rightCoupon)
        {
            if (rightCoupon == null)
            {
                submitResult = new SubmitExchangeRoomOrderResult()
                {
                    Success = 2,
                    Message = "房券输入错误，没有找到相关信息",
                    HotelName = "",
                    OrderID = 0,
                    UserID = submitParam.userID
                };
            }
            //20181017 不需要修改
            else if (rightCoupon.State != 2 || rightCoupon.RefundState > 0)
            {
                submitResult = new SubmitExchangeRoomOrderResult()
                {
                    Success = 110,
                    Message = "房券状态异常,无法在线兑换",
                    HotelName = "",
                    OrderID = 0,
                    UserID = submitParam.userID
                };
            }
            else if (rightCoupon.UserID != submitParam.userID)
            {
                submitResult = new SubmitExchangeRoomOrderResult()
                {
                    Success = 3,
                    Message = "房券非本人无法申请兑换",
                    HotelName = "",
                    OrderID = 0,
                    UserID = submitParam.userID
                };
            }
            else if (rightCoupon.SourceID > 0 && rightCoupon.SourceID != submitParam.packageID)
            {
                submitResult = new SubmitExchangeRoomOrderResult()
                {
                    Success = 4,
                    Message = "房券套餐与您写的套餐ID不一致",
                    HotelName = "",
                    OrderID = 0,
                    UserID = submitParam.userID
                };
            }
            else if (rightCoupon.NightRoomCount != submitParam.nightCount * submitParam.roomCount)
            {
                //需要支持兑换时一次使用多张房券的情况
                SubmitExchangeRoomOrderResult tmpSubmitResult = CheckUserCouponRoomNightCount(submitParam, rightCoupon);
                if (tmpSubmitResult.Success != 0)
                {
                    submitResult = tmpSubmitResult;
                }
            }
            return submitResult;
        }

        /// <summary>
        /// 检查用户是否有券能满足所选套餐的间夜数
        /// </summary>
        /// <param name="submitParam"></param>
        /// <param name="submitResult"></param>
        /// <param name="rightCoupon"></param>
        /// <returns></returns>
        public static SubmitExchangeRoomOrderResult CheckUserCouponRoomNightCount(SubmitExchangeRoomOrderParam submitParam, ExchangeCouponEntity rightCoupon)
        {
            SubmitExchangeRoomOrderResult submitResult = new SubmitExchangeRoomOrderResult { Success = 0 };
            int needRoomNightCount = submitParam.roomCount * submitParam.nightCount;

            var ecList = CouponAdapter.GetExchangeCouponListByUserIDSourceID(rightCoupon.UserID, rightCoupon.SourceID, rightCoupon.SKUID);

            int totalHasRoomNightCount = ecList.Sum(_ => _.NightRoomCount);

            if (totalHasRoomNightCount < needRoomNightCount)
            {//要兑换的房券不足
                submitResult = new SubmitExchangeRoomOrderResult()
                {
                    Success = 6,
                    Message = string.Format("单张房券可兑换{0}间夜，本次希望兑换{1}间夜，需要使用{2}张房券，但您的房券数不足，不能兑换。", rightCoupon.NightRoomCount, needRoomNightCount, needRoomNightCount / rightCoupon.NightRoomCount),
                    HotelName = "",
                    OrderID = 0,
                    UserID = submitParam.userID
                };
            }
            else
            {//间夜不匹配
                var oneNigtRoomCount = rightCoupon.NightRoomCount;
                var needCouponCount = needRoomNightCount / oneNigtRoomCount;

                if (needRoomNightCount != needCouponCount * oneNigtRoomCount)
                {
                    submitResult = new SubmitExchangeRoomOrderResult()
                    {
                        Success = 6,
                        Message = string.Format("单张房券可兑换{0}间夜，本次兑换{1}间夜，不是整数倍，不能完成兑换。", oneNigtRoomCount, needRoomNightCount, needCouponCount),
                        HotelName = "",
                        OrderID = 0,
                        UserID = submitParam.userID
                    };
                }
                else
                {//满足所选套餐的间夜数



                    submitResult = new SubmitExchangeRoomOrderResult()
                    {
                        Success = 0,
                        //Message = string.Format("单张房券可兑换{0}间夜，本次兑换{1}间夜，将使用{2}张房券，请确认兑换。", oneNigtRoomCount, needRoomNightCount, needCouponCount),

                        Message = "",
                        HotelName = "",
                        OrderID = 0,
                        UserID = submitParam.userID
                    };
                }
            }

            return submitResult;
        }

        private static PackageInfoEntity GetPackageInfo(SubmitExchangeRoomOrderParam submitParam, ExchangeCouponEntity rightCoupon)
        {
            PackageInfoEntity packageInfo = null;
            int hotelid = rightCoupon.ObjectID;
            if (rightCoupon.ActivityType == (int)CouponActivityType.专辑房券)
            {
                hotelid = submitParam.hotelID;
            }
            HotelPrice2 price = PriceAdapter.GetHotelPackageList(hotelid, submitParam.checkIn.ToString(), submitParam.checkIn.AddDays(submitParam.nightCount).ToString(), "www", "4.2", submitParam.userID, needNotSalePackage: true);

            foreach (PackageInfoEntity entity in price.Packages)
            {
                if (entity.packageBase.ID == submitParam.packageID)
                {
                    packageInfo = entity;
                    break;
                }
            }
            return packageInfo;
        }

        public static ExchangeRoomOrderConfirmResult IsExchangeNeedAddMoney(SubmitExchangeRoomOrderParam submitParam)
        {
            ExchangeCouponEntity rightCoupon = CouponAdapter.GetOneExchangeCouponInfoByExchangeNo(submitParam.exchangeNo);
            //使用券的数量
            var useCouponNum = submitParam.roomCount * submitParam.nightCount / rightCoupon.NightRoomCount;

            PackageInfoEntity packageInfo = GetPackageInfo(submitParam, rightCoupon);

            SubmitExchangeRoomOrderResult submitResult = new SubmitExchangeRoomOrderResult()
            {
                Success = 0,
                Message = "",
                HotelName = "",
                OrderID = 0,
                UserID = submitParam.userID
            };
            submitResult = CheckSubmitExchangeRoomOrderStep2(submitParam, submitResult, packageInfo);

            if (submitResult.Success > 0)
            {
                return new ExchangeRoomOrderConfirmResult
                {
                    IsNeedAddMoney = true,
                    AddMoneyAmount = -1,
                    Message = submitResult.Message
                };
            }
            else
            {
                ExchangeRoomOrderConfirmResult resultRoomNightCount = new ExchangeRoomOrderConfirmResult
                {
                    IsNeedAddMoney = false,
                    AddMoneyAmount = 0,
                    Message = "可以提交申请"
                };



                if (useCouponNum != 1)
                {
                    SubmitExchangeRoomOrderResult tmpSubmitResult = CheckUserCouponRoomNightCount(submitParam, rightCoupon);
                    if (tmpSubmitResult.Success != 0)
                    {
                        resultRoomNightCount = new ExchangeRoomOrderConfirmResult
                        {
                            IsNeedAddMoney = true,
                            AddMoneyAmount = -1,
                            Message = tmpSubmitResult.Message
                        };
                    }
                    else
                    {
                        resultRoomNightCount = new ExchangeRoomOrderConfirmResult
                        {
                            IsNeedAddMoney = true,
                            AddMoneyAmount = 0,
                            Message = tmpSubmitResult.Message
                        };
                    }
                }


                ExchangeRoomOrderConfirmResult resultNeedAddPay = new ExchangeRoomOrderConfirmResult
                {
                    IsNeedAddMoney = false,
                    AddMoneyAmount = 0,
                    Message = "可以提交申请"
                };
                if (resultRoomNightCount.AddMoneyAmount == 0)
                {
                    CouponActivityEntity activityEntity = ProductCache.GetOneCouponActivity(rightCoupon.ActivityID, false);

                    DateTime checkInDate = submitParam.checkIn.Date;
                    DateTime checkOutDate = submitParam.checkIn.AddDays(submitParam.nightCount).Date;



                    Enums.CustomerType customerType = (Enums.CustomerType)rightCoupon.CustomerType;

                    //2016.06.14 去掉按平日周末价格校验的过程
                    int money = CalCouponPackagePrice(submitParam, rightCoupon, packageInfo, checkInDate, checkOutDate, customerType);


                    decimal totalCouponPrice = rightCoupon.Price * useCouponNum;
                    decimal needAddMoney = money - totalCouponPrice;
                    //多余1元才需补款
                    if (needAddMoney >= 1)
                    {
                        resultNeedAddPay = new ExchangeRoomOrderConfirmResult
                        {
                            IsNeedAddMoney = true,
                            AddMoneyAmount = needAddMoney,
                            Message = "您所选择的日期需要补差价" + needAddMoney.ToString()
                            + "元，点击确认视为您确认申请兑换并同意补差价，稍后请在补款短信中进行补款。24小时内不补款，系统将自动取消订单。"
                        };
                    }
                    else if (money <= 0)
                    {
                        resultNeedAddPay = new ExchangeRoomOrderConfirmResult
                        {
                            IsNeedAddMoney = true,
                            AddMoneyAmount = -1,
                            Message = money == -1 ? "没有设置周末价不能选择周末入住" : money == -2 ? "没有设置平日价" : "没有价格字符串"
                        };
                    }
                }
                ExchangeRoomOrderConfirmResult result = new ExchangeRoomOrderConfirmResult
                {
                    IsNeedAddMoney = false,
                    AddMoneyAmount = 0,
                    Message = "可以提交申请"
                };
                result.IsNeedAddMoney = resultNeedAddPay.IsNeedAddMoney || resultRoomNightCount.IsNeedAddMoney;
                if (resultNeedAddPay.AddMoneyAmount == -1 || resultRoomNightCount.AddMoneyAmount == -1)
                {
                    result.AddMoneyAmount = -1;
                }
                else if (resultNeedAddPay.AddMoneyAmount > 0)
                {
                    result.AddMoneyAmount = resultNeedAddPay.AddMoneyAmount;
                }

                if (resultNeedAddPay.AddMoneyAmount == -1)
                {
                    result.Message = resultNeedAddPay.Message;
                }
                else if (resultRoomNightCount.AddMoneyAmount == -1)
                {
                    result.Message = resultRoomNightCount.Message;
                }
                else
                {
                    if (resultRoomNightCount.IsNeedAddMoney == true || resultNeedAddPay.IsNeedAddMoney == true)
                    {
                        result.Message = "";
                    }

                    if (resultRoomNightCount.IsNeedAddMoney == true)
                    {
                        result.Message += resultRoomNightCount.Message;
                    }

                    if (resultNeedAddPay.IsNeedAddMoney == true)
                    {
                        result.Message += resultNeedAddPay.Message;
                    }
                }

                result.UseCouponCount = useCouponNum;
                return result;
            }
        }
        public static string GetOriginGUID(long sourceId, long typeId)
        {
            return couponSvc.GetOriginGUIDByOrderAndTypeId(sourceId, typeId);
        }

        public static OriginCoupon GetCashCoupon(long id, string guid)
        {
            return couponSvc.GetCashCoupon(id, guid);
        }

        public static AcquiredCoupon GetAcquiredCouponById(long id)
        {
            return couponSvc.GetAcquiredCouponById(id);
        }

        public static List<InspectorRewardItem> GetInspectorRewardItemList(long userid)
        {
            return couponSvc.GetInspectorRewardItemList(userid);
        }

        /// <summary>
        /// 活动券分发
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <param name="phoneNo"></param>
        /// <returns></returns>
        public static OriginCouponResult GiveCouponByActivityType(long userId, CouponActivityCode code, string phoneNo)
        {
            return couponSvc.GenerateOriginCouponByActivity(userId, code, phoneNo);
        }

        public static OriginCouponResult GenerateOriginCouponEx(long userId, long sourceID, CouponActivityCode code, string phoneNo)
        {
            if (string.IsNullOrEmpty(phoneNo))
            {
                HJD.AccountServices.Entity.MemberProfileInfo info = AccountAdapter.GetCurrentUserInfo(userId);
                if (info != null)
                {
                    phoneNo = info.MobileAccount;
                }
                else
                {
                    return new OriginCouponResult() { Message = "没有找到userid对应的信息", Success = 1 };
                }
            }
            return couponSvc.GenerateOriginCouponEx(userId, sourceID, code, phoneNo);
        }

        public static CouponActivityEntity GetTodayCouponActivity()
        {
            return couponSvc.GetToDayCouponActivity();
        }

        public static CouponActivityEntity GetToDayCouponActivityAndSKU()
        {
            return couponSvc.GetToDayCouponActivityAndSKU();
        }


        /// <summary>
        /// 给指定推荐人奖励现金券
        /// </summary>
        /// <param name="userId">被推荐人ID</param>
        /// <param name="phoneNo">被推荐人手机号</param>
        public static void GiveCashCouponForSourceUser(long userId, string phoneNo)
        {
            //找到当前userId的推荐人（如果有的话）
            var originCoupon = couponSvc.GetOriginCouponByUserIdForT8(userId);
            if (originCoupon != null && originCoupon.SourceId > 0)
            {
                //获取当前推荐人的信息
                var sourceEntity = AccountAdapter.GetUserInfoByUserId(Convert.ToInt64(originCoupon.SourceId));
                if (sourceEntity != null)
                {
                    var _result = couponSvc.GenerateOriginCouponByActivity(sourceEntity.UserId, CouponActivityCode.cashcoupon50, sourceEntity.MobileAccount);
                    if (_result.Success == 1)
                    {
                        Log.WriteLog(string.Format("GiveCashCouponForSourceUser:[奖励失败] {0}:{1} - {2}:{3}", userId, phoneNo, sourceEntity.UserId, sourceEntity.MobileAccount));
                    }
                }
            }

            //Log.WriteLog(string.Format("GiveCashCouponForSourceUser:[新用户注册没有推荐人] {0}:{1}", userId, phoneNo));
        }

        /// <summary>
        /// 获取个人兑换券
        /// </summary>
        /// <returns></returns>
        public static List<ExchangeCouponEntity> GetExchangeCouponEntityListByUser(long userID, int activityType)
        {
            return couponSvc.GetExchangeCouponEntityListByUser(userID, activityType);
        }

        /// <summary>
        /// 获取用户券订单列表
        /// </summary> 
        /// <param name="couponOrderId">券id</param>
        /// <returns></returns>
        public static List<ExchangeCouponEntity> GetExchangCouponListByCouponOrderId(long couponOrderId)
        {
            return couponSvc.GetExchangCouponListByCouponOrderId(couponOrderId);
        }



        /// <summary>
        /// 获取用户所有类型券订单
        /// </summary>
        /// <param name="userId"></param> 
        /// <returns></returns>
        public static List<OrderListItemEntity> GetAllCouponOrderList(long userId)
        {
            List<ExchangeCouponEntity> list = couponSvc.GetAllCouponOrderList(userId);
            List<OrderListItemEntity> orderList = CouponAdapter.TransExchangeCouponEntityItem2OrderListItemEntity(list);
            return orderList;
        }
        /// <summary>
        /// 把券订单列表转换 List<OrderListItemEntity>
        /// </summary>
        /// <param name="list">券订单列表</param>
        /// <param name="cParentId">订单类型</param>
        /// <param name="icon">订单图标</param>
        /// <returns></returns>
        public static List<OrderListItemEntity> TransExchangeCouponEntityItem2OrderListItemEntity(List<ExchangeCouponEntity> list)
        {
            List<OrderListItemEntity> result = new List<OrderListItemEntity>();

            foreach (ExchangeCouponEntity item in list)
            {
                //根据支付ID获取券订单
                // var couoponList = CouponAdapter.GetExchangeCouponEntityByPayID(item.PayID);
                //获取用户预定信息
                var bookInfoList = ProductAdapter.GetBookUserDateInfoByExchangID(item.ID);
                OrderListItemEntity model = new OrderListItemEntity();
                model.IsPackage = item.IsPackage;
                model.TotalAmount = item.Price;
                model.TotalPoints = item.Points;
                model.NightCount = 0;
                model.OrderState = item.State;
                model.OrderStateName = item.StateName;
                model.OrderProductDesc = item.SKUName;
                model.OrderProductName = item.PageTitle;
                model.RoomCount = 0;
                model.EndDate = Convert.ToDateTime(item.ExpireTime);
                model.StartDate = item.StartTime.Year == 1 ? Convert.ToDateTime("1900-01-01") : item.StartTime;
                model.SubmitOrderDate = item.CreateTime;
                model.SKUID = item.SKUID;
                model.OrderAddPayURL = ""; //券没有补汇款
                if (bookInfoList.Count == 0)
                {
                    model.BookDate = Convert.ToDateTime("1900-01-01");
                }
                else
                {
                    model.BookDate = bookInfoList.FirstOrDefault().BookDay;
                }
                //券订单类型
                int parentId = item.ParentID;
                string icon = OrderHelper.GetIcon(parentId);
                model.Icon = icon;
                model.OrderId = item.ID;
                model.OrderType = parentId;
                model.GroupState = item.GroupState;
                model.GroupStateName = EnumHelper.GetEnumDescription<EnumHelper.GroupState>(item.GroupState);
                model.PayID = item.PayID;
                model.Count = 1;// couoponList.Count;
                model.CouponOrderId = item.CouponOrderId;
                model.CategoryParentId = item.ParentID;
                model.IsPromotion = item.PromotionID > 0 ? true : false; ;

                //获取sku属性，PropertyType=6是大团购产品 
                Tuple<OrderHelper.ProductType, OrderHelper.SubProductType> productType = GetSKUProductType(item.SKUID);
                model.ProductType = (int)productType.Item1;
                model.SubSkuType = (int)productType.Item2;

                model.ExchangeMethod = item.ExchangeMethod;
                model.ActivityType = item.ActivityType;
                model.RelPackageAlbumsID = item.RelPackageAlbumsID;
                model.BookUserDateList = GetBookedUserInfoByExchangid(item.ID);
                model.IsBook = item.BookPosition > 0 ? true : false;

                ////20181017 需要拿掉 新增了"退款中" 状态
                // //如果有State=2，RefundState > 0 ,那么 订单为退款中， 前端将State改写成7
                //if( item.State == 2 && item.RefundState > 0)
                //{
                //    model.OrderState = 7;
                //    model.OrderStateName = "退款中";

                //}


                result.Add(model);
            }
            return result;
        }
        /// <summary>
        /// 获取折叠订单
        /// </summary>
        /// <param name="list">券订单列表</param>
        /// <param name="cParentId">订单类型</param>
        /// <param name="icon">订单图标</param>
        /// <param name="productType">产品类型 0：普通产品，1：大团购产品，2: 打包产品</param>
        /// <param name="couponId">券ID</param>
        /// <param name="couponOrderId">券OrderID</param>
        /// <returns></returns>
        public static List<DetailOrderListEntity> GeCouponOrdertDetailList(List<ExchangeCouponEntity> list, int cParentId, string icon, int couponId = 0, OrderHelper.ProductType productType = OrderHelper.ProductType.Normal, long couponOrderId = 0)
        {
            List<DetailOrderListEntity> result = new List<DetailOrderListEntity>();
            if (productType == OrderHelper.ProductType.Normal)
            {
                list = list.Where(p => p.ID != couponId).OrderByDescending(p => p.CreateTime).ToList();
            }
            else if (productType == OrderHelper.ProductType.BigGroup)
            {
                list = list.Where(p => p.CouponOrderId == couponOrderId && p.CouponOrderId > 0).OrderBy(p => p.CreateTime).ToList();
            }
            else if (productType == OrderHelper.ProductType.Package)
            {
                list = list.OrderByDescending(p => p.IsPackage).ThenByDescending(p => p.CreateTime).ToList();
            }
            foreach (ExchangeCouponEntity item in list)
            {
                var bookInfoList = ProductAdapter.GetBookUserDateInfoByExchangID(item.ID);
                DetailOrderListEntity model = new DetailOrderListEntity();
                model.IsPackage = item.IsPackage;
                model.TotalAmount = item.IsPackage ? 0 : item.Price;
                model.OrderState = item.State;
                model.OrderStateName = item.StateName;
                model.OrderProductDesc = item.SKUName;
                model.OrderProductName = item.PageTitle;
                model.EndDate = Convert.ToDateTime(item.ExpireTime);
                model.StartDate = item.StartTime.Year == 1 ? Convert.ToDateTime("1900-01-01") : item.StartTime;
                model.SubmitOrderDate = item.CreateTime;
                if (bookInfoList.Count == 0)
                {
                    model.BookDate = Convert.ToDateTime("1900-01-01");
                }
                else
                {
                    model.BookDate = bookInfoList.FirstOrDefault().BookDay;
                }
                model.Icon = icon;
                model.OrderId = item.ID;
                model.OrderType = cParentId;
                model.GroupState = item.GroupState;
                model.GroupStateName = EnumHelper.GetEnumDescription<EnumHelper.GroupState>(item.GroupState);
                model.PayID = item.PayID;
                model.CouponOrderId = item.CouponOrderId;
                model.ParentOrderID = couponId;
                result.Add(model);
            }
            return result;
        }
        /// <summary>
        /// 获取大团购券折叠订单
        /// </summary>
        /// <param name="list">券订单列表</param>
        /// <param name="cParentId">订单类型</param>
        /// <param name="icon">订单图标</param>
        /// <param name="couponOrderId">券OrderID</param>
        /// <returns></returns>
        public static List<DetailOrderListEntity> GetGroupCouponOrderDetailList(List<ExchangeCouponEntity> list, int cParentId, string icon, long couponOrderId)
        {
            List<DetailOrderListEntity> result = new List<DetailOrderListEntity>();
            foreach (ExchangeCouponEntity item in list.Where(p => p.CouponOrderId != couponOrderId && p.CouponOrderId > 0))
            {
                var bookInfoList = ProductAdapter.GetBookUserDateInfoByExchangID(item.ID);
                DetailOrderListEntity model = new DetailOrderListEntity();
                model.IsPackage = item.IsPackage;
                model.TotalAmount = item.IsPackage ? 0 : item.Price;
                model.OrderState = item.State;
                model.OrderStateName = item.StateName;
                model.OrderProductDesc = item.SKUName;
                model.OrderProductName = item.PageTitle;
                model.EndDate = Convert.ToDateTime(item.ExpireTime);
                model.StartDate = item.StartTime.Year == 1 ? Convert.ToDateTime("1900-01-01") : item.StartTime;
                model.SubmitOrderDate = item.CreateTime;
                if (bookInfoList.Count == 0)
                {
                    model.BookDate = Convert.ToDateTime("1900-01-01");
                }
                else
                {
                    model.BookDate = bookInfoList.FirstOrDefault().BookDay;
                }
                model.Icon = icon;
                model.OrderId = item.ID;
                model.OrderType = cParentId;
                model.GroupState = item.GroupState;
                model.GroupStateName = EnumHelper.GetEnumDescription<EnumHelper.GroupState>(item.GroupState);
                model.PayID = item.PayID;
                model.CouponOrderId = item.CouponOrderId;
                result.Add(model);
            }
            return result;
        }
        /// <summary>
        /// 获取个人兑换券
        /// </summary>
        /// <returns></returns>
        public static List<ExchangeCouponEntity> GetOnePayExchangeCouponEntityByID(int ID)
        {
            List<ExchangeCouponEntity> list = couponSvc.GetExchangeCouponEntityListByIDList(new List<int> { ID });

            return couponSvc.GetExchangeCouponEntityByPayID(list.First().PayID);
        }


        private static Dictionary<int, Tuple<OrderHelper.ProductType, OrderHelper.SubProductType>> dicSKUProductType = new Dictionary<int, Tuple<OrderHelper.ProductType, OrderHelper.SubProductType>>();

        /// <summary>
        /// 获取一个SKU的产品类型和子产品类型
        /// </summary>
        /// <param name="skuID"></param>
        /// <returns></returns>
        public static Tuple<OrderHelper.ProductType, OrderHelper.SubProductType> GetSKUProductType(int skuID)
        {
            if (dicSKUProductType.ContainsKey(skuID) == false)
            {
                var skuEntity = ProductAdapter.GetSKUItemByID(skuID);
                var productPropertyList = ProductAdapter.GetProductPropertyInfoBySKU(skuID.ToString());

                if (productPropertyList.Count > 0)
                {
                    dicSKUProductType.Add(skuID, new Tuple<OrderHelper.ProductType, OrderHelper.SubProductType>(
                        CheckSKUProductType(productPropertyList.FirstOrDefault(), skuEntity.IsPackage),
                        CheckSKUSubProductType(productPropertyList.FirstOrDefault())));
                }
                else
                {
                    dicSKUProductType.Add(skuID, new Tuple<OrderHelper.ProductType, OrderHelper.SubProductType>(OrderHelper.ProductType.Normal, OrderHelper.SubProductType.Normal));
                }
            }

            return dicSKUProductType[skuID];

        }

        /// <summary>
        /// 判断返回产品的类型
        /// </summary>
        /// <param name="productPropertyInfoEntity">产品的类型数据</param>
        /// <param name="isPackage">产品是否为套餐产品</param>
        /// <returns></returns>
        private static OrderHelper.ProductType CheckSKUProductType(ProductPropertyInfoEntity productPropertyInfoEntity, bool isPackage)
        {
            OrderHelper.ProductType productType = OrderHelper.ProductType.Normal;
            switch (productPropertyInfoEntity.PropertyType)
            {
                case 6:
                    productType = OrderHelper.ProductType.BigGroup;
                    break;
            }

            if (productType == OrderHelper.ProductType.Normal && isPackage)
            {
                productType = OrderHelper.ProductType.Package;
            }

            return productType;
        }


        /// <summary>
        /// 判断SKU的子产品类型。如大团购的订金类型
        /// </summary>
        /// <param name="productPropertyInfoEntity"></param>
        /// <returns></returns>
        public static OrderHelper.SubProductType CheckSKUSubProductType(ProductPropertyInfoEntity productPropertyInfoEntity)
        {
            return productPropertyInfoEntity.PropertyType == 6
                && productPropertyInfoEntity.PropertyOptionTargetID == 1 ? OrderHelper.SubProductType.BigGroupSubscription : OrderHelper.SubProductType.Normal;
        }


        /// <summary>
        /// 获取活动的兑换券
        /// </summary>
        /// <returns></returns>
        public static List<ExchangeCouponEntity> GetExchangeCouponEntityListByActivity(int activityID)
        {
            return couponSvc.GetExchangeCouponEntityListByActivity(activityID);
        }

        /// <summary>
        /// 获取用户用户某个活动的券信息
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static List<ExchangeCouponEntity> GetExchangeCouponEntityListByActivityIDAndUserID(int activityID, long userID)
        {
            return couponSvc.GetExchangeCouponEntityListByActivityIDAndUserID(activityID, userID);
        }

        /// <summary>
        /// 插入兑换券记录
        /// </summary>
        /// <param name="ece"></param>
        /// <returns></returns>
        public static int InsertExchangeCoupon(ExchangeCouponEntity ece)
        {
            var sku = productSvc.GetSKUByID(ece.SKUID);
            if (ece.SKUID > 0 && ece.CID > 1)
            {
                ece.IsDistributed = sku.IsDistributable;
            }
            ece.RelSales = sku.RelSales;
            return couponSvc.InsertExchangeCoupon(ece);
        }

        /// <summary>
        /// 插入订单联系人
        /// </summary>
        /// <param name="ece"></param>
        /// <returns></returns>
        public static int AddCouponOrderPerson(CouponOrderPersonEntity param)
        {
            return couponSvc.AddCouponOrderPerson(param);
        }
        /// <summary>
        /// 更新订单联系人
        /// </summary>
        /// <param name="ece"></param>
        /// <returns></returns>
        public static int UpdateCouponOrderPerson(CouponOrderPersonEntity param)
        {
            return couponSvc.UpdateCouponOrderPerson(param);
        }
        /// <summary>
        /// 获取订单出行人信息
        /// </summary>
        /// <param name="ece"></param>
        /// <returns></returns>
        public static List<CouponOrderPersonEntity> GetCouponOrderPersonByOrderId(int exchangeId)
        {
            return couponSvc.GetCouponOrderPersonByOrderId(exchangeId);
        }

        /// <summary>
        /// 更新兑换券记录（如果ExchangeNo没有生成 可以在存储过程里更新）
        /// </summary>
        /// <param name="ece"></param>
        /// <returns></returns>
        public static int UpdateExchangeCoupon(ExchangeCouponEntity ece)
        {
            int result = couponSvc.UpdateExchangeCoupon(ece);
            //更新redis缓存
            if (result > 0)
            {
                ProductCache.RemoveUserDetailOrderCache(ece.ID);
            }
            return result;
        }



        /// <summary>
        /// 每生成一次 数据库验证 不重复返回；重复再重新生成，重新生成重复则返回null
        /// </summary>
        /// <returns></returns>
        public static void GenCouponRandomNo(int expectLen, IEnumerable<ExchangeCouponEntity> list)
        {
            string resultStr = null;
            if (list != null && list.Count() != 0)
            {
                foreach (ExchangeCouponEntity item in list)
                {
                    string key = HJDAPI.Controllers.Common.DescriptionHelper.GenAssignLengthRandomStr(expectLen);
                    resultStr = key + item.ID.ToString();
                    ExchangeCouponEntity ec = couponSvc.GetOneExchangeCouponInfo(0, resultStr);//检测数据库
                    if (ec.ActivityID > 0 || ec.ID > 0)
                    {
                        key = HJDAPI.Controllers.Common.DescriptionHelper.GenAssignLengthRandomStr(expectLen);
                        resultStr = key + item.ID.ToString();
                        ec = couponSvc.GetOneExchangeCouponInfo(0, resultStr);//检测数据库
                        if (ec.ActivityID > 0 || ec.ID > 0)
                        {
                            resultStr = null;
                        }
                    }
                    item.ExchangeNo = resultStr;
                }
            }
        }

        public static void GenIntCouponRandomNo(int expectLen, IEnumerable<ExchangeCouponEntity> list)
        {
            string resultStr = null;
            if (list != null && list.Count() != 0)
            {
                foreach (ExchangeCouponEntity item in list)
                {
                    resultStr = CheckExistExchagnNo(expectLen, item);
                    item.ExchangeNo = resultStr;
                }
            }
        }

        public static string CheckExistExchagnNo(int expectLen, ExchangeCouponEntity item)
        {
            string resultStr = null;
            string key = HJDAPI.Controllers.Common.DescriptionHelper.GenintLengthRandomStr(expectLen);
            resultStr = key + item.ID.ToString();
            ExchangeCouponEntity ec = couponSvc.GetOneExchangeCouponInfo(0, resultStr);//检测数据库
            if (ec.ActivityID > 0 || ec.ID > 0)
            {
                CheckExistExchagnNo(expectLen, item);
            }
            return resultStr;
        }

        public static bool GetTopSupplierCouponBySupplierID(int supplieId, IEnumerable<ExchangeCouponEntity> list)
        {
            bool isHaveCoupon = false;
            int listCount = list.Count();
            if (list != null && listCount != 0)
            {
                List<SupplierCouponEntity> couponList = couponSvc.GetTopCountSupplierCouponBySupplierID(listCount, supplieId, 0);
                if (couponList.Count == listCount)
                {
                    isHaveCoupon = true;
                    foreach (ExchangeCouponEntity item in list)
                    {
                        SupplierCouponEntity model = couponSvc.GetTopCountSupplierCouponBySupplierID(1, supplieId, 0).First();
                        item.ExchangeNo = model.CouponCode;
                        //更新券为已使用
                        model.State = 1;
                        couponSvc.UpdateSupplierCoupon(model);
                        Log.WriteLog("更新使用第三方券码：" + model.CouponCode);
                    }
                }
            }
            return isHaveCoupon;
        }

        public static bool GetSupplierCouponBySupplierID(int supplieId, int count, ExchangeCouponEntity exchangeCoupon)
        {
            bool isHaveCoupon = false;
            if (exchangeCoupon != null && count != 0)
            {
                List<SupplierCouponEntity> couponList = couponSvc.GetTopCountSupplierCouponBySupplierID(count, supplieId, 0);
                if (couponList.Count == count)
                {
                    isHaveCoupon = true;
                    SupplierCouponEntity model = couponSvc.GetTopCountSupplierCouponBySupplierID(1, supplieId, 0).First();
                    exchangeCoupon.ExchangeNo = model.CouponCode;
                    //更新券为已使用
                    model.State = 1;
                    couponSvc.UpdateSupplierCoupon(model);
                    Log.WriteLog("更新使用第三方券码：" + model.CouponCode);
                }
            }
            return isHaveCoupon;
        }

        public static int BindOrderAndExchangeCoupon(long orderID, string exchangeNo, string phoneNum, long updator)
        {
            return couponSvc.BindOrderAndExchangeCoupon(orderID, exchangeNo, phoneNum, updator);
        }



        public static CouponActivityEntity GetOneCouponActivityAndSKU(int id, bool isLock)
        {
            var entity = couponSvc.GetOneCouponActivityAndSKU(id, isLock);
            entity.PicList = new List<string>();
            if (entity.PicPath != null)
            {
                entity.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(surl =>
                {
                    entity.PicList.Add(PhotoAdapter.GenHotelPicUrl(surl, Enums.AppPhotoSize.appdetail1));
                });
            }

            return entity;
        }

        public static bool IsNeedGroupSponsorCanBuyVIPFirstCoupon(int activityID)
        {
            return ProductCache.GetOneCouponActivity(activityID).GroupSponsorCanBuyVIPFirstCoupon;
        }

        /// <summary>
        /// 根据产品id 获取该产品可使用的券
        /// </summary>
        /// <param name="bizId">对应的产品id</param>
        /// <param name="type">1：skuid，2：套餐id（pid）</param>
        /// <param name="userId"></param>
        /// <param name="showState">筛选条件：-1：不做过滤 0 前端不展示 1：前端展示</param>
        /// <returns></returns>
        public static PackageAndProductCouponDefineEntity GetCanCouponDefineByBizID(int bizId, int type, long userId, int showState = -1)
        {
            PackageAndProductCouponDefineEntity CouponInfo = new PackageAndProductCouponDefineEntity();
            List<UserCouponUseCondationEntity> couponUseCondationList = GetCouponUseCondationByInVal(bizId, type, showState);
            if (couponUseCondationList != null && couponUseCondationList.Count > 0)
            {
                string ids = string.Join(",", couponUseCondationList.Select(_ => _.CouponDefineID).ToList());
                List<UserCouponDefineEntity> userCouponDefine = CouponAdapter.GetUserCouponDefineListByIds(ids, userId);
                decimal sumDiscountAmount = userCouponDefine.Min(_ => _.DiscountAmount);
                CouponInfo.Icon = "http://whfront.b0.upaiyun.com/app/img/coupon/product/product-coupon-icon-1.png";
                CouponInfo.Link = "";
                CouponInfo.Tip = "领红包立减￥" + sumDiscountAmount + "起";
                CouponInfo.CouponDefineList = userCouponDefine;
            }
            return CouponInfo;
        }

        /// <summary>
        /// 是否是手拉手拼团发起人
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static bool IsGroupCouponSponser(long userID, int groupID)
        {
            return userID == productSvc.GetGroupPurchaseEntity(groupID).First().UserID;
        }

        public static BuyCouponCheckNumResult GenCouponSealInfo(int activityID, long userID)
        {
            BuyCouponCheckNumResult result = ProductCache.CouponSellInfoMem(activityID);
            var currentNum = ProductCache.GetCouponUserLockNumFromMemcache(activityID, userID);
            result.UserBuyNum = currentNum.HasLockedNum;
            return result;
        }

        public static int InsertCommOrders(HJD.CouponService.Contracts.Entity.CommOrderEntity coe)
        {
            coe.State = (int)HJDAPI.Controllers.EnumHelper.CommOrderState.log;
            return couponSvc.InsertCommOrders(coe);
        }


        /// <summary>
        /// 获取一次支付相应的券列表
        /// </summary>
        /// <param name="payID">支付ID</param>
        /// <returns></returns>
        public static List<ExchangeCouponEntity> GetOnePayRelCouponOrderList(int payID)
        {
            HJD.CouponService.Contracts.Entity.CommOrderEntity coe = couponSvc.GetOneCommOrderEntity(payID);
            if (payID == 0)
            {
                return new List<ExchangeCouponEntity>();
            }
            else
            {
                return couponSvc.GetOneCommOrderEntity(payID).RelExchangeCoupon;
            }
        }



        public static RoomCouponOrderEntity GetOneRoomCouponOrderEntityByPayID(int payID)
        {
            if (payID == 0)
            {
                return new RoomCouponOrderEntity() { };
            }

            HJD.CouponService.Contracts.Entity.CommOrderEntity coe = couponSvc.GetOneCommOrderEntity(payID);

            RoomCouponOrderEntity rcoe = new RoomCouponOrderEntity();
            if (coe.RelExchangeCoupon.Count > 0)
            {
                ExchangeCouponEntity ece = coe.RelExchangeCoupon.OrderByDescending(_ => _.Price).First();

                rcoe.CouponOrderID = ece.CouponOrderId;
                rcoe.PackageName = ece.CouponName;
                rcoe.HotelName = ece.ObjectName;
                rcoe.CouponPrice = ece.Price;
                rcoe.TotalPrice = coe.RelExchangeCoupon.Sum(_ => _.Price);
                rcoe.ExchangeCouponList = TransExchangeCouponEntity2ExchangeCouponModel(coe.RelExchangeCoupon);
                rcoe.CashCouponID = ece.CashCouponID;
                rcoe.VoucherIDs = ece.VoucherIDs;
                rcoe.SKUID = ece.SKUID;
                rcoe.CashCouponAmount = coe.RelExchangeCoupon.Sum(_ => _.CashCouponAmount);
                rcoe.VoucherAmount = coe.RelExchangeCoupon.Sum(_ => _.VoucherAmount);
                rcoe.UserUseHousingFundAmount = coe.RelExchangeCoupon.Sum(_ => _.UserUseHousingFundAmount);
                rcoe.State = ece.State;
                rcoe.ActivityType = ece.ActivityType;
                rcoe.CID = ece.CID;
                rcoe.UserId = ece.UserID;
                rcoe.PhoneNum = ece.PhoneNum;
                rcoe.IsNewRegistUser = AccountAdapter.GetUserInfoByMobile(ece.PhoneNum).IsTemporaryPWD;
                rcoe.GroupId = ece.GroupId;
                rcoe.ActivityID = ece.ActivityID;
                rcoe.SupplierID = ece.SupplierID;// coe.RelExchangeCoupon.Where(_ => _.Price != 0).Count() > 0 ? coe.RelExchangeCoupon.Where(_ => _.Price != 0).FirstOrDefault().SupplierID : 0;
                rcoe.IsVIPInvatation = ece.IsVIPInvatation;
                rcoe.TotalPoints = coe.RelExchangeCoupon.Sum(_ => _.Points);

                if (rcoe.GroupId > 0)
                {
                    //根据activeid获取成团数量 
                    CouponActivityEntity couponActivity = ProductCache.GetOneCouponActivity(ece.ActivityID);
                    try
                    {
                        foreach (ExchangeCouponEntity ex in coe.RelExchangeCoupon.Where(_ => _.GroupId > 0))
                        {
                            rcoe.GroupPurchase = productSvc.GetGroupPurchaseEntity(ex.GroupId).FirstOrDefault();
                            rcoe.GroupPurchase.GroupPeople = productSvc.GetGroupPurchaseDetailByGroupId(ex.GroupId);
                            rcoe.GroupPurchase.GroupPeople.ForEach(m => m.AvatarUrl = (string.IsNullOrWhiteSpace(m.AvatarUrl) ? DescriptionHelper.defaultAvatar :
                            PhotoAdapter.GenHotelPicUrl(m.AvatarUrl, Enums.AppPhotoSize.jupiter)));
                            int groupCount = rcoe.GroupPurchase.GroupPeople.Count;
                            rcoe.GroupPurchase.GroupShortageCount = couponActivity.GroupCount - groupCount;
                        }
                    }
                    catch (Exception err)
                    {
                        Log.WriteLog("GetOneRoomCouponOrderEntity:" + err.Message + err.StackTrace);
                    }
                }
            }
            return rcoe;
        }

        public static List<ExchangeCouponModel> TransExchangeCouponEntity2ExchangeCouponModel(List<ExchangeCouponEntity> couponList)
        {

            return couponList.Select(_ => new ExchangeCouponModel()
            {
                IsPackage = _.IsPackage,
                ActivityID = _.ActivityID,
                ActivityType = _.ActivityType,
                AddInfo = _.AddInfo,
                CancelTime = _.CancelTime,
                CanExchange = _.ExchangeMethod != 2 && _.State == 2 ? true : false,
                CouponName = _.CouponName,
                CreateTime = _.CreateTime,
                ExchangeMethod = _.ExchangeMethod,
                ExchangeNo = _.ExchangeNo,
                ExchangeTargetID = _.ExchangeTargetID,
                ExchangeTime = _.ExchangeTime,
                ExpireTime = _.ExpireTime,
                ID = _.ID,
                IsAllowMultiRoom = _.IsAllowMultiRoom,
                IsFestivalCanUse = _.IsFestivalCanUse,
                ObjectID = _.ObjectID,
                ObjectName = _.ObjectName,
                NightRoomCount = _.NightRoomCount,
                PayAccount = _.PayAccount,
                PayID = _.PayID,
                PayType = _.PayType,
                PhoneNum = _.PhoneNum,
                Price = _.Price,
                Points = _.Points,
                RefundState = _.RefundState,
                ReturnPolicy = _.ReturnPolicy,
                SourceID = _.SourceID,
                State = _.State,
                Type = _.Type,
                UpdateTime = _.UpdateTime,
                Updator = _.Updator,
                UserID = _.UserID,
                RelPackageAlbumsID = _.RelPackageAlbumsID,
                RelProductID = _.RelProductID,
                SKUID = _.SKUID,
                CID = _.CID,
                PromotionID = _.PromotionID,
                CouponNote = _.CouponNote,
                GroupId = _.GroupId,
                UserUseHousingFundAmount = _.UserUseHousingFundAmount,
                CashCouponID = _.CashCouponID,
                CashCouponAmount = _.CashCouponAmount,
                VoucherIDs = _.VoucherIDs,
                VoucherAmount = _.VoucherAmount,
                IsBook = _.BookPosition > 0 ? true : false,//productSvc.GetSKU_ExBySKUID(_.SKUID) == null ? false : productSvc.GetSKU_ExBySKUID(_.SKUID).StockCount > 0 ? true : false,
                IsCancelBook = (_.OperationState == (int)ExchangeOperationState.DoneNotWriteOff) ? false : true,// || _.BookPosition == 1
                BookUserDateList = GetBookedUserInfoByExchangid(_.ID),
                ExchangeNoType = _.ExchangeNoType,
                ExchangeTipsName = _.ExchangeTipsName,
                WeixinAcountId = _.WeixinAcountId,
                FromWeixinUid = _.FromWeixinUid,
                PageTitle = _.PageTitle,
                PhotoUrl = _.PhotoUrl,
                ExpansionAmount = _.ExpansionAmount,
                IsDepositOrder = _.IsDepositOrder,
                OriPrice = _.OriPrice,
                OrderCount = _.OrderCount,
                CouponOrderId = _.CouponOrderId,
                FinishPayPrice = _.FinishPayPrice,
                ShopWriteOffCouponTip = _.ShopWriteOffCouponTip,
                CashCouponAmountName = _.CashCouponAmountName,
                IsPromotion = _.PromotionID > 0 ? true : false,
                ShopWriteOffCouponTipList = string.IsNullOrWhiteSpace(_.ShopWriteOffCouponTip) ? new List<string>() : _.ShopWriteOffCouponTip.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList()
            }).ToList();
        }

        public static List<BookUserDateInfoEntity> GetBookedUserInfoByExchangid(int exchangid)
        {
            List<BookUserDateInfoEntity> bList = productSvc.GetBookedUserInfo(exchangid);
            foreach (var item in bList)
            {
                item.BookDay = productSvc.GetBookDateById(item.BookDateId).BookDate;
                item.PlayNumName = "";
                if (item.BookDetailId > 0)
                {
                    item.PlayNumName = productSvc.GetBookDetailById(item.BookDetailId).NumPlayName;
                }

                List<TravelPersonEntity> travelList = HotelAdapter.GetTravelPersonByIDS(item.TravelIDs);
                foreach (var travel in travelList)
                {
                    BookUserInfoEntity bmodel = new BookUserInfoEntity();
                    bmodel.CardNo = travel.IDNumber;
                    bmodel.CardType = travel.IDType;
                    bmodel.PersonName = travel.TravelPersonName;
                    item.BookUserList.Add(bmodel);
                }
            }

            return bList;
        }

        public static List<BookUserDateInfoEntity> GetBookedUserInfoByExchangIds(string exchangIds)
        {
            List<BookUserDateInfoEntity> result = new List<BookUserDateInfoEntity>();
            foreach (string i in exchangIds.Split(',').ToList())
            {
                BookUserDateInfoEntity bud = productSvc.GetBookedUserInfo(Convert.ToInt32(i)).OrderByDescending(_ => _.CreateTime).FirstOrDefault();
                if (bud != null)
                {
                    bud.BookDay = productSvc.GetBookDateById(bud.BookDateId).BookDate;
                    if (bud.BookDetailId > 0)
                    {
                        bud.PlayNumName = productSvc.GetBookDetailById(bud.BookDetailId).NumPlayName;
                    }
                    result.Add(bud);
                }
            }

            return result;
        }

        /// <summary>
        /// .... 有没有优雅点的处理方法，这个。。。。
        /// </summary>
        /// <param name="couponList"></param>
        /// <returns></returns>
        public static List<ExchangeCouponEntity> TransExchangeCouponModel2ExchangeCouponEntity(List<ExchangeCouponModel> couponList)
        {
            return couponList.Select(_ => new ExchangeCouponEntity()
            {
                ActivityID = _.ActivityID,
                ActivityType = _.ActivityType,
                AddInfo = _.AddInfo,
                CancelTime = _.CancelTime,
                CouponName = _.CouponName,
                CreateTime = _.CreateTime,
                ExchangeMethod = _.ExchangeMethod,
                ExchangeNo = _.ExchangeNo,
                ExchangeTargetID = _.ExchangeTargetID,
                ExchangeTime = _.ExchangeTime,
                ExpireTime = _.ExpireTime,
                ID = _.ID,
                IsAllowMultiRoom = _.IsAllowMultiRoom,
                IsFestivalCanUse = _.IsFestivalCanUse,
                ObjectID = _.ObjectID,
                ObjectName = _.ObjectName,
                NightRoomCount = _.NightRoomCount,
                PayAccount = _.PayAccount,
                PayID = _.PayID,
                PayType = _.PayType,
                PhoneNum = _.PhoneNum,
                Price = _.Price,
                RefundState = _.RefundState,
                ReturnPolicy = _.ReturnPolicy,
                SourceID = _.SourceID,
                State = _.State,
                Type = _.Type,
                UpdateTime = _.UpdateTime,
                Updator = _.Updator,
                UserID = _.UserID,
                RelPackageAlbumsID = _.RelPackageAlbumsID,
                RelProductID = _.RelProductID,
                SKUID = _.SKUID,
                CID = _.CID,
                PromotionID = _.PromotionID,
                CouponNote = _.CouponNote,
                GroupId = _.GroupId
            }).ToList();
        }


        public static HJD.CouponService.Contracts.Entity.CommOrderEntity GetOneCommOrderEntity(int payID)
        {
            return couponSvc.GetOneCommOrderEntity(payID);
        }

        public static int CouponRefund(int couponID)
        {
            return couponSvc.CouponRefund(couponID);
        }

        public static List<ExchangeCouponEntity> GetWaitingRefundCouponList()
        {
            return couponSvc.GetWaitingRefundCouponList();
        }

        public static int IsVipNoPayReserveUser(string userid)
        {
            return couponSvc.IsVipNoPayReserveUser(userid);
        }

        public static CouponActivityEntity GetCouponActivityEntity(int activityId)
        {
            return couponSvc.GetOneCouponActivity(activityId, true);
        }

        public static CouponActivityDetailModel GetCouponActivityDetail(int activityID, bool needBoughtItem = false)
        {
            string keyName = string.Format(GetCouponActivityDetailCache + ":{0}_{1}", activityID, needBoughtItem ? 1 : 0);
            return MemCacheHelper.memcached10min.GetData<CouponActivityDetailModel>(keyName, () =>
            {
                return GenCouponActivityDetail(activityID, needBoughtItem);
            });
        }

        public static void UpdateSKUCouponCacheByActivityID(int activityID)
        {
            var list = couponSvc.GetCouponActivitySKURelByActivityID(activityID);
            list.ForEach(r => GetSKUCouponActivityDetail(r.SKUID, true));
        }

        /// <summary>
        /// 通过SKUID获取对应活动CouponActivity详情
        /// </summary>
        /// <param name="SKUID"></param>
        /// <param name="bForceRefresh">是否强制刷新，默认为否</param>
        /// <param name="couponOrderId"></param>
        /// <returns></returns>
        public static SKUCouponActivityDetailModel GetSKUCouponActivityDetail(int SKUID, bool bForceRefresh = false)
        {
            string keyName = string.Format(GetSKUCouponActivityDetailCache + "_{0}", SKUID);
            if (bForceRefresh)
            {
                MemCacheHelper.memcached10min.Remove(keyName);
            }
            return MemCacheHelper.memcached10min.GetData<SKUCouponActivityDetailModel>(keyName, () =>
            {
                return GenSKUCouponActivityDetail(SKUID);
            });
        }

        public static GroupSKUCouponActivityModel GetGroupSKUCouponActivity(int SKUID, long userId, int groupid, bool bForceRefresh = false, string openId = "")
        {
            return GenGroupSKUCouponActivity(SKUID, userId, groupid, openId);
        }

        /// <summary>
        /// 获取指定SKUID的消费券产品列表【10min缓存】
        /// </summary>
        /// <param name="skuids"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static List<SKUCouponActivityEntity> GetSKUCouponActivityListBySKUIds(string skuids, int userid = 0)
        {
            //获取用户是否VIP
            var isVip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(userid));

            //get list
            var list = couponSvc.GetSKUCouponActivityListBySKUIds(skuids);
            if (list != null && list.Count > 0)
            {
                foreach (var entity in list)
                {
                    #region 【无缓存】生成产品图片(目前是用于列表显示，所以只生成一张)

                    entity.PicList = new List<string>();
                    if (!string.IsNullOrEmpty(entity.PicPath))
                    {
                        try
                        {
                            var picList = entity.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                            entity.PicList.Add(PhotoAdapter.GenHotelPicUrl(picList[0], Enums.AppPhotoSize.theme));
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    #endregion

                    #region 【缓存】价格缓存处理

                    var priceKeyName = string.Format("{0}-Price:{1}:{2}", GetSKUCouponActivityListBySKUIdsCache, entity.SKUID, userid);
                    var newEntity = MemCacheHelper.memcached10min.GetData<SKUCouponActivityEntity>(priceKeyName, () =>
                    {
                        var _cacheEntity = entity;

                        var skuInfo = ProductAdapter.GetSKUEXEntityByID(_cacheEntity.SKUID);
                        if (skuInfo != null && skuInfo != null)
                        {
                            _cacheEntity.SKUVipPrice = skuInfo.VIPPrice;
                            _cacheEntity.SKUPrice = skuInfo.Price;
                        }

                        //当前用户当前SKU享有的价格优惠策略
                        var promotionResult = ProductAdapter.CheckProductPromotionForCoupon(_cacheEntity.SKUID, 1, userid, ProductServiceEnums.SceneType.APP);
                        if (promotionResult != null && promotionResult.SellPriceItemList != null && promotionResult.SellPriceItemList.Count > 0 && promotionResult.SellVIPPriceItemList != null && promotionResult.SellVIPPriceItemList.Count > 0)
                        {
                            _cacheEntity.SKUVipPrice = promotionResult.SellVIPPriceItemList[0].Price;

                            if (promotionResult.PromotionRuleList != null && promotionResult.PromotionRuleList.Count > 0)
                            {
                                var _promotionRuleEntity = promotionResult.PromotionRuleList[0];
                                if (promotionResult.PromotionVIPPriceItemList != null && promotionResult.PromotionVIPPriceItemList.Count > 0 && promotionResult.PromotionVIPPriceItemList[0].Price > 0)
                                {
                                    //享有优惠
                                    if (_promotionRuleEntity.Valid)
                                    {
                                        _cacheEntity.SKUVipPrice = promotionResult.PromotionVIPPriceItemList[0].Price;

                                        _cacheEntity.ForVIPFirstBuy = true;
                                    }
                                    else
                                    {
                                        //不享有优惠，但排除一些指定情况（如新VIP爆款，普通会员也要看到优惠信息）
                                        if (_promotionRuleEntity.PromotionUseState == ProductServiceEnums.PromotionUseState.Refuse_UserPrivilege && _promotionRuleEntity.PrivID.Contains("2010") && !isVip)
                                        {
                                            _cacheEntity.SKUVipPrice = promotionResult.PromotionVIPPriceItemList[0].Price;

                                            _cacheEntity.ForVIPFirstBuy = true;
                                        }
                                    }
                                }
                            }
                        }

                        return _cacheEntity;
                    });

                    entity.SKUVipPrice = newEntity.SKUVipPrice;
                    entity.SKUPrice = newEntity.SKUPrice;
                    entity.ForVIPFirstBuy = newEntity.ForVIPFirstBuy;

                    #endregion
                }
            }

            return list;
        }


        /// <summary>
        /// 获取指定专辑下的消费券产品列表【10min缓存】
        /// </summary>
        /// <param name="albumId">专辑id</param>
        /// <param name="userid"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="sort">0默认排序rank 1售卖开始时间排序 </param>
        /// <returns></returns>
        public static SKUCouponActivityAlbumEntity GetProductAlbumSKUCouponActivityListByAlbumID(int albumId, int start = 0, int count = 10, int userid = 0, bool searchReceive = false, int districtID = 0, int commTagID = 0, int sort = 0)
        {
            SKUCouponActivityAlbumEntity SKUCouponActivityAlbum = new SKUCouponActivityAlbumEntity();
            //获取专辑信息
            ProductAlbumsEntity productAlbum = productSvc.GetProductAlbumsById(albumId);
            SKUCouponActivityAlbum.AlbumName = productAlbum.Name;
            SKUCouponActivityAlbum.Description = productAlbum.Description;
            ////获取专辑下 SKU信息
            //string skuids = string.Join(",", productSvc.GetProductAlbumSkuInfoByAlbumId(albumId).Select(_ => _.SKUID).ToList());



            int totalCount = 0;
            //获取用户是否VIP
            var isVip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(userid));
            ////用户是VIP，但没有首单优惠
            //if (isVip && !AccountAdapter.HasUserPriviledge(userid, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage))
            //{
            //    SKUCouponActivityAlbum.SKUCouponList = couponSvc.GetOldVIPSKUCouponActivityListByAlbumId(albumId, start, count, districtID, commTagID, sort, out totalCount);
            //}
            //else
            //{
            //}

            SKUCouponActivityAlbum.SKUCouponList = couponSvc.GetSKUCouponActivityListByAlbumId(albumId, start, count, districtID, commTagID, sort, out totalCount);


            //有新vip购买权限  查询是否已领取过免费产品 
            if (searchReceive == true && userid != 0)
            {
                foreach (SKUCouponActivityEntity item in SKUCouponActivityAlbum.SKUCouponList)
                {
                    item.BoughtCount = GetExchangeCouponListByUserIDAndSKU(userid, item.SKUID).Where(_ => _.State != 4 && _.State != 40 && _.State != 5).Count();
                }
            }

            SKUCouponActivityAlbum.TotalCount = totalCount;
            if (SKUCouponActivityAlbum.SKUCouponList != null && SKUCouponActivityAlbum.SKUCouponList.Count > 0)
            {
                foreach (var entity in SKUCouponActivityAlbum.SKUCouponList)
                {

                    entity.UserCouponDefineList = couponSvc.GetCouponDefineByIntval(entity.SKUID, CondationType.sku);

                    foreach (var couponDefine in entity.UserCouponDefineList)
                    {
                        int couponItemCount = couponSvc.GetUserCouponItemByUserIdAndCouponDefineId(userid, couponDefine.IDX);
                        couponDefine.UserBuyState = couponItemCount > 0 ? true : false;
                    }

                    #region 【无缓存】生成产品图片(目前是用于列表显示，所以只生成一张)

                    entity.PicList = new List<string>();
                    if (!string.IsNullOrEmpty(entity.PicPath))
                    {
                        try
                        {
                            var picList = entity.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                            entity.PicList.Add(PhotoAdapter.GenHotelPicUrl(picList[0], Enums.AppPhotoSize.theme));
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    #endregion

                    #region 【缓存】价格缓存处理

                    var priceKeyName = string.Format("{0}-Price:{1}:{2}", GetSKUCouponActivityListBySKUIdsCache, entity.SKUID, userid);
                    var newEntity = MemCacheHelper.memcached10min.GetData<SKUCouponActivityEntity>(priceKeyName, () =>
                    {
                        var _cacheEntity = entity;

                        var skuInfo = ProductAdapter.GetSKUEXEntityByID(_cacheEntity.SKUID);
                        if (skuInfo != null && skuInfo != null)
                        {
                            _cacheEntity.SKUVipPrice = skuInfo.VIPPrice;
                            _cacheEntity.SKUPrice = skuInfo.Price;
                        }

                        //当前用户当前SKU享有的价格优惠策略
                        var promotionResult = ProductAdapter.CheckProductPromotionForCoupon(_cacheEntity.SKUID, 1, userid, ProductServiceEnums.SceneType.APP);
                        if (promotionResult != null && promotionResult.SellPriceItemList != null && promotionResult.SellPriceItemList.Count > 0 && promotionResult.SellVIPPriceItemList != null && promotionResult.SellVIPPriceItemList.Count > 0)
                        {
                            _cacheEntity.SKUVipPrice = promotionResult.SellVIPPriceItemList[0].Price;

                            if (promotionResult.PromotionRuleList != null && promotionResult.PromotionRuleList.Count > 0)
                            {
                                var _promotionRuleEntity = promotionResult.PromotionRuleList[0];
                                if (promotionResult.PromotionVIPPriceItemList != null && promotionResult.PromotionVIPPriceItemList.Count > 0 && promotionResult.PromotionVIPPriceItemList[0].Price > 0)
                                {
                                    //享有优惠
                                    if (_promotionRuleEntity.Valid)
                                    {
                                        _cacheEntity.SKUVipPrice = promotionResult.PromotionVIPPriceItemList[0].Price;

                                        _cacheEntity.ForVIPFirstBuy = true;
                                    }
                                    else
                                    {
                                        //不享有优惠，但排除一些指定情况（如新VIP爆款，普通会员也要看到优惠信息）
                                        if (_promotionRuleEntity.PromotionUseState == ProductServiceEnums.PromotionUseState.Refuse_UserPrivilege && _promotionRuleEntity.PrivID.Contains("2010") && !isVip)
                                        {
                                            _cacheEntity.SKUVipPrice = promotionResult.PromotionVIPPriceItemList[0].Price;

                                            _cacheEntity.ForVIPFirstBuy = true;
                                        }
                                    }
                                }
                            }
                        }
                        List<int> skuid = new List<int>();
                        skuid.Add(_cacheEntity.SKUID);
                        skuInfo.TagsList = ProductAdapter.GetTagObjRelByIDs(ProductServiceEnums.TagObjectType.SKU, skuid);
                        //判断当sku首单_.TagID==1  表示首单
                        if (skuInfo.TagsList.FindAll(_ => _.TagID == 1).Count > 0)
                        {
                            _cacheEntity.ForVIPFirstBuy = true;
                        }

                        return _cacheEntity;
                    });

                    entity.SKUVipPrice = newEntity.SKUVipPrice;
                    entity.SKUPrice = newEntity.SKUPrice;
                    entity.ForVIPFirstBuy = newEntity.ForVIPFirstBuy;

                    #endregion

                    if (entity.UserCouponDefineList.Count > 0)
                    {
                        decimal discountAmount = entity.UserCouponDefineList.Min(_ => _.DiscountAmount);
                        entity.CouponDefineTip = "领红包立减￥" + discountAmount + "起";

                        entity.UseCouponPrice = entity.SKUVipPrice - discountAmount;
                    }
                    //try
                    //{
                    //    _item.CouponInfo = CouponAdapter.GetCanCouponDefineByBizID(entity.SKUID, 2, userid);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log.WriteLog("专辑获取可使用券报错 ：" + ex);
                    //}
                }
            }

            return SKUCouponActivityAlbum;
        }

        public static RoomCouponActivityListModel GetCouponActivityByAlbumSKU(int albumId, int type, int onlyVIP = 0, int count = -1)
        {
            RoomCouponActivityListModel model = new RoomCouponActivityListModel() { Items = new List<RoomCouponActivityListItem>() };
            //获取专辑下 去重后spu
            string spuids = string.Join(",", productSvc.GetProductAlbumSkuInfoByAlbumId(albumId).Where(p => p.State == 1).Select(_ => _.SPUID).Distinct().ToList());

            string skuids = string.Join(",", productSvc.GetSKUBySPUIDS(spuids).Select(_ => _.ID));
            List<int> activityType = new List<int>();
            activityType.Add(type);
            int totalCount = 0;
            List<CouponActivityEntity> activityList = couponSvc.GetCouponActivityBySKUIDSList(new CouponActivityQueryParam()
            {
                stateArray = new int[] { 1 },
                PageSize = 200,
                PageIndex = 1,
                activityTypeArray = activityType,
                merchantCode = CouponActivityMerchant.zmjd,
                lastEditTime = new DateTime(2000, 1, 1, 0, 0, 0),
                SKUIDS = skuids,
                ProductAlbumID = albumId
            }, out totalCount);

            if (activityList != null && activityList.Count != 0)
            {
                //只显示VIP专享的套餐
                if (onlyVIP == 1)
                {
                    activityList = activityList.Where(a => a.IsVipExclusive).ToList();
                }

                string regStr = @"_.*$";
                Regex regx = new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                for (int anum = 0; anum < activityList.Count; anum++)
                {
                    if (count < 0 || anum < count)
                    {
                        var item = activityList[anum];

                        CouponActivityDetailModel activityModel = GetCouponActivityDetail(item.ID);//使用房券活动页的缓存
                        List<TypeAndPrice> priceList = activityModel.package.PackagePrice.FindAll(_ => _.Price != 0);
                        model.Items.Add(new RoomCouponActivityListItem()
                        {
                            ActivityID = activityModel.activity.ID,
                            Rank = activityModel.activity.Rank,
                            ActivityOpenState = activityModel.activityOpenState,
                            HotelName = activityModel.package.HotelName,
                            PicUrl = activityModel.activity.PicPath,
                            Price = item.SKUPrice, //priceList.Min(_ => _.Price),   //价格取SKU价格   20170719   ---yh
                            PriceDateType = priceList[0].TypeName.Replace("券", ""),
                            PriceType = "",//暑期特价
                            LeaveNum = activityModel.activity.TotalNum - activityModel.activity.SellNum,
                            PackageBrief = activityModel.activity.PackageCode + "  " + activityModel.package.PackageBrief,
                            MarketPrice = activityModel.activity.MarketPrice,
                            StartSellTime = activityModel.activity.EffectiveTime,
                            EndSellTime = activityModel.activity.SaleEndDate,
                            DistrictId = activityModel.package.DistrictId,
                            DistrictName = activityModel.package.DistrictName,
                            GroupNo = item.GroupNo,
                            PackageCode = activityModel.activity.PackageCode,
                            TagID = item.TagID,
                            SKUSortNo = activityModel.activity.SKUSortNo,
                            AlbumsRank = item.AlbumsRank
                        });
                    }
                }
            }

            //if (model.Items != null && model.Items.Count > 0)
            //{
            //    model.Items = model.Items.Where(i => i.EndSellTime > DateTime.Now).OrderBy(i => i.Rank).ThenBy(i => i.StartSellTime).ToList();
            //}
            model.Items = model.Items.OrderBy(_ => _.AlbumsRank).ThenBy(p => p.SKUSortNo).ToList();

            return model;
        }

        public static SKUCouponActivityAlbumEntity GetProductSKUCouponList(int category = 14, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int userid = 0, int sort = 0, int payType = 0, double locLat = 0, double locLng = 0)
        {
            SKUCouponActivityAlbumEntity result = new SKUCouponActivityAlbumEntity();
            result.TotalCount = couponSvc.SKUCouponActivityByCategoryCount(category, districtID, lat, lng, geoScopeType, payType);
            result.SKUCouponList = couponSvc.SKUCouponActivityByCategory(category, districtID, lat, lng, geoScopeType, start, count, sort, payType, locLat, locLng);
            if (result.SKUCouponList != null && result.SKUCouponList.Count > 0)
            {
                var isVip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(userid));
                foreach (var entity in result.SKUCouponList)
                {

                    entity.UserCouponDefineList = couponSvc.GetCouponDefineByIntval(entity.SKUID, CondationType.sku);

                    foreach (var couponDefine in entity.UserCouponDefineList)
                    {
                        int couponItemCount = couponSvc.GetUserCouponItemByUserIdAndCouponDefineId(userid, couponDefine.IDX);
                        couponDefine.UserBuyState = couponItemCount > 0 ? true : false;
                    }

                    #region 【无缓存】生成产品图片(目前是用于列表显示，所以只生成一张)

                    entity.PicList = new List<string>();
                    if (!string.IsNullOrEmpty(entity.PicPath))
                    {
                        try
                        {
                            var picList = entity.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (!string.IsNullOrWhiteSpace(picList[0]))
                            {
                                entity.PicList.Add(PhotoAdapter.GenHotelPicUrl(picList[0], Enums.AppPhotoSize.theme));
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    #endregion

                    #region 【缓存】价格缓存处理


                    var priceKeyName = string.Format("{0}-Price:{1}:{2}", GetSKUCouponActivityListBySKUIdsCache, entity.SKUID, userid);
                    var newEntity = MemCacheHelper.memcached10min.GetData<SKUCouponActivityEntity>(priceKeyName, () =>
                    {
                        var _cacheEntity = entity;

                        var skuInfo = ProductAdapter.GetSKUEXEntityByID(_cacheEntity.SKUID);
                        if (skuInfo != null && skuInfo != null)
                        {
                            _cacheEntity.SKUVipPrice = skuInfo.VIPPrice;
                            _cacheEntity.SKUPrice = skuInfo.Price;
                        }

                        //当前用户当前SKU享有的价格优惠策略
                        var promotionResult = ProductAdapter.CheckProductPromotionForCoupon(_cacheEntity.SKUID, 1, userid, ProductServiceEnums.SceneType.APP);
                        if (promotionResult != null && promotionResult.SellPriceItemList != null && promotionResult.SellPriceItemList.Count > 0 && promotionResult.SellVIPPriceItemList != null && promotionResult.SellVIPPriceItemList.Count > 0)
                        {
                            _cacheEntity.SKUVipPrice = promotionResult.SellVIPPriceItemList[0].Price;

                            if (promotionResult.PromotionRuleList != null && promotionResult.PromotionRuleList.Count > 0)
                            {
                                var _promotionRuleEntity = promotionResult.PromotionRuleList[0];
                                if (promotionResult.PromotionVIPPriceItemList != null && promotionResult.PromotionVIPPriceItemList.Count > 0 && promotionResult.PromotionVIPPriceItemList[0].Price > 0)
                                {
                                    //享有优惠
                                    if (_promotionRuleEntity.Valid)
                                    {
                                        _cacheEntity.SKUVipPrice = promotionResult.PromotionVIPPriceItemList[0].Price;

                                        _cacheEntity.ForVIPFirstBuy = true;
                                    }
                                    else
                                    {
                                        //不享有优惠，但排除一些指定情况（如新VIP爆款，普通会员也要看到优惠信息）
                                        if (_promotionRuleEntity.PromotionUseState == ProductServiceEnums.PromotionUseState.Refuse_UserPrivilege && _promotionRuleEntity.PrivID.Contains("2010") && !isVip)
                                        {
                                            _cacheEntity.SKUVipPrice = promotionResult.PromotionVIPPriceItemList[0].Price;

                                            _cacheEntity.ForVIPFirstBuy = true;
                                        }
                                    }
                                }
                            }
                        }
                        List<int> skuid = new List<int>();
                        skuid.Add(_cacheEntity.SKUID);
                        skuInfo.TagsList = ProductAdapter.GetTagObjRelByIDs(ProductServiceEnums.TagObjectType.SKU, skuid);
                        //判断当sku首单_.TagID==1  表示首单
                        if (skuInfo.TagsList.FindAll(_ => _.TagID == 1).Count > 0)
                        {
                            _cacheEntity.ForVIPFirstBuy = true;
                        }

                        return _cacheEntity;
                    });

                    entity.SKUVipPrice = newEntity.SKUVipPrice;
                    entity.SKUPrice = newEntity.SKUPrice;
                    entity.ForVIPFirstBuy = newEntity.ForVIPFirstBuy;

                    #endregion
                }
            }
            return result;
        }

        public static SKUCouponActivityAlbumEntity GetProductSKUCouponListByID(int ID = 0, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int userid = 0, int sort = 0, int payType = 0, double locLat = 0, double locLng = 0)
        {
            SKUCouponActivityAlbumEntity result = new SKUCouponActivityAlbumEntity();
            result.SKUCouponList = couponSvc.SKUCouponActivityByID(ID, districtID, lat, lng, geoScopeType, start, count, sort, payType, locLat, locLng);
            //result.TotalCount = couponSvc.SKUCouponActivityByIDCount(ID, districtID, lat, lng, geoScopeType, payType);
            result.TotalCount = result.SKUCouponList.Count;

            if (result.SKUCouponList != null && result.SKUCouponList.Count > 0)
            {
                var isVip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(userid));
                foreach (var entity in result.SKUCouponList)
                {

                    entity.UserCouponDefineList = couponSvc.GetCouponDefineByIntval(entity.SKUID, CondationType.sku);

                    foreach (var couponDefine in entity.UserCouponDefineList)
                    {
                        int couponItemCount = couponSvc.GetUserCouponItemByUserIdAndCouponDefineId(userid, couponDefine.IDX);
                        couponDefine.UserBuyState = couponItemCount > 0 ? true : false;
                    }

                    #region 【无缓存】生成产品图片(目前是用于列表显示，所以只生成一张)

                    entity.PicList = new List<string>();
                    if (!string.IsNullOrEmpty(entity.PicPath))
                    {
                        try
                        {
                            var picList = entity.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (!string.IsNullOrWhiteSpace(picList[0]))
                            {
                                entity.PicList.Add(PhotoAdapter.GenHotelPicUrl(picList[0], Enums.AppPhotoSize.theme));
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    #endregion

                    #region 【缓存】价格缓存处理


                    var priceKeyName = string.Format("{0}-Price:{1}:{2}", GetSKUCouponActivityListBySKUIdsCache, entity.SKUID, userid);
                    var newEntity = MemCacheHelper.memcached10min.GetData<SKUCouponActivityEntity>(priceKeyName, () =>
                    {
                        var _cacheEntity = entity;

                        var skuInfo = ProductAdapter.GetSKUEXEntityByID(_cacheEntity.SKUID);
                        if (skuInfo != null && skuInfo != null)
                        {
                            _cacheEntity.SKUVipPrice = skuInfo.VIPPrice;
                            _cacheEntity.SKUPrice = skuInfo.Price;
                        }

                        //当前用户当前SKU享有的价格优惠策略
                        var promotionResult = ProductAdapter.CheckProductPromotionForCoupon(_cacheEntity.SKUID, 1, userid, ProductServiceEnums.SceneType.APP);
                        if (promotionResult != null && promotionResult.SellPriceItemList != null && promotionResult.SellPriceItemList.Count > 0 && promotionResult.SellVIPPriceItemList != null && promotionResult.SellVIPPriceItemList.Count > 0)
                        {
                            _cacheEntity.SKUVipPrice = promotionResult.SellVIPPriceItemList[0].Price;

                            if (promotionResult.PromotionRuleList != null && promotionResult.PromotionRuleList.Count > 0)
                            {
                                var _promotionRuleEntity = promotionResult.PromotionRuleList[0];
                                if (promotionResult.PromotionVIPPriceItemList != null && promotionResult.PromotionVIPPriceItemList.Count > 0 && promotionResult.PromotionVIPPriceItemList[0].Price > 0)
                                {
                                    //享有优惠
                                    if (_promotionRuleEntity.Valid)
                                    {
                                        _cacheEntity.SKUVipPrice = promotionResult.PromotionVIPPriceItemList[0].Price;

                                        _cacheEntity.ForVIPFirstBuy = true;
                                    }
                                    else
                                    {
                                        //不享有优惠，但排除一些指定情况（如新VIP爆款，普通会员也要看到优惠信息）
                                        if (_promotionRuleEntity.PromotionUseState == ProductServiceEnums.PromotionUseState.Refuse_UserPrivilege && _promotionRuleEntity.PrivID.Contains("2010") && !isVip)
                                        {
                                            _cacheEntity.SKUVipPrice = promotionResult.PromotionVIPPriceItemList[0].Price;

                                            _cacheEntity.ForVIPFirstBuy = true;
                                        }
                                    }
                                }
                            }
                        }
                        List<int> skuid = new List<int>();
                        skuid.Add(_cacheEntity.SKUID);
                        skuInfo.TagsList = ProductAdapter.GetTagObjRelByIDs(ProductServiceEnums.TagObjectType.SKU, skuid);
                        //判断当sku首单_.TagID==1  表示首单
                        if (skuInfo.TagsList.FindAll(_ => _.TagID == 1).Count > 0)
                        {
                            _cacheEntity.ForVIPFirstBuy = true;
                        }

                        return _cacheEntity;
                    });

                    entity.SKUVipPrice = newEntity.SKUVipPrice;
                    entity.SKUPrice = newEntity.SKUPrice;
                    entity.ForVIPFirstBuy = newEntity.ForVIPFirstBuy;

                    #endregion
                }
            }
            return result;
        }

        public static ProductAlbumSumEntity GetProductAlbumSummary(int albumId)
        {
            return couponSvc.GetProductAlbumSum(albumId);
        }

        /// <summary>
        /// 此处必须限制是发布状态下的活动详情
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static SKUCouponActivityDetailModel GenSKUCouponActivityDetail(int SKUID)
        {
            SKUCouponActivityDetailModel model = new SKUCouponActivityDetailModel();

            //int activityID = couponSvc.GetCouponActivitySKURelBySKUID(SKUID).CouponActivityID;
            CouponActivityEntity couponActivity = GetCouponActivityBySKUID(SKUID);
            //为0则限制
            //if (activityID > 0)
            if (couponActivity != null && couponActivity.ID > 0)
            {
                couponActivity.PicList = new List<string>();
                if (couponActivity.PicPath != null)
                {
                    couponActivity.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(surl =>
                    {
                        couponActivity.PicList.Add(PhotoAdapter.GenHotelPicUrl(surl, Enums.AppPhotoSize.appdetail1));
                    });
                }
                model.SKUInfo = ProductAdapter.GetSKUInfoByID(SKUID);
                //model.activity = GetOneCouponActivity(activityID, true);
                int tajId2 = 0;
                try
                {
                    tajId2 = model.SKUInfo.SKUList.Where(_ => _.ID == SKUID).First().TagsList.Where(t => t.TagID == 2).Count();

                    couponActivity.ExtendContent = model.SKUInfo.SKU.ExtendContent;
                }
                catch (Exception e)
                { }
                model.activity = couponActivity;
                model.activity.MoreDetailUrl = model.activity.MoreDetailUrl != null ? model.activity.MoreDetailUrl.Trim() : model.activity.MoreDetailUrl;
                if (model.activity.SellNum >= model.activity.TotalNum)
                {
                    model.activityOpenState = 0;//已售完
                }
                //为了在生产测试拿掉 || model.activity.State == 0
                else if (model.activity.ExpireTime <= DateTime.Now || model.activity.State == 0 || tajId2 > 0)
                {
                    model.activityOpenState = 2;//已结束
                }
                else
                {
                    model.activityOpenState = 1;//进行中
                }


                if (model.activity.Type == (int)CouponActivityType.闪购房券)
                {
                    model.relHotelID = PackageAdapter.GetOnePackageEntity((int)model.activity.SourceID).HotelID;
                }

            }

            return model;
        }

        /// <summary>
        /// 处理大团购SKU价格显示问题
        /// </summary>
        /// <param name="model"></param>
        public static void DealStepGroupSKUPrice(SKUCouponActivityDetailModel model, long couponOrderId)
        {
            try
            {
                if (model.SKUInfo.StepGroup != null && model.SKUInfo.StepGroup.SPUID > 0)
                {
                    CouponActivityEntity couponActivity = model.activity;

                    CouponActivityEntity ca = new CouponActivityEntity();
                    GradientDiscountEntity gradient = new GradientDiscountEntity();
                    //判断是否是定金产品
                    if (model.SKUInfo.SKU.IsDepositSKU)
                    {
                        ca = couponActivity;
                        SKUEntity sku = model.SKUInfo.SKUList.Where(_ => _.IsDepositSKU == false).First();
                        model.SKUInfo.StepGroup.MarketPrice = sku.MarketPrice;
                    }
                    else
                    {
                        model.SKUInfo.StepGroup.MarketPrice = model.SKUInfo.SKU.MarketPrice;
                        SKUEntity sku = model.SKUInfo.SKUList.Where(_ => _.IsDepositSKU == true).First();
                        //得到销售数量
                        ca = GetCouponActivityBySKUID(sku.ID);
                        //判断是否是支付尾款
                        if (couponOrderId > 0)
                        {
                            int couponorderCount = couponSvc.GetExchangCouponCountByCouponOrderID(couponOrderId, new List<int>() { 2 });
                            if (couponorderCount > 0)
                            {
                                decimal payPrice = model.SKUInfo.SKU.Price - sku.MarketPrice;
                                model.SKUInfo.SKU.Price = payPrice;
                                model.SKUInfo.SKU.VIPPrice = payPrice;
                            }
                        }
                    }
                    int totalNum = (ca.SellNum + ca.ManuSellNum) > ca.TotalNum ? ca.TotalNum : ca.SellNum + ca.ManuSellNum;
                    gradient = model.SKUInfo.StepGroup.GradientPriceList.Where(_ => _.GroupCount > totalNum).OrderBy(_ => _.GroupCount).FirstOrDefault();
                    if (gradient != null && gradient.GroupCount > 0)
                    {
                        model.SKUInfo.StepGroup.ShortPeopleCount = gradient.GroupCount - totalNum;
                        model.SKUInfo.StepGroup.NextPrice = gradient.Price;

                    }
                    GradientDiscountEntity gradient1 = new GradientDiscountEntity();
                    gradient1 = model.SKUInfo.StepGroup.GradientPriceList.Where(_ => _.GroupCount <= totalNum).OrderByDescending(_ => _.GroupCount).FirstOrDefault();
                    if (gradient1 != null && gradient1.Price > 0)
                    {
                        model.SKUInfo.StepGroup.CurrentPrice = gradient1.Price;
                    }
                    else
                    {
                        model.SKUInfo.StepGroup.CurrentPrice = 0;
                    }

                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("大团购报错：" + ex);
            }
        }

        public static GroupSKUCouponActivityModel GenGroupSKUCouponActivity(int SKUID, long userId, int groupid, string openId = "")
        {

            GroupPurchaseEntity groupPurchase = ProductAdapter.GetGroupPurchaseEntity(groupid).FirstOrDefault();
            if (groupPurchase != null && groupPurchase.SKUID > 0)
            {
                SKUID = groupPurchase.SKUID;
            }

            GroupSKUCouponActivityModel model = new GroupSKUCouponActivityModel();
            //int activityID = couponSvc.GetCouponActivitySKURelBySKUID(SKUID).CouponActivityID;

            CouponActivityEntity couponActivity = GetCouponActivityBySKUID(SKUID);

            //为0则限制
            if (couponActivity.ID > 0 && couponActivity != null)
            {
                couponActivity.PicList = new List<string>();
                if (couponActivity.PicPath != null)
                {
                    couponActivity.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(surl =>
                    {
                        couponActivity.PicList.Add(PhotoAdapter.GenHotelPicUrl(surl, Enums.AppPhotoSize.appdetail1));
                    });
                }
                model.activity = couponActivity;//GetOneCouponActivity(activityID, true);
                if (model.activity.SellNum >= model.activity.TotalNum)
                {
                    model.ActivityState = 0;//已售完
                }
                else if (model.activity.ExpireTime <= DateTime.Now)
                {
                    model.ActivityState = 2;//已结束
                }
                else
                {
                    model.ActivityState = 1;//进行中
                }

                model.SKUInfo = ProductAdapter.GetSKUInfoByID(SKUID);
                int categoryId = model.SKUInfo.Category != null ? model.SKUInfo.Category.ID : 0;


                GenGroupPurchaseInfo(model, SKUID, groupid, userId, categoryId, openId);

                ////判断 是否参与过 该sku团购(排除拼团失败)
                //if (!string.IsNullOrEmpty(openId))
                //{
                //    List<GroupPurchaseInfoEntity> groupList = productSvc.GetGroupPurchaseCountBySkuAndUserid(SKUID, 0, openId);
                //    model.IsJoinSKU = groupList.FindAll(_ => _.State == 0 || _.State == 1).Count > 0 ? true : false;
                //    model.IsJoinGroup = groupList.Where(_ => _.GroupId == groupid).Count() > 0 ? true : false;
                //}
                //else if (userId > 0)
                //{
                //    List<GroupPurchaseInfoEntity> groupList = productSvc.GetGroupPurchaseCountBySkuAndUserid(SKUID, userId, "");
                //    model.IsJoinSKU = groupList.FindAll(_ => _.State == 0 || _.State == 1).Count > 0 ? true : false;
                //    model.IsJoinGroup = groupList.Where(_ => _.GroupId == groupid).Count() > 0 ? true : false;
                //}
                //else
                //{
                //    //productSvc.getgrou
                //    model.IsJoinSKU = false;
                //    model.IsJoinGroup = false;
                //}

                //if (groupid > 0)
                //{
                //    GenGroupPurchase(model, groupid, userId, categoryId, openId);
                //}
                //else if (userId > 0 || !string.IsNullOrEmpty(openId))
                //{
                //    List<GroupPurchaseInfoEntity> groupList = productSvc.GetGroupPurchaseCountBySkuAndUserid(SKUID, userId, openId);
                //    if (groupList.Count > 0)
                //    {
                //        GroupPurchaseInfoEntity groupEntity = groupList.First();
                //        GenGroupPurchase(model, groupEntity.ID, userId, categoryId, openId);
                //    }
                //}

                //string skuids = string.Join(",", model.SKUInfo.SKUList.Select(_ => _.ID));
                //List<ProductPropertyInfoEntity> propertyInfo = ProductAdapter.GetProductPropertyInfoBySKU(skuids);

                //model.SKUInfo.SKU.IsGroupSKU = propertyInfo.FindAll(p => p.SKUID == model.SKUInfo.SKU.ID && p.PropertyType == (int)Enums.PropertyType.拼团 && p.PropertyOptionTargetID == 2).Count() > 0 ? true : false;

                //model.SKUInfo.SKUList.ForEach(_ => _.IsGroupSKU = propertyInfo.FindAll(p => p.SKUID == _.ID && p.PropertyType == (int)Enums.PropertyType.拼团 && p.PropertyOptionTargetID == 2).Count() > 0 ? true : false);
            }
            return model;
        }


        public static CouponActivityEntity GetCouponActivityBySKUID(int skuid)
        {
            var entity = couponSvc.GetCouponActivityBySKUID(skuid);
            entity.SellNum = ProductCache.GetOneActivityCouponSellNum(entity.ID);
            return entity;
        }

        public static void GenGroupPurchaseInfo(GroupSKUCouponActivityModel model, int skuid, int groupid, long userId, int categoryId = 0, string openId = "")
        {
            if (groupid > 0)
            {
                if (!string.IsNullOrEmpty(openId))
                {
                    List<GroupPurchaseInfoEntity> groupList = productSvc.GetGroupPurchaseCountBySkuAndUserid(skuid, 0, openId);
                    model.IsJoinSKU = groupList.FindAll(_ => _.State == 0 || _.State == 1).Count > 0 ? true : false;
                    model.IsJoinGroup = groupList.Where(_ => _.GroupId == groupid).Count() > 0 ? true : false;

                    GenGroupPurchase(model, groupid, 0, categoryId, openId);

                    model.IsCreator = model.GroupPurchase.GroupPeople.FindAll(_ => (_.OpenId == openId) && _.IsSponsor == true).Count > 0 ? true : false;
                }
                else if (userId > 0)
                {
                    List<GroupPurchaseInfoEntity> groupList = productSvc.GetGroupPurchaseCountBySkuAndUserid(skuid, userId, "");
                    model.IsJoinSKU = groupList.FindAll(_ => _.State == 0 || _.State == 1).Count > 0 ? true : false;
                    model.IsJoinGroup = groupList.Where(_ => _.GroupId == groupid).Count() > 0 ? true : false;
                    GenGroupPurchase(model, groupid, userId, categoryId, "");
                    model.IsCreator = model.GroupPurchase.GroupPeople.FindAll(_ => (_.UserId == userId) && _.IsSponsor == true).Count > 0 ? true : false;
                }
                else
                {
                    GenGroupPurchase(model, groupid, userId, categoryId, "");
                    model.IsJoinSKU = false;
                    model.IsJoinGroup = false;
                    model.IsCreator = false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(openId))
                {
                    List<GroupPurchaseInfoEntity> groupList = productSvc.GetGroupPurchaseCountBySkuAndUserid(skuid, 0, openId);
                    model.IsJoinSKU = groupList.FindAll(_ => _.State == 0 || _.State == 1).Count > 0 ? true : false;
                    model.IsJoinGroup = groupList.Where(_ => _.GroupId == groupid).Count() > 0 ? true : false;
                    if (groupList.Count > 0)
                    {
                        GroupPurchaseInfoEntity groupEntity = groupList.First();
                        GenGroupPurchase(model, groupid, 0, categoryId, openId);
                        model.IsCreator = model.GroupPurchase.GroupPeople.FindAll(_ => (_.OpenId == openId) && _.IsSponsor == true).Count > 0 ? true : false;
                    }
                }
                else if (userId > 0)
                {
                    List<GroupPurchaseInfoEntity> groupList = productSvc.GetGroupPurchaseCountBySkuAndUserid(skuid, userId, "");
                    model.IsJoinSKU = groupList.FindAll(_ => _.State == 0 || _.State == 1).Count > 0 ? true : false;
                    model.IsJoinGroup = groupList.Where(_ => _.GroupId == groupid).Count() > 0 ? true : false;
                    if (groupList.Count > 0)
                    {
                        GroupPurchaseInfoEntity groupEntity = groupList.First();
                        GenGroupPurchase(model, groupEntity.ID, userId, categoryId, "");
                        model.IsCreator = model.GroupPurchase.GroupPeople.FindAll(_ => (_.UserId == userId) && _.IsSponsor == true).Count > 0 ? true : false;
                    }
                }
                else
                {
                    model.IsJoinSKU = false;
                    model.IsJoinGroup = false;
                    model.IsCreator = false;
                }

            }

        }

        /// <summary>
        /// 判断是否可以跳过需要VIP首单权限的检查
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="activityID"></param>
        /// <param name="userID"></param>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public static bool CanJumpOverCanBuyVIPFirstBuyPackage(int activityType, int activityID, long userID, int GroupId)
        {
            if (activityType == 400)
            {
                return true;
            }
            else
            {
                return IsGroupSponserAndCanBuyVIPFirstBuyPackage(activityID, userID, GroupId);
            }
        }

        /// <summary>
        /// 是否需要检查用户是否有首单权限
        /// 如果是手拉手、发起人可享受首单权限、当前用户是发起人的话，就不需要检查
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="userID"></param>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public static bool IsGroupSponserAndCanBuyVIPFirstBuyPackage(int activityID, long userID, int GroupId)
        {
            return GroupId > 0
                                               && IsNeedGroupSponsorCanBuyVIPFirstCoupon(activityID)
                                               && IsGroupCouponSponser(userID, GroupId);
        }

        public static void GenGroupPurchase(GroupSKUCouponActivityModel model, int groupid, long userId, int categoryId = 0, string openId = "")
        {
            model.GroupPurchase = ProductAdapter.GetGroupPurchaseEntity(groupid).FirstOrDefault();
            model.GroupPurchase.GroupPeople = ProductAdapter.GetGroupPurchaseDetailByGroupId(groupid, categoryId);
            if (categoryId == 21)
            {
                model.GroupPurchase.GroupPeople.ForEach(m => m.AvatarUrl = (string.IsNullOrWhiteSpace(m.AvatarUrl) ? DescriptionHelper.defaultAvatar : m.AvatarUrl));
            }
            else
            {
                model.GroupPurchase.GroupPeople.ForEach(m => m.AvatarUrl = (string.IsNullOrWhiteSpace(m.AvatarUrl) ? DescriptionHelper.defaultAvatar :
                PhotoAdapter.GenHotelPicUrl(m.AvatarUrl, Enums.AppPhotoSize.jupiter)));
            }
            int groupCount = model.GroupPurchase.GroupPeople.Count;
            model.GroupPurchase.GroupPeopleCount = groupCount;
            model.GroupPurchase.GroupShortageCount = model.activity.GroupCount - groupCount;

            //model.IsJoinGroup = true;

            //model.IsCreator = model.GroupPurchase.GroupPeople.FindAll(_ => (_.UserId == userId || _.OpenId == openId) && _.IsSponsor == true).Count > 0 ? true : false;
        }

        public static List<ExchangeCouponEntity> GetExchangeCouponListByUserIDAndSKU(long userId, int SKUID)
        {
            return couponSvc.GetExchangeCouponListByUserIDAndSKU(userId, SKUID);
        }


        public static List<ExchangeCouponEntity> GetExchangeCouponEntityListByIDList(List<int> ids)
        {
            return couponSvc.GetExchangeCouponEntityListByIDList(ids);
        }
        /// <summary>
        /// 此处必须限制是发布状态下的活动详情
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static CouponActivityDetailModel GenCouponActivityDetail(int activityID, bool needBoughtItem = false)
        {
            CouponActivityDetailModel model = new CouponActivityDetailModel();

            //为0则限制
            if (activityID == 0)
            {
                //model.activity = GetTodayCouponActivity();
                model.activity = GetToDayCouponActivityAndSKU();
            }
            else
            {
                //model.activity = GetOneCouponActivity(activityID, true);
                model.activity = GetOneCouponActivityAndSKU(activityID, true);

            }

            //获取activity的销售数理
            model.activity.SellNum = ProductCache.GetOneActivityCouponSellNum(model.activity.ID);



            //无套餐或状态为已关闭 不出数据
            if (model.activity.SourceID == 0)
            {
                return model;
            }

            if (model.activity.SellNum >= model.activity.TotalNum)
            {
                model.activityOpenState = 0;//已售完
            }
            //为了在生产测试拿掉 || model.activity.State == 0
            else if (model.activity.ExpireTime <= DateTime.Now || model.activity.State == 0)
            {
                model.activityOpenState = 2;//已结束
            }
            else
            {
                model.activityOpenState = 1;//进行中
            }

            int pid = (int)model.activity.SourceID;
            model.package = HotelAdapter.GetTopNPackageItem(pid);//此处获得的PackagePrice是常规价格.在抢购套餐活动中,应该取活动维护的价格(两个及以上以逗号相隔，/^\d+,\d+,\d+$/gi)
            string priceStr = model.activity.Price;
            //没有设置则按默认套餐价出
            if (!string.IsNullOrWhiteSpace(priceStr) && priceStr.Length > 0)
            {
                //有则设置PackagePrice
                string[] priceArray = priceStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (priceArray != null && priceArray.Length > 0)
                {
                    //依次显示 平日 周末 节假日 价格
                    model.package.PackagePrice = new List<TypeAndPrice>();
                    for (int i = 0; i < priceArray.Length; i++)
                    {
                        decimal price = decimal.Parse(priceArray[i]);
                        if (price != 0)
                        {
                            model.package.PackagePrice.Add(new TypeAndPrice() { PID = pid, Price = (int)price, Type = i == 0 ? 0 : i == 1 ? 6 : 8 });
                        }
                    }
                }
            }

            string priceAddInfo = "";
            bool isBothDayAndWeekend = false;
            if (model.package.PackagePrice.Count > 1)
            {
                int dayPrice = model.package.PackagePrice.Find(_ => _.Type == 0).Price;
                int otherPrice = model.package.PackagePrice.FindAll(_ => _.Type > 0).Min(_ => _.Price);
                decimal marginPrice = otherPrice - dayPrice;
                if (marginPrice > 0)
                {
                    priceAddInfo = string.Format("周末需补价{0}元", marginPrice);
                }

                if (dayPrice > 0 && otherPrice > 0 && dayPrice == otherPrice)
                {
                    isBothDayAndWeekend = true;
                }
            }

            model.package.PackagePrice.ForEach((_) =>
            {
                if (_.Type == 0)
                {
                    _.AddInfo = isBothDayAndWeekend ? "平日周末均可使用" : priceAddInfo;
                    _.TypeName = isBothDayAndWeekend ? "通用券" : "平日券";
                }
                else if (_.Type == 6)
                {
                    _.AddInfo = isBothDayAndWeekend ? "平日周末均可使用" : "";
                    _.TypeName = isBothDayAndWeekend ? "通用券" : "周末券";
                }
            });

            #region 获取套餐信息

            model.DailyItems = new List<HJD.HotelManagementCenter.Domain.PItemEntity>();

            if (!string.IsNullOrEmpty(model.activity.PackageInfo))
            {
                #region 通过设置的PackageInfo值解析出套餐列表

                var packageList = (model.activity.PackageInfo.Contains("\r\n") ? Regex.Split(model.activity.PackageInfo, "\r\n") : Regex.Split(model.activity.PackageInfo, "\n"));
                if (packageList != null && packageList.Length > 0)
                {
                    foreach (var pItem in packageList)
                    {
                        model.DailyItems.Add(new HJD.HotelManagementCenter.Domain.PItemEntity()
                        {
                            Description = pItem.Trim(),
                            SourceType = 0,
                            HotelID = model.package.HotelID,
                            ID = model.package.PackageID,
                            ItemCode = model.package.PackageName,
                            Price = 0,
                            Type = 0
                        });
                    }
                }

                #endregion
            }
            else
            {
                #region 获取酒店套餐的信息

                //获取清单和注意事项
                List<HJD.HotelManagementCenter.Domain.PItemEntity> pItems = HotelAdapter.HMC_PackageService.GetPItemOfPackage(pid);
                List<HJD.HotelManagementCenter.Domain.PItemRelEntity> pirl = HotelAdapter.HMC_PackageService.GetPItemRel(pid).Where(p => p.DateType == 0).ToList();

                int index = 1;
                DateTime date = DateTime.Parse("1900-1-1");
                var ItemRels = (from l in pItems
                                join p1 in pirl
                                 on l.ID * 1000 + l.SourceType equals p1.ItemID * 1000 + p1.SourceType
                                into aj
                                from a in aj.DefaultIfEmpty()
                                orderby l.Type, (a == null ? 1000 : a.ID)
                                select new PItemRelItemEntity
                                {
                                    Sort = a == null ? 1000 : index++,
                                    date = date,
                                    dateType = 0,
                                    Description = l.Description,
                                    HotelID = l.HotelID,
                                    ID = l.ID,
                                    ItemCode = l.ItemCode,
                                    PID = pid,
                                    Price = l.Price,
                                    Type = l.Type,
                                    SourceType = l.SourceType
                                }).OrderBy(r => r.Sort).ToList();

                //增加房型信息
                PackageInfoEntity pie = HotelAdapter.HotelService.GetHotelPackages(model.package.HotelID, model.package.PackageID).First();
                if (pie != null && pie.Room != null && !string.IsNullOrEmpty(pie.Room.Description))
                {
                    string roomInfo = "1晚" + pie.Room.Description;
                    model.DailyItems = new List<HJD.HotelManagementCenter.Domain.PItemEntity>() { new HJD.HotelManagementCenter.Domain.PItemEntity() { Description = roomInfo, Type = 1, HotelID = model.package.HotelID, ID = 0, ItemCode = "", Price = 0, SourceType = 0 } };
                }
                //model.DailyItems.AddRange(pItems.FindAll(_ => _.Type == 1).ToList());
                model.DailyItems.AddRange(ItemRels.FindAll(_ => _.Type == 1).Select(_ => new HJD.HotelManagementCenter.Domain.PItemEntity()
                {
                    Description = _.Description,
                    SourceType = _.SourceType,
                    HotelID = _.HotelID,
                    ID = _.ID,
                    ItemCode = _.ItemCode,
                    Price = (int)_.Price,
                    Type = _.Type
                }));

                #endregion
            }

            #endregion

            model.NoticeItems = new List<HJD.HotelManagementCenter.Domain.PItemEntity>();
            string extraNoticeItem = model.activity.Notice;

            if (!string.IsNullOrWhiteSpace(extraNoticeItem))
            {
                string[] noticeItems = extraNoticeItem.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                model.NoticeItems = noticeItems.Select(_ => new HJD.HotelManagementCenter.Domain.PItemEntity() { Type = 2, Price = 0, Description = _ + " ", ItemCode = "", HotelID = 0, ID = 0 }).ToList();
            }
            else
            {
                model.NoticeItems = new List<HJD.HotelManagementCenter.Domain.PItemEntity>();
            }

            //读取酒店照片信息
            HotelPhotosEntity hps = HotelAdapter.GetHotelPhotos(model.package.HotelID, 0);
            int count = hps.HPList.Count();

            //PicPath
            model.activity.PicPath = "";
            if (!string.IsNullOrWhiteSpace(model.activity.PicPath))
            {
                var tempPhotoList = new List<string>();
                foreach (var photoSUrl in model.activity.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    tempPhotoList.Add(PhotoAdapter.GenHotelPicUrl(photoSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.theme));
                }
                model.activity.PicPath = tempPhotoList.First();
                model.package.PicUrls = tempPhotoList;
            }

            //pic url list
            if (count > 0)
            {
                model.package.PicUrls = new List<string>();
                for (int _num = 0; _num < count; _num++)
                {
                    if (_num >= 8) break;
                    var _url = PhotoAdapter.GenHotelPicUrl(hps.HPList[_num].SURL, HJDAPI.Common.Helpers.Enums.AppPhotoSize.theme);
                    model.package.PicUrls.Add(_url);
                }

                if (!string.IsNullOrWhiteSpace(model.activity.PicPath))
                {
                    model.package.PicUrls[0] = model.activity.PicPath;
                }
                else
                {
                    //如果没有 PicPath ，则取酒店图的第一个
                    model.activity.PicPath = model.package.PicUrls.First();
                }
            }

            return model;
        }



        /// <summary>
        /// 为周期卡生成新卡
        /// </summary>
        /// <param name="couponID">生成新周期卡的依据卡ID</param>
        /// <returns>新生成的券ID</returns>
        public static int CreateNewCouponForCycleCoupon(int couponID)
        {
            ExchangeCouponEntity entity = couponSvc.CreateNewCouponForCycleCoupon(couponID);
            //将新生成的卡加入到用户的订单中(redis) 
            OrderHelper.AddOrderToRedis(entity.CouponOrderId);
            return entity.ID;
        }



        /// <summary>
        /// 根据最新的已卖出(加索定的）券数量计算券状态 更新券售卖缓存
        /// </summary>
        /// <param name="activityID"></param>
        public static void UpdateCouponSellState(int activityID)
        {
            BuyCouponCheckNumResult result = ProductCache.CouponSellInfoMem(activityID);
            ProductCache.SetBuyCouponCheckNumActivityState(activityID, result);
            ProductCache.SetCouponSellInfoMem(activityID, result);
        }











        public static bool AcquireLock(int activityId)
        {
            AcquireLockParam param = new AcquireLockParam()
            {
                keyName = "RoomCoupon" + activityId,
                maxHoldTimeSpan = TimeSpan.FromSeconds(10),
                maxTryTimeSpan = TimeSpan.FromSeconds(2),
                retryFrequency = 500
            };
            return DistributedLock.AcquireLock(param, MemCacheHelper.memcached5min);
        }

        public static bool ReleaseLock(int activityId)
        {
            string keyName = "RoomCoupon" + activityId;
            return DistributedLock.ReleaseLock(keyName, MemCacheHelper.memcached5min);
        }

        /// <summary>
        /// 如果添加的随机字符串key失败 则认为memcached有问题
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool AddRandomKey2MemcacheItem()
        {
            try
            {
                var randomStr = new Random().Next(1, 30).ToString();//HJDAPI.Controllers.Common.DescriptionHelper.GenAssignLengthRandomStr(3);
                var finalKey = "RandomKey" + Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds).ToString() + randomStr;
                var isOkay = MemCacheHelper.memcached10min.AddNX(finalKey, 1);
                if (isOkay)
                {
                    MemCacheHelper.memcached10min.Delete(finalKey);
                }
                return isOkay;
            }
            catch
            {
                return false;
            }
        }

        public static Decimal CalculateCouponOrderTotalPrice(SubmitCouponOrderModel scom, string price, int sourceID)
        {
            Decimal totalPrice = 0;
            switch (scom.ActivityType)
            {
                case 200:
                    string priceStr = price;
                    List<TypeAndPrice> typeAndPriceList = new List<TypeAndPrice>();
                    if (!string.IsNullOrWhiteSpace(priceStr) && priceStr.Length > 0)
                    {
                        string[] priceArray = priceStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (priceArray != null && priceArray.Length > 0)
                        {
                            //依次显示 平日 周末 节假日 价格
                            for (int i = 0; i < priceArray.Length; i++)
                            {
                                //i到2则退出
                                if (i == 2)
                                {
                                    break;
                                }
                                typeAndPriceList.Add(new TypeAndPrice() { Price = int.Parse(priceArray[i]), PID = sourceID, Type = i == 0 ? 0 : i == 1 ? 6 : 8 });
                            }
                        }
                    }
                    else
                    {
                        typeAndPriceList = HotelAdapter.GetPackageTypeAndPriceList(new List<int>() { sourceID });
                    }
                    foreach (var productAndNum in scom.OrderItems)
                    {
                        productAndNum.Price = typeAndPriceList.Find(_ => _.Type == productAndNum.Type).Price;//Type等于套餐价格类型的一部分 Type=0和Type=6
                        totalPrice += productAndNum.Price * productAndNum.Number;//单价乘以数量 获得总价
                    }

                    //判断是否为通用券 平日周末价格一致
                    bool isCommonUseCoupon = scom.OrderItems.Count <= 1 ? false : scom.OrderItems[0].Price == scom.OrderItems[1].Price && scom.OrderItems[0].Price != 0 ? true : false;
                    if (isCommonUseCoupon)
                    {
                        scom.OrderItems.ForEach(_ => _.Type = 100);//Exchange Type=100 视为通用券
                    }
                    break;
                default:
                    break;
            }
            return totalPrice;
        }



        /// <summary>
        /// 10min钟缓存 优惠活动列表数据
        /// </summary>
        /// <returns></returns>
        public static RoomCouponActivityListModel GetSpeciallyCheapRoomCouponActivityListModel(int advID = 0, int groupNo = 0, int isVip = 0)
        {
            return LocalCache10Min.GetData<RoomCouponActivityListModel>(string.Format("GetSpeciallyCheapRoomCouponActivityListModel:{0}|{1}|{2}", advID, groupNo, isVip),
                () =>
                {
                    return GenSpeciallyCheapRoomCouponActivityListModel(new List<string> { groupNo.ToString() }, advID, isVip);
                });
        }

        /// <summary>
        /// 【微信小程序专用】查询指定groupNo下的所有房券列表（10min本地缓存）
        /// </summary>
        /// <param name="advID"></param>
        /// <param name="groupNo"></param>
        /// <param name="isVip"></param>
        /// <returns></returns>
        public static RoomCouponActivityListModel GetAllRoomCouponListForWxapp(int advID = 0, string groupNo = "", int isVip = 0)
        {
            return LocalCache100Min.GetData<RoomCouponActivityListModel>(string.Format("GetSpeciallyCheapRoomCouponActivityListModel:{0}|{1}|{2}", advID, groupNo, isVip),
            () =>
            {
                var groupNoList = groupNo.Split(',').ToList();

                return GenSpeciallyCheapRoomCouponActivityListModel(groupNoList, advID, isVip);
            });
        }
        /// <summary>
        /// 【微信小程序专用】查询指定groupNo下的所有城市信息
        /// </summary>
        /// <param name="districtIds"></param>
        public static List<DistrictInfoForWxappEntity> GetRoomCouponDistrictInfoForWxapp(List<int> districtIds, RoomCouponActivityListModel _listModel, int topCount)
        {
            return LocalCache10Min.GetData<List<DistrictInfoForWxappEntity>>(string.Format("GetRoomCouponDistrictInfoForWxapp:{0}", topCount),
            () =>
            {
                var dis = couponSvc.GetRoomCouponDistrictInfoForWxapp(districtIds);
                foreach (var item in dis)
                {
                    item.PicUrl = !string.IsNullOrWhiteSpace(item.PicUrl) ? PhotoAdapter.GenHotelPicUrl(item.PicUrl, Enums.AppPhotoSize.theme).Replace("_theme", "_640x426") : "";
                    //如果当前目的地没有封面图，则读取该目的地下的第一家酒店的封面图，作为该目的地的封面图（临时处理 haoy 2016.12.30）
                    if (string.IsNullOrEmpty(item.PicUrl))
                    {
                        item.PicUrl = _listModel.Items.First(_ => _.DistrictId == item.DistrictId).PicUrl;
                    }
                }
                return dis;
            });
        }
        /// <summary>
        /// 查询所有酒店的城市信息
        /// </summary>
        /// <param name="districtIds">城市id</param>
        /// <param name="listModel">酒店集合</param>
        /// <param name="lat"> 用于缓存key</param>
        /// <param name="lng">用于缓存key</param>
        /// <param name="inChina">用于缓存key</param>
        /// <returns></returns>
        public static List<DistrictInfoForWxappEntity> GetHotelDistrictInfo(List<int> districtIds, List<SimpleHotelEntity> listModel, double lat = 0, double lng = 0, int geoScopeType = 3, bool inChina = true)
        {
            return LocalCache10Min.GetData<List<DistrictInfoForWxappEntity>>(string.Format("GetRoomCouponDistrictInfoForWxapp:{0}_{1}_{2}_{3}", districtIds, inChina, lat, lng),
            () =>
            {
                var dis = couponSvc.GetRoomCouponDistrictInfoForLatLngWxapp(districtIds, lat, lng, geoScopeType, inChina);
                //var dis = couponSvc.GetRoomCouponDistrictInfoForWxapp(districtIds);
                foreach (var item in dis)
                {
                    item.PicUrl = !string.IsNullOrWhiteSpace(item.PicUrl) ? PhotoAdapter.GenHotelPicUrl(item.PicUrl, Enums.AppPhotoSize.theme).Replace("_theme", "_640x426") : "";
                    //如果当前目的地没有封面图，则读取该目的地下的第一家酒店的封面图，作为该目的地的封面图（临时处理 haoy 2016.12.30）
                    if (string.IsNullOrEmpty(item.PicUrl))
                    {
                        try
                        {
                            int hotelid = listModel.First(_ => _.DistrictID == item.DistrictId).HotelId;
                            HotelPhotosEntity hps = HotelAdapter.GetHotelPhotos(hotelid, 0);
                            item.PicUrl = PhotoAdapter.GenHotelPicUrl(hps.HPList[0].SURL, HJDAPI.Common.Helpers.Enums.AppPhotoSize.theme);
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("GetHotelDistrictInfo error 目的地获取酒店照片报错:" + e);
                        }
                    }
                }
                return dis;
            });
        }
        /// <summary>
        /// 显示所有发布状态下的券活动
        /// </summary>
        /// <param name="groupNoList">支持一次查询多个GroupNo下的券列表</param>
        /// <param name="advID"></param>
        /// <param name="onlyVIP"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static RoomCouponActivityListModel GenSpeciallyCheapRoomCouponActivityListModel(List<string> groupNoList, int advID = 0, int onlyVIP = 0, int count = -1)
        {
            if (groupNoList == null) groupNoList = new List<string> { "" };

            //activityID用来获取 bannner图片 groupNo用来设置组号
            HJD.ADServices.Contract.AdvBaseInfoEntity adv = advID != 0 ? ADService.GetAdvBaseInfoByID(advID) : new HJD.ADServices.Contract.AdvBaseInfoEntity();

            //完整的banner背景照片
            string bannerPic = adv.BgImgUrl;
            RoomCouponActivityListModel model = new RoomCouponActivityListModel()
            {
                TopPicUrl = string.IsNullOrWhiteSpace(bannerPic) ? PhotoAdapter.GenHotelPicUrl(topPicUrl, Enums.AppPhotoSize.jupiter) : bannerPic,
                Items = new List<RoomCouponActivityListItem>()
            };

            int totalCount = 0;
            var originActivityList = couponSvc.GetCouponActivityList(new CouponActivityQueryParam()
            {
                stateArray = new int[] { 1 },
                PageSize = 600,
                PageIndex = 1,
                activityTypeArray = new int[] { 100, 200, 300 },
                merchantCode = CouponActivityMerchant.zmjd,
                lastEditTime = new DateTime(2000, 1, 1, 0, 0, 0)
            }, out totalCount);

            List<CouponActivityEntity> activityList = originActivityList.FindAll(_ => _.State == 1 && groupNoList.Contains(_.GroupNo.ToString()) && _.IsValid).OrderBy(_ => _.Rank).ToList();//获得所有发布状态 某一组的房券活动

            if (activityList != null && activityList.Count != 0)
            {
                //只显示VIP专享的套餐
                if (onlyVIP == 1)
                {
                    activityList = activityList.Where(a => a.IsVipExclusive).ToList();
                }

                string regStr = @"_.*$";
                Regex regx = new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                for (int anum = 0; anum < activityList.Count; anum++)
                {
                    if (count < 0 || anum < count)
                    {
                        var item = activityList[anum];

                        CouponActivityDetailModel activityModel = GetCouponActivityDetail(item.ID);//使用房券活动页的缓存
                        List<TypeAndPrice> priceList = activityModel.package.PackagePrice.FindAll(_ => _.Price != 0);
                        model.Items.Add(new RoomCouponActivityListItem()
                        {
                            ActivityID = activityModel.activity.ID,
                            Rank = activityModel.activity.Rank,
                            ActivityOpenState = activityModel.activityOpenState,
                            HotelName = activityModel.package.HotelName,
                            PicUrl = activityModel.activity.PicPath,
                            Price = priceList.Min(_ => _.Price),
                            PriceDateType = priceList[0].TypeName.Replace("券", ""),
                            PriceType = "",//暑期特价
                            LeaveNum = activityModel.activity.TotalNum - activityModel.activity.SellNum,
                            PackageBrief = activityModel.package.PackageBrief,
                            MarketPrice = activityModel.activity.MarketPrice,
                            StartSellTime = activityModel.activity.EffectiveTime,
                            EndSellTime = activityModel.activity.SaleEndDate,
                            DistrictId = activityModel.package.DistrictId,
                            DistrictName = activityModel.package.DistrictName,
                            GroupNo = item.GroupNo

                        });
                    }
                }
            }

            if (model.Items != null && model.Items.Count > 0)
            {
                model.Items = model.Items.Where(i => i.EndSellTime > DateTime.Now).OrderBy(i => i.Rank).ThenBy(i => i.StartSellTime).ToList();
            }


            return model;
        }

        /// <summary>
        /// 获取某个活动类型 券状态
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static List<RoomCouponActivityModel> TransRoomCouponActivity(List<CouponActivityEntity> originActivityList, int activityType)
        {
            List<RoomCouponActivityModel> model = new List<RoomCouponActivityModel>();

            List<CouponActivityEntity> activityList = originActivityList.FindAll(_ => _.State == 1 && _.Type == activityType).OrderBy(_ => _.Rank).ToList();
            if (activityList != null && activityList.Count != 0)
            {
                string regStr = @"_.*$";
                Regex regx = new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                foreach (var item in activityList)
                {
                    CouponActivityDetailModel activityModel = GetCouponActivityDetail(item.ID);//使用房券活动页的缓存
                    List<TypeAndPrice> priceList = activityModel.package.PackagePrice.FindAll(_ => _.Price != 0);
                    model.Add(new RoomCouponActivityModel()
                    {
                        activityId = activityModel.activity.ID,
                        activityState = activityModel.activityOpenState,
                        hotelName = activityModel.package.HotelName,
                        hotelPicUrl = regx.Replace(activityModel.activity.PicPath, "_" + Enums.AppPhotoSize.theme.ToString()),
                        activityPrice = priceList.Min(_ => _.Price),
                        marketPrice = activityModel.activity.MarketPrice,
                        leaveNum = activityModel.activity.TotalNum - activityModel.activity.SellNum,
                        packageBrief = activityModel.package.PackageBrief,
                        activityTypeName = item.Type == 200 ? "闪购" : item.Type == 300 ? "亲子团" : "限时抢购",
                        activityType = item.Type,
                        activityTitle = item.RecommendTitle,
                        totalNum = item.TotalNum,
                        startSellTime = item.EffectiveTime,
                        endSellTime = item.SaleEndDate,
                        labelPics = string.IsNullOrWhiteSpace(item.LabelPicUrl) ? new string[] { } : item.LabelPicUrl.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    });
                }
            }
            return model;
        }

        /// <summary>
        /// 订单使用的房券
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static List<ExchangeCouponEntity> GetUsedCouponByOrderID(long orderID)
        {
            return couponSvc.GetUsedCouponByOrderID(orderID);
        }

        public static CheckOrderBeforePayResponse IsExchangeOrderCanPay(int couponOrderID)
        {
            CheckOrderBeforePayResponse res = new CheckOrderBeforePayResponse();
            res.Success = BaseResponse.ResponseSuccessState.Success;
            res.bCanPay = true;
            res.Message = "订单可以支付！";
            try
            {
                HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity commOrder = OrderAdapter.GetCommOrder(couponOrderID);
                if (commOrder.TypeID == 10000)//判断是否非发票订单
                {

                }
                else
                {
                    //超过十分钟 没有支付 则不能继续支付 返回1
                    //状态不对 不能支付 返回2
                    //参数错误 返回3
                    // 现金券已使用，返回 4
                    //住基金金额不足， 返回 5
                    RoomCouponOrderEntity rcoe = GetOneRoomCouponOrderEntityByPayID(couponOrderID);

                    if (rcoe.CouponOrderID == 0) //补汇款的，不需要检查
                    {

                    }
                    else if (rcoe != null && rcoe.ExchangeCouponList != null && rcoe.ExchangeCouponList.Count > 0)
                    {
                        ExchangeCouponEntity ece = rcoe.ExchangeCouponList.First();
                        if (ece.State != 1)
                        {
                            res.bCanPay = false;
                            res.Success = BaseResponse.ResponseSuccessState.BeforePay_Coupon_OverTime_StateChange;
                            res.Message = "支付时间超时，支付状态已改变！";
                            // return 2;
                        }
                        //其他状态再说
                        else if (ece.ActivityType != 400 && (DateTime.Now - ece.CreateTime).TotalMinutes >= 30)
                        {
                            //更新支付超期的券订单状态为已取消
                            List<CanceledExchangeCouponEntity> canceledCouponOrderIDList = couponSvc.UpdateCouponState4TimeOut(ece.ActivityID);
                            if (canceledCouponOrderIDList.Count > 0)
                            {
                                //更新Redis数据
                                canceledCouponOrderIDList.Select(_ => _.UserID).ToList().ForEach(userID =>
                                {
                                    OrderHelper.DeleteUserCouponOrderRelRedisData(userID, canceledCouponOrderIDList.Select(ec => ec.CouponOrderId).Distinct().ToList());
                                });
                            }

                            res.bCanPay = false;
                            res.Success = BaseResponse.ResponseSuccessState.BeforePay_Coupon_OverTime;
                            res.Message = "支付时间已经超过10分钟！";
                            //  return 1;
                        }

                        try
                        {

                            //购买前预约的产品 需要验证是否还有预约位置
                            if (ece.SKUID > 0)
                            {
                                SKUEntity sku = ProductAdapter.GetSKUEXEntityByID(ece.SKUID);
                                if (sku.BookPosition == 1)
                                {

                                    BookUserDateInfoEntity budi = rcoe.ExchangeCouponList.First().BookUserDateList.FirstOrDefault();
                                    if (budi != null && budi.ID > 0)
                                    {
                                        int excount = rcoe.ExchangeCouponList.Count;
                                        bool iscanbook = ProductAdapter.IsCanBookItem(budi.BookDateId, budi.BookDetailId, excount * sku.StockCount);
                                        if (iscanbook == false)
                                        {
                                            //更新为已取消状态
                                            foreach (ExchangeCouponModel item in rcoe.ExchangeCouponList)
                                            {
                                                if (item.ID > 0)
                                                {
                                                    ExchangeCouponEntity param = new ExchangeCouponEntity();
                                                    param.ID = item.ID;
                                                    param.State = 4;
                                                    param.OperationState = item.OperationState;
                                                    param.ExchangeNo = item.ExchangeNo == null ? "" : item.ExchangeNo;
                                                    param.Updator = 10000;
                                                    UpdateExchangeState(param);
                                                }
                                            }

                                            res.bCanPay = false;
                                            res.Success = BaseResponse.ResponseSuccessState.BeforePay_Coupon_BookOver;
                                            res.Message = "该场次预约名额不足，请重新选择";
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("IsExchangeOrderCanPay ERROR：" + e);
                        }

                        //如果有用券，需要检查券是否可用
                        if (rcoe.CashCouponID > 0)
                        {
                            var check = CheckSelectedCashCouponInfoForOrder(new OrderUserCouponRequestParams
                            {
                                TotalOrderPrice = rcoe.TotalPrice,
                                UserID = rcoe.UserId,
                                SelectedCashCouponID = rcoe.CashCouponID,
                                OrderTypeID = CashCouponOrderSorceType.sku,
                                OrderSourceID = rcoe.SKUID,
                                BuyCount = 1
                            });
                            if (check.Success == false)
                            {
                                res.bCanPay = false;
                                res.Success = BaseResponse.ResponseSuccessState.BeforePay_Coupon_CashCouponError;
                                res.Message = GenCannotPayMsgForUserCoupon(rcoe.CashCouponID);
                            }
                        }

                        //如果有用现金券，需要检查券是否可用
                        if (rcoe.VoucherIDs.Length > 0)
                        {
                            var check = CheckSelectedVoucherInfoForOrder(new OrderVoucherRequestParams
                            {
                                TotalOrderPrice = rcoe.TotalPrice - rcoe.CashCouponAmount,
                                UserID = rcoe.UserId,
                                OrderSourceID = rcoe.SKUID,
                                OrderTypeID = CashCouponOrderSorceType.sku,
                                SelectedVoucherIDs = CommMethods.TranStrIDsToList(rcoe.VoucherIDs),
                                MaxOrderCanUseVoucherAmount = 0,
                                BuyCount = 1
                            });
                            if (check.Success == false)
                            {
                                res.bCanPay = false;
                                res.Success = BaseResponse.ResponseSuccessState.BeforePay_Coupon_VoucherError;
                                res.Message = "抱歉你选择的代金券已失效，请重新下单";
                            }
                        }

                        //如果有往基金，那么需要检查住基金是否足够
                        if (rcoe.UserUseHousingFundAmount > 0)
                        {
                            if (FundAdapter.GetUserFundInfo(rcoe.UserId).TotalFund < rcoe.UserUseHousingFundAmount)
                            {
                                res.bCanPay = false;
                                res.Success = BaseResponse.ResponseSuccessState.BeforePay_Coupon_FundError;
                                res.Message = GenCannotPayMsgForFund();
                            }
                        }

                    }
                    else
                    {
                        res.bCanPay = false;
                        res.Success = BaseResponse.ResponseSuccessState.BeforePay_Coupon_ParamsError;
                        res.Message = "参数错误！";
                    }
                }
            }
            catch (Exception err)
            {
                Log.WriteLog("IsExchangeOrderCanPay:" + err.Message + err.StackTrace);
                res.bCanPay = false;
                res.Success = BaseResponse.ResponseSuccessState.BeforePay_Coupon_Other;
                res.Message = err.Message;
            }
            return res;
        }



        public static string GenCannotPayMsgForUserCoupon(int cashCouponID)
        {
            string Message = "";
            var couponInfo = GetUserCouponItemInfoByID(cashCouponID);
            if (couponInfo.UserCouponType == (int)UserCouponType.DiscountOverPrice)
            {
                Message = string.Format("抱歉你选择的满{0}减{1}现金券已失效，请重新下单", StringHelper.FormatMoney(couponInfo.RequireAmount), StringHelper.FormatMoney(couponInfo.DiscountAmount));
            }
            else
            {
                Message = "立减金已被使用，请重新下单。";// "抱歉你当前的立减金额度低于此订单的抵用额度，请重新下单";

            }
            return Message;
        }

        public static string GenCannotPayMsgForFund()
        {
            return "抱歉你当前的往基金额度低于此订单的抵用额度，请重新下单";
        }

        /// <summary>
        /// 主动更新缓存
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="model"></param>
        public static void SetCacheActivityDetail(int activityID, CouponActivityDetailModel model)
        {
            MemCacheHelper.memcached10min.Set(string.Format(GetCouponActivityDetailCache + ":{0}_{1}", activityID, 1), model);
            MemCacheHelper.memcached10min.Set(string.Format(GetCouponActivityDetailCache + ":{0}_{1}", activityID, 0), model);
        }

        /// <summary>
        /// 获取单张券的信息
        /// </summary>
        /// <param name="couponID"></param>
        /// <param name="exchangeNo"></param>
        /// <returns></returns>
        public static ExchangeCouponEntity GetOneExchangeCouponInfoByCouponID(int couponID)
        {
            return couponSvc.GetOneExchangeCouponInfo(couponID, "");
        }


        /// <summary>
        /// 获取单张券的信息
        /// </summary>
        /// <param name="couponID"></param>
        /// <param name="exchangeNo"></param>
        /// <returns></returns>
        public static ExchangeCouponEntity GetOneExchangeCouponInfoByExchangeNo(string exchangeNo)
        {
            return couponSvc.GetOneExchangeCouponInfo(0, exchangeNo);
        }


        /// <summary>
        /// 获取单张券的信息
        /// </summary>
        /// <param name="couponID"></param>
        /// <param name="exchangeNo"></param>
        /// <returns></returns>
        private static ExchangeCouponEntity GetOneExchangeCouponInfo(int couponID, string exchangeNo)
        {
            return couponSvc.GetOneExchangeCouponInfo(couponID, exchangeNo);
        }

        /// <summary>
        /// 获取用户某个套餐可用的房券列表
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="SourceID"></param>
        /// <returns></returns>
        public static List<ExchangeCouponEntity> GetExchangeCouponListByUserIDSourceID(Int64 UserID, Int64 SourceID, int SKUID)
        {
            return couponSvc.GetExchangeCouponListByUserIDSourceID(UserID, SourceID).Where(_ => _.State == (int)ExchangeCouponState.paied && _.RefundState == (int)CouponRefundState.NoRefund && _.SKUID == SKUID).ToList();

            // return couponSvc.GetExchangeCouponListByUserIDSourceID(UserID, SourceID);
        }

        /// <summary>
        /// 计算用户所选日期的应付价格
        /// </summary>
        /// <param name="activityPriceStr">券的价格字符串</param>
        /// <param name="nightRoomCount">单券可兑换间夜数</param>
        /// <param name="checkIn">兑换订单入住</param>
        /// <param name="checkOut">兑换订单离开</param>
        /// <param name="roomCount">订单每日房间数</param>
        /// <returns></returns>
        public static decimal GenExchangeRoomOrderAmount(string activityPriceStr, int nightRoomCount, DateTime checkIn, DateTime checkOut, int roomCount)
        {
            string[] priceArray = activityPriceStr.Split(",，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);//价格信息
            if (priceArray != null && priceArray.Length != 0)
            {
                int weekdayPrice = int.Parse(priceArray[0]);//平日价
                int weekendPrice = priceArray.Length > 1 ? int.Parse(priceArray[1]) : 0;//周末价格
                decimal sumPrice = 0;

                //假设 单券抵用多间夜的情况
                if (nightRoomCount > 1)
                {
                    decimal weekAveragePrice = weekdayPrice / nightRoomCount;
                    decimal weekendAveragePrice = weekendPrice / nightRoomCount;

                    for (DateTime startDate = checkIn.Date; startDate < checkOut; startDate = startDate.AddDays(1).Date)
                    {
                        int dayOfweek = (int)startDate.DayOfWeek;
                        bool isWeekend = dayOfweek == 5 || dayOfweek == 6;
                        sumPrice += isWeekend ? weekendAveragePrice : weekAveragePrice;

                        if (isWeekend && weekendPrice == 0)
                        {
                            return -1;
                        }
                        else if (!isWeekend && weekdayPrice == 0)
                        {
                            return -2;
                        }
                    }

                    sumPrice = Math.Round(sumPrice * roomCount, 0);
                }
                else if (nightRoomCount == 1)
                {
                    for (DateTime startDate = checkIn.Date; startDate < checkOut; startDate = startDate.AddDays(1).Date)
                    {
                        int dayOfweek = (int)startDate.DayOfWeek;
                        bool isWeekend = dayOfweek == 5 || dayOfweek == 6;
                        sumPrice += isWeekend ? weekendPrice : weekdayPrice;

                        if (isWeekend && weekendPrice == 0)
                        {
                            return -1;
                        }
                        else if (!isWeekend && weekdayPrice == 0)
                        {
                            return -2;
                        }
                    }
                }
                return sumPrice;
            }

            return 0;//没法计算 不知道是否需要补款
        }

        /// <summary>
        /// 通过后台设置的couponrate计算兑换订单总价
        /// datetype = 0 平日 指定周日到周四入住; 周末 指周五到周六入住
        /// </summary>
        /// <returns></returns>
        public static int GenExchangeRoomOrderAmountWithCoupunRate(int couponActivityID, DateTime checkIn, DateTime checkOut, int roomCount = 1, Enums.CustomerType customerType = Enums.CustomerType.general)
        {
            IEnumerable<CouponRateEntity> rateList = GetCouponRateList(couponActivityID);
            int amount = 0;
            if (rateList != null && rateList.Count() != 0)
            {

                if (customerType == Enums.CustomerType.vip || customerType == Enums.CustomerType.vip199 || customerType == Enums.CustomerType.vip199nr ||
                    customerType == Enums.CustomerType.vip3M || customerType == Enums.CustomerType.vip599 || customerType == Enums.CustomerType.vip6M)
                {
                    for (DateTime startDate = checkIn.Date; startDate < checkOut; startDate = startDate.AddDays(1).Date)
                    {
                        CouponRateEntity cre = (from i in rateList where i.DateType == 8 && i.VIPPrice > 0 && i.Date == startDate select i).FirstOrDefault();//指定日期的价格
                        if (cre != null)
                        {
                            amount += cre.VIPPrice;
                        }
                        else
                        {
                            int dayOfweek = (int)startDate.DayOfWeek;
                            cre = (from i in rateList where i.DateType == (dayOfweek == 0 ? 7 : dayOfweek) && i.VIPPrice > 0 select i).FirstOrDefault();//指定周一到周日的价格
                            if (cre != null)
                            {
                                amount += cre.VIPPrice;
                            }
                            else
                            {
                                cre = (from i in rateList where i.DateType == 0 && i.VIPPrice > 0 select i).FirstOrDefault();//指定平日的价格 周日到周四
                                if (cre != null)
                                {
                                    amount += cre.VIPPrice;
                                }
                                else
                                {
                                    return -1;//没有设定平日价格
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (DateTime startDate = checkIn.Date; startDate < checkOut; startDate = startDate.AddDays(1).Date)
                    {
                        CouponRateEntity cre = (from i in rateList where i.DateType == 8 && i.Price > 0 && i.Date == startDate select i).FirstOrDefault();//指定日期的价格
                        if (cre != null)
                        {
                            amount += cre.Price;
                        }
                        else
                        {
                            int dayOfweek = (int)startDate.DayOfWeek;
                            cre = (from i in rateList where i.DateType == (dayOfweek == 0 ? 7 : dayOfweek) && i.Price > 0 select i).FirstOrDefault();//指定周一到周日的价格
                            if (cre != null)
                            {
                                amount += cre.Price;
                            }
                            else
                            {
                                cre = (from i in rateList where i.DateType == 0 && i.Price > 0 select i).FirstOrDefault();//指定平日的价格 周日到周四
                                if (cre != null)
                                {
                                    amount += cre.Price;
                                }
                                else
                                {
                                    return -1;//没有设定平日价格
                                }
                            }
                        }
                    }
                }
                return amount * roomCount;//每日房间价格*每日房间数
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 验证兑换日期可以跨平日周末使用
        /// </summary>
        /// <param name="activityPriceStr"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        public static bool IsAllowExchangeRoomDate(string activityPriceStr, DateTime checkIn, DateTime checkOut)
        {
            //CouponActivityEntity activityEntity = GetOneCouponActivity(couponActivityID, false);
            bool flag = false;
            string[] priceArray = activityPriceStr.Split(",，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);//价格信息
            if (priceArray != null && priceArray.Length != 0)
            {
                int weekdayPrice = int.Parse(priceArray[0]);
                int weekendPrice = priceArray.Length > 1 ? int.Parse(priceArray[1]) : 0;

                bool allowAddMoney = weekdayPrice > 0 && weekendPrice > 0 ? true : false;//只有一种情况可以补差价 其他两种情况不可以

                int dayOfWeek1 = (int)checkIn.DayOfWeek;
                int dayOfWeek2 = (int)checkOut.AddDays(-1).Date.DayOfWeek;

                int nightCount = (int)(checkOut.Date - checkIn.Date).TotalDays;//总天数

                if (nightCount >= 6 && allowAddMoney)
                {
                    flag = true;
                }
                else if (dayOfWeek1 >= 0 && dayOfWeek2 <= 4 && weekdayPrice > 0)
                {
                    flag = true;
                }
                else if (dayOfWeek1 >= 5 && dayOfWeek2 >= 5 && weekendPrice > 0)
                {
                    flag = true;
                }
                else if (((dayOfWeek1 >= 0 && dayOfWeek1 <= 4 && dayOfWeek2 >= 5 && dayOfWeek2 <= 6) || (dayOfWeek2 >= 0 && dayOfWeek2 <= 4 && dayOfWeek1 >= 5 && dayOfWeek1 <= 6)) && allowAddMoney)
                {
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// 标记订单为使用预购券
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static bool MarkCouponPayOrder(long orderid, int SettlePrice, long curUserID = 1)
        {
            long userid = curUserID;
            HJD.HotelPrice.Contract.DataContract.Order.PackageOrderInfo po = PriceService.GetOrderInfo(orderid);


            PriceService.OffilnePayOrder(orderid, SettlePrice, userid.ToString(), "使用预购券", 7);

            //自动返现
            PriceService.RequestOrderRebate(orderid);
            SetOrderHasRebate(orderid, userid);

            //记录操作日志
            OrderService.UpdateOrderState(new HJD.HotelManagementCenter.Domain.OrdedrUpdateParam() { opUserID = userid, id = po.ID, orderState = (int)HJD.HotelManagementCenter.Domain.OrderState.Pay, tag = "" });
            OrderService.SaveOrderModifyLog(new HJD.HotelManagementCenter.Domain.OrderModifyLog()
            {
                OID = po.ID,
                Userid = userid,
                FieldName = "markCouponPay",
                OldValue = "",
                NewValue = "使用预购券",
                Tag = ""
            });
            OrderService.SaveOrderModifyLog(new HJD.HotelManagementCenter.Domain.OrderModifyLog()
            {
                OID = po.ID,
                Userid = userid,
                FieldName = "state",
                OldValue = po.State.ToString(),
                NewValue = ((int)HJD.HotelManagementCenter.Domain.OrderState.Pay).ToString(),
                Tag = ""
            });
            //不发送排房短信
            //OrderService.UpdateOrderSendRowRowSMSStatus(po.ID, false);
            //Log.WriteLog("2.房券不发送排房短信oid:" + po.ID.ToString());
            return true;
        }

        public static bool SetOrderHasRebate(long orderid, long curUserID)
        {
            List<HJD.HotelManagementCenter.Domain.Settlement.RefundReportEntity> rl = SettleService.GetRefundReportData(10, 0, true);
            if (rl.Where(r => r.OrderID == orderid).Count() > 0)
            {
                int refundID = rl.Where(r => r.OrderID == orderid).First().RefundID;
                HJD.HotelManagementCenter.Domain.RefundEntity refund = SettleService.GetRefundByID(refundID);
                refund.RefundTime = DateTime.Now;
                refund.UpdateUserID = curUserID;
                refund.State = 2;
                SettleService.UpdateRefund(refund);
            }
            return true;
        }

        /// <summary>
        /// 获取房券活动设置的日期价格列表
        /// </summary>
        /// <param name="couponActivityID"></param>
        /// <returns></returns>
        public static IEnumerable<CouponRateEntity> GetCouponRateList(int couponActivityID)
        {
            return couponSvc.GetCouponRateEntityList(couponActivityID);
        }

        /// <summary>
        /// 根据后台设置的 券活动日期价格 可兑换日
        /// </summary>
        /// <param name="rateList"></param>
        /// <returns></returns>
        internal static List<DateTime> genCanNotExchangeRoomDates(IEnumerable<CouponRateEntity> rateList)
        {
            if (rateList != null && rateList.Count() != 0)
            {
                //指定日 价格为0的不能兑换
                return (from i in rateList where i.DateType == 8 && i.Price == 0 select i.Date).ToList();
            }
            else
            {
                return new List<DateTime>();
            }
        }

        /// <summary>
        /// 价格列表 房券活动截止日期
        /// </summary>
        /// <param name="rateList"></param>
        /// <param name="expireDate"></param>
        /// <returns></returns>
        internal static List<DateTime> genCanExchangeRoomDates(IEnumerable<CouponRateEntity> rateList, DateTime expireDate)
        {
            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = expireDate.Date;//过期当天不能兑换

            List<DateTime> canExchangeDates = new List<DateTime>();
            if (startDate >= endDate)
            {
                return canExchangeDates;//活动过期 不能兑换
            }

            if (rateList != null && rateList.Count() != 0)
            {
                List<DateTime> canNotExchangeDates = (from i in rateList where i.DateType == 8 && i.Price == 0 select i.Date).ToList();
                List<int> canNotExchangeWeekDay = (from i in rateList where i.DateType != 8 && i.Price == 0 select i.DateType).ToList();//非特殊日期指定的不能用的 如周一到周日

                List<CouponRateEntity> hasPriceRates = (from i in rateList where i.Price != 0 select i).ToList();

                if (hasPriceRates != null && hasPriceRates.Count > 0)
                {
                    for (DateTime i = startDate; i <= endDate; i = i.AddDays(1).Date)
                    {
                        int dayOfWeek = (int)i.DayOfWeek;
                        if (dayOfWeek == 0)
                        {
                            dayOfWeek = 7;
                        }
                        //System.Diagnostics.Debug.WriteLine(i.ToString() + ":" + endDate);
                        //0.不能卖的日期
                        if (canNotExchangeDates != null && canNotExchangeDates.Contains(i))
                        {
                            continue;
                        }
                        //不能兑换的周一到周日
                        else if (canNotExchangeWeekDay != null && canNotExchangeWeekDay.Contains(dayOfWeek))
                        {
                            continue;
                        }
                        else
                        {
                            CouponRateEntity specialDate = hasPriceRates.FindAll(_ => _.DateType == 8 && _.Date == i).FirstOrDefault();
                            CouponRateEntity specialDayOfWeek = hasPriceRates.FindAll(_ => _.DateType == dayOfWeek).FirstOrDefault();
                            CouponRateEntity weekDay = hasPriceRates.FindAll(_ => _.DateType == 0).FirstOrDefault();
                            //1.指定日 //2.指定周一到周日 //3.指定平日
                            if (specialDate != null || specialDayOfWeek != null || weekDay != null)
                            {
                                canExchangeDates.Add(i);
                            }
                        }
                    }
                    return canExchangeDates;
                }
                else
                {
                    return canExchangeDates;//没有设置价格的日期 不能兑换
                }
            }
            else
            {
                return canExchangeDates;//完全没有设置日期价格 也不能兑换
            }
        }

        /// <summary>
        /// 获取要结算的券
        /// </summary>
        /// <returns></returns>
        internal static List<ExchangeCouponEntity> GetNeedSettlementBotaoCouponList()
        {
            return couponSvc.GetNeedSettlementBotaoCouponList();
        }

        /// <summary>
        /// 需要取消会员权限列表
        /// </summary>
        /// <returns></returns>
        internal static List<long> GetNeedCancelMemberUserList()
        {
            return couponSvc.GetNeedCancelMemberUserList();
        }


        public static bool IsVIP199UpGrade2VIP599(int activityID, long userId)
        {
            //如果是199的VIP，可以末过期时升级购买599  
            var bsVIP199UpGrade2VIP599 = false;

            if (activityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599)
            {
                int activityType = 400;
                int cannotBuyDays = 365;
                var exchangeCouponList = couponSvc.GetExchangeCouponEntityListByUser(userId, activityType);
                var curDate = DateTime.Now.Date;

                var hasBaiedVIP199List = exchangeCouponList.Where(_ =>
                   (_.ActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199
                   //|| _.ActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP3M
                   || _.ActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP6M  //VIP6M 也可以升级
                   || _.ActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR
                   )
                && (_.State == (int)ExchangeCouponState.paied || _.State == (int)ExchangeCouponState.exchanged)
                && (_.RefundState == 0)  //末退款
                && (curDate - _.CreateTime).TotalDays <= cannotBuyDays);

                if (hasBaiedVIP199List.Count() == 1)
                {
                    bsVIP199UpGrade2VIP599 = true;
                }

                //  Log.WriteLog(string.Format("IsVIP199UpGrade2VIP599:{0} {1} {2}", userId, exchangeCouponList.Count, hasBaiedVIP199List.Count()));
            }
            return bsVIP199UpGrade2VIP599;
        }

        public static bool IsVIPRenew(int activityID, long userId)
        {
            //如果是199的VIP，可以末过期时升级购买599  
            var bIsRenew = false;

            //    Log.WriteLog(string.Format("IsVIPRenew:{0},{1}", activityID, userId));

            if (activityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599 || activityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR)
            {
                int activityType = 400;
                int cannotBuyDays = 365;
                var exchangeCouponList = couponSvc.GetExchangeCouponEntityListByUser(userId, activityType);
                var curDate = DateTime.Now.Date;

                var hasBaiedVIP199List = exchangeCouponList.Where(_ =>
                   _.ActivityID == activityID
                && (_.State == (int)ExchangeCouponState.paied || _.State == (int)ExchangeCouponState.exchanged)
                && (_.RefundState == 0)  //末退款
                && (curDate.Date <= _.CreateTime.AddYears(1).Date)
                && CheckTimeDateSpan(_.CreateTime.AddYears(1), 30)
                );

                if (hasBaiedVIP199List.Count() == 1)
                {
                    bIsRenew = true;
                }
                //       Log.WriteLog(string.Format("IsVIPRenew:{0},{1}  ", exchangeCouponList.Count(), hasBaiedVIP199List.Count()));


            }
            return bIsRenew;
        }

        /// <summary>
        /// 判断券列表是否需要首单权限
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsCouponListNeedVIPFirstPay(List<ExchangeCouponModel> list)
        {
            bool bIsVIPFirstPayOrder = false;
            var skuIDList = list.Where(_ => _.SKUID > 0).Select(_ => _.SKUID).ToList();
            if (skuIDList.Count > 0)
            {
                bIsVIPFirstPayOrder = ProductAdapter.IsSKUNeedBuyFirstPackagePriviledge(skuIDList);
            }
            if (bIsVIPFirstPayOrder == false)
            {
                var promotionIDList = list.Where(_ => _.PromotionID > 0).Select(_ => _.PromotionID).ToList();
                bIsVIPFirstPayOrder = ProductAdapter.IsPromotionNeedBuyFirstPackagePriviledge(promotionIDList);
            }

            return bIsVIPFirstPayOrder;
        }


        /// <summary>
        /// 判断用户可不可以第二次购买会员资格
        ///   //如果是199的VIP，可以末过期时升级购买599  
        ///   // 如果是VIP6月，那么可以购买
        ///   //如果是VIP  那么提示用户联系客服转成新VIP
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityType"></param>
        /// <param name="cannotBuyDays"></param>
        /// <returns></returns>
        public static string CanBuyMemberCouponSecond(long userId, int activityID, int activityType = 400, int cannotBuyDays = 365)
        {

            string Message = ""; // "会员退费后一年内不能再次参与购买会员活动！";



            var exchangeCouponList = couponSvc.GetExchangeCouponEntityListByUser(userId, activityType);
            var curDate = DateTime.Now.Date;

            if (exchangeCouponList.Count == 0)
            {
                Message = ""; //没有购买过 可以购买
            }
            else
            {
                //购买过， 但不是VIP6M，不能够买
                //退款中的VIP可以赎买新VIP
                var usingCouponList = exchangeCouponList.Where(_ => _.ActivityType == 400 && (_.State == (int)ExchangeCouponState.paied || _.State == (int)ExchangeCouponState.exchanged) &&
                    ((curDate - _.CreateTime).TotalDays <= cannotBuyDays
                    || _.ActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_EVER)
                    ).OrderByDescending(_ => _.ID);
                if (usingCouponList.Count() > 0)
                {
                    int RefundState = usingCouponList.First().RefundState;
                    if (RefundState == 0)  //退款中的可以购卖
                    {
                        int boughtActivityID = usingCouponList.First().ActivityID;
                        if (boughtActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199 && activityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR)
                        {
                            Message = "你已是金牌会员，如果想换成新金牌会员，请电话联系客服：4000-021-702";
                        }
                        else if (
                           !(
                           boughtActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP6M    // 6M可以随便买
                           || ((boughtActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_EVER
                                   || boughtActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199
                                   || boughtActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_NR)
                               && activityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP599)  //可以升级
                           )
                           )
                        {
                            if (CheckTimeDateSpan(usingCouponList.First().CreateTime.AddYears(1), 30) == false) //30天内可续费
                            {
                                Message = "你已是会员，一年内无需再次购买会员！";
                            }
                        }
                        else if (boughtActivityID == (int)HJDAPI.Controllers.EnumHelper.ActivityID.VIP199_EVER)
                        {
                            Message = "你是永久会员，无需再次购买会员！";
                        }
                    }
                }


                if (Message.Length > 0)
                {
                    Log.WriteLog(string.Format("CanBuyMemberCouponSecond:{0} {1} {2}  ", userId, activityID, Message));

                }
            }

            return Message;
        }

        /// <summary>
        /// 判断日期是不是在30天内到期
        /// </summary>
        /// <param name="compareDateTime"></param>
        /// <param name="dateSpan"></param>
        /// <returns></returns>
        public static bool CheckTimeDateSpan(DateTime compareDateTime, int dateSpan)
        {
            //计算VIP剩余天数
            TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts2 = new TimeSpan(compareDateTime.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();

            return ts.Days <= dateSpan;

        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static void SetCache(string key, string val)
        {
            MemCacheHelper.memcached10min.Set(key, val);//设置值
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCache(string key)
        {
            var obj = MemCacheHelper.memcached10min.Get(key);
            return obj == null ? "" : obj.ToString();
        }

        //public static CouponInfoEntity GetCouponInfoByExchangeNo(string exchangeNO)
        //{
        //    return couponSvc.GetCouponInfoByExchangeNo(exchangeNO);
        //}

        public static int UpdateExchangeState(ExchangeCouponEntity param)
        {
            int result = couponSvc.UpdateExchangeState(param);
            //更新redis缓存
            if (result > 0)
            {
                ProductCache.RemoveUserDetailOrderCache(param.ID);
            }
            return result;
        }

        public static int AddUsedConsumerCouponInfo(UsedConsumerCouponInfoEntity param)
        {
            return couponSvc.AddUsedConsumerCouponInfo(param);
        }
        public static List<UsedCouponProductEntity> GetUsedCouponProductBySupplierId(int supplierId, DateTime startTime, DateTime endTime, int start = 0, int count = 20)
        {
            return couponSvc.GetUsedCouponProductBySupplierId(supplierId, startTime, endTime, start, count);
        }
        public static List<UsedCouponProductEntity> GetUsedCouponProductByOperatorId(int bopId, DateTime startTime, DateTime endTime, int start = 0, int count = 20)
        {
            return couponSvc.GetUsedCouponProductByOperatorId(bopId, startTime, endTime, start, count);
        }

        //public static List<

        public static List<BookNoUsedExchangeCouponInfoEntity> GetBookNoUsedExchangeCouponBySupplierId(int supplierId, DateTime startTime, DateTime endTime, int state = 0)
        {
            return couponSvc.GetBookNoUsedExchangeCouponBySupplierId(supplierId, startTime, endTime, state);
        }

        public static List<ExchangeCouponEntity> GetExchangeCouponEntityListByPhone(string phone, int activityType = 0)
        {
            return couponSvc.GetExchangeCouponEntityListByPhone(phone, activityType);
        }
        public static List<ExchangeCouponEntity> GetExchangeCouponEntityListByGroupId(int groupid, int activityType = 0)
        {
            return couponSvc.GetExchangeCouponEntityListByGroupId(groupid, activityType);
        }

        /// <summary>
        ///  写入RefundCoupon ，price = -1 表示全退
        /// </summary>
        /// <param name="rce"></param>
        /// <returns></returns>
        public static int AddRefundCoupon(RefundCouponsEntity rce)
        {
            CommAdapter.AddBizOpLog(new HJD.HotelManagementCenter.Domain.BizOpLogEntity
            {
                BizID = rce.CouponID,
                BizType = HJD.HotelManagementCenter.Domain.OpLogBizType.ExchangeCouponOrder,
                OpContent = "提交退款申请：" + rce.Remark,
                OperatorUserID = rce.Creator
            });
            return couponSvc.AddRefundCoupon(rce);
        }


        public static UsedConsumerCouponInfoEntity GetUsedCouponProductByExchangeNo(string exchangeNo)
        {
            return couponSvc.GetUsedCouponProductByExchangeNo(exchangeNo);
        }

        public static int GetUserCouponByCategoryParentId(long userid, int parentId)
        {
            return couponSvc.GetUserCouponByCategoryParentId(userid, parentId);
        }


        internal static List<long> CancelUnPayExchangeCouponOrderByActivityIDAndUserID(long userID, int activityID, int SKUID)
        {
            return couponSvc.CancelUnPayExchangeCouponOrderByActivityIDAndUserID(userID, activityID, SKUID);
        }

        internal static bool PresentUserCoupon(PromotionOrderInfo order, int ruleID, int userCouponDefineID)
        {
            bool result = true;

            SendUserCouponItem(new List<long> { order.UserID }, userCouponDefineID, order.UserID);

            return result;
        }

        internal static bool PromotionPresent(PromotionOrderInfo order, int ruleID, int activityID, int skuID = -1)
        {
            bool result = true;
            try
            {
                CouponActivityEntity activity = ProductCache.GetOneCouponActivity(activityID);
                //提交订单ID
                int payOrderId = order.PayID;
                if (payOrderId == 0)
                {
                    HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity coe = new HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity()
                    {
                        CustomID = activityID,
                        IDX = 0,
                        Name = string.Format("{0}({1})", activity.PageTitle, "赠送"),
                        OpNotice = null,
                        PhoneNum = order.PhoneNum,
                        Price = 0,
                        State = (int)SettleState.done,
                        TypeID = activity.Type
                    };

                    payOrderId = OrderAdapter.AddCommOrders(coe);//新建支付订单记录  
                }

                //插入券记录
                if (payOrderId > 0)
                {
                    var item = new ExchangeCouponEntity()
                    {
                        PayID = payOrderId,
                        UserID = order.UserID,
                        PhoneNum = order.PhoneNum,
                        ActivityID = activity.ID,
                        ActivityType = activity.Type,
                        ExchangeNo = null,
                        Price = 0,
                        Type = activity.Type,
                        State = (int)ExchangeCouponState.submit,
                        CID = order.CID,
                        PromotionID = ruleID,
                        CustomerType = order.CustomerType,
                        InnerBuyGroup = order.InnerBuyGroup,
                        SKUID = skuID > 0 ? skuID : order.SKUID,
                        CouponOrderId = order.CouponOrderId
                    };

                    if (item.SKUID > 0 && item.settleType != 400)//会员结算价为0
                    {
                        item.SettlePrice = productSvc.GetSKUByID(item.SKUID).PayForSupplier;
                    }

                    item.ID = InsertExchangeCoupon(item);

                    bool ishavecoupon = true;
                    //有供应商，则用第三方的券码
                    if (activity.SupplierID > 0)
                    {
                        ishavecoupon = CouponAdapter.GetTopSupplierCouponBySupplierID(activity.SupplierID, new List<ExchangeCouponEntity> { item });
                        //Log.WriteLog("rightCouponList：" + ishavecoupon + " SupplierID:" + coe.SupplierID);
                    }
                    else
                    {
                        //随机生成字符串,同时检测是否存在相同的券号。如果存在则替换,不存在则使用
                        GenIntCouponRandomNo(6, new List<ExchangeCouponEntity> { item });
                    }

                    //GenIntCouponRandomNo(6, new List<ExchangeCouponEntity> { item });
                    item.State = (int)ExchangeCouponState.paied;
                    UpdateExchangeCoupon(item);

                    //发短信
                    switch (item.ActivityType)
                    {
                        case 600: //消费券
                            var activityEntity = ProductCache.GetOneCouponActivity(activityID);
                            //如果是成团数量大于0 则认为是团购，不发短信
                            if (activityEntity.GroupCount > 0)
                            { }
                            else
                            {
                                var sms600 = string.Format(buyCouponSuccessTemplate600ForPresent, activityEntity.PageTitle, 1, item.ExchangeNo.Trim());//插入sqlserver的数据记录 自带尾空格 使用时除去
                                try
                                {
                                    SMServiceController.SendSMS(item.PhoneNum, sms600);
                                }
                                catch (Exception ex)
                                {
                                    Log.WriteLog("短信发送异常" + ex.Message);
                                }
                            }
                            break;
                        case 200: //房券
                            var couponEntity = CouponAdapter.GetOneRoomCouponOrderEntityByPayID(payOrderId);
                            //如果是成团数量大于0 则认为是团购，不发短信
                            if (couponEntity.GroupId > 0)
                            { }
                            else
                            {
                                var sms = string.Format(CouponAdapter.buyCouponSuccessTemplate200ForPresent, couponEntity.HotelName, couponEntity.PackageName, 1, item.ExchangeNo.Trim());//插入sqlserver的数据记录 自带尾空格 使用时除去

                                try
                                {
                                    SMServiceController.SendSMS(item.PhoneNum, sms);
                                }
                                catch (Exception ex)
                                {
                                    Log.WriteLog("短信发送异常" + ex.Message);
                                }
                            }
                            break;
                    }

                }
            }
            catch (Exception err)
            {
                Log.WriteLog(string.Format("PromotionPresent:userID:{0},ruleID:{1} err:{2}", order.UserID, ruleID, err.Message + err.StackTrace));
                result = false;
            }

            return result;
        }

        internal static int AddRedShare(RedShareEntity param)
        {
            return couponSvc.AddRedShare(param);
        }

        internal static List<RedShareEntity> GetRedShareEntityByGUID(string guid)
        {
            return couponSvc.GetRedShareEntityByGUID(guid);
        }

        internal static bool GetProductAlbumSKUBySKUIDAndAlbumId(int skuid, int albumid)
        {
            return couponSvc.GetProductAlbumSKUBySKUIDAndAlbumId(skuid, albumid);
        }

        internal static List<CouponActivityRetailEntity> GetRetailProductList(int pageSize, int start, out int totalCount)
        {
            //int state_online = 1;
            //var list = couponSvc.GetRetailProductList(0, 0, state_online, pageSize, start, out totalCount);
            var list = couponSvc.GetSKURetailProductList(pageSize, start, out totalCount);

            foreach (var item in list)
            {
                List<SKUCouponActivityEntity> skuCouponActivityList = CouponAdapter.GetSKUListByActivityId(item.CouponActivityID).OrderBy(_ => _.SKUVipPrice).ToList();
                item.Price = new List<decimal>();
                item.SKUCommissions = new List<decimal>();
                item.SKUIDS = new List<int>();
                foreach (var skuprice in skuCouponActivityList)
                {
                    item.Price.Add(skuprice.SKUVipPrice);
                    item.SKUCommissions.Add(skuprice.SKUCommissions);
                    item.SKUIDS.Add(skuprice.SKUID);

                }
                //item.Price = skuCouponActivityList.Select(_ => _.SKUVipPrice).ToList();
                //item.SKUCommissions = skuCouponActivityList.Select(_ => _.SKUCommissions).ToList();

                item.OnePicUrl = GetFirstPicUrl(item.PicPath, Enums.AppPhotoSize.small);

                item.TotalNum -= item.ManuSellNum;
            }

            return list;
        }

        private static string GetFirstPicUrl(string strPicList, Enums.AppPhotoSize psize)
        {
            if (strPicList.Length > 0)
            {
                string surl = strPicList.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First();
                return PhotoAdapter.GenHotelPicUrl(surl, psize);
            }
            else
                return "";
        }

        internal static List<SKUCouponActivityEntity> GetSKUListByActivityId(int couponActivityID)
        {
            return couponSvc.GetSKUListByActivityId(couponActivityID);
        }

        internal static CouponActivityRetailDetailEntity GetRetailProductByID(int ID, long CID)
        {
            try
            {
                var item = couponSvc.GetRetailProductInfoByIDAndCID(ID, CID);
                item.OnePicUrl = GetFirstPicUrl(item.PicPath, Enums.AppPhotoSize.small);

                List<SKUCouponActivityEntity> skuList = CouponAdapter.GetSKUListByActivityId(item.CouponActivityID);
                item.SKUList = new List<SKUInfo>();
                foreach (SKUCouponActivityEntity sa in skuList)
                {
                    SKUInfo sku = new SKUInfo();
                    sku.SKUID = sa.SKUID;
                    sku.Name = sa.SKUName;
                    sku.Price = sa.SKUVipPrice;
                    sku.SortNo = sa.SKUSortNo;
                    sku.Commission = sa.SKUCommissions;
                    item.SKUList.Add(sku);
                }
                item.SKUList = item.SKUList.OrderBy(_ => _.SortNo).ToList();
                //item.SKUList = CouponAdapter.GetSKUListByActivityId(item.CouponActivityID).Select(_ => new SKUInfo { SKUID = _.SKUID, Name = _.SKUName, Price = _.SKUVipPrice, SortNo = _.SKUSortNo, Commission = (decimal)((_.SKUVipPrice - _.SKUVipPrice * 0.01 - _.PayForSupplier) / 2) }).OrderBy(_ => _.SortNo).ToList();

                if (item.SKUList.Count > 0)
                {
                    string ResourceUrl = item.MoreDetailUrl == null ? "" : item.MoreDetailUrl + "&CID=" + CID.ToString();

                    string ProductUrl = string.Format(item.IsGroupProduct ? Configs.GroupProductSaleUrl : Configs.ProductSaleUrl, item.SKUList.Where(_ => _.SKUID == ID).First().SKUID, CID);


                    if (item.ResourceUrl != ResourceUrl || item.ProductUrl != ProductUrl) //如果没有短链生成，那么需要生成短链
                    {
                        if (item.ProductUrl != ProductUrl)
                        {
                            item.ProductUrl = ProductUrl;

                            item.ShortProductUrl = CommMethods.GenShortenUrl(item.ProductUrl);
                        }

                        if (ResourceUrl.Length == 0)
                        {
                            item.ResourceUrl = "";
                            item.ShortResourceUrl = "";
                        }
                        else if (item.ResourceUrl != ResourceUrl)
                        {
                            item.ResourceUrl = ResourceUrl;
                            item.ShortResourceUrl = CommMethods.GenShortenUrl(item.ResourceUrl);
                        }

                        couponSvc.AddRetailUrl(new RetailUrlEntity
                        {
                            CID = CID,
                            CreateTime = DateTime.Now,
                            ProductUrl = item.ProductUrl,
                            ResourceURL = item.ResourceUrl,
                            //RetailID = ID,
                            ShortProductUrl = item.ShortProductUrl,
                            ShortResourceURL = item.ShortResourceUrl,
                            SKUID = ID,
                            PID = 0
                        });

                    }
                }

                item.TotalNum -= item.ManuSellNum;

                return item;
            }
            catch (Exception err)
            {
                Log.WriteLog("GetRetailProductByID: Error:" + err.Message + err.StackTrace);

                return new CouponActivityRetailDetailEntity();
            }
        }

        internal static List<ChannelMOrderEntity> GetChannelMOrderList(int cid, int count, int start, out int totalCount)
        {
            var list = SettleService.GetChannelMOrderList(cid, start + 1, start + count, out totalCount);
            var hotellist = list.Where(a => a.Type == 1).Select(a => a.HotelID);
            var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotellist);
            foreach (var item in list)
            {
                if (item.Type == 1)
                {
                    item.PicPath = (hotelPicsList.FirstOrDefault(p => p.HotelId == item.HotelID) != null && hotelPicsList.First(p => p.HotelId == item.HotelID).HPList.Any()) ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == item.HotelID).HPList[0].SURL, Enums.AppPhotoSize.theme) : "";
                }
                else
                {
                    item.PicPath = item.Type == 3 ? "http://p1.zmjiudian.com/117EUIX0dG_small" : GetFirstPicUrl(item.PicPath, Enums.AppPhotoSize.small);
                }
                item.StateName = (item.OrderState == 1 ? "待消费" : item.OrderState == 2 ? "已完成" : item.OrderState == 3 ? "已退单" : "");
                item.TypeName = (item.Type == 1 ? "订单" : item.Type == 2 ? "消费券" : item.Type == 3 ? "VIP购买" : "房券");
                item.SKUName = item.Type == 3 ? item.ActivityName : item.SKUName;
                item.ActivityName = item.Type == 3 ? "周末酒店VIP会员计划" : item.ActivityName;
            }
            return list;
        }
        internal static List<ChannelMOrderEntity> GetChannelMOrderListByCidAndState(int cid, int count, int start, int state, DateTime starttime, DateTime endtime, out int totalCount)
        {
            var list = SettleService.GetChannelMOrderListByCidAndState(cid, start + 1, start + count, state, starttime, endtime, out totalCount);
            var hotellist = list.Where(a => a.Type == 1).Select(a => a.HotelID);
            var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotellist);
            foreach (var item in list)
            {
                if (item.Type == 1)
                {
                    item.PicPath = (hotelPicsList.FirstOrDefault(p => p.HotelId == item.HotelID) != null && hotelPicsList.First(p => p.HotelId == item.HotelID).HPList.Any()) ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == item.HotelID).HPList[0].SURL, Enums.AppPhotoSize.theme) : "";
                }
                else
                {
                    item.PicPath = item.Type == 3 ? "http://p1.zmjiudian.com/117EUIX0dG_small" : GetFirstPicUrl(item.PicPath, Enums.AppPhotoSize.small);
                }
                item.StateName = (item.OrderState == 1 ? "待消费" : item.OrderState == 2 ? "已完成" : item.OrderState == 3 ? "已退单" : "");
                item.TypeName = (item.Type == 1 ? "订单" : item.Type == 2 ? "消费券" : item.Type == 3 ? "VIP购买" : "房券");
                item.SKUName = item.Type == 3 ? item.ActivityName : item.SKUName;
                item.ActivityName = item.Type == 3 ? "周末酒店VIP会员计划" : item.ActivityName;
            }
            return list;
        }
        internal static ChannelMOrderDetailEntity GetChannelMOrderDetail(int id, int cid)
        {
            var model = SettleService.GetChannelMOrderDetail(id, cid);
            var hotellist = new List<int>() { model.HotelID };
            var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotellist);
            if (model.Type == 1)
            {
                model.PicPath = (hotelPicsList.FirstOrDefault(p => p.HotelId == model.HotelID) != null && hotelPicsList.First(p => p.HotelId == model.HotelID).HPList.Any()) ? PhotoAdapter.GenHotelPicUrl(hotelPicsList.First(p => p.HotelId == model.HotelID).HPList[0].SURL, Enums.AppPhotoSize.theme) : "";
            }
            else
            {
                model.PicPath = (model.Type == 3 ? "http://p1.zmjiudian.com/117EUIX0dG_small" : GetFirstPicUrl(model.PicPath, Enums.AppPhotoSize.small));
            }
            model.StateName = (model.OrderState == 1 ? "待消费" : model.OrderState == 2 ? "已完成" : model.OrderState == 3 ? "已退单" : "");
            model.ReturnPolicyName = (model.ReturnPolicy == 1 ? "过期自动退" : model.ReturnPolicy == 2 ? "随时退" : model.ReturnPolicy == 3 ? "过期不可退" : "");
            model.TypeName = (model.Type == 1 ? "订单" : model.Type == 2 ? "消费券" : model.Type == 3 ? "VIP购买" : "房券");
            model.SKUName = model.Type == 3 ? model.ActivityName : model.SKUName;
            model.ActivityName = model.Type == 3 ? "周末酒店VIP会员计划" : model.ActivityName;
            return model;
        }

        internal static List<SettlementForChannelEntity> GetTeamLeaderRewardByCid(long cid)
        {
            return SettleService.GetTeamLeaderRewardByCid(cid);
        }

        internal static List<SettlementForChannelEntity> GetTeamMemberRetailOrder(string orderIds)
        {
            return SettleService.GetTeamMemberRetailOrder(orderIds);
        }

        internal static List<RetailerInvateEntity> GetRetailerInvateByRefereesUserId(long cid)
        {
            return retailerService.GetRetailerInvateByRefereesUserId(cid);
        }

        internal static List<RetailOrderStatistics> GetTeamRetailOrderStatistics(long cid)
        {
            return SettleService.GetTeamRetailOrderStatistics(cid);
        }

        internal static List<RetailOrderStatistics> GetRetailRankings(int start, int count, DateTime startTime)
        {
            return SettleService.GetRetailRankings(start, count, startTime);
        }

        #region 我的模块
        internal static List<SettleBatchWithDrawEntity> GetChannelMWithDrawList(int cid, int count, int start, out int totalCount)
        {
            List<SettleBatchWithDrawEntity> list = SettleService.GetChannelSettleBatchList(long.Parse(cid.ToString()), DateTime.Parse("2000-01-01"), DateTime.Now, start + 1, start + count, out totalCount);
            return list.Where(a => a.State > 1).ToList<SettleBatchWithDrawEntity>();
        }

        internal static List<ChannelMOrderEntity> GetChannelMInAccountList(int cid, int count, int start, out int totalCount)
        {
            List<ChannelMOrderEntity> list = SettleService.GetChannelMInAccountList(cid, start + 1, start + count, out totalCount);
            foreach (var item in list)
            {
                item.StateName = item.OrderState == 1 ? "待提现" : item.OrderState == 2 ? "已提现" : "";
                item.ActivityName = item.Type == 3 ? "周末酒店VIP会员计划" : item.ActivityName;
            }
            return list;
        }

        internal static List<AccountRecordEntity> GetChannelAccountRecord(int cid, int start, int count, int state, out int totalCount)
        {
            List<AccountRecordEntity> list = SettleService.GetChannelAccountRecord(cid, start, count, state, out totalCount);
            foreach (var item in list)
            {
                item.StateName = (item.State == 0) ? "待入账" : item.State == 1 ? "待提现" : item.State == 2 ? "提现中" : item.State == 3 ? "已提现" : "";
            }
            return list;
        }

        internal static ChannelMineModel GetChannelMMine(int cid, int aid)
        {
            ChannelMineModel model = new ChannelMineModel();
            decimal canrequiremoney = SettleService.GetCanRequire(cid.ToString());
            decimal notsettlemonty = SettleService.GetChannelMInAccountSumPrice(cid);
            PayBankAccountLibEntity modelbank = SettleService.GetPayBankAccout(cid, GetCIDType(cid));


            List<AccountRecordEntity> listDetail = SettleService.GetChannelMInAccount(cid);
            //model.WaitAmount = listDetail.Where(_ => _.SettleState == 1 || _.SettleState == 2).Sum(_ => _.Reward);
            //model.NotPutAmount = listDetail.Where(_ => _.SettleState == 4).Sum(_ => _.Reward);
            //model.PutingAmount = listDetail.Where(_ => _.SettleState == 5).Sum(_ => _.Reward);
            //model.AlreadyAmounted = listDetail.Where(_ => _.SettleState == 7).Sum(_ => _.Reward);

            model.WaitAmount = listDetail.Where(_ => _.State == 0).Sum(_ => _.Reward);
            model.NotPutAmount = listDetail.Where(_ => _.State == 1).Sum(_ => _.Reward);
            model.PutingAmount = listDetail.Where(_ => _.State == 2).Sum(_ => _.Reward);
            model.AlreadyAmounted = listDetail.Where(_ => _.State == 3).Sum(_ => _.Reward);

            List<AccountRecordEntity> nowDaylist = listDetail.Where(_ => _.OrderCreateTime.ToShortDateString() == DateTime.Now.ToShortDateString()).ToList();

            if (nowDaylist.Count > 0)
            {
                model.NowDayOrderPrice = nowDaylist.Sum(_ => _.OrderPrice);
                model.NowDayReward = nowDaylist.Sum(_ => _.Reward);
            }
            string nickname = string.Empty;
            //if (cid == aid)//cid=aid  查询用户信息
            //{
            //    nickname=AccountAdapter.GetUserInfo(aid).Mobile
            //}
            model.NickName = nickname;
            if (canrequiremoney != null && canrequiremoney > 0)
            {
                model.CanRequireAmount = canrequiremoney;
            }
            else
            {
                model.CanRequireAmount = 0;
            }
            if (notsettlemonty != null && notsettlemonty > 0)
            {
                model.NotRequireAmount = notsettlemonty;
            }
            else
            {
                model.NotRequireAmount = 0;
            }
            if (modelbank != null)
            {
                model.CurBankAccount = modelbank.PayType == 1 ? "支付宝" : "个人账户";
            }
            else
            {
                model.CurBankAccount = "";
            }
            return model;
        }

        internal static List<PayBankAccountLibEntity> GetChannelBankAccountList(int cid)
        {
            List<PayBankAccountLibEntity> list = new List<PayBankAccountLibEntity>();
            PayBankAccountLibEntity modelapay = SettleService.GetPayBankAccoutByIDandType(cid, 1, GetCIDType(cid));//支付宝
            if (modelapay != null)
            {
                list.Add(modelapay);
            }
            else
            {
                list.Add(new PayBankAccountLibEntity() { PayType = 1, BankName = "支付宝" });
            }
            PayBankAccountLibEntity modelpbank = SettleService.GetPayBankAccoutByIDandType(cid, 3, GetCIDType(cid));//个人账户
            if (modelpbank != null)
            {
                list.Add(modelpbank);
            }
            else
            {
                list.Add(new PayBankAccountLibEntity() { PayType = 3, BankName = "个人账户" });
            }
            return list;
        }

        internal static PayBankAccountLibEntity GetChannelBankAccount(int cid, int type)
        {
            PayBankAccountLibEntity modelpay = SettleService.GetPayBankAccoutByIDandType(cid, type, GetCIDType(cid));
            if (modelpay != null)
            {
                return modelpay;
            }
            else
            {
                modelpay = new PayBankAccountLibEntity() { PayType = type };
            }
            return modelpay;
        }

        internal static OperationResult SaveBankAccount(PayBankAccountLibEntity model)
        {
            OperationResult result = new OperationResult();
            PayBankAccountLibEntity entity = new PayBankAccountLibEntity();
            entity = model;
            entity.ObjID = model.ObjID;
            entity.ObjType = GetCIDType(int.Parse(model.ObjID.ToString()));
            entity.CreateTime = DateTime.Now;
            entity.Creator = model.Creator == 0 ? model.ObjID : model.Creator;
            entity.IsSettleForChannel = true;
            entity.State = 1;
            int res = 0;
            res = SettleService.AddPayBankAccountLib(entity);//添加记录
            if (res > 0)
            {
                result.Success = true;
                result.Message = "保存成功";
            }
            else
            {
                result.Success = false;
                result.Message = "保存失败";
            }
            return result;
        }
        internal static OperationResult GetCanRequire(int cid)
        {
            OperationResult result = new OperationResult();
            decimal canrequiremoney = SettleService.GetCanRequire(cid.ToString());
            result.Success = true;
            if (canrequiremoney == null || canrequiremoney <= 0)
            {
                canrequiremoney = 0;
            }
            result.Data = canrequiremoney.ToString();
            return result;
        }
        internal static OperationResult WithDrawBankAccount(int id, int cid)
        {
            OperationResult result = new OperationResult();
            int res = SettleService.WithDrawPayBankAccountLib(id, cid, GetCIDType(cid));//更改为当前体现方式
            if (res > 0)
            {
                List<SettlementBatchItemChannelEntity> listdraw = SettleService.GetDrawBatchList(cid.ToString());//取得要提现的batchitem
                int drawstate = 0;
                foreach (var item in listdraw)
                {
                    drawstate += SettleService.ChangeChannelSettleBatchState(cid, GetCIDType(cid), item.IDX, 2);
                }
                if (drawstate > 0)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "暂无可提现金额";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "暂未绑定提现方式";
            }
            return result;
        }

        #endregion

        private static int GetCIDType(int cid)
        {
            return cid > 1000000 ? 2 : 1;
        }

        public static ShopSearchItemEntity GetSearchRetailItemList(int type)
        {
            //分享出去不想让客人看到佣金排序  20180511 by stella
            ShopSearchItemEntity searchItem = new ShopSearchItemEntity();
            List<ShopSearchEntity> searchSortList1 = new List<ShopSearchEntity>() { new ShopSearchEntity() { Name = "默认排序", Values =  0 },
                new ShopSearchEntity() { Name = "销量排序", Values = 1 },
                new ShopSearchEntity() { Name = "价格升序（从低到高）", Values = 2 },new ShopSearchEntity() { Name = "价格降序（从高到低）", Values = 3 }
                //,
                //new ShopSearchEntity() { Name = "佣金升序（从低到高）", Values = 4 },new ShopSearchEntity() { Name = "佣金降序（从高到低）", Values = 5 }
            };
            List<ShopSearchEntity> searchSortList2 = new List<ShopSearchEntity>() { new ShopSearchEntity() { Name = "默认排序", Values =  0 },
                new ShopSearchEntity() { Name = "价格升序（从低到高）", Values = 1 },new ShopSearchEntity() { Name = "价格降序（从高到低）", Values = 2 }
                //,
                //new ShopSearchEntity() { Name = "佣金升序（从低到高）", Values = 3 },new ShopSearchEntity() { Name = "佣金降序（从高到低）", Values = 4 }
            };

            List<ShopSearchEntity> searchScreenList = new List<ShopSearchEntity>() { new ShopSearchEntity() { Name = "酒店", Values = (int)EnumHelper.RetailProductType.HotelPackage,Type=1 },
                new ShopSearchEntity() { Name = "美食", Values = (int)EnumHelper.RetailProductType.FoodCoupon,Type=2 } ,new ShopSearchEntity() { Name = "玩乐", Values = (int)EnumHelper.RetailProductType.PlayCoupon,Type=2 },
                new ShopSearchEntity() { Name = "房券", Values = (int)EnumHelper.RetailProductType.RoomCoupon,Type=2 } ,new ShopSearchEntity() { Name = "机酒", Values = (int)EnumHelper.RetailProductType.JiJiuPackage,Type=1 },
                new ShopSearchEntity() { Name = "邮轮", Values = (int)EnumHelper.RetailProductType.YouLunPackage,Type=1 }};
            searchItem.SearScreen = searchScreenList;

            if (type == 1)
            {
                searchItem.SearchSort = searchSortList2;
            }
            else
            {
                searchItem.SearchSort = searchSortList1;
            }

            return searchItem;
        }

        public static List<ShopSearchEntity> GetSearchRetailScreenList()
        {
            List<ShopSearchEntity> searchScreenList = new List<ShopSearchEntity>() {new ShopSearchEntity() { Name = "玩乐", Values = (int)EnumHelper.RetailProductType.PlayCoupon,Type=2 },
                new ShopSearchEntity() { Name = "酒店", Values = (int)EnumHelper.RetailProductType.RoomCoupon,Type=2 } ,
                new ShopSearchEntity() { Name = "美食", Values = (int)EnumHelper.RetailProductType.FoodCoupon,Type=2 }
                //,
                //new ShopSearchEntity() { Name = "酒店", Values = (int)EnumHelper.RetailProductType.HotelPackage,Type=1 },
                //new ShopSearchEntity() { Name = "机酒", Values = (int)EnumHelper.RetailProductType.JiJiuPackage,Type=1 },
                //new ShopSearchEntity() { Name = "邮轮", Values = (int)EnumHelper.RetailProductType.YouLunPackage,Type=1 }
            };
            return searchScreenList;
        }

        public static List<ShopSearchEntity> GetSearchRetailSortList(int type, bool isRetailShop = false)
        {

            if (type == 1)
            {
                List<ShopSearchEntity> searchSortList1 = new List<ShopSearchEntity>() { new ShopSearchEntity() { Name = "默认排序", Values =  0 },
                new ShopSearchEntity() { Name = "价格升序（从低到高）", Values = 1 },new ShopSearchEntity() { Name = "价格降序（从高到低）", Values = 2 }
                //,
                //new ShopSearchEntity() { Name = "佣金升序（从低到高）", Values = 3 },new ShopSearchEntity() { Name = "佣金降序（从高到低）", Values = 4 }
                };
                return searchSortList1;
            }
            else
            {
                List<ShopSearchEntity> searchSortList2 = new List<ShopSearchEntity>() { new ShopSearchEntity() { Name = "默认排序", Values =  0 },
                new ShopSearchEntity() { Name = "销量排序", Values = 1 },
                new ShopSearchEntity() { Name = "佣金降序（从高到低）", Values = 5 },
                new ShopSearchEntity() { Name = "价格升序（从低到高）", Values = 2 },new ShopSearchEntity() { Name = "价格降序（从高到低）", Values = 3 }
                //,
                //new ShopSearchEntity() { Name = "佣金升序（从低到高）", Values = 4 },new ShopSearchEntity() { Name = "佣金降序（从高到低）", Values = 5 }
                };
                if (isRetailShop)
                {
                    searchSortList2 = new List<ShopSearchEntity>() { new ShopSearchEntity() { Name = "默认排序", Values =  0 },
                    new ShopSearchEntity() { Name = "销量排序", Values = 1 },
                    new ShopSearchEntity() { Name = "价格升序（从低到高）", Values = 2 },new ShopSearchEntity() { Name = "价格降序（从高到低）", Values = 3 }
                    };
                }
                return searchSortList2;
            }
        }

        public static ShopSearchItemEntity GetSearchRetailerProductItemList()
        {
            ShopSearchItemEntity searchItem = new ShopSearchItemEntity();
            List<ShopSearchEntity> searchSortList = new List<ShopSearchEntity>() { new ShopSearchEntity() { Name = "默认排序", Values =  0 },
                new ShopSearchEntity() { Name = "销量排序", Values = 1 },
                new ShopSearchEntity() { Name = "价格升序（从低到高）", Values = 2 },new ShopSearchEntity() { Name = "价格降序（从高到低）", Values = 3 },
                new ShopSearchEntity() { Name = "佣金升序（从低到高）", Values = 4 },new ShopSearchEntity() { Name = "佣金降序（从高到低）", Values = 5 }};
            searchItem.SearchSort = searchSortList;

            List<ShopSearchEntity> searchScreenList = new List<ShopSearchEntity>() { new ShopSearchEntity() { Name = "酒店", Values = 200 }, new ShopSearchEntity() { Name = "美食&玩乐", Values = 600 } };
            searchItem.SearScreen = searchScreenList;
            return searchItem;
        }

        public static ShopSearchItemEntity GetSearchRetailerProductShopItemList()
        {
            ShopSearchItemEntity searchItem = new ShopSearchItemEntity();
            List<ShopSearchEntity> searchSortList = new List<ShopSearchEntity>() { new ShopSearchEntity() { Name = "默认排序", Values =  0 },
                new ShopSearchEntity() { Name = "销量排序", Values = 1 },
                new ShopSearchEntity() { Name = "价格升序（从低到高）", Values = 2 },new ShopSearchEntity() { Name = "价格降序（从高到低）", Values = 3 }};
            searchItem.SearchSort = searchSortList;

            List<ShopSearchEntity> searchScreenList = new List<ShopSearchEntity>() { new ShopSearchEntity() { Name = "酒店", Values = 200 }, new ShopSearchEntity() { Name = "美食&玩乐", Values = 600 } };
            searchItem.SearScreen = searchScreenList;
            return searchItem;
        }

        public static SearchEntity GetSearchItem(int categoryParentId, int payType)
        {
            SearchEntity searchItem = new SearchEntity();
            List<SearchItemEntity> searchSortList = new List<SearchItemEntity>() { //new SearchItemEntity() { Name = "默认排序", Values =  0 }, 
                //new SearchItemEntity() { Name = "按销量排序", Values = 1 },
                new SearchItemEntity() { Name = "价格由低至高", Values = 2 },new SearchItemEntity() { Name = "价格由高至低", Values = 3 }
                ,
                new SearchItemEntity() { Name = "距离由近及远", Values = 4 },new SearchItemEntity() { Name = "距离由远及近", Values = 5 }
            };
            searchItem.SearchSort = searchSortList;

            List<int> ids = ProductAdapter.GetDistinctSPUDistrictID(categoryParentId, payType);
            if (ids.Count > 0 && ids != null)
            {
                List<HJD.DestServices.Contract.DistrictInfoEntity> district = ResourceAdapter.GetDistrictInfo(ids);
                //searchItem.DestinationList
                district.ForEach(_ => searchItem.DestinationList.Add(new SearchItemEntity()
                {
                    Name = _.Name,
                    Values = _.DistrictID
                }));
            }
            return searchItem;
        }

        public static SearchEntity GetDistinctSPUDistrictIDByAlbum(int albumId)
        {
            SearchEntity searchItem = new SearchEntity();
            List<int> ids = ProductAdapter.GetDistinctSPUDistrictIDByAlbum(albumId);
            if (ids.Count > 0 && ids != null)
            {
                List<HJD.DestServices.Contract.DistrictInfoEntity> district = ResourceAdapter.GetDistrictInfo(ids);
                //searchItem.DestinationList
                district.ForEach(_ => searchItem.DestinationList.Add(new SearchItemEntity()
                {
                    Name = _.Name,
                    Values = _.DistrictID
                }));
            }
            return searchItem;
        }


        public static List<GroupDistrictEntity> GetGroupDistinctSPUDistrictIDByAlbum(int albumId)
        {
            List<GroupDistrictEntity> groupList = new List<GroupDistrictEntity>();
            List<int> ids = ProductAdapter.GetDistinctSPUDistrictIDByAlbum(albumId);
            if (ids.Count > 0 && ids != null)
            {
                groupList = GenGroupDistrict(ids);
                //List<HJD.DestServices.Contract.DistrictInfoEntity> districtList = ResourceAdapter.GetDistrictInfo(ids);

                //districtList.ForEach(_ => _.RootName = ((_.RootName == "中国" && _.DataLevel == 3) ? _.Name : _.RootName));
                //IEnumerable<IGrouping<string, HJD.DestServices.Contract.DistrictInfoEntity>> groupDis = districtList.GroupBy(_ => _.RootName);
                //foreach (IGrouping<string, HJD.DestServices.Contract.DistrictInfoEntity> item in groupDis)
                //{
                //    GroupDistrictEntity groupItem = new GroupDistrictEntity();
                //    groupItem.RootName = item.Key;
                //    foreach (HJD.DestServices.Contract.DistrictInfoEntity l in item)
                //    {
                //        SearchItemEntity searchItem = new SearchItemEntity()
                //        { Name = l.Name, Values = l.DistrictID };
                //        groupItem.DestinationList.Add(searchItem);
                //    }
                //    groupList.Add(groupItem);
                //}
            }
            return groupList;
        }


        public static List<GroupDistrictEntity> GetRetailGroupDistinct()
        {
            List<GroupDistrictEntity> groupList = new List<GroupDistrictEntity>();
            List<int> ids = ProductAdapter.GetDistinctRetailSPUDistrict();
            if (ids.Count > 0 && ids != null)
            {
                groupList = GenGroupDistrict(ids);
            }
            return groupList.OrderBy(_ => _.RootName).ToList();
        }


        public static List<GroupDistrictEntity> GenGroupDistrict(List<int> districtIds)
        {
            List<GroupDistrictEntity> groupList = new List<GroupDistrictEntity>();
            List<HJD.DestServices.Contract.DistrictInfoEntity> districtList = ResourceAdapter.GetDistrictInfo(districtIds);

            districtList.ForEach(_ => _.RootName = ((_.RootName == "中国" && _.DataLevel == 3) ? _.Name : _.RootName));
            IEnumerable<IGrouping<string, HJD.DestServices.Contract.DistrictInfoEntity>> groupDis = districtList.GroupBy(_ => _.RootName);
            foreach (IGrouping<string, HJD.DestServices.Contract.DistrictInfoEntity> item in groupDis)
            {
                GroupDistrictEntity groupItem = new GroupDistrictEntity();
                groupItem.RootName = item.Key;
                foreach (HJD.DestServices.Contract.DistrictInfoEntity l in item)
                {
                    SearchItemEntity searchItem = new SearchItemEntity()
                    { Name = l.Name, Values = l.DistrictID };
                    groupItem.DestinationList.Add(searchItem);
                }
                groupList.Add(groupItem);
            }

            return groupList;
        }



        public static SearchEntity GetDistinctSPUDistrictIDByProductTagID(int productTagID)
        {
            SearchEntity searchItem = new SearchEntity();
            List<SearchItemEntity> searchSortList = new List<SearchItemEntity>() {
                new SearchItemEntity() { Name = "价格由低至高", Values = 2 },new SearchItemEntity() { Name = "价格由高至低", Values = 3 }
                ,
                new SearchItemEntity() { Name = "距离由近及远", Values = 4 },new SearchItemEntity() { Name = "距离由远及近", Values = 5 }
            };
            searchItem.SearchSort = searchSortList;

            List<int> ids = ProductAdapter.GetDistinctSPUDistrictIDByProductTagID(productTagID);
            if (ids.Count > 0 && ids != null)
            {
                List<HJD.DestServices.Contract.DistrictInfoEntity> district = ResourceAdapter.GetDistrictInfo(ids);
                //searchItem.DestinationList
                district.ForEach(_ => searchItem.DestinationList.Add(new SearchItemEntity()
                {
                    Name = _.Name,
                    Values = _.DistrictID
                }));
            }
            return searchItem;
        }


        internal static List<CouponActivityRetailEntity> GetSearchProductList(SearchProductRequestEntity param, out int totalCount)
        {

            //totalCount = couponSvc.GetSearchProductListCount(param);
            //return LocalCache10Min.GetData<List<CouponActivityRetailEntity>>(string.Format("GetSearchProductList:{0}|{1}|{2}|{3}|{4}", param.Sort, param.SearchWord, param.Start, string.Join(",", param.Screen.ProductType), param.Count),
            //    () =>
            //    {
            //        var list = couponSvc.GetSearchProductList(param);
            //        foreach (var item in list)
            //        {
            //            List<SKUCouponActivityEntity> skuCouponActivityList = CouponAdapter.GetSKUListByActivityId(item.CouponActivityID);
            //            item.Price = new List<int>();
            //            item.SKUCommissions = new List<int>();
            //            item.SKUIDS = new List<int>();
            //            foreach (var skuprice in skuCouponActivityList)
            //            {
            //                item.Price.Add(skuprice.SKUVipPrice);
            //                item.SKUCommissions.Add(skuprice.SKUCommissions);
            //                item.SKUIDS.Add(skuprice.SKUID);
            //            }
            //            item.OnePicUrl = GetFirstPicUrl(item.PicPath, Enums.AppPhotoSize.small);
            //            item.TotalNum -= item.ManuSellNum;
            //        }
            //        return list;
            //    });
            totalCount = couponSvc.GetSearchProductListCount(param);
            var list = couponSvc.GetSearchProductList(param);
            foreach (var item in list)
            {
                List<SKUCouponActivityEntity> skuCouponActivityList = CouponAdapter.GetSKUListByActivityId(item.CouponActivityID).OrderBy(_ => _.SKUVipPrice).ToList();
                item.Price = new List<Decimal>();
                item.SKUCommissions = new List<Decimal>();
                item.SKUIDS = new List<int>();
                foreach (var skuprice in skuCouponActivityList)
                {
                    item.Price.Add(skuprice.SKUVipPrice);
                    item.SKUCommissions.Add(skuprice.SKUCommissions);
                    item.SKUIDS.Add(skuprice.SKUID);

                }
                item.OnePicUrl = GetFirstPicUrl(item.PicPath, Enums.AppPhotoSize.small);

                item.TotalNum -= item.ManuSellNum;
            }
            return list;

            //int state_online = 1;
            //var list = couponSvc.GetRetailProductList(0, 0, state_online, pageSize, start, out totalCount);



            //return list;
        }

        internal static List<CouponActivityRetailEntity> GetSearchProductListByCategory(SearchProductRequestEntity param, out int totalCount)
        {
            totalCount = couponSvc.GetSearchProductListByCategoryCount(param);
            var list = couponSvc.GetSearchProductListByCategory(param);
            foreach (var item in list)
            {
                List<SKUCouponActivityEntity> skuCouponActivityList = CouponAdapter.GetSKUListByActivityId(item.CouponActivityID).OrderBy(_ => _.SKUVipPrice).ToList();
                item.Price = new List<Decimal>();
                item.SKUCommissions = new List<Decimal>();
                item.SKUIDS = new List<int>();
                foreach (var skuprice in skuCouponActivityList)
                {
                    item.Price.Add(skuprice.SKUVipPrice);
                    item.SKUCommissions.Add(skuprice.SKUCommissions);
                    item.SKUIDS.Add(skuprice.SKUID);

                }
                item.OnePicUrl = GetFirstPicUrl(item.PicPath, Enums.AppPhotoSize.small);

                item.TotalNum -= item.ManuSellNum;
            }
            return list;

        }

        internal static List<CouponActivityRetailEntity> GetSearchRetailerProductList(List<int> skuList)
        {
            List<CouponActivityRetailEntity> list = new List<CouponActivityRetailEntity>();

            list = couponSvc.GetSearchRetailerProductList(skuList);
            foreach (var item in list)
            {
                List<SKUCouponActivityEntity> skuCouponActivityList = CouponAdapter.GetSKUListByActivityId(item.CouponActivityID).OrderBy(_ => _.SKUVipPrice).ToList();
                item.Price = new List<Decimal>();
                item.SKUCommissions = new List<Decimal>();
                item.SKUIDS = new List<int>();
                foreach (var skuprice in skuCouponActivityList)
                {
                    item.Price.Add(skuprice.SKUVipPrice);
                    item.SKUCommissions.Add(skuprice.SKUCommissions);
                    item.SKUIDS.Add(skuprice.SKUID);

                }
                item.OnePicUrl = GetFirstPicUrl(item.PicPath, Enums.AppPhotoSize.small);

                item.TotalNum -= item.ManuSellNum;
            }
            return list;
        }

        public static int AddRetailUrl(RetailUrlEntity param)
        {
            return couponSvc.AddRetailUrl(param);
        }

        public static UserCouponDefineEntity GetUserCouponDefineByID(int id)
        {
            return couponSvc.GetUserCouponDefineByID(id);
        }

        public static List<ExchangeCouponEntity> GetExchangeCouponEntityByCID(long cid)
        {
            return couponSvc.GetExchangeCouponEntityByCID(cid);
        }

        public static List<ExchangeCouponEntity> GetExchangeCouponEntityByPayID(int payId)
        {
            return couponSvc.GetExchangeCouponEntityByPayID(payId);
        }

        public static List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserId(long userId, UserCouponState state)
        {
            return couponSvc.GetUserCouponInfoListByUserId(userId, state);
        }

        public static List<UserCouponItemInfoEntity> GetNewVIPGiftUserCouponInfoListByUserId(long userId)
        {
            return couponSvc.GetNewVIPGiftUserCouponInfoListByUserId(userId);
        }

        public static List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserIdAndType(long userId, UserCouponState state, UserCouponType type)
        {
            return couponSvc.GetUserCouponInfoListByUserIdAndType(userId, state, type);
        }

        public static int GetUserCouponInfoCountByUserIdAndType(long userId, UserCouponState state, UserCouponType type)
        {
            return couponSvc.GetUserCouponInfoCountByUserIdAndType(userId, state, type);
        }

        #region   使用代金券


        //获取指定产品类型指定金额下，所有可用的代金券（订单确认页使用，暂时可以不分页）
        public static List<UserCouponItemInfoEntity> GetCanUseVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req)
        {
            return couponSvc.GetCanUseVoucherInfoListForOrder(req);
        }

        //获取指定产品类型指定金额下，所有不可用的代金券（订单确认页使用，暂时可以不分页）
        public static List<UserCouponItemInfoEntity> GetCannotUseVoucherInfoListForOrder(OrderUserCouponRequestParamsBase req)
        {
            return couponSvc.GetCannotUseVoucherInfoListForOrder(req);
        }

        // 检测指定产品类型指定金额下，当前代金券ID列表是否满足条件
        public static CheckSelectedVoucherInfoForOrderViewModel CheckSelectedVoucherInfoForOrder(OrderVoucherRequestParams req)
        {
            CheckSelectedVoucherInfoForOrderViewModel m = new CheckSelectedVoucherInfoForOrderViewModel();

            List<UserCouponItemInfoEntity> canUseVoucherList = new List<UserCouponItemInfoEntity>();

            if (req.SelectedVoucherIDs.Count == 0)
            {
                m.Success = false;
                m.ErrMessage = "没有选择代金券！";
                m.CashCouponShowName = "请选择代金券";
            }
            else
            {
                canUseVoucherList = GetCanUseVoucherInfoListForOrder(new OrderUserCouponRequestParamsBase
                {
                    BuyCount = req.BuyCount,
                    OrderSourceID = req.OrderSourceID,
                    OrderTypeID = req.OrderTypeID,
                    SelectedDateFrom = req.SelectedDateFrom,
                    SelectedDateTo = req.SelectedDateTo,
                    TotalOrderPrice = req.TotalOrderPrice,
                    UserID = req.UserID
                });

                //   var canUseVoucherList = GetCanUseVoucherInfoListForOrder(req);//
                bool bCanUse = true;

                if (canUseVoucherList.Count() > 0)
                {
                    var joinList = from l in canUseVoucherList
                                   join sid in req.SelectedVoucherIDs on l.IDX equals sid
                                   select l;

                    if (joinList.Count() == req.SelectedVoucherIDs.Count)
                    {
                        m.Success = true;

                        m.VoucherInfoList = joinList.ToList();

                        m.OrderCanDiscountAmount = m.VoucherInfoList.Sum(_ => _.DiscountAmount);
                        if (req.MaxOrderCanUseVoucherAmount > 0 && m.OrderCanDiscountAmount > req.MaxOrderCanUseVoucherAmount)
                            m.OrderCanDiscountAmount = req.MaxOrderCanUseVoucherAmount;
                        if (m.OrderCanDiscountAmount > req.TotalOrderPrice)
                            m.OrderCanDiscountAmount = req.TotalOrderPrice;

                        m.CashCouponShowName = string.Format("已选{0}张代金券", m.VoucherInfoList.Count);
                    }
                    else
                    {
                        bCanUse = false;
                    }
                }
                else
                {
                    bCanUse = false;
                }

                if (bCanUse == false)
                {
                    m.Success = false;
                    m.ErrMessage = "已选代金券不可用！";
                    m.CashCouponShowName = "请选择代金券";
                }
            }

            //      Log.WriteLog(string.Format("req:{0} {1} {2}", JsonHelper.ObjToString(req), JsonHelper.ObjToString(m), JsonHelper.ObjToString(canUseVoucherList)));
            m.OrderCanDiscountAmount = Math.Round(m.OrderCanDiscountAmount, 2);
            return m;
        }


        #endregion

        #region 使用现金券
        //        获取指定产品类型指定金额下，所有可用的券（订单确认页使用，暂时可以不分页）
        public static List<UserCouponItemInfoEntity> GetCanUseCouponInfoListForOrder(OrderUserCouponRequestParams req)
        {
            return couponSvc.GetCanUseCouponInfoListForOrder(req);
        }
        //获取指定产品类型指定金额下，所有不可用的券（订单确认页使用，暂时可以不分页）

        public static List<UserCouponItemInfoEntity> GetCannotUseCouponInfoListForOrder(OrderUserCouponRequestParams req)
        {
            return couponSvc.GetCannotUseCouponInfoListForOrder(req);
        }

        //获取指定产品类型指定金额下，默认最优惠的券（订单确认页需要默认选择一个券）        
        public static TheBestCouponInfoForOrderViewModel GetTheBestCouponInfoForOrder(OrderUserCouponRequestParams req)
        {
            TheBestCouponInfoForOrderViewModel m = new TheBestCouponInfoForOrderViewModel();
            m.CashCouponInfo = couponSvc.GetTheBestCouponInfoForOrder(req);

            if (m.CashCouponInfo.IDX > 0)
            {
                m.Success = true;
                m.OrderCanDiscountAmount = m.CashCouponInfo.OrderCanUseDiscountAmount;
                //  m.CashCouponTypeName = GetCashCouponTypeName((UserCouponType)m.CashCouponInfo.UserCouponType);
                m.CashCouponShowName = GetCashCouponTypeDescription(m.CashCouponInfo);
            }
            else
            {
                m.Success = false;
                m.ErrMessage = "暂无可用现金券！";
                m.CashCouponShowName = "没有可用现金券";
            }

            return m;
        }


        // 检测指定产品类型指定金额下，当前券ID是否满足条件（比如用户选好了一个券后，手动变更数量时，需要实时检测）
        public static CheckSelectedCashCouponInfoForOrderViewModel CheckSelectedCashCouponInfoForOrder(OrderUserCouponRequestParams req)
        {
            CheckSelectedCashCouponInfoForOrderViewModel m = new CheckSelectedCashCouponInfoForOrderViewModel();

            if (req.SelectedCashCouponID == 0)
            {
                m.Success = false;
                m.ErrMessage = "没有选择现金券！";
                m.CashCouponShowName = "请选择现金券";
            }
            else
            {
                var canUseCouponList = couponSvc.GetCanUseCouponInfoListForOrder(req);

                if (canUseCouponList.Count() > 0 && canUseCouponList.Where(_ => _.IDX == req.SelectedCashCouponID).Count() == 1)
                {
                    m.CashCouponInfo = canUseCouponList.Where(_ => _.IDX == req.SelectedCashCouponID).First();

                    m.Success = true;
                    m.OrderCanDiscountAmount = m.CashCouponInfo.OrderCanUseDiscountAmount;
                    //  m.CashCouponTypeName = GetCashCouponTypeName((UserCouponType)m.CashCouponInfo.UserCouponType);
                    m.CashCouponShowName = GetCashCouponTypeDescription(m.CashCouponInfo);
                }
                else
                {
                    m.Success = false;
                    m.ErrMessage = "已选现金券不可用！";
                    m.CashCouponShowName = "请选择现金券";
                }

            }

            m.OrderCanDiscountAmount = Math.Round(m.OrderCanDiscountAmount, 2);
            return m;
        }



        private static string GetCashCouponTypeDescription(UserCouponItemInfoEntity item)
        {
            string CashCouponTypeDescription = "";
            switch ((UserCouponType)item.UserCouponType)
            {
                case UserCouponType.DiscountOverPrice:
                    CashCouponTypeDescription = string.Format("满{0}减{1}", StringHelper.FormatMoney(item.RequireAmount), StringHelper.FormatMoney(item.OrderCanUseDiscountAmount));
                    break;
                case UserCouponType.DiscountUnconditional:
                    CashCouponTypeDescription = string.Format("立减¥{0}", StringHelper.FormatMoney(item.OrderCanUseDiscountAmount));
                    break;
            }

            return CashCouponTypeDescription;
        }


        /// <summary>
        /// 为用户发现金券
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static bool PresentNewVIPGift(long UserID)
        {
            List<String> list = CommAdapter.GetNewVIPGiftCouponList();



            List<long> userList = new List<long> { UserID };

            long sendVIPGiftUserID = -111;  //将创建人设置为负的，以便在取消时可以取消送的券
            list.ForEach(id => SendUserCouponItem(userList, int.Parse(id), sendVIPGiftUserID));

            return true;
        }

        /// <summary>
        /// 批量领取现金券
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cashCouponId"></param>
        /// <returns></returns>
        public static bool SendCashCoupon(long userId, List<int> cashCouponId)
        {
            foreach (int item in cashCouponId)
            {
                couponSvc.SendUserCouponItem(new List<long>() { userId }, item, 0);
            }
            return true;
        }

        public static List<UserCouponItemInfoEntity> GetUserCouponItemByUserIdAndCouponDefineIdlist(long userId, List<int> couponDefineIds)
        {
            return couponSvc.GetUserCouponItemByUserIdAndCouponDefineIdlist(userId, couponDefineIds);
        }

        /// <summary>
        /// VIP续费.老订单做费。新订单延时
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>　
        public static bool ReNewVIPAfterPayment(long userID)
        {
            return couponSvc.ReNewVIPAfterPayment(userID);
        }

        ////VIP欢迎礼券        
        public static List<UserCouponDefineEntity> GetNewVIPGiftUserCouponList()
        {
            return LocalCache10Min.GetData<List<UserCouponDefineEntity>>("GetNewVIPGiftUserCouponList",
                () =>
                {
                    List<UserCouponDefineEntity> ucList = new List<UserCouponDefineEntity>();

                    CommAdapter.GetNewVIPGiftCouponList().
                        ForEach(id => ucList.Add(
                            couponSvc.GetUserCouponDefineByID(int.Parse(id))));

                    return ucList;
                });
        }


        public static String GetVIPConfirmSMS()
        {
            return string.Format("恭喜您已成为VIP会员！您可即刻享受平台会员专享价优惠，并获得￥{0} 现金券用于抵扣消费。同时请加微信shanglv022，回复“会员”进入会员专享福利群，享受超低价尾单福利。", GetNewVIPGiftUserCouponList().Sum(_ => _.DiscountAmount));

        }

        public static int SendUserCouponItem(List<long> userIds, int couponDefineId, long curUserId)
        {
            return couponSvc.SendUserCouponItem(userIds, couponDefineId, curUserId);
        }

        /// <summary>
        /// 发放现金券，返回ID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="couponDefineId"></param>
        /// <param name="curUserId"></param>
        /// <returns></returns>
        public static int SendOneUserCouponItem(long userId, int couponDefineId, long curUserId)
        {
            return couponSvc.SendOneUserCouponItem(userId, couponDefineId, curUserId);
        }

        #endregion

        public static int GetUserCouponInfoCountByUserId(int userId, UserCouponState state)
        {
            return couponSvc.GetUserCouponInfoCountByUserId(userId, state);
        }

        public static UserCouponItemInfoEntity GetUserCouponItemInfoByID(int id)
        {
            return couponSvc.GetUserCouponItemInfoByID(id);
        }

        public static UserCouponItemEntity GetUserCouponItemByID(int id)
        {
            return couponSvc.GetUserCouponItemByID(id);
        }

        public static int UpdateUserCouponItem(UserCouponItemEntity param)
        {
            return couponSvc.UpdateUserCouponItem(param);
        }

        public static bool UpdateUserCoupoinItemByIDs(List<Int32> IDs, Int32 CouponState)
        {
            return couponSvc.UpdateUserCoupoinItemByIDs(IDs, CouponState);
        }

        public static int AddUserCouponConsumeLog(UserCouponConsumeLogEntity param)
        {
            return couponSvc.AddUserCouponConsumeLog(param);
        }

        public static RequestResultEntity CancelUseUserCouponInfoItem(UseCashCouponItem param)
        {
            return couponSvc.CancelUseUserCouponInfoItem(param);
        }

        public static RequestResultEntity UseUserCouponInfoItem(UseCashCouponItem param)
        {
            return couponSvc.UseUserCouponInfoItem(param);
        }

        public static List<UserCouponConsumeLogEntity> GetUserCouponLogByCouponItemID(int idx)
        {
            return couponSvc.GetUserCouponLogByCouponItemID(idx);
        }

        public static RedActivityEntity GetRedActivityByGUID(string guid)
        {
            return couponSvc.GetRedActivityByGUID(guid);
        }

        public static RedRecordEntity GetRedRecordByRedActivityIdAndPhone(int activityId, string phoneNum)
        {
            return couponSvc.GetRedRecordByRedActivityIdAndPhone(activityId, phoneNum);
        }

        public static List<RedPoolDetailEntity> GetRedDetailByPoolId(int poolId)
        {
            string keyWord = ("GetRedDetailByPoolId:" + poolId).ToString();
            return MemCacheHelper.memcached10min.GetData<List<RedPoolDetailEntity>>(keyWord, () =>
            {
                return couponSvc.GetRedDetailByPoolId(poolId);
            });
        }

        public static int AddRedRecord(RedRecordEntity redrecord)
        {
            return couponSvc.AddRedRecord(redrecord);
        }

        public static int updateRedRecord(RedRecordEntity redrecord)
        {
            return couponSvc.UpdateRedRecord(redrecord);
        }

        public static List<RedRecordEntity> GetRedRecordByActivityId(int activityId, int count, int start, out int totalCount)
        {
            return couponSvc.GetRedRecordByActivityId(activityId, count, start, out totalCount);
        }

        public static RedRecordEntity GetRedRecordById(int id)
        {
            return couponSvc.GetRedRecordById(id);
        }

        public static RedPoolDetailEntity GetRedPoolDetailByID(int id)
        {
            return couponSvc.GetRedDetailBylId(id);
        }

        public static int AddRedActivity(RedActivityEntity param)
        {
            return couponSvc.AddRedActivity(param);
        }

        public static List<RedPoolEntity> GetRedPoolByOrderPrice(decimal orderPrice)
        {
            return couponSvc.GetRedPoolByOrderPrice(orderPrice);
        }

        public static List<RedActivityEntity> GetRedActivityByBizIDAndBizType(long bizId, RedActivityType bizType)
        {
            return couponSvc.GetRedActivityByBizIDAndBizType(bizId, bizType);
        }
        public static int UpdateRedRecord(RedRecordEntity param)
        {
            return couponSvc.UpdateRedRecord(param);
        }
        public static string GetThirdCouponOrderID(int CouponID, int type)
        {
            return couponSvc.GetThirdCouponOrderID(CouponID, type);
        }
        public static int UpdateThirdPartyRefundCoupon(int couponid, int thirdstate, int refundstate)
        {
            return couponSvc.UpdateThirdPartyRefundCoupon(couponid, thirdstate, refundstate);
        }
        public static int UpdateOperationState(ExchangeCouponEntity ece)
        {
            return couponSvc.UpdateOperationState(ece);
        }
        public static int UpdateExchangeCouponTravelIDs(ExchangeCouponEntity ece)
        {
            return couponSvc.UpdateTravelIDs(ece);
        }

        public static int UpdateOperationRemark(int CouponID, string remark)
        {
            return couponSvc.UpdateOperationRemark(CouponID, remark);
        }

        public static int CouponChangeUsed(int CouponID)
        {
            List<ExchangeCouponEntity> exchangeList = CouponAdapter.GetExchangeCouponEntityListByIDList(new List<int>() { CouponID });
            foreach (ExchangeCouponEntity exchange in exchangeList)
            {

                //记录到消费券表
                UsedConsumerCouponInfoEntity model = new UsedConsumerCouponInfoEntity();
                model.ExchangeNo = exchange.ExchangeNo;
                model.OperatorId = 0;
                model.SupplierId = ProductAdapter.GetSPUBySKUID(exchange.SKUID).SupplierID;
                model.CreateTime = System.DateTime.Now;
                CouponAdapter.AddUsedConsumerCouponInfo(model);

                //更新ExchangeCoupone 状态
                ExchangeCouponEntity exchangeCoupon = new ExchangeCouponEntity();
                exchangeCoupon.ExchangeNo = exchange.ExchangeNo;
                exchangeCoupon.State = 3;//消费状态为3
                exchangeCoupon.OperationState = 0;
                exchangeCoupon.Updator = 1;
                exchangeCoupon.ID = exchange.ID;
                CouponAdapter.UpdateExchangeState(exchangeCoupon);
            }
            return 1;
        }

        public static int Insert2Refund(RefundCouponsEntity rce)
        {
            return couponSvc.Insert2Refund(rce);
        }

        public static RefundCouponsEntity GetRefundCouponByRefundCouponIDX(int RefoundCouponIDX)
        {
            int count = 0;
            RefundCouponsQueryParam param = new RefundCouponsQueryParam { RefoundCouponIDX = RefoundCouponIDX, PageSize = 1 };
            return couponSvc.GetRefundCouponsList(param, out count).FirstOrDefault();
        }

        public static List<ExchangeCouponEntity> GetExchangeCouponEntityListByIDList(int couponID)
        {
            return couponSvc.GetExchangeCouponEntityListByIDList(new List<int>() { couponID });
        }
        public static int UpdateRefundCouponForPartRefund(RefundCouponsEntity rce)
        {
            return couponSvc.UpdateRefundCouponForPartRefund(rce);
        }

        /// <summary>
        /// 主要更新待退券的后台状态为已退款
        /// </summary>
        /// <param name="rce"></param>
        /// <returns></returns>
        public static int UpdateRefundCoupon(RefundCouponsEntity rce)
        {
            return couponSvc.UpdateRefundCoupon(rce);
        }

        #region 红包发送机制

        /// <summary>
        /// 生成指定券订单的红包奖励记录
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="skutitle"></param>
        /// <param name="amount">佣金奖励(单位：分)</param>
        /// <param name="fromWxuid"></param>
        public static void GenCouponOrderRedPackReward(int sourceId, string skutitle, int amount, int fromWxuid)
        {
            var addEntity = new WeixinRewardRecord();
            addEntity.SourceId = sourceId;
            addEntity.SourceType = 2;   //1酒店订单 2酒店订单 3活动合作伙伴ID
            addEntity.ReWxUid = fromWxuid;
            addEntity.Wishing = "发钱啦！";
            addEntity.Amount = amount;   //换算成分
            addEntity.Number = 1;
            addEntity.ActiveId = 0;
            addEntity.ActiveName = "分享有礼";
            addEntity.Remark = "周末酒店红包";
            addEntity.SceneId = 1;   //1:商品促销 2:抽奖 3:虚拟物品兑奖 4:企业内部福利 5:渠道分润 6:保险回馈 7:彩票派奖 8:税务刮奖
            addEntity.SendName = "周末酒店";
            addEntity.WillSendTime = DateTime.Now;
            addEntity.RealSendTime = DateTime.Now;
            addEntity.State = 0;

            var addReward = weixinService.AddWeixinRewardRecordByWxuid(addEntity);

            //Log.WriteLog(string.Format("【全员分销】AddWeixinRewardRecordByWxuid:{0} {1} {2} {3}", addReward, addEntity.SourceId, addEntity.ReWxUid, addEntity.ActiveName));
        }

        /// <summary>
        /// 通用活动红包发送
        /// </summary>
        /// <param name="addEntity"></param>
        public static int GenActiveRedPackReward(WeixinRewardRecord addEntity)
        {
            //var addEntity = new WeixinRewardRecord();
            //addEntity.SourceId = sourceId;
            //addEntity.SourceType = sourceType;    //1酒店订单 2酒店订单 3活动合作伙伴ID
            //addEntity.ReOpenid = openId;
            //addEntity.Wishing = wishing;
            //addEntity.Amount = amount;   //换算成分
            //addEntity.Number = 1;
            //addEntity.ActiveName = activeName;
            //addEntity.Remark = remark;
            //addEntity.SceneId = 3;   //1:商品促销 2:抽奖 3:虚拟物品兑奖 4:企业内部福利 5:渠道分润 6:保险回馈 7:彩票派奖 8:税务刮奖
            //addEntity.SendName = "周末酒店";
            //addEntity.WillSendTime = DateTime.Now;
            //addEntity.RealSendTime = DateTime.Now;
            //addEntity.State = 0;

            var addReward = weixinService.AddWeixinRewardRecord(addEntity);

            //Log.WriteLog(string.Format("【全员分销】AddWeixinRewardRecordByWxuid:{0} {1} {2} {3}", addReward, addEntity.SourceId, addEntity.ReWxUid, addEntity.ActiveName));

            return addReward;
        }

        /// <summary>
        /// 通用活动红包发送(for redpack union active)
        /// </summary>
        /// <param name="addEntity"></param>
        public static int GenActiveRedPackRewardForUnion(WeixinRewardRecord addEntity)
        {
            var addReward = weixinService.AddWeixinRewardRecordForRedpackUnion(addEntity);

            return addReward;
        }
        public static ExchangeCouponEntity GetExchangCouponByCouponId(int couponId)
        {
            return GetExchangCouponByCouponIdList(new List<int> { couponId }).FirstOrDefault();
        }


        public static List<ExchangeCouponEntity> GetExchangCouponByCouponIdList(List<int> couponIdList)
        {
            return couponSvc.GetExchangCouponByCouponIdList(couponIdList);
        }

        /// <summary>
        /// 获取券类型的UserOrderInfoEntity
        /// </summary>
        /// <param name="couponIdList"></param>
        /// <returns></returns>
        public static List<OrderListItemEntity> GetUserOrderInfoEntityByOrderIDList(List<int> couponIdList)
        {
            return TransExchangeCouponEntityItem2OrderListItemEntity(GetExchangCouponByCouponIdList(couponIdList));
        }
        /// <summary>
        /// 获取UserOrderInfoEntity
        /// </summary>
        /// <param name="couponIdList"></param>
        /// <returns></returns>
        public static List<OrderListItemEntity> GetUserOrderInfoEntityByOrderIDList(List<long> couponIdList)
        {
            return GetUserOrderInfoEntityByOrderIDList(couponIdList.Select(_ => (int)_).ToList());
        }

        /// <summary>
        /// 更新过期不退款券数据
        /// </summary>
        /// <returns></returns>
        public static bool UpdateExchangeCouponExpiredNotRefund()
        {
            try
            {
                var couponList = couponSvc.GetExpiredNotRefundExchangeCouponList();
                int i = couponSvc.UpdateExchangeCouponExpiredNotRefund();
                foreach (var item in couponList)
                {
                    ProductCache.RemoveUserDetailOrderCache(item.ID);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog("更新过期不退款券数据" + ex.Message + ex.StackTrace);
                return false;
            }
        }

        #endregion


        public static AlbumSkuListEntity GetSKUAlbumEntityListByAlbumId(int album, int start, int count)
        {
            AlbumSkuListEntity result = new AlbumSkuListEntity();
            List<SKUAlbumEntity> list = new List<SKUAlbumEntity>();
            int totalCount = 0;
            List<SKUAlbumEntity> skulist = couponSvc.GetSKUAlbumEntityListByAlbumId(album, start, count, out totalCount);
            string skuids = string.Join(",", skulist.Select(_ => _.SKUID).ToList());
            List<ProductPropertyInfoEntity> propertyInfo = ProductAdapter.GetProductPropertyInfoBySKU(skuids);
            IEnumerable<IGrouping<int, SKUAlbumEntity>> groupList = skulist.GroupBy(_ => _.SPUID);
            foreach (var item in groupList)
            {
                int spuid = item.Key;
                SKUAlbumEntity skualbum = new SKUAlbumEntity();
                skualbum = skulist.Where(_ => _.SPUID == spuid).First();
                if (!string.IsNullOrEmpty(skualbum.PicPath))
                {
                    try
                    {
                        skualbum.PicList = new List<string>();
                        var picList = skualbum.PicPath.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                        skualbum.PicList.Add(PhotoAdapter.GenHotelPicUrl(picList[0], Enums.AppPhotoSize.theme));
                    }
                    catch (Exception ex)
                    {

                    }
                }
                foreach (var sku in item)
                {
                    //PropertyType ： 普通 = 1,城市 = 2,日期 = 3, 大人小孩 = 4,拼团 = 5
                    bool isGroupProduct = propertyInfo.FindAll(p => p.SKUID == sku.SKUID && p.PropertyType == 5 && p.PropertyOptionTargetID == 2).Count() > 0 ? true : false;
                    if (isGroupProduct)
                    {
                        skualbum.GroupSKUPrice = sku.SKUPrice;
                        skualbum.GroupSKUVipPrice = sku.SKUVipPrice;
                    }
                    else
                    {
                        skualbum.SKUPrice = sku.SKUPrice;
                        skualbum.SKUVipPrice = sku.SKUVipPrice;
                    }
                }
                list.Add(skualbum);
            }
            result.SKUList = list.OrderBy(_ => _.Rank).ToList();
            result.TotalCount = totalCount;
            return result;
        }


        public static List<UserCouponUseCondationEntity> GetCouponUseCondationByInVal(int intVal, int condationType, int showState = -1)
        {
            return couponSvc.GetCouponCondationByIntVal(intVal, condationType, showState);
        }

        public static List<UserCouponDefineEntity> GetCouponDefineByIntVals(string intVals, int condationType, long userId)
        {
            return couponSvc.GetCouponDefineByIntVals(intVals, condationType, userId);
        }

        public static List<UserCouponDefineEntity> GetUserCouponDefineListByIds(string ids, long userId)
        {
            return couponSvc.GetUserCouponDefineListByIds(ids, userId);
        }

        public static int GetUserCouponItemByUserId(long userId, int couponDefineID)
        {
            return couponSvc.GetUserCouponItemByUserId(userId, couponDefineID);
        }
        public static int UpdateSupplierCouponState(SupplierCouponEntity supplierCouponEntity, int couponID = 0)
        {
            return couponSvc.UpdateSupplierCouponState(supplierCouponEntity, couponID);
        }

        public static List<ExchangeCouponEntity> GetExchangCouponByCouponOrderID(long couponOrderID)
        {
            return couponSvc.GetExchangCouponByCouponOrderID(couponOrderID);
        }


        /// <summary>
        /// 获取模板数据
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="bizId">2:skuid,3exchangecouponid</param>
        /// <param name="bizType">2sku产品类型3订单类型</param>
        /// <returns></returns>
        public static HJD.HotelManagementCenter.Domain.TemplateDataEntity GetTemplateData(int templateId, int bizId, int bizType)
        {
            HJD.HotelManagementCenter.Domain.TemplateDataEntity templateData = HMC_HotelService.GetTemplateDataList(templateId, bizId, bizType).FirstOrDefault();
            if (templateData != null && !string.IsNullOrWhiteSpace(templateData.TemplateItem))
            {
                templateData.ContentList = JsonConvert.DeserializeObject<List<HJD.HotelManagementCenter.Domain.TemplateContent>>(templateData.TemplateItem);
            }
            return templateData == null ? new HJD.HotelManagementCenter.Domain.TemplateDataEntity() : templateData;
        }


        /// <summary>
        /// 取消用户与券的关联关系
        /// </summary>
        /// <param name="p"></param>
        /// <param name="canceledOrderIDList"></param>
        internal static void CancelUserOrderRel(long userID, List<long> canceledOrderIDList)
        {
            OrderHelper.DeleteUserCouponOrderRelRedisData(userID, canceledOrderIDList);
        }




        internal static List<ExchangeCouponEntity> GetExchangeCouponByUserIdAndSKUID(long userId, int skuId)
        {
            return couponSvc.GetExchangeCouponListByUserIDAndSKU(userId, skuId);
        }
        internal static List<ExchangeCouponIDEntity> GetExchangeCouponIDListBySKUID(int skuId)
        {
            return couponSvc.GetExchangeCouponIDListBySKUID(skuId);
        }
    }
}