using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Params
{
    [Serializable]
    [DataContract]
    public class WeixinChatRecordParams
    {
        [DataMemberAttribute()]
        public int starttime
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public int endtime
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
        public int pagesize
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public int pageindex
        {
            get;
            set;
        }
    }
}
