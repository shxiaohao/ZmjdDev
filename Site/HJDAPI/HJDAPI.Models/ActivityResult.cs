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
    public class ActivityResult:CommentShareModel
    {
    }

    [Serializable]
    [DataContract]
    public class SuggesstionParam : BaseParam
    {
        /// <summary>
        /// 建议类型
        /// </summary>
        [DataMember]
        public int SuggestionType { get; set; }

        /// <summary>
        /// 建议内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// 提建议de用户ID
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
    }

    [Serializable]
    [DataContract]
    public class BaseSubmitResult
    {
        /// <summary>
        /// 提交建议成功与否
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
}