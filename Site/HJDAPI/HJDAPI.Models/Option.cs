using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    [DataContract]
    public class Option
    {
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string Text { get; set; }
    }
}