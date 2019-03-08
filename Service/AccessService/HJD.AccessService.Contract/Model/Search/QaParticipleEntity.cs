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
    /// 分词结果模型
    /// </summary>
    public class QaParticipleEntity
    {
        [DataMemberAttribute()]
        public string Id="";

        [DataMemberAttribute()]
        public string Word;

        [DataMemberAttribute()]
        public string Speech;

        [DataMemberAttribute()]
        public QaWordType Type;

        [DataMemberAttribute()]
        public List<QaParticipleEntity> Words;
    }
}
