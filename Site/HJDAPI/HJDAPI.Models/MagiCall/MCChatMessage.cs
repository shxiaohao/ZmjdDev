using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public  partial class MCChatMessage
    {
        public  class Body
        {

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("msg")]
            public string Msg { get; set; }
        }
    }

    public  partial class MCChatMessage
    {
        public  class Weichat2
        {

            [JsonProperty("originType")]
            public string OriginType { get; set; }
        }
    }

    public  partial class MCChatMessage
    {
        public  class Ext2
        {

            [JsonProperty("weichat")]
            public Weichat2 Weichat { get; set; }
        }
    }

    public  partial class MCChatMessage
    {
        public  class Payload2
        {

            [JsonProperty("bodies")]
            public Body[] Bodies { get; set; }

            [JsonProperty("ext")]
            public Ext2 Ext { get; set; }
        }
    }

    public  partial class MCChatMessage
    {
        public  class MsgEntity
        {

            [JsonProperty("uuid")]
            public string Uuid { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("created")]
            public long Created { get; set; }

            [JsonProperty("modified")]
            public long Modified { get; set; }

            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("from")]
            public string From { get; set; }

            [JsonProperty("msg_id")]
            public string MsgId { get; set; }

            [JsonProperty("to")]
            public string To { get; set; }

            [JsonProperty("chat_type")]
            public string ChatType { get; set; }

            [JsonProperty("payload")]
            public Payload2 Payload { get; set; }
        }
    }

    public  partial class MCChatMessage
    {

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("entities")]
        public MsgEntity[] Entities { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("cursor")]
        public string Cursor { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
