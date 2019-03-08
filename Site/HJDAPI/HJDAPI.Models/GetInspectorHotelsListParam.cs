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
    public class GetInspectorHotelsListParam:BaseParam
    {
        [DataMember]
        public long userid { get; set; }

        [DataMember]
        public long identityCode { get; set; }

        [DataMember]
        public int start { get; set; }

        [DataMember]
        public int count { get; set; }
    }
}
