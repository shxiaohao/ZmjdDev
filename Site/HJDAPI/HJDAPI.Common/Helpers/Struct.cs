using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Common.Helpers
{
    public class Struct
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
            /// 酒店玩点(主题)
            /// </summary>           
            public static long Interest = 16 * BaseOffset;

            /// <summary>
            /// 酒店玩点所属地区
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
        }

        public struct HotelDetailDisplayFilterPrefix
        {
            public static int BaseOffset = 1000000;
            public static int RoomType = 19 * BaseOffset;
        }
    }
}