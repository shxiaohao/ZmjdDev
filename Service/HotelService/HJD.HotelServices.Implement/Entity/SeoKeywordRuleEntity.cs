using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement.Entity
{
    public class SeoKeywordRuleEntity
    {
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public int ID { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string Name { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Must { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Url { get; set; }

    }
}
