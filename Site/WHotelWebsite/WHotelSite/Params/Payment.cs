using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Routing;

namespace WHotelSite.Params.Payment
{
    public class GeneralParam : ParamBase
    {
        public long OrderId;
        public string Channel;
        public long CID;

        public GeneralParam(Controller controller)
            : base(controller)
        {
            OrderId = GetLong("order", 0); 
            CID = GetLong("cid", 0);
            Channel = GetString("channel", "");
        }
    }
}