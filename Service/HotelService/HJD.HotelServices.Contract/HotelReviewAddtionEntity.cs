using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{

    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelReviewAddtionEntity
    {
        /// <summary>
        /// 用户UID
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// 点评iD
        /// </summary>
        [DataMember]
        public int Writing { get; set; }

        /// <summary>
        /// 补充点评内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int OrderID { get; set; }


    }
}
