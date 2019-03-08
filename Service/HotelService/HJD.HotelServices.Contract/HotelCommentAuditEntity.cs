using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;
namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店信息
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelReviewAuditEntity
    {      
        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int Writing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public char Deleted { get; set; }

        //[DataMemberAttribute()]
        //[DBColumnAttribute(DefaultValue = "")]
        //public string HotelName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public char Status { get; set; }

        //[DataMemberAttribute()]
        //[DBColumnAttribute(DefaultValue = "")]
        //public string Ename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public int statusDetail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string opUid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public long opUserid { get; set; }

    }
}



