using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HJD.HotelServices;
using HJDAPI.Models;
using HJDAPI.APIProxy;
using HJD.HotelPrice.Contract.DataContract.Order;
using WHotelSite.Params.Order;
using HJDAPI.Common.Security;
using WHotelSite.Params.Hotel;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers;
using WHotelSite.Common;
using HJDAPI.Controllers.Common;
using System.Xml;
using WHotelSite.Params.Payment;
using HJD.CouponService.Contracts.Entity;
using HJD.HotelPrice.Contract;
using System.Net;
using WHotelSite.Models;

namespace WHotelSite.Controllers
{
    /// <summary>
    /// 处理订单相关（如酒店订单、券订单等）
    /// </summary>
    public class OrderController : BaseController
    {

        private static string strCurrentUserWeiXinOpenID = "CurrentUserWeiXinOpenID";
        private static string strCurrentUserWeiXinPayOrderID = "CurrentUserWeiXinPayOrderID";
        private static string strPayOriURL = "PayOriURL";

        private string PayOriURL
        {
            get
            {
                if (Session[strPayOriURL] == null)
                {
                    return "";
                }
                else
                {
                    return Session[strPayOriURL].ToString();
                }
            }
            set
            {
                if (Session[strPayOriURL] == null)
                {
                    Session.Add(strPayOriURL, value);
                }
                else
                {
                    Session[strPayOriURL] = value;
                }
            }
        }

        private long  CurrentUserWeiXinPayOrderID
        {
            get
            {
                if (Session[strCurrentUserWeiXinPayOrderID] == null)
                {
                    return 0;
                }
                else
                {
                    return long.Parse( Session[strCurrentUserWeiXinPayOrderID].ToString());
                }
            }
            set
            {
                if (Session[strCurrentUserWeiXinPayOrderID] == null)
                {
                    Session.Add(strCurrentUserWeiXinPayOrderID, value);
                }
                else
                {
                    Session[strCurrentUserWeiXinPayOrderID] = value;
                }
            }
        }
        
