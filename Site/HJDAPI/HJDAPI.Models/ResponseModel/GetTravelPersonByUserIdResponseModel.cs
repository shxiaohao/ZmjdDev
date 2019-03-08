using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models.ResponseModel
{
    /// <summary>
    /// 操作结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class GetTravelPersonByUserIdResponseModel : BasePostResult
    {
      
        [DataMember]
        public List<TravelPersonEntity> TravelPersonList { get; set; }
        [DataMember]
        public List<CardEntity> CardTypeList { get; set; }
        //public Dictionary<int, string> CardTypeList { get; set; }
    }

    public class CardEntity
    {
        [DataMember]
        public String cardTypeID { get; set; }
        [DataMember]
        public String cardTypeDes { get; set; }
    }
}
