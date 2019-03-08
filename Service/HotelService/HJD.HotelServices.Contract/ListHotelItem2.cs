using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 列表酒店数据 为2.0版用
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class ListHotelItem2
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
        /// 起价
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 价格类型 1:OTA 2:ZMJD(专享） 3:Package（套餐)
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public int PriceType { get; set; }

		/// <summary>
		/// 币种
		/// </summary>
		[DataMember]
        [DBColumn(DefaultValue = "￥")]
		public string Currency { get; set; }


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
        /// 评论总数
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public int ReviewCount { get; set; }

     
        /// <summary>
        /// 酒店标签
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public List<FeaturedEntity> FeaturedList { get; set; }

        /// <summary>
        /// 酒店列表照片SURL
        /// </summary>
        [DataMember]
        [DBColumn]
        public string Picture { get; set; }

        /// <summary>
        /// 酒店列表照片SURL
        /// </summary>
        [DataMember]
        [DBColumn]
        public string PicSURL { get; set; }

        /// <summary>
        /// 套餐信息
        /// </summary>
        [DataMember]
        [DBColumn]
        public List<PackageEntity> PackgeList { get; set; }
    }
}
