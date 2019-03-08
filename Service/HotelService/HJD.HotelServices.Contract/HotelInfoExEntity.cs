using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店信息
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelInfoExEntity
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int HotelID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Resourcename { get; set; }

        //[DataMemberAttribute()]
        //[DBColumnAttribute(DefaultValue = "")]
        //public string HotelName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Enresourcename { get; set; }

        //[DataMemberAttribute()]
        //[DBColumnAttribute(DefaultValue = "")]
        //public string Ename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Telephone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string HotelSource { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string StarLicence { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int RoomQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string LocationName { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int Location { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int Star { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int DistrictID { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string DistrictName { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string DistrictEName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0D")]
        public double GLAT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0D")]
        public double GLON { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0D")]
        public double BLAT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0D")]
        public double BLON { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "0")]
        public int Zone { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string ZoneName { get; set; }

        [DataMember]
        [DBColumn()]
        public byte HotelType { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string HotelDesc { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string Brief { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string HotelPic { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string ZipCode { get; set; }

        //[DataMember]
        //[DBColumn(DefaultValue = "")]
        //public string Fax { get; set; }

        /// <summary>
        /// 酒店品牌
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "0")]
        public int Brand { get; set; }

        //[DataMember]
        //[DBColumn(DefaultValue = "1900-1-1")]
        //public DateTime OpenYear { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string WebSite { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string Email { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string BrandName { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public List<int> FacilityList { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public List<HotelRankEntity> HotelClass { get; set; }

        /// <summary>
        /// 所属目的地的排名
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "0", IsOptional = true)]
        public int Rank { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "0", IsOptional = true)]
        public int UseBookingRule { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "0001/1/1 0:00:00", IsOptional = true)]
        public DateTime Openyear { get; set; }

        [DataMember]
        [DBColumn(IsOptional=true)]
        public bool IsValued { get; set; }
    }
}


