using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HJD.HotelServices
{
    public struct HotelReviewFilterPrefix
    {
        public static long BaseOffset = 10000000000;
        public static long UserIdentity = 1 * BaseOffset;
        public static long Rate = 2 * BaseOffset;
        public static long Hotel = 3 * BaseOffset;
        //public static long District = 4 * BaseOffset;
        public static long Featured = 5 * BaseOffset;
        public static long Tag = 6 * BaseOffset;
        public static long Theme = 7 * BaseOffset;
        public static long FeaturedTreeID = 8 * BaseOffset;
        public static long RoomType = 19 * BaseOffset;
    }

    [Serializable]
    public class HotelReviewForMongodb
    {
        public int _id { get; set; }

        public string Signature { get; set; }

        /// <summary>
        /// 酒店
        /// </summary>
        public int Hotel { get; set; }

        /// <summary>
        /// 过滤字段：1 出游类型；2 评价
        /// </summary>
        public List<long> FilterCol { get; set; }

        /// <summary>
        /// 排序 发表时间 
        /// </summary>
        public long OrderColTime { get; set; }

        /// <summary>
        /// 排序 入住会员
        /// </summary>
        public long OrderColCheckIn { get; set; }

        /// <summary>
        /// 排序 评价
        /// </summary>
        public long OrderColRating { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// 删除标记，0未删除
        /// </summary>
        public int Deleted { get; set; }

        /// <summary>
        /// 审核标记，0 W 待审核；1 D 删除；2 P 审核通过
        /// </summary>
        public int Status { get; set; }
    }

    [Serializable]
    public class HotelReviewSignatureForMongoDB
    {
        public int _id { get; set; }

        public string Signature { get; set; }
    }
}
