using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class CouponOrderStepGroupEntity
    {
        /// <summary>
        /// 大团购状态
        /// </summary>
        [DataMember]
        public int StepGroupState { get; set; }

        /// <summary>
        /// 尾款开始时间
        /// </summary>
        [DataMember]
        public DateTime TailMoneyStartTime { get; set; }

        /// <summary>
        /// 尾款结束时间
        /// </summary>
        [DataMember]
        public DateTime TailMoneyEndTime { get; set; }

        /// <summary>
        /// 是否支付尾款
        /// </summary>
        [DataMember]
        public bool IsPayFinish { get; set; }

        /// <summary>
        /// 预约位置
        /// </summary>
        [DataMember]
        public int BookPosition { get; set; }


        /// <summary>
        /// 定金SKUID
        /// </summary>
        [DataMember]
        public int DepositSKUID { get; set; }

        /// <summary>
        /// 尾款SKUID
        /// </summary>
        [DataMember]
        public int TailSKUID { get; set; }

        /// <summary>
        /// 尾款支付金额
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 支付总金额
        /// </summary>
        [DataMember]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 膨胀金
        /// </summary>
        [DataMember]
        public decimal ExpansionAmount { get; set; }

        /// <summary>
        /// 是否是定金产品
        /// </summary>
        [DataMember]
        public bool IsDepositSKU { get; set; }
    }
}
