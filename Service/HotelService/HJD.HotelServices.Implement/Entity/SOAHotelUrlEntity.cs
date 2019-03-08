using System;
using HJD.Framework.Entity;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace HJD.HotelServices.Entity
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class SOAHotelUrlEntity
    {
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int Hotel { get; set; }
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "",IsOptional= true)]
        public string HotelReviewUrl { get; set; }
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string HotelUrl { get; set; }
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string HotelWriteUrl { get; set; }
    }
}
