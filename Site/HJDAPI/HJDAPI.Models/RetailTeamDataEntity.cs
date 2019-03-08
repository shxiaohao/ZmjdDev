using HJD.HotelManagementCenter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class RetailTeamDataEntity
    {
        [DataMember]
        public decimal TotalTeamOrderPrice { get; set; }

        [DataMember]
        public decimal TotalTeamReward { get; set; }

        [DataMember]
        public int TeamMemberCount { get; set; }

        [DataMember]
        public List<RetailOrderStatistics> RetailDetailList { get; set; }
    }
}
