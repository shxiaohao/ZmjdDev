using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class PDayItem
    {
        public PDayItem() { PItems = new List<Contracts.PDayPItem>(); }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public DateTime Day { get; set; } //天

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 SellState { get; set; } // 可售卖套餐状态 0：不可卖 1：可卖

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 SoldCount { get; set; } // 已售卖套餐数

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 MaxSealCount { get; set; } //总可售卖套餐数

        [System.Runtime.Serialization.DataMemberAttribute()]
        public List<PDayPItem> PItems { get; set; } //各套餐售卖情况

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 SellPrice { get; set; } //套餐当日售价 如果会员则显示会员处理之后的价格

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 NormalPrice { get; set; } //套餐正常售价 不考虑会员

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 VipPrice { get; set; } //套餐VIP会员价

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 ID { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal AutoCommission { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal ManualCommission { get; set; }
    }
}