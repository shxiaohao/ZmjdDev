using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;
namespace HJD.HotelServices.Contracts
{
     [Serializable]
    [DataContract]
    public class HotelKeyWordEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0",IsOptional = true)]
        public int ID { get; set; }


        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int HotelID { get; set; }

     

        /// <summary>
        /// keyword
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string keyword { get; set; }

        /// <summary>
        /// number
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int Number { get; set; }

        /// <summary>
        /// WritingList
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public List<int> WritingList { get; set; }
    }
}