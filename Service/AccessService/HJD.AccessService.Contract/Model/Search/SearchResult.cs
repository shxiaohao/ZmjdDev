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
    /// 全局搜索结果模型
    /// </summary>
    public class SearchResult
    {
        [DataMemberAttribute()]
        public SearchType Type;
        
        [DataMemberAttribute()]
        public List<HotelSearchResult> HotelList;

        [DataMemberAttribute()]
        public List<DistrictSearchResult> DistrictList;

        [DataMemberAttribute()]
        public List<ThemeSearchResult> ThemeList;

        [DataMemberAttribute()]
        public List<BrandSearchResult> BrandList;

        [DataMemberAttribute()]
        public List<CommentSearchResult> CommentList;
    }
}
