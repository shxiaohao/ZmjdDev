using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Models
{
    public class MagiCallUserInfo
    {
        public long userID { get; set; }
        public string userName { get; set; }


        public string userPassword { get; set; }
        public string userHeadPhoto { get; set; }
        public string kefuHeadPhoto { get; set; }

    }
}