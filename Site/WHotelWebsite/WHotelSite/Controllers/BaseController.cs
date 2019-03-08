using HJD.HotelServices.Contracts;
using HJDAPI.APIProxy;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WHotelSite.Common;

namespace WHotelSite.Controllers
{
    public class BaseController : Controller
    {
        public string DeviceType;   //区别iOS或Android设备
        public string DeviceVer;    //App版本号 如3.x 4.x
        const string CurWXUnionidSessionKey = "CurWXUnionid";   //缓存当前用户微信授权Unionid的key

        /// <summary>
        /// 当前上下文的相关信息
        /// </summary>
        public ContextBasicInfo _ContextBasicInfo = new ContextBasicInfo();

        public BaseController()
        {
            CheckCID();
            CheckActivity(); //活动ID
            ViewBag.CID = GetCurCID();

            //设备版本与版本号
            DeviceType = AppType();
            string _appTypeLower = DeviceType.ToLower();

            //IsApp
            var isApp = IsApp();

            //是否web环境
            var isWeb = _appTypeLower.Contains("web");

            //是否ios环境
            var isIOS = _appTypeLower.Contains("ios");

            //是否android环境
            var isAndroid = _appTypeLower.Contains("android");

            DeviceVer = isAndroid ? AppVerCodeForAndroid() : isIOS ? AppVerCodeForIOS() : "0";

            #region 初始_ContextBasicInfo

            //AppUserId
            _ContextBasicInfo.AppUserID = AppUserID();

            //app版本打包号 如322
            _ContextBasicInfo.AppBundleVer = 0;
            try
            {
                var appBundleVerStr = isAndroid ? System.Web.HttpContext.Current.Request.Headers["appVersionCode"] : isIOS ? System.Web.HttpContext.Current.Request.Headers["appver"] : "0";
                if (!string.IsNullOrEmpty(appBundleVerStr))
                {
                    _ContextBasicInfo.AppBundleVer = Convert.ToInt64(appBundleVerStr);
                }
            }
            catch (Exception ex)
            {

            }

            _ContextBasicInfo.IsApp = isApp;
            _ContextBasicInfo.AppVer = DeviceVer;
            _ContextBasicInfo.AppType = DeviceType;

            //是否web环境
            _ContextBasicInfo.IsWeb = isWeb;

            //是否ios环境
            _ContextBasicInfo.IsIOS = isIOS;

            //是否android环境
            _ContextBasicInfo.IsAndroid = isAndroid;

            //是否app环境
            if (_ContextBasicInfo.IsIOS || _ContextBasicInfo.IsAndroid)
            {
                _ContextBasicInfo.IsApp = true;
            }

            _ContextBasicInfo.UserHostAddress = System.Web.HttpContext.Current.Request.UserHostAddress;

            //初始配置app的版本比较变量
            InitAppCompareVer();

            #endregion
        }

        private void CheckActivity()
        {
            if (System.Web.HttpContext.Current.Request.QueryString.HasKeys() &&
                System.Web.HttpContext.Current.Request.QueryString["Activity"] != null)
            {
                System.Web.HttpContext.Current.Session["Activity"] = System.Web.HttpContext.Current.Request.QueryString["Activity"];
            }
        }

        public  string GetCurActivity()
        {
            if (System.Web.HttpContext.Current.Session["Activity"] != null)
            {
                return System.Web.HttpContext.Current.Session["Activity"].ToString();
            }
            else
            {
                return "";
            }
        }

        public GenTrackCodeParam DesSID(string sid)
        {
            if (sid == null)
            {
                return new GenTrackCodeParam();
            }
            else
            {
                string originStr = HJDAPI.Common.Security.DES.Decrypt(sid.Replace(" ", "+"));
               return  JsonConvert.DeserializeObject<GenTrackCodeParam>(originStr);
            }

        }

