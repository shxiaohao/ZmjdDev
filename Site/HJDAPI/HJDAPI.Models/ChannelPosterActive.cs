using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    /// <summary>
    /// 活动海报分享
    /// </summary>
    [Serializable]
    [DataContract]
    public class ChannelPosterActive
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string PosterBannerUrl { get; set; }
        [DataMember]
        public string PosterDesc { get; set; }
        [DataMember]
        public string PosterTip { get; set; }
        [DataMember]
        public string ShareDesc { get; set; }
        [DataMember]
        public string ActiveLink { get; set; }
        [DataMember]
        public decimal ActivePrice { get; set; }
        [DataMember]
        public decimal MarketPrice { get; set; }
        [DataMember]
        public int PosterType { get; set; }
        [DataMember]
        public int WeixinAcountId { get; set; }
        [DataMember]
        public int State { get; set; }

    }

}
