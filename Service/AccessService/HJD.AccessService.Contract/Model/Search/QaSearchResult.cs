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
    public class QaSearchResult
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public string Id;

        /// <summary>
        /// 问题的分词结果
        /// </summary>
        [DataMemberAttribute()]
        public List<QaParticipleEntity> QuestionWords;

        /// <summary>
        /// 酒店ID列表
        /// </summary>
        [DataMemberAttribute()]
        public List<BaseSearchResult> HotelIds;

        /// <summary>
        /// 酒店列表
        /// </summary>
        [DataMemberAttribute()]
        public List<HotelSearchResult> Hotels;

        /// <summary>
        /// 附属项列表
        /// </summary>
        [DataMemberAttribute()]
        public List<FilterSearchResult> Filters;

        [DataMemberAttribute()]
        public string Keyword;

        [DataMemberAttribute()]
        public string CheckIn;

        [DataMemberAttribute()]
        public string CheckOut;

        [DataMemberAttribute()]
        public int NightCount;

        [DataMemberAttribute()]
        public int MinPrice;

        [DataMemberAttribute()]
        public int MaxPrice;
    }
}