        private  string CurrentUserWeiXinOpenID
        {
            get
            {
                if (Session[strCurrentUserWeiXinOpenID] == null)
                {
                    return "";
                }
                else
                {
                    return Session[strCurrentUserWeiXinOpenID].ToString();
                }
            }
            set
            {
                if (Session[strCurrentUserWeiXinOpenID] == null)
                {
                    Session.Add(strCurrentUserWeiXinOpenID, value);
                }
                else
                {
                    Session[strCurrentUserWeiXinOpenID] = value;
                }
            }
        }

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            if (!UserState.IsLogin)
            {
                return Json(new { Message = "必须登录后才能查看", Success = 1 }, JsonRequestBehavior.AllowGet);
            }
            OrderParam param = new OrderParam(this);
            ViewBag.isMobile = Utils.IsMobile();
            ViewBag.param = param;
            ViewBag.totalCount = 0;
            if (param.UserID == 0)
            {
                return null;
            }
            List<OrderListItem> list = Order.GetUserOrderList(param.UserID, param.Start, param.Count);
            if (list != null && list.Count != 0)
            {
                ViewBag.totalCount = Order.GetUserOrderList(param.UserID, 0, int.MaxValue).Count;//获得客户订单的总数
            }
            return View("List", list);
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail()
        {
            OrderParam param = new OrderParam(this);
            LogHelper.WriteLog(string.Format("Order.Detail:{0} {1} {2}", param.UserID, param.OrderID, param.key));
           ViewBag.isMobile = Utils.IsMobile();
            ViewBag.param = param;
            if (param.UserID == 0)
            {
                if (param.key == ""
                    || EncryptMethod.GenMD5Key(param.OrderID.ToString()) != param.key)
                {
                    return null;
                }
            }
            PackageOrderInfo20 detailInfo = Order.GetPackageOrderInfo20(param.OrderID, param.UserID);

            //购买成功后，获取当前订单可用的红包信息（非团购、非积分、非免费领取的产品，参会需要弹出分享红包功能）
            var canShareRedCoupon = false;
            RedOrderInfoEntity _shareRedEntity = new RedOrderInfoEntity();
            if (param.OrderID > 0 && detailInfo != null)
            {
                try
                {
                    _shareRedEntity = Coupon.GetOrderRed(0, param.OrderID, HJDAPI.Common.Helpers.Enums.OrdersType.HotelOrder, detailInfo.Amount);
                    if (_shareRedEntity != null && _shareRedEntity.RedState == 1)
                    {
                        canShareRedCoupon = true;
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
            ViewBag.CanShareRedCoupon = canShareRedCoupon;
            ViewBag.ShareRedEntity = _shareRedEntity;

            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.IsInWeixin = isInWeixin;

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            return View(detailInfo);
        }

        /// <summary>
        /// 微信支付跳转页面
        /// </summary>
        /// <param name="order"></param>
        /// <param name="code"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult WXPay(string order = "", string code = "", int userid = 0)
        {
         
            if(!string.IsNullOrEmpty(order))
            {
                LogHelper.WriteLog("WXPay：" + order);
                CurrentUserWeiXinPayOrderID = long.Parse(order);
            }

            #region 需要换取微信openid
            if (true)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    LogHelper.WriteLog("WXPay： code" + code);
                     //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Openid))
                    {
                        LogHelper.WriteLog("WXPay： Openid" + accessToken.Openid);
                        CurrentUserWeiXinOpenID = accessToken.Openid;
                    }
                }
                else
                {
                    //授权页面
                  var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/order/wxpay")));
                  // var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjd100.com/order/wxpay")));
                  LogHelper.WriteLog("WXPay： 授权页面" + weixinGoUrl);
               
                    return Redirect(weixinGoUrl);
                }
            }

            #endregion

            var orderNo = "";
            var productName = "";
            Decimal price = 0;

            if (WHotelSite.Common.OrderHelper.IsHotelOrder(CurrentUserWeiXinPayOrderID ))
            {
                var e = Order.GetPackageOrderInfo20(CurrentUserWeiXinPayOrderID, userid);

                orderNo = e.OrderID.ToString();
                productName = e.HotelName + " " + e.PackageName;
                price = e.UserShouldPayAmount;
            }
            else
            {
                HJD.CouponService.Contracts.Entity.CommOrderEntity commOrder = new Coupon().GetOneCommOrderEntity(Convert.ToInt32(CurrentUserWeiXinPayOrderID));

                orderNo = CurrentUserWeiXinPayOrderID.ToString();
                productName = commOrder.Name;
                price = commOrder.Price;
            }

            ViewBag.OrderNo = orderNo;
            ViewBag.ProductName = productName;
            ViewBag.Price = price;
            ViewBag.PayOriUrl = PayOriURL;

            return View();
        }

        /// <summary>
        /// 订单支付成功页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PayComplete( )
        { 
            GeneralParam param = new GeneralParam(this);
            string orderId = param.OrderId.ToString();
            string channel = param.Channel.ToString();
            if (orderId == "0")
            {
                long result;
                orderId = (long.TryParse((String)this.Request.QueryString["order"], out result) ? result : 0).ToString();
                param.OrderId = long.Parse(orderId);
            }

            HJD.CouponService.Contracts.Entity.CommOrderEntity commOrder = new Coupon().GetOneCommOrderEntity(Convert.ToInt32(param.OrderId));

            return View(commOrder);
        }

        /// <summary>
        /// 订单支付方式选择页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Pay()
        {
            PayOriURL = Request.Url.OriginalString;

            string[] keywords = new string[] { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MicroMessenger" };

            bool isShowHeader = true;
            foreach (var item in keywords)
            {
                if (Request.UserAgent.IndexOf(item) > -1)
                {
                    isShowHeader = false;
                    break;
                }
            }

            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isShowHeader = isShowHeader;//如果是App里或微信里打开页面  则不显示
            ViewBag.isInWeixin = isInWeixin;
            ViewBag.isInApp = base.IsApp();
            ViewBag.OrderState = 0;         //订单状态 0:待支付  1：已支付  3：已取消 
            ViewBag.OrderStateName = "";    //订单状态 状态描述
            ViewBag.OrderTitle = "";        //订单标题
            ViewBag.OrderPrice = 0;         //订单价格
            ViewBag.UserId = 0;
         
            //支付的俩个入口 //1.入口1 就是订单ID
            long OrderNo = this.Request.Params["order"] != null ? long.Parse(this.Request.Params["order"]) : 0;
            ViewBag.OrderNo = OrderNo;

            ViewBag.CID = GetCurCID();
            //套餐支付类型（1现付 2担保 3预付）
            ViewBag.PayType = this.Request.Params["payType"] != null ? Convert.ToInt32(this.Request.Params["payType"]) : 0;

            //订单可支付方式
            var payChannels = this.Request.Params["payChannels"] != null ? this.Request.Params["payChannels"].ToString() : "chinapay,chinaflashpay,tenpay,alipay,cmbpay";

            ViewBag.PayChannelList = payChannels.Split(',').ToList();

            ViewBag.OrderState = -1;
            ViewBag.OrderStateName = "";
            ViewBag.OrderTitle = "";
            ViewBag.OrderPrice = 0.0;
            ViewBag.mobilePhone = "";

            HJDAPI.Common.Helpers.TimeLog log = new HJDAPI.Common.Helpers.TimeLog(string.Format("Pay:{0}", OrderNo), 3 * 1000);

            //2.入口2 就是提交的订单参数
            if (OrderNo == 0)
            {
                SubmitParam param = (SubmitParam)Session["orderParam"];
                ViewBag.SubmitParam = param;
            }
            else
            {
                List<PayOrderInfoEntity> payUrlList = new PaymentMethod().GenPayCompleteUrlList(Url, (long)ViewBag.OrderNo, ViewBag.PayChannelList, (long)ViewBag.CID);
                log.AddLog("GenPayCompleteUrlList");
                var ue = payUrlList.First();
                ViewBag.OrderState = ue.OrderState;
                ViewBag.OrderTitle = ue.orderName;
                ViewBag.OrderPrice = ue.orderPrice;
                ViewBag.mobilePhone = ue.MobilePhone;
                ViewBag.UserId = ue.UserID;

                ViewBag.PayUrlList = payUrlList;

                #region  处理积分支付  直接扣除积分，如果积分不足，那么
                if (ue.orderPoints > 0)
                {
                    long userid = ue.UserID;
                    GetInspectorHotelsListParam lp = new GetInspectorHotelsListParam { userid = userid };

                    PointResult userPointsInfo = Coupon.GetPersonalPoint(lp);
                    log.AddLog("GetPersonalPoint");
                    if (userPointsInfo.CanUsePoints >= ue.orderPoints)
                    {
                            string freePayUrl = PayHelper.GenPointPayUrl(OrderNo, (int)ue.orderPoints, GetCurCID());
                            string payResult = HttpHelper.Get(freePayUrl, "utf-8");
                        //重定向
                        log.AddLog("userPointsInfo.CanUsePoints >= ue.orderPoints");
                        log.Finish();
                        return Redirect(ue.completeUrl);
                    }
                    else
                    {
                        log.Finish();
                        return Redirect(ue.cancelUrl);
                    }
                }
                #endregion

                #region 如果订单金额为0，那么直接调用免费支付完成到支付过程，完成后直接跳转到支付完成页。


                if (ue.orderPrice == 0)
                {
                    string freePayUrl = PayHelper.GenFreePayUrl(OrderNo, 0, GetCurCID());
                    LogHelper.WriteLog("free pay:" + freePayUrl);
                    HttpHelper.Get(freePayUrl, "utf-8");
                    //重定向
                    return Redirect(ue.completeUrl);
                }


            #endregion
            }

          


            ViewBag.TerminalType = base._ContextBasicInfo.GetTerminalType().ToString();
            ViewBag.FinishUrl = "";
            ViewBag.SuccessIdentifier = "";

            ViewBag.BeforeCheckResult = PayHelper.CheckOrderBeforePay(OrderNo);

            log.AddLog("CheckOrderBeforePay");

            var webSiteUrl = WHotelSite.Common.Config.WebSiteUrl;


            if (ViewBag.mobilePhone == "18021036971" || ViewBag.mobilePhone == "14455667788" || ViewBag.mobilePhone == "11223344556")
            {
                LogHelper.WriteLog(string.Format("Pay:{0} {1} {2} {3} {4} {5}",
                   Request.RawUrl,
                   Session.SessionID,
                   ViewBag.mobilePhone,
                   ViewBag.isInApp,
                   Request.UserAgent,
                   string.Join(",", Request.Headers.AllKeys)));
            }
            log.AddLog("2-------");

            log.Finish();

            return View();
        }
        /// <summary>
        /// 订单支付方式选择页面存页面传来数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Pay2()
        {
            SubmitParam param = new SubmitParam(this);//获得参数
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (param.RoomCount != 0 && !string.IsNullOrEmpty(param.Contact))
            {
                Session.Add("orderParam", param);
                dict["url"] = Url.Action("Pay", "Order", new { order = 0 });
            }
            else
            {
                dict["error"] = "缺少关键参数";
            }
            return Json(dict);
        }

        /// <summary>
        /// 取订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static PackageOrderInfo20 GetOrderInfo(long orderId)
        {
            return Order.GetPackageOrderInfo20(orderId, AccountController.GetUserId());
        }

        public bool IsOrderHasPaied(long orderId)
        {
            return Order.GetPackageOrderInfo20(orderId, AccountController.GetUserId()).State >= 10;
        }

        public ActionResult CancelAuthOrder()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (!UserState.IsLogin)
            {
                dict.Add("success", "False");
                dict.Add("message", "必须登录后才能操作");
                Json(dict);
            }

            OrderParam param = new OrderParam(this);

            bool isSuccess = Order.CancelAuthOrder(param.OrderID, param.UserID);
            dict.Add("success", isSuccess.ToString());

            string message = isSuccess ? "取消订单成功！" : "取消订单失败！";
            dict.Add("message", message);

            string newUrl = Url.RouteUrl("我的订单");
            dict.Add("url", newUrl);

            return Json(dict);
        }

        public ActionResult DeleteOrder()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (!UserState.IsLogin)
            {
                dict.Add("success", "False");
                dict.Add("message", "必须登录后才能操作");
                Json(dict);
            }

            OrderParam param = new OrderParam(this);
            CancelAuthOrderParams cop = new CancelAuthOrderParams();
            cop.orderid = param.OrderID;
            cop.userid = param.UserID;

            var delResult = Order.DeleteOrder(cop);
            dict.Add("success", (delResult.success == 0).ToString().ToLower());

            string message = delResult.Message; //delResult.success == 0 ? "删除订单成功！" : "删除订单失败！";
            dict.Add("message", message);

            string newUrl = Url.RouteUrl("我的订单");
            dict.Add("url", newUrl);

            return Json(dict);
        }

