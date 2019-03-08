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
    public sealed class MagiCallTxtMsgEntity
    {
        [DataMember]
        public long userID { get; set; }

        [DataMember]
        public string from { get; set; }

        [DataMember]
        public string msg { get; set; }

        /// <summary>
        /// 1:iOS 2:android
        /// </summary>
        [DataMember]
        public int appType { get; set; }
    }
}