using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class ExistsMobileAccountResponse
    {
        [DataMember]
        public bool ExistsMobileAccount{ get; set; }
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Message { get; set; }
    }


    [DataContract]
    public class ExistsCodeResponse
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}

