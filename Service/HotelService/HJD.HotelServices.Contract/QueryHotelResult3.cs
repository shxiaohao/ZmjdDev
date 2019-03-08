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
    public class QueryHotelResult3
    {
        [DataMember]
        public int TotalCount { get; set; }
        [DataMember]
        public Dictionary<long, int> FilterCount { get; set; }
        [DataMember]
        public List<ListHotelItem3> HotelList { get; set; }

        public QueryHotelResult3()
        {
            FilterCount = new Dictionary<long, int>();
            HotelList = new List<ListHotelItem3>();
        }
    }
}
