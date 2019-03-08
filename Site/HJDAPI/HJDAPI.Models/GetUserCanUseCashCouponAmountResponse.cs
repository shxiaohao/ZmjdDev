using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class GetUserCanUseCashCouponAmountResponse: BaseResponse
    {
        /// <summary>
        /// 可用现金券余额
        /// </summary>
        [DataMember]
        public int UserCanUseCashCouponAmount { get; set; }

        /// <summary>
        /// 可用住基金余额
        /// </summary>
        [DataMember]
        public int UserCanUseHousingFundAmount { get; set; }
    }
}
