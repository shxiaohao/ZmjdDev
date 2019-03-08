using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
    [DataContract]
    public class HotelsResult:ListResult
    {
        [DataMember]
        public IEnumerable<HotelItem> Result { get; set; }

        [DataMember]
        public int ValuedCount { get; set; }

        public int Type { get; set; }

        public int Stype { get; set; }
    }

    [DataContract]
    public class CanSellDistrictHotelResult
    {
        [DataMember]
        public Dictionary<string,Dictionary<string,List<CanSaleHotelInfoEntity>>> data { get; set; }
    }
}