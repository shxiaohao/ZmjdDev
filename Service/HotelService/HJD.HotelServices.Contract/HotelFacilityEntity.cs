using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    public class HotelFacilityEntity
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int HotelID { get; set; }

        /// <summary>
        /// 设备id
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int Facility { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string FacilityName { get; set; }

        /// <summary>
        /// Icon
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string Icon { get; set; }

        /// <summary>
        /// Icon
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string Cat { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public bool isValid { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public bool IsAvailable { get; set; }
    }
}