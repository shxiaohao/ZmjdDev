using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [DataContract]
    [Serializable]
    public class HotelPackageRoomBedCountEntity
    {
        [DataMember]
        [DBColumn(IsOptional = true)]
        public int RoomID { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public DateTime  Date { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public int RCount { get; set; }  
        
        [DataMember]
        [DBColumn(IsOptional = true)]
        public int BigBed { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public int TwinBed { get; set; }
    }
}
