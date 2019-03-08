using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices
{
    public struct HotelListFilterPrefix
    {
        public static long BaseOffset = 10000000000;

        public static long DistrictID = 1 * BaseOffset;

        /// <summary>
        /// 酒店标签。。。
        /// </summary>
        public static long Tags = 2 * BaseOffset;

        /// <summary>
        /// POI附近
        /// </summary>
        public static long NearBy = 3 * BaseOffset;

        /// <summary>
        /// 设备
        /// </summary>
        public static long Facility = 4 * BaseOffset;
        public static long Zone = 5 * BaseOffset;//商业区
        public static long Class = 6 * BaseOffset;
        public static long Brand = 7 * BaseOffset;
        public static long Location = 8 * BaseOffset;//行政区

        /// <summary>
        /// 附近组 如地铁一号线
        /// </summary>
        public static long NearbyGroup = 9 * BaseOffset;

        /// <summary>
        /// 星级
        /// </summary>
        public static long Star = 10 * BaseOffset;

        /// <summary>
        /// 酒店属性
        /// </summary>
        public static long Attribute = 11 * BaseOffset;

        /// <summary>
        /// 超值酒店
        /// </summary>
        public static long Valued = Attribute + 1;

        /// <summary>
        /// 频道页展示酒店 （评分大于等于4.3）
        /// </summary>
        public static long ChannelHotel = 12 * BaseOffset;

        /// <summary>
        /// 景区
        /// </summary>
        public static long Attraction = 13 * BaseOffset;

        /// <summary>
        /// 酒店特色。。。
        /// </summary>
        public static long Featured = 14 * BaseOffset;

        /// <summary>
        /// 酒店主题
        /// </summary>
        public static long HotelTheme = 15 * BaseOffset; 
        
        /// <summary>
        /// 酒店玩点
        /// </summary>
        public static long Interest = 16 * BaseOffset;
        
        /// <summary>
        /// 主题数组
        /// </summary>
        public static long InterestArray = 16 * BaseOffset;//主题数组 一样是16

        /// <summary>
        /// 酒店玩点属地
        /// </summary>
        public static long InterestPlace = 17 * BaseOffset;

        /// <summary>
        /// 酒店靠近的商圈或景区(主要是自己画的区域)
        /// </summary>
        public static long ZonePlace = 18 * BaseOffset;

        /// <summary>
        /// 酒店维护的设施数据
        /// </summary>
        public static long HotelFacility = 19 * BaseOffset;

        /// <summary>
        /// 酒店状态   1.上线数据  2：精选酒店
        /// </summary>
        public static long HotelState = 20 * BaseOffset;

        /// <summary>
        /// 酒店类型2   1:别墅 2：公寓 3：民宿 4：可以携带宠物
        /// </summary>
        public static long Class2 = 21 * BaseOffset;

        /// <summary>
        /// 22出游类型
        /// </summary>
        public static long TripType = 22 * BaseOffset;

        /// <summary>
        /// 23 可查询标签
        /// </summary>
        public static long FeaturedTree = 23 * BaseOffset;


        /// <summary>
        /// 24 城市周边
        /// </summary>
        public static long CityAround = 24 * BaseOffset;
         

    }

    public enum HotelListSortType
    {
        Rank = 0,
        Price = 1,
        Distance = 2,
        NewReview = 3,
        ValuedFirst = 4
    }

    public enum HotelClassType
    {
        All = 0,//全部酒店
        Cheap = 1,//  经济连锁
        Hostel = 2,//	青年旅舍
        Inn = 3,//	客栈
        BNB = 4,//	家庭旅馆
        //5	酒店公寓
        ResortHotel = 9,//	度假酒店
        Resort = 10,//	度假村
        Boutique = 11	//精品酒店

        //-度假 haojiudian.com/vacation
    }

    [Serializable]
    public class HotelListInfoMongoDB
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        public int _id { get; set; } //HotelID

        public string Signature { get; set; }
        //  public List<int> HotelClass { get; set; }

        //  public int HotelBrand { get; set; }

        /// <summary>
        /// long前32位代表类型，后32位代表值
        /// 过滤字段：1 目的地；（聚合） 2.过滤组 （高性价比等）3.POI附件 
        /// </summary>
        public List<long> FilterCol { get; set; }

        public List<HotelDistanceForMongoDB> HotelDistances { get; set; }

        public Coordinate Gloc { get; set; }

        public Coordinate Bloc { get; set; }

        public int Rank { get; set; }

        public int Star { get; set; }
    }

    [Serializable]
    public class Coordinate
    {
        public double Lat;
        public double Lng;
        public Coordinate(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }
    }

    [Serializable]
    public class HotelDistanceForMongoDB
    {
        public int POIID { get; set; }

        public int Distance { get; set; }

        public HotelDistanceForMongoDB(int poiID, int distance)
        {
            POIID = poiID;
            Distance = distance;
        }
    }

    [Serializable]
    public class HotelListInfoForMongoDB
    {
        public int _id { get; set; }

        public List<HotelDistanceForMongoDB> HotelDistances { get; set; }

        public int Rank { get; set; }

        public decimal AvgPrice { get; set; }
    }
}
