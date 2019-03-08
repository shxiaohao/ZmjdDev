using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店点评实体类-用户提交点评的各项数据参数
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelReviewEntity
    {
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string ACode
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string EmailNotice
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string IsOrdersucess
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Uid
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int Hotel
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Title
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int Rating
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int RatingRoom
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int RatingAtmosphere
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int RatingService
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int RatingCostBenefit
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int RatingValued
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int User_Identity
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Identitytxt
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Together
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string CommentSubject
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Content
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string NewisCommand
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Style
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Installation
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int OrderID
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int Room
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string RoomName
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Ipaddress
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Status
        {
            get;
            set;
        }

        /// <summary>
        /// 驴评使用：状态
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int StatusDetail
        {
            get;
            set;
        }

        /// <summary>
        /// 驴评使用：是否删除
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Deleted
        {
            get;
            set;
        }

        /// <summary>
        /// 驴评使用：点评审核人
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Operator
        {
            get;
            set;
        }

        /// <summary>
        /// 驴评使用：2个数据库的点评id对应
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int OldWriting
        {
            get;
            set;
        }

        /// <summary>
        /// 驴评使用：点评来源
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string WritingType
        {
            get;
            set;
        }
        /// <summary>
        /// 新登录系统UserID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public long UserID { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int Source { get; set; }

        /// <summary>
        /// 订单中原始的酒店ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int OrderHotel { get; set; }


        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int NeedDealLater { get; set; }

        /// <summary>
        /// 用户主观总评分
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int WholeRate { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public string Wdates { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int Writing { get; set; }
    }
}
