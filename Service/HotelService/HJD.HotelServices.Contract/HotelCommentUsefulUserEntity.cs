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
    public class HotelReviewUsefulUserEntity
    {
       
        [DataMember]
        public int Writing { get; set; }


        [DataMember]
        [DBColumn(IsOptional = true)]
        public List<long> UserList { get; set; }
    }
}

