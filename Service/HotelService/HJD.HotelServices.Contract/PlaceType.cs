using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 地标类型
    /// </summary>
    [Serializable]
    [DataContract]
    public enum PlaceType
    {
        /// <summary>
        /// 医院
        /// </summary>
        [EnumMember]
        Hospital,

        /// <summary>
        /// 大学
        /// </summary>
        [EnumMember]
        University,

        /// <summary>
        /// 火车站
        /// </summary>
        [EnumMember]
        Trainstation,

        /// <summary>
        /// 机场
        /// </summary>
        [EnumMember]
        Airport,

        /// <summary>
        /// 所有
        /// </summary>
        [EnumMember]
        All
    }
}
