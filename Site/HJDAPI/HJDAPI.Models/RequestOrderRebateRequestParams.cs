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
    public class RequestOrderRebateRequestParams
    {
        [DataMember]
        public long orderID { get; set; }
        [DataMember]
        public int type { get; set; }
    }
}
