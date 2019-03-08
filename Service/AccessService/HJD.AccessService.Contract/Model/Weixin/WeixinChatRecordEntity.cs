using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model
{
    [Serializable]
    [DataContract]
    public class WeixinChatRecordEntity
    {
        [DataMemberAttribute()]
        public string worker
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public string openid
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public string opercode
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public int time
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public string text
        {
            get;
            set;
        }
    }
}
