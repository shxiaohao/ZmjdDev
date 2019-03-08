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
    public class WeixinController : WeixinBaseController
    {

        public WeixinController()
        {
            base.ChannelCode = WeiXinChannelCode.周末酒店订阅号;
            base.logFile = Configs.LogPath + string.Format("WeixinLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe";

        }
 
     

        public override string DealEventTypeEvent(RequestEntity requestXML, string toUser, string fromuserName, string createTime)
        {
            if (requestXML.Event.ToLower() == "subscribe" || requestXML.Event.ToLower() == "unsubscribe")
            {
                //关注/取消关注事件，记录这一信息
                try
                {
                    var w = new WeixinUser { Openid = toUser, WeixinAcount = "zmjiudian", SubscribeTime = DateTime.Now, CreateTime = DateTime.Now };
                    var update = weixinService.UpdateWeixinUserSubscribe(w);
                }
                catch (Exception ex)
                {

                }

                return WeiXinAdapter.getResponseForTextInput2(requestXML.Event.ToLower(), toUser, fromuserName, createTime);
            }
            else
            {
                return WeiXinAdapter.getResponseForTextInput2(requestXML.EventKey, toUser, fromuserName, createTime);
            }
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
