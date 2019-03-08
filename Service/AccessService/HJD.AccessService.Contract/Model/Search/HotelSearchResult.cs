using HJD.AccessService.Contract.Model.Hotel;
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
    /// 酒店搜索结果模型
    /// </summary>
    public class HotelSearchResult : BaseSearchResult
    {
        [DataMemberAttribute()]
        public int HotelId;

        [DataMemberAttribute()]
        public string HotelName;

        [DataMemberAttribute()]
        public string Ename;

        [DataMemberAttribute()]
        public string HotelDesc;

        [DataMemberAttribute()]
        public string HotelCoverPicUrl;

        [DataMemberAttribute()]
        public string Address;

        [DataMemberAttribute()]
        public string Star;

        [DataMemberAttribute()]
        public double ReviewScore;

        [DataMemberAttribute()]
        public string Themes; 

        /// <summary>
        /// 套餐数据
        /// </summary>
        [DataMemberAttribute()]
        public string PItemInfo; 

        
        [DataMemberAttribute()]
        public List<FilterSearchResult> FilterList;

        [DataMemberAttribute()]
        public List<HotelPriceSlot> PirceSlotList;

        public void LoadBase(BaseSearchResult baseResult)
        {
            Id = baseResult.Id;
            Boost = baseResult.Boost;
            Explain = baseResult.Explain;
        }
    }
}
