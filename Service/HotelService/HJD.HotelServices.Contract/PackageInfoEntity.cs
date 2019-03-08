using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    public class PackageInfoEntity
    {
        public PackageInfoEntity()
        {
            DailyItems = new List<PackageDailyEntity>();
            Suggest = new BuyMembershipSuggest();
            CustomerAskPrice = new CommentTextAndUrl();
            OrderPageShowBuyVIP = new CommentTextAndUrl();
            PurchaseNotes = new CommentTextAndUrl();
        }

        /// <summary>
        /// 是否在中国
        /// </summary>
        [DataMember]
        public bool InChina { get; set; }

        /// <summary>
        /// 控制 前端显示样式
        /// 0：酒店显示入住日期和离店日期
        /// 1： 机酒游轮显示 出发日
        /// </summary>
        [DataMember]
        public int CheckTimeShow { get; set; }

        /// <summary>
        /// 是否需要证件信息。目前判断 游轮套餐（pacakgeType=2）并且证件数大于0（MaxHotelPeople>0）
        /// </summary>
        [DataMember]
        public bool IsNeedDocumentInformation { get; set; }

        /// <summary>
        /// 订单详情页    购买须知内容
        /// </summary>
        [DataMember]
        public CommentTextAndUrl PurchaseNotes { get; set; }

        [DataMember]
        public bool IsNotSale { get; set; } //是否可售
  
        [DataMember]
        public bool CanUseCoupon { get; set; } //是否可用现金券

        [DataMember]
        public DateTime CheckOut { get; set; } 

        [DataMember]
        public DateTime CheckIn { get; set; } 

        [DataMember]
        public string HotelName { get; set; } 

        [DataMember]
        public int Price { get; set; } //所选日期套餐价

        [DataMember]
        public int VIPPrice { get; set; } //所选日期套餐价
            

        [DataMember]
        public int NotVIPPrice { get; set; } //所选日期套餐价

        [DataMember]
        public int Rebate { get; set; } //所选日期套餐可反现金额

        [DataMember]
        public Int32 ActiveRebate { get; set; }

        [DataMember]
        public Int32 CashCoupon { get; set; }

        [DataMember]
        public Int32 CanUseCashCoupon { get; set; } //所选日期套餐可抵用现金券

        [DataMember]
        public int CanUseCashCouponForBoTao { get; set; } //所选日期套餐可抵用现金券4博涛用户

        [DataMember]
        public int PayType { get; set; } //付款方式

        [DataMember]
        public int MarketPrice { get; set; }//所选日期市场价

        [DataMember]
        public int PackageType { get; set; }//套餐类型 1：酒店套餐 2：OTA  3：深捷旅

        [DataMember]
        public int SellState { get; set; } //销售状态  1：可售  2：不可售 3: not fit daylimit

        [DataMember]
        public DateTime LastCancelTime { get; set; } //最后需要日期

        [DataMember]
        public PackageEntity packageBase { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<PItemEntity> Items { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<PRateEntity> Rates { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public PRoomInfoEntity Room { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<PRoomOptionsEntity> RoomOptions { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<PackageDailyEntity> DailyItems { get; set; } //每日套餐数据

        /// <summary>
        /// 支付标签图片Url数组
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<string> PayLabelUrls { get; set; }

        /// <summary>
        /// alipay:支付宝
        /// upay:U付
        /// tenpay:微信支付
        /// all:已经有的全部支付方式
        /// 如果PayChannels没有任何值则不跳到支付渠道选择页面
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<string> PayChannels { get; set; }

        /// <summary>
        /// 取消政策
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string CancelPolicy { get; set; }

        /// <summary>
        /// 优惠政策
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string BenefitPolicy { get; set; }

        /// <summary>
        /// 扣除返现后价格
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int PriceAfterDeduct { get; set; }

        /// <summary>
        /// 返现入账说明
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RefundPolicy { get; set; }


        /// <summary>
        /// 最少入住人数
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int MinHotelPeople { get; set; }

        /// <summary>
        /// 最多入住人数
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int MaxHotelPeople { get; set; }

        /// <summary>
        /// 证件类型，多个用";"分割
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String CardType { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<string> CardTypeList
        {
            get;
            set;
            //get {
            //    if (this.CardType == null)
            //    {
            //        return new string[] { };
            //    }
            //    else
            //    {
            //        return CardType.Split(';').ToArray();
            //    }
            //}
            //set { }

        }


        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string TravelPersonDescribe { get; set; }


        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public UpGradePackageInfoEntity UpGradePackageInfo { get; set; }

        /// <summary>
        /// 用户身份
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public BuyMembershipSuggest Suggest { get; set; }

        /// <summary>
        /// 免费升级后的房型
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string UpGradeRoomTypeName { get; set; }

        /// <summary>
        /// 顾问询价
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public CommentTextAndUrl CustomerAskPrice { get; set; }


        /// <summary>
        /// 订单页要展示的购买VIP描述
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public CommentTextAndUrl OrderPageShowBuyVIP { get; set; }

        /// <summary>
        /// 前端是否显示VIP价格。0显示1不显示
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int NotShowVIPPrice { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int CustomBuyMin { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int CustomBuyMax { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal ManualCommission { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal AutoCommission { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UpGradePackageInfoEntity
    {
        public UpGradePackageInfoEntity() { }

        [DataMember]
        public int PID { get; set; }


        [DataMember]
        public int PriceGap { get; set; }

        [DataMember]
        public string ListDescription { get; set; }

        [DataMember]
        public string OrderDescription { get; set; }

        [DataMember]
        public DateTime LastCancelTime { get; set; }

        [DataMember]
        public String CancelPolicy { get; set; }

    }
}