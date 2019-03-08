using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
    [DataContract]
    public class ReviewResult : ListResult
    {     

         [DataMember]
        public IEnumerable<CommentItem> Result { get; set; }

         [DataMember]
         public string Featured { get; set; }

         [DataMember]
         public string RatingType { get; set; }

         [DataMember]
         public decimal Score { get; set; }


         [DataMember]
         public List<OTAInfo> OTAInfos;

    }
}
