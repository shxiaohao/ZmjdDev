using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class HotelVoucherAndInspectorHotel
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        [DataMember]
        public Int32 HotelID { get; set; }
        /// <summary>
        /// 酒店名称
        /// </summary>
        [DataMember]
        public string HotelName { get; set; }

        [DataMember]
        public bool IsExpired { get; set; }

        [DataMember]
        public bool IsValid { get; set; }
        /// <summary>
        /// 需要积分
        /// </summary>
        [DataMember]
        public int RequiredPoint { get; set; }

        [DataMember]
        public int InspectorHotelID { get; set; }

        [DataMember]
        public int HVID { get; set; }

        /// <summary>
        /// 标识1new房券，2old品鉴酒店
        /// </summary>
        [DataMember]
        public int BS { get; set; }

        [DataMember]
        public ListHotelItem2 hotelItem { get; set; }

        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 是否展示
        /// </summary>
        [DataMember]
        public bool IsShow { get; set; }
       
    }
}
