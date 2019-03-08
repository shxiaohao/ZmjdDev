using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHotelSite.Params.Account;
using HJDAPI.APIProxy;
using HJDAPI.Models;
using WHotelSite.ViewModels;
using HJD.AccountServices.Entity;
using System.Net;
using WHotelSite.Models;
using WHotelSite.Common;
using System.Text;
using HJD.Framework.Encrypt;
using System.Text.RegularExpressions;
using HJDAPI.Common.Security;
using WHotelSite.Filters;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using HJD.WeixinServices.Contracts;
using HJD.WeixinService.Contract;
using GeetestSDK;

namespace WHotelSite.Controllers
{

   // [CrossSite]
    public class AccountController : BaseController
    {
        public static long GetUserId()
        {
            // Int64.Parse(GetStringFromCookie("user", "0"))
            return 0;
        }

        public static void SetUserId(long userId)
        {
            // HttpCookie cookie = new HttpCookie("user", result.UserID.ToString());
            // cookie.Path = "/";
            // cookie.Expires = new DateTime().AddDays(1);
            // Response.Cookies.Add(cookie);
        }

        //[RequireHttps]
        [HttpPost]
        public ActionResult Verify()
        {
            if (Request.Url == Request.UrlReferrer)
            {
                return Json(":)");
            }

            VerifyParam param = new VerifyParam(this);
            //string number = Request.Form["number"];
            //string code = Request.Form["code"];
            var result = new Dictionary<string, bool>();
            if (param.Action == "send")
            {
              //  LogHelper.WriteLog("Verify:" + param.Number + Request.Url + Request.UrlReferrer + " | " + Request.UserAgent + " | " + Request.UserHostAddress);
                var urlReferrer = Request.UrlReferrer.ToString().ToLower();
                if (urlReferrer.StartsWith("https://www.zmjiudian.com")
                    || urlReferrer.StartsWith("http://www.zmjiudian.com")
                    || urlReferrer.StartsWith("http://www.shangjiudian.com")
                    || urlReferrer.StartsWith("http://www.zmjd100.com")
                    || urlReferrer.StartsWith("http://www.shang-ke.com")
                    || urlReferrer.StartsWith("http://www.zmjd123.com")
                    || urlReferrer.StartsWith("http://www.zmjd001.com")
                    || urlReferrer.StartsWith("http://www.shangclub.com")
                    || urlReferrer.StartsWith("http://www.tst.zmjd001.com")
                    || urlReferrer.StartsWith("http://www.dev.zmjd001.com")
                    || urlReferrer.StartsWith("http://www.devtst.zmjd001.com")
                    || urlReferrer.StartsWith("http://192.168.1.22:8081")
                    || urlReferrer.StartsWith("http://192.168.1.188:8081"))
                {
                        account.sendConfirmSMS(param.Number);   
                    }
                result["ok"] = true;
            }
            else if (param.Action == "check")
            {
                result["ok"] = account.checkConfirmSMS(param.Number, param.Code);
            }
            return Json(result);
        }

