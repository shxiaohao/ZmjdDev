using HJD.Framework.WCF;
using HJD.HotelManagementCenter.IServices;
using HJD.HotelServices.Contracts;
using HJD.WeixinService.Contract;
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
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace HJDAPI.Controllers
{
    public class WeixinServiceAccountController : WeixinBaseController
    {
        public WeixinServiceAccountController()
        {
            base.ChannelCode = WeiXinChannelCode.周末酒店服务号;
            base.logFile = Configs.LogPath + string.Format("WeixinServiceAccountLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe";

        }

        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity();
            return WeiXinAdapter.weixinActivityList.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();
        }
 
        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
           return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }

    }
}
