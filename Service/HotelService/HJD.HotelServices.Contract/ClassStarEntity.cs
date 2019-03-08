using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 星级筛选条件
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class ClassStarEntity
    {
        /// <summary>
        /// 星级
        /// </summary>
        [DataMember]
        public int Star { get; set; }
    }
}