        //发送验证码 有行为验证
        //[RequireHttps]
        [HttpPost]
        public ActionResult VerifyGeetest()
        {
            if (Request.Url == Request.UrlReferrer)
            {
                return Json(":)");
            }
            var result = new Dictionary<string, bool>();

            VerifyParam param = new VerifyParam(this);
            int geetestreuslt = Geetest(param.Geetest_Challenge, param.Geetest_Seccode, param.Geetest_Validate);
            if (geetestreuslt == 1)
            {
                //string number = Request.Form["number"];
                //string code = Request.Form["code"];
                if (param.Action == "sendAndGeetest")
                {
                    //  LogHelper.WriteLog("Verify:" + param.Number + Request.Url + Request.UrlReferrer + " | " + Request.UserAgent + " | " + Request.UserHostAddress);
                    var urlReferrer = Request.UrlReferrer.ToString().ToLower();
                    if (urlReferrer.StartsWith("https://www.zmjiudian.com")
                        || urlReferrer.StartsWith("http://www.zmjiudian.com")
                        || urlReferrer.StartsWith("http://www.shangjiudian.com")
                        || urlReferrer.StartsWith("http://www.zmjd100.com")
                        || urlReferrer.StartsWith("http://www.shang-ke.com")
                        || urlReferrer.StartsWith("http://www.zmjd123.com")
                        || urlReferrer.StartsWith("http://www.zmjd001.com")
                        || urlReferrer.StartsWith("http://www.shangclub.com")
                        || urlReferrer.StartsWith("http://www.tst.zmjd001.com")
                        || urlReferrer.StartsWith("http://www.dev.zmjd001.com")
                        || urlReferrer.StartsWith("http://www.devtst.zmjd001.com")
                        || urlReferrer.StartsWith("http://192.168.1.22:8081"))
                    {
                        account.sendConfirmSMS(param.Number);
                    }
                    result["ok"] = true;
                }
            }
            else
            {
                result["ok"] = false;
            }
            return Json(result);
        }
        /// <summary>
        /// 极验初始化
        /// </summary>
        /// <returns></returns>
        public string GetCaptcha()
        {
            string geetestID = System.Configuration.ConfigurationManager.AppSettings["geetestID"];
            string geetestKey = System.Configuration.ConfigurationManager.AppSettings["geetestKey"];
            GeetestLib geetest = new GeetestLib(geetestID, geetestKey);
            String userID = "test";
            string userIP = Request.ServerVariables.Get("Remote_Addr").ToString();
            Byte gtServerStatus = geetest.preProcess(userID, "web", userIP);
            Session[GeetestLib.gtServerStatusSessionKey] = gtServerStatus;
            Session["userID"] = userID;
            return geetest.getResponseStr();
        }
        /// <summary>
        /// 极验二次验证
        /// </summary>
        /// <returns></returns>
        public int Geetest(string geetest_challenge,string geetest_seccode,string geetest_validate)
        {
            string geetestID = System.Configuration.ConfigurationManager.AppSettings["geetestID"];
            string geetestKey = System.Configuration.ConfigurationManager.AppSettings["geetestKey"];
            GeetestLib geetest = new GeetestLib(geetestID, geetestKey);
            Byte gt_server_status_code = (Byte)Session[GeetestLib.gtServerStatusSessionKey];
            String userID = (String)Session["userID"];
            int result = 0;
            String challenge = geetest_challenge;// Request.Form.Get(GeetestLib.fnGeetestChallenge);
            String validate = geetest_validate; //Request.Form.Get(GeetestLib.fnGeetestValidate);
            String seccode = geetest_seccode;// Request.Form.Get(GeetestLib.fnGeetestSeccode);

            if (gt_server_status_code == 1)
            {
                result = geetest.enhencedValidateRequest(challenge, validate, seccode, userID);
            }
            else
            {
                result = geetest.failbackValidateRequest(challenge, validate, seccode);
            }
            return result;
        }
        //[RequireHttps]
        [HttpPost]
        public ActionResult VerifyLogin()
        {
            VerifyParam param = new VerifyParam(this);
            AccountInfoItem accountInfo = new AccountInfoItem() { Phone = param.Number, Password = param.Password };
            OperationResult result = account.MobileLogin(accountInfo);
            if (result.Success)
            {
                int startIndex = result.Data.IndexOf('|');
                if (startIndex > 0)
                {
                    string userid = result.Data.Substring(0, startIndex);
                    string dataWithoutUserId = result.Data.Substring(startIndex + 1);
                    int secondIndex = dataWithoutUserId.IndexOf('|');
                    string nickName = dataWithoutUserId.Substring(0, secondIndex);

                    bool isSaveCookie = param.IsSaveCookie;
                    string isv = isSaveCookie.ToString();
                    string loginToken = UserState.GetUserVerifyToken(nickName + isv + userid);

                    return Json(new { Message = "", token = loginToken, isv = isv, nn = nickName, uid = userid });
                }
            }
            //ToDo wwb 验证用户名是否存在。返回1表示用户名存在而密码不正确
            ExistsMobileAccountItem accountInfo2 = new ExistsMobileAccountItem() { Phone = param.Number };
            bool isExists = account.ExistsMobileAccount(accountInfo2);
            if (isExists)
            {
                return Json(new { Message = "1" });
            }
            //返回0则提示用户名不存在
            else
            {
                return Json(new { Message = "0" });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[RequireHttps]
        [HttpPost]
        public ActionResult VerifyLoginGeetest()
        {
            VerifyParam param = new VerifyParam(this);
            int geetestreuslt = Geetest(param.Geetest_Challenge, param.Geetest_Seccode, param.Geetest_Validate);
            if (geetestreuslt == 1)
            {
                AccountInfoItem accountInfo = new AccountInfoItem() { Phone = param.Number, Password = param.Password };
                OperationResult result = account.MobileLogin(accountInfo);
                if (result.Success)
                {
                    int startIndex = result.Data.IndexOf('|');
                    if (startIndex > 0)
                    {
                        string userid = result.Data.Substring(0, startIndex);
                        string dataWithoutUserId = result.Data.Substring(startIndex + 1);
                        int secondIndex = dataWithoutUserId.IndexOf('|');
                        string nickName = dataWithoutUserId.Substring(0, secondIndex);

                        bool isSaveCookie = param.IsSaveCookie;
                        string isv = isSaveCookie.ToString();
                        string loginToken = UserState.GetUserVerifyToken(nickName + isv + userid);

                        return Json(new { Message = "", token = loginToken, isv = isv, nn = nickName, uid = userid });
                    }
                }
                //ToDo wwb 验证用户名是否存在。返回1表示用户名存在而密码不正确
                ExistsMobileAccountItem accountInfo2 = new ExistsMobileAccountItem() { Phone = param.Number };
                bool isExists = account.ExistsMobileAccount(accountInfo2);
                if (isExists)
                {
                    return Json(new { Message = "1" });
                }
                //返回0则提示用户名不存在
                else
                {
                    return Json(new { Message = "0" });
                }
            }
            else
            {
                return Json(new { Message = "-1" });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerifyLoginByUid(int uid)
        {
            if (uid > 0)
            {
                //获取当前用户信息
                var userInfo = account.GetCurrentUserInfo(uid);
                if (userInfo != null && userInfo.UserID > 0)
                {
                    var nickName = userInfo.NickName;
                    bool isSaveCookie = false;
                    string isv = isSaveCookie.ToString();
                    string loginToken = UserState.GetUserVerifyToken(nickName + isv + userInfo.UserID);

                    return Json(new { Message = "", token = loginToken, isv = isv, nn = nickName, uid = userInfo.UserID });
                }
            }

            return Json(new { Message = "0" });
        }

        //[RequireHttps]
        [HttpPost]
        public ActionResult VerifyNewUser()
        {
            VerifyParam param = new VerifyParam(this);
            var result = new Dictionary<string, string>();

            //首先判断当前手机号是否已经注册zmjiudian账号，如果没有，则创建一个初始用户记录
            long userid = account.GetOrRegistPhoneUser(param.Number, param.CID);

            //检查验证码
            var state = account.checkConfirmSMS(param.Number, param.Code);
            result["ok"] = (state ? "1" : "0");
            result["userid"] = userid.ToString();
            return Json(result);
        }

        #region 安缦渠道短信通道

        //[RequireHttps]
        [HttpPost]
        public ActionResult VerifyAnman()
        {
            VerifyParam param = new VerifyParam(this);
            var result = new Dictionary<string, bool>();
            if (param.Action == "send")
            {
                LogHelper.WriteLog("VerifyAnman:" + param.Number + Request.Url + Request.UrlReferrer + Request.UserHostAddress);
                if (Request.UrlReferrer.ToString().ToLower().StartsWith("https://www.zmjiudian.com") || Request.UrlReferrer.ToString().ToLower().StartsWith("http://www.zmjiudian.com"))
                {
                    account.sendConfirmSMSAnman(param.Number);
                }
                result["ok"] = true;
            }
            else if (param.Action == "check")
            {
                result["ok"] = account.checkConfirmSMSAnman(param.Number, param.Code);
            }
            return Json(result);
        }

        #endregion

        [HttpPost]
        public ActionResult Login()
        {
            string token = Request.Form["token"];
            string isv = Request.Form["isv"];
            string nickName = Request.Form["nn"];
            string userid = Request.Form["uid"];
            string sessionToken = UserState.GetUserVerifyToken(nickName + isv + userid);
            if (!token.Equals(sessionToken))
            {
                return Json(new { Message = "token验证错误" });
            }
            else
            {
                ////检查当前uid是否合法（比如手机号前面加了del，标识当前账号被注销操作了）
                //var userInfo = account.GetCurrentUserInfo(Convert.ToInt64(userid));
                //if (userInfo != null)
                //{
                    
                //}

                bool isSaveCookie = false;
                bool.TryParse(isv, out isSaveCookie);

           //     AccountHelper.SetUserLoginState(userid, nickName, isSaveCookie, HttpContext.ApplicationInstance.Context);

                if (isSaveCookie)
                {
                    string userToken = UserState.GetUserVerifyToken(userid);
                    if (HttpContext.Request.Cookies["UID"] == null)
                    {
                        HttpCookie cookie = new HttpCookie("UID");
                        cookie.Value = HttpUtility.UrlEncode(userid);
                        cookie.Expires = DateTime.Now.AddDays(3);//一年过期
                        cookie.Domain = ".zmjiudian.com";
                        cookie.HttpOnly = true;
                        HttpContext.Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        HttpContext.Response.Cookies["UID"].Value = HttpUtility.UrlEncode(userid);
                    }

                    if (HttpContext.Request.Cookies["UIDToken"] == null)
                    {
                        HttpCookie cookie = new HttpCookie("UIDToken");
                        cookie.Value = HttpUtility.UrlEncode(userToken);
                        cookie.Expires = DateTime.Now.AddDays(3);//一年过期
                        cookie.Domain = ".zmjiudian.com";
                        cookie.HttpOnly = true;
                        HttpContext.Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        HttpContext.Response.Cookies["UIDToken"].Value = HttpUtility.UrlEncode(userToken);
                    }
                }
                else
                {
                    System.Web.HttpContext.Current.Session.Add("UID", userid);
                }

                if (HttpContext.Request.Cookies["NickName"] == null)
                {
                    HttpCookie cookie = new HttpCookie("NickName");
                    cookie.Value = HttpUtility.UrlEncode(nickName);
                    cookie.Expires = DateTime.Now.AddDays(3);//一年过期
                    cookie.Domain = ".zmjiudian.com";
                    HttpContext.Response.Cookies.Add(cookie);
                }
                else
                {
                    HttpContext.Response.Cookies["NickName"].Value = HttpUtility.UrlEncode(nickName);
                }

                return Json(new { Message = "" });
            }
        }

        //[RequireHttps]
        [HttpPost]
        public ActionResult Register()
        {
            VerifyParam param = new VerifyParam(this);
            //ToDo wwb 检测用户名和密码是否都存在，返回""则表示一直验证通过 由cookie获取用户id和密码
            RegistPhoneUserItem item = new RegistPhoneUserItem() { Phone = param.Number, Password = param.Password, ConfirmPassword = param.Code, CID = param.CID, Unionid = param.Unionid };
            OperationResult result = account.Register(item);
            if (result.Success)
            {
                return Json("");
            }
            else
            {
                return Json(result.Message);
            }
        }

        /// <summary>
        /// 注册新用户（邀请用户注册方式调用，给用户初始密码的方式，并验证邀请码等信息）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegisterForInvitation()
        {
            OperationResult _userResult = new OperationResult
            {
                Success = false,
                Message = "请求错误",
                UserID = 0
            };

            VerifyParam param = new VerifyParam(this);

            #region 验证短信验证码和邀请码（在填写了的情况下）

            var _ckCode = param.Code;
            var _incCode = param.InvitationCode;
            if (string.IsNullOrEmpty(_ckCode))
            {
                _userResult = new OperationResult
                {
                    Success = false,
                    Message = "请填写短信验证码",
                    UserID = 0
                };
                return Json(_userResult);
            }

            string MD5Key = Config.MD5Key;
            Int64 _timeStamp = Signature.GenTimeStamp();
            int _sourceId = 100;
            string _requestType = _ckCode;
            var _sign = Signature.GenSignature(_timeStamp, _sourceId, MD5Key, _requestType);

            //check param
            var ckParam = new ExistsCodeItem 
            {
                TimeStamp = _timeStamp,
                SourceID = _sourceId,
                RequestType = _requestType,
                Sign = _sign,
                Phone = param.Number,
                SMSCode = param.Code,
                InvitationCode = param.InvitationCode
            };

            //check
            var ckCodeResult = account.ExistsSMSCodeAndInvitationCode(ckParam);
            if (!ckCodeResult.Success)
            {
                _userResult = new OperationResult
                {
                    Success = false,
                    Message = ckCodeResult.Message,
                    UserID = 0
                };
                return Json(_userResult);
            }

            #endregion

            /* 注册新用户 */

            ExistsMobileAccountItem accountInfo = new ExistsMobileAccountItem() { Phone = param.Number };
            bool isExists = account.ExistsMobileAccount(accountInfo);

            #region 未注册

            if (!isExists)
            {
                //如果密码为空，则设置初始密码，并短信通知用户
                var _isInitPwd = false;
                var _passWord = param.Password;
                if (string.IsNullOrEmpty(_passWord))
                {
                    ////暂使用手机号的后六位为初始密码
                    //_passWord = param.Number.Substring(5);

                    //临时密码改为随机六位数 2017.04.21 haoy
                    _passWord = new Random().Next(100000, 999999).ToString();
                    _isInitPwd = true;
                }

                _timeStamp = Signature.GenTimeStamp();
                _requestType = param.Number;
                _sign = Signature.GenSignature(_timeStamp, _sourceId, MD5Key, _requestType);

                //ToDo wwb 检测用户名和密码是否都存在，返回""则表示一直验证通过 由cookie获取用户id和密码
                RegistPhoneUserItem item = new RegistPhoneUserItem()
                {
                    TimeStamp = _timeStamp,
                    SourceID = _sourceId,
                    RequestType = _requestType,
                    Sign = _sign,
                    Phone = HJDAPI.Common.Security.DES.Encrypt(param.Number),
                    Password = HJDAPI.Common.Security.DES.Encrypt(_passWord),
                    ConfirmPassword = _passWord,
                    InvitationCode = param.InvitationCode,
                    IsTemporaryPWD = true,
                    CID = param.CID,
                    Unionid = param.Unionid
                };

                _userResult = account.RegistPhoneUser50(item);
                if (_userResult.Success)
                {
                    #region 发初始密码短信 - 如果短信是系统初始的，则注册成功后，发送短信通知

                    if (_isInitPwd)
                    {
                        var _sms = string.Format(@"你的临时密码为：{0}。登录周末酒店APP，可在设置页面更改密码。点击下载周末酒店APP：http://app.zmjiudian.com", _passWord);

                        //send
                        var _send = new Access().SendSMS(param.Number, _sms);
                    }

                    #endregion

                    #region 关注推荐人 - 如果用户选择的关注推荐者，则自动关注

                    if (param.IsFollow && param.FollowUserId > 0)
                    {
                        _timeStamp = Signature.GenTimeStamp();
                        _requestType = string.Format("{0}:{1}", param.Number, param.FollowUserId);
                        _sign = Signature.GenSignature(_timeStamp, _sourceId, MD5Key, _requestType);

                        var followParam = new ChangeFollowerFollowingParam
                        {
                            TimeStamp = _timeStamp,
                            SourceID = _sourceId,
                            RequestType = _requestType,
                            Sign = _sign,
                            follower = _userResult.UserID,
                            following = param.FollowUserId,
                            isValid = true
                        };
                        var _follow = account.UpdateFollowerFollowingRel(followParam);
                    }

                    #endregion

                    return Json(_userResult);
                }
                else
                {
                    return Json(_userResult);
                }
            }

            #endregion

            #region 已注册

            else
            {
                _userResult = new OperationResult 
                {
                    Success = false,
                    Message = "用户信息查询失败",
                    UserID = 0
                };

                var _userInfo = account.GetUserInfoByMobile(param.Number);
                if (_userInfo != null && _userInfo.UserId > 0)
                {
                    _userResult = new OperationResult
                    {
                        Success = true,
                        Message = "",
                        UserID = _userInfo.UserId
                    };
                }

                return Json(_userResult);
            }

            #endregion
        }

        //[RequireHttps]
        [HttpPost]
        public ActionResult ExistsMobileAccount()
        {
            //VerifyParam param = new VerifyParam(this);
            //ToDo wwb 检测用户名和密码是否都存在，返回""则表示一直验证通过 由cookie获取用户id和密码
            ExistsMobileAccountItem accountInfo = new ExistsMobileAccountItem() { Phone = this.Request.Form["number"] };
            bool isExists = account.ExistsMobileAccount(accountInfo);
            return Json(new { isExists = isExists });
            //return Json(new { isExists = isExists }, JsonRequestBehavior.AllowGet);
            //return new JsonpResult<object>(new { isExists = isExists }, param.Callback);
        }

        /// <summary>
        /// 包含极验行为验证的 找回密码
        /// </summary>
        /// <returns></returns>
        //[RequireHttps]
        [HttpPost]
        public ActionResult ExistsMobileAccountGeetest()
        {
            VerifyParam param = new VerifyParam(this);

            int geetestreuslt = Geetest(param.Geetest_Challenge, param.Geetest_Seccode, param.Geetest_Validate);
            if (geetestreuslt == 1)
            {
                //ToDo wwb 检测用户名和密码是否都存在，返回""则表示一直验证通过 由cookie获取用户id和密码
                ExistsMobileAccountItem accountInfo = new ExistsMobileAccountItem() { Phone = this.Request.Form["number"] };
                bool isExists = account.ExistsMobileAccount(accountInfo);
                return Json(new { isExists = isExists });
                //return Json(new { isExists = isExists }, JsonRequestBehavior.AllowGet);
                //return new JsonpResult<object>(new { isExists = isExists }, param.Callback);
            }
            else
            {
                return Json(new { isExists = false });
            }

        }

        //[RequireHttps]
        [HttpPost]
        public ActionResult ResetPasswordWithPhone()
        {
            VerifyParam param = new VerifyParam(this);
            string currentPhone = param.Number;//手机号
            ResetPasswordItem item = new ResetPasswordItem() { Phone = currentPhone, newpassword = param.Password, confirmCode = param.Code };
            OperationResult result = account.ResetPasswordWithPhone(item);
            return Json(result.Success);
        }

        /// <summary>
        /// 个人信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Info()
        {
            GetUserCommInfoReqParm param = new GetUserCommInfoReqParm();
            param.UserID = UserState.UserID;
            if (param.UserID == 0 || !UserState.IsLogin)
            {
                return Json(new { Message = "请重新登录", Success = 1 }, JsonRequestBehavior.AllowGet);
            }

            MemberProfileInfo info2 = account.GetCurrentUserInfo(param.UserID);
            param.Phone = info2.MobileAccount;
            param.InfoType = 1000;//与数据库字段无关 在API里该参数用来标记是发票信息类型

            PersonalInfo result = new PersonalInfo();
            List<UserCommInfoEntity> commonInfos = account.GetUserCommInfo(param);

            result.CommonInfos = commonInfos;
            result.UserID = param.UserID;
            result.NickName = info2.NickName;
            result.PhoneNumber = long.Parse(info2.MobileAccount);
            return View(result);
        }

        /***修改昵称开始***/
        /// <summary>
        /// 昵称可用：没有人用过，昵称没有包括特殊词
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>        
        [HttpPost]
        public ActionResult CheckNickName()
        {
            UserNickNameModel u = new UserNickNameModel();
            u.userID = UserState.UserID;
            if (u.userID == 0 || !UserState.IsLogin)
            {
                return Json(new { Message = "请重新登录", Success = 1 });
            }
            u.nickName = this.Request.Form["nickName"];
            return Json(account.CheckNickName(u));
        }

        /// <summary>
        /// 更新用户昵称
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateNickName()
        {
            UserNickNameModel u = new UserNickNameModel();
            u.userID = UserState.UserID;
            if (u.userID == 0 || !UserState.IsLogin)
            {
                return Json(new { Message = "请重新登录", Success = 1 });
            }
            u.nickName = this.Request.Form["nickName"];
            u.password = this.Request.Form["password"];

            HJDAPI.Models.ResultEntity result = account.UpdateNickName(u);
            if (result.Success == 0)
            {
                if (HttpContext.Request.Cookies["NickName"] == null)
                {
                    HttpCookie cookie = new HttpCookie("NickName");
                    cookie.Value = HttpUtility.UrlEncode(u.nickName);
                    cookie.Expires = DateTime.Now.AddDays(3);//一年过期
                    cookie.Domain = ".zmjiudian.com";
                    HttpContext.Response.Cookies.Add(cookie);
                }
                else
                {
                    HttpContext.Response.Cookies["NickName"].Value = HttpUtility.UrlEncode(u.nickName);
                }
            }
            return Json(result);
        }
        /***修改昵称结束***/

        /// <summary>
        /// 发送用来修改密码的手机验证码
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        //[RequireHttps]
        [HttpPost]
        public ActionResult SendResetPasswordWithPhoneConfirmCode()
        {
            //if(!UserState.IsLogin){
            //    return Json(new { Message = "请重新登录", Success = 1 });
            //}
            ResetPasswordItem r = new ResetPasswordItem();
            r.Phone = this.Request.Form["phone"];
            return Json(account.SendResetPasswordWithPhoneConfirmCode(r));
        }

        /// <summary>
        /// 修改手机
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        //[RequireHttps]
        [HttpPost]
        public ActionResult ModifyUserPhone()
        {
            ModifyUserPhoneItem m = new ModifyUserPhoneItem();
            try
            {
                m.userid = long.Parse(this.Request.Form["userid"]);
            }
            catch
            {
                m.userid = 0;
            }

            if (m.userid == 0)
            {
                return Json(new { Message = "参数错误", Success = false });
            }
            m.newPhone = this.Request.Form["newphone"];
            m.confirmCode = this.Request.Form["confirmCode"];
            m.password = this.Request.Form["password"];

            OperationResult result = account.ModifyUserPhone(m);

            return Json(result);
        }

        [HttpPost]
        public ActionResult ReLoginAgain()
        {
            clearUserAllCookieAndSession();
            return Json(new { Success = 0 });
        }

        //[RequireHttps]
        [HttpPost]
        public ActionResult SendModifyUserPhoneConfirmCode()
        {
            ModifyUserPhoneItem m = new ModifyUserPhoneItem();
            try
            {
                m.userid = long.Parse(this.Request.Form["userid"]);
            }
            catch
            {
                m.userid = 0;
            }
            if (m.userid == 0)
            {
                return Json(new { Message = "参数错误", Success = false });
            }
            m.newPhone = this.Request.Form["newphone"];
            m.password = this.Request.Form["password"];
            return Json(account.SendModifyUserPhoneConfirmCode(m));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        //[RequireHttps]
        [HttpPost]
        public ActionResult ModifyPassword()
        {
            ModifyPasswordItem r = new ModifyPasswordItem();
            try
            {
                r.userid = long.Parse(this.Request.Form["userid"]);
            }
            catch
            {
                r.userid = 0;
            }
            if (r.userid == 0)
            {
                return Json(new { Message = "参数错误", Success = false });
            }
            r.oldpassword = this.Request.Form["oldpassword"];
            r.newpassword = this.Request.Form["newpassword"];
            OperationResult result = account.ModifyPassword(r);

            return Json(result);
        }

        /// <summary>
        /// 获得通用个人信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public List<UserCommInfoEntity> GetUserCommInfo(GetUserCommInfoReqParm p)
        {
            if (!UserState.IsLogin)
            {
                return new List<UserCommInfoEntity>();
            }
            return account.GetUserCommInfo(p);
        }

        /// <summary>
        /// 添加新的用户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUserCommInfo()
        {
            HJDAPI.Models.ResultEntity result = new HJDAPI.Models.ResultEntity();
            UserCommInfoEntity entity = new UserCommInfoEntity();
            int InfoType = 0;
            int State = 0;
            int IDX = 0;

            entity.Info = this.Request.Form["info"];
            int.TryParse(this.Request.Form["infotype"], out InfoType);
            int.TryParse(this.Request.Form["state"], out State);
            int.TryParse(this.Request.Form["idx"], out IDX);

            long userid = UserState.UserID;

            if (string.IsNullOrEmpty(entity.Info))
            {
                result.Success = 1;
                result.Message = "信息不能为空";
                return Json(result);
            }
            else if (State == 0)
            {
                result.Success = 2;
                result.Message = "信息状态不能为空";
                return Json(result);
            }
            else if (userid == 0 || !UserState.IsLogin)
            {
                result.Success = 3;
                result.Message = "需要登录才能修改";
                return Json(result);
            }
            entity.InfoType = InfoType;
            entity.State = State;
            entity.IDX = IDX;
            entity.UserID = userid;
            entity.IDType = 1;//UserID不为空 则以IDtype=1去添加
            result.Success = account.AddUserCommInfo(entity) ? 0 : 4;
            result.Message = result.Success == 0 ? "" : "更新失败";
            return Json(result);
        }

        [HttpPost]
        public ActionResult QuitLogin()
        {
            clearUserAllCookieAndSession();

            OperationResult result = new OperationResult();
            result.Success = true;
            result.Message = "";
            return Json(result);
        }

        private void clearUserAllCookieAndSession()
        {
            HttpCookie newCookie = null;
            if (HttpContext.Request.Cookies["UID"] != null)
            {
                newCookie = new HttpCookie("UID");
                newCookie.Expires = DateTime.Now.AddDays(-360);
                newCookie.Domain = ".zmjiudian.com";
                HttpContext.Response.Cookies.Add(newCookie);
            }
            if (HttpContext.Request.Cookies["UIDToken"] != null)
            {
                newCookie = new HttpCookie("UIDToken");
                newCookie.Expires = DateTime.Now.AddDays(-360);
                newCookie.Domain = ".zmjiudian.com";
                HttpContext.Response.Cookies.Add(newCookie);
            }
            if (HttpContext.Request.Cookies["NickName"] != null)
            {
                newCookie = new HttpCookie("NickName");
                newCookie.Expires = DateTime.Now.AddDays(-360);
                newCookie.Domain = ".zmjiudian.com";
                HttpContext.Response.Cookies.Add(newCookie);
            }

            if (System.Web.HttpContext.Current.Session["UID"] != null)
            {
                System.Web.HttpContext.Current.Session["UID"] = null;
            }
        }

        public ActionResult ContractDescription()
        {
            return View();
        }

        public ActionResult ThrowException()
        {
            for (int i = 0; i < 10; i++)
            {

            }

            Convert.ToInt32("0.15");
            return null;
        }

        ///// <summary>
        ///// 用户个性签名
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult UserTag(int userId = 0, string regname = "", string regtell = "")
        //{
        //    var userTags = new List<UserTagOption>();// new HJDAPI.APIProxy.Inspector().GetUserTagOptionList();
        //    var workTag = userTags.Find(ut => ut.TypeId == 1);
        //    var hotelTag = userTags.Find(ut => ut.TypeId == 2);
        //    var levelTag = userTags.Find(ut => ut.TypeId == 3);
        //    var travelTag = userTags.Find(ut => ut.TypeId == 4);
        //    ViewBag.UserTags = userTags;
        //    ViewBag.WorkTag = workTag;
        //    ViewBag.HotelTag = hotelTag;
        //    ViewBag.LevelTag = levelTag;
        //    ViewBag.TravelTag = travelTag;

        //    ViewBag.UserId = userId;
        //    ViewBag.RegName = regname;
        //    ViewBag.RegTell = regtell;
        //    return View();
        //}

        ///// <summary>
        ///// 提交用户个性标签
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult SaveUserTag(int userId, string tags, string name, string tell, string mail)
        //{
        //    var result = new Dictionary<string, bool>();

        //    //解析用户tag信息
        //    var userTags = new List<UserTagRelEntity>();
        //    var tagList = Regex.Split(tags, ";");
        //    for (int t = 0; t < tagList.Length; t++)
        //    {
        //        var tagStr = tagList[t];
        //        if (!string.IsNullOrEmpty(tagStr.Trim()))
        //        {
        //            var tagArray = Regex.Split(tagStr.Trim(), ",");
        //            if (tagArray.Length > 2)
        //            {
        //                var tagObj = new UserTagRelEntity();
        //                tagObj.TypeId = Convert.ToInt32(tagArray[0]);
        //                tagObj.TagID = Convert.ToInt32(tagArray[1]);
        //                tagObj.TagName = tagArray[2];
        //                tagObj.UserID = Convert.ToInt64(userId);
        //                userTags.Add(tagObj);
        //            }
        //        }
        //    }

        //    InspectorApplyData insData = new InspectorApplyData
        //    {
        //        UserID = 0,
        //        TrueName = name,
        //        MobilePhone = tell,
        //        Job = "",
        //        JobExperience = "",
        //        JobSpecialty = "",
        //        Tags = userTags
        //    };

        //    ResultEntity re = new HJDAPI.APIProxy.Inspector().SubmitInspectorApplyData(insData);
        //    return Json(re, JsonRequestBehavior.AllowGet);
        //}

        #region 重新获取验证码 check验证码的方法

        //[HttpGet]
        //public ActionResult ChangeValidationCodeImg()
        //{
        //    var sessionId = Session.SessionID;
        //    var validationCodeResult = new Access().GenValidationCodeBase64Str(sessionId);
        //    validationCodeResult.base64Url = string.Format("data:image/gif;base64,{0}", validationCodeResult.base64Url);
        //    return Json(validationCodeResult, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult CheckValidationCodeImg(string validationCode)
        //{
        //    var sessionId = Session.SessionID;
        //    var validationCodeResult = new Access().VerifyValidationCode(sessionId, validationCode);
        //    validationCodeResult.base64Url = string.Format("data:image/gif;base64,{0}", validationCodeResult.base64Url);
        //    return Json(validationCodeResult, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        /// <summary>
        /// VIP权益
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult VipRights(int userid = 0)
        {
            //2017-09-13晚开始，之前旧的购买VIP页面，全部转向新的 VIP专区 页面VipAreaInfo
            return Redirect(string.Format("/Coupon/VipAreaInfo?userid={0}", userid));

            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.UserId = userid;

            //调试使用
            ViewBag.AppType = AppType();
            if (AppType() == "iosapp") ViewBag.AppVerForIOS = AppVerForIOS();
            else ViewBag.AppVerForIOS = "";
            if (AppType() == "android") ViewBag.AppVerForAndroid = AppVerForAndroid();
            else ViewBag.AppVerForAndroid = "";
            ViewBag.IsLatestVerApp = IsLatestVerApp();

            ViewBag.isMobile = Utils.IsMobile();

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(userid);
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //如果当前用户非VIP，则直接重定向购买VIP页面
            if (userInfo == null ||
            ((HJDAPI.Common.Helpers.Enums.CustomerType)userInfo.CustomerType != HJDAPI.Common.Helpers.Enums.CustomerType.vip
            && (HJDAPI.Common.Helpers.Enums.CustomerType)userInfo.CustomerType != HJDAPI.Common.Helpers.Enums.CustomerType.vip3M
            && (HJDAPI.Common.Helpers.Enums.CustomerType)userInfo.CustomerType != HJDAPI.Common.Helpers.Enums.CustomerType.vip6M
            && (HJDAPI.Common.Helpers.Enums.CustomerType)userInfo.CustomerType != HJDAPI.Common.Helpers.Enums.CustomerType.vip199
            && (HJDAPI.Common.Helpers.Enums.CustomerType)userInfo.CustomerType != HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr
            && (HJDAPI.Common.Helpers.Enums.CustomerType)userInfo.CustomerType != HJDAPI.Common.Helpers.Enums.CustomerType.vip599))
            {
                return Redirect(string.Format("/Coupon/VipShopInfo?userid={0}", userid));
            }

            return View();
        }

        #region 邀请好友

        /// <summary>
        /// 邀请好友
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult InviteFriend(int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.UserId = userid;
            ViewBag.isMobile = Utils.IsMobile();

            return View();
        }

        /// <summary>
        /// 邀请朋友购买VIP
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult InviteBuyVip(int userid = 0)
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;
            ViewBag.UserId = userid;
            ViewBag.isMobile = Utils.IsMobile();

            //获取当前用户信息
            var userInfo = account.GetUserInfoByUserID(userid);
            ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            //相关分享参数
            var s_title = string.Format("你的朋友{0}邀你成为VIP", (userInfo != null && !string.IsNullOrEmpty(userInfo.NickName)) ? userInfo.NickName : "");
            var s_poto_url = "http://whfront.b0.upaiyun.com/app/img/coupon/vipshopinfo/vip-area-share-img.png";
            var s_url = string.Format("http://www.zmjiudian.com/Coupon/VipAreaInfo?CID={0}", userid);
            s_url = HttpUtility.UrlEncode(s_url);
            var s_content = "成为周末酒店VIP，价值万元专享福利优惠等你拿！";

            //原生分享链接
            var shareLink = "whfriend://comment?title={0}&photoUrl={1}&shareLink={2}&nextUrl={3}&content={4}&shareType={5}";
            shareLink = string.Format(shareLink, HttpUtility.UrlEncode(s_title), HttpUtility.UrlEncode(s_poto_url), HttpUtility.UrlEncode(s_url), "", HttpUtility.UrlEncode(s_content), "{0}");
            ViewBag.ShareLink = string.Format(shareLink, 0);

            return View();
        }

        #endregion

        #region 微信环境下使用的页面

        /// <summary>
        /// 微信菜单中转页面（主要是提供到达页面前的登录功能）
        /// </summary>
        /// <param name="menu">0 指定链接重定向 1我的订单 2我的房券 3我的会籍 4顾问 5我的消费券 6我的钱包 7主页(app环境跳app下的首页；其它环境跳h5首页) 8邀请好友总页面 9VIP专区页面 10版本6.2的订单列表 100猪鼓励旅行活动报名链接 101猪鼓励旅行活动详情页面 -1清楚本地登录缓存/纯功能性操作</param>
        /// <param name="orderid">当menu=1或者10的时候，如果传递了orderid，则重定向订单详情页</param>
        /// <param name="redurl"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult WxMenuTransfer(int menu = 0, string orderid = "", string redurl = "", string code = "")
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            var userid = UserState.UserID;
            ViewBag.UserId = userid;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;// = true;

            #region 微信环境下，做微信静默授权

            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();
            if (isInWeixin)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var weixinAcountName = "weixinservice_haoyi";

                    //通过code换取网页授权access_token
                    var accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Openid))
                    {
                        //获取用户信息
                        //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                        weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                        if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                        {
                            //存储微信用户
                            try
                            {
                                //insert
                                var w = new WeixinUser
                                {
                                    Openid = weixinUserInfo.Openid,
                                    Unionid = weixinUserInfo.Unionid,
                                    Nickname = weixinUserInfo.Nickname,
                                    Sex = weixinUserInfo.Sex.ToString(),
                                    Province = weixinUserInfo.Province,
                                    City = weixinUserInfo.City,
                                    Country = weixinUserInfo.Country,
                                    Headimgurl = weixinUserInfo.Headimgurl,
                                    Privilege = "",
                                    Phone = "",
                                    Remark = "",
                                    GroupId = "0",
                                    Subscribe = 0,
                                    WeixinAcount = weixinAcountName,   //WeiXinChannelCode.周末酒店服务号_皓颐
                                    Language = "zh_CN",
                                    SubscribeTime = DateTime.Now,
                                    CreateTime = DateTime.Now
                                };
                                var update = new Weixin().UpdateWeixinUserSubscribe(w);
                            }
                            catch (Exception ex)
                            {

                            }

                            #region 处理微信号绑定用户信息相关操作

                            //微信环境下，如果当前已经登录，则直接绑定
                            if (userid > 0)
                            {
                                WeiXinHelper.BindingWxAndUid(userid, weixinUserInfo.Unionid);
                            }

                            #endregion

                            #region 更新Unionid缓存并更新CID信息

                            SetCurWXUnionid(weixinUserInfo.Unionid);
                            CheckCID();

                            #endregion
                        }
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Account/WxMenuTransfer?menu={0}&orderid={1}&redurl={2}", menu, orderid, redurl)));
                        return Redirect(weixinGoUrl);
                    }
                }
                else
                {
                    //授权页面
                    var weixinGoUrl = WeiXinHelper.GenSilenceAuthorUrlForHaoYi(HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/Account/WxMenuTransfer?menu={0}&orderid={1}&redurl={2}", menu, orderid, redurl)));
                    return Redirect(weixinGoUrl);
                }
            }
            ViewBag.Unionid = weixinUserInfo.Unionid;

