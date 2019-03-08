using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Dialog
{
    [Serializable]
    [DataContract]
    public class MessageEntity
    {
        [DataMemberAttribute()]
        public long UserID{get;set;}

        [DataMemberAttribute()]
        public String SourceType { get; set; }

        [DataMemberAttribute()]
        public String Service_Session_ID { get; set; }

        [DataMemberAttribute()]
        public String FromName { get; set; }

        [DataMemberAttribute()]
        public MessageType messageType{get;set;}

        [DataMemberAttribute()]
        public string msg{get;set;}

        [DataMemberAttribute()]
        public DateTime CreateTime { get; set; }

    }
}
