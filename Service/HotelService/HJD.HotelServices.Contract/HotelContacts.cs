using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    [DataContract]
    [Serializable]

    public class HotelContacts
    {
        [DataMember]
        [DBColumn(IsOptional = true)]
        public int HotelId { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public int OpenState { get; set; }
        
        [DataMember]
        [DBColumn(IsOptional = true)]
        public int PriceChannelID { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public int SettleRate { get; set; }

    }
}
