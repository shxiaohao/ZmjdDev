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
    public class SearchParamsForCity
    {
        public SearchParamsForCity()
        {
            cityCount = 10;   needHighlight = false; onlyInChina = true;
        }
        [DataMember]
        public string keyword { get; set; }

        [DataMember]
        public int cityCount { get; set; }


        [DataMember]
        public bool onlyInChina { get; set; }


        [DataMember]
        public bool needHighlight { get; set; }
    }
}
