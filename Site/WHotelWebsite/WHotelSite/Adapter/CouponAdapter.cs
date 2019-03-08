using HJDAPI.APIProxy;
using HJDAPI.Common.Security;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Adapter
{
    public class CouponAdapter
    {

        /// <summary>
        /// 赠送现金券
        /// </summary>
        /// <param name="param">userID 、 CashCouponType typeID、 phoneNo </param>
        /// <returns></returns>
        public static  BaseResponse PresentCashCoupon(PresentCashCouponParam param){
            Signature.SignBaseParam(param, "PresentCashCoupon", param.userID);
            return Coupon.PresentCashCoupon(param);
        }
    }
}