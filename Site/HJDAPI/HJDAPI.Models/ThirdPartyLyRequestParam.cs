using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class ThirdPartyLyRequestParam
    {
        [DataMember]
        public requestHead requestHead { get; set; }

        [DataMember]
        public requestBody requestBody { get; set; }
    }

    public class requestHead
    {
        [DataMember]
        public string digitalSign { get; set; }

        [DataMember]
        public string agentAccount { get; set; }
    }
    public class requestBody
    {
        [DataMember]
        public string returnCode { get; set; }

        [DataMember]
        public string returnMsg { get; set; }

        [DataMember]
        public string refundTime { get; set; }

        [DataMember]
        public string serialId { get; set; }

        [DataMember]
        public int RefundType { get; set; }

        [DataMember]
        public int refundTicketsNum { get; set; }

        [DataMember]
        public int refundAmount { get; set; }

        [DataMember]
        public decimal poundageAmount { get; set; }

        [DataMember]
        public int orderBillId { get; set; }
    }
}
