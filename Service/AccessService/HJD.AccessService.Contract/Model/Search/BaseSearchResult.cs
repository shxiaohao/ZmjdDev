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
    /// 搜索引擎缺省搜索结果模型
    /// </summary>
    public class BaseSearchResult
    {
        [DataMemberAttribute()]
        public string Id;

        [DataMemberAttribute()]
        public float Boost;

        //[DataMemberAttribute()]
        public string Explain;
    }
}
