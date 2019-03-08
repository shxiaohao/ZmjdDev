using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.HotelServices.Contracts;
using HJD.HotelServices;

namespace HJDAPI.Models
{
    [DataContract]
    public class ReviewResult2
    {
        public  ReviewResult2() { TFTList  = new List<HotelTFTRelItemEntity>();}

        [DataMember]
        public int HotelID { get; set; }

        [DataMember]
        public int Start { get; set; }

        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int InterestID { get; set; }

        [DataMember]
        public int TFTType { get; set; }

        [DataMember]
        public int TFTID { get; set; }

        [DataMember]
        public int RatingType { get; set; }

        [DataMember]
        public string RatingPercent { get; set; }
        
        [DataMember]
        public decimal Score { get; set; }

        [DataMember]
        public IEnumerable<CommentItem> Result { get; set; }

        //[DataMember]
        //public List<OTAInfo> OTAInfos;


        [DataMember]
        public List<HotelTFTRelItemEntity> TFTList;

    }
}
