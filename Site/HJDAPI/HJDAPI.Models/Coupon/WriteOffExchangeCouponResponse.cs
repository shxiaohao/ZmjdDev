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
    public class WriteOffExchangeCouponResponse
    {
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        [DataMember]
        public String StateName { get; set; }

        /// <summary>
        /// 产品支付价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 核销时间
        /// </summary>
        [DataMember]
        public DateTime WriteOffTime { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataMember]
        public string SupplierName { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [DataMember]
        public string SKUName { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        [DataMember]
        public int SKUID { get; set; }
    }
}
