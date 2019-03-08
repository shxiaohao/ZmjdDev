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
    public class HotelVoucherAndInspectorHotelListResult
    {
        [DataMember]
        public HotelVoucherListResult HotelVoucherResult { get; set; }
        [DataMember]
        public InspectorHotelListResult InspectorHotelResult { get; set; }
    }


    [DataContract]
    public class HotelVoucherListResult
    {
        [DataMember]
        public HotelVoucherList currentResult { get; set; }
        [DataMember]
        public HotelVoucherList pastResult { get; set; }

    }

    [DataContract]
    public class HotelVoucherList
    {
        /// <summary>
        /// 品鉴酒店的数量
        /// </summary>
        [DataMember]
        public int count { get; set; }

        [DataMember]
        public List<HotelVoucherResult> items { get; set; }
    }

    [DataContract]
    public class HotelVoucherResult
    {
        [DataMember]
        public HotelVoucherEntity HotelVoucher { get; set; }

        [DataMember]
        public ListHotelItem2 hotelItem { get; set; }

        [DataMember]
        public bool isEnrolled { get; set; }
    }

}
