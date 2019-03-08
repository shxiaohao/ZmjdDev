using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HJD.HotelServices
{
    public struct VacationFilterPrefix
    {
        public const long BaseOffset = 10000000000;
        public const long HotelClass = 1 * BaseOffset;
        public const long Location = 2 * BaseOffset;
        public const long Zone = 3 * BaseOffset;
        //public const long District = 4 * BaseOffset;

    }

    [Serializable]
    public class VacationForMongodb
    {
        public long _id { get; set; }

        public string Signature { get; set; }

        /// <summary>
        /// 目的地id
        /// </summary>
        public int DistinctID { get; set; }

        /// <summary>
        /// 酒店id
        /// </summary>
        public int HotelID { get; set; }

        /// <summary>
        /// 过滤字段：1 酒店类别；2 行政区；3 商区
        /// </summary>
        public List<long> FilterCol { get; set; }

       

        /// <summary>
        /// 排序 拼音
        /// </summary>
        public long OrderColPinYin { get; set; }

     
    }

    
}

