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
    public class ExchangeCouponRequestParam
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string OperationRemark { get; set; }

    }
}
