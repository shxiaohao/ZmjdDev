using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class NearbyHotelEntity
    {
        /// <summary>
        /// poi
        /// </summary>
        [DataMember]
        [DBColumn(IsOptional = true)]
        public int POIID { get; set; }

        /// <summary>
        /// 酒店Id
        /// </summary>
        [DataMember]
        [DBColumn(IsOptional = true)]
        public int HotelId { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        [DataMember]
        [DBColumn(IsOptional = true)]
        public string HotelName { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        [DataMember]
        [DBColumn(IsOptional = true)]
        public int Distance { get; set; }
    }
}
