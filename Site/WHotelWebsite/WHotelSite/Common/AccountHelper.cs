using HJD.AccountServices.Entity;
using HJDAPI.APIProxy;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Common
{
    public class AccountHelper
    {
        public enum UserRoleEnum
        {
            Normal = 0,
            VIP = 12,
            VIP199 = 16,
            VIP599 = 17
        }

        public static bool UserHasVIPFirstBuyPriviledge(string userid)
        {
            return UserHasVIPFirstBuyPriviledge(long.Parse(userid));
        }
        /// <summary>
        /// 返回用户是否有VIP首单购买权限
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static bool UserHasVIPFirstBuyPriviledge(long userid)
        {
            var _userInfoPrivileges = account.GetAllPrivilegeByUserId(new AccountInfoItem { Uid = userid.ToString() });
            //2010 首单爆款
            return _userInfoPrivileges.Exists(_p => _p.PrivID == 2010);
        }

        public static OperationResult OAuthLogin(AccountInfoItem item)
        {
              OperationResult result =      account.OAuthLogin( item);
              //4512272|ADA431358C714D54A38827DB4CCFE849|TTG3104
              if (result.Success)
              {
                  string []  strDataList = result.Data.Split('|');
                  if (strDataList.Length >2)
                  {
                      string userid = strDataList[0];
                      string nickName = strDataList[1].Length == 32 ? strDataList[2] : strDataList[1]; //Data 形式不一致，这里只能这样处理了。 BUG

                      bool isSaveCookie = false;
                      string isv = isSaveCookie.ToString();
                      string loginToken = UserState.GetUserVerifyToken(nickName + isv + userid);

                      SetUserLoginState(userid, nickName, isSaveCookie, HttpContext.Current);
                  }
              }

              return result;                
        }

        public  static void SetUserLoginState(string userid, string nickName, bool isSaveCookie, HttpContext httpContext)
        { 
            if (isSaveCookie)  //是否在Cookie中存在UID
            {
                string userToken = UserState.GetUserVerifyToken(userid);
                if (httpContext.Request.Cookies["UID"] == null)
                {
                    HttpCookie cookie = new HttpCookie("UID");
                    cookie.Value = HttpUtility.UrlEncode(userid);
                    cookie.Expires = DateTime.Now.AddDays(3);//一年过期
                    cookie.Domain = ".zmjiudian.com";
                    cookie.HttpOnly = true;
                    httpContext.Response.Cookies.Add(cookie);
                }
                else
                {
                    httpContext.Response.Cookies["UID"].Value = HttpUtility.UrlEncode(userid);
                }

                if (httpContext.Request.Cookies["UIDToken"] == null)
                {
                    HttpCookie cookie = new HttpCookie("UIDToken");
                    cookie.Value = HttpUtility.UrlEncode(userToken);
                    cookie.Expires = DateTime.Now.AddDays(3);//一年过期
                    cookie.Domain = ".zmjiudian.com";
                    cookie.HttpOnly = true;
                    httpContext.Response.Cookies.Add(cookie);
                }
                else
                {
                    httpContext.Response.Cookies["UIDToken"].Value = HttpUtility.UrlEncode(userToken);
                }
            }
            else
            {
                httpContext.Session.Add("UID", userid);
            }
 

            if (httpContext.Request.Cookies["NickName"] == null)
            {
                HttpCookie cookie = new HttpCookie("NickName");
                cookie.Value = HttpUtility.UrlEncode(nickName);
                cookie.Expires = DateTime.Now.AddDays(3);//一年过期
                cookie.Domain = ".zmjiudian.com";
                httpContext.Response.Cookies.Add(cookie);
            }
            else
            {
                httpContext.Response.Cookies["NickName"].Value = HttpUtility.UrlEncode(nickName);
            }
        }
                          

        private static Dictionary<long, int> dicThirdParty = new Dictionary<long, int>();
        public  static int GetLoginTypeWithChannelID(long channelID)
        {
            if (!dicThirdParty.ContainsKey(channelID))
            {
                List<UserBindTypeEntity> list = account.GetUserBindTypeList();
                list.ForEach(b =>
                {
                    if (b.RelChannelIDs.Length > 0)
                    {
                        if (b.RelChannelIDs.Replace(" ","").Split(',').Contains(channelID.ToString()))
                        {
                            if (!dicThirdParty.ContainsKey(channelID))
                                dicThirdParty.Add(channelID, b.ID);
                        }
                    }
                });
                if (!dicThirdParty.ContainsKey(channelID))
                {
                    dicThirdParty.Add(channelID, -1);
                }
            }

            return dicThirdParty[channelID];
        }


        public static void SetAuthorizeHead(int id, int supplierId, string operatorName,string showName)
        {
            if (HttpContext.Current == null) return;
            HttpContext.Current.Session["Shop-Id"] = id.ToString();
            HttpContext.Current.Session["Shop-SupplierId"] = supplierId;
            HttpContext.Current.Session["Shop-OperatorName"] = operatorName;
            HttpContext.Current.Session["Shop-ShowName"] = showName;
        }

        public static string ShowName
        {
            get
            {
                try
                {
                    string ShowName = "";
                    if (HttpContext.Current.Session != null && HttpContext.Current.Session["Shop-ShowName"] != null)
                    {
                        ShowName = HttpContext.Current.Session["Shop-ShowName"].ToString();
                    }
                    return ShowName;
                }
                catch
                {
                    return "";
                }
            }
        }
        public static int OperatorID
        {
            get
            {
                try
                {
                    int Id = 0;
                    if (HttpContext.Current.Session != null && HttpContext.Current.Session["Shop-Id"] != null)
                    {
                        int.TryParse(HttpContext.Current.Session["Shop-Id"].ToString(), out Id);
                    }
                    return Id;
                }
                catch
                {
                    return -1;
                }
            }
        }

        public static int SupplierId
        {
            get
            {
                try
                {
                    int supplierId = 0;
                    if (HttpContext.Current.Session != null && HttpContext.Current.Session["Shop-SupplierId"] != null)
                    {
                        int.TryParse(HttpContext.Current.Session["Shop-SupplierId"].ToString(), out supplierId);
                    }
                    return supplierId;
                }
                catch
                {
                    return -1;
                }
            }
        }

        public static bool IsShopLogin()
        {

            if (HttpContext.Current == null || HttpContext.Current.Session["Shop-Id"] == null) return false;

            long operatorID = OperatorID;
            return OperatorID > 0;

            //判断用户是否已登录
            var encrypte = HttpContext.Current.Request.Headers["OperatorName"];

            bool hasLogin = true;
            if (operatorID == 0 || encrypte == null)
            {
                hasLogin = false;
            }
            return hasLogin;
        }

        public static bool LoginOut()
        {
            HttpContext.Current.Session["Shop-Id"] = null;
            return true;
        }
    }
}