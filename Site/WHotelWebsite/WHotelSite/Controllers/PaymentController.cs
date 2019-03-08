using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

using HJD.DestServices.Contract;
using HJD.HotelServices;
using HJD.HotelServices.Contracts;
using HJDAPI.APIProxy;
using HJDAPI.Models;

using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelPrice.Contract;

using WHotelSite.Params.Payment;

using Com.Alipay;
using HJD.CouponService.Contracts.Entity;
using WHotelSite.Common;
using HJDAPI.Common.Security;
using WHotelSite.Models;

namespace WHotelSite.Controllers
{
    public class PaymentController : BaseController
    {
        const string alipay_wap_gateway = "http://wappaygw.alipay.com/service/rest.htm";
        const string alipay_web_gateway = "https://mapi.alipay.com/gateway.do";
        const string alipay_seller_email ="alizhifu@zmjiudian.com";//  "zhifu@zmjiudian.com";
        const string alipay_notify_url = "http://alipay.zmjiudian.com/notifyurl_ForHaoYi.aspx";
        const string alipay_wap_notify_url = "http://alipay.zmjiudian.com/notify_url_ForHaoYi.aspx"; //RSA认证通不过，只能用MD5了

        // 渠道选择
        public ActionResult Choose()
        {
            return View();
        }

        // 渠道跳转
        public ActionResult Direct()
        {
            GeneralParam param = new GeneralParam(this);
            string strOrderId = param.OrderId.ToString();
            if (strOrderId == "0")
            {
                long result;
                strOrderId = (long.TryParse((String)this.Request.QueryString["order"], out result) ? result : 0).ToString();
                param.OrderId = long.Parse(strOrderId);
            }

            if (string.IsNullOrEmpty(param.Channel))
            {
                param.Channel = (String)this.Request.QueryString["channel"];
            }

            if( this.Request.QueryString["cid"] != null)
            {
                long.TryParse(this.Request.QueryString["cid"], out  param.CID);
            }

            //支付参数           
             new PayOrderInfoEntity();

            long OrderID = param.OrderId;
            string Channel = param.Channel;
            long CID = param.CID;

            var ue = new PaymentMethod().GenOneOrderFinalPayUrl(Url,Server, OrderID, Channel, CID); 
            if (ue.payUrl.Length > 0)
            {
                return Redirect(ue.payUrl);
            }
            else
            {
                throw new Exception("无效的支付渠道");
            }
        }

      
        // 支付完成
        public ActionResult Complete()
        {
            GeneralParam param = new GeneralParam(this);

            //检查channel&order参数的完整性
            if (param == null || !string.IsNullOrEmpty(param.Channel))
            {
                param.Channel = Request.Params["channel"] ?? "";
            }
            if (param == null || param.OrderId <= 0)
            {
                param.OrderId = Convert.ToInt64(Request.Params["order"]);
            }

            PackageOrderInfo20 order = null;
            if (param.Channel == "alipay")
            {
                if (Request.QueryString["sign"] != null)  //不确定什么时候有返回串  KevinCai 待搞清楚
                {
                    string sign_type = Request.QueryString["sign_type"];
                    if (new Notify().VerifyReturn(GetQueryStringDict(), Request.QueryString["sign"], sign_type))
                    {
                        order = Order.GetPackageOrderInfo20(param.OrderId, AccountController.GetUserId());
                    }
                }
                else
                {
                    order = Order.GetPackageOrderInfo20(param.OrderId, AccountController.GetUserId());

                }
            }
            else if (param.Channel == "upay")
            {
                order = Order.GetPackageOrderInfo20(param.OrderId, AccountController.GetUserId());
            }
            else
            {
                LogHelper.WriteLog(string.Format("GetPackageOrderInfo20:{0} {1}", param.OrderId, AccountController.GetUserId()));
                order = Order.GetPackageOrderInfo20(param.OrderId, AccountController.GetUserId());
            }

            if (order != null)
            {
                ViewBag.order = order;
                ViewBag.hotel = HotelController.GetHotel(order.HotelID);
            }
            ViewBag.param = param;

            //购买成功后，获取当前订单可用的红包信息（非团购、非积分、非免费领取的产品，参会需要弹出分享红包功能）
            var canShareRedCoupon = false;
            RedOrderInfoEntity _shareRedEntity = new RedOrderInfoEntity();
            if (param.OrderId > 0 && order != null)
            {
                try
                {
                    _shareRedEntity = Coupon.GetOrderRed(0, param.OrderId, HJDAPI.Common.Helpers.Enums.OrdersType.HotelOrder, order.Amount);
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

            //支付完成页加载
            var value = string.Format("{{\"channel\":\"{0}\",\"orderid\":\"{1}\"}}", param.Channel, param.OrderId);
            RecordBehavior("PaymentCompleteLoad", value);
            ViewBag.PageTag = "PaymentComplete";

            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.IsInWeixin = isInWeixin;

            var isMobile = Utils.IsMobile();
            ViewBag.IsMobile = isMobile;

            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            return View(order);
        }

        // 支付取消
        public ActionResult Cancel()
        {
            GeneralParam param = new GeneralParam(this);

            string productUrl = "";

            if (  OrderHelper.IsHotelOrder( param.OrderId )  )
            {
                PackageOrderInfo20 order = Order.GetPackageOrderInfo20(param.OrderId, AccountController.GetUserId());
                productUrl = HotelController.GetPackageReBookUrl(Url, order);
            }
            else
            {
                HJD.CouponService.Contracts.Entity.CommOrderEntity order = new Coupon().GetOneCommOrderEntity(Convert.ToInt32(param.OrderId));
                if (order.TypeID == (int)CommOrderType.Active_OneToFive)
                {
                    productUrl = string.Format("http://www.zmjiudian.com/wx/active/reg/7/{0}", order.CustomID);
                }
                else
                {
                    productUrl = CouponController.GetCommCouponActivityUrl(Url, order);   
                }
            }

            //支付取消
            var value = string.Format("{{\"channel\":\"{0}\",\"orderid\":\"{1}\"}}", param.Channel, param.OrderId);
            RecordBehavior("PaymentCancelLoad", value);

            return Redirect(productUrl);
        }

        private Dictionary<string, string> GetQueryStringDict()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string key in  Request.QueryString.AllKeys)
            {
                dict[key] = Request.QueryString[key];
            }
            return dict;
        }
    }
}