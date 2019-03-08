﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class GetOrRegistPhoneUserResponse : BaseResponse
    {
        [DataMember]
        public long UserID { get; set; }
        [DataMember]
        public long CID { get; set; }
    }
}
