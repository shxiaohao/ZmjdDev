using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Common.Alipay
{
    public class AlipayEntity
    {
        ///接口名称  接口方法名称。
        public string service = "";
        ///合作者身份ID  签约的支付宝账号对应的支付宝唯一用户号。
        ///  以2088开头的16位纯数字组成。string ="";
        public string partner = Com.Alipay.Config.Partner;
        ///签名  参数签名
        public string sign = "";
        ///签名方式  签名方式：RSA、DSA
        public string sign_type = "";
        ///参数编码字符集  商户网站使用的编码格式，如utf-8、gbk、gb2312等。
        public string _input_charset = Com.Alipay.Config.Input_charset.ToLower();
        ///接口请求时间  接口请求时间。精确到秒，格式为：yyyy-MM-dd HH:mm:ss。
        public string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        ///终端类型  请求终端类型，范围：WEB，WAP，APP
        public string terminal_type = "";
        ///终端详情  请求终端详情，以“操作系统^版本号”为格式。
        public string terminal_info = "";
        ///通知返回url  支付宝服务器主动通知商户网站里指定的页面http路径。
        ///  接收支付宝代扣事件消息的URL地址。string ="";
        ///参数名称  参数string  参数="";
        public string notify_url = "";
        ///  string 业务参数="";
        ///  string 订单信息(支付环节必须传，无订单无需传)="";
        ///商户订单唯一标识号  商户订单唯一标识号
        public string order_no = "";
        ///订单下单时间  订单下单时间。精确到秒，格式为：yyyy-MM-dd HH:mm:ss。
        public string order_credate_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        ///订单商品所在类目  订单商品所在类目，三级。
        public string order_category = "虚拟^旅行^住宿";
        ///订单商品名称  订单商品名称
        public string order_item_name = "";
        ///订单商品所在城市  订单商品上架时所在的城市
        public string order_item_city = "";
        ///订单总金额  订单总的实际付款金额，取值范围为[0.01,100000000.00]，精确到小数点后两位。
        ///  如果currency是日币，则取值范围是[1,100000000]。string ="";
        public string order_amount = "";
        ///订单产品单价  订单产品单价金额，
        ///  取值范围为[0.01,100000000.00]，精确到小数点后两位。string ="";
        ///  如果currency是日币，则取值范围是[1,100000000]。string ="";
        //string item_unit_price=unit_price;
        ///订单产品数量  购买产品的数量（不可为小数）
        ///string item_quantity="";
        ///币种  商户本次交易的清算币种。请参见“6.4 支持的币种表”。
        ///string currency="";
        ///  外币清算时，本参数不可空。string ="";
        ///场景编码  商户当前风险识别的场景编码，需要与支付宝约定，见场景编码约定。
        public string scene_code = "PAYMENT";
        ///买家业务处理时使用的银行卡号  买家在场景中使用的银行卡号，遵循卡号规则。
        ///string buyer_scene_bankcard="";
        ///买家业务处理时使用的银行卡类型  买家在场景中使用的银行卡类型，参见“6.5银行卡类型列表”。
        ///string buyer_scene_bankcard_type="";
        ///买家业务处理时使用的手机号  买家在场景中使用的手机号。如：充值手机号
        public string buyer_scene_mobile = "";
        ///买家业务处理时使用的邮箱  买家在场景中使用的邮箱。如：充值用的邮箱
        ///string buyer_scene_email="";
        ///  string 买家信息（即用户信息）="";
        ///买家账户编号  买家账户唯一标识，在商户网站的账号
        public string buyer_account_no = "";// userid == 0 ? mobile : userid.ToString();
        ///买家注册Email  买家注册时留的Emailstring buyer_reg_email="";
        ///买家注册手机号  买家注册时留的手机号
        public string buyer_reg_mobile = "";
        ///买家绑定手机号  买家账户绑定的手机号
        public string buyer_bind_mobile = "";
        ///买家账户等级  买家账户在商家的等级，范围：VIP（高级买家）, NORMAL(普通买家）。为空默认NORMAL
        public string buyer_grade = "";
        ///买家真实姓名  买家真实姓名
        public string buyer_real_name = "";
        ///买家注册时间  买家注册时间，精确到日，格式为：yyyy-MM-dd。
        public string buyer_reg_date = "";
        ///买家证件类型  买家证件类型，参见“6.3证件类型列表”。
        public string buyer_identity_type = "";
        ///买家证件号码  买家证件号码string buyer_identity_no="";
        ///买家绑定银行卡号  买家绑定银行卡号string buyer_bind_bankcard="";
        ///买家绑定银行卡的卡类型  买家在场景中使用的银行卡类型，参见“6.5银行卡类型列表”string buyer_bind_bankcard_type="";
        ///  string 收货地址及物流信息(实物订单必须传)="";
        ///订单收货人姓名  订单收货人姓名string receiver_name="";
        ///订单收货人手机  订单收货人手机号string receiver _mobile="";
        ///订单收货人邮箱  订单收货人邮箱string receiver_email="";
        ///订单收货人地址省份  订单收货人地址省份string receiver _state="";
        ///  string ="";
        ///订单收货人地址城市  订单收货人地址城市string receiver_city="";
        ///订单收货人地址所在区  订单收货人地址所在区string receiver _district="";
        ///订单收货人地址  订单收货人地址string receiver _address="";
        ///订单收货人地址邮编  订单收货人地址邮编string receiver_zip="";
        ///订单物流方式  订单物流方式，如快递，平邮，货到付款。string transport_type="";
        ///  string 卖家信息（平台商传，非平台商无需传）="";
        ///卖家账户编号  卖家账户唯一标识，在商户网站的账号
        // public string seller_account_no = "";
        ///卖家注册Email  卖家注册时留的Emailstring seller_reg_email="";
        ///卖家注册手机号  卖家注册时留的手机号string seller_reg_moile="";
        ///卖家绑定手机号  卖家账户绑定的手机号string seller_bind_mobile="";
        ///卖家真实姓名  卖家真实姓名string seller_real_name="";
        ///卖家注册时间  卖家注册时间，精确到日，格式为：yyyy-MM-dd。
        // public string seller_reg_date = "";
        ///卖家证件类型  卖家证件类型，参见“6.3证件类型列表”。string seller_identity_type="";
        ///卖家证件号码  卖家证件号码string seller_identity_no="";
        ///卖家绑定银行卡号  卖家绑定银行卡号string seller_bind_bankcard="";
        ///卖家绑定的银行卡的卡类型  卖家绑定的银行卡的卡类型，参见“6.5银行卡类型列表”。string seller_bind_bankcard_type="";
        ///  string 客户端环境信息="";
        ///客户端ip  客户端ip，输入的必须是一个符合ipv4或ipv6的ip地址
        public string env_client_ip = "";
        ///客户端mac  客户端mac
        public string env_client_mac = "";
        ///客户端的imei  客户端IMEI串号
        public string env_client_imei = "";
        ///客户端的imsi  客户端IMSI识别码
        public string env_client_imsi = "";
        ///客户端的基带版本  客户端的基带版本号，即无线终端的调制解调器使用的驱动版本号
        public string env_client_base_band = "";
        ///客户端的经纬度坐标  客户端所在的经纬度坐标，格式为：经度^纬度；如：119.306266^26.072595。
        public string env_client_coordinates = "";
        ///客户端连接的基站信息  操作的客户端连接的基站小区编码CELLID和位置区域码LAC，格式为：CELLID^LAC；如：
        public string env_client_base_station = "";
        ///客户端的屏幕分辨率  操作的客户端分辨率，格式为：水平像素^垂直像素；如：800^600
        public string env_client_screen = "";
        ///客户端设备的统一识别码UUID  客户端设备的统一识别码Universally Unique Identifier，格式为：xxxxxxxx-xxxx-xxxx-xxxxxx-xxxxxxxxxx (8-4-4-4-12)
        public string env_client_uuid = "";
        ///IOS设备的UDID  操作的IOS设备的唯一识别码（40个字符）Unique Device Identifier
        public string env_client_ios_udid = "";
        ///JS SDK生成的 tokenID  设备指纹JS SDK包对应生成的 tokenID，格式如下：partnerCode_sceneCode_sessionId，JS SDK在WEB/WAP网站上必须部署，详见《设备指纹识别接入规范》
        public string js_token_id = "";

    }
}