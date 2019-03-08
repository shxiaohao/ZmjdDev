using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    [DataContract]
    public class ShortUrlEntity50r
    {
        [JsonProperty("error")]
        public object Error { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}