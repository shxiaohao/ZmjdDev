
using HJDAPI.APIProxy;
using HJDAPI.Models;
using MagiCallService.Contracts.Model.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WHotelSite.Common;
using WHotelSite.Filters;
using WHotelSite.Models;

namespace WHotelSite.Controllers
{
    public class MagiCallAPIController : ApiController
    {
     //   [CrossSite]
        [System.Web.Http.HttpPost]
        public string SendTxtMessageToUser(MagiCallTxtMsgEntity m)
        {
            return  new EMChat().SendTxtMessageToUser(m);
        }
        
        [System.Web.Http.HttpGet]
        [HttpPost]
        public string MagiCallClientHeart(long userid)
        {
            return new MagiCall().MagiCallClientHeart(new MessageEntity { UserID = userid, messageType = MessageType.UserWord, msg = "" });
        }
        [System.Web.Http.HttpPost]
        public string MagiCallClientMessage(MessageEntity message)
        {
            return new MagiCall().MagiCallClientMessage(message);
        }

        [System.Web.Http.HttpGet]
        public MagiCallClientViewModel GetMagiCallUserInfo(long userID)
        {

            new EMChat().AccountCreate(userID);
            MagiCallClientViewModel m = new MagiCallClientViewModel();

            m.userID = userID;
            m.userName = Config.magicalPreUserName + userID.ToString();
            m.userPassword = Config.magicalPreUserPassword + userID.ToString();
            m.kefuHeadPhoto =  Config.KefuHeadPhoto;
            m.userHeadPhoto = account.GetCurrentUserInfo(userID).AvatarUrl.Replace("jupiter", Config.AvatarSize);
            return m;
        }

    }
}
