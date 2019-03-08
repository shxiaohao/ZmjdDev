using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices
{
    [Serializable]
    [DefaultColumn]
    public class HotelRankByDistrict
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        [DBColumnAttribute(DefaultValue = "0")]
        public int HotelID { get; set; }

        [DBColumnAttribute(DefaultValue = "0")]
        public int Location { get; set; }

        [DBColumnAttribute(DefaultValue = "0")]
        public int DistrictID { get; set; }

        [DBColumn(DefaultValue = "0")]
        public int Zone { get; set; }

        /// <summary>
        /// 酒店品牌
        /// </summary>
        [DBColumn(DefaultValue = "0")]
        public int Brand { get; set; }

        //[DBColumn(IsOptional = true)]
        //public List<int> FacilityList { get; set; }

        [DBColumn(IsOptional = true)]
        public List<HotelClassRankEntity> HotelClass { get; set; }

        [DBColumn(DefaultValue = "0")]
        public long CtripScoreRank { get; set; }

        [DBColumn(DefaultValue = "0")]
        public long HotelNameRank { get; set; }
    }
}
