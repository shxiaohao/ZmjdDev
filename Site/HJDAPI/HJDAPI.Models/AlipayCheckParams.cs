using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class AlipayCheckParams
    {
        [DataMember]
        public long orderid { get; set; }

        [DataMember]
        public string terminalType { get; set; }
    }
}
