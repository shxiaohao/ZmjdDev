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
    /// 关联词模型
    /// </summary>
    public class QaRelationWordEntity
    {
        [DataMemberAttribute()]
        public Int64 Id;

        [DataMemberAttribute()]
        public string OriWord;

        [DataMemberAttribute()]
        public string RelWord;

        [DataMemberAttribute()]
        public int Type;
    }
}
