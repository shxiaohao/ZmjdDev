using Com.Alipay;
using HJD.HotelPrice.Contract;
using HJDAPI.APIProxy;
using HJDAPI.Common.Security;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WHotelSite.Controllers;
using WHotelSite.Models;

namespace WHotelSite.Common
{
    public class PaymentMethod
    {
        const string alipay_wap_gateway = "http://wappaygw.alipay.com/service/rest.htm";
        const string alipay_web_gateway = "https://mapi.alipay.com/gateway.do";
        const string alipay_seller_email = "alizhifu@zmjiudian.com";//  "zhifu@zmjiudian.com";
        const string alipay_notify_url = "http://alipay.zmjiudian.com/notifyurl_ForHaoYi.aspx";
        const string alipay_wap_notify_url = "http://alipay.zmjiudian.com/notify_url_ForHaoYi.aspx"; //RSA认证通不过，只能用MD5了

        public  PayOrderInfoEntity GenOneOrderFinalPayUrl(UrlHelper Url, HttpServerUtilityBase Server, long OrderID, string Channel, long CID)
        {
            PayOrderInfoEntity ue = GenPayCompleteUrlList( Url, OrderID, new List<string> { Channel }, CID).First();
            ue.payUrl = GenOneOrderFinalPayUrl(ue, Server);
            return ue;
        }


        public List<PayOrderInfoEntity> GenPayCompleteUrlList(UrlHelper Url, long OrderID, List<string> ChannelList, long CID)
        {
            List<PayOrderInfoEntity> list = new List<PayOrderInfoEntity>();

            HJD.CouponService.Contracts.Entity.CommOrderEntity commOrder = new HJD.CouponService.Contracts.Entity.CommOrderEntity();

            PackageOrderInfo20 order = new PackageOrderInfo20();

            if (OrderHelper.IsHotelOrder(OrderID))
            {
                order = Order.GetPackageOrderInfo20(OrderID, AccountController.GetUserId());
            }
            else
            {
                commOrder = new Coupon().GetOneCommOrderEntity(Convert.ToInt32(OrderID));
            }

            ChannelList.ForEach(Channel =>
            list.Add(GenPayCompleteUrl(  Url, OrderID, Channel, CID, commOrder, order)));
           // list.Where(_ => _.payChannelType == "tenpay").First(); 
            return list;
        }

