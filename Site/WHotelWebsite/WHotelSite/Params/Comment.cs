using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WHotelSite.Params
{
    public class CommentParam : ParamBase
    {
        public int Start;
        public int Count;//equals pagesize not totalCount
        public long UserID;
        public long CommentID;

        const int PageSize = 15;

        public CommentParam(Controller controller) : base(controller)
        {
            Start = GetIntFromQuery("start", 0);
            Count = PageSize;
            CommentID = (long)GetLong("comment", 0);
            if (CommentID == 0 && controller.Request.Params["comment"] != null)
            {
                CommentID = long.Parse(controller.Request.Params["comment"].ToString());
            }
            UserID = UserState.IsLogin ? UserState.UserID : 0;//保留
        }
    }
}