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
    public class UpdateCouponPhotoReqParms:BaseParam
    {
        [DataMember]
        public Int32 CouponID { get; set; }


        [DataMember]
        public string  PhotoUrl { get; set; }


    }
}
