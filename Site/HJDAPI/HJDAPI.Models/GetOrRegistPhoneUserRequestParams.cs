using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class GetOrRegistPhoneUserRequestParams : BaseParam
    {
        [DataMember]
        public string  Phone { get; set; }

        [DataMember]
        public long CID { get; set; }
    }
}
