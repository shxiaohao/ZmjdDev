using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices
{
    [Serializable]
    public class VacationInfoMongoDB
    {
        /// <summary>
        /// 酒店id
        /// </summary>
      
        public int HotelID { get; set; }

        public string Signature { get; set; }
        public List<int> HotelClass { get; set; }

        public int DistrictID { get; set; }

        public int Zone { get; set; }

        public int Location { get; set; }

        /// <summary>
        /// 拼音排序
        /// </summary>      
        public long pySort { get; set; }
    }
    [Serializable]
    public class VacationSignatureForMongoDB
    {
        public int HotelID { get; set; }
        
    }
}

