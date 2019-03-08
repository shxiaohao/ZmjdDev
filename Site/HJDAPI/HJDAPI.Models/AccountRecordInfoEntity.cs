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
    public class AccountRecordInfoEntity
    {
        [DataMember]
        public int totalCount { get; set; }
        [DataMember]
        public List<AccountRecordEntity> list { get; set; }
    }
}
