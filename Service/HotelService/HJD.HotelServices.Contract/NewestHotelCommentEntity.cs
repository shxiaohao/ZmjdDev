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
    public class NewestHotelReviewEntity
    {
        /// <summary>
        /// 酒店编号
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0")]
        public int HotelId
        {
            get;
            set;
        }


        /// <summary>
        /// 酒店名称    
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string HotelName
        {
            get;
            set;
        }

        /// <summary>
        /// 酒店英文名称
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string HotelEnName { get; set; }

        /// <summary>
        /// 点评分数
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0.0", IsOptional = true)]
        public decimal Points
        {
            get;
            set;
        }

        /// <summary>
        /// 点评人数
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int CountNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 排名数
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int Ranking
        {
            get;
            set;
        }

        /// <summary>
        /// 酒店数量
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int HotelCount
        {
            get;
            set;
        }

        /// <summary>
        /// 类型名称
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string ClassTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// AllMark
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int AllMark { get; set; }

        /// <summary>
        /// MaxID
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int MaxID { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int DistrictID
        {
            get;
            set;
        }
        /// <summary>
        /// 目的地
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string DistrictName
        {
            get;
            set;
        }
        /// <summary>
        /// 目的地英文名称
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string DistrictEnName { get; set; }
    }
}
