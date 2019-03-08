using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.CouponService.Contracts;
using HJD.CouponService.Contracts.Entity;
using HJD.HotelServices.Contracts;
using ProductService.Contracts.Entity;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class WalletResult
    {
        //[DataMember]
        //public List<OriginCoupon> cashList { get; set; }

        [DataMember]
        public List<AcquiredCoupon> couponList { get; set; }

        [DataMember]
        public int UsedCouponAmount { get; set; }

        [DataMember]
        public int CanUseCouponAmount { get; set; }
    }

    [Serializable]
    [DataContract]
    public class PointResult
    {
        [DataMember]
        public List<PointsEntity> pointList { get; set; }

        [DataMember]
        public List<PointsConsumeEntity> pointConsumeList { get; set; }

        [DataMember]
        public int CanUsePoints { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CouponResult
    {
        [DataMember]
        public List<AcquiredCoupon> couponList { get; set; }

        [DataMember]
        public List<UseCouponRecordEntity> couponRecordList { get; set; }

        [DataMember]
        public int CanUseCoupon { get; set; }
    }

    [Serializable]
    [DataContract]
    public class RoomCouponResult
    {
        [DataMember]
        public List<RoomCouponOrderEntity> couponList { get; set; }


    }

    [Serializable]
    [DataContract]
    public class RoomCouponOrderEntity:BaseCouponResult
    {

        ///// <summary>
        ///// 拼团状态
        ///// </summary>
        //[DataMember]
        //public int GroupState { get; set; }

        ///// <summary>
        ///// 参团人数
        ///// </summary>
        //[DataMember]
        //public int GroupPeopleCount { get; set; }

        ///// <summary>
        ///// 团购缺少人数
        ///// </summary>
        //[DataMember]
        //public int GroupShortageCount { get; set; }

        ///// <summary>
        ///// 团购人员明细
        ///// </summary>
        //[DataMember]
        //public List<GroupPurchaseDetailEntity> GroupPeople { get; set; }

        [DataMember]
        public GroupPurchaseEntity GroupPurchase { get; set; }

        [DataMember]
        public CouponOrderStepGroupEntity CouponOrderStepGroup { get; set; }
    }

    public class ExchangeCouponModel : ExchangeCouponEntity
    {
        /// <summary>
        /// 前台是否可兑换房券
        /// </summary>
        [DataMember]
        public bool CanExchange { get; set; }

        /// <summary>
        /// 是否可预约
        /// </summary>
        [DataMember]
        public bool IsBook { get; set; }
        /// <summary>
        /// 是否可取消预约
        /// </summary>
        [DataMember]
        public bool IsCancelBook { get; set; }

        /// <summary>
        /// 预约时间
        /// </summary>
        [DataMember]
        public List<BookUserDateInfoEntity> BookUserDateList { get; set; }

        [DataMember]
        public List<string> ShopWriteOffCouponTipList { get; set; }
        /// <summary>
        /// 预约出行人信息
        /// </summary>
        //[DataMember]
        //public List<BookUserInfoEntity> BookUserInfoList { get; set; }
        ///// <summary>
        ///// 现金券名称
        ///// </summary>
        //[DataMember]
        //public string CashCouponAmountName { get; set; }

        ///// <summary>
        ///// 是否定金订单
        ///// </summary>
        //[DataMember]
        //public bool IsDepositOrder { get; set; }

        ///// <summary>
        ///// 标题
        ///// </summary>
        //[DataMember]
        //public string PageTitle { get; set; }


    }

    [Serializable]
    [DataContract]
    public class BaseCouponResult
    {
        [DataMember]
        public int ActivityID { get; set; }
        [DataMember]
        public int GroupId { get; set; }
        [DataMember]
        public long CouponOrderID { get; set; }

        [DataMember]
        public string HotelName { get; set; }

        [DataMember]
        public string PackageName { get; set; }

        [DataMember]
        public decimal CouponPrice { get; set; }

        [DataMember]
        public decimal TotalPrice { get; set; }

        [DataMember]
        public int CashCouponID { get; set; }

        [DataMember]
        public Decimal CashCouponAmount { get; set; } 
        
        [DataMember]
        public string VoucherIDs { get; set; }


        [DataMember]
        public int SKUID{ get; set; }

        [DataMember]
        public Decimal VoucherAmount { get; set; }


        [DataMember]
        public Decimal UserUseHousingFundAmount { get; set; }


        /// <summary>
        /// 1为待支付（已提交） 2 已支付（未消费） 3 已消费 4 已取消 5 已退款(单券状态)
        /// </summary>
        [DataMember]
        public int State { get; set; }

        [DataMember]
        public List<ExchangeCouponModel> ExchangeCouponList { get; set; }



        [DataMember]
        public string PayResultMessage { get; set; }

        [DataMember]
        public Int32 ActivityType { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public long CID { get; set; }

        [DataMember]
        public bool IsVIPInvatation { get; set; }

        [DataMember]
        public long UserId { get; set; }

        [DataMember]
        public string PhoneNum { get; set; }

        [DataMember]
        public bool IsNewRegistUser { get; set; }

        [DataMember]
        public int SupplierID { get; set; }

        [DataMember]
        public decimal TotalPoints { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ProductCouponResult
    {
        [DataMember]
        public List<ProductCouponOrderResult> productCouponList { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ProductCouponOrderResult : BaseCouponResult
    {
        /// <summary>
        /// 订单icon
        /// </summary>
        [DataMember]
        public string OrderIcon { get; set; }
        ///// <summary>
        ///// 打包产品
        ///// </summary>
        [DataMember]
        public bool IsPackage { get; set; }

        [DataMember]
        public CouponOrderStepGroupEntity CouponOrderStepGroup { get; set; }

        [DataMember]
        public GroupPurchaseEntity GroupPurchase { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int SkuID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string SkuName { get; set; }

        /// <summary>
        /// 消费券类型 17团购 18 团购房券
        /// </summary>
        [DataMember]
        public int SPUTempType { get; set; }

        /// <summary>
        /// 产品类型父id
        /// </summary>
        [DataMember]
        public int CategoryParentId { get; set; }

        /// <summary>
        /// 产品类型id
        /// </summary>
        [DataMember]
        public int CategoryId { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [DataMember]
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// 券码开始时间
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 注意事项
        /// </summary>
        [DataMember]
        public List<string> Notice { get; set; }

        /// <summary>
        /// 套餐内容
        /// </summary>
        [DataMember]
        public List<string> PackageInfoList { get; set; }
        /// <summary>
        /// 更新信息
        /// </summary>
        [DataMember]
        public Dictionary<string, string > DicProperties { get; set; }

        /// <summary>
        /// 购买时填写的补充信息
        /// </summary>
        [DataMember]
        public HJD.HotelManagementCenter.Domain.TemplateDataEntity TemplateData { get; set; }

        /// <summary>
        /// 出行人信息
        /// </summary>
        [DataMember]
        public List<TravelPersonEntity> TravelPerson { get; set; }

        /// <summary>
        /// 订单类型 6：大团购订单（6对应了spu的属性）
        /// </summary>
        [DataMember]
        public int ExchangeCouopnOrderType { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [DataMember]
        public string PageTitle { get; set; }
    }
}