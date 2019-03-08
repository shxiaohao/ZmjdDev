using System;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    /// <summary>
    /// 图片数据
    /// </summary>
    [Serializable]
    [DataContract]
    public class PictureItem
    {
        /// <summary>
        /// 图片url
        /// </summary>
        [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// 图片描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string OrgUrl { get; set; }

        [DataMember]
        public string Nickname { get; set; }

        [DataMember]
        public string PhotoTime { get; set; }
    }
}
