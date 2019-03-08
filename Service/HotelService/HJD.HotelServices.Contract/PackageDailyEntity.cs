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
    public class PackageDailyEntity
    {
        [DataMember]
        public DateTime Day { get; set; } //所选日期
        [DataMember]
        public int Price { get; set; } //所选日期套餐价

        [DataMember]
        public int VIPPrice { get; set; } //所选日期套餐价

        [DataMember]
        public int NotVIPPrice { get; set; } //所选日期套餐价
        [DataMember]
        public int Rebate { get; set; } //所选日期套餐返现金额

        [DataMember]
        public int ActiveRebate { get; set; } //所选日期套餐分享返现金额

        [DataMember]
        public int CashCoupon { get; set; } //所选日期套餐可分享现金券

        [DataMember]
        public int CanUseCashCoupon { get; set; } //所选日期套餐可抵用现金券
        
        [DataMember]
        public int CanUseCashCouponForBoTao { get; set; } //所选日期套餐可抵用现金券4博涛用户

        [DataMember]
        public int PayType { get; set; } //支付类型
        [DataMember]
        public int MarketPrice { get; set; }//所选日期市场价 

        [DataMember]
        public List<PItemEntity> Items { get; set; }
        
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
    }
}
