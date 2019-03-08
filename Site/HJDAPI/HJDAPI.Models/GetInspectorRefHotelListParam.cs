using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class GetInspectorRefHotelListParam:BaseParam
    {
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public long userid { get; set; }
    }
}
