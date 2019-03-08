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
    public class HotelInfoChannelEntity
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
        [DBColumnAttribute(DefaultValue = "")]
        public string Channel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public Int16 ChannelID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public int HotelOriID { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public bool CanSyncPrice { get; set; }

    }
}