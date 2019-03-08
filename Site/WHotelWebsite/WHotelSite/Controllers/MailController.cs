using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HJDAPI.APIProxy;
using HJDAPI.Models;
using Newtonsoft.Json;
using WHotelSite.Params;
using WHotelSite.Common;
using WHotelSite.Params.Account;

namespace WHotelSite.Controllers
{
    public class MailController : BaseController
    {
        public const string AccessProtocal_IsApp = "whotelapp://www.zmjiudian.com/";
        public const string AccessProtocalPage_IsApp = "whotelapp://gotopage?url=http://www.zmjiudian.com/";

        public const string AccessProtocal_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/";  //"http://www.zmjiudian.com/hotel/";  //"http://m1.zmjiudian.com/";
        public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/";
        //public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://192.168.1.22:8081/";

        //取消订阅
        public ActionResult EUserManager(int state = 2, long idx = 0)
        {
            if (idx == 0 || state < 0)
            {
                return Redirect("/");
            }

            //if (state != 1 && state != 2)
            //{
		        
            //}

            var update = new Mail().UpdateEdmUsersState(idx, state);    //1;// 

            ViewBag.Update = update;

            return View();
        }
    }
}