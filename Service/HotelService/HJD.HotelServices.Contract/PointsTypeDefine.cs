using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class PointsTypeDefineEntity
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int ID { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Code { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Type { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Title { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Description { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime CreateTime { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime StartTime { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime EndTime { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Amount { get; set; }
    }
}