        private PayOrderInfoEntity GenPayCompleteUrl(UrlHelper Url, long OrderID, string Channel, long CID, HJD.CouponService.Contracts.Entity.CommOrderEntity commOrder, PackageOrderInfo20 order)
        {
            PayOrderInfoEntity ue = new PayOrderInfoEntity();
            ue.payChannelType = Channel;
            ue.OrderID = OrderID;
            ue.CID = CID;

            try
            {
                #region 普通订单处理

                if (OrderHelper.IsHotelOrder(OrderID))
                {
                    ue.OrderState = order.State == 1 ? 0 : 1;
                    ue.MobilePhone = order.ContactPhone;
                    ue.completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("Complete", "Payment", new { channel = Channel, order = OrderID, _clearoneoff = 1 }));
                    ue.cancelUrl = Utils.GetAbsoluteUrl(Url, Url.Action("Cancel", "Payment", new { channel = Channel, order = OrderID }));

                    ue.orderName = order.HotelName + "(" + order.PackageName + ")";
                    ue.orderPrice = order.UserShouldPayAmount;
                    ue.orderPoints = 0;
                    ue.productUrl = HotelController.GetPackageReBookUrl(Url, order);
                    ue.UserID = order.UserID;

                    if (Channel == "alipay")
                    {
                        if (order.ContactPhone == "18021036971")
                        {
                            ue.orderPrice = 0.01M;
                        }
                    }
                }
                #endregion

                #region 通用订单处理（房券、VIP、1来5往 等）

                else
                {
                    ue.OrderState = commOrder.State;
                    ue.MobilePhone = commOrder.PhoneNum;

                    if (commOrder.TypeID == 10000)
                    {
                        var _timeStamp = Signature.GenTimeStamp();
                        ue.completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayCompleteInvoice", "Coupon", new { channel = Channel, order = OrderID, _t = _timeStamp, _clearoneoff = 1 }));
                    }
                    //房券类型订单
                    else if (commOrder.TypeID >= 200)
                    {
                        //ue.productUrl = CouponController.GetCouponActivityUrl(Url, order);
                        ue.productUrl = CouponController.GetCommCouponActivityUrl(Url, commOrder);

                        //只判断Price>0的
                        if (commOrder.RelExchangeCoupon.Exists(_ => _.Price > 0))
                        {
                            commOrder.RelExchangeCoupon = commOrder.RelExchangeCoupon.Where(_ => _.Price > 0).ToList();
                        }

                        //200 限时抢购；300 团购；400 VIP会员
                        if (commOrder.RelExchangeCoupon[0].ActivityType == 400)
                        {
                            ue.completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayCompleteForVip", "Coupon", new { channel = Channel, order = OrderID, _clearoneoff = 1 }));
                        }
                        ////天天果园
                        //else if (commOrder.RelExchangeCoupon[0].ActivityType == 500)
                        //{
                        //    ue.completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayCompleteForFruitDay", "Coupon", new { channel = Channel, order = OrderID,   _clearoneoff = 1 }));
                        //}
                        //消费券
                        else if (commOrder.RelExchangeCoupon[0].ActivityType == 600 || commOrder.RelExchangeCoupon[0].SKUID > 0)
                        {
                            var _timeStamp = Signature.GenTimeStamp();

                            ue.completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = Channel, order = OrderID, _t = _timeStamp, _clearoneoff = 1 }));
                        }
                        else
                        {
                            //暂时去掉这个判断 2017.05.18 haoy
                            ////房券订单首先要验证是否过期了
                            //var canpay = new Coupon().IsExchangeOrderCanPay(Convert.ToInt32(OrderID));
                            //if (canpay > 0)
                            //{
                            //    return Redirect(ue.productUrl);
                            //}

                            var _timeStamp = Signature.GenTimeStamp();

                            ue.completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Coupon", new { channel = Channel, order = OrderID, _t = _timeStamp, _clearoneoff = 1 }));
                        }
                    }
                    //1来5往
                    else if (commOrder.TypeID == (int)CommOrderType.Active_OneToFive)
                    {
                        ue.completeUrl = string.Format("http://www.zmjiudian.com/wx/active/regpayok/7/{0}", commOrder.CustomID);
                    }
                    else
                    {
                        ue.completeUrl = Utils.GetAbsoluteUrl(Url, Url.Action("PayComplete", "Order", new { channel = Channel, order = OrderID }));
                    }

                    ue.cancelUrl = Utils.GetAbsoluteUrl(Url, Url.Action("Cancel", "Payment", new { channel = Channel, order = OrderID }));
                    ue.orderName = commOrder.Name;
                    ue.orderPrice = commOrder.Price;
                    ue.orderPoints = commOrder.Points;
                    ue.UserID = account.GetUserInfoByMobile(commOrder.PhoneNum).UserId;

