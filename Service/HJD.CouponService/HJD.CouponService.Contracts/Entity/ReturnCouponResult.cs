using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace HJD.CouponService.Contracts
{
    [Serializable]
    [DataContract]
    public sealed class ReturnCouponResult
    {
        [DataMember]
        public string Message
        {
            get;
            set;
        }

        [DataMember]
        public int Success
        {
            get;
            set;
        }

        [DataMember]
        public int DiscountMoney
        {
            get;
            set;
        }
    }
}