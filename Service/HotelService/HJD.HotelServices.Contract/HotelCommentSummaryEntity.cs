
using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店点评汇总
    /// </summary>
    [Serializable]
    [DataContract]
    public class HotelReviewSummaryEntity
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int HotelID { get; set; }

        /// <summary>
        /// 点评总数
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int CountHotelReview { get; set; }

        /// <summary>
        /// 酒店点评分
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public decimal Rating { get; set; }

        /// <summary>
        /// 房间卫生
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public decimal RatingRoom { get; set; }

        /// <summary>
        /// 周边环境
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public decimal RatingAtmosphere { get; set; }

        /// <summary>
        /// 酒店服务
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public decimal RatingService { get; set; }

        /// <summary>
        /// 设施设备
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public decimal RatingCostBenefit { get; set; }

        /// <summary>
        /// 最新三条点评
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public List<HotelReviewTop3Entity> HotelReviewTop3 { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //[DataMemberAttribute()]
        //[DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        //public List<UserIdentityEntity> GroupUserIdentity { get; set; }

        /// <summary>
        /// 很好、好、较好、一般、较差、很差汇总 //评价、出游类型分类汇总
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public List<RatingEntity> GroupRating { get; set; }

       
        /// <summary>
        /// 推荐人数
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int CountRecommend { get; set; }

        /// <summary>
        /// 总推荐数
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int CountTotalRecommend { get; set; }

    }
}