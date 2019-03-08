using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店数据
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelInfoEditEntity
    {  
        /// <summary>
        /// 操作员ID
        /// </summary>
        [DataMember]
        public long OperatorUserID { get; set; }

        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMember]
        public int HotelID { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        [DataMember]
        public string p1 { get; set; }
        
      

        /// <summary>
        /// 操作类型
        /// </summary>
        [DataMember]
        public int OpType { get; set; }

    }
}
