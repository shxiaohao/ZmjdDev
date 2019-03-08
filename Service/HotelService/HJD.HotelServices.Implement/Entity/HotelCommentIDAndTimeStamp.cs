
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices
{
    [Serializable]
    [DefaultColumn]
    public class HotelReviewIDAndTimeStamp
    {
        /// <summary>
        /// 问答id
        /// </summary>
        public int HotelReviewID { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long TimeStamp { get; set; }
    }
}