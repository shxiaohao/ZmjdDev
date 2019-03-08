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
    public class SearchParamsForCoupon
    {
        public SearchParamsForCoupon()
        {
            count = 10;   needHighlight = false;  
        }
        [DataMember]
        public string keyword { get; set; }

        [DataMember]
        public int count { get; set; }


        [DataMember]
        public bool needHighlight { get; set; }


        [DataMember]
        public bool OnlyDistributable { get; set; }
    }
}
