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
    /// 酒店品牌数据模型
    /// </summary>
    public class BrandEntity
    {
        [DataMemberAttribute()]
        public int Brand;

        [DataMemberAttribute()]
        public string BrandName;

        [DataMemberAttribute()]
        public string BrandEname;

        [DataMemberAttribute()]
        public string Group;

        [DataMemberAttribute()]
        public string BrandType;
    }
}
