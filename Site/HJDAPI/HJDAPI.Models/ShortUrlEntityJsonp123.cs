using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    public class Url
    {

        [JsonProperty("object_type")]
        public string ObjectType { get; set; }

        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("url_short")]
        public string UrlShort { get; set; }

        [JsonProperty("object_id")]
        public string ObjectId { get; set; }

        [JsonProperty("url_long")]
        public string UrlLong { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }
    }

    public class ShortUrlEntityJsonp123
    {

        [JsonProperty("urls")]
        public Url[] Urls { get; set; }
    }
}