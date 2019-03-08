using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    public class ArguHotelReview
    {
        [DataMember]
        public int Hotel { get; set; }

        [DataMember]
        public int Start { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public long UserID { get; set; }

        [DataMember]
        public long HideUserID { get; set; }
        
        [DataMember]
        public Int32 Featured { get; set; }

        [DataMember]
        public Int32 Tag { get; set; }

        [DataMember]
        public Int32 Theme { get; set; }

        [DataMember]
        public Int32 TFTType { get; set; }

        [DataMember]
        public Int32 TFTID { get; set; }

        /// <summary>
        /// 标签标签块过滤数组
        /// </summary>
        [DataMember]
        public List<int> FeaturedTreeList { get; set; }

        [DataMember]
        public Int32 FeaturedTreeID { get; set; }

        [DataMember]
        public Int32 RoomType { get; set; }

        [DataMember]
        public RatingType RatingType { get; set; }

        [DataMember]
        public UserIdentityType UserIdentityType { get; set; }

        [DataMember]
        public HotelReviewOrderType HotelReviewOrderType { get; set; }

        [DataMember]
        public List<long> FriendUserID { get; set; }

        [DataMember]
        public Int32 NeedFilter { get; set; }

        [DataMember]
        public Int32 NeedReview { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ArguHotelSEOKeywordReview
    {
        [DataMember]
        public int HotelID { get; set; }

        [DataMember]
        public int SEOKeywordID { get; set; }

        [DataMember]
        public int StartIndex { get; set; }

        [DataMember]
        public int ReturnCount { get; set; }

        [DataMember]
        public HotelReviewOrderType HotelReviewOrderType { get; set; }
    }

    [Serializable]
    [DataContract]
    public enum HotelReviewOrderType
    {
        /// <summary>
        /// 排序 发表时间 
        /// </summary>
        [EnumMember]
        Time_Up,
        [EnumMember]
        Time_Down,

        /// <summary>
        /// 排序 入住会员
        /// </summary>
        [EnumMember]
        CheckIn_Up,
        [EnumMember]
        CheckIn_Down,

        /// <summary>
        /// 排序 评价
        /// </summary>
        [EnumMember]
        Rating_Up,
        [EnumMember]
        Rating_Down,

        /// <summary>
        /// 先看朋友
        /// </summary>
        [EnumMember]
        Friend,

        /// <summary>
        /// 排序 点评来源 周末酒店用户写的点评在前
        /// </summary>
        [EnumMember]
        Source
    }
}
