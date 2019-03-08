using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.DestServices.Contract;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
    [DataContract]
    public class WapAttractionResult1
    {      
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int Start { get; set; }

        [DataMember]
        public string DistrictName { get; set; }
        

        //[DataMember]
        //public int Type { get; set; }


        [DataMember]
        public List<AttractionEntity> AttractionList { get; set; }

        //[DataMember]
        //public List<FilterDicEntity> ThemesList { get; set; }
    }
}