        public ActionResult CheckCanPay()
        {
            if (!UserState.IsLogin)
            {
                return Json(new { Message = "您尚未登录，无法继续操作", Success = 1, bCanPay = false });
            }
            long orderId = 0;
            orderId = long.TryParse(this.Request.Form["order"], out orderId) ? orderId : 0;
            //CheckOrderBeforePayRequestParams p = new CheckOrderBeforePayRequestParams();
            //p.OrderID = orderId;
            //p.TimeStamp = (long)Math.Round((DateTime.Now - new DateTime(1971, 1, 1, 8, 0, 0)).TotalSeconds, 0);
            //p.SourceID = 3;
            //p.RequestType = "CheckOrderBeforePay";
            //p.Sign = HJDAPI.Common.Security.Signature.GenSignature(p.TimeStamp, p.SourceID, WHotelSite.Common.Config.MD5Key, p.RequestType);
            //p.AppVer = "4.2";
            //p.UserID = UserState.UserID;
            CheckOrderBeforePayResponse response = PayHelper.CheckOrderBeforePay(orderId);
            return Json(response);
        }

        public ActionResult WeixinPay()
        {
            long orderid = CurrentUserWeiXinPayOrderID;

            LogHelper.WriteLog("WeixinPayReturnParam: orderid:" + orderid.ToString());
            //order 信息
            if (orderid == 0)
            {
                return Json(new { Success = 1, Message = "没有订单ID" }, JsonRequestBehavior.AllowGet);
            }
            else
            {              
                    var result = new WeixinPayReturnParam();
                    try
                    {
                        string openid = CurrentUserWeiXinOpenID;
                        var clientIP = Request.UserHostAddress;

                        string pre_pay_url = string.Format("{3}?orderid={0}&clientIP={1}&openid={2}", orderid, clientIP, openid, WHotelSite.Common.Config.TenPay_pre_pay_url);

                        LogHelper.WriteLog("pre_pay_url:" + pre_pay_url);

                        string xml = HttpHelper.Get(pre_pay_url, "utf-8"); //<WeixinPayReturnParam_ForService xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/Tenpay.Models"><err_code i:nil="true" /><err_code_des i:nil="true" /><prepay_id i:nil="true" /><result_code i:nil="true" /><return_code i:nil="true" /><return_msg i:nil="true" /><nonceStr>eBftKN0EdNnd6nuS</nonceStr><package>prepay_id=wx201605241917307bea7f635e0023591484</package><paySign>DA0947B61BC3F06AF99A79C547266D3C</paySign><signType>MD5</signType><timeStamp>1464117451</timeStamp></WeixinPayReturnParam_ForService>
                        string ns = @"xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/Tenpay.Models""";
                        string nil = @"i:nil=""true""";

                        xml = xml.Replace(ns, "").Replace(nil, "");
                        LogHelper.WriteLog("pre_pay_url Resykt:" + xml);



                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml);
                        XmlElement rootElement = xmlDoc.DocumentElement;


                        result = new WeixinPayReturnParam
                        {
                            err_code = WeiXinHelper.GetXMLNodeValue(rootElement, "err_code"),
                            err_code_des = WeiXinHelper.GetXMLNodeValue(rootElement, "err_code_des"),
                            nonceStr = WeiXinHelper.GetXMLNodeValue(rootElement, "nonceStr"),
                            package = WeiXinHelper.GetXMLNodeValue(rootElement, "package"),
                            paySign = WeiXinHelper.GetXMLNodeValue(rootElement, "paySign"),
                            prepay_id = WeiXinHelper.GetXMLNodeValue(rootElement, "prepay_id"),
                            result_code = WeiXinHelper.GetXMLNodeValue(rootElement, "result_code"),
                            return_code = WeiXinHelper.GetXMLNodeValue(rootElement, "return_code"),
                            return_msg = WeiXinHelper.GetXMLNodeValue(rootElement, "return_msg"),
                            signType = WeiXinHelper.GetXMLNodeValue(rootElement, "signType"),
                            timeStamp = int.Parse(WeiXinHelper.GetXMLNodeValue(rootElement, "timeStamp"))
                        };

                        LogHelper.WriteLog("WeixinPayReturnParam:" + result.package);

                    }
                    catch (Exception err)
                    {
                        LogHelper.WriteLog("WeixinPayReturnParam err:" + err.Message + err.StackTrace);

                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        #region 6.2 全部订单、券订单详情等

        /// <summary>
        /// 全部订单
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="selectedtype">默认选中的菜单类型 -1全部 28遛娃卡 0酒店 1机酒 2邮轮 15房券 20玩乐 14美食</param>
        /// <param name="specifyuid">是否制定userid,如果是,则已参数传递的uid为准,忽略session登录信息</param>
        /// <returns></returns>
        public ActionResult AllOrders(long userid = 0, int selectedtype = -1, int specifyuid = 0)
        {
            //if (!UserState.IsLogin)
            //{
            //    return Json(new { Message = "必须登录后才能查看", Success = 1 }, JsonRequestBehavior.AllowGet);
            //}

            //是否app环境
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = UserState.UserID > 0 ? UserState.UserID : (isApp ? userid : 0);
            if (specifyuid == 1 && userid > 0)
            {
                curUserID = userid;
            }
            ViewBag.UserId = curUserID;

            //当前默认选中的菜单类型
            ViewBag.SelectedType = selectedtype;

            return View();
        }

        /// <summary>
        /// 券订单详情
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult CouponOrderDetail(long oid, long userid = 0)
        {
            //if (!UserState.IsLogin)
            //{
            //    return Json(new { Message = "必须登录后才能查看", Success = 1 }, JsonRequestBehavior.AllowGet);
            //}

            //是否app环境
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;

            var curUserID = userid > 0 ? userid : UserState.UserID;
            ViewBag.UserId = curUserID;

            //订单ID
            ViewBag.OID = oid;

            return View();
        }


        public ActionResult OpenInvoice(long orderID = 0, long hotelID = 0,int pid= 0,int packageType = 0, long userID = 0, string phone = "")
        {
            ViewBag.Phone = phone;
            ViewBag.UserID = userID;
            ViewBag.HotelID = hotelID;
            ViewBag.Pid = pid;
            ViewBag.PackageType = packageType;
            ViewBag.OrderID = orderID;
            return View();
        }

        #endregion
    }
}