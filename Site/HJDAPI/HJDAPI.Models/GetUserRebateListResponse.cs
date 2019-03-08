using HJD.HotelPrice.Contract.DataContract.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
     [DataContract]
    public class GetUserRebateListResponse: BaseResponse
    {
          [DataMember]
       public  List<UserRebateInfoEntity> RebateList { get; set; }
          [DataMember]
       public int WaitingRebateAmount { get; set; }
          [DataMember]
       public int HadRebateAmount { get; set; }
    }
}
