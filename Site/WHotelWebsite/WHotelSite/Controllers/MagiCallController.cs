
using HJDAPI.APIProxy;
using HJDAPI.Models;
using MagiCallService.Contracts.Model.Dialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHotelSite.Common;
using WHotelSite.Models;

namespace WHotelSite.Controllers
{
    public class MagiCallController :   BaseController
    {
        //
        // GET: /MagiCall/

       


        public ActionResult MagiCallClient(long userID, string checkIn = "", string checkOut = "", string PCode = "", int price = 0, int VIPPrice = 0, int hotelId = 0, int payType = 0, int cancelPolicy = 0)
        {
            
            new EMChat().AccountCreate(userID);
            MagiCallClientViewModel m = new MagiCallClientViewModel();

            List<HJD.AccountServices.Entity.UserRole> roleList = account.GetUserRoleRelByUserId(new HJD.AccountServices.Entity.UserRole { UserID = userID });
         
            if (userID > 0)
               m.userInfo = account.GetUserInfoByUserID(userID);

            m.UserRole = AccountHelper.UserRoleEnum.Normal;
            bool isVIPUser = false;

            foreach (var r in roleList)
            {
                switch (r.RoleID)
                {
                    case (int)AccountHelper.UserRoleEnum.VIP:
                    case (int)AccountHelper.UserRoleEnum.VIP199:
                    case (int)AccountHelper.UserRoleEnum.VIP599:
                        m.UserRole = (AccountHelper.UserRoleEnum)r.RoleID;
                        isVIPUser = true;
                        break;
                }
            }

            //if (isVIPUser)//如果是VIP用户，获取其指定的客服
            //{
            //    var ccInfo = new MagiCall().GetLastUserCustomerCareInfo(userID);
            //    if (ccInfo.IDX > 0)
            //    {
            //        CustomerCareEntity ucc = new MagiCall().GetCustomerCareByUserID(ccInfo.CustomerCareUserID);
            //         if (ucc.State == (int)CustomerCareState.online || ucc.State == (int)CustomerCareState.busy)
            //         {
            //             m.DesignatedCustomerCare = ccInfo.KeFuEmail;
            //             m.DesignatedCustomerCareName = ccInfo.Name;
            //         }
            //    }
            //    else
            //    {

            //    }
            //}

            if (hotelId > 0)
            {
                RequreMoreHotelPrice(userID, checkIn, checkOut, PCode, price, VIPPrice, hotelId, payType, cancelPolicy);
            }
            else
            {

                string greeting = new MagiCall().GetGreeting(userID, isVIPUser).Trim("\"".ToCharArray());


                if (greeting.Length > 0)
                {
                    new EMChat().SendTxtMessageToUser(new HJDAPI.Models.MagiCallTxtMsgEntity { from = "kevincai", userID = userID, msg = "WHAPP:html," + greeting });
                }
            }

            m.isAPP = base.IsApp();
            m.userID = userID;
            m.userName = Config.magicalPreUserName + userID.ToString();
            m.userPassword = Config.magicalPreUserPassword + userID.ToString();
            m.kefuHeadPhoto = System.Configuration.ConfigurationManager.AppSettings["kefuHeadPhoto"];
            m.userHeadPhoto =  account.GetCurrentUserInfo(userID).AvatarUrl.Replace("jupiter", System.Configuration.ConfigurationManager.AppSettings["AvatarSize"]);
            return View(m);
        }

        public void RequreMoreHotelPrice(long userID, string checkIn = "", string checkOut = "", string PCode = "", int price = 0, int VIPPrice = 0, int hotelId = 0, int payType = 0, int cancelPolicy = 0)
        {
            if (hotelId <= 0) return;

            var hotelItem = HotelController.GetHotel(hotelId);
            RequireHotelPriceFeedbackMessageEntity feedback = new RequireHotelPriceFeedbackMessageEntity
            {
                title = "",
                packageInfo = new HotelPackageInfoEntity
                {
                    cancelPolicy = cancelPolicy,
                    checkIn = DateTime.Parse(checkIn),
                    checkOut = DateTime.Parse(checkOut),
                    hotelId = hotelId,
                    payType = payType,
                    price = price,
                    VIPPrice = VIPPrice,
                    PCode = PCode,
                    hotelName = hotelItem.Name
                }
            };

            string msgJson = JsonConvert.SerializeObject(feedback);

            string finalMsgJson = msgJson.Replace("\"", "\\\"");//.Replace("{","｛").Replace("}","｝");           

            new EMChat().SendTxtMessageToUser(new MagiCallTxtMsgEntity { appType = 0, from = "kevincai", msg = "WHAPP:" + finalMsgJson, userID = userID });

            UserActionItem ua = new UserActionItem { ActionType = UserActionType.RequireHotelPrice, Text = msgJson };

            new MagiCall().MagiCallClientMessage(new MessageEntity
            {
                UserID = userID,
                messageType = MessageType.UserAction,
                msg = JsonConvert.SerializeObject(ua).Replace("\"", "\\\"")
            });


            RequireHotelPriceChoiceFeedbackMessageEntity rpFeedBack = new RequireHotelPriceChoiceFeedbackMessageEntity();
            rpFeedBack.title = "该酒店价格变动较快，你需要查询以上房型最新的价格吗？";
            rpFeedBack.items.Add(new ChoiceItem { idx = 1, content = "需要" });
            rpFeedBack.items.Add(new ChoiceItem { idx = 2, content = "不需要" });

            new EMChat().SendTxtMessageToUser(new MagiCallTxtMsgEntity { appType = 0, from = "kevincai", msg = "WHAPP:" + GenObjectJson(rpFeedBack), userID = userID });


        }

