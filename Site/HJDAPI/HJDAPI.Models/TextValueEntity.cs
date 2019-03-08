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
    public class TextValueEntity
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string TextValue { get; set; }
    }
}
