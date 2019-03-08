using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class ShopLoginParam
    {
        [DataMember]
        public string OperatorName { get; set; }
        [DataMember]
        public string PassWord { get; set; }

    }
}
