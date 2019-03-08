using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class OTACodeEntity
    {
        [DataMemberAttribute()]
        public int OTAHotelID { get; set; }

        [DataMemberAttribute()]
        public int ChannelID { get; set; }

        [DataMemberAttribute()]
        public string OTAHotelCode { get; set; }

        [DataMemberAttribute()]
        public string OTACity { get; set; }
    }
}
