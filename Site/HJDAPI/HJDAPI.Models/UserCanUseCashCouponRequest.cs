using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HJDAPI.Models
{
    [DataContract]
    public class UserCanUseCashCouponRequest : BaseParam
    {    
         [DataMember]
        public long UserID { get; set; }
      
    } 
}
