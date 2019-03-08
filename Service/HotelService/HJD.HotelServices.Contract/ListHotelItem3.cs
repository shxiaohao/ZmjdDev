using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 列表酒店数据 为3.0版用
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class ListHotelItem3
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
        /// 酒店短名称
        /// </summary>
        [DataMember]
        [DBColumn(DefaultValue = "")]
        public string ShortName { get; set; }

        /// <summary>
        /// 起价
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public decimal MinPrice { get; set; }
        /// <summary>
        /// VIP价格
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public decimal VIPPrice { get; set; }

        /// <summary>
        /// 价格类型 1:OTA 2:ZMJD(专享） 3:Package（套餐)
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public int PriceType { get; set; }

        /// <summary>
        /// 前端显示的标签  如“特惠”标签
        /// </summary>
        [DataMember]
        [DBColumn(Ignore = true)]
        public string  Tag { get; set; }

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
        /// 评分 用于ios android
        /// </summary>
        [DataMember]
        [DBColumn]
        public decimal Score { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [DataMember]
        [DBColumn]
        public decimal HotelScore { get; set; }

        /// <summary>
        /// 点评数量
        /// </summary>
        [DataMember]
        [DBColumn]
        public int ReviewCount { get; set; }

        /// <summary>
        /// 推荐数量
        /// </summary>
        [DataMember]
        [DBColumn]
        public int RecommendCount { get; set; }

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
        /// 酒店地区描述
        /// </summary>
        [DataMember]
        public string DistrictName { get; set; }

        /// <summary>
        /// 酒店星级描述
        /// </summary>
        [DataMember]
        public string HotelStar { get; set; }

        /// <summary>
        /// 酒店列表照片Url
        /// </summary>
        [DataMember]
        [DBColumn]
        public List<string> PictureList { get; set; }

        /// <summary>
        /// 酒店列表照片SURL
        /// </summary>
        [DataMember]
        [DBColumn]
        public List<string> PictureSURLList { get; set; }

        /// <summary>
        /// 酒店特色点评
        /// </summary>
        [DataMember]
        [DBColumn]
        public string InterestComment { get; set; }

        /// <summary>
        /// 酒店介绍
        /// </summary>
        [DataMember]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string HotelIntro { get; set; }
    }
}