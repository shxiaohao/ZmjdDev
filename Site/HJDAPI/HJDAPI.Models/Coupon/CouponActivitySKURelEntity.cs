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
    public class CouponActivitySKURelEntity
    {
        [DataMember]
        public int ActivityID { get; set; }

        [DataMember]
        public int SKUID { get; set; }

    }
}