        public string GenObjectJson(object obj)
        {
            return  JsonConvert.SerializeObject(obj).Replace("\"", "\\\"");//.Replace("{","｛").Replace("}","｝");           
        }


        public ActionResult MagiCallClientTest(long userID)
        {

//            new EMChat().AccountCreate(userID);
            MagiCallClientViewModel m = new MagiCallClientViewModel();


            List<HJD.AccountServices.Entity.UserRole> roleList = account.GetUserRoleRelByUserId(new HJD.AccountServices.Entity.UserRole { UserID = userID });

             m.UserRole = AccountHelper.UserRoleEnum.Normal;
            bool isVIPUser=false;

            foreach (var r in roleList)
            {
                switch (r.RoleID)
                {
                    case (int)AccountHelper.UserRoleEnum.VIP:
                    case (int)AccountHelper.UserRoleEnum.VIP199:
                    case (int)AccountHelper.UserRoleEnum.VIP599:
                         m.UserRole = (AccountHelper.UserRoleEnum)r.RoleID;
                        isVIPUser= true;
                        break;
                }
            }

            if (isVIPUser)//如果是VIP用户，获取其指定的客服
            {
                var ccInfo = new MagiCall().GetLastUserCustomerCareInfo(userID);
                if (ccInfo.IDX > 0)
                {
                    m.DesignatedCustomerCare = ccInfo.KeFuEmail;
                    m.DesignatedCustomerCareName = ccInfo.Name;
                }
            }

            string greeting = new MagiCall().GetGreeting(userID, isVIPUser);
 
            m.userID = userID;
            m.userName = Config.magicalPreUserName  + userID.ToString();
            m.userPassword = Config.magicalPreUserPassword + userID.ToString();
            m.kefuHeadPhoto = System.Configuration.ConfigurationManager.AppSettings["kefuHeadPhoto"];
            m.userHeadPhoto = "http://p1.zmjiudian.com/4539575aNul.jpg_60X60";// account.GetCurrentUserInfo(userID).AvatarUrl.Replace("jupiter", System.Configuration.ConfigurationManager.AppSettings["AvatarSize"]);
            return View(m);
        }

    

        public ActionResult KFUserBind(string userCode, string channel = "weixinservice", string tag = "")
        {
            ViewBag.UserCode = userCode;
            ViewBag.Channel = channel;
            ViewBag.Tag = tag;

            string phone = "";
            string phoneCode = "";

            if (phone.Length > 0)
            {
                long userid = account.GetOrRegistPhoneUser(phone);
                var result = account.AddUserChannelRel(new HJD.AccountServices.Entity.UserChannelRelEntity { Channel = channel, Code = userCode, CreateTime = DateTime.Now, UserId = userid });
                if (result.Success)
                {

                }
            }

            return View();
        }

        public ActionResult SubmitBind(string userCode = "", string phone = "", string channel = "weixinservice", string tag = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(phone))
            {
                long userid = account.GetOrRegistPhoneUser(phone,GetCurCID());
                var result = account.AddUserChannelRel(new HJD.AccountServices.Entity.UserChannelRelEntity { Channel = channel, Code = userCode, CreateTime = DateTime.Now, UserId = userid });
                if (result.Success)
                {
                    dict["Message"] = "关联成功";
                    dict["Success"] = "1";
                    dict["Url"] = "";
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }

            dict["Message"] = "关联失败";
            dict["Success"] = "0";
            dict["Url"] = "";
            return Json(dict, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult MagiCallClientTest436(long userID)
        //{

        //    new EMChat().AccountCreate(userID);
        //    MagiCallClientViewModel m = new MagiCallClientViewModel();

        //    m.userID = userID;
        //    m.userName = Config.magicalPreUserName + userID.ToString();
        //    m.userPassword = Config.magicalPreUserPassword + userID.ToString();
        //    m.kefuHeadPhoto = System.Configuration.ConfigurationManager.AppSettings["kefuHeadPhoto"];
        //    m.userHeadPhoto = account.GetCurrentUserInfo(userID).AvatarUrl.Replace("jupiter", System.Configuration.ConfigurationManager.AppSettings["AvatarSize"]);
        //    return View(m);
        //}

        public ActionResult CustomerServiceHelper(int easemobId, string visitorImId)
        {
            long userID = long.Parse(visitorImId.Replace(Config.magicalPreUserName, ""));

            return View();
        }

        public ActionResult MagiCallClientTestVisitor(long userID)
        {
            return View();
        }
    }
}
