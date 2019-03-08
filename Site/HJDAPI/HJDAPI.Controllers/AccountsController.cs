using System;
using System.Net;
using System.Web.Http;
using HJD.AccountServices.Contracts;
using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using HJDAPI.Common.Security;
using HJDAPI.Controllers.Adapter;
using HJD.CouponService.Contracts.Entity;
using HJDAPI.Controllers.Common;
using System.Web;
using MessageService.Contract;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Linq;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Controllers
{
    public class AccountsController : BaseApiController
    {
        ///// <summary>
        ///// 获取用户可用现金券数量
        ///// </summary>
        ///// <param name="p"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public UserCanUseCashCouponResponse GetUserCanUseCashCouponAmount(UserCanUseCashCouponRequest p)
        //{
        //    UserCanUseCashCouponResponse r = new UserCanUseCashCouponResponse();

        //    if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
        //    {
        //        r.UserID = p.UserID;
        //        r.UserCanUseCashCouponAmount = CouponAdapter.GetUserCanUseCashCouponAmount(p.UserID);
        //    }
        //    else
        //    {
        //        r.SignError();
        //    }
        //    return r;
        //}

        //[HttpGet]
        //public long GetServiceTime()
        //{
        //    return Signature.GenTimeStamp();
        //}

        [HttpGet]
        public List<UserBindTypeEntity> GetUserBindTypeList()
        {
            return AccountAdapter.GetUserBindTypeList();
        }

        /// <summary>
        /// 根据渠道类型和渠道ID获取用户第三方绑定记录
        /// </summary>
        /// <param name="userchannelrel">（如微信：Channel=weixin Code=unionid）</param>
        /// <returns></returns>
        [HttpPost]
        public UserChannelRelEntity GetOneUserChannelRel(UserChannelRelEntity userchannelrel)
        {
            return AccountAdapter.GetOneUserChannelRel(userchannelrel);
        }

        /// <summary>
        /// 根据渠道类型和用户ID获取用户第三方绑定记录
        /// </summary>
        /// <param name="userchannelrel">（如微信：Channel=weixin）</param>
        /// <returns></returns>
        [HttpPost]
        public UserChannelRelEntity GetOneUserChannelRelByUID(UserChannelRelEntity userchannelrel)
        {
            return AccountAdapter.GetOneUserChannelRelByUID(userchannelrel);
        }

        /// <summary>
        /// 新增用户第三方绑定关系
        /// </summary>
        /// <param name="userchannelrel"></param>
        /// <returns></returns>
        [HttpPost]
        public OperationResult AddUserChannelRel(UserChannelRelEntity userchannelrel)
        {
            var result = new OperationResult();

            int i = AccountAdapter.AddUserChannelRel(userchannelrel);

            result.Success = i > 0;

            return result;
        }


        [HttpPost]
        public List<HJD.AccountServices.Entity.UserRole> GetUserRoleRelByUserId(UserRole p)
        {
            return AccountAdapter.GetUserRoleRelByUserId(p.UserID);
        }


        [HttpPost]
        public GetOrRegistPhoneUserResponse GetOrRegistPhoneUser(GetOrRegistPhoneUserRequestParams p)
        {
            GetOrRegistPhoneUserResponse r = new GetOrRegistPhoneUserResponse();

            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                if (p.Phone.Length == 0)
                {
                    r.NoPhoneError();
                }
                else
                {
                    r.UserID = AccountAdapter.GetOrRegistPhoneUser(p.Phone, p.CID).UserId;
                }
            }
            else
            {
                r.SignError();
            }
            return r;
        }

        [HttpPost]
        public List<UserCommInfoEntity> GetUserCommInfo(GetUserCommInfoReqParm p)
        {
            return AccountAdapter.GetUserCommInfo(p);
        }

        /// <summary>
        /// 获取发票信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public UserCommInvoiceInfoModel GetUserCommInvoiceInfo(GetUserCommInfoReqParm p)
        {

            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                p.Phone = DES.Decrypt(p.Phone);
                return AccountAdapter.GetUserCommInvoiceInfo(p, AppVer);
            }
            else
            {
                return new UserCommInvoiceInfoModel();
            }
            //return AccountAdapter.GetUserCommInvoiceInfo(p);
        }

        /// <summary>
        /// 获取发票信息 6.3版本开始使用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public InvoiceParamEntity GetInvoiceInfo(GetUserCommInfoReqParm param)
        {
            return AccountAdapter.GetInvoiceInfo(param, AppVer);
        }

        [HttpPost]
        public List<UserCommInfoEntity> GetUserCommInfo40(GetUserCommInfoReqParm p)
        {
            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                p.Phone = DES.Decrypt(p.Phone);
                return AccountAdapter.GetUserCommInfo(p);
            }
            else
            {
                return new List<UserCommInfoEntity>();
            }
        }

        //[HttpPost]
        //[HttpGet]
        //public List<UserCommInfoEntity> GetUserInvoiceTitleAddreeInfoByPhone(string phone)
        //{
        //    return AccountAdapter.GetUserInvoiceTitleAddreeInfo(long.Parse(phone), 2);
        //}

        //[HttpPost]
        //[HttpGet]
        //public List<UserCommInfoEntity> GetUserInvoiceTitleAddreeInfoByUserID(long userid)
        //{
        //    return AccountAdapter.GetUserInvoiceTitleAddreeInfo(userid, 1);
        //}

        /// <summary>
        /// 昵称可用：没有人用过，昵称没有包括特殊词
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        // [HttpGet] 
        [HttpPost]
        public ResultEntity CheckNickName([FromBody]UserNickNameModel u)
        {
            return AccountAdapter.CheckNickName(u.nickName, u.userID);
        }

        //  [HttpGet]
        //[HttpPost]
        //public ResultEntity UpdateNickName([FromBody]UserNickNameModel u)
        //{
        //    return AccountAdapter.UpdateNickName(u.nickName, u.userID, u.password);
        //}

        [HttpPost]
        public ResultEntity UpdateNickName40([FromBody]UserNickNameModel u)
        {
            if (Signature.IsRightSignature(u.TimeStamp, u.SourceID, u.RequestType, u.Sign))
            {
                u.nickName = DES.Decrypt(u.nickName);
                //  u.password = DES.Decrypt(u.password);
                return AccountAdapter.UpdateNickName(u.nickName, u.userID, u.password);
            }
            else
            {
                Log.WriteLog(string.Format("UpdateNickName40: 签名错误 {0} {1} {2} {3} {4}", u.userID, u.TimeStamp, u.SourceID, u.RequestType, u.Sign));
                return new ResultEntity { Success = 100, Message = "签名错误！" };
            }
        }

        [HttpPost]
        public Boolean IsVIPCustomer(AccountInfoItem ai)
        {
            var curUserID = Convert.ToInt64(ai.Uid);
            return AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(curUserID));
        }

        /// <summary>
        /// 获取指定用户的所有权限
        /// </summary>
        /// <param name="ai"></param>
        /// <returns></returns>
        [HttpPost]
        public List<UserPrivilegeRel> GetAllPrivilegeByUserId(AccountInfoItem ai)
        {
            return PrivilegeAdapter.GetAllPrivilegeByUserId(Convert.ToInt64(ai.Uid)) ?? new List<UserPrivilegeRel>();
        }

        [HttpPost]
        public int GetVIPTypeNum(AccountInfoItem ai)
        {
            var curUserID = Convert.ToInt64(ai.Uid);
            return Convert.ToInt32(AccountAdapter.GetCustomerType(curUserID));
        }

        /// <summary>
        /// 向手机发送验证短信
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        [HttpGet]
        public bool sendConfirmSMS(string phoneNum, Int64 TimeStamp = 0, int SourceID = 0, string RequestType = "", string sign = "")
        {
            try
            {
                var value = string.Format("{{\"phoneNum\":\"{0}\",\"TimeStamp\":\"{1}\",\"SourceID\":\"{2}\",\"RequestType\":\"{3}\",\"sign\":\"{4}\"}}", phoneNum, TimeStamp, SourceID, RequestType, sign);
                RecordBehavior("sendConfirmSMS", value);
            }
            catch (Exception ex) { }

            //Log.WriteLog("sendConfirmSMS:" + System.DateTime.Now+"   IP "+_ContextBasicInfo.UserHostAddress);
            return AccountAdapter.sendConfirmSMS(phoneNum);

            //if ((TimeStamp == 0 && SourceID == 0 && RequestType == "" && sign == "") || (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign)))
            //{
            //    //Log.WriteLog("sendConfirmSMS:" + System.DateTime.Now+"   IP "+_ContextBasicInfo.UserHostAddress);
            //    return AccountAdapter.sendConfirmSMS(phoneNum);
            //}
            //else
            //    return false;
        }

        [HttpGet]
        public bool sendConfirmSMS40(string phoneNum, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                // Log.WriteLog(string.Format("sendConfirmSMS40:{0} {1} {2} {3} {4} {5}", phoneNum, TimeStamp, SourceID, RequestType, sign, HttpContext.Current.Request.UserHostAddress));

                return AccountAdapter.sendConfirmSMS(phoneNum);
            }
            else
                return false;
        }

        /// <summary>
        /// 向手机发送密码
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        [HttpGet]
        public bool sendNewPasswordSMS40(string phoneNum, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {

            bool signResult = Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign);
            if (signResult)
            {

                return AccountAdapter.sendNewPasswordSMS(phoneNum);
            }

            // Log.WriteLog(string.Format("sendNewPasswordSMS40:{0} {1} {2} {3} {4} {5} {6}", phoneNum, TimeStamp, SourceID, RequestType, sign, HttpContext.Current.Request.UserHostAddress, signResult));

            return true;
        }

        //[HttpGet]
        //public bool sendNewPasswordSMS(string phoneNum)
        //{
        //    Log.WriteLog(string.Format("sendNewPasswordSMS40:{0} {1}  ", phoneNum,  HttpContext.Current.Request.UserHostAddress));
        //    return AccountAdapter.sendNewPasswordSMS(phoneNum);
        //}

        /// <summary>
        /// 验证手机验证码是否正确
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        [HttpGet]
        public bool checkConfirmSMS(string phoneNum, string confirmCode)
        {
            return AccountAdapter.checkConfirmSMS(phoneNum, confirmCode);
        }

        /// <summary>
        /// 判断邀请码是否存在
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        [HttpGet]
        public bool checkInvitationCode(string InvitationCode)
        {
            if (string.IsNullOrEmpty(InvitationCode))
            {
                return true;
            }
            else
            {
                return AccountAdapter.checkInvitationCode(InvitationCode);
            }
        }
        [HttpGet]
        public bool checkConfirmSMS40(string phoneNum, string confirmCode, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                return AccountAdapter.checkConfirmSMS(phoneNum, confirmCode);
            }
            else
                return false;
        }

        #region 安缦渠道短信通道

        [HttpGet]
        public bool sendConfirmSMSAnman(string phoneNum)
        {
            // Log.WriteLog("sendConfirmSMSAnman:" + phoneNum + ":" + HttpContext.Current.Request.UserHostAddress);
            return true;  // AccountAdapter.sendConfirmSMSAnman(phoneNum);
        }

        [HttpGet]
        public bool checkConfirmSMSAnman(string phoneNum, string confirmCode)
        {
            // Log.WriteLog("checkConfirmSMSAnman:" + phoneNum + ":" + HttpContext.Current.Request.UserHostAddress);

            return true; // AccountAdapter.checkConfirmSMSAnman(phoneNum, confirmCode);
        }

        #endregion

        [HttpGet]
        [ActionName("GetInfo")]
        public string GetInfo()
        {
            return "hello";
        }

        [HttpGet]
        public OperationResult Logout1()
        {
            return null;
        }

        [HttpPost]
        [ActionName("Login")]
        public OperationResult Login(AccountInfoItem ai)
        {
            var result = new OperationResult();

            try
            {
                IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");

                HJD.AccountServices.Entity.LoginResult serviceReturn;
                LoginRequest regrequest = new LoginRequest();
                regrequest.LoginID = ai.Email;
                regrequest.Password = ai.Password;
                regrequest.ClientIP = ai.RemoteAddress;

                serviceReturn = AccService.Login(regrequest);

                result.Success = serviceReturn.RetCode.StartsWith("TS");
                result.Message = result.Success ? "" : serviceReturn.Message;
                result.Email = ai.Email;


                if (result.Success)
                {
                    long userid = 0;
                    try
                    {
                        userid = long.Parse(serviceReturn.RetCode.Substring(serviceReturn.RetCode.LastIndexOf("|") + 1));
                    }
                    catch
                    {
                        userid = 0;
                    }
                    string nickname = string.Empty;
                    if (userid > 0)
                    {
                        var u = AccountAdapter.GetCurrentUserInfo(userid);

                        if (u != null)
                        {
                            nickname = u.NickName;
                            SetAuthorizeHead(userid, ai.Password, ai.Email);
                        }
                    }
                    string retcode = serviceReturn.RetCode;
                    string md5 = retcode.Substring(retcode.IndexOf("|") + 1, retcode.LastIndexOf("|") - retcode.IndexOf("|") - 1);
                    result.Email = ai.Email;
                    result.Data = userid + "|" + StringHelper.UidMask(nickname) + "|" + md5;   //使用正确的昵称
                }

            }
            catch (Exception e)
            {
                result.Data = e.Message;
            }
            return result;
        }

        [HttpPost]
        [ActionName("LoginForBGMember")]
        public OperationResult LoginForBGMember(AccountInfoItem ai)
        {
            var result = new OperationResult();

            try
            {
                IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");

                HJD.AccountServices.Entity.LoginResult serviceReturn;
                LoginRequest regrequest = new LoginRequest();
                regrequest.LoginID = ai.Email;
                regrequest.Password = ai.Password;
                regrequest.ClientIP = ai.RemoteAddress;

                serviceReturn = AccService.Login(regrequest);

                result.Success = serviceReturn.RetCode.StartsWith("TS");
                result.Message = result.Success ? "" : serviceReturn.Message;
                result.Email = ai.Email;


                if (result.Success)
                {
                    long userid = 0;
                    try
                    {
                        userid = long.Parse(serviceReturn.RetCode.Substring(serviceReturn.RetCode.LastIndexOf("|") + 1));
                    }
                    catch
                    {
                        userid = 0;
                    }
                    string nickname = string.Empty;
                    if (userid > 0)
                    {
                        var u = AccountAdapter.GetCurrentUserInfo(userid);

                        PrivilegeAdapter.RemoveUserPrivilegeCache(u.UserID);
                        result.UserPrivileges = PrivilegeAdapter.GetAllPrivilegeByUserId(u.UserID);//获取用户权限 

                        if (u != null)
                        {
                            nickname = u.NickName;
                            SetAuthorizeHead(userid, ai.Password, ai.Email);
                        }
                    }
                    string retcode = serviceReturn.RetCode;
                    string md5 = retcode.Substring(retcode.IndexOf("|") + 1, retcode.LastIndexOf("|") - retcode.IndexOf("|") - 1);
                    result.Email = ai.Email;
                    result.Data = userid + "|" + StringHelper.UidMask(nickname) + "|" + md5;   //使用正确的昵称
                }

            }
            catch (Exception e)
            {
                result.Data = e.Message;
            }
            return result;
        }

        [HttpPost]
        [ActionName("MobileLogin40")]
        public OperationResult MobileLogin40(AccountInfoItem ai)
        {
            OperationResult result = _MobileLogin40(ai);
            if (!result.Success && result.Message.Contains("手机号码尚未注册"))
            {
                result.Message = "登录失败，" + result.Message;
            }
            return result;
        }

        [HttpPost]
        [ActionName("PullUserBasicInfo")]
        public OperationResult PullUserBasicInfo(AccountInfoItem ai)
        {
            return _MobileLogin40(ai);
        }

        private OperationResult _MobileLogin40(AccountInfoItem ai)
        {
            var result = new OperationResult();
            try
            {
                if (Signature.IsRightSignature(ai.TimeStamp, ai.SourceID, ai.RequestType, ai.Sign))
                {
                    ai.Phone = DES.Decrypt(ai.Phone);
                    ai.Password = DES.Decrypt(ai.Password);

                    IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");

                    HJD.AccountServices.Entity.MobileLoginResult serviceReturn;
                    LoginRequest regrequest = new LoginRequest();
                    regrequest.LoginID = ai.Phone;
                    regrequest.Password = ai.Password;
                    regrequest.ClientIP = ai.RemoteAddress;

                    if (!AccountAdapter.NotRegistMobileAccount(ai.Phone))
                    {
                        return new OperationResult() { Success = false, Message = "手机号码尚未注册！" };
                    }

                    serviceReturn = AccService.MobileLogin(regrequest);

                    result.Success = serviceReturn.RetCode.StartsWith("TS");
                    result.Message = result.Success ? "" : serviceReturn.Message;
                    result.Mobile = ai.Phone;
                    result.Email = "";
                    result.Avatar = "";
                    result.PersonalSignature = "";
                    result.InvitationCode = serviceReturn.MemberProfileInfo.InvitationCode;

                    if (result.Success)
                    {
                        long userid = 0;
                        try
                        {
                            userid = long.Parse(serviceReturn.RetCode.Substring(serviceReturn.RetCode.LastIndexOf("|") + 1));
                        }
                        catch
                        {
                            userid = 0;
                        }

                        string nickname = string.Empty;
                        if (userid > 0)
                        {
                            UserInfoByLogin(result, userid, out nickname);

                            #region 注释
                            //var u = AccountAdapter.GetCurrentUserInfo(userid);
                            //if (u != null)
                            //{
                            //    nickname = u.NickName;
                            //    SetAuthorizeHead(userid, ai.Password, ai.Email);
                            //    //默认头像地址
                            //    result.Avatar = u.AvatarUrl;

                            //    result.FollowingsCount = AccountAdapter.GetFollowingsCountByUserID(userid);
                            //    result.FollowersCount = AccountAdapter.GetFollowersCountByUserID(userid);
                            //    result.PersonalSignature = u.MemberBrief;//个性签名
                            //    result.ThemeCodeSN = u.ThemeCodeSN;//个人主题

                            //    result.RealName = u.RealName;//真实姓名

                            //    result.IsTemporaryPWD = u.IsTemporaryPWD; //是否是临时密码

                            //    result.UserPrivileges = PrivilegeAdapter.GetAllPrivilegeByUserId(userid);//获取用户权限
                            //    result.UserPrivilegeNames = AccountAdapter.GetUserPrivilegeNames(result.UserPrivileges);

                            //    if (u.SaveMoney > 0)
                            //    {
                            //        result.SaveMoneyDesc = "已为你节省 ¥" + u.SaveMoney;
                            //    }
                            //    //result
                            //    //result.IsTemporaryPWD=u

                            //    if (CommMethods.NeedModifyNickName(nickname))
                            //    {
                            //        result.UserPrivilegeNames.Add("ShowHelloName");//添加需要修改昵称标识  由于业务调整不确定 暂时不宜新增字段维护
                            //    }

                            //    int CustomerType = (int)AccountAdapter.GetCustomerType(userid);
                            //    result.CustomerTypeDescribe = AccountAdapter.GetCustomerTypeDescribe(CustomerType);
                            //    switch (CustomerType)
                            //    {
                            //        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip:
                            //            result.CustomerTypeInterests = "终身VIP权益";
                            //            result.CustomerTypeInterestsUrl = "http://www.zmjiudian.com/Account/VipRights?userid={userid}&realuserid=1&_newpage=1";
                            //            result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip;
                            //            break;
                            //        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199:
                            //        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr:
                            //        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip3M:
                            //        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip6M:
                            //            result.CustomerTypeInterests = "VIP会员专区";
                            //            result.CustomerTypeInterestsUrl = "http://www.zmjiudian.com/Account/VipRights?userid={userid}&realuserid=1&_newpage=1";
                            //            result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199;
                            //            break;
                            //        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599:
                            //            result.CustomerTypeInterests = "VIP会员专区";
                            //            result.CustomerTypeInterestsUrl = "http://www.zmjiudian.com/Account/VipRights?userid={userid}&realuserid=1&_newpage=1";
                            //            result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599;
                            //            break;
                            //        default:
                            //            result.CustomerTypeInterests = "成为VIP";
                            //            result.CustomerTypeInterestsUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1";
                            //            result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.general;
                            //            break;
                            //    }

                            //    result.StartVipTime = Convert.ToDateTime("1900-01-01");
                            //    result.EndVipTime = Convert.ToDateTime("1900-01-01").ToString("yyyy年MM月dd日");

                            //    //购买VIP时间
                            //    List<ExchangeCouponEntity> exchangeVip = CouponAdapter.GetExchangeCouponEntityListByUser(userid, (int)(HJDAPI.Common.Helpers.Enums.CouponType.VIP));
                            //    if (exchangeVip.Count > 0)
                            //    {
                            //        result.StartVipTime = exchangeVip.OrderByDescending(_ => _.CreateTime).FirstOrDefault().CreateTime;
                            //        switch (CustomerType)
                            //        {
                            //            case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip:
                            //                result.EndVipTime = Convert.ToDateTime("3000-01-01").ToString("yyyy年MM月dd日");
                            //                break;
                            //            case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199:
                            //            case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr:
                            //            case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599:
                            //                result.EndVipTime = result.StartVipTime.AddYears(1).ToString("yyyy年MM月dd日");
                            //                break;
                            //            case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip3M:
                            //                result.EndVipTime = result.StartVipTime.AddMonths(3).ToString("yyyy年MM月dd日");
                            //                break;
                            //            case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip6M:
                            //                result.EndVipTime = result.StartVipTime.AddMonths(6).ToString("yyyy年MM月dd日");
                            //                break;
                            //            default:
                            //                result.EndVipTime = Convert.ToDateTime("1900-01-01").ToString("yyyy年MM月dd日");
                            //                break;
                            //        }
                            //    }

                            //    //if(result.Mobile == "18021036971")
                            //    //{
                            //    //    Log.WriteLog(string.Format("_MobileLogin40:{0} {1} {2} {3} {4}", userid, CustomerType, exchangeVip.Count, result.StartVipTime, result.EndVipTime));
                            //    //}
                            //}
                            #endregion
                        }

                        string retcode = serviceReturn.RetCode;
                        string md5 = retcode.Substring(retcode.IndexOf("|") + 1, retcode.LastIndexOf("|") - retcode.IndexOf("|") - 1);
                        result.Email = "";
                        result.Mobile = ai.Phone;
                        //Regex rg = new Regex(@"^1([0-9]{2}\*{4}\w*)");
                        result.Data = userid + "|" + StringHelper.UidMask(nickname) + "|" + md5;   //使用正确的昵称


                        if (AccountAdapter.IsInspector(userid))//如果是品鉴师
                        {
                            result.Data += "|Inspector";
                        }

                        HotelAdapter.BindUserAccountAndOrders(userid, ai.Phone); //关联帐号与手机号下的订单，以便关联用户在退出状态下的订单
                    }
                    else
                    {
                        result.Success = false;
                        result.Data = "登录验证失败！";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Data = "签名验证错误！";
                }
            }
            catch (Exception e)
            {
                result.Data = e.Message;
            }
            return result;
        }

        [HttpPost]
        [ActionName("PhoneNumLogin")]
        public OperationResult PhoneNumLogin(AccountInfoItem ai)
        {
            var result = new OperationResult();
            try
            {
                if (Signature.IsRightSignature(ai.TimeStamp, ai.SourceID, ai.RequestType, ai.Sign))
                {
                    ai.Phone = DES.Decrypt(ai.Phone);
                    if (AccountAdapter.checkConfirmSMS(ai.Phone, ai.ConfirmCode))
                    {
                        //result.Success = true;
                        //判断是否注册过
                        if (!AccountAdapter.NotRegistMobileAccount(ai.Phone))
                        {
                            int r = new Random().Next(100000, 999999);
                            string passWord = r.ToString();
                            string phone = ai.Phone;
                            AccountAdapter.RegisterPhoneUser(phone, passWord, 2, 0, "", true, CID: 0);
                            string registMsg = string.Format(@"你的临时密码为：{0}。登录周末酒店APP，可在设置页面更改密码。点击下载周末酒店APP：http://app.zmjiudian.com", passWord);
                            SMServiceController.SendSMS(phone, registMsg);
                        }

                        HJD.AccountServices.Entity.MobileLoginResult serviceReturn;
                        LoginRequest regrequest = new LoginRequest();
                        regrequest.LoginID = ai.Phone;
                        regrequest.ClientIP = ai.RemoteAddress;

                        serviceReturn = AccountAdapter.PhoneNumConfirmCodeLogin(regrequest);
                        result.Success = serviceReturn.RetCode.StartsWith("TS");
                        result.InvitationCode = serviceReturn.MemberProfileInfo.InvitationCode;


                        User_Info userInfo = AccountAdapter.GetUserInfoByMobile(ai.Phone);
                        string nickname = string.Empty;
                        UserInfoByLogin(result, userInfo.UserId, out nickname);

                        string retcode = serviceReturn.RetCode;
                        string md5 = retcode.Substring(retcode.IndexOf("|") + 1, retcode.LastIndexOf("|") - retcode.IndexOf("|") - 1);
                        result.Email = "";
                        result.Mobile = ai.Phone;
                        result.Data = userInfo.UserId + "|" + StringHelper.UidMask(nickname) + "|" + md5;   //使用正确的昵称


                        if (AccountAdapter.IsInspector(userInfo.UserId))//如果是品鉴师
                        {
                            result.Data += "|Inspector";
                        }

                        HotelAdapter.BindUserAccountAndOrders(userInfo.UserId, ai.Phone); //关联帐号与手机号下的订单，以便关联用户在退出状态下的订单
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "验证码不正确";
                        result.Data = "验证码不正确";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Data = "签名验证错误！";
                }
            }
            catch (Exception e)
            {
                result.Data = e.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取用户信息，不需要用户密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("PullUserInfo")]
        public OperationResult PullUserInfo(AccountInfoItem ai)
        {
            var result = new OperationResult();
            try
            {
                if (Signature.IsRightSignature(ai.TimeStamp, ai.SourceID, ai.RequestType, ai.Sign))
                {
                    ai.Phone = DES.Decrypt(ai.Phone);
                    HJD.AccountServices.Entity.MobileLoginResult serviceReturn;
                    LoginRequest regrequest = new LoginRequest();
                    regrequest.LoginID = ai.Phone;
                    regrequest.ClientIP = ai.RemoteAddress;

                    serviceReturn = AccountAdapter.PhoneNumConfirmCodeLogin(regrequest);
                    result.Success = serviceReturn.RetCode.StartsWith("TS");
                    result.InvitationCode = serviceReturn.MemberProfileInfo.InvitationCode;
                    result.Success = true;
                    string nickname = string.Empty;

                    long userid = 0;
                    try
                    {
                        userid = long.Parse(serviceReturn.RetCode.Substring(serviceReturn.RetCode.LastIndexOf("|") + 1));
                    }
                    catch
                    {
                        userid = 0;
                    }

                    UserInfoByLogin(result, userid, out nickname);

                    string retcode = serviceReturn.RetCode;
                    string md5 = retcode.Substring(retcode.IndexOf("|") + 1, retcode.LastIndexOf("|") - retcode.IndexOf("|") - 1);
                    result.Email = "";
                    result.Mobile = result.Mobile;
                    result.Data = userid + "|" + StringHelper.UidMask(nickname) + "|" + md5;   //使用正确的昵称

                    if (AccountAdapter.IsInspector(userid))//如果是品鉴师
                    {
                        result.Data += "|Inspector";
                    }

                    HotelAdapter.BindUserAccountAndOrders(userid, result.Mobile); //关联帐号与手机号下的订单，以便关联用户在退出状态下的订单
                }
                else
                {
                    result.Success = false;
                    result.Data = "签名验证错误！";
                }
            }
            catch (Exception e)
            {
                result.Data = e.Message;
            }
            return result;
        }

        public void UserInfoByLogin(OperationResult result, long userid, out string nickname)
        {
            var u = AccountAdapter.GetCurrentUserInfo(userid);
            nickname = "";
            if (u != null)
            {
                nickname = u.NickName;
                //默认头像地址
                result.Avatar = u.AvatarUrl;
                result.HelloWord = "你好，" + StringHelper.UidMask(nickname);

                result.FollowingsCount = AccountAdapter.GetFollowingsCountByUserID(userid);
                result.FollowersCount = AccountAdapter.GetFollowersCountByUserID(userid);
                result.PersonalSignature = u.MemberBrief;//个性签名
                result.ThemeCodeSN = u.ThemeCodeSN;//个人主题

                result.RealName = u.RealName;//真实姓名

                result.IsTemporaryPWD = u.IsTemporaryPWD; //是否是临时密码

                result.UserPrivileges = PrivilegeAdapter.GetAllPrivilegeByUserId(userid);//获取用户权限
                result.UserPrivilegeNames = AccountAdapter.GetUserPrivilegeNames(result.UserPrivileges);

                result.Mobile = u.MobileAccount;

                if (u.SaveMoney > 0)
                {
                    result.SaveMoneyDesc = "已为你节省 ¥" + u.SaveMoney;
                }
                //result
                //result.IsTemporaryPWD=u

                if (CommMethods.NeedModifyNickName(u.NickName))
                {
                    result.UserPrivilegeNames.Add("ShowHelloName");//添加需要修改昵称标识  由于业务调整不确定 暂时不宜新增字段维护
                }

                int CustomerType = (int)AccountAdapter.GetCustomerType(userid);
                result.CustomerTypeDescribe = AccountAdapter.GetCustomerTypeDescribe(CustomerType);
                switch (CustomerType)
                {
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip:
                        result.CustomerTypeInterests = "终身VIP权益";
                        result.CustomerTypeInterestsUrl = "http://www.zmjiudian.com/Account/VipRights?userid={userid}&realuserid=1&_newpage=1";
                        result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip;
                        result.HelloTip = "你已是金牌VIP";
                        break;
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199:
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr:
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip3M:
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip6M:
                        result.CustomerTypeInterests = "VIP会员专区";
                        result.CustomerTypeInterestsUrl = "http://www.zmjiudian.com/Account/VipRights?userid={userid}&realuserid=1&_newpage=1";
                        result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199;
                        result.HelloTip = "你已是金牌VIP";
                        break;
                    case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599:
                        result.CustomerTypeInterests = "VIP会员专区";
                        result.CustomerTypeInterestsUrl = "http://www.zmjiudian.com/Account/VipRights?userid={userid}&realuserid=1&_newpage=1";
                        result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599;
                        result.HelloTip = "你已是铂金VIP";
                        break;
                    default:
                        result.CustomerTypeInterests = "成为VIP";
                        result.CustomerTypeInterestsUrl = "http://www.zmjiudian.com/Coupon/VipShopInfo?userid={userid}&_newpage=1&_isoneoff=1";
                        result.CustomerType = (int)HJDAPI.Common.Helpers.Enums.CustomerType.general;
                        result.HelloTip = "旅行休闲，又好又划算";
                        break;
                }

                result.StartVipTime = Convert.ToDateTime("1900-01-01");
                result.EndVipTime = Convert.ToDateTime("1900-01-01").ToString("yyyy年MM月dd日");

                //购买VIP时间
                List<ExchangeCouponEntity> exchangeVip = CouponAdapter.GetExchangeCouponEntityListByUser(userid, (int)(HJDAPI.Common.Helpers.Enums.CouponType.VIP));
                if (exchangeVip.Count > 0)
                {
                    result.StartVipTime = exchangeVip.OrderByDescending(_ => _.CreateTime).FirstOrDefault().CreateTime;
                    switch (CustomerType)
                    {
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip:
                            result.EndVipTime = Convert.ToDateTime("3000-01-01").ToString("yyyy年MM月dd日");
                            break;
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199:
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr:
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip599:
                            result.EndVipTime = result.StartVipTime.AddYears(1).ToString("yyyy年MM月dd日");
                            break;
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip3M:
                            result.EndVipTime = result.StartVipTime.AddMonths(3).ToString("yyyy年MM月dd日");
                            break;
                        case (int)HJDAPI.Common.Helpers.Enums.CustomerType.vip6M:
                            result.EndVipTime = result.StartVipTime.AddMonths(6).ToString("yyyy年MM月dd日");
                            break;
                        default:
                            result.EndVipTime = Convert.ToDateTime("1900-01-01").ToString("yyyy年MM月dd日");
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// 根据UserId得到用户信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public UserInfoResult GetUserInfoByUserID(long userid)
        {
            return AccountAdapter.GetUserInfo(userid);
        }
        /// <summary>
        /// 修改会员真实姓名
        /// </summary>
        /// <param name="realName">Name</param>
        /// <param name="userId">UserID</param>
        /// <returns></returns>
        [HttpGet]
        public int UpdateRealName(string realName, long userId)
        {
            realName = System.Web.HttpUtility.UrlDecode(realName);
            return AccountAdapter.UpdateRealName(realName, userId);
        }


        [HttpPost]
        [ActionName("MobileLogin")]
        public OperationResult MobileLogin(AccountInfoItem ai)
        {
            var result = new OperationResult();

            try
            {
                IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");

                HJD.AccountServices.Entity.MobileLoginResult serviceReturn;
                LoginRequest regrequest = new LoginRequest();
                regrequest.LoginID = ai.Phone;
                regrequest.Password = ai.Password;
                regrequest.ClientIP = ai.RemoteAddress;

                if (!AccountAdapter.NotRegistMobileAccount(ai.Phone))
                {
                    return new OperationResult() { Success = false, Message = "登录失败,手机号码尚未注册！" };
                }

                serviceReturn = AccService.MobileLogin(regrequest);

                result.Success = serviceReturn.RetCode.StartsWith("TS");
                result.Message = result.Success ? "" : serviceReturn.Message;
                result.Mobile = ai.Phone;
                result.Email = "";


                if (result.Success)
                {
                    long userid = 0;
                    try
                    {
                        userid = long.Parse(serviceReturn.RetCode.Substring(serviceReturn.RetCode.LastIndexOf("|") + 1));
                    }
                    catch
                    {
                        userid = 0;
                    }
                    string nickname = string.Empty;
                    if (userid > 0)
                    {
                        var u = AccountAdapter.GetCurrentUserInfo(userid);

                        if (u != null)
                        {
                            nickname = u.NickName;
                            SetAuthorizeHead(userid, ai.Password, ai.Email);
                            result.Avatar = u.AvatarUrl;
                        }
                    }
                    string retcode = serviceReturn.RetCode;
                    string md5 = retcode.Substring(retcode.IndexOf("|") + 1, retcode.LastIndexOf("|") - retcode.IndexOf("|") - 1);
                    result.Email = "";
                    result.Mobile = ai.Phone;
                    //Regex rg = new Regex(@"^1([0-9]{2}\*{4}\w*)");
                    result.Data = userid + "|" + StringHelper.UidMask(nickname) + "|" + md5;   //使用正确的昵称

                    HotelAdapter.BindUserAccountAndOrders(userid, ai.Phone); //关联帐号与手机号下的订单，以便关联用户在退出状态下的订单
                }

            }
            catch (Exception e)
            {
                result.Data = e.Message;
            }
            return result;
        }

        [HttpPost]
        [ActionName("RegistPhoneUser")]
        public OperationResult RegistPhoneUser(RegistPhoneUserItem r)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(r.Password) || r.Password.Length < 6)
                {
                    return new OperationResult() { Success = false, Message = "密码长度不得少于6位" };
                }
                else if (checkConfirmSMS(r.Phone, r.ConfirmPassword))
                {
                    OperationResult result = AccountAdapter.RegisterPhoneUser(r.Phone, r.Password, 2, CID: r.CID, unionid: r.Unionid);
                    Log.WriteLog(string.Format("RegistPhoneUser:{0}:{1}", r.Phone, result.Success));
                    if (result.Success) //更新订单与帐号关系 
                    {
                        string phone = r.Phone;
                        long userid = long.Parse(result.Data.Split('|')[0]);

                        HotelAdapter.HotelService.BindUserAccountAndOrders(userid, phone);


                        if (DateTime.Now < DateTime.Parse("2016-1-1"))
                        {
                            Log.WriteLog(string.Format("RegistPhoneUser:{0}:{1}", r.Phone, "GiveCouponByActivityType"));
                            CouponAdapter.GiveCouponByActivityType(userid, CouponActivityCode.regist, phone);
                        }
                    }
                    result.Email = "";
                    return result;
                }
                else
                {
                    return new OperationResult() { Success = false, Message = "验证码不正确" };
                }
            }
            catch (Exception err)
            {
                Log.WriteLog(string.Format("RegistPhoneUser:{0}:{1}", r.Phone, err.Message));

                return new OperationResult() { Success = false, Message = err.Message };

            }
        }

        [HttpPost]
        [ActionName("RegistPhoneUser40")]
        public OperationResult RegistPhoneUser40(RegistPhoneUserItem r)
        {
            try
            {
                if (Signature.IsRightSignature(r.TimeStamp, r.SourceID, r.RequestType, r.Sign))
                {
                    r.Phone = DES.Decrypt(r.Phone);
                    r.Password = DES.Decrypt(r.Password);

                    if (string.IsNullOrWhiteSpace(r.Password) || r.Password.Length < 6)
                    {
                        return new OperationResult() { Success = false, Message = "密码长度不得少于6位" };
                    }
                    else if (AccountAdapter.NotRegistMobileAccount(r.Phone))
                    {
                        return new OperationResult() { Success = false, Message = "该手机号已经注册过了" };
                    }
                    else if (checkConfirmSMS(r.Phone, r.ConfirmPassword))
                    {
                        OperationResult result = AccountAdapter.RegisterPhoneUser(r.Phone, r.Password, 2, CID: r.CID, unionid: r.Unionid);
                        if (result.Success)
                        {
                            string phone = r.Phone;
                            long userid = long.Parse(result.Data.Split('|')[0]);


                            //PointsEntity pe = new PointsEntity()
                            //{
                            //    BusinessID = 0,
                            //    LeavePoint = 200,
                            //    TotalPoint = 200,
                            //    TypeID = 12,//12注册得到积分
                            //    UserID = userid,
                            //    Approver = 0
                            //};
                            //try
                            //{
                            //    HotelAdapter.HotelService.InsertOrUpdatePoints(pe);
                            //    //插入发送系统消息表
                            //    //MessageAdapter.InsertSysMessage(new SysMessageEnitity()
                            //    //{
                            //    //    state = 0,
                            //    //    businessID = userid,
                            //    //    businessType = (int)HJDAPI.Common.Helpers.Enums.SysMessigeType.Register,
                            //    //    receiver = userid
                            //    //});
                            //    //MessageAdapter.SendAppNotice(new SendNoticeEntity()
                            //    //{
                            //    //    actionUrl = "",
                            //    //    appType = 0,
                            //    //    from = "周末酒店",
                            //    //    msg = "恭喜注册成功，我们已将10奖励积分存放在您的钱包中。",
                            //    //    noticeType = ZMJDNoticeType.register,
                            //    //    title = "周末酒店",
                            //    //    userID = userid
                            //    //});
                            //}
                            //catch (Exception e)
                            //{
                            //    Log.WriteLog("注册插入积分报错：userid" + userid + " " + e.Message + "\r\n" + e.StackTrace);
                            //    throw e;
                            //}

                            //更新订单与帐号关系 
                            HotelAdapter.HotelService.BindUserAccountAndOrders(userid, phone);

                            //如果当前用户是被推荐的用户，则注册成功后奖励推荐人现金券
                            CouponAdapter.GiveCashCouponForSourceUser(userid, phone);

                            //2016年之前的注册现金券奖励机制，暂停止奖励
                            if (DateTime.Now < DateTime.Parse("2016-1-1")) CouponAdapter.GiveCouponByActivityType(userid, CouponActivityCode.regist, phone);
                        }
                        result.Email = "";
                        return result;
                    }
                    else
                    {
                        return new OperationResult() { Success = false, Message = "验证码不正确" };
                    }
                }
                else
                {
                    return new OperationResult() { Success = false, Message = "签名错误！" };
                }
            }
            catch (Exception err)
            {
                HJDAPI.Controllers.Common.Log.WriteLog("RegistPhoneUser40:" + err.Message + err.StackTrace);
                return new OperationResult() { Success = false, Message = err.Message };

            }
        }

        [HttpPost]
        [ActionName("RegistPhoneUser50")]
        public OperationResult RegistPhoneUser50(RegistPhoneUserItem r)
        {
            try
            {
                if (Signature.IsRightSignature(r.TimeStamp, r.SourceID, r.RequestType, r.Sign))
                {
                    r.Phone = DES.Decrypt(r.Phone);
                    r.Password = DES.Decrypt(r.Password);

                    Log.WriteLog("注册信息参数 RegistPhoneUser50：" + JsonConvert.SerializeObject(r));

                    if (string.IsNullOrWhiteSpace(r.Password) || r.Password.Length < 6)
                    {
                        return new OperationResult() { Success = false, Message = "密码长度不得少于6位" };
                    }
                    else if (AccountAdapter.NotRegistMobileAccount(r.Phone))
                    {
                        return new OperationResult() { Success = false, Message = "该手机号已经注册过了" };
                    }
                    else
                    {
                        //5.2版本 注册时没传邀请码，需要从验证邀请码和验证码 中获取
                        if (string.IsNullOrWhiteSpace(r.InvitationCode))
                        {
                            r.InvitationCode = AccountAdapter.LocalCache10Min.GetData<string>(string.Format("ExistsSMSCodeAndInvitationCode{0}", r.Phone), () => { return r.InvitationCode; });
                        }

                        User_Info inviteationCodeUser = null;
                        long CID = r.CID;
                        if (!string.IsNullOrEmpty(r.InvitationCode))
                        {
                            string invitationCode = r.InvitationCode.ToUpper();
                            inviteationCodeUser = AccountAdapter.GetUserIDByInvitationCode(invitationCode);
                            ChannelInfoEntity channelInfo = ChannelAdapter.GetChannelInfoByInvitationCode(invitationCode);
                            if (channelInfo.IDX > 0)
                            {
                                CID = channelInfo.IDX;
                            }
                        }
                        OperationResult result = AccountAdapter.RegisterPhoneUser(r.Phone, r.Password, 2, inviteationCodeUser == null ? 0 : inviteationCodeUser.UserId, r.NickName == null ? "" : r.NickName, r.IsTemporaryPWD, CID, r.Unionid);
                        if (result.Success)
                        {
                            string phone = r.Phone;
                            long userid = long.Parse(result.Data.Split('|')[0]);

                            //拿掉邀请码奖励 
                            //if (inviteationCodeUser != null)
                            //{
                            //    ////发现金券（注册类型，50rmb）注册人发现金券
                            //    //AccountAdapter.InvitationCodeJoinZmjdGetCoupon(userid, phone, inviteationCodeUser.UserId);
                            //    ////发现金券（注册类型，50rmb）推荐人
                            //    //AccountAdapter.InvitationCodeJoinZmjdGetCoupon(inviteationCodeUser.UserId, inviteationCodeUser.MobileAccount, 0);

                            //    //发积分推荐人
                            //    AccountAdapter.InvitationCodeRegisterInsertPonit(inviteationCodeUser.UserId, 50, 19);
                            //    AccountAdapter.InvitationCodeRegisterInsertPonit(userid, 50, 20);
                            //}

                            //更新订单与帐号关系 
                            HotelAdapter.HotelService.BindUserAccountAndOrders(userid, phone);

                            ////如果当前用户是被推荐的用户，则注册成功后奖励推荐人现金券
                            //CouponAdapter.GiveCashCouponForSourceUser(userid, phone);

                            //2016年之前的注册现金券奖励机制，暂停止奖励
                            if (DateTime.Now < DateTime.Parse("2016-1-1")) CouponAdapter.GiveCouponByActivityType(userid, CouponActivityCode.regist, phone);
                        }
                        result.Email = "";
                        return result;
                    }
                }
                else
                {
                    return new OperationResult() { Success = false, Message = "签名错误！" };
                }
            }
            catch (Exception err)
            {
                HJDAPI.Controllers.Common.Log.WriteLog("RegistPhoneUser50:" + err.Message + err.StackTrace);
                return new OperationResult() { Success = false, Message = err.Message };

            }
        }

        /// <summary>
        /// 手机用户修改密码：
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="olapassword"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ModifyPassword")]
        public OperationResult ModifyPassword(ModifyPasswordItem r)
        {
            if (!string.IsNullOrWhiteSpace(r.newpassword) && r.newpassword.Length >= 6)
            {
                return AccountAdapter.ModifyPassword(r);
            }
            else
            {
                return new OperationResult { Success = false, Message = "密码长度不得少于6位" };
            }
        }

        /// <summary>
        /// 手机用户修改密码：
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="olapassword"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ModifyPassword40")]
        public OperationResult ModifyPassword40(ModifyPasswordItem r)
        {
            if (Signature.IsRightSignature(r.TimeStamp, r.SourceID, r.RequestType, r.Sign))
            {
                r.oldpassword = DES.Decrypt(r.oldpassword);
                r.newpassword = DES.Decrypt(r.newpassword);
                if (!string.IsNullOrWhiteSpace(r.newpassword) && r.newpassword.Length >= 6)
                {
                    return AccountAdapter.ModifyPassword(r);
                }
                else
                {
                    return new OperationResult { Success = false, Message = "密码长度不得少于6位" };
                }
            }
            else
            {
                return new OperationResult { Success = false, Message = "签名错误！" };
            }
        }

        [HttpPost]
        [ActionName("SendModifyUserPhoneConfirmCode")]
        public OperationResult SendModifyUserPhoneConfirmCode(ModifyUserPhoneItem m)
        {
            Log.WriteLog("SendModifyUserPhoneConfirmCode: " + System.DateTime.Now);
            if (Signature.IsRightSignature(m.TimeStamp, m.SourceID, m.RequestType, m.Sign))
            {
                if (!AccountAdapter.CheckUserIDAndPassword(m.userid, m.password))
                {
                    return new OperationResult() { Success = false, Message = "密码不正确！" };
                }
                else if (AccountAdapter.ExistsMobileAccount(m.newPhone))
                {
                    return new OperationResult() { Success = false, Message = "该手机号码已存在！" };
                }
                else if (m.newPhone.Length < 11 || !(new Regex("^1[0-9]{10}$", RegexOptions.None).IsMatch(m.newPhone)))
                {
                    return new OperationResult() { Success = true, Message = "手机号码错误！" };
                }
                else
                {
                    AccountAdapter.sendConfirmSMS(m.newPhone);
                    return new OperationResult() { Success = true, Message = "密码正确" };
                }
            }
            else
            {
                return new OperationResult() { Success = false, Message = "签名不正确！" };
            }
        }

        [HttpPost]
        [ActionName("SendModifyUserPhoneConfirmCode40")]
        public OperationResult SendModifyUserPhoneConfirmCode40(ModifyUserPhoneItem m)
        {
            if (Signature.IsRightSignature(m.TimeStamp, m.SourceID, m.RequestType, m.Sign))
            {
                m.password = DES.Decrypt(m.password);
                m.newPhone = DES.Decrypt(m.newPhone);
                if (!AccountAdapter.CheckUserIDAndPassword(m.userid, m.password))
                {
                    return new OperationResult() { Success = false, Message = "密码不正确！" };
                }
                else if (AccountAdapter.ExistsMobileAccount(m.newPhone))
                {
                    return new OperationResult() { Success = false, Message = "该手机号码已存在！" };
                }
                else if (m.newPhone.Length < 11 || !(new Regex("^1[0-9]{10}$", RegexOptions.None).IsMatch(m.newPhone)))
                {
                    return new OperationResult() { Success = true, Message = "手机号码错误！" };
                }
                else
                {
                    AccountAdapter.sendConfirmSMS(m.newPhone);
                    return new OperationResult() { Success = true, Message = "密码正确" };
                }
            }
            else
            {
                return new OperationResult() { Success = false, Message = "签名不正确！" };
            }
        }

        [HttpPost]
        [ActionName("ModifyUserPhone")]
        public OperationResult ModifyUserPhone(ModifyUserPhoneItem m)
        {
            if (AccountAdapter.checkConfirmSMS(m.newPhone, m.confirmCode))
            {
                if (AccountAdapter.CheckUserIDAndPassword(m.userid, m.password))
                {
                    return ModifyUserPhoneNo(m.userid, m.newPhone);
                }
                else
                {
                    return new OperationResult() { Success = false, Message = "手机号修改失败" };
                }
            }
            else
            {
                return new OperationResult() { Success = false, Message = "新手机验证码不正确" };
            }
        }

        [HttpPost]
        [ActionName("ModifyUserPhone40")]
        public OperationResult ModifyUserPhone40(ModifyUserPhoneItem m)
        {
            if (Signature.IsRightSignature(m.TimeStamp, m.SourceID, m.RequestType, m.Sign))
            {
                m.password = DES.Decrypt(m.password);
                m.newPhone = DES.Decrypt(m.newPhone);

                if (AccountAdapter.checkConfirmSMS(m.newPhone, m.confirmCode))
                {
                    if (AccountAdapter.CheckUserIDAndPassword(m.userid, m.password))
                    {
                        return ModifyUserPhoneNo(m.userid, m.newPhone);
                    }
                    else
                    {
                        return new OperationResult() { Success = false, Message = "手机号修改失败" };
                    }
                }
                else
                {
                    return new OperationResult() { Success = false, Message = "新手机验证码不正确" };
                }
            }
            else
            {
                return new OperationResult() { Success = false, Message = "验证码错误！" };
            }
        }

        /// <summary>
        /// 修改手机号 同时修改订单相关券码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPhone"></param>
        /// <param name="oldPhone"></param>
        private OperationResult ModifyUserPhoneNo(long userId, string newPhone, string oldPhone = "")
        {
            try
            {
                userId = userId == 0 && !string.IsNullOrWhiteSpace(oldPhone) ? AccountAdapter.GetPhoneUser(oldPhone).UserId : userId;
            }
            catch
            {
                userId = 0;
            }
            // * RetCode=-200 操作失败    
            //* -201:记录不存在   
            //* -202:手机号已存在 
            // 0 :成功
            int result = AccountAdapter.UpdateMobileByUserId(userId, newPhone);
            OperationResult ret = new OperationResult() { Success = true, Message = "手机号修改成功！" };
            switch (result)
            {
                case 0:
                    HotelAdapter.HotelService.BindUserAccountAndOrders(userId, newPhone);
                    break;
                case -200:  // * RetCode=-200 操作失败    
                    ret = new OperationResult() { Success = false, Message = "操作失败！" };
                    break;
                case -201:  // * RetCode=-200 操作失败    
                    ret = new OperationResult() { Success = false, Message = "帐户不存在！" };
                    break;
                case -202:  // * RetCode=-200 操作失败    
                    ret = new OperationResult() { Success = false, Message = "手机帐号已存在！" };
                    break;
            }

            return ret;
        }

        [HttpPost]
        [ActionName("ExistsMobileAccount")]
        public ExistsMobileAccountResponse ExistsMobileAccount(ExistsMobileAccountItem p)
        {
            ExistsMobileAccountResponse r = new ExistsMobileAccountResponse();
            r.ExistsMobileAccount = AccountAdapter.NotRegistMobileAccount(p.Phone);
            return r;
        }

        [HttpPost]
        [ActionName("ExistsMobileAccount40")]
        public ExistsMobileAccountResponse ExistsMobileAccount40(ExistsMobileAccountItem p)
        {
            ExistsMobileAccountResponse r = new ExistsMobileAccountResponse();
            if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
            {
                r.ExistsMobileAccount = AccountAdapter.NotRegistMobileAccount(p.Phone);
                r.Success = true;
            }
            else
            {
                r.ExistsMobileAccount = false;
                r.Success = false;
                r.Message = "签名错误！";
            }
            return r;
        }


        [HttpPost]
        [ActionName("ExistsSMSCodeAndInvitationCode")]
        public ExistsCodeResponse ExistsSMSCodeAndInvitationCode(ExistsCodeItem p)
        {
            ExistsCodeResponse r = new ExistsCodeResponse();

            try
            {
                if (Signature.IsRightSignature(p.TimeStamp, p.SourceID, p.RequestType, p.Sign))
                {
                    bool checksms = checkConfirmSMS(p.Phone, p.SMSCode);

                    p.InvitationCode = p.InvitationCode == null ? "" : p.InvitationCode;

                    bool checkInvitation = checkInvitationCode(p.InvitationCode);
                    if (!checksms)
                    {
                        r.Success = false;
                        r.Message = "验证码错误";
                    }
                    else if (!checkInvitation)
                    {
                        r.Success = false;
                        r.Message = "邀请码不存在";
                    }
                    else if (checksms && checkInvitation)
                    {
                        r.Success = true;
                        //AccountAdapter.LocalCache10Min.GetData(string.Format("{0},{1}",p.Phone,p.InvitationCode))
                        //ios5.2版本注册时 没有传 邀请码 。
                        //string.
                        AccountAdapter.LocalCache10Min.Set(string.Format("ExistsSMSCodeAndInvitationCode{0}", p.Phone), p.InvitationCode.ToUpper());
                    }
                }
                else
                {
                    r.Success = false;
                    r.Message = "签名错误！";
                }
            }
            catch (Exception err)
            {
                Log.WriteLog("ExistsSMSCodeAndInvitationCode: " + err.Message + err.StackTrace);
            }
            return r;
        }

        [HttpPost]
        [ActionName("CheckUserIDAndPassword")]
        public CheckUserIDAndPasswordResponse CheckUserIDAndPassword(CheckUserIDAndPasswordItem m)
        {
            CheckUserIDAndPasswordResponse r = new CheckUserIDAndPasswordResponse();
            r.UserIDAndPasswordCorrect = AccountAdapter.CheckUserIDAndPassword(m.userid, m.password);
            return r;
        }

        [HttpPost]
        [ActionName("CheckUserIDAndPassword40")]
        public CheckUserIDAndPasswordResponse CheckUserIDAndPassword40(CheckUserIDAndPasswordItem m)
        {
            CheckUserIDAndPasswordResponse r = new CheckUserIDAndPasswordResponse();
            if (Signature.IsRightSignature(m.TimeStamp, m.SourceID, m.RequestType, m.Sign))
            {
                m.password = DES.Decrypt(m.password);

                r.UserIDAndPasswordCorrect = AccountAdapter.CheckUserIDAndPassword(m.userid, m.password);
                r.Success = true;
            }
            else
            {
                Log.WriteLog(string.Format("CheckUserIDAndPassword40: 签名错误 {0} {1} {2} {3} {4}", m.userid, m.TimeStamp, m.SourceID, m.RequestType, m.Sign));

                r.UserIDAndPasswordCorrect = false;
                r.Success = false;
                r.Message = "签名错误！";
            }
            return r;
        }

        [HttpPost]
        [ActionName("SendResetPasswordWithPhoneConfirmCode")]
        public OperationResult SendResetPasswordWithPhoneConfirmCode(ResetPasswordItem r)
        {
            if (!AccountAdapter.NotRegistMobileAccount(r.Phone))
            {
                return new OperationResult() { Success = false, Message = "手机号未注册过！" };
            }
            else
            {
                AccountAdapter.sendConfirmSMS(r.Phone);
                return new OperationResult() { Success = true, Message = "确认短信已发送！" };
            }
        }

        [HttpPost]
        [ActionName("ResetPasswordWithPhone")]
        public OperationResult ResetPasswordWithPhone(ResetPasswordItem r)
        {
            if (AccountAdapter.checkConfirmSMS(r.Phone, r.confirmCode))
            {
                User_Info ui = AccountAdapter.GetPhoneUser(r.Phone);

                if (ui != null && ui.UserId > 0)
                {
                    int retCode = AccountAdapter.UpdatePasswordByUserId(ui.UserId.ToString(), r.newpassword, "");// AccountAdapter.UpdateMobileByUserId(ui.UserId, r.newpassword);
                    if (retCode == 0)
                    {
                        return new OperationResult() { Success = true, Message = "密码重置成功！" };
                    }
                    else
                    {
                        return new OperationResult() { Success = false, Message = "密码重置失败" + retCode.ToString() };

                    }
                }
                else
                {
                    return new OperationResult() { Success = false, Message = "手机用户不存在！" };

                }
            }
            else
            {
                return new OperationResult() { Success = false, Message = "密码重置失败" };
            }
        }

        [HttpPost]
        [ActionName("ResetPasswordWithPhone40")]
        public OperationResult ResetPasswordWithPhone40(ResetPasswordItem r)
        {
            if (Signature.IsRightSignature(r.TimeStamp, r.SourceID, r.RequestType, r.Sign))
            {
                r.Phone = DES.Decrypt(r.Phone);
                r.newpassword = DES.Decrypt(r.newpassword);

                if (string.IsNullOrWhiteSpace(r.newpassword) || r.newpassword.Length < 6)
                {
                    return new OperationResult() { Success = false, Message = "密码长度不得少于6位" };
                }
                else if (AccountAdapter.checkConfirmSMS(r.Phone, r.confirmCode))
                {
                    User_Info ui = AccountAdapter.GetPhoneUser(r.Phone);

                    if (ui != null && ui.UserId > 0)
                    {
                        int retCode = AccountAdapter.UpdatePasswordByUserId(ui.UserId.ToString(), r.newpassword, "");// AccountAdapter.UpdateMobileByUserId(ui.UserId, r.newpassword);
                        if (retCode == 0)
                        {
                            return new OperationResult() { Success = true, Message = "密码重置成功！" };
                        }
                        else
                        {
                            return new OperationResult() { Success = false, Message = "密码重置失败" + retCode.ToString() };

                        }
                    }
                    else
                    {
                        return new OperationResult() { Success = false, Message = "手机用户不存在！" };

                    }
                }
                else
                {
                    return new OperationResult() { Success = false, Message = "验证码不正确！" };
                }
            }
            else
            {
                return new OperationResult() { Success = false, Message = "签名不正确！" };
            }
        }


        [HttpGet]
        public List<UserChannelRelHistoryEntity> GetChannelBelongToUserList(long cid)
        {
            return AccountAdapter.GetChannelBelongToUserList(cid);
        }


        [HttpPost]
        [ActionName("Register")]
        public OperationResult Register(AccountInfoItem item)
        {
            return AccountAdapter.Register(item);

            //string email = item.Email;
            //string nickname = item.NickName;
            //string password = item.Password;
            //string remoteAddress = item.RemoteAddress;
            //var result = new OperationResult();


            //IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");

            //string serviceReturn = "";
            //RegisterRequest regrequest = new RegisterRequest();
            //regrequest.Nickname = nickname;
            //regrequest.Email = email;
            //regrequest.Password = password;
            //regrequest.RemoteAddress = remoteAddress;
            //regrequest.IsMobile = item.IsMobile == 1 ? true : false;

            //serviceReturn = AccService.Register(regrequest);

            //result.Success = serviceReturn.StartsWith("TA01");
            //result.Message = result.Success ? "" : serviceReturn;

            //long userid = 0;
            //try
            //{
            //    userid = long.Parse(serviceReturn.Substring(serviceReturn.LastIndexOf("|") + 1));
            //}
            //catch
            //{
            //    userid = 0;
            //}
            //if (result.Success)
            //{
            //    SetAuthorizeHead(userid, password, email);
            //    string keyId = serviceReturn.Substring(serviceReturn.IndexOf("|") + 1, serviceReturn.LastIndexOf("|")-serviceReturn.IndexOf("|") - 1);
            //    result.Data = userid + "|" + StringHelper.UidMask(nickname) + "|" + keyId;
            //    result.Email = email;
            //}

            //return result;
        }

        [HttpPost]
        [ActionName("OAuthLogin")]
        public OperationResult OAuthLogin(AccountInfoItem item)
        {
            var result = new OperationResult();
            IAccountService AccService = null;
            string retCode = "";
            AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");

            retCode = AccService.ThirdPatryLogin(item.LoginType, item.Uid, item.NickName, item.AccessToken, item.AccessTokenSecret);

            if (!retCode.Contains("|"))
            {
                result.Success = false;
                result.Message = "登录失败";
            }
            else
            {
                result.Success = true;
                result.Data = retCode;
                result.Message = "";
                result.Email = "";

                long userid = 0;
                try
                {
                    userid = long.Parse(retCode.Substring(0, retCode.IndexOf("|")));
                }
                catch
                {
                    userid = 0;
                }

                SetAuthorizeHead(userid);
            }

            return result;
        }

        /// <summary>
        /// 添加新的用户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddUserCommInfo")]
        public bool AddUserCommInfo(UserCommInfoEntity entity)
        {
            return AccountAdapter.AddUserCommInfo(entity);
        }

        /// <summary>
        /// 由userID获得用户信息(不暴露给APP)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetCurrentUserInfo")]
        public MemberProfileInfo GetCurrentUserInfo(GetUserCommInfoReqParm param)
        {
            MemberProfileInfo info = null;
            if (param.UserID != 0)
            {
                info = AccountAdapter.GetCurrentUserInfo(param.UserID);
            }
            return info;
        }

        [HttpPost]
        public List<User_Info> GetUserInfoZmjiudian()
        {
            return AccountAdapter.GetUserInfoZmjiudian();
        }

        [HttpPost]
        public List<UserRightRole> GetUserRightRole()
        {
            return AccountAdapter.GetUserRightRole();
        }

        [HttpPost]
        public User_Info GetUserInfoByMobile(GetUserCommInfoReqParm param)
        {
            return AccountAdapter.GetPhoneUser(param.Phone);
        }

        /// <summary>
        /// 上传头像或者更新用户头像
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="avatarSUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UploadUserAvatar")]
        public UploadAvatarResult UploadUserAvatar(AccountAvatarParam param)
        {
            int appChannel = StringHelper.TransAppTypeHeaderToAppType(AppType);
            if (appChannel == 1 || appChannel == 2 || Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                int result = AccountAdapter.UploadUserAvatar(param.userID, param.avatarSUrl);
                var resultUpload = new UploadAvatarResult();
                if (result == 0)
                {
                    try
                    {
                        int isGetPoints = HotelAdapter.GetPointslistNumByUserIdAndTypeId(param.userID, 10) != null ? HotelAdapter.GetPointslistNumByUserIdAndTypeId(param.userID, 10).Count : 0;
                        if (isGetPoints == 0)
                        {

                            PointsEntity pe = new PointsEntity()
                            {
                                BusinessID = 0,
                                LeavePoint = 5,//上传头像由20积分改为5积分  20170829  by  luoli
                                TotalPoint = 5,
                                TypeID = 10,//10表示上传头像得到积分类型
                                UserID = param.userID,
                                Approver = 0
                            };
                            HotelAdapter.HotelService.InsertOrUpdatePoints(pe);

                            MessageAdapter.InsertSysMessage(new SysMessageEnitity()
                            {
                                state = 0,
                                businessID = param.userID,
                                businessType = (int)HJDAPI.Common.Helpers.Enums.SysMessigeType.UploadUserAvatar,
                                receiver = param.userID
                            });
                            MessageAdapter.SendAppNotice(new SendNoticeEntity()
                            {
                                actionUrl = "",
                                appType = 0,
                                from = "周末酒店",
                                msg = "换个头像换个心情，20积分送给美好的你。",
                                noticeType = ZMJDNoticeType.uploaduseravatar,
                                title = "周末酒店",
                                userID = param.userID
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog("第一次上传头像插入积分报错：userid" + param.userID + " " + e.Message + "\r\n" + e.StackTrace);
                    }
                    resultUpload = new UploadAvatarResult
                {
                    Success = true,
                    Message = "上传成功",
                    Avatar = string.IsNullOrWhiteSpace(param.avatarSUrl) ? DescriptionHelper.defaultAvatar :
                    PhotoAdapter.GenHotelPicUrl(param.avatarSUrl, Enums.AppPhotoSize.jupiter)
                };
                }
                else if (result == 1)
                {
                    resultUpload = new UploadAvatarResult()
                {
                    Success = false,
                    Message = "没有链接",
                    Avatar = DescriptionHelper.defaultAvatar
                };
                }
                else
                {
                    resultUpload = new UploadAvatarResult()
                    {
                        Success = false,
                        Message = "发生错误，请联系我们",
                        Avatar = DescriptionHelper.defaultAvatar
                    };
                }
                return resultUpload;
                //return
                //result == 0 ? new UploadAvatarResult()
                //{
                //    Success = true,
                //    Message = "上传成功",
                //    Avatar = string.IsNullOrWhiteSpace(param.avatarSUrl) ? DescriptionHelper.defaultAvatar :
                //    PhotoAdapter.GenHotelPicUrl(param.avatarSUrl, Enums.AppPhotoSize.jupiter)
                //} :
                //result == 1 ? new UploadAvatarResult()
                //{
                //    Success = false,
                //    Message = "没有链接",
                //    Avatar = DescriptionHelper.defaultAvatar
                //} :
                //    new UploadAvatarResult()
                //    {
                //        Success = false,
                //        Message = "发生错误，请联系我们",
                //        Avatar = DescriptionHelper.defaultAvatar
                //    };
            }
            else
            {
                return new UploadAvatarResult()
                {
                    Avatar = string.IsNullOrWhiteSpace(param.avatarSUrl) ? DescriptionHelper.defaultAvatar :
                        PhotoAdapter.GenHotelPicUrl(param.avatarSUrl, Enums.AppPhotoSize.jupiter),
                    Success = false,
                    Message = "签名错误"
                };
            }
        }

        /// <summary>
        /// 更新关注状态
        /// </summary>
        /// <param name="follower">粉丝UserID</param>
        /// <param name="following">被关注者UserID</param>
        /// <param name="IsValid">关注状态有效无效</param>
        /// <returns></returns>
        [HttpPost]
        public ChangeFollowerFollowingResult UpdateFollowerFollowingRel(ChangeFollowerFollowingParam param)
        {
            ChangeFollowerFollowingResult result = null;
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                if (param.follower * param.following == 0)
                {
                    result = new ChangeFollowerFollowingResult()
                    {
                        FollowersCount = 0,
                        Success = false,
                        FollowState = 0,
                        Message = "关注人和被关注人UserID不能为0"
                    };
                }
                else if (param.follower == param.following)
                {
                    result = new ChangeFollowerFollowingResult()
                    {
                        FollowersCount = 0,
                        Success = false,
                        FollowState = 0,
                        Message = "自己不能关注自己"
                    };
                }
                else
                {
                    MemberProfileInfo memberProfileInfo = AccountAdapter.GetCurrentUserInfo(param.following);
                    if (memberProfileInfo != null && memberProfileInfo.UserID > 0)
                    {
                        AccountAdapter.UpdateFollowerFollowingRel(param.follower, param.following, param.isValid);

                        int followersCount = AccountAdapter.GetFollowersCountByUserID(param.follower);
                        int followingsCount = AccountAdapter.GetFollowingsCountByUserID(param.follower);

                        int othersFollowersCount = AccountAdapter.GetFollowersCountByUserID(param.following);
                        int othersFollowingsCount = AccountAdapter.GetFollowingsCountByUserID(param.following);

                        result = new ChangeFollowerFollowingResult()
                        {
                            FollowersCount = followersCount,
                            FollowingsCount = followingsCount,
                            OthersFollowersCount = othersFollowersCount,
                            OthersFollowingsCount = othersFollowingsCount,
                            Success = true,
                            FollowState = AccountAdapter.GetFollowFollowingRelState(param.following, param.follower),
                            Message = param.isValid ? "关注成功！" : "取消关注成功！"
                        };

                        var msgList = MessageAdapter.GetUnReadMessageList(param.following, 5);
                        if (msgList != null && msgList.Count != 0)
                        {
                            var msg = msgList[msgList.Count - 1];
                            MessageAdapter.SendAppNotice(new SendNoticeEntity()
                            {
                                actionUrl = "whotelapp://www.zmjiudian.com/msglist?userid={userid}&realuserid=1",
                                title = "周末酒店",
                                msg = string.Format("{0}关注了你", msg.sendNickName),
                                appType = 0,
                                from = msg.sendNickName,
                                noticeType = ZMJDNoticeType.follow,
                                userID = param.following
                            });
                        }
                    }
                    else
                    {
                        result = new ChangeFollowerFollowingResult()
                        {
                            FollowersCount = 0,
                            Success = false,
                            FollowState = 0,
                            Message = "该账号不存在！"
                        };
                    }
                }
            }
            else
            {
                result = new ChangeFollowerFollowingResult()
                {
                    FollowersCount = 0,
                    Success = false,
                    FollowState = 0,
                    Message = "验证错误，请检查参数！"
                };
            }
            return result;
        }

        #region 当时为了测试
        /// <summary>
        /// 更新关注状态new 
        /// </summary>
        /// <param name="follower">粉丝UserID</param>
        /// <param name="following">被关注者UserID</param>
        /// <param name="IsValid">关注状态有效无效</param>
        /// <returns></returns>
        //[HttpPost]
        //public ChangeFollowerFollowingResult UpdateFollowerFollowingRelNew([FromBody]ChangeFollowerFollowingParam param, [FromUri] ChangeFollowerFollowingParam paramBase)
        //{
        //    ChangeFollowerFollowingResult result = null;
        //    if (Signature.IsRightSignature(paramBase.TimeStamp, paramBase.SourceID, paramBase.RequestType, paramBase.Sign))
        //    {
        //        if (param.follower * param.following == 0)
        //        {
        //            result = new ChangeFollowerFollowingResult()
        //            {
        //                FollowersCount = 0,
        //                Success = false,
        //                FollowState = 0,
        //                Message = "关注人和被关注人UserID不能为0"
        //            };
        //        }
        //        else if (param.follower == param.following)
        //        {
        //            result = new ChangeFollowerFollowingResult()
        //            {
        //                FollowersCount = 0,
        //                Success = false,
        //                FollowState = 0,
        //                Message = "自己不能关注自己"
        //            };
        //        }
        //        else
        //        {
        //            MemberProfileInfo memberProfileInfo = AccountAdapter.GetCurrentUserInfo(param.following);
        //            if (memberProfileInfo != null && memberProfileInfo.UserID > 0)
        //            {
        //                AccountAdapter.UpdateFollowerFollowingRel(param.follower, param.following, param.isValid);

        //                int followersCount = AccountAdapter.GetFollowersCountByUserID(param.follower);
        //                int followingsCount = AccountAdapter.GetFollowingsCountByUserID(param.follower);

        //                int othersFollowersCount = AccountAdapter.GetFollowersCountByUserID(param.following);
        //                int othersFollowingsCount = AccountAdapter.GetFollowingsCountByUserID(param.following);

        //                result = new ChangeFollowerFollowingResult()
        //                {
        //                    FollowersCount = followersCount,
        //                    FollowingsCount = followingsCount,
        //                    OthersFollowersCount = othersFollowersCount,
        //                    OthersFollowingsCount = othersFollowingsCount,
        //                    Success = true,
        //                    FollowState = AccountAdapter.GetFollowFollowingRelState(param.following, param.follower),
        //                    Message = param.isValid ? "关注成功！" : "取消关注成功！"
        //                };

        //                var msgList = MessageAdapter.GetUnReadMessageList(param.following, 5);
        //                if (msgList != null && msgList.Count != 0)
        //                {
        //                    var msg = msgList[msgList.Count - 1];
        //                    MessageAdapter.SendAppNotice(new SendNoticeEntity()
        //                    {
        //                        actionUrl = "whotelapp://www.zmjiudian.com/msglist?userid={userid}&realuserid=1",
        //                        title = "周末酒店",
        //                        msg = string.Format("{0}关注了你", msg.sendNickName),
        //                        appType = 0,
        //                        from = msg.sendNickName,
        //                        noticeType = ZMJDNoticeType.follow,
        //                        userID = param.following
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                result = new ChangeFollowerFollowingResult()
        //                {
        //                    FollowersCount = 0,
        //                    Success = false,
        //                    FollowState = 0,
        //                    Message = "该账号不存在！"
        //                };
        //            }
        //        }
        //    }
        //    else
        //    {
        //        result = new ChangeFollowerFollowingResult()
        //        {
        //            FollowersCount = 0,
        //            Success = false,
        //            FollowState = 0,
        //            Message = "验证错误，请检查参数！"
        //        };
        //    }
        //    return result;
        //}
        /// <summary>
        /// 更新关注状态new 
        /// </summary>
        /// <param name="follower">粉丝UserID</param>
        /// <param name="following">被关注者UserID</param>
        /// <param name="IsValid">关注状态有效无效</param>
        /// <returns></returns>
        //[HttpGet]
        //public ChangeFollowerFollowingResult UpdateFollowerFollowingRelGetNew([FromUri]ChangeFollowerFollowingParam param)
        //{
        //    ChangeFollowerFollowingResult result = null;
        //    if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
        //    {
        //        if (param.follower * param.following == 0)
        //        {
        //            result = new ChangeFollowerFollowingResult()
        //            {
        //                FollowersCount = 0,
        //                Success = false,
        //                FollowState = 0,
        //                Message = "关注人和被关注人UserID不能为0"
        //            };
        //        }
        //        else if (param.follower == param.following)
        //        {
        //            result = new ChangeFollowerFollowingResult()
        //            {
        //                FollowersCount = 0,
        //                Success = false,
        //                FollowState = 0,
        //                Message = "自己不能关注自己"
        //            };
        //        }
        //        else
        //        {
        //            MemberProfileInfo memberProfileInfo = AccountAdapter.GetCurrentUserInfo(param.following);
        //            if (memberProfileInfo != null && memberProfileInfo.UserID > 0)
        //            {
        //                AccountAdapter.UpdateFollowerFollowingRel(param.follower, param.following, param.isValid);

        //                int followersCount = AccountAdapter.GetFollowersCountByUserID(param.follower);
        //                int followingsCount = AccountAdapter.GetFollowingsCountByUserID(param.follower);

        //                int othersFollowersCount = AccountAdapter.GetFollowersCountByUserID(param.following);
        //                int othersFollowingsCount = AccountAdapter.GetFollowingsCountByUserID(param.following);

        //                result = new ChangeFollowerFollowingResult()
        //                {
        //                    FollowersCount = followersCount,
        //                    FollowingsCount = followingsCount,
        //                    OthersFollowersCount = othersFollowersCount,
        //                    OthersFollowingsCount = othersFollowingsCount,
        //                    Success = true,
        //                    FollowState = AccountAdapter.GetFollowFollowingRelState(param.following, param.follower),
        //                    Message = param.isValid ? "关注成功！" : "取消关注成功！"
        //                };

        //                var msgList = MessageAdapter.GetUnReadMessageList(param.following, 5);
        //                if (msgList != null && msgList.Count != 0)
        //                {
        //                    var msg = msgList[msgList.Count - 1];
        //                    MessageAdapter.SendAppNotice(new SendNoticeEntity()
        //                    {
        //                        actionUrl = "whotelapp://www.zmjiudian.com/msglist?userid={userid}&realuserid=1",
        //                        title = "周末酒店",
        //                        msg = string.Format("{0}关注了你", msg.sendNickName),
        //                        appType = 0,
        //                        from = msg.sendNickName,
        //                        noticeType = ZMJDNoticeType.follow,
        //                        userID = param.following
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                result = new ChangeFollowerFollowingResult()
        //                {
        //                    FollowersCount = 0,
        //                    Success = false,
        //                    FollowState = 0,
        //                    Message = "该账号不存在！"
        //                };
        //            }
        //        }
        //    }
        //    else
        //    {
        //        result = new ChangeFollowerFollowingResult()
        //        {
        //            FollowersCount = 0,
        //            Success = false,
        //            FollowState = 0,
        //            Message = "验证错误，请检查参数！"
        //        };
        //    }
        //    return result;
        //} 
        #endregion

        /// <summary>
        /// 新增关注
        /// </summary>
        /// <param name="follower">粉丝UserID</param>
        /// <param name="following">被关注者UserID</param>
        /// <returns></returns>
        [HttpPost]
        public ChangeFollowerFollowingResult AddFollowerFollowingRel(ChangeFollowerFollowingParam param)
        {
            ChangeFollowerFollowingResult result = null;
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                if (param.follower * param.following == 0)
                {
                    result = new ChangeFollowerFollowingResult()
                    {
                        FollowersCount = 0,
                        Success = false,
                        FollowState = 0,
                        Message = "关注人和被关注人UserID不能为0"
                    };
                }
                else if (param.follower == param.following)
                {
                    result = new ChangeFollowerFollowingResult()
                    {
                        FollowersCount = 0,
                        Success = false,
                        FollowState = 0,
                        Message = "自己不能关注自己"
                    };
                }
                else
                {
                    AccountAdapter.AddFollowerFollowingRel(param.follower, param.following);

                    int followersCount = AccountAdapter.GetFollowersCountByUserID(param.follower);
                    int followingsCount = AccountAdapter.GetFollowingsCountByUserID(param.follower);

                    int othersFollowersCount = AccountAdapter.GetFollowersCountByUserID(param.following);
                    int othersFollowingsCount = AccountAdapter.GetFollowingsCountByUserID(param.following);

                    result = new ChangeFollowerFollowingResult()
                    {
                        FollowersCount = followersCount,
                        FollowingsCount = followingsCount,
                        OthersFollowersCount = othersFollowersCount,
                        OthersFollowingsCount = othersFollowingsCount,
                        Success = true,
                        FollowState = AccountAdapter.GetFollowFollowingRelState(param.following, param.follower),//followers.Exists(_ => _.Follower == param.follower) ? (followings.Exists(_ => _.Following == param.follower) ? 3 : 2) : (followings.Exists(_ => _.Following == param.follower) ? 1 : 0)
                        Message = "关注成功！"
                    };
                }
            }
            else
            {
                result = new ChangeFollowerFollowingResult()
                {
                    FollowersCount = 0,
                    Success = false,
                    FollowState = 0,
                    Message = "验证错误，请检查参数！"
                };
            }
            return result;
        }

        /// <summary>
        /// 获取关注列表
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="curUserID"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public List<FollowerFollowingRelItem> GetFollowings(long userID, long curUserID = 0, int start = 0, int count = 1000)
        {
            List<FollowerFollowingRelEntity> dataList = AccountAdapter.GetFollowingsByUserID(userID, start, count == 0 ? 1000 : count);
            List<FollowerFollowingRelItem> resultList = new List<FollowerFollowingRelItem>();
            dataList.ForEach(_ =>
            {
                resultList.Add(new FollowerFollowingRelItem()
                {
                    FollowState = AccountAdapter.GetFollowFollowingRelState(_.Following, curUserID == 0 ? AppUserID > 0 ? AppUserID : userID : curUserID),//followers.Exists(j => j.Follower == _.Following) ? (followings.Exists(j => j.Following == _.Following) ? 3 : 1) : (followings.Exists(j => j.Following == _.Following) ? 2 : 0)
                    NickName = _.FollowingNickName,
                    UserID = _.Following,
                    AvatarUrl = String.IsNullOrWhiteSpace(_.FollowingAvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.FollowingAvatarUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter)
                });
            });
            return resultList;
        }

        /// <summary>
        /// 获取粉丝列表
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="curUserID"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public List<FollowerFollowingRelItem> GetFollowers(long userID, long curUserID = 0, int start = 0, int count = 1000)
        {
            List<FollowerFollowingRelEntity> dataList = AccountAdapter.GetFollowersByUserID(userID, start, count == 0 ? 1000 : count);
            List<FollowerFollowingRelItem> resultList = new List<FollowerFollowingRelItem>();
            dataList.ForEach(_ =>
            {
                resultList.Add(new FollowerFollowingRelItem()
                {
                    FollowState = AccountAdapter.GetFollowFollowingRelState(_.Follower, curUserID == 0 ? AppUserID > 0 ? AppUserID : userID : curUserID),//followers.Exists(j => j.Follower == _.Follower) ? (followings.Exists(j => j.Following == _.Follower) ? 3 : 1) : (followings.Exists(j => j.Following == _.Following) ? 2 : 0),
                    NickName = _.FollowerNickName,
                    UserID = _.Follower,
                    AvatarUrl = String.IsNullOrWhiteSpace(_.FollowerAvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.FollowerAvatarUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter)
                });
            });
            return resultList;
        }

        /// <summary>
        /// 更新个性签名
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public AccountMemberBriefResult UpdatePersonalSignature(AccountMemberBriefParam param)
        {
            AccountMemberBriefResult result = null;
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                string nonSpaceSignature = string.IsNullOrWhiteSpace(param.personalSignature) ? "" : param.personalSignature.Replace("\r", "").Replace("\n", "").Trim();
                int maxLength = 80;

                if (nonSpaceSignature.Length > maxLength)
                {
                    return result = new AccountMemberBriefResult()
                    {
                        Success = false,
                        Message = "个性签名最多写" + maxLength + "个字哦，请再想想~",
                        PersonalSignature = param.personalSignature
                    };
                }

                string utf8_Signature = System.Web.HttpUtility.UrlEncode(nonSpaceSignature, System.Text.Encoding.UTF8);

                AccountAdapter.UpdatePersonalSignature(param.userID, utf8_Signature);
                return result = new AccountMemberBriefResult()
                {
                    Success = true,
                    Message = "更新签名成功！",
                    PersonalSignature = param.personalSignature
                };
            }
            else
            {
                return new AccountMemberBriefResult()
                {
                    Success = false,
                    Message = "验证签名错误",
                    PersonalSignature = param.personalSignature
                };
            }
        }

        /// <summary>
        /// 更新品鉴师 App首页推荐关注 介绍图片
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="desPicUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public InspectorDescPicSurlResult UpdateInspectorDesPicSurl(InspectorDescPicSurlParam param)
        {
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                AccountAdapter.UpdateInspectorDesPicSurl(param.userID, param.desPicUrl);
                return new InspectorDescPicSurlResult()
                {
                    DescPicSurl = param.desPicUrl,
                    Success = true,
                    Message = "更新成功！"
                };
            }
            else
            {
                return new InspectorDescPicSurlResult()
                {
                    DescPicSurl = param.desPicUrl,
                    Success = false,
                    Message = "签名验证失败！"
                };
            }
        }

        /// <summary>
        /// 更新用户主题封面
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public BasePostResult UpdateUserThemeCover(PersonalThemeParam param)
        {
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                AccountAdapter.UpdateUserThemeCover(new User_Info_Theme()
                {
                    ThemeCodeSN = param.themeCodeSN,
                    ThemeName = param.themeName,
                    Type = param.type,
                    UserID = param.userID
                });
                return new BasePostResult()
                {
                    Success = true,
                    Message = "更新主题成功！"
                };
            }
            else
            {
                return new BasePostResult()
                {
                    Success = false,
                    Message = "签名验证失败！"
                };
            }
        }

        /// <summary>
        /// 用户未读状态的消息数量
        /// </summary>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        [HttpGet]
        public UserMsgResult GetSysMsgUnReadCount(long curUserID)
        {
            int unReadCount = MessageAdapter.GetSysMessageEntityCount(curUserID, 0);
            return new UserMsgResult()
            {
                unReadCount = unReadCount
            };
        }

        /// <summary>
        /// 当前用户ID
        /// </summary>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        [HttpGet]
        public UserMsgListResult GetUserMsgList([FromUri]UserMsgSearchParam param)
        {
            List<SysMsgItem> msgList = new List<SysMsgItem>();
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                int totalCount = MessageAdapter.GetSysMessageEntityCount(param.curUserID, -1);//全部消息的数量
                if (totalCount > 0)
                {
                    var list = MessageAdapter.GetSysMessageEntityList(new MessageSearchParam()
                    {
                        receiver = param.curUserID,
                        state = -1,
                        messageType = 0,
                        start = param.start,
                        count = param.count
                    });
                    msgList = MessageAdapter.GenSysMsgItemListBatch(list);//切换到按批次查询
                }
                return new UserMsgListResult()
                {
                    totalCount = totalCount,
                    items = msgList
                };
            }
            else
            {
                return new UserMsgListResult()
                {
                    totalCount = 0,
                    items = msgList
                };
            }
        }

        /// <summary>
        /// 设置评论回复状态
        /// 如果id > 0 则更新该条消息为已阅读; receiver > 0 则更新该用户的所有消息已查看
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public BasePostResult UpdateSysMessageState(UserMsgUpdateParam param)
        {
            if (Signature.IsRightSignature(param.TimeStamp, param.SourceID, param.RequestType, param.Sign))
            {
                //curUserID不等于0 且 ids数组有0的
                if (param.curUserID > 0 && param.ids.Count == 1 && param.ids[0] == 0)
                {
                    MessageAdapter.UpdateSysMessage(new SysMessageEnitity()
                    {
                        id = 0,
                        state = 1,
                        receiver = param.curUserID
                    });
                }
                else if (param.ids != null && param.ids.Count != 0)
                {
                    foreach (var id in param.ids)
                    {
                        MessageAdapter.UpdateSysMessage(new SysMessageEnitity()
                        {
                            id = id,
                            state = 1,
                            receiver = 0
                        });
                    }
                }
                else
                {
                    return new UpdateUserMsgState()
                    {
                        Success = false,
                        Message = "缺少参数，无法更新消息状态！",
                        unReadCount = 0
                    };
                }
                return new UpdateUserMsgState()
                {
                    Success = true,
                    Message = "设置成功！",
                    unReadCount = GetSysMsgUnReadCount(param.curUserID).unReadCount
                };
            }
            else
            {
                return new UpdateUserMsgState()
                {
                    Success = false,
                    Message = "验证签名错误",
                    unReadCount = 0
                };
            }
        }

        /// <summary>
        /// 等待测试让铂汇调用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public BasePostResponse SynExternalUserInfo(ThirdPartyUserInfoParam param)
        {
            var result = new BasePostResponse();
            try
            {
                int maxUserCount = 100;
                if (param != null && param.userinfos != null && param.userinfos.Count != 0 && !string.IsNullOrWhiteSpace(param.merchantcode))
                {
                    Dictionary<string, string> dicKeyVal = new Dictionary<string, string>();
                    dicKeyVal.Add("userinfos", JsonConvert.SerializeObject(param.userinfos));
                    dicKeyVal.Add("noncestr", param.noncestr);
                    dicKeyVal.Add("timestamp", param.timestamp.ToString());
                    dicKeyVal.Add("extrefnumber", param.extrefnumber);
                    dicKeyVal.Add("merchantcode", param.merchantcode);

                    string resultSign = "";
                    if (!Signature.VerifySignFromBoTao(dicKeyVal, true, param.sign, out resultSign))
                    {
                        result = new BasePostResponse()
                        {
                            result = false,
                            data = "",
                            errorcode = "10002",
                            errormsg = "签名验证失败！"
                        };
                    }
                    else if (param.userinfos.Count > maxUserCount)
                    {
                        result = new BasePostResponse()
                        {
                            result = false,
                            data = "",
                            errorcode = "10003",
                            errormsg = string.Format("同步数据过多，请限制单次调用传递的用户数不超过{0}！", maxUserCount)
                        };
                    }
                    else
                    {
                        //ToDo 插入新的第三方导入用户记录
                        //var strBuilder = new StringBuilder();
                        foreach (var userinfo in param.userinfos)
                        {
                            var phone = !string.IsNullOrWhiteSpace(userinfo.mobilephone) ? DES.Decrypt(userinfo.mobilephone, DES.bohuijinrongDESKey) : "";
                            var name = DES.Decrypt(userinfo.truename, DES.bohuijinrongDESKey);
                            var identity = DES.Decrypt(userinfo.identitycard, DES.bohuijinrongDESKey);

                            //strBuilder.Append(phone + ",");

                            AccountAdapter.RegisterPhoneUser(phone, "", 1);
                        }

                        //var result = strBuilder.ToString().TrimEnd(",".ToCharArray());
                        Log.WriteLog("收到了铂汇金融的数据，内容为:" + JsonConvert.SerializeObject(param.userinfos));
                        //BasePostResponse bpr = AccountAdapter.RegisterUser4BT(param.mobileStr, 0, "");
                        result = new BasePostResponse()
                        {
                            result = true,
                            data = "",
                            errorcode = "0",
                            errormsg = "同步数据成功！"
                        };
                    }
                }
                else
                {
                    result = new BasePostResponse()
                    {
                        result = false,
                        data = "",
                        errorcode = "10001",
                        errormsg = "参数不能为空！"
                    };
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.WriteLog("记录铂韬的错误信息:" + ex.Message + ex.StackTrace);
                return new BasePostResponse()
                {
                    result = false,
                    data = "",
                    errorcode = "10004",
                    errormsg = "系统出现异常！"
                };
            }
        }

        [HttpPost]
        public BasePostResponse ModifyExternalUserMobile(ThirdPartyModifyPhoneParam param)
        {
            if (param != null && !string.IsNullOrWhiteSpace(param.oldphone)
                && !string.IsNullOrWhiteSpace(param.newphone) && !string.IsNullOrWhiteSpace(param.merchantcode))
            {
                Dictionary<string, string> dicKeyVal = new Dictionary<string, string>();
                dicKeyVal.Add("oldphone", param.oldphone);
                dicKeyVal.Add("newphone", param.newphone);
                dicKeyVal.Add("merchantcode", param.merchantcode);
                dicKeyVal.Add("noncestr", param.noncestr);
                dicKeyVal.Add("timestamp", param.timestamp.ToString());
                dicKeyVal.Add("extrefnumber", param.extrefnumber);

                string resultSign = "";
                if (!Signature.VerifySignFromBoTao(dicKeyVal, true, param.sign, out resultSign))
                {
                    return new BasePostResponse()
                    {
                        result = false,
                        data = "",
                        errorcode = "10002",
                        errormsg = "签名验证失败！"
                    };
                }
                else
                {
                    var newPhone = DES.Decrypt(param.newphone, DES.bohuijinrongDESKey);
                    var oldPhone = DES.Decrypt(param.oldphone, DES.bohuijinrongDESKey);

                    OperationResult ret = ModifyUserPhoneNo(0, newPhone, oldPhone);

                    if (ret.Success)
                    {
                        Log.WriteLog(string.Format("收到了铂汇金融修改手机号的数据，老手机号:{0}，新手机号{1}", oldPhone, newPhone));
                        return new BasePostResponse()
                        {
                            result = true,
                            data = "",
                            errorcode = "0",
                            errormsg = "修改手机号成功！"
                        };
                    }
                    else
                    {
                        return new BasePostResponse()
                        {
                            result = false,
                            data = "",
                            errorcode = "1",
                            errormsg = ret.Message
                        };
                    }
                }
            }
            else
            {
                return new BasePostResponse()
                {
                    result = false,
                    data = "",
                    errorcode = "10001",
                    errormsg = "参数不能为空！"
                };
            }
        }

        #region account test
        [HttpGet]
        public BasePostResponse TestVisit()
        {
            return new BasePostResponse()
            {
                data = "",
                errorcode = "110",
                errormsg = "msg from zmjd"
            };
        }
        #endregion

        [HttpPost]
        public OperationResult RegisterWithoutPassword(RegistPhoneUserItem r)
        {
            return AccountAdapter.RegisterWithoutPassword(r.Phone, 1, r.CID, r.Unionid);
        }

        //[HttpPost]
        //public OperationResult InsertOrDeleteUserPrivilegeRel(UserPriviledgeInsertParam param)
        //{
        //    var isSuccess = AccountAdapter.InsertOrDeleteUserPrivilegeRel(param.UserId, param.PriviledgeId, param.IsAdd);
        //    return new OperationResult()
        //    {
        //        Success = isSuccess,
        //        UserID = param.UserId
        //    };
        //}

        [HttpGet]
        public CheckZMJDMemberResponse IsZMJDMember([FromUri]long userId, [FromUri]string phoneNum)
        {
            if (userId == 0 && string.IsNullOrWhiteSpace(phoneNum))
            {
                return new CheckZMJDMemberResponse()
                {
                    Success = HJDAPI.Models.BaseResponse.ResponseSuccessState.ParamError,//  200,
                    Message = "缺少用户信息参数",
                    isMember = false
                };
            }
            else
            {
                if (userId == 0)
                {
                    userId = AccountAdapter.GetPhoneUser(phoneNum).UserId;//获取手机号对应的用户ID
                }
                var result = AccountAdapter.HasUserPriviledge(userId, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.MemberPrice);
                return new CheckZMJDMemberResponse()
                {
                    Success = 0,
                    Message = "操作成功",
                    isMember = result
                };
            }
        }

        [HttpGet]
        public void GetExpirePointsTips()
        {
            try
            {
                List<PointsEntity> listPoints = AccountAdapter.GetExpirePointsEntity();
                Log.WriteLog("GetExpirePointsTips " + DateTime.Now + "  count：" + listPoints.Count);
                string msg = "";
                foreach (PointsEntity item in listPoints)
                {
                    msg = "您有" + item.LeavePoint + "积分三个月内到期，请尽快兑换好礼！";
                    if (item.PhoneNo == "15502126909" || item.PhoneNo == "18021036971")
                    {
                        MessageAdapter.InsertSysMessage(new SysMessageEnitity()
                        {
                            state = 0,
                            businessID = item.UserID,
                            businessType = (int)ZMJDNoticeType.PointsExpire,
                            receiver = item.UserID,
                            sendNickName = "",
                            CreateTime = DateTime.Now
                        });

                        MessageAdapter.SendAppNotice(new SendNoticeEntity()
                        {
                            actionUrl = "http://www.zmjiudian.com/Coupon/MoreList/1/0/0?albumId=22",
                            appType = 0,
                            from = "周末酒店",
                            msg = msg,
                            noticeType = ZMJDNoticeType.PointsExpire,
                            title = "积分过期提醒",
                            userID = item.UserID
                        });
                        SMSAdapter.SendSMS(item.PhoneNo, msg + "http://dm12.me/kGvx");
                        Log.WriteLog("积分到期提醒推送 ： " + item.UserID + "积分：" + msg);
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("GetExpirePointsTips 报错：" + e);
            }
        }

        /// <summary>
        /// 新增收件信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponseEx AddReceivePeopleInformation(ReceivePeopleInformationEntity param)
        {
            BaseResponseEx result = new BaseResponseEx();
            int i = AccountAdapter.AddReceivePeopleInformation(param);

            result.BizID = i;
            result.Success = i > 0 ? HJDAPI.Models.BaseResponse.ResponseSuccessState.Success : HJDAPI.Models.BaseResponse.ResponseSuccessState.Failed;
            result.Message = i > 0 ? "添加成功" : "添加失败";
            return result;
        }

        /// <summary>
        /// 修改收件信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public BasePostResult UpdateReceivePeopleInformation(ReceivePeopleInformationEntity param)
        {
            BasePostResult result = new BasePostResult();
            int i = AccountAdapter.UpdateReceivePeopleInformation(param);
            result.Success = i > 0 ? true : false;
            result.Message = i > 0 ? "修改成功" : "修改失败";
            return result;
        }

        /// <summary>
        /// 删除收件信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public BasePostResult DelReceivePeopleInformation(int id)
        {
            BasePostResult result = new BasePostResult();
            result.Success = false;
            result.Message = "删除失败";
            ReceivePeopleInformationEntity model = AccountAdapter.GetReceivePeopleInformationById(id);
            if (model != null)
            {
                model.State = 1;
                int i = AccountAdapter.UpdateReceivePeopleInformation(model);
                result.Success = i > 0 ? true : false;
                result.Message = i > 0 ? "删除成功" : "删除失败";
            }
            return result;
        }


        /// <summary>
        /// 获取用户的收件信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<ReceivePeopleInformationEntity> GetReceivePeopleInformation(long userId)
        {
            return AccountAdapter.GetReceivePeopleInformationByUserId(userId);
        }


        /// <summary>
        /// 新增CID关联
        /// </summary>
        /// <param name="uc"></param>
        /// <returns></returns>
        [HttpPost]
        public OperationResult AddUserChannelRelHistory(UserChannelRelHistoryEntity uc)
        {
            var result = new OperationResult();

            int i = AccountAdapter.AddUserChannelRelHistory(uc);

            result.Success = i > 0;

            return result;
        }

        #region wxapp acount api

        protected static string APISiteUrl = System.Configuration.ConfigurationManager.AppSettings["APISiteUrl"];

        /// <summary>
        /// 获取指定手机号的用户ID，如果不存在该手机号用户，则创建一个
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public GetOrRegistPhoneUserResponse GetOrRegPhoneUserForWxapp(GetOrRegistPhoneUserRequestParams p)
        {
            GetOrRegistPhoneUserResponse r = new GetOrRegistPhoneUserResponse();

            r.UserID = AccountAdapter.GetOrRegistPhoneUser(p.Phone, p.CID).UserId;

            return r;
        }

        [HttpGet]
        public bool sendConfirmSMSForWxapp(string phoneNum, Int64 TimeStamp = 0, int SourceID = 0, string RequestType = "", string sign = "")
        {
            #region 行为记录

            try
            {
                var value = string.Format("{{\"phoneNum\":\"{0}\",\"TimeStamp\":\"{1}\",\"SourceID\":\"{2}\",\"RequestType\":\"{3}\",\"sign\":\"{4}\"}}", phoneNum, TimeStamp, SourceID, RequestType, sign);
                RecordBehavior("sendConfirmSMSForWxapp", value);
            }
            catch (Exception ex) { }

            #endregion

            //if ((TimeStamp == 0 && SourceID == 0 && RequestType == "" && sign == "") || (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign)))
            //{
            //    return AccountAdapter.sendConfirmSMS(phoneNum);

            //    //string url = APISiteUrl + "api/accounts/sendConfirmSMS";
            //    //string postDataStr = string.Format("phoneNum={0}", phoneNum);

            //    //CookieContainer cc = new CookieContainer();
            //    //string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            //    //return JsonConvert.DeserializeObject<bool>(json);
            //}
            //else
            //{
            //    return false;
            //}

            Log.WriteLog("sendConfirmSMSForWxapp:" + phoneNum + ":" + HttpContext.Current.Request.UserHostAddress);
            return AccountAdapter.sendConfirmSMS(phoneNum);
        }

        [HttpGet]
        public bool checkConfirmSMSForWxapp(string phoneNum, string confirmCode)
        {
            string url = APISiteUrl + "api/accounts/checkConfirmSMS";
            string postDataStr = string.Format("phoneNum={0}&confirmCode={1}", phoneNum, confirmCode);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);

            //return AccountAdapter.checkConfirmSMS(phoneNum, confirmCode);
        }

        #endregion

        #region 活动粉丝相关操作

        /// <summary>
        /// 获取指定Unionid的粉丝关联信息
        /// </summary>
        /// <param name="unionid"></param>
        /// <returns></returns>
        [HttpGet]
        public UserFansRel GetOneFansRelByUnionid(string unionid)
        {
            return AccountAdapter.GetOneFansRelByUnionid(unionid);
        }

        /// <summary>
        /// 查询指定用户的指定Unionid粉丝关联信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="unionid"></param>
        /// <returns></returns>
        [HttpGet]
        public UserFansRel GetOneUserRelFans(long userId, string unionid)
        {
            return AccountAdapter.GetOneUserRelFans(userId, unionid);
        }

        /// <summary>
        /// 新增用户活动粉丝关联
        /// </summary>
        /// <param name="userfansrel"></param>
        /// <returns></returns>
        [HttpPost]
        public OperationResult AddUserFansRel(UserFansRel userfansrel)
        {
            var result = new OperationResult();

            int i = AccountAdapter.AddUserFansRel(userfansrel);

            result.Success = i > 0;

            return result;
        }

        /// <summary>
        /// 查询指定用户的所有活动粉丝关联信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<UserFansRel> GetUserRelFans(long userId)
        {
            return AccountAdapter.GetUserRelFans(userId);
        }

        /// <summary>
        /// 获取指定活动来源产生的报表数据（主要包含哪些分销获取了多少粉丝等信息）
        /// </summary>
        /// <param name="sourceType">来源类型（如 1活动海报）</param>
        /// <param name="sourceId">来源ID</param>
        /// <returns></returns>
        [HttpGet]
        public List<UserFansRelReport> GetUserRelFansReportBySourceId(int sourceType, int sourceId)
        {
            return AccountAdapter.GetUserRelFansReportBySourceId(sourceType, sourceId);
        }

        /// <summary>
        /// 获取指定活动来源产生的活动粉丝总数量
        /// </summary>
        /// <param name="sourceType">来源类型（如 1活动海报）</param>
        /// <param name="sourceId">来源ID</param>
        /// <returns></returns>
        [HttpGet]
        public int GetUserRelFansCountBySourceId(int sourceType, int sourceId)
        {
            return AccountAdapter.GetUserRelFansCountBySourceId(sourceType, sourceId);
        }

        #endregion
    }
}