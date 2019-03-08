using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
    [DataContract]
    public class WapThemeHotelsResult
    {
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int Start { get; set; }

        [DataMember]
        public int ThemeID { get; set; }

        [DataMember]
        public string ThemeName { get; set; }

        [DataMember]
        public string HotelName { get; set; }

        [DataMember]
        public IEnumerable<SimpleHotelItem> Result { get; set; }

        public WapThemeHotelsResult()
        {
        }
    }
}