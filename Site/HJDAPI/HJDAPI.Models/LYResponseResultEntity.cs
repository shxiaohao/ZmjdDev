using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class LYResponseResultEntity
    {

        [DataMember]
        public Int32 respCode { get; set; }

        [DataMember]
        public string respMsg { get; set; }

        [DataMember]
        public string respTime { get; set; }

    }
}
