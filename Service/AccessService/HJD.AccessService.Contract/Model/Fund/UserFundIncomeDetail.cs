using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Fund
{
    [Serializable]
    [DataContract]
    /// <summary>
    /// 
    /// </summary>
    public class UserFundIncomeDetail
    {
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 Id { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 UserId { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 TypeId { get; set; }
        
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public decimal Fund { get; set; }
        
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public string Label { get; set; }
        
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 RelationUserId { get; set; }
        
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 OriginOrderId { get; set; }
        
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public decimal OriginAmount { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime OriCreateTime { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime CreateTime { get; set; }
    }
}
