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
    /// <summary>
    /// 订单ID与订单详情关系
    /// </summary>
    public class OrderIDDetailOrderIDRelEntity
    {
        /// <summary>
        /// 订单ID： coupunOrderID or Orders.OrderID
        /// </summary>
        [DataMember]
        public long OrderID { get; set; }

        /// <summary>
        /// 明细订单ID： ExchangeCoupon.ID or  Orders.OrderID
        /// </summary>
        [DataMember]
        public string DetailOrderIDList { get; set; }

    }
}
