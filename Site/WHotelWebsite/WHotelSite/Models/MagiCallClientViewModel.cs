using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WHotelSite.Common;

namespace WHotelSite.Models
{
    public class MagiCallClientViewModel
    {
        public MagiCallClientViewModel() { DesignatedCustomerCare = ""; userInfo = new UserInfoResult(); }
        public long userID { get; set; }
        public string userName { get; set; }

        public UserInfoResult userInfo { get; set; } 

        public string userPassword { get; set; }
        public string userHeadPhoto { get; set; }
        public string kefuHeadPhoto { get; set; }

        public AccountHelper.UserRoleEnum UserRole { get; set; }
         
        public string DesignatedCustomerCare { get; set; }

        public string DesignatedCustomerCareName { get; set; }
        public string Greeting { get; set; }

        public bool isAPP { get; set; }
        


    }
}