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
//using HJDAPI.Common.Extensions;
//using HJDAPI.Common.Helpers;
//using HJDAPI.Controllers;
//using HJDAPI.Controllers.Adapter;
using HJDAPI.Models;

using HJD.HotelPrice.Contract.DataContract.Order;
using HJD.HotelPrice.Contract;
using Newtonsoft.Json;
using WHotelSite.Params;
using PersonalServices.Contract;
using WHotelSite.Common;
using HJD.AccountServices.Entity;
using HJD.HotelManagementCenter.Domain;
using WHotelSite.Params.Account;

namespace WHotelSite.Controllers
{
    public class InspectorController : BaseController
    {
        public const string AccessProtocal_IsApp = "whotelapp://www.zmjiudian.com/";
        public const string AccessProtocalPage_IsApp = "whotelapp://gotopage?url=http://www.zmjiudian.com/";

        public const string AccessProtocal_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/";  //"http://www.zmjiudian.com/hotel/";  //"http://m1.zmjiudian.com/";
        public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/";
        //public const string AccessProtocalPage_UnApp = "/inspector/jump?jumpurl=whotelapp://www.zmjiudian.com/gotopage?url=http://192.168.1.22:8081/";

        //品鉴师
        public ActionResult Explain(int userid = 0)
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

            InspectorParam param = new InspectorParam(this);
            param.UserId = userid;
            ViewBag.param = param;
            return View();
        }

        //品鉴师条件
        public ActionResult Rules(int userid = 0)
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