            #endregion

            //是否登录
            var isLogin = UserState.IsLogin;
            ViewBag.IsLogin = isLogin;

            //是否非法访问
            var isErrPage = false;

            //是否清楚缓存处理
            var isClearCache = false;

            //是否制定重定向
            var isRedUrl = false;
            if (menu == 0 && !string.IsNullOrEmpty(redurl))
            {
                isRedUrl = true;

                ViewBag.IsErrPage = isErrPage;
                ViewBag.IsClearCache = isClearCache;
                ViewBag.IsRedUrl = isRedUrl;
                ViewBag.RedirectUrl = redurl;
                isRedUrl = true;

                return View();
            }

            //重定向地址
            var _goUrl = "";

            //以当前实际登录身份跳转
            switch (menu)
            {
                
                case 7:
                    #region 移动端主页
                    {
                        if (isApp)
                        {
                            _goUrl = "whotelapp://www.zmjiudian.com";
                        }
                        else
                        {
                            _goUrl = string.Format("http://m.zmjiudian.com/home6.html?userid={0}&web=1", userid);
                        }
                        break;
                    }
                #endregion
                case 100:
                    #region 2019猪鼓励旅行活动报名页面
                    {
                        _goUrl = "https://mp.weixin.qq.com/s?__biz=MzA5NTIwODUzMQ==&mid=517801433&idx=1&sn=22316fb013b228ab17ca3715380bd18f&chksm=0b6b70e53c1cf9f355094c819f97af7817a6abf9ca7e937db6f41862fa0fcb69d13901db5eb2#rd";
                        break;
                    }
                #endregion
                case 101:
                    #region 2019猪鼓励旅行活动详情
                    {
                        _goUrl = "http://www.zmjiudian.com/active/activepage?midx=16888";
                        break;
                    }
                    #endregion
            }

