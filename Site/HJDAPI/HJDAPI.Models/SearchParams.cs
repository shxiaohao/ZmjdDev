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
    public class SearchParams
    {
        public SearchParams()
        {
            cityCount = 2; hotelCount = 3; playCount = 3; foodCount = 3; needHighlight = true;
        }
        [DataMember]
        public string keyword { get; set; }

        [DataMember]
        public int cityCount { get; set; }

        [DataMember]
        public int hotelCount { get; set; }

        [DataMember]
        public int playCount { get; set; }

        [DataMember]
        public int foodCount { get; set; }

        [DataMember]
        public bool needHighlight { get; set; }
    }
}
