using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class ExchangeCouponsParam
    {
        [DataMember]
        public string ExchangeIds { get; set; }
    }
}
