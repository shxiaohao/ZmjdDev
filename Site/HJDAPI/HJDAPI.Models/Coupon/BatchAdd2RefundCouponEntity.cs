using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HJDAPI.Models.Coupon
{

    [Serializable]
    [DataContract]
    public class BatchAdd2RefundCouponEntity
    {
        [DataMember]
        public List<Int32> ExchangeCouponIDs { get; set; }

        [DataMember]
        public long UserID { get; set; }

        [DataMember]
        public string Remark { get; set; }
    }
}
