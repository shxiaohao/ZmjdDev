using HJD.AccountServices.Entity;
using HJD.CommentService.Contract;
using HJD.CouponService.Contracts.Entity;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain.Settlement;
using HJD.HotelManagementCenter.IServices;
using HJD.HotelPrice.Contract;
using HJD.HotelPrice.Contract.DataContract;
using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Common.Security;
using HJDAPI.Controllers.Cache;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HJDAPI.Controllers.Adapter
{
    public class OrderAdapter
    {
        public static IHotelPriceService PriceService = ServiceProxyFactory.Create<IHotelPriceService>("BasicHttpBinding_IHotelPriceService");
        public static IOrderService HMCOrderService = ServiceProxyFactory.Create<IOrderService>("IOrderService");
        public static ISettlementService HMCSettlementService = ServiceProxyFactory.Create<ISettlementService>("ISettlementService");
        public static ISMSService SMSService = ServiceProxyFactory.Create<ISMSService>("ISMSService");
        public static ISupplierService SupplierService = ServiceProxyFactory.Create<ISupplierService>("HMC_ISupplierService");
        public static IPackageService PackageService = ServiceProxyFactory.Create<IPackageService>("HMC_IPackageService");

        static string BotaoPayLabelSUrl = "116SKh305e";//116QOxm0ed
        static string GuaranteePayLabelSUrl = "116QOzl0ut";//担保付
        static string CashPayPayLabelSUrl = "116QJ3103P";//"116KKgW0J5";//"116QOys0W1";//现付
        static string PrePayLabelSUrl = "116KKgW3jY";//"116QOzh0RM";//预付
        static string NoCancelLabelSUrl = "116QJ3i1HS";//"116KKgW2JF";//不可取消
        static string LimitCancelLabelSUrl = "116QJ3i0di"; //"116KKgW1jF";//限时取消

        static readonly string BoTaoAPIKey = Configs.BoTaoAPIKey;
        static readonly string BoTaoSignKey = Configs.BoTaoSignKey;
        static readonly string BoTaoAPI = Configs.BoTaoAPI;

        static readonly object _lock = new object();

        public static HJD.HotelManagementCenter.Domain.OrderAddPayDetailEntity GetOrderAddPayListByOID(long OID)
        {
            HJD.HotelManagementCenter.Domain.OrderAddPayDetailEntity result = new HJD.HotelManagementCenter.Domain.OrderAddPayDetailEntity();
            List<HJD.HotelManagementCenter.Domain.OrderAddPayDetailEntity> orderAddPayList = HMCOrderService.GetOrderAddPayListByOID(OID).Where(_ => _.State == 4 && _.PayChannelType == 1).ToList();
            if (orderAddPayList.Count > 0 && orderAddPayList != null)
            {
                result = orderAddPayList.First();
            }
            return result;
        }


        public static List<TravelPersonOrderEntity> GetTravelPersonByOid(long oid, bool isNeedTravelPersonDetail = false)
        {
            List<TravelPersonOrderEntity> travelPersonEncrypt = new List<TravelPersonOrderEntity>();
            foreach (TravelPersonOrderEntity tpo in PriceService.GetTravelPersonByOId(oid))
            {
                try
                {
                    if (tpo.CardType == 1)
                    {

                        tpo.CardNum = tpo.CardNum.Substring(0, 7) + "********" + tpo.CardNum.Substring(tpo.CardNum.Length - 3);
                    }
                    else
                    {
                        tpo.CardNum = tpo.CardNum.Substring(0, 2) + "********" + tpo.CardNum.Substring(tpo.CardNum.Length - 2);
                    }
                    ////是否需要查询出行人照片
                    //if (isNeedTravelPersonDetail)
                    //{
                    //    tpo.OrderContactInfo = GetOrderContacts(0, tpo.OId, 0, tpo.ID);
                    //}
                }
                catch
                {

                }
                travelPersonEncrypt.Add(tpo);
            }
            return travelPersonEncrypt;
        }
        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddCommOrders(HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity commorders)
        {
            return PriceService.AddCommOrders(commorders);
        }


        public static HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity GetCommOrder(int OrderID)
        {
            return PriceService.GetCommOrder(OrderID);
        }


        public static bool TempSendActiveRebate(long orderid, int activeRebateAmount, long CID)
        {
            //暂停发红包
            //PackageOrderInfo order = GetPackageOrderInfo(orderid, 0);

            //if (order.UserID == 0)
            //{
            //    string phone = order.ContactPhone;

            //    order.UserID = AccountAdapter.GetOrRegistPhoneUser(phone, CID).UserId;

            //    HotelAdapter.HotelService.BindUserAccountAndOrders(order.UserID, phone);
            //}

            //int CashCouponAmount = activeRebateAmount * 10;
            //int shareUserNum = CashCouponAmount < 300 ? 8 : 10;


            //List<int> redBegItem = RedBegMath.GenMun(CashCouponAmount, shareUserNum, 200, 20);
            //HJD.CouponService.Contracts.Entity.OriginCouponResult oc = CouponAdapter.GenerateOriginCoupon(order.UserID, 100, orderid, CashCouponAmount, activeRebateAmount,
            //    string.Join(",", redBegItem.Select(i => i * 100)));


            //ADAdapter.AddUserNotices_ActiveRabate(order.UserID, activeRebateAmount);

            return true;
        }

        public static List<HJD.HotelManagementCenter.Domain.Settlement.InvoiceInfoEntity> GetInvoiceInfoByOID(long oid)
        {
            return HMCSettlementService.GetInvoiceInfoByOID(oid);
        }

        public static List<UserRebateInfoEntity> GetUserRebateList(long userid)
        {
            List<UserRebateInfoEntity> rl = PriceService.GetUserRebateList(userid);
            foreach (var r in rl)
            {
                r.Rebate *= 100; //转换成分
                switch (r.RebateType)
                {
                    case 1:
                        r.Description = "成功预订了" + r.HotelName;
                        break;
                    case 2:
                        r.Description = "领取返现红包";
                        break;
                }
            }
            return rl;
        }

        /// <summary>
        /// 提交支付前重新验证一下订单
        /// 1.支付金额是否已变化。
        /// 2.是否还有房
        /// 3.现金券是否还有
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="appVer"></param>
        /// <returns></returns>
        public static CheckOrderBeforePayResponse CheckOrderBeforePay(long orderid, string appVer = null)
        {
            CheckOrderBeforePayResponse res = new CheckOrderBeforePayResponse();
            res.bCanPay = true;
            res.Message = "订单可以支付！";

            TimeLog log = new TimeLog("CheckOrderBeforePay", 1000);

            ///orderId 小于1000000 表示房券订单 独立的一套验证方法
            if (orderid < 1000000)
            {
                res = CouponAdapter.IsExchangeOrderCanPay((int)orderid);
            }
            else
            {
                PackageOrderInfo order = PriceService.GetOrderInfo(orderid);
                log.AddLog("GetOrderInfo==========");
                try
                {
                    var orderUserId = order.UserID;
                    var blackList = AccountAdapter.AccService.GetBlackList(orderUserId);
                    log.AddLog("GetBlackList==========");
                    var isInBlackList = blackList != null && blackList.Any() ? true : false;

                    if (isInBlackList)
                    {
                        res.bCanPay = false;
                        res.Message = "此订单暂时无法支付, 如需预订请联系客服4000-021-702";
                    }
                    else
                    {
                        HotelPrice2 h2 = PriceAdapter.GetHotelPackageList(order.HotelID, order.CheckIn.ToString(), order.CheckIn.AddDays(order.NightCount).ToShortDateString(), "www", appVer, orderUserId, pid: order.PID);

                        log.AddLog("GetHotelPackageList==========");
                        PackageInfoEntity pinfo = h2.Packages.Where(h => h.packageBase.ID == order.PID).FirstOrDefault();

                        int amount = pinfo.Price * order.RoomCount;

                        int userHasCouponAmount = 0;
                        //如果用户使用了现金券，检查现金券
                        if (order.UserUseCashCouponAmount > 0)
                        {
                            userHasCouponAmount = CouponAdapter.GetUserCanUseCashCouponAmount(orderUserId);
                        }

                        log.AddLog("GetUserCanUseCashCouponAmount==========");
                        if (order.UserUseCashCouponAmount > 0 && userHasCouponAmount < order.UserUseCashCouponAmount)
                        {
                            res.bCanPay = false;
                            res.Message = "现金券余额不足！";
                        }
                        else if (pinfo.SellState == 2)
                        {
                            res.bCanPay = false;
                            res.Message = "该套餐已售完！";
                        }
                        else if (amount != order.Amount)
                        {
                            res.bCanPay = false;
                            if (amount < order.Amount)
                            {
                                res.Message = "套餐售价已调整，更加优惠，请重新下单！";
                            }
                            else
                            {
                                res.Message = "套餐售价已调整，请重新下单！";
                            }
                        }


                        //如果有用券，需要检查券是否可用
                        if (order.CashCouponID > 0)
                        {
                            var check = CouponAdapter.CheckSelectedCashCouponInfoForOrder(new OrderUserCouponRequestParams
                            {
                                TotalOrderPrice = order.Amount,
                                UserID = order.UserID,
                                SelectedCashCouponID = order.CashCouponID,
                                BuyCount = 1,
                                OrderSourceID = order.PID,
                                SelectedDateFrom = order.CheckIn,
                                SelectedDateTo = order.CheckIn.AddDays(order.NightCount),
                                OrderTypeID = CashCouponOrderSorceType.hotelPackage
                            });
                            log.AddLog("CheckSelectedCashCouponInfoForOrder==========");
                            if (check.Success == false)
                            {
                                res.bCanPay = false;
                                res.Success = BaseResponse.ResponseSuccessState.BeforePay_Coupon_OverTime;
                                res.Message = CouponAdapter.GenCannotPayMsgForUserCoupon(order.CashCouponID);
                            }
                            log.AddLog("check.Success == false==========");

                        }



                        //如果有往基金，那么需要检查住基金是否足够
                        if (order.UserUseHousingFundAmount > 0)
                        {
                            if (FundAdapter.GetUserFundInfo(order.UserID).TotalFund < order.UserUseHousingFundAmount)
                            {
                                res.bCanPay = false;
                                res.Success = BaseResponse.ResponseSuccessState.BeforePay_Coupon_FundError;
                                res.Message = CouponAdapter.GenCannotPayMsgForFund();
                            }
                        }
                        log.AddLog("order.UserUseHousingFundAmount==========");


                        //  Log.WriteLog(string.Format("userHasCouponAmount:{0}   UserUseCashCouponAmount:{1}", userHasCouponAmount, order.UserUseCashCouponAmount));
                    }
                }
                catch (Exception err)
                {

                    Log.WriteLog("CheckOrderBeforePay:" + err.Message + err.StackTrace);
                    res.bCanPay = false;
                    res.Success = BaseResponse.ResponseSuccessState.Order_CannotPay;// 100;
                    res.Message = "订单不能支付！";
                }
            }
            log.Finish();

            return res;
        }

        public static CheckSubmitOrderResonse CheckSubmitOrderBefore(int pid, long userId, int packageType = 1)
        {
            CheckSubmitOrderResonse result = new CheckSubmitOrderResonse();

            PackageEntity package = PackageAdapter.GetOnePackageEntity(pid);

            HJDAPI.Common.Helpers.Enums.CustomerType customerType = AccountAdapter.GetCustomerType(userId);
            HotelServiceEnums.PricePolicyType ppt = PriceAdapter.TransCustomerType2PricePolicyType(customerType);

            if ((package.OnlyForVIP == true || (packageType != 1 && pid < 0)) && ppt != HotelServiceEnums.PricePolicyType.VIP)
            {
                result.ResultCode = 1;
                result.ResponseResult.Text = "成为VIP";
                result.ResponseResult.Description = "抱歉，此价格为新VIP会员专享，成为VIP会员才能购买哦～";
                result.ResponseResult.ActionUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1";
            }
            if (package.ForVIPFirstBuy == true || (packageType != 1 && pid < 0))
            {
                if (ppt != HotelServiceEnums.PricePolicyType.VIP)
                {
                    result.ResultCode = 1;
                    result.ResponseResult.Text = "成为VIP";
                    result.ResponseResult.Description = "抱歉，此价格仅供新VIP会员专享哦～";
                    result.ResponseResult.ActionUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1";
                }
                else if (!(customerType == Enums.CustomerType.vip199nr
                    || customerType == Enums.CustomerType.vip599)
                    )
                {
                    result.ResultCode = 2;
                    result.ResponseResult.Text = "2";
                    result.ResponseResult.Description = "抱歉，此价格仅供新VIP会员专享哦～";
                    result.ResponseResult.ActionUrl = "";
                }
                else if (HasBuyFORVIPFirstBuyPackage(userId))
                {
                    result.ResultCode = 2;
                    result.ResponseResult.Text = "2";
                    result.ResponseResult.Description = "抱歉，此专享优惠套餐每位VIP会员限购一套哦，您之前已经购买过啦～";
                    result.ResponseResult.ActionUrl = "";
                }
                else if (AccountAdapter.HasUserPriviledge(userId, PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage) == false)
                {
                    result.ResultCode = 2;
                    result.ResponseResult.Text = "2";
                    result.ResponseResult.Description = "抱歉，此专享优惠套餐每位VIP会员限购一套哦，您之前已经购买过啦～";
                    result.ResponseResult.ActionUrl = "";
                }
            }
            return result;
        }

        public static OrderSubmitResult SubmitOrder(OrderEntity order, string appVer = null)
        {

            Log.WriteLog("SubmitOrder:" + JsonConvert.SerializeObject(order));
            OrderSubmitResult submitResult = new OrderSubmitResult();
            submitResult.ErrorCode = OrderErrorCode.SUCCESS;
            try
            {

                //过了5点不能预订当天的酒店 
                if (order.package.CheckIn.Date == DateTime.Now.Date && DateTime.Now.Hour > 17)
                {
                    submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_DAY;
                    submitResult.ErrorMessage = "当天下午5点后不能预订当天入住的订单，谢谢！";
                }
                else
                {


                    TimeLog tlog = new TimeLog("SubmitOrder", 1000);

                    // 提交订单 userid 和 手机号不一致。submitphone是userid对应的手机号 
                    if (order.main.UserID != 0)
                    {
                        User_Info userInfo = AccountAdapter.GetUserInfoByUserId(order.main.UserID);
                        order.package.SubmitPhone = userInfo.MobileAccount;
                    }
                    else
                    {
                        order.package.SubmitPhone = order.package.ContactPhone;
                    }

                    tlog.AddLog(order.main.HotelID.ToString() + ":" + order.package.ContactPhone.ToString());

                    FullFillUserID(order);
                    tlog.AddLog("FullFillUserID");
                    HJDAPI.Common.Helpers.Enums.CustomerType customerType = GetCustomerType(order);
                    tlog.AddLog("GetCustomerType");
                    order.package.CustomerType = (int)customerType;//获取客户类型 提交订单保存
                    long submitOrderWithUserId = order.main.UserID;//提交订单时使用的userId 如果未登录或带有特殊标识 即便手机号正确也认为不享受相关优惠        
                    HotelServiceEnums.PricePolicyType ppt = PriceAdapter.TransCustomerType2PricePolicyType(customerType);

                    Log.WriteLog(string.Format("SubmitOrder: HotelID:{0}, CheckIn:{1},  CheckOut:{2}, appVer:{3} ,  UserID:{4}  PID：{5} Contact:{6} ContactPhone:{7}", order.main.HotelID, order.package.CheckIn.ToString(), order.package.CheckIn.AddDays(order.package.NightCount).ToShortDateString(), appVer, order.main.UserID, order.package.PID, order.package.Contact, order.package.ContactPhone));



                    HotelPrice2 h2 = PriceAdapter.GetHotelPackageList(order.main.HotelID, order.package.CheckIn.ToString(), order.package.CheckIn.AddDays(order.package.NightCount).ToShortDateString(), "www", appVer, order.main.UserID, needNotSalePackage: true);
                    tlog.AddLog("PriceAdapter.Get3");
                    PackageInfoEntity curPackageInfo = GetOrderPackageInfo(order, appVer, h2);
                    tlog.AddLog("GetOrderPackageInfo");
                    int canCustomBuyMin = curPackageInfo.CustomBuyMin;
                    int canCustomBuyMax = curPackageInfo.CustomBuyMax;
                    if (curPackageInfo.packageBase != null && curPackageInfo.packageBase.ID != 0)
                    {
                        if (order.package.CheckIn < DateTime.Now.Date)
                        {
                            submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_DAY;
                            submitResult.ErrorMessage = "入住日期不能早于今天，请重新选择，谢谢！";
                        }
                        else if (curPackageInfo.packageBase.IsSellOut == false)
                        {
                            if (canCustomBuyMax == 0 || order.package.RoomCount <= canCustomBuyMax)
                            {
                                if (canCustomBuyMin == 0 || order.package.RoomCount >= canCustomBuyMin)
                                {
                                    try
                                    {
                                        order.package.LastCancelTime = curPackageInfo.LastCancelTime;

                                        CheckOnlyVIPOrder(order, appVer, ppt, submitResult, curPackageInfo);
                                        tlog.AddLog("CheckOnlyVIPOrder");
                                        CheckForVIPFirstBuyOrder(order, appVer, ppt, customerType, submitResult, curPackageInfo);
                                        tlog.AddLog("CheckForVIPFirstBuyOrder");
                                        if (submitResult.ErrorCode == OrderErrorCode.SUCCESS)
                                        {
                                            order.main.OriAmount = curPackageInfo.NotVIPPrice * order.package.RoomCount;

                                            // CheckUseCashCoupon(order,  submitOrderWithUserId, curPackageInfo);
                                            //验证订单可用的现金券数量
                                            if (order.package.UserUseCashCouponAmount > 0 && order.package.PayType != 1)
                                            {
                                                var userCanUseCashCouponAmount = CouponAdapter.GetUserCanUseCashCouponAmount(submitOrderWithUserId) / 100;//分转元 全部可用的现金券数量
                                                tlog.AddLog("GetUserCanUseCashCouponAmount");
                                                var maxUserUseCashCouponAmount = curPackageInfo.CanUseCashCoupon * order.package.RoomCount;//套餐可用现金券 2016.01.13 王文斌 前台现付不享受现金券
                                                //App 4.6版以客户端传过来的现金券金额为准 4.6之前的版本需要可用现金券最大值来计算
                                                if (string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("4.6") < 0)
                                                {
                                                    order.package.UserUseCashCouponAmount = maxUserUseCashCouponAmount > userCanUseCashCouponAmount ? userCanUseCashCouponAmount : maxUserUseCashCouponAmount;
                                                }
                                                else
                                                {
                                                    //4.6之后版本 以客户端传过来的现金券作为参考 不再默认取套餐可用的最大
                                                    order.package.UserUseCashCouponAmount = maxUserUseCashCouponAmount > order.package.UserUseCashCouponAmount ? order.package.UserUseCashCouponAmount : maxUserUseCashCouponAmount;
                                                }
                                            }
                                            else
                                            {
                                                order.package.UserUseCashCouponAmount = 0;
                                            }

                                            //验证订单可用的住基金数量  前台现付不享受住基金
                                            if (order.package.UserUseHousingFundAmount > 0 && order.package.PayType != 1)
                                            {
                                                var maxUserUseHousingFundAmount = (int)FundAdapter.GetUserFundInfo(submitOrderWithUserId).TotalFund;//用户一共可用的住基金数量
                                                tlog.AddLog("GetUserFundInfo");
                                                order.package.UserUseHousingFundAmount = maxUserUseHousingFundAmount > order.package.UserUseHousingFundAmount ? order.package.UserUseHousingFundAmount : maxUserUseHousingFundAmount;
                                            }
                                            else
                                            {
                                                order.package.UserUseHousingFundAmount = 0;
                                            }

                                            order.package.PCode = curPackageInfo.packageBase.Code;
                                            order.package.RoomTypeID = curPackageInfo.Room.ID;
                                            order.package.Brief = curPackageInfo.packageBase.Brief;
                                            order.package.VIPFirstPayDiscount = curPackageInfo.packageBase.VIPFirstPayDiscount;
                                            order.package.DiscountAmount = (curPackageInfo.NotVIPPrice - curPackageInfo.VIPPrice) * order.package.RoomCount + order.package.UserUseCashCouponAmount;
                                            order.package.RoomDesc = curPackageInfo.Room.Description;
                                            if (order.package.IsVipHot == 0)
                                            {
                                                order.package.IsVipHot = curPackageInfo.packageBase.ForVIPFirstBuy == true ? 1 : 0;
                                            }
                                            SetOrderDailyItems(order, h2, appVer);
                                            tlog.AddLog("SetOrderDailyItems");
                                            SetOrderCanlendar(order);
                                            tlog.AddLog("SetOrderCanlendar");
                                            SetCashRebateCoupon(order);
                                            tlog.AddLog("SetCashRebateCoupon");
                                            if (order.main.Type == OrderType.Package)
                                            {
                                                SetSupplierRate(order, ppt);
                                                tlog.AddLog("SetSupplierRate");
                                            }
                                            else if (order.main.Type == OrderType.JLTour) //捷旅订单
                                            {
                                                order.package.RoomSupplierID = 5;//捷旅作为房间供应商的ID
                                                order.SupplierRateList = new List<OrderSupplierRateEntity>();
                                            }
                                            else if (order.main.Type == OrderType.CtripPackage || order.main.Type == OrderType.CtripPackageByApi)
                                            {
                                                SetCtripSupplierRate(order, ppt);
                                                tlog.AddLog("SetCtripSupplierRate");
                                                //如果是担保订单，那么用携程担保房间供应商ID：114
                                                if (order.package.PayType == 2)
                                                {
                                                    order.package.RoomSupplierID = 114;
                                                    order.SupplierRateList.ForEach(r => r.ProductID = 114);
                                                }
                                                else
                                                {
                                                    order.package.RoomSupplierID = 10;//携程作为房间供应商的ID
                                                }
                                            }
                                            else if (order.main.Type == OrderType.CtripPackageForHotel)
                                            {
                                                order.package.RoomSupplierID = 0;//默认供应商0
                                                SetCtripSupplierRate(order, ppt);
                                                tlog.AddLog("SetCtripSupplierRate");
                                            }

                                            //判断是否为分销产品
                                            if (order.main.ChannelID > 1)
                                            {
                                                order.main.IsDistributed = curPackageInfo.packageBase.IsDistributable;
                                            }

                                            if (string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("4.2") < 0)
                                            {
                                                submitResult = PriceService.SubmitOrder(order);
                                                tlog.AddLog("SubmitOrder");
                                            }
                                            else
                                            {
                                                order.Calendar = order.Calendar.Take(10).ToList();
                                                submitResult = PriceService.SubmitOrderV42(order);
                                                tlog.AddLog("SubmitOrderV42");
                                            }

                                            if (submitResult.ErrorCode == OrderErrorCode.SUCCESS)
                                            {
                                                long orderId = submitResult.OrderID;
                                                //PayType == 1 套餐维护的支付类型 前台现付
                                                if (order.package.PayType == 1)
                                                {
                                                    PriceService.OffilnePayOrder(orderId, (int)(order.main.Amount * 100), orderId.ToString(), "用户前台现付", 8);
                                                    tlog.AddLog("OffilnePayOrder");
                                                }

                                                //提交订单时不支持开发票  --2018-11-02
                                                //SetOrderInvoiceInfo(order, submitResult);
                                                tlog.AddLog("SetOrderInvoiceInfo");

                                                //如果该订单的套餐绑定了相应的供应商产品 则需插入相应的记录数据 2015-03-31 wbwang@zmjiudian.com
                                                HMCOrderService.InsertOrderSProducts(submitResult.OrderID);//插入数据
                                                tlog.AddLog("InsertOrderSProducts");

                                                UpdateOrderRelWHSales(submitResult.OID, GetOrderRelWHSaleInfo(order.main.HotelID, order.package.RoomSupplierID, order.package.PID));

                                                //更新Redis缓存
                                                OrderHelper.AddOrderToRedis(orderId);

                                            }
                                        }
                                    }
                                    catch (Exception err)
                                    {
                                        Log.WriteLog(err.Message + err.StackTrace);// + JsonConvert.SerializeObject(order) + "\r\n\r\n");

                                        submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_SERVER;
                                        submitResult.ErrorMessage = "订单提交失败，请您重新下单，谢谢！";

                                        SMSService.SendSMS("18021036971", string.Format("订单提交失败：{0} {1} {2} {3} {4} {5}",
                                            order.main.HotelID, order.package.PID, order.package.CheckIn,
                                            order.package.Contact, order.package.ContactPhone, err.Message));
                                    }
                                }
                                else
                                {
                                    submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_OVER_PACKAGE_COUNT;
                                    submitResult.ErrorMessage = "每位用户" + canCustomBuyMin + "套起购, 请重新选择！";
                                }
                            }
                            else
                            {
                                submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_OVER_PACKAGE_COUNT;
                                submitResult.ErrorMessage = "每位用户限购" + canCustomBuyMax + "套, 请重新选择！";
                            }
                        }
                        else
                        {
                            submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_SELLOUT;
                            submitResult.ErrorMessage = "套餐已售完！";
                        }
                    }
                    else
                    {
                        submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_NOPID;
                        submitResult.ErrorMessage = "套餐已过期，请稍后再重新下单，谢谢！";

                        //  Log.WriteLog(string.Format("SubmitOrder 末找到套餐： HotelID:{0}, CheckIn:{1},  CheckOut:{2}, appVer:{3} ,  UserID:{4}  PID：{5}",   order.main.HotelID, order.package.CheckIn.ToString(), order.package.CheckIn.AddDays(order.package.NightCount).ToShortDateString(), appVer, order.main.UserID, order.package.PID));
                    }

                    if (submitResult.ErrorCode == OrderErrorCode.SUCCESS && submitResult.State == 10)
                    {
                        submitResult.NextURL = string.Format("whotelapp://www.zmjiudian.com/personal/order/{0}", submitResult.OrderID);
                    }
                    else
                    {
                        submitResult.NextURL = "";
                    }

                    if (submitResult.ErrorCode != OrderErrorCode.SUCCESS)
                    {
                        Log.WriteLog(string.Format("SubmitOrder {6}： HotelID:{0}, CheckIn:{1},  CheckOut:{2}, appVer:{3} ,  UserID:{4}  PID：{5}", order.main.HotelID, order.package.CheckIn.ToString(), order.package.CheckIn.AddDays(order.package.NightCount).ToShortDateString(), appVer, order.main.UserID, order.package.PID, submitResult.ErrorMessage));
                    }

                    tlog.Finish();
                }
            }
            catch (Exception err)
            {
                submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_SELLOUT;
                Log.WriteLog(string.Format("SubmitOrder Error {0}", err.Message + err.StackTrace));

            }
            return submitResult;
        }


        public static long BGSubimitOrder(OrderEntity order)
        {
            //long oid = PriceService.OrderMain_Add(order.main);
            //order.package.ID = oid;
            //int bigbed = order.package.Note.IndexOf("大床");
            //int twinBed = order.package.Note.IndexOf("双床");
            //if (bigbed > -1 && twinBed == -1)
            //{
            //    bigbed = 1 * order.package.RoomCount;
            //    twinBed = 0;
            //}
            //else if (bigbed == -1 && twinBed > -1)
            //{
            //    bigbed = 0;
            //    twinBed = 1 * order.package.RoomCount;
            //}
            //else 
            //{
            //    bigbed = 1 * order.package.RoomCount;
            //    twinBed = 0;
            //}

            //PriceService.OrderPackage_Add(order.package, JsonConvert.SerializeObject(order), bigbed, twinBed);
            long oid = 0;
            try
            {
                SetOrderCanlendar(order);
                order.Calendar = order.Calendar.Take(10).ToList();
                SetSupplierRate(order, HotelServiceEnums.PricePolicyType.Default);
                User_Info userinfo = AccountAdapter.GetOrRegistPhoneUser(order.package.ContactPhone);
                order.main.UserID = userinfo.UserId;
                oid = PriceService.BGSubimitOrder(order);
                //修改订单关联销售
                UpdateOrderRelWHSales(oid, GetOrderRelWHSaleInfo(order.main.HotelID, order.package.RoomSupplierID, order.package.PID));
            }
            catch (Exception e)
            {
                Log.WriteLog("SetOrderCanlendar error: " + e);
            }
            return oid;
        }


        /// <summary>
        /// 检验订单可用现金券
        /// </summary>
        /// <param name="order"></param>
        /// <param name="submitOrderWithUserId"></param>
        /// <param name="curPackageInfo"></param>
        //private static void CheckUseCashCoupon(OrderEntity order,    long submitOrderWithUserId, PackageInfoEntity curPackageInfo)
        //{
        //    if (order.package.UseCashCouponInfo.CashCouponID > 0)
        //    {
        //        List<UserCouponItemInfoEntity> cashCouponList = CouponAdapter.GetUserCouponInfoListByUserId(order.main.UserID, UserCouponState.log);
        //        var cc = order.package.UseCashCouponInfo;
        //        var canUseCoupon =  cashCouponList.Where(_=> _.UserCouponType == cc.CashCouponType && _.IDX == cc.CashCouponID && _.RequireAmount <= order.main.Amount && _.RestAmount >= cc.UseCashAmount); 

        //        // 有可以使用现金券
        //        if (canUseCoupon.Count() > 0)
        //        {
        //            order.package.DiscountAmount = 0;
        //        }


        //        ////验证订单可用的现金券数量
        //        //if (order.package.UserUseCashCouponAmount > 0 && order.package.PayType != 1)
        //        //{
        //        //    var userCanUseCashCouponAmount = CouponAdapter.GetUserCanUseCashCouponAmount(submitOrderWithUserId) / 100;//分转元 全部可用的现金券数量
        //        //    var maxUserUseCashCouponAmount = curPackageInfo.CanUseCashCoupon * order.package.RoomCount;//套餐可用现金券 2016.01.13 王文斌 前台现付不享受现金券

        //        //    //4.6之后版本 以客户端传过来的现金券作为参考 不再默认取套餐可用的最大
        //        //    order.package.UserUseCashCouponAmount = maxUserUseCashCouponAmount > order.package.UserUseCashCouponAmount ? order.package.UserUseCashCouponAmount : maxUserUseCashCouponAmount;
        //        //}
        //        //else
        //        //{
        //        //    order.package.UserUseCashCouponAmount = 0;
        //        //}

        //    }
        //}

        private static void CheckOnlyVIPOrder(OrderEntity order, string appVer, HotelServiceEnums.PricePolicyType ppt, OrderSubmitResult submitResult, PackageInfoEntity curPackageInfo)
        {
            if (curPackageInfo.packageBase.OnlyForVIP == true && ppt != HotelServiceEnums.PricePolicyType.VIP)
            {

                submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_NOPID;
                submitResult.ErrorMessage = "该套餐为VIP专享！";

                //    Log.WriteLog(string.Format("SubmitOrder 该套餐为VIP专享： HotelID:{0}, CheckIn:{1},  CheckOut:{2}, appVer:{3} ,  UserID:{4}  PID：{5}", order.main.HotelID, order.package.CheckIn.ToString(), order.package.CheckIn.AddDays(order.package.NightCount).ToShortDateString(), appVer, order.main.UserID, order.package.PID));

            }
        }
        /// <summary>
        /// 新VIP会员只能购买一间夜
        /// </summary>
        /// <param name="order"></param>
        /// <param name="appVer"></param>
        /// <param name="ppt"></param>
        /// <param name="submitResult"></param>
        /// <param name="curPackageInfo"></param>
        private static void CheckForVIPFirstBuyOrder(OrderEntity order, string appVer, HotelServiceEnums.PricePolicyType ppt, HJDAPI.Common.Helpers.Enums.CustomerType customerType, OrderSubmitResult submitResult, PackageInfoEntity curPackageInfo)
        {
            //  Log.WriteLog(string.Format("CheckForVIPFirstBuyOrder:ForVIPFirstBuy:{0} customerType:{1}", curPackageInfo.packageBase.ForVIPFirstBuy, customerType));
            if (curPackageInfo.packageBase.ForVIPFirstBuy == true)
            {
                if (customerType == Enums.CustomerType.general)
                {
                    submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_NOPID;
                    submitResult.ErrorMessage = "抱歉，此价格仅供新VIP会员专享哦～";
                    submitResult.TipsMsg = new TextAndUrl()
                    {
                        ActionUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1",
                        Description = "抱歉，此价格为新VIP会员专享，成为VIP会员才能购买哦～",
                        Text = "成为VIP"
                    };
                }
                else if (ppt != HotelServiceEnums.PricePolicyType.VIP)
                {
                    submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_NOPID;
                    //submitResult.ErrorMessage = "抱歉，此套餐仅供从2017/2/23开始新成为VIP的会员购买。";
                    // 5.4之前版本用
                    submitResult.ErrorMessage = "抱歉，此价格仅供新VIP会员专享哦～";

                    // 5.4及后版本用
                    submitResult.TipsMsg = new TextAndUrl()
                    {
                        ActionUrl = "",
                        Description = "抱歉，此价格仅供新VIP会员专享哦～",
                        Text = ""
                    };
                }
                else if (!(customerType == Enums.CustomerType.vip199nr
                    || customerType == Enums.CustomerType.vip599)
                    )
                {
                    submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_NOPID;
                    //submitResult.ErrorMessage = "抱歉，此套餐仅供从2017/2/23开始新成为VIP的会员购买。";
                    submitResult.ErrorMessage = "抱歉，此价格仅供新VIP会员专享哦～";
                    // 5.4及后版本用
                    submitResult.TipsMsg = new TextAndUrl()
                    {
                        ActionUrl = "",
                        Description = "抱歉，此价格仅供新VIP会员专享哦～",
                        Text = ""
                    };
                }
                else if (order.package.RoomCount > 1 || order.package.NightCount > 1)
                {
                    submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_NOPID;
                    //submitResult.ErrorMessage = "抱歉，该专享套餐每位新会员限购一套。";
                    submitResult.ErrorMessage = "抱歉，此专享优惠套餐每位VIP会员限购一套哦～";
                    // 5.4及后版本用
                    submitResult.TipsMsg = new TextAndUrl()
                    {
                        ActionUrl = "",
                        Description = "抱歉，此专享优惠套餐每位VIP会员限购一套哦～",
                        Text = ""
                    };
                }
                else if (HasBuyFORVIPFirstBuyPackage(order.main.UserID))
                {
                    submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_NOPID;
                    //submitResult.ErrorMessage = "抱歉，每位VIP会员只能购买一次新会员专享套餐，您之前已经购买过啦。";
                    submitResult.ErrorMessage = "抱歉，此专享优惠套餐每位VIP会员限购一套哦，您之前已经购买过啦～";
                    // 5.4及后版本用
                    submitResult.TipsMsg = new TextAndUrl()
                    {
                        ActionUrl = "",
                        Description = "抱歉，此专享优惠套餐每位VIP会员限购一套哦，您之前已经购买过啦～",
                        Text = ""
                    };
                }
                else if (AccountAdapter.HasUserPriviledge(order.main.UserID, PrivilegeEnums.UserPrivilege.CanBuyVIPFirstPackage) == false)
                {
                    submitResult.ErrorCode = OrderErrorCode.ORDER_ERROR_NOPID;
                    //submitResult.ErrorMessage = "抱歉，每位VIP会员限享受一次“成为VIP”活动优惠，您已享受过其他优惠活动啦。";
                    submitResult.ErrorMessage = "抱歉，此专享优惠套餐每位VIP会员限购一套哦，您之前已经购买过啦～";
                    // 5.4及后版本用
                    submitResult.TipsMsg = new TextAndUrl()
                    {
                        ActionUrl = "",
                        Description = "抱歉，此专享优惠套餐每位VIP会员限购一套哦，您之前已经购买过啦～",
                        Text = ""
                    };
                }
            }
            if (submitResult.ErrorCode != OrderErrorCode.SUCCESS)
            {
                Log.WriteLog(string.Format("SubmitOrder 该套餐为新VIP专享： HotelID:{0}, CheckIn:{1},  CheckOut:{2}, appVer:{3} ,  UserID:{4}  PID：{5} ErrorMessage：{6}", order.main.HotelID, order.package.CheckIn.ToString(), order.package.CheckIn.AddDays(order.package.NightCount).ToShortDateString(), appVer, order.main.UserID, order.package.PID, submitResult.ErrorMessage));
            }
        }

        public static bool HasBuyFORVIPFirstBuyPackage(long userid)
        {
            int count = HMCOrderService.GetUserBuyForVIPFirstBuyPackageOrderCount(userid);
            return count > 0;
        }

        private static PackageInfoEntity GetOrderPackageInfo(OrderEntity order, string appVer, HotelPrice2 h2)
        {
            PackageInfoEntity curPackageInfo = new PackageInfoEntity();
            var plist = h2.Packages.Where(h => h.packageBase.ID == order.package.PID);
            if (plist.Count() > 0)
            {
                curPackageInfo = plist.FirstOrDefault();
            }

            return curPackageInfo;
        }

        private static Enums.CustomerType GetCustomerType(OrderEntity order)
        {
            HJDAPI.Common.Helpers.Enums.CustomerType customerType = AccountAdapter.GetCustomerType(order.main.UserID);
            //if ((order.main.TerminalID == 4 || order.main.TerminalID == 1) && customerType == Enums.CustomerType.vip) //  如果是微信或网站，那么将订单类型设成普通用户 因为微信上用户末注册，所以VIP用户看到的价格也是普通价格，不做调整就会出现价格不正确的问题
            //{
            //    customerType = Enums.CustomerType.general;
            //}
            return customerType;
        }

        private static void FullFillUserID(OrderEntity order)
        {
            if (order.main.UserID == 0)//如果用户没有注册或登录，则通过手机号查找用户信息，如果用户末注册，则注册
            {
                User_Info ui = AccountAdapter.GetPhoneUser(order.package.SubmitPhone);
                if (ui != null && ui.MobileAccount != null && ui.MobileAccount.Length > 0)
                {
                    order.main.UserID = ui.UserId;
                }
                else
                {
                    OperationResult or = AccountAdapter.RegisterPhoneUser(order.package.SubmitPhone, "", 1, CID: order.main.ChannelID);
                    order.main.UserID = long.Parse(or.Data.Split('|')[0]);
                }
            }
        }

        private static void SetSupplierRate(OrderEntity order, HotelServiceEnums.PricePolicyType ppt)
        {
            int hotelid = order.main.HotelID;
            DateTime CheckIn = order.package.CheckIn;
            DateTime CheckOut = order.package.CheckIn.AddDays(order.package.NightCount);
            int pid = order.package.PID;
            int ptype = (int)order.main.Type;

            PackageRateEntity pr = PriceAdapter.GetHotelOnePackageRate(hotelid, CheckIn, CheckOut, pid, order.package.Note, order.package.RoomCount, order.package.RoomSupplierID, ppt);

            order.SupplierRateList = new List<OrderSupplierRateEntity>();

            foreach (var p in pr.DailyList)
            {
                foreach (var d in p.ItemList)
                {
                    order.SupplierRateList.Add(new OrderSupplierRateEntity
                    {
                        Day = p.Day,
                        OrderPrice = d.Price,
                        CanUseCashCoupon = d.CanUseCashCoupon,
                        PayForSupplier = d.SettlePrice,
                        ProductID = d.ProductID,
                        Rebate = d.Rebate,
                        SID = d.SupplierID,
                        Type = d.SupplierType
                    });

                    //  Log.WriteLog(string.Format("SetSupplierRate:{0} {1} ", p.Day, d.Price));
                }
            }

            order.package.RoomSupplierID = order.SupplierRateList.Where(s => s.Type == 1).First().ProductID;
        }

        /// <summary>
        /// 将 OrderType 转换成对应的 PackageType
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        private static HJD.HotelServices.Contracts.HotelServiceEnums.PackageType TransOrderTypeToPackageType(OrderType orderType)
        {
            return (HJD.HotelServices.Contracts.HotelServiceEnums.PackageType)((int)orderType);
        }

        private static void SetCtripSupplierRate(OrderEntity order, HotelServiceEnums.PricePolicyType ppt)
        {
            int hotelid = order.main.HotelID;
            DateTime CheckIn = order.package.CheckIn;
            DateTime CheckOut = order.package.CheckIn.AddDays(order.package.NightCount);
            int pid = order.package.PID;
            int ptype = (int)order.main.Type;


            //  Log.WriteLog(string.Format("SetCtripSupplierRate:{0}  {1} {2} {3} {4} {5} {6} {7} {8}", hotelid, CheckIn, CheckOut, pid, order.main.Type, order.package.Note, order.package.RoomCount, order.package.RoomSupplierID, ppt));

            PackageRateEntity pr = PriceAdapter.GetCtripHotelOnePackageRate(hotelid, CheckIn, CheckOut, pid, TransOrderTypeToPackageType(order.main.Type), order.package.Note, order.package.RoomCount, order.package.RoomSupplierID, ppt);

            //   Log.WriteLog(string.Format("SetCtripSupplierRate GetCtripHotelOnePackageRate:{0} ", pr.DailyList.Count()));

            order.SupplierRateList = new List<OrderSupplierRateEntity>();

            foreach (var p in pr.DailyList)
            {
                foreach (var d in p.ItemList)
                {
                    order.SupplierRateList.Add(new OrderSupplierRateEntity
                    {
                        Day = p.Day,
                        OrderPrice = d.Price,
                        CanUseCashCoupon = d.CanUseCashCoupon,
                        PayForSupplier = d.SettlePrice,
                        ProductID = d.ProductID,
                        Rebate = d.Rebate,
                        SID = d.SupplierID,
                        Type = d.SupplierType
                    });
                }
            }
            if (order.SupplierRateList.Count > 0)
            {
                order.package.RoomSupplierID = order.SupplierRateList.Where(s => s.Type == 1).First().ProductID;
            }
        }

        private static string GetCouponDetailURL(long OrderID)
        {
            var acUrl = string.Format("{0}/Coupon/Detail?orderid={1}", Configs.WWWURL, OrderID);

            //更换域名
            acUrl = AccessAdapter.GenShortUrl(0, acUrl);

            return acUrl;
        }

        //设置活动可返现金、可分享抵用券、可用抵用券信息
        //检查使用现金券是否正确
        private static void SetCashRebateCoupon(OrderEntity order)
        {
            if (order.DailyItems != null)
            {
                int activeRebate = order.DailyItems.Sum(c => c.ShareRebate) * order.package.RoomCount;

                // order.package.ActiveRebate = 0;//暂不发放红包
                order.package.ActiveRebate = activeRebate >= 50 ? 50 : (activeRebate >= 30 ? 30 : (activeRebate >= 20 ? 20 : 0));

                order.package.CashCouponAmount = order.package.ActiveRebate * 10;

                if (order.package.UserUseCashCouponAmount > 0)//如果金额大于0， 表示使用现金券，目前使用现金券只有使用和不使用之分，如果使用了，那么这里在服务器端重新计算一次现金券金额，以免出错。
                {
                    //用户可用现金券 单位分
                    int UserCanUseCashCouponAmount = CouponAdapter.GetUserCanUseCashCouponAmount(order.main.UserID);
                    if (order.package.UserUseCashCouponAmount * 100 > UserCanUseCashCouponAmount)
                    {
                        order.package.UserUseCashCouponAmount = UserCanUseCashCouponAmount / 100;
                    }
                }
            }
        }

        public static List<OrderListItem> GetPackageOrderInfos(List<long> oids)
        {
            var orderList = PriceService.GetPackageOrderInfos(oids);
            orderList.ForEach(o =>
            {
                o.StateName = o.PayType == 1 ? genPayAsGoStateName(o.State, o.StateName) : o.StateName;
            });
            return orderList;
        }

        private static void SetOrderCanlendar(OrderEntity order)
        {
            // (int)order.main.Type
            List<HJD.HotelServices.Contracts.PDayItem> pl = PackageAdapter.GetHotelPackageCalendar30(order.main.HotelID, order.package.CheckIn, order.package.PID, order.main.UserID);

            order.Calendar = new List<HJD.HotelPrice.Contract.DataContract.Order.PDayItem>();

            foreach (HJD.HotelServices.Contracts.PDayItem p in pl)
            {
                if (p != null && p.PItems.Count > 0)
                {
                    order.Calendar.Add(new HJD.HotelPrice.Contract.DataContract.Order.PDayItem
                    {
                        Day = p.Day,
                        MaxSealCount = p.MaxSealCount,
                        PItems = new List<HJD.HotelPrice.Contract.DataContract.Order.PDayPItem>{
                            new HJD.HotelPrice.Contract.DataContract.Order.PDayPItem{
                                    Day = p.PItems.First().Day,
                                    MaxSealCount  = p.PItems.First().MaxSealCount,
                                    PID = p.PItems.First().PID,
                                    SealState = p.PItems.First().SealState,
                                    SoldCount = p.PItems.First().SoldCount
                            }},
                        SellState = p.SellState,
                        SoldCount = p.SoldCount
                    });
                }
            }
        }

        private static void SetOrderInvoiceInfo(OrderEntity order, OrderSubmitResult submitResult)
        {
            //处理发票
            if (submitResult.ErrorCode == OrderErrorCode.SUCCESS)
            {
                if (order.invoiceInfo != null && order.invoiceInfo.Title.Trim().Length > 0 && order.invoiceInfo.Title != "0")
                {
                    InvoiceEntity invoice = new InvoiceEntity()
                    {
                        Address = order.invoiceInfo.Address,
                        Contact = order.invoiceInfo.Contact,
                        CreateTime = DateTime.Now,
                        CreateUserID = submitResult.UserID,
                        Note = "由用户自主提交的发票信息",
                        Price = ((int)order.main.Amount - order.package.UserUseCashCouponAmount - order.package.UserUseHousingFundAmount) * 100 - (int)(order.package.UseCashCouponInfo.UseCashAmount == null ? 0 : order.package.UseCashCouponInfo.UseCashAmount * 100),
                        State = 0,
                        TelPhone = string.IsNullOrEmpty(order.invoiceInfo.TelPhone) ? order.package.ContactPhone : order.invoiceInfo.TelPhone,
                        Title = order.invoiceInfo.Title,
                        Type = order.invoiceInfo.Type,//3,
                        UpdateTime = DateTime.Now,
                        UpdateUserID = 0,
                        Zip = "",
                        Logistics = "",
                        ShippingType = order.invoiceInfo.ShippingType == null ? "" : order.invoiceInfo.ShippingType,
                        TaxNumber = order.invoiceInfo.TaxNumber
                    };

                    var orderInfo = PriceService.GetOrderInfo(submitResult.OrderID);
                    invoice.OID = orderInfo.ID;
                    invoice.OIDList = new List<long>() { invoice.OID };


                    SettlementAdapter.AddInvoiceInfo(invoice);

                    UserCommInfoEntity info = new UserCommInfoEntity();
                    info.IDType = order.main.UserID == 0 ? 2 : 1;
                    info.UserID = info.IDType == 1 ? order.main.UserID : long.Parse(order.package.ContactPhone);
                    info.State = 1;
                    info.InfoType = 1;
                    info.Info = invoice.Title;
                    AccountAdapter.AddUserCommInfo(info);

                    info.InfoType = 2;
                    info.Info = invoice.Address;
                    AccountAdapter.AddUserCommInfo(info);


                    info.InfoType = 3;
                    info.Info = invoice.Contact;
                    AccountAdapter.AddUserCommInfo(info);

                    info.InfoType = 4;
                    info.Info = invoice.ShippingType;
                    AccountAdapter.AddUserCommInfo(info);

                    info.InfoType = 5;
                    info.Info = invoice.TelPhone;
                    AccountAdapter.AddUserCommInfo(info);

                    if (!string.IsNullOrWhiteSpace(invoice.TaxNumber))
                    {
                        info.InfoType = 6;
                        info.Info = invoice.TaxNumber.Trim(); ;
                        AccountAdapter.AddUserCommInfo(info);
                    }
                }
            }
        }

        public static bool AddInvoiceInfo(InvoiceEntity invoice, long orderId)
        {
            //6.2.1之后前的版本 不让用户开发票，开发票需要升级app版本
            return false;
            if (invoice != null)
            {
                var orderInfo = PriceService.GetOrderInfo(orderId);
                if (orderInfo != null)
                {
                    InvoiceEntity invoiceInfo = new InvoiceEntity()
                    {
                        OID = orderInfo.ID,
                        Address = invoice.Address,
                        Contact = invoice.Contact,
                        CreateTime = DateTime.Now,
                        CreateUserID = orderInfo.UserID,
                        Note = "由用户自主提交的发票信息",
                        Price = orderInfo.UserFinalTotalPayAmount, //UserFinalTotalPayAmount 本向就是以分记的
                        State = 0,
                        TelPhone = invoice.TelPhone,
                        Title = invoice.Title,
                        Type = invoice.Type,
                        UpdateTime = DateTime.Now,
                        UpdateUserID = 0,
                        Zip = "",
                        Logistics = "",
                        ShippingType = invoice.ShippingType,
                        TaxNumber = invoice.TaxNumber
                    };
                    UserCommInfoEntity info = new UserCommInfoEntity();
                    info.IDType = orderInfo.UserID == 0 ? 2 : 1;
                    info.UserID = info.IDType == 1 ? orderInfo.UserID : long.Parse(invoice.TelPhone);
                    info.State = 1;
                    info.InfoType = 1;
                    info.Info = invoice.Title;
                    AccountAdapter.AddUserCommInfo(info);

                    info.InfoType = 2;
                    info.Info = invoice.Address;
                    AccountAdapter.AddUserCommInfo(info);


                    info.InfoType = 3;
                    info.Info = invoice.Contact;
                    AccountAdapter.AddUserCommInfo(info);

                    info.InfoType = 4;
                    info.Info = invoice.ShippingType;
                    AccountAdapter.AddUserCommInfo(info);

                    info.InfoType = 5;
                    info.Info = invoice.TelPhone;
                    AccountAdapter.AddUserCommInfo(info);
                    if (!string.IsNullOrWhiteSpace(invoice.TaxNumber))
                    {
                        info.InfoType = 6;
                        info.Info = invoice.TaxNumber.Trim();
                        AccountAdapter.AddUserCommInfo(info);
                    }

                    return SettlementAdapter.AddInvoiceAndOrderRel(invoiceInfo);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static BaseResponseEx AddInvoiceInfo_Ver6_3(HJDAPI.Models.RequestParams.GetInvoiceParams param)
        {
            BaseResponseEx result = new BaseResponseEx();
            InvoiceEntity invoice = param.invoice;
            long orderId = param.orderid;
            int receivePeopleInformationId = param.ReceivePeopleInformationId;
            if (invoice != null)
            {
                var orderInfo = PriceService.GetOrderInfo(orderId);
                if (orderInfo != null)
                {
                    lock (_lock)
                    {
                        int invoiceLogCount = GetInvoiceInfoByOID(orderInfo.ID).Where(_ => _.State != 3).Count();
                        if (invoiceLogCount == 0)
                        {
                            InvoiceEntity invoiceInfo = new InvoiceEntity()
                            {
                                OID = orderInfo.ID,
                                Address = invoice.Address == null ? "" : invoice.Address,
                                Contact = invoice.Contact == null ? "" : invoice.Contact,
                                CreateTime = DateTime.Now,
                                CreateUserID = orderInfo.UserID,
                                Note = "由用户自主提交的发票信息",
                                Price = orderInfo.UserFinalTotalPayAmount, //UserFinalTotalPayAmount 本向就是以分记的
                                State = invoice.InvoiceFormType == 1 ? 3 : 0,//纸质发票提交时标识为已取消，支付 费用后改为已标记
                                TelPhone = invoice.TelPhone == null ? "" : invoice.TelPhone,
                                Title = string.IsNullOrEmpty(invoice.Title) ? "个人" : invoice.Title,//ios6.4的bug，发票类型是个人时，没有传title
                                Type = invoice.Type,
                                UpdateTime = DateTime.Now,
                                UpdateUserID = 0,
                                Zip = "",
                                Logistics = "",
                                ShippingType = invoice.ShippingType == null ? "" : invoice.ShippingType,
                                TaxNumber = invoice.TaxNumber == null ? "" : invoice.TaxNumber,
                                TicketOpeningType = invoice.TicketOpeningType,
                                InvoiceFormType = invoice.InvoiceFormType,
                                Email = invoice.Email,
                                PayType = invoice.PayType
                            };
                            UserCommInfoEntity info = new UserCommInfoEntity();

                            info.IDType = orderInfo.UserID == 0 ? 2 : 1;
                            info.UserID = info.IDType == 1 ? orderInfo.UserID : long.Parse(invoice.TelPhone);
                            info.State = 1;

                            //个人发票title前端固定”“个人” 所以这里 个人发票类型 不存储 发票抬头
                            if (invoice.TicketOpeningType == 1)
                            {
                                info.InfoType = 1;
                                info.Info = invoice.Title;
                                AccountAdapter.AddUserCommInfo(info);
                            }

                            info.InfoType = 2;
                            info.Info = invoice.Address;
                            AccountAdapter.AddUserCommInfo(info);


                            info.InfoType = 3;
                            info.Info = invoice.Contact;
                            AccountAdapter.AddUserCommInfo(info);

                            info.InfoType = 4;
                            info.Info = invoice.ShippingType;
                            AccountAdapter.AddUserCommInfo(info);

                            info.InfoType = 5;
                            info.Info = invoice.TelPhone;
                            AccountAdapter.AddUserCommInfo(info);
                            if (!string.IsNullOrWhiteSpace(invoice.TaxNumber))
                            {
                                info.InfoType = 6;
                                info.Info = invoice.TaxNumber.Trim();
                                AccountAdapter.AddUserCommInfo(info);
                            }


                            if (!string.IsNullOrWhiteSpace(invoice.TaxNumber))
                            {
                                info.InfoType = 7;
                                info.Info = invoice.Email;
                                AccountAdapter.AddUserCommInfo(info);
                            }
                            int invoiceId = SettlementAdapter.AddInvoiceAndOrderRel_BackInt(invoiceInfo);

                            if (receivePeopleInformationId > 0)
                            {
                                ReceivePeopleInformationEntity receivePeopleInformation = new ReceivePeopleInformationEntity();
                                receivePeopleInformation.ID = receivePeopleInformationId;
                                receivePeopleInformation.UserId = orderInfo.UserID;
                                AccountAdapter.UpdateReceivePeopleInformation_UpdateIsLastSelected(receivePeopleInformation);
                            }
                            //纸质发票需要支付给用，生成一条支付记录
                            if (invoice.InvoiceFormType == 1 && invoice.PayType != 0)
                            {
                                User_Info userInfo = AccountAdapter.GetUserInfoByUserId(orderInfo.UserID);

                                HJD.CouponService.Contracts.Entity.CommOrderEntity commOrder = new HJD.CouponService.Contracts.Entity.CommOrderEntity();
                                commOrder.CreateTime = DateTime.Now;
                                commOrder.CustomID = invoiceId;
                                commOrder.Name = invoice.Title;
                                commOrder.OpNotice = "";
                                commOrder.PhoneNum = userInfo.MobileAccount;
                                commOrder.Price = invoice.PayType == 1 ? 10 : 0;//判断支付方式
                                commOrder.State = 1;
                                commOrder.TypeID = 10000;
                                commOrder.Points = invoice.PayType == 2 ? 50 : 0;//判断是否积分支付
                                result.BizID = CouponAdapter.InsertCommOrders(commOrder);
                            }

                            result.Success = invoiceId > 0 ? HJDAPI.Models.BaseResponse.ResponseSuccessState.Success : HJDAPI.Models.BaseResponse.ResponseSuccessState.Failed;
                            result.Message = "添加成功";
                        }
                        else
                        {
                            result.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.Failed;
                            result.Message = "该订单已开过发票";
                        }
                    }
                }
                else
                {

                    result.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.Failed;
                    result.Message = "该订单不能开发票";
                }
            }
            else
            {
                result.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.Failed;
                result.Message = "缺少关键参数";
            }
            return result;
        }
        private static void SetOrderDailyItems(OrderEntity order, HotelPrice2 hp, string appVer = null)
        {
            //取出套餐每日条目数据
            //int hotelid = order.main.HotelID;
            //DateTime CheckIn = order.package.CheckIn;
            //DateTime CheckOut = order.package.CheckIn.AddDays(order.package.NightCount);

            int pid = order.package.PID;
            int ptype = (int)order.main.Type;//套餐来源  4携程套餐

            //HotelPrice2 hp = PriceAdapter.Get3(hotelid, CheckIn.ToShortDateString(), CheckOut.ToShortDateString(), "wap", appVer);
            var tempPackageInfos = hp.Packages.Where(o => o.packageBase.ID == pid && o.PackageType == ptype);

            #region 发布4.2版本 去除这段跟踪代码
            //if (order.package.PayType == 1){
            //    Log.WriteLog("套餐" + pid.ToString() + "包括的payType:" + string.Join(",",tempPackageInfos.Select(_=>_.PayType)));
            //}
            #endregion

            if (tempPackageInfos.Count() > 0)
            {
                HJD.HotelServices.Contracts.PackageInfoEntity p = tempPackageInfos.First();


                order.DailyItems = new List<HJD.HotelPrice.Contract.DataContract.Order.PackageDailyEntity>();
                foreach (HJD.HotelServices.Contracts.PackageDailyEntity pdi in p.DailyItems)
                {
                    HJD.HotelPrice.Contract.DataContract.Order.PackageDailyEntity pde = new HJD.HotelPrice.Contract.DataContract.Order.PackageDailyEntity();
                    pde.Day = pdi.Day;
                    pde.PayType = pdi.PayType;
                    pde.Price = pdi.Price;
                    pde.Rebate = pdi.Rebate;
                    pde.CanUseCashCoupon = pdi.CanUseCashCoupon;
                    pde.ShareRebate = pdi.ActiveRebate;

                    pde.Items = new List<HJD.HotelPrice.Contract.DataContract.Order.PItemEntity>();

                    foreach (HJD.HotelServices.Contracts.PItemEntity pie in pdi.Items)
                    {
                        pde.Items.Add(new HJD.HotelPrice.Contract.DataContract.Order.PItemEntity()
                        {
                            Date = pie.Date,
                            DateType = pie.DateType,
                            Description = pie.Description,
                            HotelID = pie.HotelID,
                            ID = pie.ID,
                            ItemCode = pie.ItemCode,
                            PID = pie.PID,
                            Price = pie.Price,
                            Type = pie.Type
                        });
                    }
                    order.DailyItems.Add(pde);
                }
                order.NoticeItems = new List<string>();
                foreach (HJD.HotelServices.Contracts.PItemEntity i in p.Items.Where(o => o.Type > 1))
                {
                    order.NoticeItems.Add(i.Description);
                }

                order.package.PayType = p.PayType;
            }
        }

        public static int RequestOrderRebate(long orderid)
        {
            int ret = PriceService.RequestOrderRebate(orderid);
            return ret;
        }

        public static int RequestOrderActiveRebate(long orderid)
        {
            int ret = PriceService.RequestOrderActiveRebate(orderid);
            return ret;
        }

        public static void OrderActiveRebateShared(long orderid)
        {
            //0：已记录
            //1：已申请
            //2：已返现
            //3：已取消
            //10：已申领（用户已分享）

            PriceService.SetOrderActiveRebateState(orderid, 10);
        }

        public static PackageOrderInfo GetPackageOrderInfo(long orderid, long userid)
        {
            PackageOrderInfo o = PriceService.GetOrderInfo(orderid);
            o.StateName = o.PayType == 1 ? genPayAsGoStateName(o.State, o.StateName) : o.StateName;
            //wwb 不到点评时间的也不能写点评
            if (o.CommentID == 0 && o.State == 12 && o.CheckIn <= DateTime.Now)
            {
                o.CanWriteComment = 1; //可以写点评
            }
            else
            {
                o.CanWriteComment = 0;
            }
            return o;
        }

        /// <summary>
        /// 通过阿里云检查订单支付是否安全
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="terminal_type"></param>
        /// <returns></returns>
        public static bool CheckAlipaySecurity(AlipayCheckParams acp)
        {
            PackageOrderInfo order = null;
            if (acp.orderid < 1000000)
            {
                RoomCouponOrderEntity coe = CouponAdapter.GetOneRoomCouponOrderEntityByPayID((int)acp.orderid);
                if (coe != null && coe.CouponOrderID > 0 && coe.ExchangeCouponList.Count > 0)
                {
                    ExchangeCouponEntity ece = coe.ExchangeCouponList.First();
                    string contactPhone = ece.PhoneNum;
                    long userID = ece.UserID;
                    order = new PackageOrderInfo() { ContactPhone = contactPhone, PackageName = coe.PackageName, HotelName = coe.HotelName, UserID = userID, Amount = coe.TotalPrice };
                }
                else
                {
                    return false;
                }
            }
            else
            {
                order = PriceService.GetOrderInfo(acp.orderid);
            }
            return AlipayCheck.CheckAlipaySecurity(order, acp.orderid, acp.terminalType);
        }


        /// <summary>
        /// 用户主动取消订单
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static OrderCancelResult CancelAuthOrder(long orderid, long userid)
        {
            OrderCancelResult r = new OrderCancelResult() { Message = "", success = 0 };

            PackageOrderInfo p = PriceService.GetOrderInfo(orderid);

            if (p == null)
            {
                r.success = 100;
                r.Message = "无此订单！";
            }
            else if (p.UserID != userid)
            {
                r.success = 1;
                r.Message = "用户信息不正确！";
            }
            else if (!(p.State == 101 || p.State == 1 || p.State == 3 || (p.PayType == 1 && p.State == 10))) //已授权待确认  3:有房待用户支付
            {
                r.success = 2;
                r.Message = "订单状态不正确！";
            }
            else if (p.State == 101 && p.LastCancelTime != DateTime.Parse("2000-1-1") && p.LastCancelTime < DateTime.Now) //已过最晚取消时间
            {
                r.success = 3;
                r.Message = "订单已超过最晚取消日期。";
            }
            else
            {
                int orderSate = p.State == 101 ? 34 : 5;  //如果是预授权，那么进入待确认取消

                string result = HMCOrderService.UpdateOrderState(new HJD.HotelManagementCenter.Domain.OrdedrUpdateParam()
                {
                    id = p.ID,
                    opUserID = userid,
                    orderState = orderSate,
                    tag = "客户端取消"
                });
                if (result.Split(':')[0] == "0")
                {
                    r.success = 4;
                    r.Message = result.Split(':')[1];
                }
                else  //取消成功
                {
                    r.success = 0;
                    r.Message = p.State == 101 ? "订单取消申请已提交。" : "订单取消成功。";

                    //取消用户使用的现金券
                    CouponAdapter.SetOrderCoupon(userid, orderid, 0, false);
                }
            }

            ProductCache.RemoveUserDetailOrderCache(orderid);

            return r;
        }

        public static PackageOrderInfo20 GetPackageOrderInfo20(long orderid, long userid, string appVer = null, ContextBasicInfo contextBasicInfo = null)
        {
            try
            {
                //List<TravelPersonOrderEntity> travelPersonEncrypt = new List<TravelPersonOrderEntity>();
                PackageOrderInfo p = GetPackageOrderInfo(orderid, userid);
                bool isJJOrYL = (p.PackageType == 1 || p.PackageType == 2) ? true : false;
                List<OrderContactInfoEntity> orderContactList = new List<OrderContactInfoEntity>();
                List<TravelPersonOrderEntity> travelPersonEncrypt = new List<TravelPersonOrderEntity>();//GetTravelPersonByOid(p.ID, isJJOrYL);
                //if (isJJOrYL)
                //{
                //    orderContactList = OrderAdapter.GetOrderContactList(orderid, 0);
                //}
                //else
                //{
                //    travelPersonEncrypt = GetTravelPersonByOid(p.ID, isJJOrYL);
                //}

                orderContactList = OrderAdapter.GetOrderContactList(orderid);
                travelPersonEncrypt = GetTravelPersonByOid(p.ID, isJJOrYL);

                HJDAPI.Common.Helpers.Enums.CustomerType customerType =
                    AccountAdapter.HasUserPriviledge(userid, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.BoTaoUser) ?
                    HJDAPI.Common.Helpers.Enums.CustomerType.botao : HJDAPI.Common.Helpers.Enums.CustomerType.general;

                //获取此订单绑定的房券号码
                List<ExchangeCouponEntity> list = CouponAdapter.GetUsedCouponByOrderID(orderid);

                HJD.HotelManagementCenter.Domain.OrderAddPayDetailEntity orderAddPay = OrderAdapter.GetOrderAddPayListByOID(p.ID);


                //已支付待确认的订单，如果有补汇款修改状态为102
                if (orderAddPay.Price > 0 && p.State == 10)
                {
                    p.StateName = "待补汇款";
                    p.State = 102;
                }

                string packageUrl = string.Format(Configs.PackageURL, p.PID, 0, "&userid=" + userid + "&_newpage=1&_newtitle=1");
                string whotelAppUrl = string.Format(Configs.WhotelAppUrl, "hotel/" + p.HotelID + "?userid=" + userid);

                PackageOrderInfo20 p2 = new PackageOrderInfo20
                {
                    StateName = p.PayType == 1 ? genPayAsGoStateName(p.State, p.StateName) : p.StateName,//此处需要根据套餐的 PayType的区别
                    UserShouldPayAmount = p.UserShouldPayAmount,
                    UserFinalTotalPayAmount = p.UserFinalTotalPayAmount / 100,
                    Amount = p.Amount,
                    PayInfo = list != null && list.Count > 0 ? "房券支付" : p.PayType == 3 ? "预付" : p.PayType == 1 ? "现付" : p.PayType == 2 ? "现付担保" : "其他支付方式",
                    PayType = p.PayType,
                    CheckIn = p.CheckIn,
                    Contact = p.Contact.Replace("//", " "),
                    ContactPhone = p.ContactPhone,
                    ID = p.ID,
                    CID = p.CID,
                    NightCount = p.NightCount,
                    Note = p.Note,
                    PID = p.PID,
                    RoomCount = p.RoomCount,
                    State = p.State,
                    SubmitDate = p.SubmitDate,
                    SubDate = p.SubDate,
                    Type = p.Type,
                    UserID = p.UserID,
                    OrderNotice = p.Note,//p.OrderNotice, IOS绑定字段错误 返回值统一为Note
                    LastCancelTime = p.LastCancelTime,
                    CommentID = p.CommentID,
                    ActiveRebate = p.ActiveRebate,
                    ActiveRebateState = p.ActiveRebateState,
                    ActiveURL = p.PayType == 1 ? "" : GetCouponDetailURL(orderid),//现付没有分享现金券
                    OrderID = orderid,
                    CanWriteComment = p.CanWriteComment,
                    UserUseCashCouponAmount = p.UserUseCashCouponAmount,
                    PayChannels = genOrderPayChannels(customerType, p.PayType, p.ContactPhone),
                    TravelPersonList = travelPersonEncrypt,    //出行人信息
                    PayLabelUrls = genOrderPayLabelUrls(customerType, p.PayType, p.LastCancelTime <= DateTime.Now ? 1 : 2),
                    InvoiceType = -1,
                    LoadFile = new List<KeyValueEntity>(),
                    OrderDetailTip = orderAddPay.Price > 0 ? ("订单需补汇款 ￥" + orderAddPay.Price / 100) : "",
                    OrderContactList = orderContactList,
                    //CheckTimeShow = (p.PackageType == 1 || p.PackageType == 2) ? 1 : 0,
                    DateSelectType = p.DateSelectType,
                    PackageType = p.PackageType,
                    CanShowButtons = genOrderOptionBtns(p.State, p.PayType, p.CommentID, p.CheckIn, orderAddPay.Price, contextBasicInfo),//2016-01-19 wwb 遗留问题 订单号太长 导致找不到点评
                    HotelIcon = "http://whfront.b0.upaiyun.com/app/img/order/order-logo-hotel.png",
                    CashCouponDiscount = p.CashCouponDiscount,
                    CashCouponDiscountName = "",
                    UserUseHousingFundAmount = p.UserUseHousingFundAmount,
                    OrderAddPayPrice = orderAddPay.Price / 100,
                    OrderAddPayURL = Configs.WWWURL + "/Order/Pay?order=" + orderAddPay.PayOrderID.ToString() + "&_isoneoff=1", //orderAddPay.PayShortURL,
                    LookDetailURL = (p.Type == 6 || (list != null && list.Count > 0)) ? whotelAppUrl : packageUrl//"whotelapp://www.zmjiudian.com/hotel/" + p.HotelID + "?userid=" + userid : "http://www.zmjiudian.com/hotel/package/" + p.PID + "?userid=" + userid + "&_newpage=1&_newtitle=1"
                };
                if (p.CashCouponID > 0)
                {
                    UserCouponItemInfoEntity usercoupon = CouponAdapter.GetUserCouponItemInfoByID(p.CashCouponID);
                    if (usercoupon != null)
                    {
                        p2.CashCouponDiscountName = usercoupon.Name;
                    }
                }

                if (isJJOrYL)
                {
                    bool haveUpOrderContact = orderContactList.Where(_ => _.DocumentTipState != 1).Count() > 0;
                    p2.OrderContactsTip = haveUpOrderContact ? "证件材料待上传" : ""; //orderContactList.Select(_ => string.IsNullOrWhiteSpace(_.PassportPhoto) ||string.IsNullOrWhiteSpace(_.IDCardPhoto)).Count() > 0 ? "证件材料待上传" : "";
                    p2.OrderContactsEndTime = p2.SubmitDate.AddHours(24);
                    p2.OrderDetailTip = haveUpOrderContact ? "请尽快提交出行人信息，以免耽误你的行程" : "";
                }

                try
                {
                    p2.LoadFile = CommAdapter.GetUploadFileByOrderID(orderid);
                }
                catch (Exception e)
                { }

                //如果订单状态为末支付或已取消、待线下支付，则不能分享红包。
                if (p2.State <= 5 || p2.State == 33)
                {
                    p2.ActiveRebateState = -1;
                }



                if (p.Type == 1)
                {
                    XElement root = XElement.Parse(p.PackageData);

                    p2.PackageName = root.Element("Code").Value;
                    p2.HotelID = int.Parse(root.Element("HotelID").Value);
                    p2.RoomDescription = root.Element("PRoomInfos").Element("PRoomInfo").Attribute("Description").Value;

                    if (root.Element("DateSelectType") != null)
                    {
                        switch (int.Parse(root.Element("DateSelectType").Value))
                        {
                            case 2:
                                p2.OrderDateName = "出行日期";
                                p2.OrderDateDescription = p.CheckIn.ToString("yyyy-MM-dd");
                                break;
                            case 3:
                                p2.OrderDateName = "入住开始日";
                                p2.OrderDateDescription = p.CheckIn.ToString("yyyy-MM-dd");
                                break;
                            case 4:
                                p2.OrderDateName = "消费日期";
                                p2.OrderDateDescription = p.CheckIn.ToString("yyyy-MM-dd");
                                break;
                        }
                    }
                    else
                    {
                        p2.OrderDateName = "";
                    }

                    p2.HotelName = ResourceAdapter.GetHotel(p2.HotelID).Name;//why not p.HotelName??


                    List<OrderDetailItem> Items = new List<OrderDetailItem>();

                    if (root.Element("PItems") != null)
                    {
                        foreach (XElement item in root.Element("PItems").Elements("PItem"))
                        {
                            OrderDetailItem pi = new OrderDetailItem();
                            pi.ID = int.Parse(item.Attribute("ID").Value);
                            pi.Description = item.Attribute("Description") == null ? "" : CommMethods.TransEnCodeHTML2HTML(item.Attribute("Description").Value);
                            pi.Date = item.Element("PItemRel").Attribute("Date") == null ? DateTime.Now : DateTime.Parse(item.Element("PItemRel").Attribute("Date").Value);
                            pi.DateType = item.Element("PItemRel").Attribute("DateType") == null ? 0 : int.Parse(item.Element("PItemRel").Attribute("DateType").Value);
                            pi.Type = int.Parse(item.Attribute("type").Value);
                            pi.Price = item.Attribute("Price") == null ? 0 : int.Parse(item.Attribute("Price").Value);

                            Items.Add(pi);
                        }
                    }

                    //购买须知 追加取消提示信息
                    var lastCancelNotice = "";
                    if (p2.LastCancelTime.Year == 2000)
                    {
                        if (p2.State >= 12)
                        {
                            lastCancelNotice = "订单不可更改或取消。";
                        }
                        else
                        {
                            lastCancelNotice = "订单确认之后，不可更改或取消。";
                        }
                    }
                    else if (DateTime.Now < p2.LastCancelTime)
                    {
                        lastCancelNotice = p2.LastCancelTime.ToString("yyyy年MM月dd日") + (p2.LastCancelTime.Hour < 12 ? "上午" : (p2.LastCancelTime.Hour == 12 ? "中午" : "")) + p2.LastCancelTime.Hour.ToString() + (p2.LastCancelTime.Minute == 0 ? "点整" : "点" + p2.LastCancelTime.Minute.ToString() + "分") + "之前取消预订，可无理由全额退款；过期不可取消或更改。";
                    }
                    else
                    {
                        int DayDiff = (int)(p2.CheckIn - p2.LastCancelTime.Date).TotalDays;
                        if (p2.State >= 12)
                        {
                            lastCancelNotice = string.Format("入住日{0}中午12点之前，可无条件取消或更改。之后，不可取消或更改。", (DayDiff == 0 ? "当天" : "前" + DayDiff.ToString() + "天"));
                        }
                        else
                        {
                            lastCancelNotice = string.Format("订单确认后，入住日{0}中午12点之前，可无条件取消或更改。之后，不可取消或更改。", (DayDiff == 0 ? "当天" : "前" + DayDiff.ToString() + "天"));
                        }
                    }
                    OrderDetailItem refundRule = new OrderDetailItem();
                    refundRule.ID = 0;
                    refundRule.Description = lastCancelNotice;
                    refundRule.Date = DateTime.Now;
                    refundRule.DateType = 2;
                    refundRule.Type = 2;
                    refundRule.Price = 0;

                    Items.Add(refundRule);

                    p2.Notice = Items.Where(i => i.Type == 2 || i.Type == 3).ToList();

                    //for (int i = 0; i < p2.NightCount; i++)
                    //{
                    //    p2.DailyItems.Add(GenDailyItems(p2.CheckIn.AddDays(i), Items));
                    //}

                    //订单详情页只显示一天的内容                   
                    p2.DailyItems.Add(GenDailyItems(p2.CheckIn.AddDays(0), Items));
                }
                else if (p.Type == 3) //深捷旅
                {
                    XElement root = XElement.Parse(p.PackageData);

                    p2.PackageName = p.PackageName;
                    p2.HotelID = p.HotelID;
                    p2.RoomDescription = p.RoomDesc;

                    p2.HotelName = p.HotelName;

                    List<OrderDetailItem> Items = new List<OrderDetailItem>();

                    foreach (XElement PackageDailyEntity in root.Element("DailyItems").Elements("PackageDailyEntity"))
                    {
                        foreach (XElement PItemEntity in PackageDailyEntity.Element("Items").Elements("PItemEntity"))
                        {
                            OrderDetailItem pi = new OrderDetailItem();
                            pi.ID = int.Parse(PItemEntity.Element("ID").Value);
                            pi.Description = CommMethods.TransEnCodeHTML2HTML(PItemEntity.Element("Description").Value);
                            pi.Date = DateTime.Parse(PackageDailyEntity.Element("Day").Value);
                            pi.DateType = 8;
                            pi.Type = int.Parse(PItemEntity.Element("Type").Value);
                            pi.Price = PItemEntity.Element("Price") == null ? 0 : int.Parse(PItemEntity.Element("Price").Value);

                            Items.Add(pi);
                        }
                    }

                    if (root.Element("NoticeItems") != null)
                    {
                        foreach (XElement Desc in root.Element("NoticeItems").Elements("string"))
                        {
                            OrderDetailItem pi = new OrderDetailItem();
                            pi.ID = 0;
                            pi.Description = CommMethods.TransEnCodeHTML2HTML(Desc.Value);
                            pi.Date = DateTime.Parse("1900-1-1");
                            pi.DateType = 0;
                            pi.Type = 2;
                            pi.Price = 0;

                            Items.Add(pi);
                        }
                    }

                    p2.Notice = Items.Where(i => i.Type == 2 || i.Type == 3).ToList();

                    //for (int i = 0; i < p2.NightCount; i++)
                    //{
                    //    p2.DailyItems.Add(GenDailyItems(p2.CheckIn.AddDays(i), Items));
                    //}
                    //订单详情页只显示一天的内容 
                    p2.DailyItems.Add(GenDailyItems(p2.CheckIn.AddDays(0), Items));
                }
                else if (p.Type == 4)   //携程
                {
                    XElement root = XElement.Parse(p.PackageData);
                    p2.PackageName = p.PackageName;
                    p2.HotelID = p.HotelID;
                    p2.RoomDescription = p.RoomDesc;
                    p2.HotelName = p.HotelName;

                    List<OrderDetailItem> Items = new List<OrderDetailItem>();
                    if (root.Element("NoticeItems") != null)
                    {
                        foreach (XElement Desc in root.Element("NoticeItems").Elements("string"))
                        {
                            OrderDetailItem pi = new OrderDetailItem();
                            pi.ID = 0;
                            pi.Description = CommMethods.TransEnCodeHTML2HTML(Desc.Value);
                            pi.Date = DateTime.Parse("1900-1-1");
                            pi.DateType = 0;
                            pi.Type = 2;
                            pi.Price = 0;

                            Items.Add(pi);
                        }
                    }

                    p2.Notice = Items.Where(i => i.Type == 2 || i.Type == 3).ToList();

                    //for (int i = 0; i < p2.NightCount; i++)
                    //{
                    //    p2.DailyItems.Add(GenDailyItems(p2.CheckIn.AddDays(i), Items));
                    //}

                    //订单详情页只显示一天的内容 
                    p2.DailyItems.Add(GenDailyItems(p2.CheckIn.AddDays(0), Items));
                }
                else if (p.Type == 5 || p.Type == 6)   //携程 其他
                {
                    XElement root = XElement.Parse(p.PackageData);
                    p2.PackageName = p.PackageName;
                    p2.HotelID = p.HotelID;
                    p2.RoomDescription = p.RoomDesc;
                    p2.HotelName = p.HotelName;

                    List<OrderDetailItem> Items = new List<OrderDetailItem>();
                    if (root.Element("NoticeItems") != null)
                    {
                        foreach (XElement Desc in root.Element("NoticeItems").Elements("string"))
                        {
                            OrderDetailItem pi = new OrderDetailItem();
                            pi.ID = 0;
                            pi.Description = CommMethods.TransEnCodeHTML2HTML(Desc.Value);
                            pi.Date = DateTime.Parse("1900-1-1");
                            pi.DateType = 0;
                            pi.Type = 2;
                            pi.Price = 0;

                            Items.Add(pi);
                        }
                    }

                    p2.Notice = Items.Where(i => i.Type == 2 || i.Type == 3).ToList();

                    //for (int i = 0; i < p2.NightCount; i++)
                    //{
                    //    p2.DailyItems.Add(GenDailyItems(p2.CheckIn.AddDays(i), Items));
                    //}

                    //订单详情页只显示一天的内容 
                    p2.DailyItems.Add(GenDailyItems(p2.CheckIn.AddDays(0), Items));
                }

                //userid == 4688764 测试账号方便测试
                if ((p2.State == 10 || p2.State == 12 || p2.State == 31 || p2.State == 32) && p2.CheckIn.AddDays(p2.NightCount) < DateTime.Now || userid == 4688764)//) && p2.CheckIn.AddDays(p2.NightCount) < DateTime.Now
                {
                    //p2.InvoiceType = PriceService.GetInvoiceByOId(p.ID);



                    if (string.IsNullOrWhiteSpace(appVer) || appVer.CompareTo("4.6.2") >= 0)
                    {
                        List<HJD.HotelManagementCenter.Domain.Settlement.InvoiceInfoEntity> il = GetInvoiceInfoByOID(p2.ID).Where(_ => _.State != 3).OrderByDescending(_ => _.ID).ToList();

                        //0 未开发票 1不可开发票 -1不可开发票
                        p2.InvoiceType = il.Count;
                        if (il.Count > 0)
                        {
                            HJD.HotelManagementCenter.Domain.Settlement.InvoiceInfoEntity i = il.First();
                            p2.InvoiceInfo = new Models.InvoiceInfoEntity();
                            p2.InvoiceInfo.Title = i.Title;
                            p2.InvoiceInfo.CreateTime = i.CreateTime;
                            p2.InvoiceInfo.Price = (i.ManualPrice > 0 ? i.ManualPrice : i.Price);  //按分来计算的
                            p2.InvoiceInfo.State = i.InvoiceFormType == 1 ? TransInvoiceState(i.State) : TransElectricInvoiceState(i.State);
                            p2.InvoiceInfo.Type = ((Enums.InvoiceType)(i.Type)).ToString();//TransInvoiceType(i.Type);
                            p2.InvoiceInfo.ShippingType = i.ShippingType;
                            p2.InvoiceInfo.TelPhone = (i.InvoiceFormType == 1 && i.TelPhone.Length > 8) ? i.TelPhone.Substring(0, 3) + "****" + i.TelPhone.Substring(7) : "";
                            p2.InvoiceInfo.TaxNumber = i.TaxNumber;
                            p2.InvoiceInfo.ElectronicInvoiceUrl = i.InvoiceFormType == 0 ? "" : "";
                            p2.InvoiceInfo.InvoiceFormType = i.InvoiceFormType;
                            p2.InvoiceInfo.PostagePrice = i.InvoiceFormType == 1 ? "￥" + Convert.ToInt32(i.PaperInvoicePrice) : "";
                            p2.InvoiceInfo.PostagePoints = i.InvoiceFormType == 1 ? Convert.ToInt32(i.PaperInvoicePoints) + "积分抵扣" : "";
                            if (contextBasicInfo != null && contextBasicInfo.IsThanVer6_2_1)
                            {
                                p2.InvoiceInfo.Contact = i.InvoiceFormType == 1 ? i.Contact : "";
                                p2.InvoiceInfo.Address = i.InvoiceFormType == 1 ? i.Address : "";
                            }
                            else
                            {
                                p2.InvoiceInfo.Contact = i.Contact;
                                p2.InvoiceInfo.Address = i.Address;
                            }
                            p2.InvoiceInfo.Email = i.InvoiceFormType == 1 ? "" : i.Email;
                            p2.InvoiceInfo.InvoicePrintTypeName = "增值税普通发票";
                            p2.InvoiceInfo.PayType = i.PayType;
                        }
                    }
                }
                else
                {
                    p2.InvoiceType = -1;
                }


                return p2;
            }
            catch (Exception err)
            {
                Log.WriteLog("GetPackageOrderInfo20 Err:" + err.Message + err.StackTrace);
                return null;
            }
        }

        private static string TransInvoiceState(int state)
        {
            string stateName = "信息已提交，待开发票";
            switch (state)
            {
                case 4:
                case 0: stateName = "信息已提交，待开发票"; break;
                case 1: stateName = "发票已打印，待寄出"; break;
                case 2: stateName = "发票已寄出"; break;
                case 3: stateName = "已取消"; break;
                    //case 0: stateName = "已记录"; break;
                    //case 1: stateName = "已打印"; break;
                    //case 2: stateName = "已邮寄"; break;
                    //case 3: stateName = "已取消"; break;
            }

            return stateName;
        }


        private static string TransElectricInvoiceState(int state)
        {
            string stateName = "信息已提交，待开发票";
            switch (state)
            {
                case 4:
                case 0: stateName = "信息已提交，待开发票"; break;
                case 1:
                case 2:
                    stateName = "发票已开";
                    break;
                case 3: stateName = "已取消"; break;
            }

            return stateName;
        }

        private static string TransInvoiceType(int type)
        {
            string typeName = "住宿费";
            switch (type)
            {
                case 0:
                    typeName = "服务费";
                    break;
                case 1:
                    typeName = "会务服务费";
                    break;
                case 2:
                    typeName = "住宿费";
                    break;
            }
            return typeName;
        }

        private static string TransSate(int p)
        {
            string statename = "";
            switch (p)
            {
                case 1:
                    statename = "已提交";
                    break;
                case 10:
                    statename = "已付款";
                    break;
                case 12:
                    statename = "已确认";
                    break;
                case 5:
                    statename = "已取消";
                    break;
            }

            if (p > 12) statename = "已确认";

            return statename;

        }

        private static OrderDetailDailyEntity GenDailyItems(DateTime dateTime, List<OrderDetailItem> Items)
        {

            OrderDetailDailyEntity pd = new OrderDetailDailyEntity();
            pd.Day = dateTime;

            List<OrderDetailItem> pItems = Items.Where(i => i.Type == 1 && i.Date == dateTime).ToList();
            if (pItems == null || pItems.Count == 0)//如果不是指定日期条目，取指定星期条目
            {
                int weekday = (int)dateTime.DayOfWeek;
                weekday = weekday == 0 ? 7 : weekday; //星期日=0，需要处理。 数据库定义中0为任意时间
                pItems = Items.Where(r => r.Type == 1 && r.DateType == weekday).ToList();

                if (pItems == null || pItems.Count == 0)//取平日价格
                    pItems = Items.Where(r => r.Type == 1 && r.DateType == 0).ToList();

            }

            pd.Items = pItems;

            return pd;
        }

        public static List<OrderListItem> GetUserOrderList(long userid, int start, int count)
        {
            if (userid > 0)
            {
                List<OrderListItem> list = PriceService.GetUserOrderList(userid, start, count);
                if (list != null && list.Count > 0)
                {
                    HJDAPI.Common.Helpers.Enums.CustomerType customerType = AccountAdapter.HasUserPriviledge(userid, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.BoTaoUser) ? HJDAPI.Common.Helpers.Enums.CustomerType.botao : HJDAPI.Common.Helpers.Enums.CustomerType.general;

                    //批量获取点评涉及酒店照片Url集合
                    var hotelIds = list.Select(_ => _.HotelId).Distinct();
                    var hotelPicsList = HotelAdapter.GetManyHotelPhotos(hotelIds);

                    var finalHasPicHotel = new Dictionary<int, List<HJD.HotelServices.Contracts.HotelPhotoEntity>>();
                    foreach (var item in hotelPicsList)
                    {
                        HJD.HotelServices.Contracts.HotelPhotoEntity photo = item.HPList.FirstOrDefault();
                        if (photo != null && photo.HotelID > 0)
                        {
                            finalHasPicHotel.Add(photo.HotelID, item.HPList);
                        }
                    }

                    list.ForEach((_) =>
                    {
                        if (finalHasPicHotel.ContainsKey(_.HotelId))
                        {
                            var photos = finalHasPicHotel[_.HotelId];
                            var coverPic = photos.FirstOrDefault(j => j.IsCover == true);
                            if (coverPic != null)
                            {
                                _.HotelPics = new List<string>() { PhotoAdapter.GenHotelPicUrl(coverPic.SURL, HJDAPI.Common.Helpers.Enums.AppPhotoSize.applist) };
                            }
                            else
                            {
                                _.HotelPics = new List<string>() { PhotoAdapter.GenHotelPicUrl(photos[0].SURL, HJDAPI.Common.Helpers.Enums.AppPhotoSize.applist) };
                            }
                        }
                        else
                        {
                            _.HotelPics = new List<string>();
                        }


                        _.StateName = _.PayType == 1 ? genPayAsGoStateName(_.State, _.StateName) : _.StateName;
                        _.PayLabelUrls = genOrderPayLabelUrls(customerType, _.PayType);
                        _.PayChannels = genOrderPayChannels(customerType, _.PayType, _.ContactPhone);

                        _.CanShowButtons = genOrderOptionBtns(_.State, _.PayType, _.CommentID, _.CheckIn);//2016-01-19 wwb 遗留问题 订单号太长 导致找不到点评

                        _.HasSurprise = _.PayType == 1 ? 0 : _.HasSurprise;//现付不能获得现金券红包
                    });
                }
                return list;
            }
            else
            {
                return new List<OrderListItem>();
            }
        }


        /// <summary>
        /// 获取用户酒店订单列表,Redis缓存有数据就从缓存取
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<OrderListItemEntity> GetAllHotelOrderList(long userId)
        {
            List<OrderListItem> list = PriceService.GetAllHotelOrderList(userId);
            List<OrderListItemEntity> orderList = OrderAdapter.TransOrderListItem2OrderListItemEntity(list);
            orderList.ForEach(_ => _.CouponOrderId = _.OrderId);
            return orderList;
        }



        /// <summary>
        /// 获取用户酒店订单列表
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public static OrderListItem GetUserOrderInfoByOrderID(long orderId)
        {
            return GetUserOrderInfoByOrderIDList(new List<long> { orderId }).FirstOrDefault();
        }

        public static List<OrderListItem> GetUserOrderInfoByOrderIDList(List<long> orderIdList)
        {
            return PriceService.GetUserOrderInfoByOrderIDList(orderIdList);
        }

        public static List<OrderListItemEntity> GetUserOrderInfoEntityByOrderIDList(List<long> orderIdList)
        {
            return TransOrderListItem2OrderListItemEntity(
                            GetUserOrderInfoByOrderIDList(orderIdList)
                );
        }

        /// <summary>
        /// 把酒店订单列表转换 List<OrderListItemEntity>
        /// </summary>
        /// <param name="list">酒店订单列表</param>
        /// <param name="ptype">订单类型</param>
        /// <param name="icon">订单图标</param>
        /// <returns></returns>
        public static List<OrderListItemEntity> TransOrderListItem2OrderListItemEntity(List<OrderListItem> list)
        {
            List<OrderListItemEntity> result = new List<OrderListItemEntity>();
            foreach (OrderListItem item in list)
            {
                OrderListItemEntity model = new OrderListItemEntity();
                model.TotalPoints = 0; //订单没有积分兑换
                model.TotalAmount = item.Amount;
                model.NightCount = item.NightCount;

                model.OrderState = item.State;
                model.OrderStateName = item.StateName;
                model.OrderAddPayURL = item.OrderAddPayUrl;

                model.OrderProductDesc = item.PackageName;
                model.OrderProductName = item.HotelName;
                model.RoomCount = item.RoomCount;
                model.StartDate = item.CheckIn;
                model.SubmitOrderDate = item.SubmitDate;
                model.EndDate = Convert.ToDateTime("1900-01-01");
                model.BookDate = Convert.ToDateTime("1900-01-01");
                model.Icon = OrderHelper.GetIcon(item.PackageType);
                model.OrderId = item.Orderid;
                model.OrderType = item.PackageType;
                model.GroupState = -1;
                model.GroupStateName = "";
                model.Count = 1;
                model.DetailOrderList = new List<DetailOrderListEntity>();
                model.PayID = 0;
                model.CouponOrderId = model.OrderId;
                //是机酒游轮套餐
                if (item.PackageType == 1 || item.PackageType == 2)
                {
                    model.OrderDetailTip = "请尽快提交出行人信息，以免耽误你的行程";
                }
                else
                {
                    model.OrderDetailTip = "";
                }
                result.Add(model);
            }
            return result;
        }
        /// <summary>
        /// 获取用户全部类型订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderType">订单类型</param>
        /// <param name="start">分页索引</param>
        /// <param name="count">获取数据条数</param>
        /// <returns></returns>
        public static List<OrderListItemEntity> GetAllOrderList(long userId, EnumHelper.OrderType orderType, int start, int count)
        {
            return OrderHelper.GetOrderListFromRedis(userId, orderType, start, count);
        }

        public static int GetUserOrderCountByPackageType(int pType, long userid)
        {
            return PriceService.GetUserOrderCountByPackageType(pType, userid);
        }
        public static int AddOrderAddPay(long oid, int price, string reason, long curUserID = 0)
        {
            int AddPayID = HMCOrderService.AddOrderAddPay(new HJD.HotelManagementCenter.Domain.OrderAddPayEntity
            {
                CreateTime = DateTime.Now,
                Creator = curUserID,
                Note = "",
                OID = oid,
                PayChannelType = (int)HJD.HotelManagementCenter.Domain.PayChannelType.tbd,
                Price = price,
                RequestPrice = price,
                StartDate = DateTime.Now,
                State = (int)HJD.HotelManagementCenter.Domain.SettleState.waiting,
                SysNote = reason,
                Type = (int)HJD.HotelManagementCenter.Domain.SettleType.UserAddPay
            });

            //不发送排房短信
            HMCOrderService.UpdateOrderSendRowRowSMSStatus(oid, false);
            Log.WriteLog("1.房券不发送排房短信oid:"+oid.ToString());

            return AddPayID;
        }

        /// <summary>
        /// 从客户端移除订单
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static OrderCancelResult DeleteOrder(CancelAuthOrderParams p)
        {
            if (p.orderid * p.userid == 0)
            {
                return new OrderCancelResult()
                {
                    success = 100,
                    Message = "缺少关键参数，无法删除！"
                };
            }

            //PackageOrderInfo orderInfo = GetPackageOrderInfo(p.orderid,p.userid);

            int affectedRowCount = HMCOrderService.DeleteOrder(p.userid, p.orderid);
            if (affectedRowCount == 1)
            {
                //移除订单缓存
                OrderHelper.RemoveOrderFromRedis(p.orderid);

                return new OrderCancelResult()
                {
                    success = 0,
                    Message = "订单删除成功！"
                };
            }
            else
            {
                return new OrderCancelResult()
                {
                    success = 200,
                    Message = "订单删除失败！"
                };
            }
        }

        public static int AddOrderContacts(OrderContactsEntity ordercontacts)
        {
            return PriceService.AddOrderContacts(ordercontacts);
        }

        public static int UpdateOrderContacts(OrderContactsEntity ordercontacts)
        {
            return PriceService.UpdateOrderContacts(ordercontacts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">出行人详细信息id</param>
        /// <param name="oid">订单id</param>
        /// <param name="orderId"></param>
        /// <param name="travelId">出行人id</param>
        /// <returns></returns>
        public static OrderContactInfoEntity GetOrderContacts(int id = 0)
        {

            OrderContactInfoEntity result = new OrderContactInfoEntity();
            OrderContactsEntity model = new OrderContactsEntity();
            model = PriceService.GetOrderContactsById(id);

            result.CardTypeList = new List<KeyValue>() { new KeyValue() { Value = "身份证", Key = "1" }, new KeyValue() { Value = "护照", Key = "2" }, new KeyValue() { Value = "户口簿", Key = "3" } };
            //, new KeyValue() { Value = "港澳通行证", Key = "4" }, new KeyValue() { Value = "台胞证", Key = "5" }, new KeyValue() { Value = "其他", Key = "10" } };


            if (model != null && model.ID > 0)
            {
                result.ID = model.ID;
                result.BirthDay = model.BirthDay <= Convert.ToDateTime("1901-01-01") ? DateTime.Now : model.BirthDay;
                result.CardNum = model.CardNum;
                result.CardType = model.CardType;
                result.CName = model.CName;
                result.Email = model.Email;
                result.EName = model.EName;
                result.ESurName = model.ESurName;
                result.PhoneNum = model.PhoneNum;
                result.UserId = model.UserId;
                result.OrderId = model.OrderId;
                result.IDCardPhoto = model.IDCardPhoto;
                result.PassportPhoto = model.PassportPhoto;
                result.HeadTip = "需提供英文名、身份证和护照首页照片等信息";



                DocumentInfoEntity diIDCard = new DocumentInfoEntity();
                diIDCard.Name = "身份证电子版";
                diIDCard.Tips = "拍照上传";
                diIDCard.State = string.IsNullOrWhiteSpace(model.IDCardPhoto) ? 0 : 1;//
                diIDCard.StateName = string.IsNullOrWhiteSpace(model.IDCardPhoto) ? "待上传" : "已上传";

                diIDCard.BouncedTip = "提供正反面电子版；\n如是学生、学龄前儿童未办理身份证，请提供户口薄首页基本人信息页电子版或集体户口本人信息页电子版替代。";
                diIDCard.BouncedTipNote = "注意：请尽量拍身份证原件或上传身份证原件图";
                diIDCard.PreviewSampleList = new List<PicInfoEntity>();
                PicInfoEntity kv1 = new PicInfoEntity();
                kv1.Url = "http://whfront.b0.upaiyun.com/app/img/hotel/book/paper-sample-id.png";
                kv1.Desc = "请确保身份证清晰可见";
                kv1.Name = "身份证示例";
                diIDCard.PreviewSampleList.Add(kv1);
                PicInfoEntity kv2 = new PicInfoEntity();
                kv2.Url = "http://whfront.b0.upaiyun.com/app/img/hotel/book/paper-sample-reg.png";
                kv2.Desc = "请确保户口簿清晰可见";
                kv2.Name = "户口簿示例";
                diIDCard.PreviewSampleList.Add(kv2);
                diIDCard.TypeName = "IDCardPhoto";

                diIDCard.PhotoList = new List<PicInfoEntity>();
                foreach (string item in model.IDCardPhoto.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    PicInfoEntity p = new PicInfoEntity();
                    p.Name = item;
                    p.Desc = PhotoAdapter.GenHotelPicUrl(item, Enums.AppPhotoSize.jupiter);
                    p.Url = PhotoAdapter.GenHotelPicUrl(item, Enums.AppPhotoSize.small);
                    diIDCard.PhotoList.Add(p);
                }
                result.DocumentIfonList.Add(diIDCard);

                DocumentInfoEntity diPassport = new DocumentInfoEntity();
                diPassport.Name = "中国大陆因私护照首页电子版";
                diPassport.Tips = "拍照上传";
                diPassport.State = string.IsNullOrWhiteSpace(model.PassportPhoto) ? 0 : 1;
                diPassport.StateName = string.IsNullOrWhiteSpace(model.PassportPhoto) ? "待上传" : "已上传";
                diPassport.TypeName = "PassportPhoto";//表示护照
                diPassport.BouncedTip = "1、行程结束后至少还有6个月有效期；\n2、护照至少留有2页空白签证页，不含备注页；\n3、不接受有6个月内有日本拒签记录的护照；";
                diPassport.BouncedTipNote = "注意：请尽量直接拍护照原件或者上传护照原件图";

                diPassport.PreviewSampleList = new List<PicInfoEntity>();
                diPassport.PhotoList = new List<PicInfoEntity>();
                PicInfoEntity kv3 = new PicInfoEntity();
                kv3.Url = "http://whfront.b0.upaiyun.com/app/img/hotel/book/paper-sample-passport.png";
                kv3.Desc = "请确保护照清晰可见";
                kv3.Name = "护照示例";
                diPassport.PreviewSampleList.Add(kv3);
                foreach (string item in model.PassportPhoto.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    PicInfoEntity p = new PicInfoEntity();
                    p.Name = item;
                    p.Desc = PhotoAdapter.GenHotelPicUrl(item, Enums.AppPhotoSize.jupiter);
                    p.Url = PhotoAdapter.GenHotelPicUrl(item, Enums.AppPhotoSize.small);
                    diPassport.PhotoList.Add(p);
                }
                result.DocumentIfonList.Add(diPassport);
            }
            return result;
        }

        public static List<OrderContactInfoEntity> GetOrderContactList(long orderId)
        {
            List<OrderContactInfoEntity> resultList = new List<OrderContactInfoEntity>();
            List<OrderContactsEntity> modelList = new List<OrderContactsEntity>();
            modelList = PriceService.GetOrderContactsByOrderId(orderId);

            foreach (OrderContactsEntity model in modelList)
            {
                OrderContactInfoEntity result = new OrderContactInfoEntity();
                //result.IDCardPhotoTip = "资料待上传";
                //result.PassporPhotoTip = "资料待上传";
                if (model != null && model.ID > 0)
                {
                    result.ID = model.ID;
                    result.BirthDay = model.BirthDay;
                    result.CardNum = model.CardNum;
                    result.CardType = model.CardType;
                    result.PhoneNum = model.PhoneNum;
                    result.UserId = model.UserId;
                    result.OrderId = model.OrderId;
                    result.IDCardPhoto = model.IDCardPhoto;
                    result.PassportPhoto = model.PassportPhoto;
                    result.CName = model.CName;
                    result.Email = model.Email;
                    result.EName = model.EName;
                    result.ESurName = model.ESurName;

                    DocumentInfoEntity diIDCard = new DocumentInfoEntity();
                    diIDCard.Name = "身份证电子版";
                    diIDCard.Tips = "拍照上传";
                    diIDCard.State = string.IsNullOrWhiteSpace(model.IDCardPhoto) ? 0 : 1;//
                    diIDCard.StateName = string.IsNullOrWhiteSpace(model.IDCardPhoto) ? "材料待上传" : "资料已上传";
                    diIDCard.TypeName = "IDCardPhoto";//表示身份证
                    diIDCard.PreviewSampleList = new List<PicInfoEntity>();
                    PicInfoEntity kv1 = new PicInfoEntity();
                    kv1.Url = "http://whfront.b0.upaiyun.com/app/img/hotel/book/paper-sample-id.png";
                    kv1.Desc = "请确保身份证清晰可见";
                    kv1.Name = "身份证示例";
                    diIDCard.PreviewSampleList.Add(kv1);
                    PicInfoEntity kv2 = new PicInfoEntity();
                    kv2.Url = "http://whfront.b0.upaiyun.com/app/img/hotel/book/paper-sample-reg.png";
                    kv2.Desc = "请确保户口簿清晰可见";
                    kv2.Name = "户口簿示例";
                    diIDCard.PreviewSampleList.Add(kv2);

                    diIDCard.PhotoList = new List<PicInfoEntity>();
                    foreach (string item in model.IDCardPhoto.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList())
                    {
                        PicInfoEntity p = new PicInfoEntity();
                        p.Name = item;
                        p.Desc = PhotoAdapter.GenHotelPicUrl(item, Enums.AppPhotoSize.jupiter);
                        p.Url = PhotoAdapter.GenHotelPicUrl(item, Enums.AppPhotoSize.small);
                        diIDCard.PhotoList.Add(p);
                    }
                    result.DocumentIfonList.Add(diIDCard);

                    DocumentInfoEntity diPassport = new DocumentInfoEntity();
                    diPassport.Name = "中国大陆因私护照首页电子版";
                    diPassport.Tips = "拍照上传";
                    diPassport.State = string.IsNullOrWhiteSpace(model.PassportPhoto) ? 0 : 1;
                    diPassport.StateName = string.IsNullOrWhiteSpace(model.PassportPhoto) ? "材料待上传" : "资料已上传";
                    diPassport.TypeName = "PassportPhoto";//表示身份证
                    diPassport.PreviewSampleList = new List<PicInfoEntity>();
                    diPassport.PhotoList = new List<PicInfoEntity>();
                    PicInfoEntity kv3 = new PicInfoEntity();
                    kv3.Url = "http://whfront.b0.upaiyun.com/app/img/hotel/book/paper-sample-passport.png";
                    kv3.Desc = "请确保护照清晰可见";
                    kv3.Name = "护照示例";
                    diPassport.PreviewSampleList.Add(kv3);
                    foreach (string item in model.PassportPhoto.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList())
                    {
                        PicInfoEntity p = new PicInfoEntity();
                        p.Name = item;
                        p.Desc = PhotoAdapter.GenHotelPicUrl(item, Enums.AppPhotoSize.jupiter);
                        p.Url = PhotoAdapter.GenHotelPicUrl(item, Enums.AppPhotoSize.small);
                        diPassport.PhotoList.Add(p);
                    }

                    result.DocumentTip = (diPassport.State == 1 && diIDCard.State == 1) ? "资料已上传" : "材料待上传";
                    result.DocumentTipState = (diPassport.State == 1 && diIDCard.State == 1) ? 1 : 0;
                    result.DocumentIfonList.Add(diPassport);
                }
                resultList.Add(result);
            }
            return resultList;
        }

        #region 订单状态处理
        /// <summary>
        /// 为现付订单生成特别的状态名称
        /// </summary>
        /// <param name="orderState"></param>
        /// <param name="orderStateName"></param>
        /// <returns></returns>
        private static string genPayAsGoStateName(int orderState, string orderStateName)
        {
            switch (orderState)
            {
                case 1:
                    return "已提交待确认";
                case 2:
                    return "已提交待确认";
                case 10:
                    return "已提交待确认";
                case 102:
                    return "待补汇款";
                default:
                    return orderStateName;
            }
        }
        #endregion

        #region 生成订单支付渠道数组 和 支付标签图标链接数组  详情页操作按钮

        /// <summary>
        /// 生成订单支付渠道数组
        /// </summary>
        /// <param name="customerType"></param>
        /// <param name="payType"></param>
        /// <returns></returns>
        public static List<string> genOrderPayChannels(HJDAPI.Common.Helpers.Enums.CustomerType customerType, int payType, string phone)
        {
            List<string> channels = new List<string>();
            switch (customerType)
            {
                case HJDAPI.Common.Helpers.Enums.CustomerType.botao:
                    channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.botao.ToString());
                    break;
                default:
                    switch (payType)
                    {
                        case 1:
                        case 4:
                            break;
                        case 2:
                            channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.chinapay.ToString());
                            channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.alipay.ToString());
                            channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.tenpay.ToString());
                            //    channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.cmbpay.ToString());
                            //  channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.upay.ToString());
                            break;
                        default:
                            channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.chinapay.ToString());
                            channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.tenpay.ToString());
                            channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.alipay.ToString());
                            //   channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.cmbpay.ToString());
                            //  channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.upay.ToString());
                            break;
                    }
                    break;
            }

            //支付测试CMB
            if (phone == "18021036971")
            {
                channels.Add(HJDAPI.Common.Helpers.Enums.PayChannelType.cmbpay.ToString());
            }
            return channels;
        }

        /// <summary>
        /// 生成订单PayType显示标签图片
        /// </summary>
        /// <param name="customerType"></param>
        /// <param name="payType"></param>
        /// <returns></returns>
        public static List<string> genOrderPayLabelUrls(HJDAPI.Common.Helpers.Enums.CustomerType customerType, int payType, int cancelPolicy = 0, bool ForVIPFirstBuy = false, int packageType = 1, int dayLimitMin = 0)
        {
            List<string> payLabels = new List<string>();

            if (dayLimitMin > 1)
            {
                payLabels.Add("http://whfront.b0.upaiyun.com/app/img/icon/more_day_night.png");
            }

            if (ForVIPFirstBuy)
            {
                if (packageType != 1)
                {
                    payLabels.Add("http://whfront.b0.upaiyun.com/app/img/icon/new_vip.png");
                }
                payLabels.Add("http://whfront.b0.upaiyun.com/app/img/icon/one_buy.png");
            }
            //支付方式
            switch (customerType)
            {
                case HJDAPI.Common.Helpers.Enums.CustomerType.botao:
                    payLabels.Add(PhotoAdapter.GenHotelPicUrl(BotaoPayLabelSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter));
                    break;
                default:
                    switch (payType)
                    {
                        case 1:
                            payLabels.Add("http://whfront.b0.upaiyun.com/app/img/icon/xf_pay.png");
                            //payLabels.Add(PhotoAdapter.GenHotelPicUrl(CashPayPayLabelSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter));
                            break;
                        case 2:
                            payLabels.Add("http://whfront.b0.upaiyun.com/app/img/icon/db_pay.png");//担保支付显示预付标签
                            //payLabels.Add(PhotoAdapter.GenHotelPicUrl(PrePayLabelSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter));//担保支付显示预付标签
                            break;
                        case 3:
                            payLabels.Add("http://whfront.b0.upaiyun.com/app/img/icon/prepay.png");
                            //payLabels.Add(PhotoAdapter.GenHotelPicUrl(PrePayLabelSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter));
                            break;
                        default:
                            break;
                    }
                    break;
            }
            //取消政策
            switch (cancelPolicy)
            {
                case 1:
                    payLabels.Add("http://whfront.b0.upaiyun.com/app/img/icon/cant_cancel.png");//不可取消
                    //payLabels.Add(PhotoAdapter.GenHotelPicUrl(NoCancelLabelSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter));
                    break;
                case 2:
                    payLabels.Add("http://whfront.b0.upaiyun.com/app/img/icon/time_cancel.png");//限时取消
                    //payLabels.Add(PhotoAdapter.GenHotelPicUrl(LimitCancelLabelSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter));
                    break;
                default: break;
            }

            return payLabels;
        }

        /// <summary>
        /// 输出订单可以操作的按钮
        /// </summary>
        /// <param name="orderState"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static List<string> genOrderOptionBtns(int orderState, int payType, int commentId, DateTime checkIn, decimal orderAddPayPrice = 0, ContextBasicInfo contextBasicInfo = null)
        {
            List<string> canShowBtns = new List<string>();
            if (orderState == 1 || orderState == 3)
            {
                canShowBtns.Add(HJDAPI.Common.Helpers.Enums.OrderOptionButton.pay.ToString());
                if (contextBasicInfo != null && contextBasicInfo.IsThanVer6_2)
                {
                    canShowBtns.Add(HJDAPI.Common.Helpers.Enums.OrderOptionButton.cancelpay.ToString());
                }
            }
            if (contextBasicInfo != null && contextBasicInfo.IsThanVer6_2)
            {
                if (orderState == 101 || (payType == 1 && orderState == 10))
                {
                    canShowBtns.Add(HJDAPI.Common.Helpers.Enums.OrderOptionButton.cancel.ToString());
                }
            }
            else
            {
                if (orderState == 1 || orderState == 3 || orderState == 101 || (payType == 1 && orderState == 10))
                {
                    canShowBtns.Add(HJDAPI.Common.Helpers.Enums.OrderOptionButton.cancel.ToString());
                }
            }
            if (checkIn <= DateTime.Now)
            {
                canShowBtns.Add(HJDAPI.Common.Helpers.Enums.OrderOptionButton.delete.ToString());
            }
            if (orderAddPayPrice > 0)
            {
                canShowBtns.Add(HJDAPI.Common.Helpers.Enums.OrderOptionButton.addpay.ToString());
            }
            if (orderState != 12 || checkIn > DateTime.Now) return canShowBtns;
            if (commentId == 0)
            {
                canShowBtns.Add(HJDAPI.Common.Helpers.Enums.OrderOptionButton.writecomment.ToString());
            }
            return canShowBtns;
        }
        #endregion

        #region 铂涛结算的问题

        public static BoTaoOrderEntity GetBoTaoOrderInfo(long orderId)
        {
            BoTaoOrderEntity result = null;
            if (orderId <= 0)
            {
                result = new BoTaoOrderEntity();
            }
            else if (orderId <= 1000000)
            {
                var couponOrder = CouponAdapter.GetOneRoomCouponOrderEntityByPayID((int)orderId);
                var exchangeCouponList = couponOrder.ExchangeCouponList;
                var firstExchangeCoupon = exchangeCouponList.First();

                var couponActivity = CouponAdapter.GetCouponActivityDetail(firstExchangeCoupon.ActivityID, false);

                result = new BoTaoOrderEntity()
                {
                    amount = (int)firstExchangeCoupon.Price,//exchangeCouponList.Sum(_=>_.Price),
                    begintime = DateTime.MinValue,
                    endtime = DateTime.MinValue,
                    goodsname = string.Format("{0}({1})", couponActivity.package.HotelName, couponActivity.package.PackageBrief),
                    hotelname = couponOrder.HotelName,
                    indate = firstExchangeCoupon.ExpireTime,
                    mans = 2,
                    packagename = couponOrder.PackageName,
                    packagepic = couponActivity.activity.PicPath,
                    price = firstExchangeCoupon.Price,
                    qty = exchangeCouponList.Count,
                    merchantorderno = string.Join(",", exchangeCouponList.Select(_ => "couponorder_" + _.ID)),
                    guestname = "手机用户" + firstExchangeCoupon.PhoneNum.Substring(0, 3) + "****" + firstExchangeCoupon.PhoneNum.Substring(7, 4),
                    mobilePhone = firstExchangeCoupon.PhoneNum
                };
            }
            else
            {
                var hotelOrder = OrderAdapter.GetPackageOrderInfo20(orderId, 0);
                var packageId = hotelOrder.PID;
                var hotelId = hotelOrder.HotelID;

                var packageEntity = HotelAdapter.HMC_PackageService.GetPackage(packageId, hotelId, -1).FirstOrDefault();

                var hps = HotelAdapter.GetHotelPhotos(hotelId, 0);
                var picUrl = hps.HPList.OrderByDescending(_ => _.IsCover).First().URL;//_appview

                result = new BoTaoOrderEntity()
                {
                    amount = (int)hotelOrder.Amount,//exchangeCouponList.Sum(_=>_.Price),
                    begintime = hotelOrder.CheckIn,
                    endtime = hotelOrder.CheckIn.AddDays(hotelOrder.NightCount).Date,
                    goodsname = string.Format("{0}({1}晚{2}间 {3})", hotelOrder.HotelName, hotelOrder.NightCount, hotelOrder.RoomCount, packageEntity != null ? packageEntity.Brief : hotelOrder.RoomDescription),
                    hotelname = hotelOrder.HotelName,
                    indate = null,
                    mans = hotelOrder.RoomCount,
                    packagename = hotelOrder.PackageName,
                    packagepic = picUrl,
                    price = Math.Round(hotelOrder.Amount / (hotelOrder.NightCount * hotelOrder.RoomCount), 0),
                    qty = hotelOrder.RoomCount,
                    merchantorderno = "hotelorder_" + orderId.ToString(),
                    guestname = hotelOrder.Contact,
                    mobilePhone = hotelOrder.ContactPhone
                };
            }
            return result;
        }

        public static BoTaoResponse ReConfirmBotaoOrder(BotaoSettleOrderEntity hotel, bool isDevEnv = false)
        {
            var orderId = hotel.OrderID;
            var botaoOrderNo = string.Format("hotelorder_{0}", orderId);
            var cancelResult = CancelBotaoOrder(botaoOrderNo, isDevEnv);

            if ((cancelResult.result && cancelResult.errorcode.Equals("0")) || cancelResult.errorcode.Equals("20008"))
            {
                var payToken = hotel.PayToken.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var code = payToken[0];
                var ticket = payToken[1];

                var botaoOrderInfo = GetBoTaoOrderInfo(orderId);
                var param = new BoTaoBookParam()
                {
                    amount = (decimal)(hotel.Price / 100),
                    begintime = botaoOrderInfo.begintime.ToString("yyyy").Equals("0001") ? "" : botaoOrderInfo.begintime.ToString("yyyy-MM-dd hh:mm:ss"),
                    booktime = DateTime.Now,
                    checkpaycode = true,
                    consumetype = orderId < 1000000 ? ThirdPartyProductType.roomcoupon : ThirdPartyProductType.hotel,
                    endtime = botaoOrderInfo.endtime.ToString("yyyy").Equals("0001") ? "" : botaoOrderInfo.endtime.ToString("yyyy-MM-dd hh:mm:ss"),
                    goodsname = botaoOrderInfo.goodsname,
                    hotelname = botaoOrderInfo.hotelname,
                    indate = botaoOrderInfo.indate == null ? "" : ((DateTime)botaoOrderInfo.indate).ToString("yyyy-MM-dd hh:mm:ss"),
                    mans = botaoOrderInfo.mans,
                    merchantcode = HJDAPI.Common.Helpers.Enums.ThirdPartyMerchantCode.zmhotel.ToString(),
                    merchantorderno = botaoOrderInfo.merchantorderno,
                    mobile = botaoOrderInfo.mobilePhone,
                    packagename = botaoOrderInfo.packagename,
                    packagepic = botaoOrderInfo.packagepic,
                    paymsgcode = code,
                    paymsgticket = ticket,
                    price = botaoOrderInfo.price,
                    qty = botaoOrderInfo.qty,
                    guestname = botaoOrderInfo.guestname,
                    sign = "",
                    mebphone = botaoOrderInfo.mobilePhone
                };

                string botaoAPIUrl = isDevEnv ? "http://www.dreamstech.cn:9105/consume/book" : string.Format("{0}consume/book", BoTaoAPI);
                var bookResult = PreBook(param, botaoAPIUrl);
                if ((bookResult.result && bookResult.errorcode.Equals("0")) || bookResult.errorcode.Equals("20006"))
                {
                    bookResult = ConfirmBotaoOrder(botaoOrderNo, isDevEnv);
                }
                return bookResult;
            }
            return cancelResult;
        }

        private static BoTaoResponse PreBook(BoTaoBookParam param, string botaoAPIUrl)
        {
            Dictionary<string, string> dicKeyVal = new Dictionary<string, string>();
            dicKeyVal.Add("hotelname", param.hotelname);
            dicKeyVal.Add("paymsgticket", param.paymsgticket);
            dicKeyVal.Add("paymsgcode", param.paymsgcode);
            dicKeyVal.Add("checkpaycode", param.checkpaycode.ToString());
            dicKeyVal.Add("merchantcode", param.merchantcode);
            dicKeyVal.Add("merchantorderno", param.merchantorderno);
            dicKeyVal.Add("packagename", param.packagename);
            dicKeyVal.Add("packagepic", param.packagepic);
            dicKeyVal.Add("mobile", param.mobile);
            dicKeyVal.Add("amount", param.amount.ToString());
            dicKeyVal.Add("consumetype", param.consumetype.ToString("d"));
            dicKeyVal.Add("days", "0");
            dicKeyVal.Add("qty", param.qty.ToString());
            dicKeyVal.Add("mans", param.mans.ToString());
            dicKeyVal.Add("goodsid", "0");
            dicKeyVal.Add("goodsname", param.goodsname);
            dicKeyVal.Add("price", param.price.ToString());
            dicKeyVal.Add("booktime", param.booktime.ToString("yyyy-MM-dd hh:mm:ss"));
            dicKeyVal.Add("outtelephone", "4000-021-702");
            dicKeyVal.Add("guestname", param.guestname);
            dicKeyVal.Add("mebphone", param.mebphone);

            if (!string.IsNullOrWhiteSpace(param.begintime) && !string.IsNullOrWhiteSpace(param.endtime))
            {
                dicKeyVal.Add("begintime", param.begintime);
                dicKeyVal.Add("endtime", param.endtime);
            }
            else
            {
                dicKeyVal.Add("begintime", "0001-01-01 00:00:00");
                dicKeyVal.Add("endtime", "0001-01-01 00:00:00");
            }
            if (!string.IsNullOrWhiteSpace(param.indate))
            {
                dicKeyVal.Add("indate", param.indate);
            }
            else
            {
                dicKeyVal.Add("indate", "0001-01-01 00:00:00");
            }

            dicKeyVal.Add("timestamp", HJDAPI.Common.Security.Signature.GenSince1970_01_01_00_00_00Seconds().ToString());
            dicKeyVal.Add("noncestr", DescriptionHelper.GenAssignLengthRandomStr(6));
            dicKeyVal.Add("extrefnumber", Guid.NewGuid().ToString());

            var sign = Signature.CalculateBoTaoSign(dicKeyVal.Where(_ => !string.IsNullOrWhiteSpace(_.Value)).ToDictionary(_ => _.Key, _ => _.Value), true, BoTaoSignKey);

            System.Net.CookieContainer cc = new System.Net.CookieContainer();

            var postBodyParam = "";
            dicKeyVal.Add("apikey", BoTaoAPIKey);
            dicKeyVal.Add("sign", sign);
            foreach (var item in dicKeyVal)
            {
                if (true)
                {
                    postBodyParam += "&" + item.Key + "=" + System.Web.HttpUtility.UrlEncode(item.Value);
                }
                else
                {
                    postBodyParam += "&" + item.Key + "=" + item.Value;
                }
            }
            postBodyParam = postBodyParam.Trim("&".ToCharArray());

            string json = HttpRequestHelper.Post(botaoAPIUrl, postBodyParam, ref cc);
            return JsonConvert.DeserializeObject<BoTaoResponse>(json);
        }

        public static BoTaoResponse CancelBotaoOrder(string orderId4Botao, bool isDevEnv = false)
        {
            string url = isDevEnv ? "http://www.dreamstech.cn:9105/consume/cancelbook" : string.Format("{0}consume/cancelbook", BoTaoAPI);

            Dictionary<string, string> dicKeyVal = new Dictionary<string, string>();
            dicKeyVal.Add("merchantcode", "zmhotel");
            dicKeyVal.Add("merchantorderno", orderId4Botao);

            dicKeyVal.Add("timestamp", HJDAPI.Common.Security.Signature.GenSince1970_01_01_00_00_00Seconds().ToString());
            dicKeyVal.Add("noncestr", "zxc234ed12");
            dicKeyVal.Add("extrefnumber", Guid.NewGuid().ToString());

            var sign = Signature.CalculateBoTaoSign(dicKeyVal, true, BoTaoSignKey);
            System.Net.CookieContainer cc = new System.Net.CookieContainer();

            var postBodyParam = "";

            dicKeyVal.Add("apikey", BoTaoAPIKey);
            dicKeyVal.Add("sign", sign);
            foreach (var item in dicKeyVal)
            {
                postBodyParam += "&" + item.Key + "=" + item.Value;
            }
            postBodyParam = postBodyParam.Trim("&".ToCharArray());
            string json = "";
            try
            {
                json = HttpRequestHelper.Post(url, postBodyParam, ref cc);
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message + " " + ex.StackTrace + " " + url + " " + postBodyParam);
            }
            return JsonConvert.DeserializeObject<BoTaoResponse>(json);
        }

        public static BoTaoResponse ConfirmBotaoOrder(string orderId4Botao, bool isDevEnv = false)
        {
            string url = isDevEnv ? "http://www.dreamstech.cn:9105/consume/confirmconsume" : string.Format("{0}consume/confirmconsume", BoTaoAPI);

            Dictionary<string, string> dicKeyVal = new Dictionary<string, string>();

            dicKeyVal.Add("merchantcode", "zmhotel");
            dicKeyVal.Add("merchantorderno", orderId4Botao);
            dicKeyVal.Add("days", "1");
            dicKeyVal.Add("qty", "1");
            dicKeyVal.Add("mans", "2");
            dicKeyVal.Add("goodsid", "0");
            dicKeyVal.Add("goodsname", "周末酒店");
            dicKeyVal.Add("price", "100");
            dicKeyVal.Add("begintime", DateTime.Now.AddDays(7).ToString("yyyy-MM-dd hh:mm:ss"));
            dicKeyVal.Add("endtime", DateTime.Now.AddDays(8).ToString("yyyy-MM-dd hh:mm:ss"));

            dicKeyVal.Add("timestamp", HJDAPI.Common.Security.Signature.GenSince1970_01_01_00_00_00Seconds().ToString());
            dicKeyVal.Add("noncestr", "zxc234ed12");
            dicKeyVal.Add("extrefnumber", Guid.NewGuid().ToString());

            var sign = Signature.CalculateBoTaoSign(dicKeyVal, true, BoTaoSignKey);
            System.Net.CookieContainer cc = new System.Net.CookieContainer();

            var postBodyParam = "";
            dicKeyVal.Add("apikey", BoTaoAPIKey);
            dicKeyVal.Add("sign", sign);
            foreach (var item in dicKeyVal)
            {
                postBodyParam += "&" + item.Key + "=" + System.Web.HttpUtility.UrlEncode(item.Value);
            }
            postBodyParam = postBodyParam.Trim("&".ToCharArray());

            string json = "";
            try
            {
                json = HttpRequestHelper.Post(url, postBodyParam, ref cc);
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message + " " + ex.StackTrace + " " + url + " " + postBodyParam);
            }

            return JsonConvert.DeserializeObject<BoTaoResponse>(json);
        }

        /// <summary>
        /// 获取订单对应的销售数据
        /// </summary>
        /// <param name="hotelid">酒店</param>
        /// <param name="supplierid">供应商ID</param>
        /// <param name="pid">订单套餐ID</param>
        /// <returns></returns>
        public static string GetOrderRelWHSaleInfo(int hotelid, int supplierid, int pid)
        {
            string RelSale = string.Empty;
            if (pid > 0)
            {
                var package = PackageAdapter.GetOnePackageEntity(pid);
                if (package != null && package.RelSales > 0)
                {
                    RelSale = AccountAdapter.GetUserInfo(package.RelSales).Email.Split('@')[0];
                }
            }
            if (string.IsNullOrEmpty(RelSale))
            {
                if (supplierid > 0)
                {
                    HJD.HotelManagementCenter.Domain.RoomSupplierEntity supplier = SupplierService.GetRoomSupplierByID(supplierid);
                    if (supplier != null)
                    {
                        RelSale = supplier.RelWHSales == null ? "" : ((supplier.RelWHSales.ToLower() == "ylxu" || supplier.RelWHSales.Trim() == "*") ? "" : supplier.RelWHSales);
                    }
                }
                if (string.IsNullOrEmpty(RelSale))
                {
                    HJD.HotelManagementCenter.Domain.HotelContactsEntity hotelcontact = PackageService.GetOneHotelContactsEntity(hotelid);
                    if (hotelcontact != null)
                    {
                        RelSale = hotelcontact.RelWHSales == null ? "" : (hotelcontact.RelWHSales == "*" ? "" : hotelcontact.RelWHSales);

                    }
                    if (string.IsNullOrEmpty(RelSale))
                    {
                        int DistrictID = HotelAdapter.GetHotelDistrictID(hotelid);
                        if (hotelcontact != null && DistrictID > 0)
                        {
                            RelSale = PackageService.GetTopDistrictSales(DistrictID);
                        }
                    }
                }
            }
            return RelSale;
        }

        public static int UpdateOrderRelWHSales(long oid, string RelWHSales)
        {
            try
            {
                return HMCOrderService.UpdateOrderRelWHSales(oid, RelWHSales);
            }
            catch
            {
                return 1;
            }
        }
        #endregion

        //public static string UpdateOrderState()
        //{
        //    redis.StringSet("test", "order");
        //    return "";
        //}
        /// <summary>
        /// 取消超时订单
        /// </summary>
        /// <returns></returns>
        public static bool CancelOverTimeOrder()
        {
            try
            {
                HJD.HotelManagementCenter.Domain.OrdedrQueryParam orderQueryParam = new HJD.HotelManagementCenter.Domain.OrdedrQueryParam();
                orderQueryParam.pageIndex = 1;
                orderQueryParam.pageSize = 1000;
                orderQueryParam.orderState = 1;
                orderQueryParam.submitStartDate = Convert.ToDateTime("2000-01-01");
                orderQueryParam.submitEndDate = DateTime.Now.AddHours(-72);
                var orderList = HMCOrderService.QueryOrder(orderQueryParam);
                foreach (var item in orderList)
                {
                    HJD.HotelManagementCenter.Domain.OrdedrUpdateParam orderUpdateParam = new HJD.HotelManagementCenter.Domain.OrdedrUpdateParam();
                    orderUpdateParam.id = item.ID;
                    orderQueryParam.orderState = 4;
                    HMCOrderService.UpdateOrderState(orderUpdateParam);
                    OrderHelper.DeleteUserPackageOrderRelRedisData(item.UserID, new List<long> { item.OrderID });
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog("取消超时订单失败：" + ex.Message + ex.StackTrace);
                return false;
            }
        }
    }
}