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
    public class TopTownHotelPriceEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int TopTownHotelId { get; set; }
        /// <summary>
        /// 房间表idx
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int RoomId { get; set; }
        /// <summary>
        /// 价格表idx
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int RateId { get; set; }
        
        /// <summary>
        /// 当前币种
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string CurrencyCode { get; set; }
        /// <summary>
        /// 支付类型.
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string PaymentType { get; set; }

        /// <summary>
        /// 价格计划code
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RatePlanCode { get; set; }
        /// <summary>
        /// 价格计划名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RatePlanName { get; set; }
        /// <summary>
        /// 房型code
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RoomTypeCode { get; set; }
        /// <summary>
        /// 房型名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RoomTypeName { get; set; }
        /// <summary>
        ///  税后金额，为-1时表示当前日期无价格
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal AmountAfterTax { get; set; }
        /// <summary>
        ///  税前金额，为-1时表示当前日期无价格
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal AmountBeforeTax { get; set; }

        /// <summary>
        ///  早餐数量
        /// 0  不含早
        /// 1  单早
        /// 2  双早
        /// -2  含早 根据入住人数匹配早餐分数（注：最多2份）
        /// 3  三早
        /// 4:四早
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Breakfast { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Child { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string EffectiveDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string ExpireDate { get; set; }
        
        /// <summary>
        /// 当天预订截止时间（精确到分钟，例如17:00）
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string LastOrderTime { get; set; }

        /// <summary>
        /// 促销规则-连续入住N天及以上优惠，值为0时表示没有此促销。
        /// LastDays和AdvanceDays同时有值时表示的促销规则为：提前AdvanceDays天预定，且连住LastDays晚及以上
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int LastDays { get; set; }

        /// <summary>
        /// 促销规则-提前N天预订优惠，值为0时表示没有此促销。
        /// LastDays和AdvanceDays同时有值时表示的促销规则为：提前AdvanceDays天预定，且连住LastDays晚及以上
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int AdvanceDays { get; set; }

        /// <summary>
        /// 房型的当前房量，为-1表示当前房型不可售（关房/满房）
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Ability { get; set; }

        /// <summary>
        /// 具体房型和价格计划上的退变更原则描述。
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RateCancelPolicy { get; set; }

        /// <summary>
        /// 床型名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string BedTypeName { get; set; }

        /// <summary>
        /// 具体房型和价格计划上的是否即订即保信息。
        ///是:表示该价格的房型即订即保，下单后不可取消。
        ///否：表示该价格的房型非即订即保。
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RateNotUpdateable { get; set; }

        /// <summary>
        /// 具体房型和价格计划上的截止退变更日期。根据查询时输入的预定日期（CheckIn）计算
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime StopCancelDate { get; set; }

        /// <summary>
        /// 具体房型和价格计划上的截止退变更日期（单位：分钟）= 0表示不可取消；
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int RTStopCancelTime { get; set; }

    }
}
