﻿using HJD.HotelServices.Contracts;
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
    public class BecomVIPTip : CommentTextAndUrl
    {
        [DataMember]
        public string TipTitle { get; set; }
    }
}
