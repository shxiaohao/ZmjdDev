using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Dialog;
using HJD.AccountServices.Entity;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain.Fund;
using HJD.HotelManagementCenter.IServices;
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
    public class FundController : BaseApiController
    {
        [HttpGet]
        public UserFundEntity GetUserFundInfo(Int64 userId)
        {
            return FundAdapter.GetUserFundInfo(userId);
        }

        [HttpGet]
        public List<UserFundIncomeStatEntity> GetUserFundIncomeStat(Int64 userId)
        {
            return FundAdapter.GetUserFundIncomeStat(userId);
        }

        [HttpGet]
        public List<UserFundExpendDetailEntity> GetUserFundExpendDetail(Int64 userId)
        {
            return FundAdapter.GetUserFundExpendDetail(userId);
        }

        [HttpPost]
        public int SendFundRelNotice(MagiCallTxtMsgEntity txtEntity)
        {
            var isTest = AccountAdapter.HasUserPriviledge(txtEntity.userID, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.InnerTest);
            if (isTest)
            {
                var sourceId = 100;
                var timeStamp = HJDAPI.Common.Security.Signature.GenSince1970_01_01_00_00_00Seconds();

                var urlWithoutSign = string.Format("http://www.zmjiudian.com/personal/wallet/{0}/fund?TimeStamp={1}&SourceID={2}",txtEntity.userID, timeStamp, sourceId);
            
                var sign = HJDAPI.Common.Security.Signature.GenSignature(timeStamp, sourceId, Configs.MD5Key, urlWithoutSign);
                var completeUrl = urlWithoutSign + "&sign=" + sign;

                var magicallActionUrl = "whotelapp://www.zmjiudian.com/gotopage?url=" + completeUrl;            

                var umengExtra = new UmengMessageExtra()
                {
                    action = magicallActionUrl,
                    type = "comment",
                    bgColor = "",
                    btnName = "",
                    isShowInApp = true
                };
                var title = "";
                //var noticeType = (int)txtEntity.noticeType;
                //var title = noticeType == 7 ? "您邀请的1位好友注册成功" : noticeType == 8 ? "恭喜您获得了10元住基金" : "您收到一条周末酒店App的通知";
                switch (txtEntity.appType)
                {
                    case 1:
                        MessageAdapter.SendMsgByUmeng(title, txtEntity.msg, txtEntity.userID, umengExtra, false);
                        break;
                    case 2:
                        MessageAdapter.SendMsgByUmeng(title, txtEntity.msg, txtEntity.userID, umengExtra, true);
                        break;
                    default:
                        MessageAdapter.SendMsgByUmeng(title, txtEntity.msg, txtEntity.userID, umengExtra, false);
                        MessageAdapter.SendMsgByUmeng(title, txtEntity.msg, txtEntity.userID, umengExtra, true);
                        break;
                }
            }
            return 0;
        }

        /// <summary>
        /// 增加指定用户的指定类型的住基金
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        [HttpPost]
        public int AddUserFund(UserFundIncomeDetail detail)
        {
            return FundAdapter.AddUserFund(detail);
        }
    }
}