using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    public enum CashCouponState : sbyte
    {
        Create=0,
        Acquired=1,
        Expired = 2,
        Cancel = 3
    }
}