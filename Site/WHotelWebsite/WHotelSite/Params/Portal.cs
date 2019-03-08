using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Routing;

namespace WHotelSite.Params.Portal
{
    public class HomeParam : ParamBase
    {
        public int DistrictId;

        public HomeParam(Controller controller)
            : base(controller)
        {
            DistrictId = GetIntFromQuery("district", 2);
        }
    }
}