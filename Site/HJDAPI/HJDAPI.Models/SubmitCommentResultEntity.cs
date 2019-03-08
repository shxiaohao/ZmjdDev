using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class SubmitCommentResultEntity
    {
        [DataMember]
        public int Success { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public int CommentID { get; set; }
        [DataMember]
        public int ShareCoupon { get; set; }
        [DataMember]
        public string DetailUrl { get; set; }
        /// <summary>
        /// 点评提交成功后 前往的h5页面
        /// </summary>
        [DataMember]
        public string NextUrl { get; set; }
    }
}