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
    /// 搜索行为模型
    /// </summary>
    public class QaSearchBehavior
    {
        [DataMemberAttribute()]
        public int Id;

        [DataMemberAttribute()]
        public string ParentQuestion;
        
        [DataMemberAttribute()]
        public string Question;

        [DataMemberAttribute()]
        public string QuestionUrl;

        [DataMemberAttribute()]
        public int AnswerCount;

        [DataMemberAttribute()]
        public DateTime CreateTime;

        [DataMemberAttribute()]
        public string CreateBy;
    }
}
