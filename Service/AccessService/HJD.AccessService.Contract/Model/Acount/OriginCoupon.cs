using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Acount
{
    [Serializable]
    [DataContract]
    /// <summary>
    /// 
    /// </summary>
    public class OriginCoupon
    {
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 Id { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 SourceId { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 TypeId { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 UserId { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Decimal CashMoney { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Decimal TotalMoney { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime CreateTime { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime AcquiredTime { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 State { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime ExpiredTime { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public string GUID { get; set; }


        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime RegisterTime { get; set; }
    }
}
