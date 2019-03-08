using HJD.HotelManagementCenter.Domain.Settlement;
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
    public class GetChannelMOrderListResult
    {
        [DataMember]
        public int pageSize { get; set; }
        [DataMember]
        public int start { get; set; }
        [DataMember]
        public int totalCount { get; set; }
        [DataMember]
        public List<ChannelMOrderEntity> list { get; set; }
    }
}
