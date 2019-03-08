using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model
{
    [Serializable]
    [DataContract]
    /// <summary>
    /// 搜索反馈模型
    /// </summary>
    public class QaFeedback
    {
        [DataMemberAttribute()]
        public int Id;

        [DataMemberAttribute()]
        public string Question;

        [DataMemberAttribute()]
        public string QuestionUrl;

        [DataMemberAttribute()]
        public int AnswerCount;

        [DataMemberAttribute()]
        public string Feedback;

        [DataMemberAttribute()]
        public int Sure;

        [DataMemberAttribute()]
        public DateTime CreateTime;

        [DataMemberAttribute()]
        public string CreateBy;
    }
}
