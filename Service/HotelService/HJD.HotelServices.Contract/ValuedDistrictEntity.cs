using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    public class ValuedDistrictEntity
    {
        [DataMember]
        [DBColumn]
        public int DistrictId { get; set; }
        [DataMember]
        [DBColumn]
        public int MostValuedHotelId { get; set; }
        [DataMember]
        [DBColumn]
        public int ValuedHotelCount { get; set; }
        [DataMember]
        [DBColumn]
        public string DistrictName { get; set; }
    }
}
