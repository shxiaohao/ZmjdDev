using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Dialog
{


    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Size
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int height { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class BodiesItem
    {
        /// <summary>
        /// 座席回复的消息
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string msg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string filename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Size size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string secret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string url { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Visitor
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string mp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string openid { get; set; }

        /// <summary>
        /// 微信粉丝昵称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string userNickname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string source { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Weichat
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Visitor visitor { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Ext
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Weichat weichat { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Payload
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public List<BodiesItem> bodies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Ext ext { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class EasemobEventsEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long timestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string chat_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string from { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string to { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Payload payload { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string msg_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string callId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string eventType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string security { get; set; }

    }
}
