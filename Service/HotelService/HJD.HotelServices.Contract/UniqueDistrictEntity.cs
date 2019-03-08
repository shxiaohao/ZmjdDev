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
    public class UniqueDistrictEntity
    {
        [DataMember]
        [DBColumn]
        public int DistrictId { get; set; }

        [DataMember]
        [DBColumn]
        public string DistrictName { get; set; }

        [DataMember]
        [DBColumn]
        public int FirstTagId { get; set; }

        [DataMember]
        [DBColumn]
        public string FirstTagName { get; set; }
        [DataMember]
        [DBColumn]
        public int FirstTagHotelNum { get; set; }
        [DataMember]
        [DBColumn]
        public int SecondTagId { get; set; }
        [DataMember]
        [DBColumn]
        public string SecondTagName { get; set; }
        [DataMember]
        [DBColumn]
        public int SecondTagHotelNum { get; set; }
        [DataMember]
        [DBColumn]
        public int ThirdTagId { get; set; }
        [DataMember]
        [DBColumn]
        public string ThirdTagName { get; set; }
        [DataMember]
        [DBColumn]
        public int ThirdTagHotelNum { get; set; }
        [DataMember]
        [DBColumn]
        public int ShowHotelId { get; set; }
    }
}
