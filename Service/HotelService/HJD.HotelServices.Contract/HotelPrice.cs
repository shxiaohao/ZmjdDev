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

    public class HotelPrice
    {
        [DataMember]
        [DBColumn(IsOptional = true)]
        public int HotelId { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public int Price { get; set; }
    }
}
