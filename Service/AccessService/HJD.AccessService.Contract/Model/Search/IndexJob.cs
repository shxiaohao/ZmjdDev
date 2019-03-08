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
    /// 索引操作类
    /// </summary>
    public class IndexJob
    {
        [DataMemberAttribute()]
        public string Id;

        [DataMemberAttribute()]
        public IndexJobType JobType;

        [DataMemberAttribute()]
        public SearchType IndexType;
    }
}
