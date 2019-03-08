﻿using System;
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
    public class HotelItem
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMember]
        [DBColumn]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string Name { get; set; }

        /// <summary>
        /// 短名称
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string ShortName { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string EName { get; set; }

        /// <summary>
        /// 起价
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public decimal MaxPrice { get; set; }

		/// <summary>
		/// 币种
		/// </summary>
		[DataMember]
        [DBColumn(DefaultValue = "￥")]
		public string Currency { get; set; }

		/// <summary>
		/// 预订
		/// </summary>
		[DataMember]
        [DBColumn(DefaultValue = "")]
        public string HotelSource { get; set; }

        /// <summary>
        /// 排名
        /// </summary>
        [DataMember]
        [DBColumn]
        public int Rank { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [DataMember]
        [DBColumn]
        public decimal Score { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string Telephone { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string Address { get; set; }

        /// <summary>
        /// 星级
        /// </summary>
        [DataMember]
        [DBColumn]
        public int Star { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [DataMember]
        [DBColumn]
        public double GLat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [DataMember]
        [DBColumn]
        public double GLon { get; set; }

        /// <summary>
        /// 所在城市ID
        /// </summary>
        [DataMember]
        [DBColumn]
        public int DistrictId { get; set; }

        /// <summary>
        /// 所在城市名称
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string DistrictName { get; set; }

        /// <summary>
        /// 所在城市英文名称
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string DistrictEName { get; set; }

        /// <summary>
        /// RootName
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string ProvinceName { get; set; }

        /// <summary>
        /// 是否属于中国
        /// </summary>
        [DataMember]
        [DBColumn]
        public bool InChina { get; set; }

        /// <summary>
        /// 所在城市酒店总数
        /// </summary>
        [DataMember]
        [DBColumn]
        public int DistrictCount { get; set; }

        /// <summary>
        /// 评论总数
        /// </summary>
        [DataMember]
        [DBColumn]
        public int ReviewCount { get; set; }

        /// <summary>
        /// 酒店评论打分分组统计 
        /// </summary>
         [DataMember]
         [DBColumn(Ignore = true)]
        public string VoteComponenets { get; set; }



        /// <summary>
        /// 酒店类型
        /// </summary>
        [DataMember]
        [DBColumn(Ignore=true)]
        public IEnumerable<string> Types { get; set; }

        /// <summary>
        /// 酒店设施
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public List<HotelFacilityEntity> Facilities { get; set; }

        /// <summary>
        /// 主图片
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public string Picture { get; set; }

        /// <summary>
        /// 照片总数
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public int PictureCount { get; set; }

        /// <summary>
        /// 照片总数
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public int OfficalPictureCount { get; set; }

        /// <summary>
        /// 酒店描述
        /// </summary>
        [DataMember]
        [DBColumn]
        public string Description { get; set; }

        /// <summary>
        /// OTA数据
        /// </summary>
        [DataMember]
        [DBColumn(Ignore=true)]
        public Dictionary<string,string> OTAID{ get; set; }


        /// <summary>
        /// 商业区
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public int Zone { get; set; }

        /// <summary>
        /// 商业区
        /// </summary>
        [DataMember]
        public String ZoneName { get; set; }


        /// <summary>
        /// 行政区
        /// </summary>
        [DataMember]
        public int Location { get; set; }

        /// <summary>
        /// 行政区名称
        /// </summary>
        [DataMember]
        public String LocationName { get; set; }

        /// <summary>
        /// 酒店标签
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public List<FeaturedEntity> FeaturedList { get; set; }

        /// <summary>
        /// 超值酒店
        /// </summary>
        [DataMember]
        [DBColumn]
        public Boolean IsValued { get; set; }

        [DataMember]
        [DBColumn]
        public int UseBookingRule { get; set; }

        [DataMember]
        [DBColumn]
        public string AttractionDistance { get; set; }

        [DataMember]
        [DBColumn]
        public string OpenDate { get; set; }
         
        [DataMember]
        [DBColumn]
        public string ReBuildInfo { get; set; }

        [DataMember]
        [DBColumn]
        public DateTime OpenYear { get; set; }
    }

    /// <summary>
    /// 酒店基本信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class HotelBasicInfo
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int HotelID { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public string HotelName { get; set; }

        /// <summary>
        /// 地区ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int DistrictID { get; set; }

        /// <summary>
        /// 点评数
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int ReviewCount { get; set; }

        /// <summary>
        /// 点评分
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public decimal ReviewScore { get; set; }

        /// <summary>
        /// 推荐数
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int RecommendCount { get; set; }
    }

    [Serializable]
    [DataContract]
    public class HotelMapBasicInfo
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int HotelID { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public string HotelName { get; set; }

        /// <summary>
        /// 地区ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int DistrictID { get; set; }

        /// <summary>
        /// google纬度
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public double Glat { get; set; }

        /// <summary>
        /// google经度
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public double Glon { get; set; }

        /// <summary>
        /// baidu纬度
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public double BLat { get; set; }

        /// <summary>
        /// baidu经度
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public double BLon { get; set; }

        /// <summary>
        /// 是否在国内
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public bool InChina { get; set; }
    }
}