using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WHotelSite.Models
{
    public class WeixinAccessToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("openid")]
        public string Openid { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("unionid")]
        public string Unionid { get; set; }
    }
}