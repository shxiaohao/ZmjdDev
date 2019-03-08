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
    public class OrderCacheParam
    { 

        /// <summary>
        /// 订单明细ID： exchangeCoupon.ID  orders.orderid 
        /// </summary>
        [DataMember]
        public List<long> DetailOrderIdList { get; set; }
         
       
    }
}
