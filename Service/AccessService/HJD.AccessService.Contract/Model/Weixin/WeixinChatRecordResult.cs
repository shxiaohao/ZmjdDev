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
    public class WeixinChatRecordResult
    {
        [DataMemberAttribute()]
        public List<WeixinChatRecordEntity> recordlist
        {
            get;
            set;
        }
    }
}
