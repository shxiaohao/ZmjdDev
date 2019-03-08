using HJD.CouponService.Contracts.Entity;
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
    public class PresentCashCouponParam : BaseParam
    {
        [DataMember]
        public long userID { get; set; }

        [DataMember]
        public CouponActivityCode typeID { get; set; }

        [DataMember]
        public string phoneNo { get; set; }

    }
}
