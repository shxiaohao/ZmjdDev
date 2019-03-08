using HJD.CouponService.Contracts.Entity;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public  class OrderSubmit
    {
        [DataMember]
        public int packageID { get; set; }
        [DataMember]
        public string name { get; set; }
    }

    [DataContract]
    public class CouponOrderResult
    {
        public CouponOrderResult()
        {
            PayChannels = new List<string>();
            ExchangeIdList = new List<int>();
        }

        [DataMember]
        public int Success { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public long OrderID { get; set; }
        [DataMember]
        public long UserID { get; set; }

        [DataMember]
        public List<int> ExchangeIdList { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        [DataMember]
        public string GoodsInfo { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        [DataMember]
        public Decimal Amount { get; set; }
        [DataMember]
        public List<string> PayChannels { get; set; }
        [DataMember]
        public BuyCouponCheckNumResult CouponCheckNumResult { get; set; }
    }

    [Serializable]
    [DataContract]
    public class SubmitCouponOrderModel
    {
        public SubmitCouponOrderModel() { UseCashCouponInfo = new UseCashCouponItem(); UseVoucherInfo = new UseVoucherInfoEntity(); }
        [DataMember]
        public int ActivityID { get; set; }

        [DataMember]
        public bool IsVIPInvatation{ get; set; }

        [DataMember]
        public int ActivityType { get; set; }
        [DataMember]
        public long UserID { get; set; }
        [DataMember]
        public long CID { get; set; }
        [DataMember]
        public Int32 SKUID { get; set; }
        [DataMember]
        public string PhoneNo  { get; set; }
        [DataMember]
        public string RealName { get; set; }
        /// <summary>
        /// 多个联系人请用英文半角","号相隔
        /// </summary>
        [DataMember]
        public string Contacts { get; set; }
        [DataMember]
        public List<ProductAndNum> OrderItems { get; set; }
        /// <summary>
        /// 控制并发lock之后是否执行数据库插入
        /// </summary>
        [DataMember]
        public int Flag { get; set; }
        [DataMember]
        public ExchangeCouponAddInfoItem AddInfo { get; set; }

        [DataMember]
        public int SceneType { get; set; }

        /// <summary>
        /// 团号
        /// </summary>
        [DataMember]
        public int GroupId { get; set; }
        /// <summary>
        /// 是否团购
        /// </summary>
        [DataMember]
        public bool IsGroupActivity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string OpenId { get; set; }

        //用户使用住基金金额
        [DataMember]
        public Decimal UserUseHousingFundAmount { get; set; }

        [DataMember] 
        public UseCashCouponItem UseCashCouponInfo { get; set; }

        [DataMember]
        public UseVoucherInfoEntity UseVoucherInfo { get; set; }

        [DataMember]
        public TemplateDataEntity TemplateData { get; set; }

        [DataMember]
        public List<int> TravelId { get; set; }

        [DataMember]
        public int FromWeixinUid { get; set; }

        [DataMember]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 大团购支付尾款时该参数必传
        /// </summary>
        [DataMember]
        public long CouponOrderID { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UseVoucherInfoEntity
    {
        public UseVoucherInfoEntity() { UseVoucherIDList = new List<Int32>(); }
        [DataMember]
        public decimal UseVoucherAmount { get; set; }
        [DataMember]
        public List<int> UseVoucherIDList { get; set; }
    }

    [Serializable]
    [DataContract]
    public class SubmitCouponOrderForWxapp
    {
        /// <summary>
        /// 现金券id
        /// </summary>
        [DataMember]
        public int CashCouponID { get; set; }

        /// <summary>
        /// 现金券抵扣金额
        /// </summary>
        [DataMember]
        public decimal CashCouponAmount { get; set; }

        [DataMember]
        public int ActivityID { get; set; }

        [DataMember]
        public int ActivityType { get; set; }

        [DataMember]
        public int SKUID { get; set; }

        [DataMember]
        public int GroupId { get; set; }

        [DataMember]
        public long UserID { get; set; }

        [DataMember]
        public string PhoneNo { get; set; }

        [DataMember]
        public string RealName { get; set; }

        [DataMember]
        public string OpenId { get; set; }

        /// <summary>
        /// 多个联系人请用英文半角","号相隔
        /// </summary>
        [DataMember]
        public string Contacts { get; set; }

        /// <summary>
        /// 初始用来标明套餐ID 作为产品ID标志 (iphone6是产品)
        /// </summary>
        [DataMember]
        public int SourceID { get; set; }

        /// <summary>
        /// 小类型 初始用来区分平日和周末 作为产品版本 (iphone6 16G 64G 128G是产品版本)
        /// </summary>
        [DataMember]
        public int Type { get; set; }

        /// <summary>
        /// 相应数量
        /// </summary>
        [DataMember]
        public int Number { get; set; }

        /// <summary>
        /// 相应数量
        /// </summary>
        [DataMember]
        public Decimal Price { get; set; }

        /// <summary>
        /// 控制并发lock之后是否执行数据库插入
        /// </summary>
        [DataMember]
        public int Flag { get; set; }

        [DataMember]
        public int CID { get; set; }

        [DataMember]
        public bool IsGroupActivity { get; set; }

        /// <summary>
        /// 模板信息
        /// </summary>
        [DataMember]
        public TemplateDataEntity TemplateData { get; set; }

        /// <summary>
        /// 出行人id
        /// </summary>
        [DataMember]
        public List<int> TravelId { get; set; }
    }

    [DataContract]
    public class ProductAndNum
    {
        /// <summary>
        /// 初始用来标明套餐ID 作为产品ID标志 (iphone6是产品)
        /// </summary>
        [DataMember]
        public int SourceID { get; set; }

        /// <summary>
        /// 小类型 初始用来区分平日和周末 作为产品版本 (iphone6 16G 64G 128G是产品版本)
        /// </summary>
        [DataMember]
        public int Type { get; set; }

        /// <summary>
        /// 相应数量
        /// </summary>
        [DataMember]
        public int Number { get; set; }

        /// <summary>
        /// 相应价格
        /// </summary>
        [DataMember]
        public Decimal Price { get; set; }

        /// <summary>
        /// 分销价格
        /// </summary>
        [DataMember]
        public decimal RetailPrice { get; set; }

        /// <summary>
        /// 相应积分
        /// </summary>
        [DataMember]
        public int Points { get; set; }
    }
}