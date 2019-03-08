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
    public struct HotelTypeDefine
    {
         [DataMemberAttribute()]
        public int TypeID { get; set; }
         [DataMemberAttribute()]
        public string Name { get; set; }
         [DataMemberAttribute()]
        public Dictionary<int, string> SubHotelClass {get;set;}        
    }
}
