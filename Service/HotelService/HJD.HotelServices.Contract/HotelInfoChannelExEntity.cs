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
    public class HotelInfoChannelExEntity
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int HotelID { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public List<HotelInfoChannelEntity> HotelOriIDList { get; set; }      

        /// <summary>
        /// 是否有携程图片
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string ViewHotelImgUrl { get; set; }
       
    }
}




