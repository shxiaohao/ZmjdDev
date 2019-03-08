using HJD.AccountServices.Entity;
using HJDAPI.Common.Helpers;
using HJDAPI.Common.Security;
using HJDAPI.Controllers;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class account : BaseProxy
    {

        public static OperationResult OAuthLogin(AccountInfoItem item)
        {
            OperationResult info = null;
            if (item.Uid.Trim().Length != 0)
            {

                string url = APISiteUrl + "api/accounts/OAuthLogin";
                    CookieContainer cc = new CookieContainer();
                    string json = HttpRequestHelper.PostJson(url, item, ref cc);
                    info = JsonConvert.DeserializeObject<OperationResult>(json); 
            }
            return info;

        }
        public static List<UserChannelRelHistoryEntity> GetChannelBelongToUserList(long cid)
        {
            string url = APISiteUrl + "api/accounts/GetChannelBelongToUserList";
            string postDataStr = string.Format("cid={0}", cid);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<UserChannelRelHistoryEntity>>(json);
        }
         public static List<UserBindTypeEntity> GetUserBindTypeList()
         { 
             string url = APISiteUrl + "api/accounts/GetUserBindTypeList";
             CookieContainer cc = new CookieContainer();
             string json = HttpRequestHelper.Get(url, "", ref cc);
             return JsonConvert.DeserializeObject<List<UserBindTypeEntity>>(json); 
         }
          

        public static List<HJD.AccountServices.Entity.UserRole> GetUserRoleRelByUserId(UserRole p)
        {
            List<HJD.AccountServices.Entity.UserRole> info = null;
            if (p.UserID != 0)
            {
                if (IsProductEvn)
                {
                    info = AccountAdapter.GetUserRoleRelByUserId(p.UserID);
                }
                else
                {
                    string url = APISiteUrl + "api/accounts/GetUserRoleRelByUserId";
                    CookieContainer cc = new CookieContainer();
                    string json = HttpRequestHelper.PostJson(url, p, ref cc);
                    info = JsonConvert.DeserializeObject<List<HJD.AccountServices.Entity.UserRole>>(json);
                }
            }
            return info;
        }

        public static UserChannelRelEntity GetOneUserChannelRel(UserChannelRelEntity userchannelrel)
        {
            string url = APISiteUrl + "api/accounts/GetOneUserChannelRel";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, userchannelrel, ref cc);

            UserChannelRelEntity result = JsonConvert.DeserializeObject<UserChannelRelEntity>(json);

            return result;
        }

        public static UserChannelRelEntity GetOneUserChannelRelByUID(UserChannelRelEntity userchannelrel)
        {
            string url = APISiteUrl + "api/accounts/GetOneUserChannelRelByUID";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, userchannelrel, ref cc);

            UserChannelRelEntity result = JsonConvert.DeserializeObject<UserChannelRelEntity>(json);

            return result;
        }

        public static OperationResult AddUserChannelRel(UserChannelRelEntity userchannelrel)
        {
            string url = APISiteUrl + "api/accounts/AddUserChannelRel";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, userchannelrel, ref cc);

            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);

            return result;
        }


        public static long GetOrRegistPhoneUser(string Phone, long CID =0)
        {
            if (IsProductEvn)
                return AccountAdapter.GetOrRegistPhoneUser(Phone, CID).UserId;
            else
            {
                string url = APISiteUrl + "api/Accounts/GetOrRegistPhoneUser";
                //string postDataStr = string.Format("districtid={0}"
                //  , districtid);

                GetOrRegistPhoneUserRequestParams p = new GetOrRegistPhoneUserRequestParams();

                p.Phone = Phone; 
                p.CID = CID;
                Signature.SignBaseParam(p,Phone,3);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.PostJson(url, p, ref cc);
                GetOrRegistPhoneUserResponse resp = JsonConvert.DeserializeObject<GetOrRegistPhoneUserResponse>(json);

                return resp.UserID;
            }
        }

        public static OperationResult LoginForBGMember(AccountInfoItem ai)
        {
            string url = APISiteUrl + "api/accounts/LoginForBGMember";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, ai, ref cc);

            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);

            return result;
        }

        public static Boolean IsVIPCustomer(AccountInfoItem ai)
        {
            string url = APISiteUrl + "api/accounts/IsVIPCustomer";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, ai, ref cc);
            return JsonConvert.DeserializeObject<Boolean>(json);
        }

        public static List<UserPrivilegeRel> GetAllPrivilegeByUserId(AccountInfoItem ai)
        {
            string url = APISiteUrl + "api/accounts/GetAllPrivilegeByUserId";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, ai, ref cc);
            return JsonConvert.DeserializeObject<List<UserPrivilegeRel>>(json);
        }

        public static int GetVIPTypeNum(AccountInfoItem ai)
        {
            string url = APISiteUrl + "api/accounts/GetVIPTypeNum";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, ai, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /***登录 注册开始***/
        public static OperationResult MobileLogin(AccountInfoItem ai)
        {
            string url = APISiteUrl + "api/accounts/MobileLogin";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, ai, ref cc);

            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);

            return result;
        }

        public static OperationResult Register(RegistPhoneUserItem item)
        {
            string url = APISiteUrl + "api/accounts/RegistPhoneUser";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, item, ref cc);

            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);

            return result;
        }
        /***登录 注册结束***/

        /***修改密码开始***/
        public static OperationResult ResetPasswordWithPhone(ResetPasswordItem r)
        {
            string url = APISiteUrl + "api/accounts/ResetPasswordWithPhone";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, r, ref cc);

            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);
            return result;
        }
        /***修改密码结束***/

        /***修改昵称开始***/
        /// <summary>
        /// 昵称可用：没有人用过，昵称没有包括特殊词
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public static ResultEntity CheckNickName(UserNickNameModel u)
        {
            string url = APISiteUrl + "api/accounts/CheckNickName";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, u, ref cc);
            ResultEntity result = JsonConvert.DeserializeObject<ResultEntity>(json);
            return result;
        }

        /// <summary>
        /// 更新用户昵称
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static ResultEntity UpdateNickName(UserNickNameModel u)
        {
            string url = APISiteUrl + "api/accounts/UpdateNickName";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, u, ref cc);
            ResultEntity result = JsonConvert.DeserializeObject<ResultEntity>(json);
            return result;
        }
        /***修改昵称结束***/

        /// <summary>
        /// 发送用来修改密码的手机验证码
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static OperationResult SendResetPasswordWithPhoneConfirmCode(ResetPasswordItem r)
        {
            string url = APISiteUrl + "api/accounts/SendResetPasswordWithPhoneConfirmCode";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, r, ref cc);
            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);
            return result;
        }

        /// <summary>
        /// 修改手机
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static OperationResult ModifyUserPhone(ModifyUserPhoneItem m)
        {
            string url = APISiteUrl + "api/accounts/ModifyUserPhone";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, m, ref cc);
            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);
            return result;
        }

        public static OperationResult SendModifyUserPhoneConfirmCode(ModifyUserPhoneItem m)
        {
            string url = APISiteUrl + "api/accounts/SendModifyUserPhoneConfirmCode";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, m, ref cc);
            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);
            return result;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static OperationResult ModifyPassword(ModifyPasswordItem r)
        {
            string url = APISiteUrl + "api/accounts/ModifyPassword";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, r, ref cc);
            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);
            return result;
        }

        /// <summary>
        /// 添加通用信息（内部包含了更新操作）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool AddUserCommInfo(UserCommInfoEntity entity)
        {
            string url = APISiteUrl + "api/accounts/AddUserCommInfo";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, entity, ref cc);

            bool result = JsonConvert.DeserializeObject<bool>(json);
            return result;
        }

        /***通用API开始***/
        /// <summary>
        /// 手机号是否存在
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool ExistsMobileAccount(ExistsMobileAccountItem item)
        {
            string url = APISiteUrl + "api/accounts/ExistsMobileAccount";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, item, ref cc);

            ExistsMobileAccountResponse result = JsonConvert.DeserializeObject<ExistsMobileAccountResponse>(json);
            return result.ExistsMobileAccount;
        }

        /// <summary>
        /// 获得通用个人信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static List<UserCommInfoEntity> GetUserCommInfo(GetUserCommInfoReqParm p)
        {
            string url = APISiteUrl + "api/accounts/GetUserCommInfo";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, p, ref cc);

            List<UserCommInfoEntity> result = JsonConvert.DeserializeObject<List<UserCommInfoEntity>>(json);
            return result;
        }
        /***验证码开始***/

        /// <summary>
        /// 向手机发送验证短信
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public static bool sendConfirmSMS(string phoneNum, Int64 TimeStamp = 0, int SourceID = 0, string RequestType = "", string sign = "")
        {
            if (IsProductEvn)
                return AccountAdapter.sendConfirmSMS(phoneNum);
            else
            {

                string url = APISiteUrl + "api/accounts/sendConfirmSMS";
                string postDataStr = string.Format("phoneNum={0}&TimeStamp={1}&SourceID={2}&RequestType={3}&sign={4}", phoneNum, TimeStamp, SourceID, RequestType, sign);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<bool>(json);
            }
        }

        /// <summary>
        /// 验证手机验证码是否正确
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public static bool checkConfirmSMS(string phoneNum, string confirmCode)
        {
            if (IsProductEvn)
                return AccountAdapter.checkConfirmSMS(phoneNum, confirmCode);
            else
            {
                string url = APISiteUrl + "api/accounts/checkConfirmSMS";
                string postDataStr = string.Format("phoneNum={0}&confirmCode={1}", phoneNum, confirmCode);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<bool>(json);
            }
        }

        #region 安缦渠道短信通道

        public static bool sendConfirmSMSAnman(string phoneNum)
        {
            if (IsProductEvn)
                return AccountAdapter.sendConfirmSMSAnman(phoneNum);
            else
            {
                string url = APISiteUrl + "api/accounts/sendConfirmSMSAnman";
                string postDataStr = string.Format("phoneNum={0}", phoneNum);

                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<bool>(json);
            }
        }

        public static bool checkConfirmSMSAnman(string phoneNum, string confirmCode)
        {
            if (IsProductEvn)
                return AccountAdapter.checkConfirmSMSAnman(phoneNum, confirmCode);
            else
            {
                string url = APISiteUrl + "api/accounts/checkConfirmSMSAnman";
                string postDataStr = string.Format("phoneNum={0}&confirmCode={1}", phoneNum, confirmCode);

                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
                return JsonConvert.DeserializeObject<bool>(json);
            }
        }

        #endregion

        /***验证码结束***/

        public static MemberProfileInfo GetCurrentUserInfo(long userID)
        {
            MemberProfileInfo info = null;
            if (userID != 0)
            {
                if (IsProductEvn)
                {
                    info = AccountAdapter.GetCurrentUserInfo(userID);
                }
                else
                {
                    GetUserCommInfoReqParm param = new GetUserCommInfoReqParm() { UserID = userID };
                    string url = APISiteUrl + "api/accounts/GetCurrentUserInfo";
                    CookieContainer cc = new CookieContainer();
                    string json = HttpRequestHelper.PostJson(url, param, ref cc);
                    info = JsonConvert.DeserializeObject<MemberProfileInfo>(json);
                }
            }
            return info;
        }
        /***通用API结束***/

        public static User_Info GetUserInfoByMobile(string phone)
        {
            GetUserCommInfoReqParm param = new GetUserCommInfoReqParm() { Phone = phone };
            string url = APISiteUrl + "api/accounts/GetUserInfoByMobile";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<User_Info>(json);
        }

        public static OperationResult RegisterWithoutPassword(RegistPhoneUserItem r)
        {
            string url = APISiteUrl + "api/accounts/RegisterWithoutPassword";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, r, ref cc);

            return JsonConvert.DeserializeObject<OperationResult>(json);
        }

        public static OperationResult InsertOrDeleteUserPrivilegeRel(UserPriviledgeInsertParam param)
        {
            string url = APISiteUrl + "api/accounts/InsertOrDeleteUserPrivilegeRel";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, param, ref cc);

            return JsonConvert.DeserializeObject<OperationResult>(json);
        }

        public static CheckZMJDMemberResponse IsZMJDMember(long userId, string phone)
        {
            string url = APISiteUrl + "api/accounts/IsZMJDMember";
            string getDataStr = string.Format("userId={0}&phoneNum={1}", userId, phone);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, getDataStr, ref cc);
            return JsonConvert.DeserializeObject<CheckZMJDMemberResponse>(json);
        }

        public static UserInfoResult GetUserInfoByUserID(long userId)
        {
            string url = APISiteUrl + "api/accounts/GetUserInfoByUserID";
            string getDataStr = string.Format("userId={0}", userId);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, getDataStr, ref cc);
            return JsonConvert.DeserializeObject<UserInfoResult>(json);
        }
        public static int UpdateRealName(string realName, long userId)
        {
            string url = APISiteUrl + "api/accounts/UpdateRealName";
            string getDataStr = string.Format("realName={0}&userId={1}", realName,userId);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, getDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public static ExistsCodeResponse ExistsSMSCodeAndInvitationCode(ExistsCodeItem codeParam)
        {
            string url = APISiteUrl + "api/accounts/ExistsSMSCodeAndInvitationCode";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, codeParam, ref cc);
            ExistsCodeResponse result = JsonConvert.DeserializeObject<ExistsCodeResponse>(json);
            return result;
        }
        public static OperationResult RegistPhoneUser50(RegistPhoneUserItem registParam)
        {
            string url = APISiteUrl + "api/accounts/RegistPhoneUser50";
            CookieContainer cc = new CookieContainer(); 
            string json = HttpRequestHelper.PostJson(url, registParam, ref cc);
            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);
            return result;
        }

        /// <summary>
        /// 新增CID关联
        /// </summary>
        /// <param name="uc"></param>
        /// <returns></returns>
        public OperationResult AddUserChannelRelHistory(UserChannelRelHistoryEntity uc)
        {
            string url = APISiteUrl + "api/accounts/AddUserChannelRelHistory";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, uc, ref cc);
            OperationResult result = JsonConvert.DeserializeObject<OperationResult>(json);
            return result;
        }

        #region 用户关注相关操作

        /// <summary>
        /// 更新关注状态
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static ChangeFollowerFollowingResult UpdateFollowerFollowingRel(ChangeFollowerFollowingParam param)
        {
            string url = APISiteUrl + "api/accounts/UpdateFollowerFollowingRel";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.PostJson(url, param, ref cc);

            ChangeFollowerFollowingResult result = JsonConvert.DeserializeObject<ChangeFollowerFollowingResult>(json);

            return result;
        }

        #endregion

        #region 活动粉丝相关操作

        /// <summary>
        /// 获取指定Unionid的粉丝关联信息
        /// </summary>
        /// <param name="unionid"></param>
        /// <returns></returns>
        public static UserFansRel GetOneFansRelByUnionid(string unionid)
        {
            string url = APISiteUrl + "api/accounts/GetOneFansRelByUnionid";
            string getDataStr = string.Format("unionid={0}", unionid);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, getDataStr, ref cc);
            return JsonConvert.DeserializeObject<UserFansRel>(json);
        }

        #endregion
    }
}