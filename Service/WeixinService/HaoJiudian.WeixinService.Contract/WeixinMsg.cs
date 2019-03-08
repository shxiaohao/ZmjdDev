using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;
using HJD.WeixinService.Contract;
using Newtonsoft.Json;

namespace HJD.WeixinServices.Contracts
{
    #region 微信消息 Text

    public class WeixinMsgText
    {
        public string WeixinAcount { get; set; }

        [JsonProperty("touser")]
        public string Touser { get; set; }

        [JsonProperty("msgtype")]
        public string Msgtype { get; set; }

        [JsonProperty("text")]
        public MsgText Text { get; set; }
    }

    public class MsgText
    {

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    #endregion

    #region 微信消息 News

    public class WeixinMsgNews
    {
        public string WeixinAcount { get; set; }

        [JsonProperty("touser")]
        public string Touser { get; set; }

        [JsonProperty("msgtype")]
        public string Msgtype { get; set; }

        [JsonProperty("news")]
        public MsgNews News { get; set; }
    }

    public class MsgNews
    {

        [JsonProperty("articles")]
        public MsgNewsArticle[] Articles { get; set; }
    }

    public class MsgNewsArticle
    {

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("picurl")]
        public string Picurl { get; set; }
    }

    #endregion

    #region 微信模板消息发送对象（业务）

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class WeixinTemplateMessageEntity
    {
        /// <summary>
        /// 发送消息的微信公众号
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public WeiXinChannelCode WeixinAccount { get; set; }

        /// <summary>
        /// 接收者OPENID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string ToOpenId { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string TemplateId { get; set; }

        /// <summary>
        /// 网页跳转链接
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string TemplateUrl { get; set; }

        /// <summary>
        /// 小程序跳转链接
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string MiniProgram { get; set; }

        /// <summary>
        /// 消息内容：first
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string DataFirst { get; set; }

        /// <summary>
        /// 消息内容：remark
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string DataRemark { get; set; }

        /// <summary>
        /// 消息内容：keyword list
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public List<string> DataKeywords { get; set; }
    }

    #endregion
}