            if (!string.IsNullOrEmpty(_goUrl))
            {
                return Redirect(_goUrl);
            }

            //微信下的登录跳转
            if (isInWeixin || menu == 9 || menu == 10 || menu == -1 || (menu == 1 && !string.IsNullOrEmpty(orderid)))
            {
                //如果用户没有登录，强制弹出登录窗口
                if (isLogin || menu == -1)
                {
                    switch (menu)
                    {
                        //我的订单
                        case 1:
                            {
                                if (!string.IsNullOrEmpty(orderid))
                                {
                                    _goUrl = string.Format("/personal/order/{0}", orderid);
                                }
                                else
                                {
                                    //_goUrl = "/personal/order";
                                    _goUrl = "/app/order";
                                }
                                break;
                            }
                        //我的房券
                        case 2:
                            {
                                _goUrl = GetUserWalletUrl(userid, "roomcoupon");
                                break;
                            }
                        //我的会籍
                        case 3:
                            {
                                _goUrl = string.Format("/Account/VipRights?userid={0}", userid);
                                break;
                            }
                        //magicall
                        case 4:
                            {
                                _goUrl = string.Format("/MagiCall/MagiCallClient?userid={0}", userid);
                                break;
                            }
                        //我的消费券
                        case 5:
                            {
                                _goUrl = GetUserWalletUrl(userid, "productcoupon", "detail");
                                break;
                            }
                        //我的钱包
                        case 6:
                            {
                                _goUrl = GetUserWalletUrl(userid, "", "");
                                break;
                            }
                        //邀请好友
                        case 8:
                            {
                                _goUrl = string.Format("/Account/InviteFriend?userid={0}", userid);
                                break;
                            }
                        //VIP专区
                        case 9:
                            {
                                _goUrl = string.Format("/Coupon/VipAreaInfo?userid={0}", userid);
                                break;
                            }
                        //6.2版本订单列表
                        case 10:
                            {
                                if (!string.IsNullOrEmpty(orderid))
                                {
                                    _goUrl = string.Format("/personal/order/{0}", orderid);
                                }
                                else
                                {
                                    //_goUrl = "/personal/order";
                                    _goUrl = "/order/allorders";
                                }
                                break;
                            }
                        //如果menu传-1的话，说明只做清楚本地登录缓存处理
                        case -1: 
                            {
                                try
                                {
                                    clearUserAllCookieAndSession();
                                }
                                catch (Exception ex)
                                {

                                }
                                isClearCache = true;
                                break;
                            }
                    }

                    if (!string.IsNullOrEmpty(_goUrl))
                    {
                        return Redirect(_goUrl);
                    }
                    else
                    {
                        //如果登录了，但是进来的页面指向参数是不对的，则一样提示 非法访问
                        isErrPage = true;
                    }
                }
            }
            else 
            {
                isErrPage = true;
                //Response.Write("非法访问");
            }

