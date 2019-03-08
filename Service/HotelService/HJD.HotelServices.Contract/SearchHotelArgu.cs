using System.Collections.Generic;

namespace HJD.HotelServices.Contracts
{
    public class SearchHotelArgu
    {
        /// <summary>
        /// 目的地id列表
        /// </summary>
        public List<int> DistrictID { get; set; }
        /// <summary>
        /// 酒店类型  全部:0,度假:1,精品:2,客栈:3
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 酒店子分类  超值：0 ，排名：1
        /// </summary>
        public int stype { get; set; }
        /// <summary>
        /// 排序类型(口碑：0  价格：1 距离：2 )
        /// </summary>
        public string Sort { get; set; }
        /// <summary>
        /// 排序方式 正序:0,倒序：1
        /// </summary>
        public string Order { get; set; }
        /// <summary>
        /// 开始数
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// 返回酒店数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 排除的酒店id
        /// </summary>
        public int HotelId { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 偏移范围
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// 范围：东北维度点  如果是0则忽略
        /// </summary>
        public double nLat { get; set; }
        /// <summary>
        /// 范围：东北经度点  如果是0则忽略
        /// </summary>
        public double nLng { get; set; }
        /// <summary>
        /// 范围：西南维度点  如果是0则忽略
        /// </summary>
        public double sLat { get; set; }
        /// <summary>
        /// 范围：西南经度点  如果是0则忽略
        /// </summary>
        public double sLng { get; set; }

    }
}