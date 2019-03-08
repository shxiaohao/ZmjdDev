using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店点评
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelReviewExEntity
    {
        /// <summary>
        /// 点评id
        /// </summary>
        [DataMember]
        public int Writing { get; set; }

        /// <summary>
        /// 点评类型  1:携程 2：其它 3：周末
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int WritingTypeID { get; set; }

        /// <summary>
        /// 点评照片数
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int PhotoCount { get; set; }

        /// <summary>
        /// 房间信息
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string  RoomInfo { get; set; }

        /// <summary>
        /// 出游类型
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string TripInfo { get; set; }

        /// <summary>
        /// 打分
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public float Score { get; set; }

        /// <summary>
        /// 酒店ID
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int HotelID { get; set; }

        /// <summary>
        /// 点评标题
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string CommentSubject { get; set; }

        /// <summary>
        /// 主表中的点评分，四项的平均分,目前是酒店列表中所用
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public decimal Rating { get; set; }

        /// <summary>
        /// 房间卫生
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int RatingRoom { get; set; }

        /// <summary>
        /// 周边环境
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int RatingAtmosphere { get; set; }

        /// <summary>
        /// 酒店服务
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int RatingService { get; set; }

        /// <summary>
        /// 设施设备
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int RatingCostBenefit { get; set; }

        /// <summary>
        /// 用户UserID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public long UserID { get; set; }

        /// <summary>
        /// 出游类型
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int UserIdentity { get; set; }

        /// <summary>
        /// 点评发表时间
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "1990-1-1", IsOptional = true)]
        public DateTime WDate { get; set; }

        /// <summary>
        /// 携程订单号
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int OrderID { get; set; }

        /// <summary>
        /// 删除标志 F未删除 T删除
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "F", IsOptional = true)]
        public string Deleted { get; set; }

        /// <summary>
        /// 审核标志 W待审核 D删除 P审核通过
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "D", IsOptional = true)]
        public string Status { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string Content { get; set; }

        /// <summary>
        /// 点评标题
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string CommentTitle { get; set; }

        /// <summary>
        /// 其它出游类型
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string UserIdentityText { get; set; }

        /// <summary>
        /// 是否推荐 T推荐
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "F", IsOptional = true)]
        public string IsRecommend { get; set; }

        /// <summary>
        /// 目的地ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int DistrictID { get; set; }
        
        /// <summary>
        /// 点评类型
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string WritingType { get; set; }

        /// <summary>
        /// 有用点评数
        /// </summary>
        [DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int Useful { get; set; }

        /// <summary>
        /// 补充点评时间
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "1990-1-1", IsOptional = true)]
        public DateTime MDate { get; set; }

        /// <summary>
        /// 补充点评内容
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string HoContent { get; set; }

        /// <summary>
        /// 酒店反馈时间
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "1990-1-1", IsOptional = true)]
        public DateTime FBDate { get; set; }

        /// <summary>
        /// 酒店反馈内容
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string FBContent { get; set; }

        /// <summary>
        /// Uid
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string Uid { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int NeedDealLater { get; set; }

         
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int ChannelType { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public decimal Wholerate { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int OTAReviewID { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<string> CommentPics { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<string> BigCommentPics { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 UsefulCount { get; set; }

        ///// <summary>
        ///// OTA点评访问连接
        ///// </summary>
        //[DataMemberAttribute()]
        //[DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        //public string AccessUrl { get; set; }
    }
}