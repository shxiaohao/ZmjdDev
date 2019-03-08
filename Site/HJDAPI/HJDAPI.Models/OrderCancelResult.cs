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
    public class OrderCancelResult
    {
        [DataMember]
        public int success { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}
