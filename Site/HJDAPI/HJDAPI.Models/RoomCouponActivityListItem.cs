using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class RoomCouponActivityListItem
    {
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int AlbumsRank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int SKUSortNo { get; set; }
        /// <summary>
        /// 套餐code
        /// </summary>
        [DataMember]
        public string PackageCode { get; set; }
        /// <summary>
        ///  sku对应的属性ID  1：首单
        /// </summary>
        [DataMember]
        public int TagID { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        [DataMember]
        public int ActivityID { get; set; }

        [DataMember] 
        public int Rank { get; set;  }
        /// <summary>
        /// 抢购券活动状态 0卖完 1结束 2可以继续
        /// </summary>
        [DataMember]
        public int ActivityOpenState { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        [DataMember]
        public string HotelName { get; set; }

        [DataMember]
        public string DistrictEName { get; set; }

        [DataMember]
        public int DistrictId { get; set; }
        
        [DataMember]
        public string DistrictName { get; set; }

        /// <summary>
        /// 价格优惠标签
        /// </summary>
        [DataMember]
        public string PriceType { get; set; }

        /// <summary>
        /// 平日或周末
        /// </summary>
        [DataMember]
        public string PriceDateType { get; set; }

        /// <summary>
        /// 价格（售价）
        /// </summary>
        [DataMember]
        public Decimal Price { get; set; }

        /// <summary>
        /// 酒店照片
        /// </summary>
        [DataMember]
        public string PicUrl { get; set; }

        /// <summary>
        /// 剩余套数(包括锁定的数量 totalNum - SellNum)
        /// </summary>
        [DataMember]
        public int LeaveNum { get; set; }

        /// <summary>
        /// 套餐的简短介绍
        /// </summary>
        [DataMember]
        public string PackageBrief { get; set; }

        /// <summary>
        /// 券活动的市场价（单位元）
        /// </summary>
        [DataMember]
        public Int32 MarketPrice { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        [DataMember]
        public DateTime StartSellTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        [DataMember]
        public DateTime EndSellTime { get; set; }


        [DataMember]
        public int GroupNo { get; set; }
    }

    [Serializable]
    [DataContract]
    public class RoomCouponActivityListModel
    {
        /// <summary>
        /// top图片
        /// </summary>
        [DataMember]
        public string TopPicUrl { get; set; }
        
        /// <summary>
        /// 房券抢购活动列表
        /// </summary>
        [DataMember]
        public List<RoomCouponActivityListItem> Items { get; set; }
    }
}
