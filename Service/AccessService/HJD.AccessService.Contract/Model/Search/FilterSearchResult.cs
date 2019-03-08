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
    /// 标签/筛选项/主题项
    /// </summary>
    public class FilterSearchResult
    {
        [DataMemberAttribute()]
        public string Id;

        [DataMemberAttribute()]
        public string Name;

        [DataMemberAttribute()]
        public QaWordType Type;
    }
}
