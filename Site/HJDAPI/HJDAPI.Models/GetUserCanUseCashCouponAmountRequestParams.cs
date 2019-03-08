using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    public class GetUserCanUseCashCouponAmountRequestParams: BaseParam
    {
        public GetUserCanUseCashCouponAmountRequestParams()
        {
            packagePayType = HJD.HotelServices.Contracts.HotelServiceEnums.PackagePayType.PrePay;
        }
        [DataMember]
        public long userID { get; set; }

        [DataMember]
        public  HJD.HotelServices.Contracts.HotelServiceEnums.PackagePayType packagePayType { get; set; } //套餐支付方式 
    }
}
