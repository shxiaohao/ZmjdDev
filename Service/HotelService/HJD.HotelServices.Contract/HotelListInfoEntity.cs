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
    [DataContract]
    [DefaultColumn]
    public class HotelListInfoEntity
    {
        [DataMember]
        public int _id { get; set; }

        [DataMember]
        public decimal AvgPrice { get; set; }
    }
}
