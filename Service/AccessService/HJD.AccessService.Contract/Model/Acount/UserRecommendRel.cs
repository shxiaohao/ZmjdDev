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
    public class UserRecommendRel
    {
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 Id { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 UserId { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 ReUserId { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 RecommendChannel { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime RecommendDate { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime ReRegisterDate { get; set; }
    }
}
