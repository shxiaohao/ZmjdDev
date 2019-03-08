using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
    [DataContract]
    public class WapHotelsResult:ListResult
    {
        [DataMember]
        public IEnumerable<SimpleHotelItem> Result { get; set; }

        [DataMember]
        public AppMenuEntity menu { get; set; }

        public WapHotelsResult()
        {
            menu = new AppMenuEntity();
        }
    }
}