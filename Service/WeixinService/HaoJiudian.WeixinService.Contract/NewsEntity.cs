using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.WeixinServices.Contracts
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class NewsEntity
    {
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string title { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string thumb_media_id { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string author { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string digest { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public bool show_cover_pic { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string content { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string content_source_url { get; set; }

    }
}
