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
    public class GetUserCommInfoReqParm:BaseParam
    {
        [DataMember]
        public long UserID { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public int InfoType { get; set; }
        [DataMember]
        public int HotelId { get; set; }
        [DataMember]
        public int Pid { get; set; }

        [DataMember]
        public int PackageType { get; set; }
    }
}
