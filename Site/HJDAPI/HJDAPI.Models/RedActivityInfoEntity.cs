using HJD.CouponService.Contracts.Entity;
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
    public class RedActivityInfoEntity
    {
        [DataMember]
        public List<RedRecordEntity> RedRecordList { get; set; }

        [DataMember]
        public int RedState { get; set; }

        [DataMember]
        public int TotalCount { get; set; }

    }
}
