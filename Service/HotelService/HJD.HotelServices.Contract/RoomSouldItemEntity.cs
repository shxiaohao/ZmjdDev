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
    public class RoomSouldItemEntity
    {
        [DataMember]
        [DBColumn(IsOptional = true)]
        public int RoomID { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public DateTime CheckIn { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public int NightCount { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public int RoomCount { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public int BigBed { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public int TwinBed { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public int RoomSupplierID { get; set; }
        
        [DataMember]
        [DBColumn(IsOptional = true)]
        public int RoomSupplierType { get; set; }
    }
}