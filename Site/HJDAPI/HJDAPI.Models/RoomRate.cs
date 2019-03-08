using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HJDAPI.Models
{
    [DataContract]
    public class RoomRate2
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Int32 RoomID { get; set; }
        [DataMember]
        public Int32 RateID { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public decimal BackCount { get; set; }
        [DataMember]
        public int GuaranteeType { get; set; }
        [DataMember]
        public List<string> ShortDes { get; set; }
        [DataMember]
        public List<string> DetailDes { get; set; } //HTML 类型，以便标注重点
        [DataMember]
        public List<string> Others { get; set; } //其它一些房间说明，如礼品等
        [DataMember]
        public List<string> Pics { get; set; }

    }
}
