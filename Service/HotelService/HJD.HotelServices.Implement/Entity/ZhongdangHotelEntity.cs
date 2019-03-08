using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;

namespace HJD.HotelServices.Implement.Entity
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class ZhongdangHotelEntity
    {
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int HotelID { get; set; }

        [DataMemberAttribute()]
        public int TagId { get; set; }

        [DataMemberAttribute()]
        public string Name { get; set; }
    }
}
