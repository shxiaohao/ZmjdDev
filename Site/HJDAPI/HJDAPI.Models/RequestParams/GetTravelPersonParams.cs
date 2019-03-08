using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models.RequestParams
{
    [Serializable]
    [DataContract]
    public class GetTravelPersonParams : BaseParam
    {
        [DataMember]
        public TravelPersonEntity travelPerson { get; set; }

    }

}
