using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices
{
    /// <summary>
    /// 酒店数据
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelTFTReviewItemEntity
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMember]
        [DBColumn]
        public int HotelID { get; set; }

        /// <summary>
        /// 3:主题、2:特色、1:标签
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public int Type { get; set; }

        /// <summary>
        ///  主题、特色或标签ID
        /// </summary>
        [DataMember]
        [DBColumn]
        public int TFTID { get; set; }

        /// <summary>
        ///  主题、特色或标签名字
        /// </summary>
        [DataMember]
        [DBColumn]
        public String TFTName { get; set; }
            

        /// <summary>
        /// 点评
        /// </summary>
        [DataMember]
        [DBColumn]
        public int ReviewID { get; set; }


    }
}
