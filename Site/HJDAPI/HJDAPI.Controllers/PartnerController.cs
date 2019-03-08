using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace HJDAPI.Controllers
{
    public class PartnerController : BaseApiController
    {
        static IRetailerService retailerService = ServiceProxyFactory.Create<IRetailerService>("HMC_IRetailerService");

        /// <summary>
        /// 验证渠道用户审核状态
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet]
        public RetailerInvateState GetRetailerInvateState(long userID)
        {
            RetailerInvateState result = RetailerInvateState.NoLog;
            var list = retailerService.GetRetailerInvateByUserID(userID);
            if (list.Count > 0)
            {
                result = RetailerInvateState.Reject;
                if (list.Where(_ => _.State == (int)RetailerInvateState.Pass).Count() > 0)
                {
                    result = RetailerInvateState.Pass;
                }
                else if (list.Where(_ => _.State == (int)RetailerInvateState.Auditing).Count() > 0)
                {
                    result = RetailerInvateState.Auditing;
                }
            }

            return result;
        }

        /// <summary>
        /// 根据手机号验证渠道用户
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        [HttpGet]
        public RetailerInvateEntity GetRetailerInvateByPhone(string phoneNum)
        {
            RetailerInvateEntity result = new RetailerInvateEntity();
            User_Info userInfo = AccountAdapter.GetPhoneUser(phoneNum);
            if (userInfo != null && userInfo.UserId > 0)
            {
                var list = retailerService.GetRetailerInvateByUserID(userInfo.UserId);
                if (list.Count > 0)
                {
                    result = list.First();
                }
            }
            return result;
        }

        /// <summary>
        /// 验证渠道用户审核状态
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet]
        public RetailerInvateEntity GetRetailerInvate(long userID)
        {
            RetailerInvateEntity result = new RetailerInvateEntity();
            if (userID > 0)
            {
                var list = retailerService.GetRetailerInvateByUserID(userID);
                if (list.Count > 0)
                {
                    result = list.First();
                }
            }
            return result;
        }


        /// <summary>
        /// 添加河道用户审核信息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [HttpPost]
        public bool AddRetailerInvateinfo(RetailerInvateEntity e)
        {
            RetailerInvateState state = GetRetailerInvateState(e.UserID);
            if (state == RetailerInvateState.NoLog)
            {
                if (string.IsNullOrEmpty(e.PhoneNum))
                {
                    var userEntity = AccountAdapter.GetCurrentUserInfo(e.UserID);
                    if (userEntity != null)
                    {
                        e.PhoneNum = userEntity.MobileAccount;
                    }
                }
                //e.State = (int)RetailerInvateState.Pass;//设置为通过状态  数据库中设置
                retailerService.AddRetailerInvate(e);
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}
