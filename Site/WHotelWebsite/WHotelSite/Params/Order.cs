using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WHotelSite.Params.Order
{
    public class OrderParam : ParamBase
    {
        public int Start;
        public int Count;//equals pagesize not totalCount
        public long UserID;
        public long OrderID;
        public bool IsUncomment;
        public int ShowTit;
        public string key;


        const int PageSize = 15;

        public OrderParam(Controller controller) : base(controller)
        {
            Start = GetIntFromQuery("start", 0);
            Count = PageSize;
            OrderID = (long)GetLong("order", 0);
            if (OrderID == 0 && controller.Request.Params["order"] != null)
            {
                OrderID = long.Parse(controller.Request.Params["order"].ToString());
            }
            UserID = UserState.IsLogin ? UserState.UserID : 0;//保留

            if (controller.Request.Params["isuncomment"] != null)
            {
                bool.TryParse(controller.Request.Params["isuncomment"].ToString(), out IsUncomment);
            }
            else
            {
                IsUncomment = true;
            }

            ShowTit = 0;
            if (controller.Request.Params["showtit"] != null)
            {
                ShowTit = Convert.ToInt32(controller.Request.Params["showtit"]);
            }
  
            key = "";
            if (controller.Request.Params["key"] != null)
            {
                key = controller.Request.Params["key"].ToString();
            }

        }
    }
}