using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 机场火车站信息 add by hcdu 2012/02/06
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class DistrictAirportTrainstationEntity
    {
        /// <summary>
        /// 自增id
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int ID { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int DistrictID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Name { get; set; }

        /// <summary>
        /// 类型机场or 火车站
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Type { get; set; }
    }
    
    /// <summary>
    /// 周边地区及其名称 距离
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class ArounDistrictEntity
    {
        /// <summary>
        /// 地区ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int DistrictID { get; set; }

        /// <summary>
        /// 地区名称
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public string DistrictName { get; set; }

        /// <summary>
        /// 距离 单位米
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public float Distance { get; set; }

        /// <summary>
        /// 酒店数量
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int HotelCount { get; set; }

        /// <summary>
        /// 默认为0，app端根据 geoScopeTyp=3 来判断是周边跳转
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]  
        public int GeoScopeType { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")] 
        public string Icon { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")] 
        public double Lat { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")] 
        public double Lon { get; set; }


        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// 类型 D：城市  H：酒店  P：玩乐 F：美食
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public string Type { get; set; }
    }
}