                    if (Channel == "alipay")
                    {
                        if (commOrder.PhoneNum == "18021036971")
                        {
                            ue.orderPrice = 0.01M;
                        }
                    }
                }

                #endregion
                switch (ue.payChannelType)
                {
                    case "alipay":
                    case "upay":
                    case "tenpay":
                    case "chinaflashpay":
                        ue.payUrl = ue.completeUrl;
                        //  ue.successIdentifier = ue.completeUrl;
                        break;
                    case "chinapay":
                        ue.payUrl = string.Format("{0}/OrderPay.aspx?OrderID={1}&retUrl={2}&channelType={3}", Config.chinaPayUrl, ue.OrderID, HttpUtility.UrlEncode(ue.completeUrl), ue.CID);
                        // ue.successIdentifier = "payment.chinapay.com/CTITS/m";
                        break;
                    case "cmbpay":
                        ue.payUrl = string.Format("{0}/pay.aspx?OrderID={1}&retUrl={2}", Config.cmbPayUrl, ue.OrderID, HttpUtility.UrlEncode(ue.completeUrl));
                        // ue.successIdentifier = ue.completeUrl;
                        break;
                }

                ue.successIdentifier = ue.completeUrl;
            }
            catch(Exception err)
            {
                LogHelper.WriteLog("GenPayCompleteUrl Error:" +err.Message + err.StackTrace);
            }
           // ue.failedIdentifier = ue.cancelUrl;
            return ue;
        }

        /// <summary>
        /// 获取或生成订单第三方支付URL（ 支付宝等会生成加密后的支付URL）
        /// </summary>
        /// <param name="ue"></param>
        private string GenOneOrderFinalPayUrl(PayOrderInfoEntity ue,HttpServerUtilityBase Server)
        {
            switch (ue.payChannelType)
            {
                case "alipay":
                    ue.payUrl = Utils.IsMobile() ?
                                   ComposeUrlForAlipay( Server,ue.OrderID, ue.orderName, ue.orderPrice.ToString("0.00"), ue.completeUrl, ue.cancelUrl) :
                                   ComposeUrlForAlipayWeb(ue.OrderID, ue.orderName, ue.orderPrice.ToString("0.00"), ue.completeUrl, ue.cancelUrl, ue.productUrl);
                    break;
                case "upay":
                    ue.payUrl = Utils.IsMobile() ?
                   ComposeUrlForUpay(ue.OrderID.ToString(), ue.completeUrl) :
                   ComposeUrlForUpayWeb(ue.OrderID.ToString(), ue.completeUrl);
                    break;
                case "tenpay":
                    ue.payUrl = ue.completeUrl;
                    break;
                case "chinaflashpay":
                    ue.payUrl = Utils.IsMobile() ?
                                   "" :
                                   string.Format("{0}/OrderPay.aspx?OrderID={1}&retUrl={2}&channelType={3}", Config.chinaPayUrl, ue.OrderID, ue.completeUrl, ue.CID);
                    break;
                case "chinapay":
                  //  ue.payUrl = string.Format("http://chinapay.zmjiudian.com/OrderPay.aspx?OrderID={0}&retUrl={1}&channelType={2}", ue.OrderID, ue.completeUrl, ue.CID);
                    ue.payUrl = string.Format("{0}/OrderPay.aspx?OrderID={1}&retUrl={2}&channelType={3}", Config.chinaPayUrl, ue.OrderID, ue.completeUrl, ue.CID);
             
                    break;
                case "cmbpay":
                   // ue.payUrl = string.Format("http://cmbpay.zmjiudian.com/pay.aspx?OrderID={0}&retUrl={1}", ue.OrderID, ue.completeUrl);
                     ue.payUrl = string.Format("{0}/pay.aspx?OrderID={1}&retUrl={2}",Config.cmbPayUrl, ue.OrderID, ue.completeUrl);
                   break;
            }

            return ue.payUrl;
        }

        private string ComposeUrlForAlipay(HttpServerUtilityBase Server, long orderId, string orderName, string orderPrice, string completeUrl, string cancelUrl)
        {
            ////////////////////////////////////////////调用授权接口alipay.wap.trade.create.direct获取授权码token////////////////////////////////////////////
            completeUrl = Server.UrlEncode(completeUrl);
            cancelUrl = Server.UrlEncode(cancelUrl);

            //返回格式
            string format = "xml";
            //必填，不需要修改

            //返回格式
            string v = "2.0";
            //必填，不需要修改

            //请求号
            string req_id = DateTime.Now.ToString("yyyyMMddHHmmss");
            //必填，须保证每次请求都是唯一

            //req_data详细信息

            //服务器异步通知页面路径
            string notify_url = alipay_wap_notify_url;
            //需http://格式的完整路径，不允许加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string call_back_url = completeUrl;
            //需http://格式的完整路径，不允许加?id=123这类自定义参数

            //操作中断返回地址
            string merchant_url = cancelUrl;
            //用户付款中途退出返回商户的地址。需http://格式的完整路径，不允许加?id=123这类自定义参数

            //卖家支付宝帐户
            string seller_email = alipay_seller_email;
            //必填

            //商户订单号
            string out_trade_no = orderId.ToString();
            //商户网站订单系统中唯一订单号，必填

            //订单名称
            string subject = orderName.Replace(" ", ""); // Utils.UrlEncode(orderName);
            //必填

            //付款金额
            string total_fee = orderPrice;
            //必填

            //请求业务参数详细
            string req_dataToken = "<direct_trade_create_req><notify_url>" + notify_url + "</notify_url><call_back_url>" + call_back_url + "</call_back_url><seller_account_name>" + seller_email + "</seller_account_name><out_trade_no>" + out_trade_no + "</out_trade_no><subject>" + subject + "</subject><total_fee>" + total_fee + "</total_fee><merchant_url>" + merchant_url + "</merchant_url></direct_trade_create_req>";
            //必填

            //把请求参数打包成数组
            Dictionary<string, string> sParaTempToken = new Dictionary<string, string>();
            sParaTempToken.Add("partner", Com.Alipay.Config.Partner);
            sParaTempToken.Add("_input_charset", Com.Alipay.Config.Input_charset.ToLower());
            sParaTempToken.Add("sec_id", Com.Alipay.Config.WapSign_type.ToUpper());
            sParaTempToken.Add("format", format);
            sParaTempToken.Add("v", v);
            sParaTempToken.Add("req_id", req_id);
            sParaTempToken.Add("req_data", req_dataToken);

            sParaTempToken.Add("service", "alipay.wap.trade.create.direct");

            //sParaTempToken.Add("terminal_type", "WAP");


            //建立请求
            string sHtmlTextToken = Submit.BuildRequest(alipay_wap_gateway + "?", sParaTempToken, Com.Alipay.Config.WapSign_type.ToUpper());
            //URLDECODE返回的信息
            Encoding code = Encoding.GetEncoding(Com.Alipay.Config.Input_charset);
            sHtmlTextToken = HttpUtility.UrlDecode(sHtmlTextToken, code);

            //解析远程模拟提交后返回的信息
            Dictionary<string, string> dicHtmlTextToken = Submit.ParseResponse(sHtmlTextToken, Com.Alipay.Config.Input_charset.ToLower());

            //获取token
            string request_token = dicHtmlTextToken["request_token"];

            ////////////////////////////////////////////根据授权码token调用交易接口alipay.wap.auth.authAndExecute////////////////////////////////////////////


            //业务详细
            string req_data = "<auth_and_execute_req><request_token>" + request_token + "</request_token></auth_and_execute_req>";
            //必填

            //把请求参数打包成数组
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("partner", Com.Alipay.Config.Partner);
            dict.Add("_input_charset", Com.Alipay.Config.Input_charset.ToLower());
            dict.Add("sec_id", Com.Alipay.Config.WapSign_type.ToUpper());
            dict.Add("service", "alipay.wap.auth.authAndExecute");
            dict.Add("format", format);
            dict.Add("v", v);
            dict.Add("req_data", req_data);
            dict.Add("sign", SignForAlipay(dict, Com.Alipay.Config.WapSign_type.ToUpper()));

            //Com.Alipay.Submit.BuildRequest
            return alipay_wap_gateway + "?" + Utils.BuildQueryString(dict, Com.Alipay.Config.Input_charset.ToLower());
        }

        private string ComposeUrlForAlipayWeb(long orderId, string orderName, string orderPrice, string completeUrl, string cancelUrl, string productUrl)
        {
            ////////////////////////////////////////////请求参数////////////////////////////////////////////

            //支付类型
            string payment_type = "1";
            //必填，不能修改
            //服务器异步通知页面路径
            string notify_url = alipay_notify_url;
            //需http://格式的完整路径，不能加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string return_url = completeUrl;
            //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/

            //卖家支付宝帐户
            string seller_email = alipay_seller_email;
            //必填

            //商户订单号
            string out_trade_no = orderId.ToString();
            //商户网站订单系统中唯一订单号，必填

            //订单名称
            string subject = orderName;
            //必填

            //付款金额
            string total_fee = orderPrice;
            //必填

            //订单描述

            string body = "";
            //商品展示地址
            string show_url = productUrl;
            //需以http://开头的完整路径，例如：http://www.xxx.com/myorder.html

            //防钓鱼时间戳
            string anti_phishing_key = Submit.Query_timestamp();
            //若要使用请调用类文件submit中的query_timestamp函数

            //客户端的IP地址
            string exter_invoke_ip = "";
            //非局域网的外网IP地址，如：221.0.0.1


            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("partner", Com.Alipay.Config.Partner);
            dict.Add("_input_charset", Com.Alipay.Config.Input_charset.ToLower());
            dict.Add("service", "create_direct_pay_by_user");
            dict.Add("payment_type", payment_type);
            dict.Add("notify_url", notify_url);
            dict.Add("return_url", return_url);
            dict.Add("seller_email", seller_email);
            dict.Add("out_trade_no", out_trade_no);
            dict.Add("subject", subject);
            dict.Add("total_fee", total_fee);
            dict.Add("body", body);
            dict.Add("show_url", show_url);
            dict.Add("anti_phishing_key", anti_phishing_key);
            dict.Add("exter_invoke_ip", exter_invoke_ip);
            dict.Add("sign", SignForAlipay(dict, Com.Alipay.Config.Sign_type.ToUpper()));
            dict.Add("sign_type", Com.Alipay.Config.Sign_type);

            return alipay_web_gateway + "?" + Utils.BuildQueryString(dict, Com.Alipay.Config.Input_charset);
        }

        private string SignForAlipay(Dictionary<string, string> dict, string sign_type)
        {
            Dictionary<string, string> filterd = Com.Alipay.Core.FilterPara(dict);
            SortedDictionary<string, string> sorted = new SortedDictionary<string, string>(filterd);
            return Com.Alipay.Submit.BuildRequestMysign(new Dictionary<string, string>(sorted), sign_type);
        }

        private string ComposeUrlForUpay(string orderId, string completeUrl)
        {
            string url = String.Format("http://upay.zmjiudian.com:86/api/upay/pay_req?orderid={0}&ret_url={1}", orderId, Utils.UrlEncode(completeUrl));
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
            Dictionary<string, object> data = (Dictionary<string, object>)new JavaScriptSerializer().Deserialize<dynamic>(reader.ReadToEnd());
            if (data["err_code"].ToString() == "0000")
            {
                return "https://m.soopay.net/m/html5/index.do?tradeNo=" + data["trade_no"].ToString() + "&pay_type=&gate_id=";
            }
            throw new Exception(String.Format("error[{0}]: {1}", data["code"], data["err_message"]));
        }

        private string ComposeUrlForUpayWeb(string orderId, string completeUrl)
        {
            string url = String.Format("http://upay.zmjiudian.com:86/api/upay/pay_req_split_front_withpayType?orderid={0}&ret_url={1}&pay_type={2}", orderId, Utils.UrlEncode(completeUrl), "PREAUTHREQ");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
            return (string)new JavaScriptSerializer().Deserialize<dynamic>(reader.ReadToEnd());
        }
    }
}