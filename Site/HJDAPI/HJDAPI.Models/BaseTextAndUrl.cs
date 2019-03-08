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
    public class BaseTextAndUrl
    {
        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
}
