using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices
{
    [Serializable]
    [DefaultColumn]
    public class HotelClassRankEntity
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        [DBColumn(DefaultValue = "0")]
        public int HotelID { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [DBColumn(DefaultValue = "0")]
        public int HotelClass { get; set; }

        /// <summary>
        /// 分类排名
        /// </summary>
        [DBColumn(DefaultValue = "0")]
        public int Rank { get; set; }
    }
}
