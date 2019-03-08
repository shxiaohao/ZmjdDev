using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
    [DataContract]
    public class CommentsResult : ListResult
    {
         [DataMember]
        public IEnumerable<CommentItem> Result { get; set; }

    }
}