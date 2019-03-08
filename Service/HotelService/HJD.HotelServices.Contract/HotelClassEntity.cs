using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelClassEntity
    {
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int HotelID { get; set; }

        /// <summary>
        /// classID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int ClassID { get; set; }

        /// <summary>
        /// ClassName
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string ClassName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int Sort { get; set; }
    }
}
