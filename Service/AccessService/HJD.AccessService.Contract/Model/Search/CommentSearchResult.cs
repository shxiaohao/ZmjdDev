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
    /// 点评搜索结果模型
    /// </summary>
    public class CommentSearchResult
    {
        [DataMemberAttribute()]
        public int HotelId;
    }
}
