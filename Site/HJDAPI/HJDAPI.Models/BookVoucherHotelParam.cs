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
    public class BookVoucherHotelParam : BaseParam
    {
        /// <summary>
        /// 房券Id
        /// </summary>
        [DataMember]
        public int HVID { get; set; }

        /// <summary>
        /// 房券编号id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 邮寄地址
        /// </summary>
        public string ShippingAddress { get; set; }

    }
}
