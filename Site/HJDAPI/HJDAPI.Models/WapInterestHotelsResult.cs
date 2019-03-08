using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
    [DataContract]
    public class WapInterestHotelsResult
    {
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int Start { get; set; }

        [DataMember]
        public int InterestID { get; set; }

        [DataMember]
        public int InterestType { get; set; }

        [DataMember]
        public string InterestName { get; set; }


        [DataMember]
        public IEnumerable<SimpleHotelItem> Result { get; set; }

        public WapInterestHotelsResult()
        {
        }
    }
}