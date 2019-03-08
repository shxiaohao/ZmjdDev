using HJD.CouponService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
     [DataContract]
    public class CheckSelectedCashCouponInfoForOrderViewModel
    {
        /// <summary>
        /// 可用则为true, 否则为false
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

         /// <summary>
         /// 不可用提示信息
         /// </summary>
        [DataMember]
        public string ErrMessage { get; set; }
        /// <summary>
        /// 显示名称： 立减**，满*减*
        /// </summary>
        [DataMember]
        public string CashCouponShowName { get; set; }

        /// <summary>
        /// 订单可以减金额
        /// </summary>
        [DataMember]
        public decimal OrderCanDiscountAmount { get; set; }

        /// <summary>
        /// 对应的现金券对象信息
        /// </summary>
        [DataMember]
        public UserCouponItemInfoEntity CashCouponInfo { get; set; }


    }
}
