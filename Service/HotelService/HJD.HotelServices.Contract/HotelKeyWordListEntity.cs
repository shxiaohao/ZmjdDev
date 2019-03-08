using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;
namespace HJD.HotelServices.Contracts
{
     [Serializable]
    [DataContract]
    public class HotelKeyWordListEntity
    {
     
        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int HotelID { get; set; }     


        /// <summary>
        /// WritingList
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public List<HotelKeyWordEntity> HotelKeyWordList { get; set; }
    }
}
