using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 中档酒店标签
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    class HotelZhongdangTag
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        [DataMember]
        public int HotelId { get; set; }

        /// <summary>
        /// 标签ID
        /// </summary>
        [DataMember]
        public int TagId { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "2005-1-1")]
        public DateTime HotelID { get; set; }
    }
}
