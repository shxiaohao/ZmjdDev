
using HJD.HotelPrice.Contract.DataContract.Order;
using HJDAPI.Common.Alipay;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HJDAPI.Controllers.Common
{
    public  class AlipayCheck
    {
        const string alipay_wap_gateway = "http://wappaygw.alipay.com/service/rest.htm";
        const string alipay_web_gateway = "https://mapi.alipay.com/gateway.do";
        const string alipay_seller_email = "tracycjr@gmail.com";
        const string alipay_notify_url = "http://alipay.zmjiudian.com/notifyurl.aspx";
        const string alipay_wap_notify_url = "http://alipay.zmjiudian.com/notify_url.aspx"; //RSA认证通不过，只能用MD5了
        const string alipay_service = "alipay.security.risk.detect";


        public static bool CheckAlipaySecurity(PackageOrderInfo order,long orderid, string terminal_type)
        {
            AlipayEntity ae = new AlipayEntity();
            ae.service = "alipay.security.risk.detect";
            ae.partner = Com.Alipay.Config.Partner;
            ae.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ae.terminal_type = terminal_type;// Utils.IsMobile() ? "WAP" : "WEB";
            ae._input_charset = Com.Alipay.Config.Input_charset.ToLower();

            ae.order_no = orderid.ToString();
            ae.order_credate_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ae.order_category = "虚拟^旅行^住宿";
            ae.order_item_name = order.PackageName;
            ae.order_amount = order.Amount.ToString();
            ae.scene_code = "PAYMENT";
            ae.buyer_account_no = order.UserID == 0 ? order.ContactPhone : order.UserID.ToString();
            ae.buyer_reg_mobile = order.ContactPhone;
            ae.buyer_bind_mobile = order.ContactPhone;
            ae.buyer_scene_mobile = order.ContactPhone;
            ae.buyer_reg_date = DateTime.Now.ToString("yyyy-MM-dd");


            return SecurityForAlipayWeb(ae);
        }

        public static string BuildQueryString(IDictionary<string, string> dict, string charset)
        {
            StringBuilder sb = new StringBuilder();
            Encoding encoding = Encoding.GetEncoding(charset);
            foreach (KeyValuePair<string, string> p in dict)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.AppendFormat("{0}={1}", p.Key, HttpUtility.UrlEncode(p.Value, encoding));
            }
            return sb.ToString();
        }

        private static bool SecurityForAlipayWeb(AlipayEntity ae)
        {
            ////////////////////////////////////////////请求参数////////////////////////////////////////////

            // ///接口名称  接口方法名称。string service="";
            // ///合作者身份ID  签约的支付宝账号对应的支付宝唯一用户号。
            // ///  以2088开头的16位纯数字组成。string ="";
            // string partner = Com.Alipay.Config.Partner;
            // ///签名  参数签名
            // string sign="";
            // ///签名方式  签名方式：RSA、DSA
            // string sign_type="";
            // ///参数编码字符集  商户网站使用的编码格式，如utf-8、gbk、gb2312等。
            // string _input_charset= Com.Alipay.Config.Input_charset.ToLower();
            // ///接口请求时间  接口请求时间。精确到秒，格式为：yyyy-MM-dd HH:mm:ss。
            // string timestamp= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // ///终端类型  请求终端类型，范围：WEB，WAP，APP
            //  string terminal_type=Terminal_Type;
            // ///终端详情  请求终端详情，以“操作系统^版本号”为格式。
            // string terminal_info="";
            // ///通知返回url  支付宝服务器主动通知商户网站里指定的页面http路径。
            // ///  接收支付宝代扣事件消息的URL地址。string ="";
            // ///参数名称  参数string  参数="";
            // string notify_url=alipay_notify_url;
            // ///  string 业务参数="";
            // ///  string 订单信息(支付环节必须传，无订单无需传)="";
            // ///商户订单唯一标识号  商户订单唯一标识号
            // string order_no=orderId;
            // ///订单下单时间  订单下单时间。精确到秒，格式为：yyyy-MM-dd HH:mm:ss。
            // string order_credate_time=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // ///订单商品所在类目  订单商品所在类目，三级。
            // string order_category="虚拟^旅行^住宿";
            // ///订单商品名称  订单商品名称
            // string order_item_name=orderName;
            // ///订单商品所在城市  订单商品上架时所在的城市
            // string order_item_city="";
            // ///订单总金额  订单总的实际付款金额，取值范围为[0.01,100000000.00]，精确到小数点后两位。
            //   ///  如果currency是日币，则取值范围是[1,100000000]。string ="";
            // string order_amount=orderPrice;
            // ///订单产品单价  订单产品单价金额，
            // ///  取值范围为[0.01,100000000.00]，精确到小数点后两位。string ="";
            //   ///  如果currency是日币，则取值范围是[1,100000000]。string ="";
            ////string item_unit_price=unit_price;
            // ///订单产品数量  购买产品的数量（不可为小数）
            // ///string item_quantity="";
            // ///币种  商户本次交易的清算币种。请参见“6.4 支持的币种表”。
            // ///string currency="";
            // ///  外币清算时，本参数不可空。string ="";
            // ///场景编码  商户当前风险识别的场景编码，需要与支付宝约定，见场景编码约定。
            // string scene_code="PAYMENT";
            // ///买家业务处理时使用的银行卡号  买家在场景中使用的银行卡号，遵循卡号规则。
            // ///string buyer_scene_bankcard="";
            // ///买家业务处理时使用的银行卡类型  买家在场景中使用的银行卡类型，参见“6.5银行卡类型列表”。
            // ///string buyer_scene_bankcard_type="";
            // ///买家业务处理时使用的手机号  买家在场景中使用的手机号。如：充值手机号
            // string buyer_scene_mobile= mobile;
            // ///买家业务处理时使用的邮箱  买家在场景中使用的邮箱。如：充值用的邮箱
            // ///string buyer_scene_email="";
            // ///  string 买家信息（即用户信息）="";
            // ///买家账户编号  买家账户唯一标识，在商户网站的账号
            // string buyer_account_no= userid==0?mobile:userid.ToString();
            // ///买家注册Email  买家注册时留的Emailstring buyer_reg_email="";
            // ///买家注册手机号  买家注册时留的手机号
            // string buyer_reg_mobile=mobile;
            // ///买家绑定手机号  买家账户绑定的手机号
            // string buyer_bind_mobile=mobile;
            // ///买家账户等级  买家账户在商家的等级，范围：VIP（高级买家）, NORMAL(普通买家）。为空默认NORMAL
            // string buyer_grade="";
            // ///买家真实姓名  买家真实姓名
            // string buyer_real_name="";
            // ///买家注册时间  买家注册时间，精确到日，格式为：yyyy-MM-dd。
            // string buyer_reg_date="";
            // ///买家证件类型  买家证件类型，参见“6.3证件类型列表”。string buyer_identity_type="";
            // ///买家证件号码  买家证件号码string buyer_identity_no="";
            // ///买家绑定银行卡号  买家绑定银行卡号string buyer_bind_bankcard="";
            // ///买家绑定银行卡的卡类型  买家在场景中使用的银行卡类型，参见“6.5银行卡类型列表”string buyer_bind_bankcard_type="";
            // ///  string 收货地址及物流信息(实物订单必须传)="";
            // ///订单收货人姓名  订单收货人姓名string receiver_name="";
            // ///订单收货人手机  订单收货人手机号string receiver _mobile="";
            // ///订单收货人邮箱  订单收货人邮箱string receiver_email="";
            // ///订单收货人地址省份  订单收货人地址省份string receiver _state="";
            // ///  string ="";
            // ///订单收货人地址城市  订单收货人地址城市string receiver_city="";
            // ///订单收货人地址所在区  订单收货人地址所在区string receiver _district="";
            // ///订单收货人地址  订单收货人地址string receiver _address="";
            // ///订单收货人地址邮编  订单收货人地址邮编string receiver_zip="";
            // ///订单物流方式  订单物流方式，如快递，平邮，货到付款。string transport_type="";
            // ///  string 卖家信息（平台商传，非平台商无需传）="";
            // ///卖家账户编号  卖家账户唯一标识，在商户网站的账号string seller_account_no="";
            // ///卖家注册Email  卖家注册时留的Emailstring seller_reg_email="";
            // ///卖家注册手机号  卖家注册时留的手机号string seller_reg_moile="";
            // ///卖家绑定手机号  卖家账户绑定的手机号string seller_bind_mobile="";
            // ///卖家真实姓名  卖家真实姓名string seller_real_name="";
            // ///卖家注册时间  卖家注册时间，精确到日，格式为：yyyy-MM-dd。string seller_reg_date="";
            // ///卖家证件类型  卖家证件类型，参见“6.3证件类型列表”。string seller_identity_type="";
            // ///卖家证件号码  卖家证件号码string seller_identity_no="";
            // ///卖家绑定银行卡号  卖家绑定银行卡号string seller_bind_bankcard="";
            // ///卖家绑定的银行卡的卡类型  卖家绑定的银行卡的卡类型，参见“6.5银行卡类型列表”。string seller_bind_bankcard_type="";
            // ///  string 客户端环境信息="";
            // ///客户端ip  客户端ip，输入的必须是一个符合ipv4或ipv6的ip地址string env_client_ip="";
            // ///客户端mac  客户端macstring env_client_mac="";
            // ///客户端的imei  客户端IMEI串号string env_client_imei="";
            // ///客户端的imsi  客户端IMSI识别码string env_client_imsi="";
            // ///客户端的基带版本  客户端的基带版本号，即无线终端的调制解调器使用的驱动版本号string env_client_base_band="";
            // ///客户端的经纬度坐标  客户端所在的经纬度坐标，格式为：经度^纬度；如：119.306266^26.072595。string env_client_coordinates="";
            // ///客户端连接的基站信息  操作的客户端连接的基站小区编码CELLID和位置区域码LAC，格式为：CELLID^LAC；如：string env_client_base_station="";
            // ///客户端的屏幕分辨率  操作的客户端分辨率，格式为：水平像素^垂直像素；如：800^600string env_client_screen="";
            // ///客户端设备的统一识别码UUID  客户端设备的统一识别码Universally Unique Identifier，格式为：xxxxxxxx-xxxx-xxxx-xxxxxx-xxxxxxxxxx (8-4-4-4-12)string env_client_ uuid="";
            // ///IOS设备的UDID  操作的IOS设备的唯一识别码（40个字符）Unique Device Identifierstring env_client_ios_udid="";
            // ///JS SDK生成的 tokenID  设备指纹JS SDK包对应生成的 tokenID，格式如下：partnerCode_sceneCode_sessionId，JS SDK在WEB/WAP网站上必须部署，详见《设备指纹识别接入规范》string js_token_id="";




            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            Dictionary<string, string> dict = new Dictionary<string, string>();


            dict.Add("service", ae.service);
            dict.Add("partner", ae.partner);
            //dict.Add("sign", ae.sign);
            //dict.Add("sign_type", ae.sign_type);
            dict.Add("_input_charset", ae._input_charset);
            dict.Add("timestamp", ae.timestamp);
            dict.Add("terminal_type", ae.terminal_type);
            dict.Add("terminal_info", ae.terminal_info);
            dict.Add("notify_url", ae.notify_url);

            dict.Add("order_no", ae.order_no);
            dict.Add("order_credate_time", ae.order_credate_time);
            dict.Add("order_category", ae.order_category);
            dict.Add("order_item_name", ae.order_item_name);
            dict.Add("order_item_city", ae.order_item_city);
            dict.Add("order_amount", ae.order_amount);
            //dict.Add("item_unit_price", ae.item_unit_price);
            //dict.Add("item_quantity", ae.item_quantity);
            //dict.Add("currency", ae.currency);
            dict.Add("scene_code", ae.scene_code);
            //dict.Add("buyer_scene_bankcard", ae.buyer_scene_bankcard);
            //dict.Add("buyer_scene_bankcard_type", ae.buyer_scene_bankcard_type);
            dict.Add("buyer_scene_mobile", ae.buyer_scene_mobile);
            //dict.Add("buyer_scene_email", ae.buyer_scene_email);
            //dict.Add("买家信息（即用户信息）", ae.买家信息（即用户信息）);
            dict.Add("buyer_account_no", ae.buyer_account_no);
            //dict.Add("buyer_reg_email", ae.buyer_reg_email);
            dict.Add("buyer_reg_mobile", ae.buyer_reg_mobile);
            dict.Add("buyer_bind_mobile", ae.buyer_bind_mobile);
            dict.Add("buyer_grade", ae.buyer_grade);
            dict.Add("buyer_real_name", ae.buyer_real_name);
            dict.Add("buyer_reg_date", ae.buyer_reg_date);
            dict.Add("buyer_identity_type", ae.buyer_identity_type);
            //dict.Add("buyer_identity_no", ae.buyer_identity_no);
            //dict.Add("buyer_bind_bankcard", ae.buyer_bind_bankcard);
            //dict.Add("buyer_bind_bankcard_type", ae.buyer_bind_bankcard_type);
            //dict.Add("收货地址及物流信息(实物订单必须传)", ae.收货地址及物流信息(实物订单必须传));
            //dict.Add("receiver_name", ae.receiver_name);
            //dict.Add("receiver _mobile", ae.receiver _mobile);
            //dict.Add("receiver_email", ae.receiver_email);
            //dict.Add("receiver _state", ae.receiver _state);
            //dict.Add("", ae.);
            //dict.Add("receiver_city", ae.receiver_city);
            //dict.Add("receiver _district", ae.receiver _district);
            //dict.Add("receiver _address", ae.receiver _address);
            //dict.Add("receiver_zip", ae.receiver_zip);
            //dict.Add("transport_type", ae.transport_type);
            //dict.Add("卖家信息（平台商传，非平台商无需传）", ae.卖家信息（平台商传，非平台商无需传）);
            //dict.Add("seller_account_no", ae.seller_account_no);
            //dict.Add("seller_reg_email", ae.seller_reg_email);
            //dict.Add("seller_reg_moile", ae.seller_reg_moile);
            //dict.Add("seller_bind_mobile", ae.seller_bind_mobile);
            //dict.Add("seller_real_name", ae.seller_real_name);
            //dict.Add("seller_reg_date", ae.seller_reg_date);
            //dict.Add("seller_identity_type", ae.seller_identity_type);
            //dict.Add("seller_identity_no", ae.seller_identity_no);
            //dict.Add("seller_bind_bankcard", ae.seller_bind_bankcard);
            //dict.Add("seller_bind_bankcard_type", ae.seller_bind_bankcard_type);
            //dict.Add("客户端环境信息", ae.客户端环境信息);
            //dict.Add("env_client_ip", ae.env_client_ip);
            //dict.Add("env_client_mac", ae.env_client_mac);
            //dict.Add("env_client_imei", ae.env_client_imei);
            //dict.Add("env_client_imsi", ae.env_client_imsi);
            //dict.Add("env_client_base_band", ae.env_client_base_band);
            //dict.Add("env_client_coordinates", ae.env_client_coordinates);
            //dict.Add("env_client_base_station", ae.env_client_base_station);
            //dict.Add("env_client_screen", ae.env_client_screen);
            //dict.Add("env_client_uuid", ae.env_client_uuid);
            //dict.Add("env_client_ios_udid", ae.env_client_ios_udid);
            //dict.Add("js_token_id", ae.js_token_id);

            dict.Add("sign", SignForAlipay(dict, Com.Alipay.Config.Sign_type.ToUpper()));
            dict.Add("sign_type", Com.Alipay.Config.Sign_type);

            //alipay_web_gateway + "?" +
            string parms = BuildQueryString(dict, Com.Alipay.Config.Input_charset);


            string retXML = HttpHelper.HttpGet(alipay_web_gateway, parms);

            WriteLog(retXML);

            
            //	发生错误时输出：
            //<?xml version="1.0" encoding="GBK" ?>
            //<alipay>
            //  <is_success>F</is_success>
            //  <error>BUYER_NOT_EXIST</error>
            //  <sign>d7a3609c5b5d57aab4447381999e2222</sign>
            //  <sign_type>MD5</sign_type>
            //</alipay>

            //            <?xml version="1.0" encoding="utf-8"?>
            //<alipay>
            //    <is_success>T</is_success>
            //    <request>
            //        <param name="order_credate_time">2014-07-07 14:45:19</param>
            //        <param name="scene_code">PAYMENT</param>
            //        <param name="buyer_reg_mobile">18021036971</param>
//...
            //        <param name="partner">2088211546603233</param>
            //        <param name="buyer_reg_date">2014-07-07</param>
            //    </request>
            //    <response>
            //        <alipay.security.risk.detect>
            //            <risk_code>000000</risk_code>
            //            <risk_level>0</risk_level>
            //        </alipay.security.risk.detect>
            //    </response>
            //    <sign>mYpO65pIxY9T+LW3XNVxe3OAEJUXBthcmZL2IjknpdGSDj03Rgrh7YIiuKgPm7O3F+gZAvo3RMXYnnd/JsWOqPxB1qsDVHyF/ban55unZ0FNhUNZuql8/bGohUyJzBZX71OiPRsOs3mQVi3+3BhIE3Sf16iKdFoiSfQzn4O+xc4=</sign>
            //    <sign_type>RSA</sign_type>
            //</alipay>


            return true;
        }

        private static void WriteLog(string msg)
        {
            try
            {
                string LogFile = string.Format(@"d:\log\alipay\checkLog{0}{1}.txt", DateTime.Now.Month, DateTime.Now.Day);
                File.AppendAllText(LogFile, string.Format("{0}\r\n{1}\r\n\r\n", DateTime.Now, msg));
            }
            catch { }
        }

        private static  string SignForAlipay(Dictionary<string, string> dict, string sign_type)
        {
            Dictionary<string, string> filterd = Com.Alipay.Core.FilterPara(dict);
            SortedDictionary<string, string> sorted = new SortedDictionary<string, string>(filterd);
            return Com.Alipay.Submit.BuildRequestMysign(new Dictionary<string, string>(sorted), sign_type);
        }

       

    }
}