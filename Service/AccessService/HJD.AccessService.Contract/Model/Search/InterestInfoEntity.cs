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
    /// 酒店主题数据模型
    /// </summary>
    public class InterestInfoEntity
    {
        [DataMemberAttribute()]
        public int Id;

        [DataMemberAttribute()]
        public string Name;

        [DataMemberAttribute()]
        public int Type;

        [DataMemberAttribute()]
        public bool Released;
    }
}
