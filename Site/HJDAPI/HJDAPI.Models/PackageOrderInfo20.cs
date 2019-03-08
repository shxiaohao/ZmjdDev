using HJD.HotelPrice.Contract.DataContract;
using HJD.HotelPrice.Contract.DataContract.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class PackageOrderInfo20
    {
        public PackageOrderInfo20()
        {
            DailyItems = new List<OrderDetailDailyEntity>();
            Notice = new List<OrderDetailItem>();
            OrderContactList = new List<OrderContactInfoEntity>();
            LoadFile = new List<KeyValueEntity>();
        }

        /// <summary>
        /// 订单查看详情链接
        /// </summary>
        public string LookDetailURL { get; set; }

        /// <summary>
        /// 补款连接
        /// </summary>
        public string OrderAddPayURL { get; set; }
        /// <summary>
        /// 补款金额
        /// </summary>
        public decimal OrderAddPayPrice { get; set; }

        public int UserFinalTotalPayAmount { get; set; }

        public int UserShouldPayAmount { get; set; }
        public int CanWriteComment { get; set; }
        public long OrderID { get; set; }
        public int ActiveRebate { get; set; }
        public int ActiveRebateState { get; set; }
        public string ActiveURL { get; set; }
        public int CashCouponAmount { get; set; }

        public int UserUseCashCouponAmount { get; set; }

        /// <summary>
        /// 满减券金额
        /// </summary>
        public decimal CashCouponDiscount { get; set; }
        /// <summary>
        /// 满减券类型
        /// </summary>
        public string CashCouponDiscountName { get; set; }

        /// <summary>
        /// 住基金
        /// </summary>
        public decimal UserUseHousingFundAmount { get; set; }
        
        /// <summary>
        /// 酒店订单图标
        /// </summary>
        public string HotelIcon { get; set; }


        public List<OrderDetailDailyEntity> DailyItems { get; set; }
        public List<OrderDetailItem> Notice { get; set; }

        public InvoiceInfoEntity InvoiceInfo { get; set; }

        public string OrderNotice { get; set; }
        public DateTime LastCancelTime { get; set; }

        public string HotelName { get; set; }
        public int HotelID { get; set; }
        public string PackageName { get; set; }
        public string RoomDescription { get; set; }

        public decimal Amount { get; set; }
        public string PayInfo { get; set; }

        public DateTime CheckIn { get; set; }

        public string OrderDateName { get;set;}
        public string OrderDateDescription { get; set; }

        public string Contact { get; set; }

        public string ContactPhone { get; set; }

        public long ID { get; set; }
        public long CID { get; set; }

        public int NightCount { get; set; }

        public string Note { get; set; }

        public int CommentID { get; set; }

        public int PID { get; set; }

        public int RoomCount { get; set; }

        public int State { get; set; }

        public string StateName { get; set; }

        public int SubDate { get; set; }

        public DateTime SubmitDate { get; set; }

        public int Type { get; set; }

        public long UserID { get; set; }

        public int PayType { get; set; }

        /// <summary>
        /// 支付标签图片Url数组
        /// </summary>
        public List<string> PayLabelUrls { get; set; }

        /// <summary>
        /// alipay:支付宝
        /// upay:U付
        /// tenpay:微信支付
        /// all:已经有的全部支付方式
        /// 如果PayChannels没有任何值则不跳到支付渠道选择页面
        /// </summary>
        public List<string> PayChannels { get; set; }

        /// <summary>
        /// 订单详情页可以查看的按钮数组
        /// pay
        /// writecomment
        /// cancel
        /// delete
        /// </summary>
        public List<string> CanShowButtons;

        public List<TravelPersonOrderEntity> TravelPersonList { get; set; }

        /// <summary>
        /// 发票状态 -1不开发票，0未开发票，1已开发票
        /// </summary>
        public int InvoiceType { get; set; }

        public DateTime OrderContactsEndTime { get; set; }

        public string OrderContactsTip { get; set; }

        public List<OrderContactInfoEntity> OrderContactList { get; set; }

        public List<KeyValueEntity> LoadFile { get; set; }

        public string OrderDetailTip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DateSelectType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PackageType { get; set; }
    }

    public class InvoiceInfoEntity
    {
        public string Title { get; set; }

        public string Address { get; set; }
        public string Contact { get; set; }
        public DateTime CreateTime { get; set; }
        public int Price { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string ShippingType { get; set; }
        public string TelPhone { get; set; }
        public string TaxNumber { get; set; }

        /// <summary>
        /// 发票形式
        /// </summary>
        public int InvoiceFormType { get; set; }

        /// <summary>
        /// 打印 发票 类型
        /// </summary>
        public string InvoicePrintTypeName { get; set; }
        /// <summary>
        /// 发票支付类型 1货币支付 2积分支付
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 电子发票连接
        /// </summary>
        public string ElectronicInvoiceUrl { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 邮费 price
        /// </summary>
        public string PostagePrice { get; set; }
        /// <summary>
        /// 邮费 point
        /// </summary>
        public string PostagePoints { get; set; }
    }
    
    public class OrderDetailDailyEntity
    {
        public OrderDetailDailyEntity() { Items = new List<OrderDetailItem>(); }        
        public DateTime Day { get; set; }        
        public List<OrderDetailItem> Items { get; set; }        
       
    }

    public sealed class OrderDetailItem
    {
        public int ID { get; set; }
        public int Price { get; set; }
       public string Description { get; set; }
       public int Type { get; set; }
       public int DateType { get; set; }
       public DateTime Date { get; set; }
     
    }
    
    #region BoTao 查询余额 发送验证码 冻结指定金额 确认扣款

    public class BoTaoBookParam : BaseSignParam
    {
        /// <summary>
        /// 酒店名称
        /// </summary>
        public string hotelname { get; set; }
        /// <summary>
        /// 支付短信凭证
        /// </summary>
        public string paymsgticket { get; set; }
        /// <summary>
        /// 支付短信验证码
        /// </summary>
        public string paymsgcode { get; set; }
        /// <summary>
        /// 是否需要验证短信验证码 仅当一笔订单或房券交易存在取消冻结记录（再次冻结无需验证短信）才有效
        /// </summary>
        public bool checkpaycode { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string merchantcode { get; set; }
        /// <summary>
        /// 商家订单号 一个订单号或者房券券号（？？？券号只有支付成功才有的 铂汇用户提前生成券号？？？？）
        /// 商家内部系统此订单号唯一
        /// </summary>
        public string merchantorderno { get; set; }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string packagename { get; set; }
        /// <summary>
        /// 套餐图片Url 必填
        /// </summary>
        public string packagepic { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 产品消费类型
        /// </summary>
        public ThirdPartyProductType consumetype { get; set; }
        /// <summary>
        /// 消费房间数
        /// </summary>
        public Int32 qty { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public Int32 mans { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string goodsname { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 预订时间 商家预订时间（购买时间？）
        /// </summary>
        public DateTime booktime { get; set; }
        /// <summary>
        /// 消费开始时间
        /// </summary>
        public string begintime { get; set; }
        /// <summary>
        /// 消费结束时间 endtime
        /// </summary>
        public string endtime { get; set; }
        /// <summary>
        /// 房券的过期时间 其他产品类型可不传
        /// </summary>
        public string indate { get; set; }
        /// <summary>
        /// 客人名字
        /// </summary>
        public string guestname { get; set; }
        /// <summary>
        /// 会员手机号
        /// </summary>
        public string mebphone { get; set; }
    }

    public class BoTaoSendMsg : BaseSignParam
    {
        /// <summary>
        /// 商家编号
        /// </summary>
        public string merchantcode { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal amount { get; set; }
    }

    public class BoTaoResponse
    {
        /// <summary>
        /// 版本信息
        /// </summary>
        public string ver { get; set; }
        /// <summary>
        /// 本次操作的结果
        /// </summary>
        public bool result { get; set; }
        /// <summary>
        /// 可空的数据
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// 错误消息码
        /// </summary>
        public string errorcode { get; set; }
        /// <summary>
        /// 错误消息内容
        /// </summary>
        public string errormsg { get; set; }
    }
    #endregion

    public class BoTaoOrderEntity
    {
        /// <summary>
        /// 订单号 多个（如房券）以英文半角逗号分隔
        /// </summary>
        public string merchantorderno { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 酒店名称
        /// </summary>
        public string hotelname { get; set; }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string packagename { get; set; }
        /// <summary>
        /// 套餐图片Url 必填
        /// </summary>
        public string packagepic { get; set; }
        /// <summary>
        /// 消费房间数
        /// </summary>
        public Int32 qty { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public Int32 mans { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string goodsname { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 消费开始时间
        /// </summary>
        public DateTime begintime { get; set; }
        /// <summary>
        /// 消费结束时间 endtime
        /// </summary>
        public DateTime endtime { get; set; }
        /// <summary>
        /// 房券的过期时间 其他产品类型可不传
        /// </summary>
        public DateTime? indate { get; set; }
        /// <summary>
        /// 客人名称 房券无显示手机号 酒店订单有
        /// </summary>
        public string guestname { get; set; }
        /// <summary>
        /// 客人手机号
        /// </summary>
        public string mobilePhone { get; set; }
    }

}