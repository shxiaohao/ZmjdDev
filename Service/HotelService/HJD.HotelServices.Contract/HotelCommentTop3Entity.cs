using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    public class HotelReviewTop3Entity
    {
        /// <summary>
        /// 点评ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int Writing { get; set; }

        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int HotelID { get; set; }

        /// <summary>
        /// 点评标题
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string CommentSubject { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Content { get; set; }

        /// <summary>
        /// 酒店点评分
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public decimal Rating { get; set; }
       
        /// <summary>
        /// 点评发表时间
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "1990-1-1", IsOptional = true)]
        public DateTime WDate { get; set; }

        /// <summary>
        /// 点评Userid
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public long UserID { get; set; }
    }
}