        public void CheckCID()
        {
            if (System.Web.HttpContext.Current.Request.QueryString.HasKeys() && 
                ( (System.Web.HttpContext.Current.Request.QueryString["cid"] != null && System.Web.HttpContext.Current.Request.QueryString["cid"] != "")
                ||( System.Web.HttpContext.Current.Request.QueryString["sid"] != null &&   System.Web.HttpContext.Current.Request.QueryString["sid"] !="")
                ))
            {
                try
                {
                    long CID = 0;
                    long.TryParse(System.Web.HttpContext.Current.Request.QueryString["CID"], out CID);
                    if (CID == 0 && System.Web.HttpContext.Current.Request.QueryString["sid"] != null)
                    {
                       var sid =    DesSID(System.Web.HttpContext.Current.Request.QueryString["sid"]);
                       CID = sid.UserID;
                    }

                    if (CID == 0)  //可能是 我们的点评分享
                    {
                        string pCID = System.Web.HttpContext.Current.Request.QueryString["CID"].ToString();
                        if (pCID.Length > 1)
                        {
                            var SessionChannelID = pCID.Replace(" ", "+");
                            string originStr = HJDAPI.Common.Security.DES.Decrypt(SessionChannelID);
                            long.TryParse(!string.IsNullOrWhiteSpace(originStr) ? originStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1] : "0", out CID);

                            System.Web.HttpContext.Current.Session["ChannelID"] = CID;
                        }
                    }
                    else
                    {
                        ChannelInfoEntity channel = HotelHelper.GetChannelInfo(CID);

                        if (channel.NeedVerify)
                        {
                            if (System.Web.HttpContext.Current.Request.QueryString["CUserID"] != null)
                            {
                                if (System.Web.HttpContext.Current.Request.QueryString["TimeStamp"] != null
                                    && System.Web.HttpContext.Current.Request.QueryString["Sign"] != null
                                    )
                                {
                                    string CUserID = System.Web.HttpContext.Current.Request.QueryString["CUserID"];
                                    string TimeStamp = System.Web.HttpContext.Current.Request.QueryString["TimeStamp"];
                                    string Sign = System.Web.HttpContext.Current.Request.QueryString["Sign"];
                                    if (System.Web.HttpContext.Current.Session["ChannelUserID"] == null
                                        || System.Web.HttpContext.Current.Session["ChannelUserID"].ToString() != CUserID
                                        || System.Web.HttpContext.Current.Session["ChannelID"].ToString() != CID.ToString())
                                    {

                                        bool isValid = ChannelSignIsValid(channel, CUserID, TimeStamp, Sign);
                                        if (isValid)
                                        {
                                            System.Web.HttpContext.Current.Session["ChannelID"] = System.Web.HttpContext.Current.Request.QueryString["CID"];
                                            if (CUserID.Length > 0) //如果CUserID为空，那么不注册用户。 有些渠道需要验证，但不整合用户信息
                                            {
                                                System.Web.HttpContext.Current.Session["ChannelUserID"] = CUserID;
                                                string NickName = channel.Code + CUserID;
                                                AccountHelper.OAuthLogin(new HJDAPI.Models.AccountInfoItem { Uid = CUserID, AutoLogin = true, LoginType = AccountHelper.GetLoginTypeWithChannelID(CID), NickName = NickName });
                                            }
                                        }
                                        else
                                        {
                                            if (System.Web.HttpContext.Current.Session["ChannelID"] != null)
                                                System.Web.HttpContext.Current.Session.Remove("ChannelID");
                                            if (System.Web.HttpContext.Current.Session["ChannelUserID"] != null)
                                                System.Web.HttpContext.Current.Session.Remove("ChannelUserID");
                                            System.Web.HttpContext.Current.Response.Redirect("/Error404.html");
                                            // base.Redirect("/Error404.html");  //看看如何跳转。
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            System.Web.HttpContext.Current.Session["ChannelID"] = CID;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("CheckCID:" + System.Web.HttpContext.Current.Request.Url + ex.Message  );
                    System.Web.HttpContext.Current.Session["ChannelID"] = 0;
                }
            }
        }

        /// <summary>
        /// IsVIPInvatation:如果为true，则标识在购买VIP后，发送给推荐者短信，说“您推荐的XXX已经购买VIP..”类似信息
        /// </summary>
        /// <param name="IsVIPInvatation"></param>
        public void SetIsVIPInvatation(bool IsVIPInvatation)
        {
            System.Web.HttpContext.Current.Session["IsVIPInvatation"] = IsVIPInvatation;
        }

        public bool GetIsVIPInvatation()
        {
            var _ = false;

            if (System.Web.HttpContext.Current.Session["IsVIPInvatation"] != null)
            {
                bool.TryParse(System.Web.HttpContext.Current.Session["IsVIPInvatation"].ToString(), out _);
            }

            return _;
        }

        /// <summary>
        /// 获取当前用户的渠道ID
        /// </summary>
        /// <returns></returns>
        public int GetCurCID()
        {
            var cid = 0;

            if (System.Web.HttpContext.Current.Session["ChannelID"] != null)
            {
                int.TryParse(System.Web.HttpContext.Current.Session["ChannelID"].ToString(), out cid);
            }

            return cid;
        }

        /// <summary>
        /// 获取当前请求参数中的CID
        /// </summary>
        /// <returns></returns>
        public long GetCurCIDForRequest()
        {
            long cid = 0;

            if (System.Web.HttpContext.Current.Request.QueryString.HasKeys() &&
                (System.Web.HttpContext.Current.Request.QueryString["CID"] != null
                ||
                System.Web.HttpContext.Current.Request.QueryString["sid"] != null))
            {
                try
                {
                    long.TryParse(System.Web.HttpContext.Current.Request.QueryString["CID"], out cid);
                    if (cid == 0 && System.Web.HttpContext.Current.Request.QueryString["sid"] != null)
                    {
                        var sid = DesSID(System.Web.HttpContext.Current.Request.QueryString["sid"]);
                        cid = sid.UserID;
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return cid;
        }

        private bool ChannelSignIsValid( ChannelInfoEntity channel, string CUserID, string TimeStamp, string Sign)
        {
            long CID = channel.IDX;
            string toSign = string.Format("CID={0}&CUserID={1}&TimeStamp={2}", CID, CUserID, TimeStamp);
            string DESKey = channel.DESKey ;
            //switch (CID)
            //{
            //    case 150:
            //        DESKey = HJDAPI.Common.Security.DES.ChinaPayDESKey;
            //        break;
            //}

            if (DESKey.Length > 0)
            {
                try
                {
                    string strSign = Sign.Replace(" ", "+");
                    var desStr = HJDAPI.Common.Security.DES.DecryptString(DESKey, strSign);

                    return toSign == desStr;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 获取当前用户的微信授权Unionid
        /// </summary>
        /// <returns>Unionid</returns>
        public string GetCurWXUnionid()
        {
            var _unionid = "";

            if (System.Web.HttpContext.Current.Session[CurWXUnionidSessionKey] != null)
            {
                _unionid = System.Web.HttpContext.Current.Session[CurWXUnionidSessionKey].ToString();
            }

            return _unionid;
        }

        /// <summary>
        /// 缓存当前用户的微信授权Unionid至Session
        /// </summary>
        /// <param name="_unionid">微信授权Unionid</param>
        public void SetCurWXUnionid(string _unionid)
        {
            System.Web.HttpContext.Current.Session[CurWXUnionidSessionKey] = _unionid;
        }

        /// <summary>
        /// 记录用户行为
        /// </summary>
        /// <param name="code">行为Key</param>
        /// <param name="value">行为信息</param>
        public void RecordBehavior(string code, string value)
        {
            try
            {
                var _clientTypeName = Utils.GetTerminalName(Request.UserAgent, _ContextBasicInfo);

                var _behaviorEntity = new HJD.AccessService.Contract.Model.Behavior
                {
                    Code = code, 
                    Value = value,
                    UserId = Convert.ToInt32(_ContextBasicInfo.AppUserID),
                    Phone = "",
                    AppKey = Session.SessionID, 
                    AppVer = _ContextBasicInfo.AppVer,
                    IP = _ContextBasicInfo.UserHostAddress,
                    RecordLayer = "www",
                    ClientType = _clientTypeName
                };

                new HJDAPI.APIProxy.Access().RecordBehavior(_behaviorEntity);
            }
            catch (Exception ex)
            {
                
            }
        }
        public void RecordBehavior(string code)
        {
            try
            {
                var _clientTypeName = Utils.GetTerminalName(Request.UserAgent, _ContextBasicInfo);

                var _behaviorEntity = new HJD.AccessService.Contract.Model.Behavior
                {
                    Code = code,
                    Value = "",
                    UserId = Convert.ToInt32(_ContextBasicInfo.AppUserID),
                    Phone = "",
                    AppKey = Session.SessionID,
                    AppVer = _ContextBasicInfo.AppVer,
                    IP = _ContextBasicInfo.UserHostAddress,
                    RecordLayer = "www",
                    ClientType = _clientTypeName
                };

                new HJDAPI.APIProxy.Access().RecordBehavior(_behaviorEntity);
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// 检测请求
        /// </summary>
        /// <returns></returns>
        public bool CheckIpRequest()
        {
            var isOk = true;


            return isOk;
        }

        /// <summary>
        /// 静态资源更新版本号
        /// </summary>
        /// <returns></returns>
        public static string curAppVer()
        {
            var cssVersion = DateTime.Now.ToString("yyyyMMdd");
            try
            {
                var last = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var lastTime = System.IO.File.GetLastWriteTime(last);
                cssVersion = lastTime.ToString("yyyyMMddhhmm");
            }
            catch (Exception ex) { }

            return cssVersion;
        }

        #region With APP

        /// <summary>
        /// 判断打开网页的是不是客户端App
        /// </summary>
        /// <returns></returns>
        public bool IsApp()
        {
            if (Session != null && Session["apptype"] != null)
            {
                return true;
            }
            else if (Request != null && Request.Headers != null && Request.Headers["apptype"] != null)
            {
                Session["apptype"] = Request.Headers["apptype"];

                if (Request.Headers["appver"] != null) Session["appver"] = Request.Headers["appver"];
                if (Request.Headers["appVersionCode"] != null) Session["appVersionCode"] = Request.Headers["appVersionCode"];

                return true;
            }
            //else if (System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Headers != null && System.Web.HttpContext.Current.Request.Headers["apptype"] != null)
            //{
            //    Session["apptype"] = System.Web.HttpContext.Current.Request.Headers["apptype"];

            //    if (System.Web.HttpContext.Current.Request.Headers["appver"] != null) Session["appver"] = System.Web.HttpContext.Current.Request.Headers["appver"];
            //    if (System.Web.HttpContext.Current.Request.Headers["appVersionCode"] != null) Session["appVersionCode"] = System.Web.HttpContext.Current.Request.Headers["appVersionCode"];

            //    return true;
            //}
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 当前app存在Headers里的UserId
        /// </summary>
        /// <returns></returns>
        public int AppUserID()
        {
            var _userid = 0;

            //android
            //iOSApp
            if (Request != null && Request.Headers != null && Request.Headers["userid"] != null)
            {
                _userid = Convert.ToInt32(Request.Headers["userid"].ToString());
            }
            else if (System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Headers != null && System.Web.HttpContext.Current.Request.Headers["userid"] != null)
            {
                _userid = Convert.ToInt32(System.Web.HttpContext.Current.Request.Headers["userid"].ToString());
            }

            return _userid;
        }

        /// <summary>
        /// 当前app的平台类型
        /// </summary>
        /// <returns>android or iosapp</returns>
        public string AppType()
        {
            //android
            //iOSApp
            if (Session != null && Session["apptype"] != null)
            {
                return Session["apptype"].ToString().ToLower();
            }
            else if (Request != null && Request.Headers != null && Request.Headers["apptype"] != null)
            {
                return Request.Headers["apptype"].ToString().ToLower();
            }
            else if (System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Headers != null && System.Web.HttpContext.Current.Request.Headers["apptype"] != null)
            {
                return System.Web.HttpContext.Current.Request.Headers["apptype"].ToString().ToLower();
            }

            return "";
        }

        /// <summary>
        /// 当前IOS app的版本
        /// </summary>
        /// <returns></returns>
        public int AppVerForIOS()
        {
            if (Session != null && Session["appver"] != null)
            {
                return Convert.ToInt32(Session["appver"]);
            }
            else if (Request != null && Request.Headers != null && Request.Headers["appver"] != null && Request.Headers["appver"].ToString() != "")
            {
                return Convert.ToInt32(Request.Headers["appver"]);
            }
            else if (System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Headers != null && System.Web.HttpContext.Current.Request.Headers["appver"] != null && System.Web.HttpContext.Current.Request.Headers["appver"].ToString() != "")
            {
                return Convert.ToInt32(System.Web.HttpContext.Current.Request.Headers["appver"]);
            }

            return 0;
        }

        /// <summary>
        /// 当前安卓app的版本
        /// </summary>
        /// <returns></returns>
        public int AppVerForAndroid()
        {
            if (Session != null && Session["appVersionCode"] != null)
            {
                return Convert.ToInt32(Session["appVersionCode"]);
            }
            else if (Request != null && Request.Headers != null && Request.Headers["appVersionCode"] != null && Request.Headers["appVersionCode"].ToString() != "")
            {
                return Convert.ToInt32(Request.Headers["appVersionCode"]);
            }
            else if (System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Headers != null && System.Web.HttpContext.Current.Request.Headers["appVersionCode"] != null && System.Web.HttpContext.Current.Request.Headers["appVersionCode"].ToString() != "")
            {
                return Convert.ToInt32(System.Web.HttpContext.Current.Request.Headers["appVersionCode"]);
            }

            return 0;
        }

        /// <summary>
        /// 当前IOS app的版本（如3.x 4.x）
        /// </summary>
        /// <returns></returns>
        public string AppVerCodeForIOS()
        {
            if (Session != null && Session["shortappver"] != null)
            {
                return Session["shortappver"].ToString();
            }
            else if (Request != null && Request.Headers != null && Request.Headers["shortappver"] != null && Request.Headers["shortappver"].ToString() != "")
            {
                return Request.Headers["shortappver"].ToString();
            }
            else if (System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Headers != null && System.Web.HttpContext.Current.Request.Headers["shortappver"] != null && System.Web.HttpContext.Current.Request.Headers["shortappver"].ToString() != "")
            {
                return System.Web.HttpContext.Current.Request.Headers["shortappver"].ToString();
            }

            return "0";
        }

        /// <summary>
        /// 当前安卓app的版本（如3.x 4.x）
        /// </summary>
        /// <returns></returns>
        public string AppVerCodeForAndroid()
        {
            if (Session != null && Session["appver"] != null)
            {
                return Session["appver"].ToString();
            }
            else if (Request != null && Request.Headers != null && Request.Headers["appver"] != null && Request.Headers["appver"].ToString() != "")
            {
                return Request.Headers["appver"].ToString();
            }
            else if (System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Headers != null && System.Web.HttpContext.Current.Request.Headers["appver"] != null && System.Web.HttpContext.Current.Request.Headers["appver"].ToString() != "")
            {
                return System.Web.HttpContext.Current.Request.Headers["appver"].ToString();
            }

            return "0";
        }

        /// <summary>
        /// 获取当前APP的版本号(如4.x)
        /// </summary>
        /// <returns></returns>
        public string AppVersionCode()
        {
            switch (AppType())
            {
                case "iosapp":
                    {
                        return AppVerCodeForIOS();
                    }
                case "android":
                    {
                        return AppVerCodeForAndroid();
                    }
            }
            return "";
        }

        /// <summary>
        /// 当前APP是否为最新版
        /// </summary>
        /// <returns></returns>
        public bool IsLatestVerApp()
        {
            switch (AppType())
            {
                case "iosapp":
                    {
                        return AppVerForIOS() >= 128;
                    }
                case "android":
                    {
                        return AppVerForAndroid() >= 36;
                    }
            }
            return false;
        }

        //大于等于4.0版本
        public bool IsThanVer4_0()
        {
            switch (AppType())
            {
                case "iosapp":
                    {
                        return AppVerForIOS() >= 170;
                        break;
                    }
                case "android":
                    {
                        return AppVerForAndroid() >= 40;
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// 大于等于4.4版本
        /// </summary>
        /// <returns></returns>
        public bool IsThanVer4_4()
        {
            switch (AppType())
            {
                case "iosapp":
                    {
                        return AppVerForIOS() >= 232;
                        break;
                    }
                case "android":
                    {
                        return AppVerForAndroid() >= 46;
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// 大于等于4.6版本
        /// </summary>
        /// <returns></returns>
        public bool IsThanVer4_6()
        {
            switch (AppType())
            {
                case "iosapp":
                    {
                        return AppVerForIOS() >= 260;
                        break;
                    }
                case "android":
                    {
                        return AppVerForAndroid() >= 50;
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// 大于等于4.7版本
        /// </summary>
        /// <returns></returns>
        public bool IsThanVer4_7()
        {
            switch (AppType())
            {
                case "iosapp":
                    {
                        return AppVerForIOS() >= 263;
                        break;
                    }
                case "android":
                    {
                        return AppVerForAndroid() >= 51;
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// 大于等于5.0版本
        /// </summary>
        /// <returns></returns>
        public bool IsThanVer5_0()
        {
            switch (AppType())
            {
                case "iosapp":
                    {
                        return AppVerForIOS() >= 300;
                        break;
                    }
                case "android":
                    {
                        return AppVerForAndroid() >= 59;
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// 大于等于5.1版本
        /// </summary>
        /// <returns></returns>
        public bool IsThanVer5_1()
        {
            switch (AppType())
            {
                case "iosapp":
                    {
                        return AppVerForIOS() >= 302;
                        break;
                    }
                case "android":
                    {
                        return AppVerForAndroid() >= 61;
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// 大于等于5.2版本
        /// </summary>
        /// <returns></returns>
        public bool IsThanVer5_2()
        {
            switch (AppType())
            {
                case "iosapp":
                    {
                        return AppVerForIOS() >= 306;
                        break;
                    }
                case "android":
                    {
                        return AppVerForAndroid() >= 62;
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// 大于等于5.4版本
        /// </summary>
        /// <returns></returns>
        public bool IsThanVer5_4()
        {
            switch (AppType())
            {
                case "iosapp":
                    {
                        return AppVerForIOS() >= 318;
                        break;
                    }
                case "android":
                    {
                        return AppVerForAndroid() >= 66;    //暂时android没有5.4版本 2017.05.15 haoy
                        break;
                    }
            }
            return false;
        }

        #region app 版本控制

        /// <summary>
        /// 初始app版本的比较变量
        /// </summary>
        public void InitAppCompareVer()
        {
            if (_ContextBasicInfo.IsIOS)
            {
                if (_ContextBasicInfo.AppBundleVer >= 325) _ContextBasicInfo.IsThanVer5_6_2 = true;
                if (_ContextBasicInfo.AppBundleVer >= 323) _ContextBasicInfo.IsThanVer5_6_1 = true;
                if (_ContextBasicInfo.AppBundleVer >= 322) _ContextBasicInfo.IsThanVer5_6 = true;
                if (_ContextBasicInfo.AppBundleVer >= 320) _ContextBasicInfo.IsThanVer5_4 = true;
                if (_ContextBasicInfo.AppBundleVer >= 314) _ContextBasicInfo.IsThanVer5_3 = true;
                if (_ContextBasicInfo.AppBundleVer >= 306) _ContextBasicInfo.IsThanVer5_2 = true;
                if (_ContextBasicInfo.AppBundleVer >= 302) _ContextBasicInfo.IsThanVer5_1 = true;
                if (_ContextBasicInfo.AppBundleVer >= 301) _ContextBasicInfo.IsThanVer5_0 = true;
                if (_ContextBasicInfo.AppBundleVer >= 300) _ContextBasicInfo.IsThanVer4_8 = true;
                if (_ContextBasicInfo.AppBundleVer >= 284) _ContextBasicInfo.IsThanVer4_7 = true;
                if (_ContextBasicInfo.AppBundleVer >= 263) _ContextBasicInfo.IsThanVer4_6_1 = true;
                if (_ContextBasicInfo.AppBundleVer >= 260) _ContextBasicInfo.IsThanVer4_6 = true;
                if (_ContextBasicInfo.AppBundleVer >= 232) _ContextBasicInfo.IsThanVer4_4 = true;
            }
            else if (_ContextBasicInfo.IsAndroid)
            {
                if (_ContextBasicInfo.AppBundleVer >= 68) _ContextBasicInfo.IsThanVer5_6_2 = true;
                if (_ContextBasicInfo.AppBundleVer >= 67) _ContextBasicInfo.IsThanVer5_6 = true;
                if (_ContextBasicInfo.AppBundleVer >= 62) _ContextBasicInfo.IsThanVer5_2 = true;
                if (_ContextBasicInfo.AppBundleVer >= 61) _ContextBasicInfo.IsThanVer5_1 = true;
                if (_ContextBasicInfo.AppBundleVer >= 60) _ContextBasicInfo.IsThanVer5_0 = true;
                if (_ContextBasicInfo.AppBundleVer >= 59) _ContextBasicInfo.IsThanVer4_8 = true;
                if (_ContextBasicInfo.AppBundleVer >= 58) _ContextBasicInfo.IsThanVer4_7 = true;
                if (_ContextBasicInfo.AppBundleVer >= 51) _ContextBasicInfo.IsThanVer4_6_1 = true;
                if (_ContextBasicInfo.AppBundleVer >= 50) _ContextBasicInfo.IsThanVer4_6 = true;
                if (_ContextBasicInfo.AppBundleVer >= 46) _ContextBasicInfo.IsThanVer4_4 = true;
            }
        }

        #endregion

        #endregion
    }
}