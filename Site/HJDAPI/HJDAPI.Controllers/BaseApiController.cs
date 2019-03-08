using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HJD.AccountServices.Contracts;
using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;

namespace HJDAPI.Controllers
{
    public class BaseApiController : ApiController
    {
        public string Uid;
        public long UserId;

        /// <summary>
        /// 当前上下文的相关信息
        /// </summary>
        public ContextBasicInfo _ContextBasicInfo = new ContextBasicInfo();

        /** 目前下面变量还有使用的地方，其实_ContextBasicInfo对象的字段完全包含了这些定义，后续请使用_ContextBasicInfo替代这些变量的直接使用 2017-08-11 haoy **/
        public long AppUserID;      //当前使用设备访问的用户
        public bool IsApp = false;
        public bool IsWeb = false;
        public bool IsAndroid = false;
        public bool IsIOS = false;
        public string AppType;      //区别iOS或Android设备
        public string AppVer;       //App版本号 如3.x 4.x
        public Int64 AppBundleVer;  //App版本打包号
        public bool IsThanVer4_4 = false;
        public bool IsThanVer4_6 = false;
        public bool IsThanVer4_6_1 = false;
        public bool IsThanVer4_7 = false;
        public bool IsThanVer4_8 = false;
        public bool IsThanVer5_0 = false;
        public bool IsThanVer5_1 = false;
        public bool IsThanVer5_2 = false;
        public bool IsThanVer5_3 = false;
        public bool IsThanVer5_4 = false;
        public bool IsThanVer5_6 = false;
        public bool IsThanVer5_6_1 = false;
        public bool IsThanVer5_6_2 = false;
        public bool IsThanVer5_7 = false;
        public bool IsThanVer5_9 = false;
        public bool IsThanVer6_0 = false;
        public bool IsThanVer6_2 = false;
        public bool IsThanVer6_2_1 = false;
        public bool IsThanVer6_4_2 = false;
        /***********************************************************************************************************************************************/

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Uid = HttpContext.Current.Request.Headers["WH-Uid"];
            AppType = HttpContext.Current.Request.Headers["apptype"];
            AppType = string.IsNullOrWhiteSpace(AppType) ? "web" : AppType;
            long.TryParse(HttpContext.Current.Request.Headers["userid"], out AppUserID);
            long.TryParse(HttpContext.Current.Request.Headers["WH-UserId"], out UserId);

            //是否web环境
            IsWeb = AppType.ToLower().Contains("web");

            //是否ios环境
            IsIOS = AppType.ToLower().Contains("ios");

            //是否android环境
            IsAndroid = AppType.ToLower().Contains("android");

            //是否app环境
            if (IsIOS || IsAndroid)
            {
                IsApp = true;
            }

            //app版本号 如5.6.0
            AppVer = IsAndroid ? HttpContext.Current.Request.Headers["appver"] : IsIOS ? HttpContext.Current.Request.Headers["shortappver"] : "0";

            //app版本打包号 如322
            AppBundleVer = 0;
            var appBundleVerStr = IsAndroid ? HttpContext.Current.Request.Headers["appVersionCode"] : IsIOS ? HttpContext.Current.Request.Headers["appver"] : "0";
            try
            {
                if (!string.IsNullOrEmpty(appBundleVerStr))
                {
                    AppBundleVer = Convert.ToInt64(appBundleVerStr);   
                }
            }
            catch (Exception ex)
            {
                
            }

            //初始配置app的版本比较变量
            InitAppCompareVer();

            #region 初始_ContextBasicInfo对象

            _ContextBasicInfo.IsApp = IsApp;
            _ContextBasicInfo.AppUserID = AppUserID;
            _ContextBasicInfo.IsIOS = IsIOS;
            _ContextBasicInfo.IsWeb = IsWeb;
            _ContextBasicInfo.IsAndroid = IsAndroid;
            _ContextBasicInfo.AppType = AppType;
            _ContextBasicInfo.AppVer = AppVer;
            _ContextBasicInfo.AppBundleVer = AppBundleVer;
            _ContextBasicInfo.IsThanVer4_4 = IsThanVer4_4;
            _ContextBasicInfo.IsThanVer4_6 = IsThanVer4_6;
            _ContextBasicInfo.IsThanVer4_6_1 = IsThanVer4_6_1;
            _ContextBasicInfo.IsThanVer4_7 = IsThanVer4_7;
            _ContextBasicInfo.IsThanVer4_8 = IsThanVer4_8;
            _ContextBasicInfo.IsThanVer5_0 = IsThanVer5_0;
            _ContextBasicInfo.IsThanVer5_1 = IsThanVer5_1;
            _ContextBasicInfo.IsThanVer5_2 = IsThanVer5_2;
            _ContextBasicInfo.IsThanVer5_3 = IsThanVer5_3;
            _ContextBasicInfo.IsThanVer5_4 = IsThanVer5_4;
            _ContextBasicInfo.IsThanVer5_6 = IsThanVer5_6;
            _ContextBasicInfo.IsThanVer5_6_1 = IsThanVer5_6_1;
            _ContextBasicInfo.IsThanVer5_6_2 = IsThanVer5_6_2;
            _ContextBasicInfo.IsThanVer5_7 = IsThanVer5_7;
            _ContextBasicInfo.IsThanVer5_9 = IsThanVer5_9;
            _ContextBasicInfo.IsThanVer6_0 = IsThanVer6_0;
            _ContextBasicInfo.IsThanVer6_2 = IsThanVer6_2;
            _ContextBasicInfo.IsThanVer6_2_1 = IsThanVer6_2_1;
            _ContextBasicInfo.IsThanVer6_4_2 = IsThanVer6_4_2;


