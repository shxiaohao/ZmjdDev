using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using HJD.DestServices.Contract;
using HJD.HotelServices;
using HJD.HotelServices.Contracts;
using HJDAPI.APIProxy;
using HJDAPI.Models;

using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelPrice.Contract;
using Newtonsoft.Json;
using WHotelSite.Params;
using PersonalServices.Contract;
using WHotelSite.Common;
using HJD.AccountServices.Entity;
using WHotelSite.Params.Account;

namespace WHotelSite.Controllers
{
    public class AdController : BaseController
    {
        public const string AccessProtocal_IsApp = "whotelapp://www.zmjiudian.com/";
        public const string AccessProtocalPage_IsApp = "whotelapp://gotopage?url=http://www.zmjiudian.com/";

        public const string AccessProtocal_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/";  //"http://www.zmjiudian.com/hotel/";  //"http://m1.zmjiudian.com/";
        public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/";
        //public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://192.168.1.22:8081/";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="type">1:推荐朋友 2:VIP剩余2天提示 3:VIP剩余1天提示 4:VIP剩余0天提示</param>
        /// <returns></returns>
        public ActionResult AdBox(int userid = 0, int type = 1)
        {
            if (IsApp())
            {
                ViewBag.AccessProtocal = AccessProtocal_IsApp;
                ViewBag.AccessProtocalPage = AccessProtocalPage_IsApp;
            }
            else
            {
                ViewBag.AccessProtocal = AccessProtocal_UnApp;
                ViewBag.AccessProtocalPage = AccessProtocalPage_UnApp;
            }

            ViewBag.UserId = userid;
            ViewBag.Type = type;

            //当前系统环境（ios | android）
            ViewBag.AppType = AppType();

            //跳转url
            var jumpUrl = "whotelapp://www.zmjiudian.com/gotopage?url={0}";
            jumpUrl = string.Format(jumpUrl, HttpUtility.UrlEncode("http://www.zmjiudian.com/Fund/UserRecommend2?userid={userid}&realuserid=1"));
            ViewBag.JumpUrl = jumpUrl;

            return View();
        }
    }
}