using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.DestServices.Contract;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
    [DataContract]
    public class PicturesResult : ListResult
    {
        [DataMember]
        public IEnumerable<PictureItem> Result { get; set; }

        [DataMember]
        public HotelItem Hotel
        {
            get;
            set;
        }
    }

    [DataContract]
    public class ValidationResult : BasePostResponse
    {
        [DataMember]
        public string base64Url { get; set; }
    }
}