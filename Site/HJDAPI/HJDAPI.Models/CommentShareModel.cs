using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class CommentShareModel
    {
        /// <summary>
        /// 默认显示酒店名分享标题
        /// </summary>
        [DataMember]
        public string title { get; set; }
        /// <summary>
        /// 非酒店名分享title
        /// </summary>
        [DataMember]
        public string notHotelNameTitle { get; set; }
        [DataMember]
        public string photoUrl { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string shareLink { get; set; }
        /// <summary>
        /// 分享成功后的回调页面
        /// </summary>
        [DataMember]
        public string returnUrl { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CommentAddReviewResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }
    }


    [Serializable]
    [DataContract]
    public class CommentAddReviewParam:BaseParam
    {
        [DataMember]
        public int CommentID { get; set; }
        
        /// <summary>
        /// 回复的哪条评论 以后更换评论列表显示用
        /// </summary>
        [DataMember]
        public int ParentReviewID { get; set; }

        [DataMember]
        public long ReceiveUser { get; set; }

        [DataMember]
        public long SendUser { get; set; }

        [DataMember]
        public string ReviewContent { get; set; }
    }
}