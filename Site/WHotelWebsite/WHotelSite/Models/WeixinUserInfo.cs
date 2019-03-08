using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WHotelSite.Models
{
    public class WeixinUserInfo
    {
        [JsonProperty("subscribe")]
        public string Subscribe { get; set; }

        [JsonProperty("openid")]
        public string Openid { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("sex")]
        public int Sex { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("headimgurl")]
        public string Headimgurl { get; set; }

        [JsonProperty("privilege")]
        public object[] Privilege { get; set; }

        [JsonProperty("unionid")]
        public string Unionid { get; set; }
    }
}