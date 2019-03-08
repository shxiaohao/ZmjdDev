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
    /// 主题搜索结果模型
    /// </summary>
    public class ThemeSearchResult
    {
        [DataMemberAttribute()]
        public int ThemeId;

        [DataMemberAttribute()]
        public string Name;

        [DataMemberAttribute()]
        public string Ename;

        [DataMemberAttribute()]
        public int DistrictId;

        [DataMemberAttribute()]
        public string DistrictName;
    }
}