            InspectorParam param = new InspectorParam(this);
            param.UserId = userid;
            ViewBag.param = param;
            return View();
        }

        //品鉴师说明页
        public ActionResult RulesDoc(int userid = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;

            InspectorParam param = new InspectorParam(this);
            param.UserId = userid;
            ViewBag.param = param;
            return View();
        }

        //品鉴师注册
        public ActionResult Register(int userid = 0)
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

            ViewBag.IsApp = IsApp();

            InspectorRegisterParam param = new InspectorRegisterParam(this);
            param.UserId = userid;

            OperationResult res = new OperationResult { Mobile = "" };
            if (userid > 0)
            {
                MemberProfileInfo minfo = account.GetCurrentUserInfo((long)userid);
                if (minfo != null) res = new OperationResult { Mobile = minfo.MobileAccount };
            }
            //res = new OperationResult { Mobile = "15001966513" };
            param.RegisterResult = res;
            ViewBag.param = param;
            return View();
        }

        //报名成功
        public ActionResult RegisterCompleted(int userid = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;

            //检查当前版本是否大于等于4.0版本
            ViewBag.VersionIsOk = IsThanVer4_0();

            ViewBag.AppType = AppType();

            InspectorParam param = new InspectorParam(this);
            param.UserId = userid;
            ViewBag.param = param;
            return View();
        }

        public ActionResult GetUserTel(string user = "0")
        {
            ResultEntity re = new ResultEntity();

            OperationResult res = new OperationResult { Mobile = "" };
            var userid = Convert.ToInt32(user);
            if (userid > 0)
            {
                MemberProfileInfo minfo = account.GetCurrentUserInfo((long)userid);
                if (minfo != null) res = new OperationResult { Mobile = minfo.MobileAccount };
            }

            return Json(res.Mobile, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Verify()
        {
            VerifyParam param = new VerifyParam(this);
            //string number = Request.Form["number"];
            //string code = Request.Form["code"];
            var result = new Dictionary<string, bool>();
            if (param.Action == "send")
            {
                result["ok"] = true;// account.sendConfirmSMS(param.Number);
            }
            else if (param.Action == "check")
            {
                result["ok"] = true; //account.checkConfirmSMS(param.Number, param.Code);
            }
            return Json(result);
        }

        //To Register 
        public ActionResult RegisterInspector(string name, string tell, string mail, string mineJob)
        {
            InspectorApplyData insData = new InspectorApplyData
            {
                UserID = 0,
                TrueName = name,
                MobilePhone = tell,
                Job = mineJob,
                JobExperience = "",
                JobSpecialty = ""
            };

            ResultEntity re = new HJDAPI.APIProxy.Inspector().SubmitInspectorApplyData(insData);
            return Json(re, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckInspector(string userid, string name, string tell, string mail)
        {
            IsLatestVerApp();

            InspectorApplyData insData = new InspectorApplyData
            {
                UserID = (!string.IsNullOrEmpty(userid) ? Convert.ToInt64(userid) : 0),
                TrueName = name,
                MobilePhone = tell,
                Job = "",
                JobExperience = "",
                JobSpecialty = ""
            };

            ResultEntity re = new ResultEntity();
            re.Success = new HJDAPI.APIProxy.Inspector().IsUserHasApplyInspector(insData);
            re.Message = "";

            return Json(re, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 招募品鉴师
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult RecruitInspector(int userid = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;

            //检查当前版本是否大于等于4.0版本
            ViewBag.VersionIsOk = IsThanVer4_0();

            ViewBag.AppType = AppType();

            InspectorParam param = new InspectorParam(this);
            param.UserId = userid;
            ViewBag.param = param;
            return View();
        }

        public ActionResult RecruitInspectorForMail()
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;

            //检查当前版本是否大于等于4.0版本
            ViewBag.VersionIsOk = IsThanVer4_0();

            ViewBag.AppType = AppType();

            InspectorParam param = new InspectorParam(this);
            ViewBag.param = param;
            return View();
        }

        public ActionResult HotelList(int userid = 0)
        {
            var isApp = IsApp();
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
            ViewBag.IsApp = isApp;

            InspectorHotelParam param = new InspectorHotelParam(this);
            ViewBag.param = param;
            param.UserId = userid;

            GetInspectorHotelsListParam hlp = new GetInspectorHotelsListParam { };
            hlp.userid = userid;//4512003//4512004
            hlp.start = 1;
            hlp.count = 100;

            //InspectorHotelListResult hotelList = new HJDAPI.APIProxy.Inspector().GetInspectorHotelsList(hlp);
            List<HotelVoucherAndInspectorHotel> hotelList = new HJDAPI.APIProxy.Inspector().GetHotelVoucherList(hlp);
            ViewBag.HotelList = hotelList;

            return View();
        }
        
        public ActionResult Hotel()
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

            InspectorHotelParam param = new InspectorHotelParam(this);
            var getparam = new GetInspectorHotelByIdParam() { id = param.Id };
            HJD.HotelServices.Contracts.HotelVoucherEntity inshotel = new HJDAPI.APIProxy.Inspector().GetHotelVoucherById(getparam);

            //生成日历
            var calendar = new List<HJD.HotelServices.Contracts.PDayItem>();

            //后台设置的日历
            if (inshotel.PDayItem != null && inshotel.PDayItem.Count > 0)
            {
                calendar = inshotel.PDayItem;
            }

            #region 新的房券方式的日历生成

            //var startDate = (inshotel.StartDate >= DateTime.Now.Date ? inshotel.StartDate : DateTime.Now.Date);
            //var endDate = (inshotel.EndDate > startDate ? inshotel.EndDate : startDate.AddDays(60));
            //while (startDate < endDate)
            //{
            //    calendar.Add(new HJD.HotelServices.Contracts.PDayItem
            //    {
            //        Day = startDate,
            //        MaxSealCount = 1,
            //        SellState = 1
            //    });

            //    startDate = startDate.AddDays(1);
            //}

            ////filter calendar
            //calendar = FilterCalendar(calendar, false, false);

            #endregion

            //兑换日必须是提交日的14天之后
            var minDate = DateTime.Now.Date.AddDays(14);
            calendar = calendar.Where(_ => _.Day > minDate).ToList();

            ViewBag.calendar = calendar;

            ViewBag.Hotel = inshotel;
            ViewBag.BS = param.BS;
            ViewBag.param = param;
            return View();
        }

        private List<HJD.HotelServices.Contracts.PDayItem> FilterCalendar(List<HJD.HotelServices.Contracts.PDayItem> calendar, bool canHoliday, bool canWeekend)
        {
            var holidays = new List<DateTime>();
            var holidays20 = new Hotel().GetHolidays();

            var currentYear = (DateTime.Now.Year == 2016 ? 2017 : DateTime.Now.Year);
            if (holidays20 != null && holidays20.Exists(_ => _.Day.Year >= currentYear))
            {
                var needHolidays = holidays20.FindAll(_ => _.Day.Year >= currentYear);
                if (needHolidays.Any())
                {
                    holidays = needHolidays.Select(_ => _.Day).ToList();
                }
            }
            else
            {
                holidays = new List<DateTime>()
                {
                    new DateTime(2017, 1, 1),
                    new DateTime(2017, 1, 2),
                    new DateTime(2017, 1, 27),
                    new DateTime(2017, 1, 28),
                    new DateTime(2017, 1, 29),
                    new DateTime(2017, 1, 30),
                    new DateTime(2017, 1, 31),
                    new DateTime(2017, 2, 1),
                    new DateTime(2017, 2, 2),
                    new DateTime(2017, 2, 3),
                    new DateTime(2017, 2, 4),
                    new DateTime(2017, 4, 2),
                    new DateTime(2017, 4, 3),
                    new DateTime(2017, 4, 4),
                    new DateTime(2017, 4, 5),
                    new DateTime(2017, 5, 1),
                    new DateTime(2017, 5, 2),
                    new DateTime(2017, 5, 3),
                    new DateTime(2017, 5, 29),
                    new DateTime(2017, 9, 30)
                }; //节假日前一天才可以
            }

            foreach (var item in calendar)
            {
                if (!canHoliday)
                {
                    if (item.SellState == 1 && holidays.Exists(_ => _ == item.Day.Date))
                    {
                        item.SellState = 0;
                    }
                }
                if (!canWeekend)
                {
                    var dayOfWeek = (int)item.Day.Date.DayOfWeek;
                    if (item.SellState == 1 && (dayOfWeek == 5 || dayOfWeek == 6))
                    {
                        item.SellState = 0;
                    }
                }
            }

            return calendar;
        }

        //打开APP的中转页面
        public ActionResult Jump(string jumpUrl)
        {
            InspectorJumpParam param = new InspectorJumpParam(this);
            param.JumpUrl = (!string.IsNullOrEmpty(jumpUrl) ? jumpUrl : "whotelapp://www.zmjiudian.com/");
            ViewBag.param = param;

            string agent = HttpContext.Request.UserAgent;
            string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };
            // agent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_1_2 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Mobile/12B440 MicroMessenger/6.1 NetType/WIFI";
            ViewBag.IsInWeixin = agent.IndexOf("MicroMessenger") > 0;

            return View();
        }

        /// <summary>
        /// 提交申请品鉴酒店
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hotelid"></param>
        /// <param name="userid"></param>
        /// <param name="checkin"></param>
        /// <param name="checkout"></param>
        /// <param name="nightCount"></param>
        /// <param name="bs"></param>
        /// <returns></returns>
        public ActionResult SubmitInspectorHotel(int id, int hotelid, int userid, string checkin, string checkout, int nightCount = 1, int bs = 1)
        {
            ResultEntity re = new ResultEntity();
            BookInspectorHotelParam bParam = new BookInspectorHotelParam();
            bParam.HVID = id;
            bParam.hotelid = hotelid;
            bParam.userid = userid;
            bParam.checkin = checkin;
            bParam.checkout = checkout;

            //验证所选天数等于房券设置入住天数
            try
            {
                var cin = DateTime.Parse(checkin);
                var cot = DateTime.Parse(checkout);
                var days = (cot - cin).Days;
                if (days != nightCount)
                {
                    re = new ResultEntity { Success = -1, Message = string.Format("您只能选择{0}晚入住", nightCount) };
                    return Json(re, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                
            }

            re = new HJDAPI.APIProxy.Inspector().BookVoucherHotel(bParam);

            return Json(re, JsonRequestBehavior.AllowGet);
        }

        //申请品鉴
        public ActionResult ApplyInsHotel(int hotelid, string userid)
        {
            Dictionary<string, string> back = new Dictionary<string, string>();

            //当前品鉴酒店基本信息
            var getparam = new GetInspectorHotelByIdParam() { id = hotelid };

            //验证当前用户的积分
            var requiredPoint = 0;

            ////获取房券信息
            //var inshotel = new HJDAPI.APIProxy.Inspector().GetHotelVoucherById(getparam);
            ////requiredPoint = inshotel.

            ////判断当前用户的积分是否满足品鉴该酒店
            //getparam = new GetInspectorHotelByIdParam() { userid = Convert.ToInt64(userid) };
            //var userPoint = new HJDAPI.APIProxy.Inspector().GetAvailablePointByUserID(getparam);
            //if (userPoint < requiredPoint)
            //{
            //    back["Message"] = "您的积分不足，请重新选择酒店";
            //    back["Success"] = "4";
            //    return Json(back, JsonRequestBehavior.AllowGet);
            //}

            //符合资格，一般会直接跳转至选择入住酒店页
            back["Message"] = "您符合资格";
            back["Success"] = "0";
            return Json(back, JsonRequestBehavior.AllowGet);
        }
    }
}