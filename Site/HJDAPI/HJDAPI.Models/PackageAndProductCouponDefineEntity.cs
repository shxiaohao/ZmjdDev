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
    public class PackageAndProductCouponDefineEntity
    {
        public PackageAndProductCouponDefineEntity()
        {
            Icon = "";
            Tip = "";
            Link = "";
            CouponDefineList = new List<UserCouponDefineEntity>();
        }
             [DataMember]
        public string Icon { get; set; }

             [DataMember]
        public string Tip { get; set; }

             [DataMember]
        public string Link { get; set; }
             [DataMember]
        public List<UserCouponDefineEntity> CouponDefineList { get; set; }
    }
}
