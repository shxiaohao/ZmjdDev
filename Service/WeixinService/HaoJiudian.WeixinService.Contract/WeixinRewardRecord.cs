using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;
using Newtonsoft.Json;

namespace HJD.WeixinServices.Contracts
{
    /// <summary>
    /// 微信红包奖励记录实体
    /// </summary>
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class WeixinRewardRecord
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        [JsonProperty("id")]
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        [JsonProperty("sourceId")]
        /// <summary>
        /// 
        /// </summary>
        public Int64 SourceId { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        [JsonProperty("sourceType")]
        /// <summary>
        /// 
        /// </summary>
        public int SourceType { get; set; }

        [JsonProperty("reOpenid")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string ReOpenid { get; set; }

        [JsonProperty("reWxUid")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ReWxUid { get; set; }

        [JsonProperty("wishing")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Wishing { get; set; }



        [JsonProperty("amount")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int Amount { get; set; }

        [JsonProperty("number")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int Number { get; set; }

        [JsonProperty("activeId")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ActiveId { get; set; }

        [JsonProperty("activeName")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string ActiveName { get; set; }

        [JsonProperty("remark")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        [JsonProperty("sceneId")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int SceneId { get; set; }

        [JsonProperty("sendName")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string SendName { get; set; }

        [JsonProperty("willSendTime")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public DateTime WillSendTime { get; set; }

        [JsonProperty("realSendTime")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public DateTime RealSendTime { get; set; }

        [JsonProperty("state")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int State { get; set; }
    }
}
