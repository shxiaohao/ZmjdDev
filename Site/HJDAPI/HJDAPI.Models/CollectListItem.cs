using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class CollectListItem
    {        
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public List<FeaturedEntity> FeaturedList { get; set; }
        [DataMember]
        public double GLat { get; set; }
        [DataMember]
        public double GLon { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public decimal MinPrice { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string PicSURL { get; set; }
        [DataMember]
        public string Picture { get; set; }
        [DataMember]
        public int PriceType { get; set; }
        [DataMember]
        public int Rank { get; set; }
        [DataMember]
        public int ReviewCount { get; set; }
        [DataMember]
        public decimal Score { get; set; }
        [DataMember]
        public int InterestID { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Brief { get; set; }
    }
}
