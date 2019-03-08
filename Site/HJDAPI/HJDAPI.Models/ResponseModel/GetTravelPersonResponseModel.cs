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
    public class GetTravelPersonResponseModel : BasePostResult
    {

        [DataMember]
        public string CardTypeName { get; set; }
        //[DataMember]
        //public BaseResponse UpdateTravelPerson { get; set; }
    }
}
