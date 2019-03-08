using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.IServices;
using HJD.HotelServices.Contracts;
using HJD.WeixinServices.Contract;
using HJD.WeixinServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Activity;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace HJDAPI.Controllers
{
    public class EMChatApiController : BaseApiController
    {

       
        [System.Web.Http.HttpPost]
        public string SendTxtMessageToUser(MagiCallTxtMsgEntity m)
        {
            return EMChatHelper.SendTxtMessageToUser(m);
        }
        [System.Web.Http.HttpPost]
        public string SendPicTxtMessageToUser(MagiCallTxtMsgEntity m)
        {
            return EMChatHelper.SendPicTxtMessageToUser(m);
        } 
        //[System.Web.Http.HttpGet]
        //public string GetChatLogs(long lastTimeStamp)
        //{
        //    return EMChatHelper.GetChatLogs(lastTimeStamp);
        //}

        [System.Web.Http.HttpGet]
        public string CheckChatLogs()
        {
            return EMChatHelper.CheckChatLogs();
        }

        [System.Web.Http.HttpGet]
        public string GetAccount(long userID)
        {
             EMChatHelper.AccountCreate(userID);

             return string.Format("{0}:{1}", EMChatHelper.GenUserID(userID), EMChatHelper.GenUserPWD(userID));
        }

        [System.Web.Http.HttpGet]
        public string AccountCreate(long userID)
        {
            return EMChatHelper.AccountCreate(userID);
        }

        [System.Web.Http.HttpGet]
        public string AccountUpdateNickName(long userID)
        {
            return EMChatHelper.AccountUpdateNickName(userID );
        } 
         
    }
}