            ViewBag.IsErrPage = isErrPage;
            ViewBag.IsClearCache = isClearCache;
            ViewBag.IsRedUrl = isRedUrl;
            ViewBag.RedirectUrl = redurl;

            ////获取当前用户信息
            //var userInfo = account.GetUserInfoByUserID(userid);
            //ViewBag.UserInfo = userInfo ?? new UserInfoResult();

            return View();
        }

        /// <summary>
        /// 获取钱包下的各个功能的URL
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tag"></param>
        /// <param name="mode"></param>
        /// <param name="newtitle">url是否要追加 _newtitle=1 参数</param>
        /// <param name="newpage">url是否要追加 _newpage=1 参数</param>
        /// <returns></returns>
        public string GetUserWalletUrl(long userId, string tag, string mode = "", bool newtitle = false, bool newpage = false)
        {
            Int64 url_TimeStamp = Signature.GenTimeStamp();
            int url_SourceID = 100;
            string webSiteUrl = Config.WebSiteUrl;
            if (string.IsNullOrWhiteSpace(webSiteUrl))
            {
                webSiteUrl = "http://www.zmjiudian.com/";
            }
            //webSiteUrl = "http://192.168.1.22:8081/";
            string url_RequestType = String.Format("{4}personal/wallet/{0}/{3}{5}?TimeStamp={1}&SourceID={2}", userId, url_TimeStamp, url_SourceID, tag, webSiteUrl, (string.IsNullOrEmpty(mode) ? "" : "/" + mode));

            if (string.IsNullOrEmpty(tag))
            {
                url_RequestType = String.Format("{0}personal/wallet/{1}?TimeStamp={2}&SourceID={3}", webSiteUrl, userId, url_TimeStamp, url_SourceID);
            }

            string MD5Key = Config.MD5Key;
            string Sign = Signature.GenSignature(url_TimeStamp, url_SourceID, MD5Key, url_RequestType);
            string UserInfoUrl = String.Format("{0}&Sign={1}{2}{3}", url_RequestType, Sign, (newtitle ? "&_newtitle=1" : ""), (newpage ? "&_newpage=1" : ""));
            return UserInfoUrl;
        }

