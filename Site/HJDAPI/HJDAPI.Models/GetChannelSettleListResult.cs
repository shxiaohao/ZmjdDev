using HJD.HotelManagementCenter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class GetChannelSettleListResult
    {
        [DataMember]
        public int pageSize { get; set; }
        [DataMember]
        public int start { get; set; }
        [DataMember]
        public int totalCount { get; set; }
        [DataMember]
        public List<SettleBatchWithDrawEntity> list { get; set; }
    }
}
