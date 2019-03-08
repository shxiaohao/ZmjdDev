using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    public class BuyMembershipSuggest
    {

        public BuyMembershipSuggest()
        {
            ActionUrl = "";
            Text = "";
            Description = "";
        }

        /// <summary>
        /// 跳转链接
        /// </summary>
        [DataMember]
        public string ActionUrl { get; set; }

        /// <summary>
        /// 建议购买会员的内容
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CommentTextAndUrl
    {

        public CommentTextAndUrl()
        {
            ActionUrl = "";
            Text = "";
            Description = "";
            //Item = new List<string>();
        }

        /// <summary>
        /// 跳转链接
        /// </summary>
        [DataMember]
        public string ActionUrl { get; set; }

        /// <summary>
        /// 文字描述
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        //[DataMember]
        //public List<string> Item { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CommentTextAndUrlEx :CommentTextAndUrl
    {

        public CommentTextAndUrlEx()
        {
            Item = new List<string>();
        }

        [DataMember]
        public List<string> Item { get; set; }
    }
}
