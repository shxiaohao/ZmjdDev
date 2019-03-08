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
    public class HotelDistanceEntity
    {
        /// <summary>
        /// 酒店编号
        /// </summary>
        [DataMember]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelId { get; set; }

        /// <summary>
        /// 酒店距离
        /// </summary>
        [DataMember]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal Distance { get; set; }
    }
}
