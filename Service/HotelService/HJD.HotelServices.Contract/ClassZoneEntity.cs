using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 商区筛选条件
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class ClassZoneEntity
    {
        /// <summary>
        /// 商区
        /// </summary>
        [DataMember]
        public int Zone { get; set; }

        /// <summary>
        /// 商区名
        /// </summary>
        [DataMember]
        public string ZoneName { get; set; }

        /// <summary>
        /// 类别：1 商区 2 行政区
        /// </summary>
        [DataMember]
        [DBColumn(IsOptional = true)]
        public ZoneType ZoneType { get; set; }
    }

    [Serializable]
    [DataContract]
    public enum ZoneType
    {
        [EnumMember]
        Zone,
        [EnumMember]
        Location,
    }
}
