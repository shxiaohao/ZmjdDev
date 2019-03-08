using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models.Coupon
{

    [Serializable]
    [DataContract]
    public class CouponOrderEntity
    {
        [DataMember]
        public Int32 ExchangeCouponID { get; set; }

        /// <summary>
        /// 券码
        /// </summary>
        [DataMember]
        public String ExchangeNo { get; set; }

        /// <summary>
        /// SKUID
        /// </summary>
        [DataMember]
        public Int32 SKUID { get; set; }

        /// <summary>
        /// sku名称
        /// </summary>
        [DataMember]
        public String SKUName { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        [DataMember]
        public String PageTitle { get; set; }

        /// <summary>
        /// 券码有效开始时间
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 券码有效结束时间
        /// </summary>
        [DataMember]
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// 订单支付价
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 使用积分
        /// </summary>
        [DataMember]
        public decimal Points { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public int OrderState { get; set; }

        /// <summary>
        /// 订单类型的ICON图标
        /// </summary>
        [DataMember]
        public string OrderTypeIcon { get; set; }
    }
}
