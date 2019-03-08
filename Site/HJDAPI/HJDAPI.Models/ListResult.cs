using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    [DataContract]
    public class ListResult
    {
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int Start { get; set; }

        [DataMember]
        public string DistrictName { get; set; }

        [DataMember]
        public string AttractionName { get; set; }

        [DataMember]
        public int ValuedCount { get; set; }

        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public int Stype{ get; set; }

        [DataMember]
        public string HotelName { get; set; }

      
      
    }
}