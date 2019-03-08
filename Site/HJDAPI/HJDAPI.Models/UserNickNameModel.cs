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
    public class UserNickNameModel:BaseParam
    {
        [DataMember]
        public string nickName { get; set; }
        [DataMember]
        public long userID { get; set; }
        [DataMember]
        public string password { get; set; }
    }
}
