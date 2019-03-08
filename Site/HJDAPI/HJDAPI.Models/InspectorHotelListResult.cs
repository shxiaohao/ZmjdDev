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
    public class InspectorHotelListResult
    {
        [DataMember]
        public HotelListResult currentResult { get; set; }

        [DataMember]
        public HotelListResult pastResult { get; set; }
    }

    [DataContract]
    public class HotelListResult
    {
        [DataMember]
        public int count { get; set; }

        [DataMember]
        public List<InspectorHotelResult> items { get; set; }
    }

    [DataContract]
    public class InspectorHotelResult
    {
        //[DataMember]
        //public string hotelName { get; set; }
        //[DataMember]
        //public List<string> hotelPics { get; set; }

        [DataMember]
        public InspectorHotel inspectorHotel { get; set; }

        [DataMember]
        public ListHotelItem2 hotelItem { get; set; }

        [DataMember]
        public bool isEnrolled { get; set; }
    }
}