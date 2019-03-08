using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 点评有用用户信息
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class ReviewUsefulUserEntity
    {
        /// <summary>
        /// userid
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue ="0", IsOptional = true)]
        public long UserID { get; set; }


        [DataMember]
        public int Writing { get; set; }
    
    }
}
