using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HJDAPI.Models
{
      [DataContract]
        public class OTAInfo
        {
            [DataMember]
            public int OTAHotelID { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string EName { get; set; }
            [DataMember]
            public int ChannelID { get; set; }
            [DataMember]
            public string AccessURL { get; set; }
            [DataMember]
            public Decimal Price { get; set; }
            [DataMember]
            public bool IsInnerOpen { get; set; }
            [DataMember]
            public bool CanSyncPrice { get; set; }
            [DataMember]
            public string PriceName { get; set; }
            [DataMember]
            public string PriceBrief { get; set; }

    }
}