            try
            {
                _ContextBasicInfo.UserHostAddress = HttpContext.Current.Request.UserHostAddress;
            }
            catch (Exception ex) { }

            #endregion

            HttpContext.Current.Response.Headers["Access-Control-Allow-Origin"] = "*";
            HttpContext.Current.Response.Headers["Access-Control-Expose-Headers"] = "*";
            HttpContext.Current.Response.Headers["Access-Control-Request-Method"] = "GET,POST";
            HttpContext.Current.Response.Headers["Access-Control-Allow-Headers"] = "WH-Uid,WH-UserId,WH-Encrypte,WH-EncDataExt";
            HttpContext.Current.Response.ContentType = "application/json";
        }

        public void SetAuthorizeHead(long userid, string password, string email)
        {

            if (HttpContext.Current == null) return;
            IAccountService AccService = ServiceProxyFactory.Create<IAccountService>("BasicHttpBinding_IAccountService");
           
            HttpContext.Current.Response.Headers.Add("WH-UserId", userid.ToString());
            HttpContext.Current.Response.Headers.Add("WH-Encrypte", SecurityHelper.EncryptDES(userid.ToString(), Configs.WHMo));
          //  HttpContext.Current.Response.Headers.Add("WH-EncDataExt", SecurityHelper.EncryptDES(email, Configs.WHMo));
        }

        protected void SetAuthorizeHead(long userid)
        {
            HttpContext.Current.Response.Headers.Add("WH-UserId", userid.ToString());
            HttpContext.Current.Response.Headers.Add("WH-Encrypte", SecurityHelper.EncryptDES(userid.ToString(), Configs.WHMo));
        }

        protected bool Authorize()
        {
            var encrypte = HttpContext.Current.Request.Headers["WH-Encrypte"];

            if (UserId == 0 || encrypte == null)
            {
                HttpContext.Current.Response.Headers.Add("WH-Msg", "NotAuthorization");
                return false;
            }

            var encrypteString = SecurityHelper.DecryptDES(encrypte, Configs.WHMo);
            if (UserId.ToString() != encrypteString)
            {
                HttpContext.Current.Response.Headers.Add("WH-Msg", "NotAuthorization");
                return false;
            }

            return true;
        }

        protected static int GetStarValue(decimal score)
        {
            var d = (double)score;
            if (d > 4.5)
            {
                return 10;
            }
            if (d <= 4.5 && d > 4)
            {
                return 9;
            }
            if (d <= 4 && d > 3.5)
            {
                return 8;
            }
            if (d <= 3.5 && d > 3)
            {
                return 7;
            }
            if (d <= 3 && d > 2.5)
            {
                return 6;
            }
            if (d <= 2.5 && d > 2)
            {
                return 5;
            }
            if (d <= 2 && d > 1.5)
            {
                return 4;
            }
            if (d <= 1.5 && d > 1)
            {
                return 3;
            }
            if (d <= 1 && d > 0.5)
            {
                return 2;
            }
            if (d <= 0.5 && d > 0)
            {
                return 1;
            }

            return 0;
        }

        protected static string GetSuggestType(string type)
        {
            switch (type)
            {
                case "D":
                    return "City";
                case "S":
                    return "Sight";
                case "H":
                    return "Hotel";
                default:
                    return "City";
            }
        }

