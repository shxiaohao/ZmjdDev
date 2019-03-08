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
    public class ResultEntity
    {
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string errcode { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string errmsg { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string media_id { get; set; }

    }
}
