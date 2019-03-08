using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class ActivityParam : BaseParam
    {
        [DataMember]
        public long userID { get; set; }
        [DataMember]
        public int activityID { get; set; }
        /// <summary>
        /// 1微信好友 2朋友圈 3短信
        /// </summary>
        [DataMember]
        public int mediaType { get; set; }
        [DataMember]
        public string activityUrlType { get; set; }
        [DataMember]
        public float lat { get; set; }
        [DataMember]
        public float lon { get; set; }
    }
}