        protected static int[] ParseIntArray(string str)
        {
            if (str == null) return null;
            List<int> result = new List<int>();
            foreach (string i in str.Split(','))
            {
                if (i.Length > 0)
                {
                    int t = -1;
                    if (int.TryParse(i, out t))
                    {
                        result.Add(t);
                    }

                }
            }

            return result.ToArray();
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
                var _clientTypeName = "WWW";
                if (_ContextBasicInfo.IsApp)
                {
                    _clientTypeName = _ContextBasicInfo.IsAndroid ? "Android" : "iOS";
                }
                else
                {
                    if (HttpContext.Current.Request != null && !string.IsNullOrEmpty(HttpContext.Current.Request.UserAgent))
                    {
                        if (HttpContext.Current.Request.UserAgent.IndexOf("MicroMessenger") > 0)
                        {
                            _clientTypeName = "Weixin";
                        }
                    }
                }
                
                var _behaviorEntity = new HJD.AccessService.Contract.Model.Behavior
                {
                    Code = code,
                    Value = value,
                    UserId = Convert.ToInt32(_ContextBasicInfo.AppUserID),
                    Phone = "",
                    AppKey = (HttpContext.Current != null && HttpContext.Current.Session != null && !string.IsNullOrEmpty(HttpContext.Current.Session.SessionID)) ? HttpContext.Current.Session.SessionID : "",
                    AppVer = _ContextBasicInfo.AppVer,
                    IP = _ContextBasicInfo.UserHostAddress,
                    RecordLayer = "api",
                    ClientType = _clientTypeName
                };

                new AccessController().RecordBehavior(_behaviorEntity);
            }
            catch (Exception ex)
            {
                HJDAPI.Controllers.Common.Log.WriteLog("RecordBehavior Err:" + ex.Message);
                HJDAPI.Controllers.Common.Log.WriteLog("RecordBehavior:" + code + ":" + value + ":" + System.DateTime.Now + "  IP " + _ContextBasicInfo.UserHostAddress);
            }
        }

        #region app 版本控制

        /// <summary>
        /// 初始app版本的比较变量
        /// </summary>
        public void InitAppCompareVer()
        {
            if (IsIOS)
            {
                if (AppBundleVer >= 400) IsThanVer6_4_2 = true;
                if (AppBundleVer >= 384) IsThanVer6_2_1 = true;
                if (AppBundleVer >= 383) IsThanVer6_2 = true;
                if (AppBundleVer >= 342) IsThanVer6_0 = true;
                if (AppBundleVer >= 340) IsThanVer5_9 = true;
                if (AppBundleVer >= 327) IsThanVer5_7= true;
                if (AppBundleVer >= 325) IsThanVer5_6_2 = true;
                if (AppBundleVer >= 323) IsThanVer5_6_1 = true;
                if (AppBundleVer >= 322) IsThanVer5_6 = true;
                if (AppBundleVer >= 320) IsThanVer5_4 = true;
                if (AppBundleVer >= 314) IsThanVer5_3 = true;
                if (AppBundleVer >= 306) IsThanVer5_2 = true;
                if (AppBundleVer >= 302) IsThanVer5_1 = true;
                if (AppBundleVer >= 301) IsThanVer5_0 = true;
                if (AppBundleVer >= 300) IsThanVer4_8 = true;
                if (AppBundleVer >= 284) IsThanVer4_7 = true;
                if (AppBundleVer >= 263) IsThanVer4_6_1 = true;
                if (AppBundleVer >= 260) IsThanVer4_6 = true;
                if (AppBundleVer >= 232) IsThanVer4_4 = true;
            }
            else if (IsAndroid)
            {
                if (AppBundleVer >= 92) IsThanVer6_4_2 = true;
                if (AppBundleVer >= 80) IsThanVer6_2_1 = true;
                if (AppBundleVer >= 78) IsThanVer6_2 = true;
                if (AppBundleVer >= 73) IsThanVer6_0 = true;
                if (AppBundleVer >= 72) IsThanVer5_9 = true;
                if (AppBundleVer >= 69) IsThanVer5_7 = true;
                if (AppBundleVer >= 68) IsThanVer5_6_2 = true;
                if (AppBundleVer >= 67) IsThanVer5_6 = true;
                if (AppBundleVer >= 62) IsThanVer5_2 = true;
                if (AppBundleVer >= 61) IsThanVer5_1 = true;
                if (AppBundleVer >= 60) IsThanVer5_0 = true;
                if (AppBundleVer >= 59) IsThanVer4_8 = true;
                if (AppBundleVer >= 58) IsThanVer4_7 = true;
                if (AppBundleVer >= 51) IsThanVer4_6_1 = true;
                if (AppBundleVer >= 50) IsThanVer4_6 = true;
                if (AppBundleVer >= 46) IsThanVer4_4 = true;
            }
        }

        #endregion
    }
} 