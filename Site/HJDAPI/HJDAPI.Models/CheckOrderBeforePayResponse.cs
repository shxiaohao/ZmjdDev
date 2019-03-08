using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class CheckOrderBeforePayResponse:BaseResponse
    {
        [DataMember]
        public bool bCanPay { get; set; }
    }

    [DataContract]
    public class CheckZMJDMemberResponse : BaseResponse
    {
        [DataMember]
        public bool isMember { get; set; }
    }
}
