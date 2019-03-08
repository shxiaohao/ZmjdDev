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
    public class UserFund
    {
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 Id { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 UserId { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public decimal TotalFund { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime CreateTime { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime UpdateTime { get; set; }
    }
}
