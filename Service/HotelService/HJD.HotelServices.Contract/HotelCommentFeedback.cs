using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店点评反馈
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DefaultColumn]
    public class HotelReviewFeedback
    {
        #region 基本
        /// <summary>
        /// 主键
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public int Id { get; set; }

        private DateTime _createDate;
        /// <summary>
        /// 创建日期（默认值为当前日期）
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public DateTime CreateDate
        {
            get
            {
                if (_createDate == default(DateTime))
                {
                    return DateTime.Now;
                }
                return _createDate;
            }
            set { _createDate = value; }
        }

        /// <summary>
        /// 操作人（可以为空）
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string Operator { get; set; }

        private string _status;
        /// <summary>
        /// 状态，包含审核，删除等
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string Status
        {
            get { return _status ?? "W"; }
            set { _status = value; }
        }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public short Flag { get; set; }
        #endregion

        #region 关联
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public int HotelId { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public int WritingId { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string WritingContent { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string WritingTitle { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public DateTime WritingDate { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string HotelName { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string Nickname { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string ProfileUrlNo { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string OwnerName { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string OwnerStatus { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public int? OwnerId { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string WritingType { get; set; }

        #endregion

        #region 重要
        /// <summary>
        /// 反馈内容
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string Content { get; set; }

        /// <summary>
        /// 反馈者id
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string Uid { get; set; }

        /// <summary>
        /// 反馈idlist
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string Ids { get; set; }


        #endregion

    }
}
