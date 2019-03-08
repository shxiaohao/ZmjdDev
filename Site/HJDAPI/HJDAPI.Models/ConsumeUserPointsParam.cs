using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class ConsumeUserPointsParam : BaseParam
    {
        [DataMember]
        public long userID { get; set; }

        [DataMember]
        public int requiredPoints { get; set; }

        [DataMember]
        public HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType typeID { get; set; }

        [DataMember]
        public long businessID { get; set; }

    }
}
