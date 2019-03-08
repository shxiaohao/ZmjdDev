using System;
using System.Net;
using System.Web.Http;
using HJD.AccountServices.Contracts;
using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using Newtonsoft.Json;
using HJD.HotelManagementCenter.Domain.Comm;
using HJD.HotelManagementCenter.IServices;
using HJDAPI.Common.Security;
using HJD.ADServices.Contract;
using System.Collections.Generic;
using HJDAPI.Controllers.Adapter;
using MessageService.Contract;
using HJDAPI.Controllers.Common;

namespace HJDAPI.Controllers
{
    public class NoticeController:BaseApiController
    {
        static IADService ADService = ServiceProxyFactory.Create<IADService>("IADService");

        [HttpGet]
        public List<UserNoticesEntity> GetNotice(long userID,int  AppType, int AppVer  )
        {
            List<UserNoticesEntity> list = new List<UserNoticesEntity>();

            List<UserNoticeEntity>  lu =  ADService.GetOneUserValidNotices(userID);

            foreach (UserNoticeEntity un in lu)
            {
                switch (un.NoticeType)
                {
                    case 1:
                        list.Add(new UserNoticesEntity
                        {
                            NoticeID = un.IDX,
                            Actions = new List<ActionInfo> { new ActionInfo { ActionType = 1,
                                ActionURL =un.Action1URL, NeedNotifyService = true }, 
                                new ActionInfo { ActionType = 2, ActionURL = "", NeedNotifyService = true } },
                            Message = un.Message,
                            BackgroundColor = "#ffff8c00"
                        });
                        break;
                }
            }
            return list;
        }

        [HttpGet]
        public bool LogNoticeAction(long NoticeID, int actionType)
        {

            UserNoticeEntity un = new UserNoticeEntity();
            un.IDX = NoticeID;
            un.ActionType = actionType;
            un.UpdateTime = DateTime.Now;
            ADService.UpdateUserNotices(un);
           return true;
        }

        /// <summary>
        /// 发送App系统通知
        /// </summary>
        /// <param name="txtEntity"></param>
        /// <returns></returns>
        [HttpPost]
        public SendAppNoticeResponse SendAppNotice(SendNoticeEntity txtEntity)
        {
         //   Log.WriteLog(string.Format("SendAppNotice:{0} {1} {3}", txtEntity.userID, txtEntity.msg,txtEntity.noticeType));
            SendAppNoticeResponse response = new SendAppNoticeResponse();
            int i = MessageAdapter.SendAppNotice(txtEntity);
            switch (i)
            {
                case (int)HJDAPI.Models.BaseResponse.ResponseSuccessState.Success:
                    response.Success = (int)HJDAPI.Models.BaseResponse.ResponseSuccessState.Success;
                    break;
                default:
                    response.Success = (int)HJDAPI.Models.BaseResponse.ResponseSuccessState.Success;
                    break;
            }

            return response;
        }


        /// <summary>
        /// 向所有用户发送App系统通知
        /// </summary>
        /// <param name="txtEntity"></param>
        /// <returns></returns>
        [HttpPost]
        public SendAppNoticeResponse SendAllAppNotice(SendNoticeEntity txtEntity)
        {
            //   Log.WriteLog(string.Format("SendAppNotice:{0} {1} {3}", txtEntity.userID, txtEntity.msg,txtEntity.noticeType));
            SendAppNoticeResponse response = new SendAppNoticeResponse();
            int i = MessageAdapter.SendAllAppNotice(txtEntity);
            switch (i)
            {
                case (int)HJDAPI.Models.BaseResponse.ResponseSuccessState.Success:
                    response.Success = (int)HJDAPI.Models.BaseResponse.ResponseSuccessState.Success;
                    break;
                default:
                    response.Success = (int)HJDAPI.Models.BaseResponse.ResponseSuccessState.Success;
                    break;
            }

            return response;
        }
    }
}