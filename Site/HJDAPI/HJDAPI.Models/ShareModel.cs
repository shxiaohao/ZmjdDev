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
    public class ShareModel
    {
        [DataMember]
        public string notHotelNameTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string photoUrl { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string shareLink { get; set; }
        /// <summary>
        /// 分享成功后的回调页面
        /// </summary>
        [DataMember]
        public string returnUrl { get; set; }

        [DataMember]
        public string returnApiUrl { get; set; }
    }
}