        #endregion
        
        #region 验证码
        public ActionResult IdentifyCode(string goUrl = "")
        {
            ViewBag.AcUrl = goUrl;
            return View();
        }
        public ActionResult GetIdentifyCode()
        {
            string oldcode = TempData["SecurityCode"] as string;
            string code = CreateRandomCode(4); //验证码的字符为4个
            HttpContext.Session["VerifyCode"] = code; //验证码存放在TempData中
            return File(CreateValidateGraphic(code), "image/Jpeg");
            //return View();
        }

        public string CreateRandomCode(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,a,b,c,d,e,f,g,h,i,g,k,l,m,n,o,p,q,r,F,G,H,I,G,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(35);
                if (temp == t)
                {
                    return CreateRandomCode(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        public byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 16.0), 27);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, x2, y1, y2);
                }
                Font font = new Font("Arial", 13, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);

                //画图片的前景干扰线
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);

                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }

        public bool CheckIdentifyCode(string code)
        {
            bool isTrue = false;
            if (HttpContext.Session["VerifyCode"].ToString().ToUpper() == code.ToUpper())
            {
                isTrue = true;
            }
            return isTrue;
        } 
        #endregion

        #region 微信环境下的调试页面

        /// <summary>
        /// 
        /// </summary>
        /// <param name="no"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult WxAccountInfo(int no = 7, string code = "")
        {
            var isApp = IsApp();
            ViewBag.IsApp = isApp;

            var userid = UserState.UserID;
            ViewBag.UserId = userid;

            //Weixin
            bool isInWeixin = Request.UserAgent.IndexOf("MicroMessenger") > 0 ? true : false;
            ViewBag.isInWeixin = isInWeixin;// = true;

            //微信用户信息
            WeixinUserInfo weixinUserInfo = new WeixinUserInfo();

            //获取当前产品的归属微信账号
            var weixinAcount = no;
            ViewBag.WeixinAcount = weixinAcount;

            if (isInWeixin)
            {
                #region 有code，说明微信已回调

                if (!string.IsNullOrEmpty(code))
                {
                    var weixinAcountName = "weixinservice_haoyi";

                    //通过code换取网页授权access_token
                    WeixinAccessToken accessToken = new WeixinAccessToken();
                    switch ((WeiXinChannelCode)weixinAcount)
                    {
                        case WeiXinChannelCode.周末酒店服务号_皓颐:
                            weixinAcountName = "weixinservice_haoyi";
                            accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYi(code);
                            break;
                        case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                            weixinAcountName = "weixinservice_haoyi_sz";
                            accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiSZ(code);
                            break;
                        case WeiXinChannelCode.周末酒店成都服务号_皓颐:
                            weixinAcountName = "weixinservice_haoyi_cd";
                            accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiCD(code);
                            break;
                        case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                            weixinAcountName = "weixinservice_haoyi_shenz";
                            accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiShenZ(code);
                            break;
                        case WeiXinChannelCode.遛娃指南服务号_皓颐:
                            weixinAcountName = "weixinservice_haoyi_liuwa";
                            accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwa(code);
                            break;
                        case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                            weixinAcountName = "weixinservice_haoyi_liuwa_nj";
                            accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaNJ(code);
                            break;
                        case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                            weixinAcountName = "weixinservice_haoyi_liuwa_wx";
                            accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaWX(code);
                            break;
                        case WeiXinChannelCode.遛娃指南广州服务号_皓颐:
                            weixinAcountName = "weixinservice_haoyi_liuwa_gz";
                            accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaGZH(code);
                            break;
                        case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                            weixinAcountName = "weixinservice_haoyi_liuwa_hzh";
                            accessToken = WeiXinHelper.GetWeixinAccessTokenForHaoYiLiuwaHZH(code);
                            break;
                    }
                    //var accessToken = new WeixinAccessToken { Openid = "okg6-uBqijun__Frr_dJfV8m6Tjs" };

                    //通过access_token拉取用户信息(需scope为 snsapi_userinfo)（这是通过微信api获取的真实用户信息）
                    weixinUserInfo = WeiXinHelper.GetWeixinUserInfo(accessToken.AccessToken, accessToken.Openid);
                    if (weixinUserInfo != null && !string.IsNullOrEmpty(weixinUserInfo.Openid))
                    {
                        ////存储微信用户
                        //try
                        //{
                        //    var w = new WeixinUser
                        //    {
                        //        Openid = weixinUserInfo.Openid,
                        //        Unionid = weixinUserInfo.Unionid,
                        //        Nickname = weixinUserInfo.Nickname,
                        //        Sex = weixinUserInfo.Sex.ToString(),
                        //        Province = weixinUserInfo.Province,
                        //        City = weixinUserInfo.City,
                        //        Country = weixinUserInfo.Country,
                        //        Headimgurl = weixinUserInfo.Headimgurl,
                        //        Privilege = "",
                        //        Phone = "",
                        //        Remark = "",
                        //        GroupId = "0",
                        //        Subscribe = 0,
                        //        WeixinAcount = weixinAcountName,
                        //        Language = "zh_CN",
                        //        SubscribeTime = DateTime.Now,
                        //        CreateTime = DateTime.Now
                        //    };
                        //    var update = new Weixin().UpdateWeixinUserSubscribe(w);
                        //}
                        //catch (Exception ex)
                        //{

                        //}
                    }
                    else
                    {
                        //如果code有值，但不能获取，一般说明code过期了，那么重新获取
                        var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/account/wxaccountinfo?no={0}", no)));
                        return Redirect(weixinGoUrl);
                    }
                }

                #endregion

                #region 没有code，需要微信授权回调

                else
                {
                    //没有授权的跳转至授权页面
                    var weixinGoUrl = WeiXinHelper.GenConfrimAuthorUrlByWeixinAcount((WeiXinChannelCode)weixinAcount, HttpUtility.UrlEncode(string.Format("http://www.zmjiudian.com/account/wxaccountinfo?no={0}", no)));
                    return Redirect(weixinGoUrl);
                }

                #endregion
            }
            ViewBag.WeixinUserInfo = weixinUserInfo;
            ViewBag.Unionid = weixinUserInfo.Unionid;

            return View();
        }


        #endregion
    }
}