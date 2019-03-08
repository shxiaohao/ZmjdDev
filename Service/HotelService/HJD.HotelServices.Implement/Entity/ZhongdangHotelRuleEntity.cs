using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Implement.Entity
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class ZhongdangHotelRuleEntity
    {
        [DataMemberAttribute()]
        public int ID { get; set; }

        [DataMemberAttribute()]
        public string Name { get; set; }


        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Should { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Must { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Mustnot { get; set; }
    }
}
