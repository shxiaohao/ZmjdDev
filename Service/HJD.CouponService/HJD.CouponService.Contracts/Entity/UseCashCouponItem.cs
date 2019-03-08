using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class UseCashCouponItem
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CashCouponType { get; set; }
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CashCouponID { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int OrderType { get; set; }
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long OrderID { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal UseCashAmount { get; set; }

    }
}