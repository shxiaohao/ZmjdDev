using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;
namespace HJD.HotelServices.Contracts
{
     [Serializable]
    [DataContract]
    public class HotelKeyWordWritingEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int ID { get; set; }


        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int HotelID { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int Writing { get; set; }

     
    }
}
