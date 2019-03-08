using HJD.HotelManagementCenter.Domain.Settlement;
using HJD.HotelPrice.Contract;
using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Common.Security;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Cache;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using HJDAPI.Models.RequestParams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace HJDAPI.Controllers
{
    public class OrderController : BaseApiController
    {

        public static string LiuWaIcon = "http://whfront.b0.upaiyun.com/app/img/me/icon-kids.png";
        public static string HotelOrderIcon = "http://whfront.b0.upaiyun.com/app/img/me/icon-hotel.png";
        public static string FoodCouponIcon = "http://whfront.b0.upaiyun.com/app/img/me/icon-foods.png";
        public static string playCouponIcon = "http://whfront.b0.upaiyun.com/app/img/me/icon-play.png";
        public static string RoomCouponIcon = "http://whfront.b0.upaiyun.com/app/img/me/icon-room.png";
        public static string JiJiuOrderIcon = "http://whfront.b0.upaiyun.com/app/img/me/icon-airtrip.png";
        public static string YouLunOrderIcon = "http://whfront.b0.upaiyun.com/app/img/me/icon-liner.png";
        [HttpPost]
        /// </summary>
        public int AddCommOrders(HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity commorders)
        {
            return OrderAdapter.AddCommOrders(commorders);
        }

        [HttpPost]
        public HJD.HotelPrice.Contract.DataContract.Order.CommOrderEntity GetCommOrder(int OrderID)
        {
            return OrderAdapter.GetCommOrder(OrderID);
        }

        [HttpPost]
        public OrderCancelResult CancelAuthOrder(CancelAuthOrderParams p)
        {
            return OrderAdapter.CancelAuthOrder(p.orderid, p.userid);
        }

        [HttpPost]
        public OrderCancelResult CancelAuthOrder40(CancelAuthOrderParams p)
        {
            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                return OrderAdapter.CancelAuthOrder(p.orderid, p.userid);
            }
            else
            {
                return new OrderCancelResult { Message = "签名错误！", success = 100 };
            }
        }

        [HttpPost]
        public OrderSubmitResult SubmitOrder(OrderEntity order)
        {
            OrderSubmitResult result = OrderAdapter.SubmitOrder(order, AppVer);
            return result;
        }

        [HttpPost]
        public OrderSubmitResult SubmitOrderV42(OrderEntity order)
        {
            OrderSubmitResult result = OrderAdapter.SubmitOrder(order, "4.2");
            return result;
        }

        /// <summary>
        /// PayType=1 现付订单 ToDo不再输出现金券分享
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        public OrderSubmitResult SubmitOrder40(OrderEntity order)
        {
            OrderSubmitResult result = new OrderSubmitResult();
            if (Signature.IsRightSignature(order.TimeStamp, order.SourceID, order.RequestType, order.Sign))
            {
                order.package.Contact = DES.Decrypt(order.package.Contact);
                order.package.ContactPhone = DES.Decrypt(order.package.ContactPhone);
                if (order.invoiceInfo != null)
                {
                    order.invoiceInfo.Contact = DES.Decrypt(order.invoiceInfo.Contact);
                    order.invoiceInfo.Title = DES.Decrypt(order.invoiceInfo.Title);
                    order.invoiceInfo.Address = DES.Decrypt(order.invoiceInfo.Address);
                    order.invoiceInfo.TelPhone = order.invoiceInfo.TelPhone != null ? DES.Decrypt(order.invoiceInfo.TelPhone) : "";
                    order.invoiceInfo.TaxNumber = !string.IsNullOrWhiteSpace(order.invoiceInfo.TaxNumber) ? DES.Decrypt(order.invoiceInfo.TaxNumber) : "";
                }
                result = OrderAdapter.SubmitOrder(order, AppVer);
            }
            else
            {
                result.ErrorCode = HJD.HotelPrice.Contract.OrderErrorCode.ORDER_ERROR_SIGN;
                result.ErrorMessage = "签名错误！";
            }

            return result;
        }

        public long BGSubimitOrder(OrderEntity order)
        {
            return OrderAdapter.BGSubimitOrder(order);
        }

        /// <summary>
        /// 判断用户是否有权限购买此套餐
        /// </summary>
        /// <param name="pid">套餐id</param>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        [HttpGet]
        public CheckSubmitOrderResonse CheckSubmitOrderBefore(int pid, long userid, int packageType = 1)
        {
            return OrderAdapter.CheckSubmitOrderBefore(pid, userid, packageType);
        }


        /// <summary>
        /// 添加发票信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public BasePostResult AddInvoiceInfo(GetInvoiceParams param)
        {

            //OrderAdapter.AddInvoiceInfo(new GetInvoiceParams().invoice, new GetInvoiceParams().orderid);
            //return new BasePostResult()
            //        {
            //            Success = true,
            //            Message = "添加成功"
            //        };
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                if (AppType.ToLower().Contains("android") && (string.IsNullOrWhiteSpace(AppVer) || AppVer.CompareTo("4.6.2") == 0))
                {
                    if (OrderAdapter.AddInvoiceInfo(param.invoice, param.orderid))
                    {
                        return new BasePostResult()
                        {
                            Success = true,
                            Message = "添加成功"
                        };
                    }
                    else
                    {
                        return new BasePostResult()
                        {
                            Success = false,
                            Message = "添加失败"
                        };
                    }
                }
                else
                {
                    param.invoice.Contact = DES.Decrypt(param.invoice.Contact);
                    param.invoice.Title = DES.Decrypt(param.invoice.Title);
                    param.invoice.Address = DES.Decrypt(param.invoice.Address);
                    param.invoice.TelPhone = DES.Decrypt(param.invoice.TelPhone);
                    param.invoice.TaxNumber = !string.IsNullOrWhiteSpace(param.invoice.TaxNumber) ? DES.Decrypt(param.invoice.TaxNumber) : "";
                    if (OrderAdapter.AddInvoiceInfo(param.invoice, param.orderid))
                    {
                        return new BasePostResult()
                        {
                            Success = true,
                            Message = "添加成功"
                        };
                    }
                    else
                    {
                        return new BasePostResult()
                        {
                            Success = false,
                            Message = "添加失败"
                        };
                    }
                }

            }
            else
            {
                return new BasePostResult()
                {
                    Success = false,
                    Message = "验证签名错误"
                };
            }
        }


        /// <summary>
        /// 添加发票 6.3版本后开始使用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponseEx AddInvoiceInfoNew(GetInvoiceParams param)
        {
            return OrderAdapter.AddInvoiceInfo_Ver6_3(param);
        }

        [HttpPost]
        public CheckOrderBeforePayResponse CheckOrderBeforePayWXApp(CheckOrderBeforePayRequestParams p)
        {
            CheckOrderBeforePayResponse r = new CheckOrderBeforePayResponse();

            r = OrderAdapter.CheckOrderBeforePay(p.OrderID, "6.0");

            return r;
        }

        /// <summary>
        /// 订单支付前判断
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public CheckOrderBeforePayResponse CheckOrderBeforePay(CheckOrderBeforePayRequestParams p)
        {
            CheckOrderBeforePayResponse r = new CheckOrderBeforePayResponse();

            //if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            //{
            //    r = OrderAdapter.CheckOrderBeforePay(p.OrderID, string.IsNullOrWhiteSpace(p.AppVer) ? AppVer : p.AppVer);
            //}
            //else
            //{
            //    r.SignError();
            //}

            r = OrderAdapter.CheckOrderBeforePay(p.OrderID, string.IsNullOrWhiteSpace(p.AppVer) ? AppVer : p.AppVer);

            return r;
        }

        [HttpPost]
        public GetUserRebateListResponse GetUserRebateList(GetUserRebateListRequestParams p)
        {
            GetUserRebateListResponse r = new GetUserRebateListResponse();

            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                r.RebateList = OrderAdapter.GetUserRebateList(p.userid).FindAll(i => i.RebateState != 3 && i.Rebate > 0).ToList();
                r.WaitingRebateAmount = (int)r.RebateList.Where(b => b.RebateState != 2).Sum(b => b.Rebate);
                r.HadRebateAmount = (int)r.RebateList.Where(b => b.RebateState == 2).Sum(b => b.Rebate);
            }
            else
            {
                r.SignError();
            }


            return r;
        }

        [HttpGet]
        public PackageOrderInfo GetPackageOrderInfo(long orderid, long userid)
        {
            return OrderAdapter.GetPackageOrderInfo(orderid, userid);
        }

        [HttpPost]
        public BaseResponse RequestOrderRebate(RequestOrderRebateRequestParams p)
        {
            BaseResponse response = new BaseResponse();
            int rebateState = 0;
            response.Success = 0;
            response.Message = "成功！";
            switch (p.type)
            {
                case 1:
                    rebateState = OrderAdapter.RequestOrderRebate(p.orderID);
                    break;
                case 2:

                    rebateState = OrderAdapter.RequestOrderActiveRebate(p.orderID);
                    break;
            }
            switch (rebateState)
            {
                case 0:
                    response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.Rebat_NoOrderID;// 100;
                    response.Message = "您的订单号不存在，请重新输入。";
                    break;
                case 1:
                    response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.Success;//0;
                    response.Message = "申请提交成功，您会在1-5个工作日内收到返现。返现金额将直接退至您的支付帐户中，请注意查收，谢谢！";
                    break;
                case 2:
                    response.Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.Rebat_CannotRepeatApply;// 102;
                    response.Message = "该订单号已申请过，无需重复申请，谢谢！";
                    break;
                case 3:
                    response.Success = BaseResponse.ResponseSuccessState.Rebat_HasRebat;// 103;
                    response.Message = "您的订单已返现，请查收。";
                    break;
                case 4:
                    response.Success = BaseResponse.ResponseSuccessState.Rebat_NotPaied;// 104;
                    response.Message = "您的订单尚末付款，请付款后再申请返现，谢谢！";
                    break;
                case 5:
                    response.Success = BaseResponse.ResponseSuccessState.Rebat_NoRebat;// 105;
                    response.Message = "您好，你输入的订单没有返现，谢谢！";
                    break;
                case 6:
                    response.Success = BaseResponse.ResponseSuccessState.Rebat_OrderNotConfirmed;// 106;
                    response.Message = "您好，您的订单尚末完成酒店确认，请确认后再申请返现，谢谢！";
                    break;
            }
            return response;
        }

        [HttpGet]
        [HttpPost]
        public bool CheckAlipaySecurity(AlipayCheckParams acp)
        {
            return OrderAdapter.CheckAlipaySecurity(acp);
        }

        [HttpGet]
        public PackageOrderInfo20 GetPackageOrderInfo20(long orderid, long userid)
        {
            return OrderAdapter.GetPackageOrderInfo20(orderid, userid);
        }

        [HttpGet]
        public PackageOrderInfo20 GetPackageOrderInfo40(long orderid, long userid, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                return OrderAdapter.GetPackageOrderInfo20(orderid, userid, AppVer, _ContextBasicInfo);
            }
            else
            {
                return new PackageOrderInfo20();
            }
        }

        [HttpGet]
        public List<OrderListItem> GetUserOrderList(long userid, int start, int count)
        {
            return OrderAdapter.GetUserOrderList(userid, start, count);
        }

        /// <summary>
        /// 获取“我的”页面的订单菜单项（目前用于6.2版本的我的页面）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<MenuItemEntity> GetOrderCountDisplayEntity(long userId)
        {
            Log.WriteLog("GetOrderCountDisplayEntity");
            List<MenuItemEntity> orderCountList = new List<MenuItemEntity>();

            //-1全部 28遛娃卡 0酒店 1机酒 2邮轮 15房券 20玩乐 14美食
            var baseActionUrl = Configs.WWWURL + "/order/allorders?userid={0}&selectedtype={1}&specifyuid=1&_newpage=1";

            MenuItemEntity menuItem = new MenuItemEntity();
            menuItem.ShowCount = 0;// userId > 0 ? CouponAdapter.GetUserCouponByCategoryParentId(userId, (int)EnumHelper.OrderType.LiuWaCouponOrder) : 0;
            menuItem.Type = (int)EnumHelper.OrderType.LiuWaCouponOrder;
            menuItem.Icon = LiuWaIcon;
            menuItem.ShowName = "遛娃卡";
            menuItem.ActionUrl = string.Format(baseActionUrl, userId, 28);
            orderCountList.Add(menuItem);

            menuItem = new MenuItemEntity();
            menuItem.ShowCount = 0;//userId > 0 ? OrderAdapter.GetUserOrderCountByPackageType((int)EnumHelper.OrderType.HotelOrder, userId) : 0;
            menuItem.Icon = HotelOrderIcon;
            menuItem.Type = (int)EnumHelper.OrderType.HotelOrder;
            menuItem.ShowName = "酒店";
            menuItem.ActionUrl = string.Format(baseActionUrl, userId, 0);
            orderCountList.Add(menuItem);

            menuItem = new MenuItemEntity();
            menuItem.ShowCount = 0;//userId > 0 ? OrderAdapter.GetUserOrderCountByPackageType((int)EnumHelper.OrderType.JiJiuHotelOrder, userId) : 0;
            menuItem.Icon = JiJiuOrderIcon;
            menuItem.Type = (int)EnumHelper.OrderType.JiJiuHotelOrder;
            menuItem.ShowName = "机酒";
            menuItem.ActionUrl = string.Format(baseActionUrl, userId, 1);
            orderCountList.Add(menuItem);

            menuItem = new MenuItemEntity();
            menuItem.ShowCount = 0;//userId > 0 ? OrderAdapter.GetUserOrderCountByPackageType((int)EnumHelper.OrderType.YouLunHotelOrder, userId) : 0;
            menuItem.Icon = YouLunOrderIcon;
            menuItem.Type = (int)EnumHelper.OrderType.YouLunHotelOrder;
            menuItem.ShowName = "邮轮";
            menuItem.ActionUrl = string.Format(baseActionUrl, userId, 2);
            orderCountList.Add(menuItem);

            menuItem = new MenuItemEntity();
            menuItem.ShowCount = 0;// userId > 0 ? CouponAdapter.GetUserCouponByCategoryParentId(userId, (int)EnumHelper.OrderType.RoomCouponOrder) : 0;
            menuItem.Type = (int)EnumHelper.OrderType.RoomCouponOrder;
            menuItem.Icon = RoomCouponIcon;
            menuItem.ShowName = "房券";
            menuItem.ActionUrl = string.Format(baseActionUrl, userId, 15);
            orderCountList.Add(menuItem);

            menuItem = new MenuItemEntity();
            menuItem.ShowCount = 0;//userId > 0 ? CouponAdapter.GetUserCouponByCategoryParentId(userId, (int)EnumHelper.OrderType.PlayCouponOrder) : 0;
            menuItem.Type = (int)EnumHelper.OrderType.PlayCouponOrder;
            menuItem.Icon = playCouponIcon;
            menuItem.ShowName = "玩乐";
            menuItem.ActionUrl = string.Format(baseActionUrl, userId, 20);
            orderCountList.Add(menuItem);

            menuItem = new MenuItemEntity();
            menuItem.ShowCount = 0;//userId > 0 ? CouponAdapter.GetUserCouponByCategoryParentId(userId, (int)EnumHelper.OrderType.FoodCouponOrder) : 0;
            menuItem.Type = (int)EnumHelper.OrderType.FoodCouponOrder;
            menuItem.Icon = FoodCouponIcon;
            menuItem.ShowName = "美食";
            menuItem.ActionUrl = string.Format(baseActionUrl, userId, 14);
            orderCountList.Add(menuItem);


            return orderCountList;
        }


        [HttpGet]
        public bool RemoveUserOrderRedisData(long userId)
        {
            OrderHelper.RemoveUserOrderRedisData(new List<long> { userId }); 

            return true;
        }


        /// <summary>
        /// 6.2版获取用户订单列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="oType">订单类型</param>
        /// <param name="start">页码</param>
        /// <param name="count">每页数据条数</param>
        /// <returns></returns>

        [HttpGet]
        public List<OrderListItemEntity> GetOrderListByUserId(long userId, EnumHelper.OrderType oType, int start = 0, int count = 10)
        {
            List<OrderListItemEntity> result = new List<OrderListItemEntity>();
            try
            {
                return OrderAdapter.GetAllOrderList(userId, oType, start, count);  
            }
            catch (Exception ex)
            {
                Log.WriteLog("获取订单列表报错：" + userId + " oType：" + oType + "  报错原因：" + ex + ex.StackTrace);
            }
            return result;
        }
        
        [HttpPost]
        public bool UpdateOrderStateFromRedis(OrderCacheParam param)
        {
            param.DetailOrderIdList.ForEach(DetailOrderID => ProductCache.RemoveUserDetailOrderCache(DetailOrderID));
             return true;
        }
        

        [HttpPost]
        public BasePostResult AddOrderContacts(OrderContactsEntity ordercontacts)
        {
            BasePostResult result = new BasePostResult();
            result.Success = true;
            result.Message = "操作成功";
            int id = 0;
            if (ordercontacts.ID > 0)
            {
                ordercontacts.UpdateTime = DateTime.Now;
                id = OrderAdapter.UpdateOrderContacts(ordercontacts);
            }
            else
            {
                ordercontacts.CreateTime = DateTime.Now;
                ordercontacts.UpdateTime = DateTime.Now;
                id =OrderAdapter.AddOrderContacts(ordercontacts);
            }
            if (id == 0)
            {
                result.Success = false;
                result.Message = "操作异常";
            }
            return result;
        }

        [HttpGet]
        public OrderContactInfoEntity GetOrderContactsById(int id)
        {
            return OrderAdapter.GetOrderContacts(id);
        }

        [HttpGet]
        public List<OrderListItem> GetUserOrderList40(long userid, int start, int count, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                return OrderAdapter.GetUserOrderList(userid, start, count);
            }
            else
            {
                return new List<OrderListItem>();
            }
        }

        [HttpPost]
        public WeixinPayReturnParam GetWeixinPayParam(WeixinPayRequestParam param)
        {
            //1.openId
            string APPID = Configs.WeiXinAPPID2;
            string WeiXinSecret = Configs.WeiXinSecret2;
            string url1 = "https://api.weixin.qq.com/sns/oauth2/access_token";
            string pars1 = string.Format("appid={0}&secret={1}&code={2}&grant_type=authorization_code", APPID, WeiXinSecret, param.code);//获得网页授权的access_token
            string json1 = HttpHelper.HttpGet(url1, pars1);
            WeixinWebAuthResult result1 = JsonConvert.DeserializeObject<WeixinWebAuthResult>(json1);
            string openId = result1.openid;

            //2.unifored
            WeixinPrePayResult result2 = WeiXinAdapter.GetWeixinUnifiedOrder(param.body, "", param.out_trade_no, param.total_fee, param.spbill_create_ip, param.notify_url, param.trade_type, openId);
            if (string.IsNullOrEmpty(result2.prepay_id))
            {
                return new WeixinPayReturnParam() { return_code = result2.return_code, return_msg = result2.return_msg, result_code = result2.result_code, err_code = result2.err_code, err_code_des = result2.err_code_des };
            }
            else
            {
                //完成接口的签名及返回签名参数
                string non_str = DescriptionHelper.GenerateRandomStr();
                int timeStamp = DescriptionHelper.getSecondCountSince19700101();
                string package = "prepay_id=" + result2.prepay_id;
                string signType = "MD5";
                string paySign = Signature.WeixinPaySignSignature(APPID, WeiXinSecret, non_str, package, signType, timeStamp);

                return new WeixinPayReturnParam() { nonceStr = non_str, package = package, signType = signType, paySign = paySign };
            }
        }

        /// <summary>
        /// 移除订单
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public OrderCancelResult DeleteOrder(CancelAuthOrderParams p)
        {
            return OrderAdapter.DeleteOrder(p);
        }

        /// <summary>
        /// 移除订单
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public OrderCancelResult DeleteOrder40(CancelAuthOrderParams p)
        {
            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                p.userid = p.userid == 0 ? AppUserID : p.userid;
                return OrderAdapter.DeleteOrder(p);
            }
            else
            {
                return new OrderCancelResult { Message = "签名错误！", success = 100 };
            }
        }

        /// <summary>
        /// 铂韬订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        public BoTaoOrderEntity GetBoTaoOrderInfo(long orderId)
        {
            return OrderAdapter.GetBoTaoOrderInfo(orderId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId4Botao"></param>
        /// <returns></returns>
        [HttpGet]
        public BoTaoResponse CancelBoTaoOrder([FromUri]string orderId4Botao, [FromUri]bool isDevEnv = false)
        {
            if (!string.IsNullOrWhiteSpace(orderId4Botao))
            {
                var result = OrderAdapter.CancelBotaoOrder(orderId4Botao, isDevEnv);
                if (result.result && result.errorcode.Equals("0"))
                {
                    var orderId = long.Parse(orderId4Botao.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]);
                    var orderInfo = OrderAdapter.GetBoTaoOrderInfo(orderId);
                    HotelAdapter.InsertBotaoSettlementRecord(new BotaoSettlementEntity()
                    {
                        orderId = orderId,
                        orderType = orderId4Botao.Contains("hotel") ? BotaoOrderType.hotel : BotaoOrderType.roomcoupon,
                        settleType = BotaoSettleType.cancel,
                        price = orderInfo.amount * 100
                    });
                }
                return result;
            }
            else
            {
                var cancelCouponList = CouponAdapter.GetNeedSettlementBotaoCouponList().FindAll(_ => _.settleType == 1);

                foreach (var coupon in cancelCouponList)
                {
                    var result = OrderAdapter.CancelBotaoOrder(string.Format("couponorder_{0}", coupon.ID), isDevEnv);
                    if (result.result && result.errorcode.Equals("0"))
                    {
                        HotelAdapter.InsertBotaoSettlementRecord(new BotaoSettlementEntity()
                        {
                            orderId = coupon.ID,
                            orderType = BotaoOrderType.roomcoupon,
                            settleType = BotaoSettleType.cancel,
                            price = (int)(coupon.Price * 100)
                        });
                    }
                }

                var cancelOrderList = HotelAdapter.GetBotaoSettleOrderList().FindAll(_ => _.SettleType == 1);

                foreach (var hotel in cancelOrderList)
                {
                    var result = OrderAdapter.CancelBotaoOrder(string.Format("hotelorder_{0}", hotel.OrderID), isDevEnv);
                    if (result.result && result.errorcode.Equals("0"))
                    {
                        HotelAdapter.InsertBotaoSettlementRecord(new BotaoSettlementEntity()
                        {
                            orderId = hotel.OrderID,
                            orderType = BotaoOrderType.hotel,
                            settleType = BotaoSettleType.cancel,
                            price = (int)hotel.Price
                        });
                    }
                }

                return new BoTaoResponse()
                {
                    data = "",
                    errorcode = "0",
                    errormsg = "没有异常 取消成功",
                    result = true,
                    ver = ""
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId4Botao"></param>
        /// <returns></returns>
        [HttpGet]
        public BoTaoResponse ConfirmBoTaoOrder([FromUri]string orderId4Botao, [FromUri]bool isDevEnv = false)
        {
            if (!string.IsNullOrWhiteSpace(orderId4Botao))
            {
                var result = OrderAdapter.ConfirmBotaoOrder(orderId4Botao, isDevEnv);
                if (result.result && result.errorcode.Equals("0"))
                {
                    var orderId = long.Parse(orderId4Botao.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]);
                    var orderInfo = OrderAdapter.GetBoTaoOrderInfo(orderId);
                    HotelAdapter.InsertBotaoSettlementRecord(new BotaoSettlementEntity()
                    {
                        orderId = orderId,
                        orderType = orderId4Botao.Contains("hotel") ? BotaoOrderType.hotel : BotaoOrderType.roomcoupon,
                        settleType = BotaoSettleType.confirm,
                        price = orderInfo.amount * 100
                    });
                }
                return result;
            }
            else
            {
                var confirmCouponList = CouponAdapter.GetNeedSettlementBotaoCouponList().FindAll(_ => _.settleType == 2);
                foreach (var coupon in confirmCouponList)
                {
                    var result = OrderAdapter.ConfirmBotaoOrder(string.Format("couponorder_{0}", coupon.ID), isDevEnv);
                    if (result.result && result.errorcode.Equals("0"))
                    {
                        HotelAdapter.InsertBotaoSettlementRecord(new BotaoSettlementEntity()
                        {
                            orderId = coupon.ID,
                            orderType = BotaoOrderType.roomcoupon,
                            settleType = BotaoSettleType.confirm,
                            price = (int)(coupon.Price * 100)
                        });
                    }
                }

                var confirmOrderList = HotelAdapter.GetBotaoSettleOrderList().FindAll(_ => _.SettleType == 2);
                foreach (var hotel in confirmOrderList)
                {
                    var result = OrderAdapter.ConfirmBotaoOrder(string.Format("hotelorder_{0}", hotel.OrderID), isDevEnv);
                    if (result.result && result.errorcode.Equals("0"))
                    {
                        HotelAdapter.InsertBotaoSettlementRecord(new BotaoSettlementEntity()
                        {
                            orderId = hotel.OrderID,
                            orderType = BotaoOrderType.hotel,
                            settleType = BotaoSettleType.confirm,
                            price = (int)hotel.Price
                        });
                    }
                }

                return new BoTaoResponse()
                {
                    data = "",
                    errorcode = "0",
                    errormsg = "没有异常 确认成功",
                    result = true,
                    ver = ""
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId4Botao"></param>
        /// <returns></returns>
        [HttpGet]
        public BoTaoResponse ReConfirmBotaoOrder([FromUri]bool isDevEnv = false)
        {
            var confirmOrderList = HotelAdapter.GetBotaoSettleOrderList().FindAll(_ => _.SettleType == 3);
            foreach (var hotel in confirmOrderList)
            {
                var result = OrderAdapter.ReConfirmBotaoOrder(hotel, isDevEnv);
                if (result.result && result.errorcode.Equals("0"))
                {
                    HotelAdapter.InsertBotaoSettlementRecord(new BotaoSettlementEntity()
                    {
                        orderId = hotel.OrderID,
                        orderType = BotaoOrderType.hotel,
                        settleType = BotaoSettleType.confirm,
                        price = (int)hotel.Price
                    });
                }
            }

            return new BoTaoResponse()
            {
                data = "",
                errorcode = "0",
                errormsg = "没有异常 重新确认扣款成功",
                result = true,
                ver = ""
            };
        }

        /// <summary>
        /// 获取订单要关联的销售
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        public string GetOrderRelWHSaleInfo(int hotelid, int supplierid)
        {
            return OrderAdapter.GetOrderRelWHSaleInfo(hotelid, supplierid, 0);
        }
        /// <summary>
        /// 修改订单要关联的销售
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="supplierid"></param>
        /// <returns></returns>
        [HttpGet]
        public int UpdateOrderRelWHSales(long oid, string RelWHSales)
        {
            return OrderAdapter.UpdateOrderRelWHSales(oid, RelWHSales);
        }
        [HttpPost]
        public bool CancelOverTimeOrder()
        {
            return OrderAdapter.CancelOverTimeOrder();
        }
        #region wx small app pay

        /// <summary>
        /// 【高端酒店特价】小程序内获取支付配置参数
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public WxAppPayConfig WxAppPay(WxAppPayRequestParam p)
        {
            WxAppPayConfig config = WeiXinAdapter.GetWxAppUnifiedOrder(p);

            //config.timeStamp = DescriptionHelper.getSecondCountSince19700101();

            //config.nonceStr = DescriptionHelper.CreateNoncestr();

            config.signType = "MD5";

            return config;
        }

        /// <summary>
        /// 【周末酒店Lite】小程序内获取支付配置参数
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public WxAppPayConfig WxAppPayForZmjdLite(WxAppPayRequestParam p)
        {
            WxAppPayConfig config = WeiXinAdapter.GetWxAppUnifiedOrderForZmjdLite(p);

            //config.timeStamp = DescriptionHelper.getSecondCountSince19700101();

            //config.nonceStr = DescriptionHelper.CreateNoncestr();

            config.signType = "MD5";

            return config;
        }

        /// <summary>
        /// 【遛娃指南Lite】小程序内获取支付配置参数
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public WxAppPayConfig WxAppPayForLiuwaLite(WxAppPayRequestParam p)
        {
            WxAppPayConfig config = WeiXinAdapter.GetWxAppUnifiedOrderForLiuwaLite(p);

            //config.timeStamp = DescriptionHelper.getSecondCountSince19700101();

            //config.nonceStr = DescriptionHelper.CreateNoncestr();

            config.signType = "MD5";

            return config;
        }

        /// <summary>
        /// 【微信小程序】酒店套餐订单提交
        /// </summary>
        /// <param name="orderForWxapp"></param>
        /// <returns></returns>
        [HttpPost]
        public OrderSubmitResult SubmitOrderForWxapp(SubmitHotelOrderParamForWxapp orderForWxapp)
        {
            //设置订单提交参数
            OrderPackageEntity package = new OrderPackageEntity();
            package.CheckIn = orderForWxapp.CheckIn;
            package.NightCount = orderForWxapp.NightCount; //(int)(orderForWxapp.CheckOut - orderForWxapp.CheckIn).TotalDays;
            package.Contact = orderForWxapp.Contact;
            package.ContactPhone = orderForWxapp.ContactPhone;
            package.PID = orderForWxapp.PackageId;
            package.RoomCount = orderForWxapp.RoomCount;
            package.Note = orderForWxapp.Note;

            PackageInfoEntity packageInfo = null;

            HotelPrice2 price = PriceAdapter.GetHotelPackageList(orderForWxapp.HotelId, orderForWxapp.CheckIn.ToString("yyyy-MM-dd"), orderForWxapp.CheckOut.ToString("yyyy-MM-dd"), "wap", "4.2", orderForWxapp.UserId, needNotSalePackage: true);
            foreach (PackageInfoEntity entity in price.Packages)
            {
                if (entity.packageBase.ID == orderForWxapp.PackageId)
                {
                    packageInfo = entity;
                    break;
                }
            }

            //计算现金券使用
            if (orderForWxapp.ChooseCash)
            {
                var cashAmount = packageInfo.CanUseCashCoupon * orderForWxapp.RoomCount;
                int userCouponAmount = 0; if (orderForWxapp.UserId > 0) userCouponAmount = CouponAdapter.GetUserCanUseCashCouponAmount(orderForWxapp.UserId) / 100;
                var userUseCashCouponAmount = cashAmount <= userCouponAmount ? cashAmount : userCouponAmount;
                package.UserUseCashCouponAmount = userUseCashCouponAmount;
            }
            else
            {
                package.UserUseCashCouponAmount = 0;
            }

            //支付类型
            package.PayType = packageInfo.PayType;

            if (packageInfo == null)
            {
                return new OrderSubmitResult
                {
                    ErrorCode = OrderErrorCode.ORDER_ERROR_NOPID,
                    ErrorMessage = "套餐已过期，请稍后再重新下单，谢谢！"
                };
            }
            if (packageInfo.packageBase.PackageCount == 0)
            {
                return new OrderSubmitResult
                {
                    ErrorCode = OrderErrorCode.ORDER_ERROR_SELLOUT,
                    ErrorMessage = "套餐已售尽，请选择其它套餐"
                };
            }

            OrderMainEntity main = new OrderMainEntity();
            main.Amount = packageInfo.Price * package.RoomCount;
            main.Type = (OrderType)packageInfo.PackageType;
            main.HotelID = orderForWxapp.HotelId;
            main.TerminalID = 5;    //wxapp 微信小程序
            main.ChannelID = -1;
            main.UserID = orderForWxapp.UserId;

            if (packageInfo.packageBase.PackageCount < package.RoomCount)
            {
                return new OrderSubmitResult
                {
                    ErrorCode = OrderErrorCode.ORDER_ERROR_SELLOUT,
                    ErrorMessage = "套餐目前仅剩" + packageInfo.packageBase.PackageCount + "套, 请重新选择"
                };
            }
            int canCustomBuyMin = packageInfo.CustomBuyMin;
            int canCustomBuyMax = packageInfo.CustomBuyMax;
            if (canCustomBuyMax != 0 && packageInfo.packageBase.PackageCount > canCustomBuyMax)
            {
                if ((package.RoomCount <= 0 ? 1 : package.RoomCount) > canCustomBuyMax)
                {
                    return new OrderSubmitResult
                    {
                        ErrorCode = OrderErrorCode.ORDER_ERROR_SELLOUT,
                        ErrorMessage = "每位用户限购" + canCustomBuyMax + "套, 请重新选择"
                    };
                }
            }

            if (canCustomBuyMin != 0)
            {
                if ((package.RoomCount <= 0 ? 1 : package.RoomCount) < canCustomBuyMin)
                {
                    return new OrderSubmitResult
                    {
                        ErrorCode = OrderErrorCode.ORDER_ERROR_SELLOUT,
                        ErrorMessage = "每位用户至少购" + canCustomBuyMin + "套, 请重新选择"
                    };
                }
            }

            //待提交对象
            OrderEntity orderEntity = new OrderEntity();
            orderEntity.main = main;
            orderEntity.package = package;

            //出行人
            var _travelPersons = new List<int>();
            if (packageInfo.MinHotelPeople > 0 && !string.IsNullOrEmpty(orderForWxapp.TravelPersons))
            {
                try
                {
                    _travelPersons = orderForWxapp.TravelPersons.Split(',').ToList().Select(_id => Convert.ToInt32(_id)).ToList();
                }
                catch (Exception ex)
                {
                    return new OrderSubmitResult
                    {
                        ErrorCode = OrderErrorCode.ORDER_ERROR_NOPID,
                        ErrorMessage = "出行人读取错误，请重试"
                    };
                }
            }
            orderEntity.TravelId = _travelPersons;

            //submit
            OrderSubmitResult result = OrderAdapter.SubmitOrder(orderEntity, "4.2");

            return result;
        }

        #endregion
    }

    [DataContract]
    [Serializable]
    public class WxAppPayRequestParam
    {
        /// <summary>
        /// 商家orderid
        /// </summary>
        [DataMember]
        public string orderid { get; set; }

        /// <summary>
        /// 用户唯一标识，请注意，在未关注公众号时，
        /// 用户访问公众号的网页，也会产生一个用户和公众号唯一的OpenID
        /// </summary>
        [DataMember]
        public string openid { get; set; }
    }

    [DataContract]
    [Serializable]
    public class WxAppPayConfig
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int timeStamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string nonceStr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string prepay_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string package { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string signType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string paySign { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string openid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string orderid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string err_code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string err_code_des { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string result_code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string return_code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string return_msg { get; set; }
    }

    [DataContract]
    [Serializable]
    public class WeixinPayReturnParam : WeixinPrePayResult
    {
        /// <summary>
        /// 支付接口的时间搓
        /// </summary>
        [DataMember]
        public int timeStamp { get; set; }
        /// <summary>
        /// 随机字符串
        /// </summary>
        [DataMember]
        public string nonceStr { get; set; }
        /// <summary>
        /// 格式为： prepay_id=***
        /// </summary>
        [DataMember]
        public string package { get; set; }
        /// <summary>
        /// 加密方法
        /// </summary>
        [DataMember]
        public string signType { get; set; }
        /// <summary>
        /// 支付签名(appId也要参与签名)
        /// </summary>
        [DataMember]
        public string paySign { get; set; }
    }

    [DataContract]
    [Serializable]
    public class WeixinWebAuthResult
    {
        /// <summary>
        /// 网页授权access_token 与接口access_token 不同
        /// </summary>
        [DataMember]
        public string access_token { get; set; }
        /// <summary>
        /// 过期时间 2小时
        /// </summary>
        [DataMember]
        public int expires_in { get; set; }
        /// <summary>
        /// refresh_token用来延长有效期 用户刷新access_token
        /// </summary>
        [DataMember]
        public string refresh_token { get; set; }
        /// <summary>
        /// 用户唯一标识，请注意，在未关注公众号时，
        /// 用户访问公众号的网页，也会产生一个用户和公众号唯一的OpenID
        /// </summary>
        [DataMember]
        public string openid { get; set; }
        /// <summary>
        /// 用户授权的作用域，使用逗号（,）分隔
        /// </summary>
        [DataMember]
        public string scope { get; set; }
        /// <summary>
        /// 错误信息返回码
        /// </summary>
        [DataMember]
        public string errcode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        [DataMember]
        public string errmsg { get; set; }
    }

    [DataContract]
    [Serializable]
    public class SubmitHotelOrderParamForWxapp
    {
        [DataMember]
        public int HotelId { get; set; }

        [DataMember]
        public int PackageId { get; set; }

        [DataMember]
        public string Contact { get; set; }

        [DataMember]
        public string ContactPhone { get; set; }

        [DataMember]
        public DateTime CheckIn { get; set; }

        [DataMember]
        public DateTime CheckOut { get; set; }

        [DataMember]
        public int NightCount { get; set; }

        [DataMember]
        public int RoomCount { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public string TravelPersons { get; set; }

        [DataMember]
        public bool ChooseCash { get; set; }

        [DataMember]
        public int TotalPrice { get; set; }

        [DataMember]
        public int UserId { get; set; }
    }
}