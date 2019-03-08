using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店数量
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistinctHotelCountEntity
    {
        /// <summary>
        /// 酒店类型
        /// </summary>
        [DataMember]
        public HotelType HotelType;

        /// <summary>
        /// 酒店数量
        /// </summary>
        [DataMember]
        public int HotelCount;
    }
}
