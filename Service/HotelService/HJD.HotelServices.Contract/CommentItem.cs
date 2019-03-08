using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 评论数据
    /// </summary>
    [DataContract]
    [Serializable]
    public class CommentItem
    {
        public CommentItem()
        {
            RoomInfo = "";
            TripInfo = "";
            Time = "";
            Author = "";
            AvatarUrl = "";
            Text = "";
            AdditionalText = "";
            ChannelName = "";
            ScoreDetail = "";
            OTAAccessUrl = "";
            CommentPics = new List<string>();
            BigCommentPics = new List<string>(); ;
        }
        /// <summary>
        /// 评论ID
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int PhotoCount { get; set; }

        /// <summary>
        /// 房间信息
        /// </summary>
        [DataMember]
        public string RoomInfo { get; set; }

        /// <summary>
        /// 出游类型
        /// </summary>
        [DataMember]
        public string TripInfo { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        [DataMember]
        public string Time { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [DataMember]
        public decimal Score { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [DataMember]
        public string Author { get; set; }
        
        /// <summary>
        /// 点评照片
        /// </summary>
        [DataMember]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// 追加点评内容
        /// </summary>
        [DataMember]
        public string AdditionalText { get; set; }

        /// <summary>
        /// 点评标题
        /// </summary>
        [DataMemberAttribute()]
        public string CommentTitle { get; set; }

        /// <summary>
        /// OTAID
        /// </summary>
        [DataMember]
        public Int32 ChannelID { get; set; }

        /// <summary>
        /// OTA中文名
        /// </summary>
        [DataMember]
        public string ChannelName { get; set; }

        /// <summary>
        /// 卫生4  服务5 设施 5 位置 3  
        /// </summary>
        [DataMember]
        public string ScoreDetail { get; set; }

        /// <summary>
        /// OTA点评访问路径
        /// </summary>
        [DataMember]
        public string OTAAccessUrl { get; set; }

        [DataMember]
        public List<string> CommentPics { get; set; }

        [DataMember]
        public List<string> BigCommentPics { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Int32 UsefulCount { get; set; }

        /// <summary>
        /// 查看点评列表的用户是否 对该点评点过有帮助
        /// </summary>
        [DataMember]
        public bool HasClickUseful { get; set; }

        /// <summary>
        /// 作者在周末酒店的yhuID
        /// </summary>
        [DataMember]
        public long AuthorUserID { get; set; }
    }
    
    [DataContract]
    [Serializable]
    public class CommentAddHotelEntity
    {
        /// <summary>
        /// 添加酒店临时ID
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int ID { get; set; }

        /// <summary>
        /// 转为真正的酒店ID
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelID { get; set; }

        /// <summary>
        /// 添加酒店名称
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string HotelName { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public long Creator { get; set; }
    }
}