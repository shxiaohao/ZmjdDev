using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class HotelReviewItem
    {
         [DataMember]
        public int id { get; set; }
         [DataMember]
        public string name { get; set; }
         [DataMember]
        public string title { get; set; }
         [DataMember]
        public string text { get; set; }
         [DataMember]
        public  int wholeRate { get; set; }
         [DataMember]
        public int ratingRoom { get; set; }
         [DataMember]
        public int ratingAtmosphere { get; set; }
         [DataMember]
        public int ratingService { get; set; }
         [DataMember]
        public int ratingCostBenefit { get; set; }
         [DataMember]
        public string tripRadio { get; set; }
         [DataMember]
        public string identity { get; set; }
         [DataMember]
        public bool recommend { get; set; }
        [DataMember]
        public int source { get; set; }
        
    }
}
