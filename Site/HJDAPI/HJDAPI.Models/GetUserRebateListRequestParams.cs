using HJD.HotelPrice.Contract.DataContract.Order;
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
    public class GetUserRebateListRequestParams:BaseParam
    {
        [DataMember]
        public long userid { get; set; }
    }
}
