using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices
{
    [Serializable]
    [DefaultColumn]
    public class VacationHotelPyRank
    {
        /// <summary>
        /// 度假酒店id
        /// </summary>
        [DBColumn(DefaultValue = "0")]
        public int VacationHotelID { get; set; }
               

        /// <summary>
        /// py排名
        /// </summary>
        [DBColumn(DefaultValue = "0")]
        public long PyRank { get; set; }
    }
}

