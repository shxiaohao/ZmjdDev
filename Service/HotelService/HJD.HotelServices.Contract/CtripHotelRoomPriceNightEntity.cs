using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class CtripHotelRoomPriceNightEntity
    {
        [DataMember]
        public long ID { get; set; }

        [DataMember]
        public DateTime Night { get; set; }
    }
}
