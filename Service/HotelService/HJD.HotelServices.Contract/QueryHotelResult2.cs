using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    public class QueryHotelResult2
    {
        [DataMember]
        public int TotalCount { get; set; }
        [DataMember]
        public Dictionary<long, int> FilterCount { get; set; }
        [DataMember]
        public List<ListHotelItem2> HotelList { get; set; }

        public QueryHotelResult2()
        {
            FilterCount = new Dictionary<long, int>();
            HotelList = new List<ListHotelItem2>();
        }
    }
}
