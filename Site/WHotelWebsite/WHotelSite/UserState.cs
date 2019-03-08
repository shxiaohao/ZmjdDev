using HJD.AccountServices.Entity;
using HJD.Framework.Encrypt;
using HJDAPI.APIProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WHotelSite.Common;

namespace WHotelSite
{
    public static class UserState
    {
        public static bool IsLogin
        {
            get {
                string ss = GetUserVerifyToken(UserID.ToString());
                bool isVerifyRight = false;
                if(ss.Equals(UserToken)){
                    isVerifyRight = true;
                }
                else if (HttpContext.Current.Request.Cookies["UID"] == null && HttpContext.Current.Request.Cookies["UIDToken"] == null)
                {
                    isVerifyRight = true;
                }
                else
                {
                    LogHelper.WriteLog(HttpContext.Current.Request.UserHostAddress + ";" + HttpContext.Current.Request.UserHostName + ";UserToken:" + UserToken + "," + ss + ";UserID:" + UserID.ToString());
                }
                if (UserID != 0 && isVerifyRight)
                {
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        public static long UserID
        {
            get
            {
                long _userId = 0;
                if (HttpContext.Current.Session["UID"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["UID"].ToString()))
                {
                    long.TryParse(HttpContext.Current.Session["UID"].ToString(), out _userId);
                }
                else if (HttpContext.Current.Request.Cookies["UID"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["UID"].Value))
                {
                    long.TryParse(HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["UID"].Value), out _userId);                    
                }
                return _userId;
            }
        }

        public static string UserToken {
            get
            {
                string _userToken = "";
                if (HttpContext.Current.Request.Cookies["UIDToken"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["UIDToken"].Value))
                {
                    _userToken = HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["UIDToken"].Value);
                }
                return _userToken;
            }
        }

        public static string GetUserVerifyToken(string userId)
        {
            string phoneNum = "18817309081";
            string s1 = string.Format("{0}{1}{2}", phoneNum, userId, "!&$123");
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s1);
            byte[] result2 = MD5.ComputeHash(data);
            StringBuilder buf = new StringBuilder();
            foreach (byte b in result2)
            {
                buf.Append(b.ToString("X2"));
            }
            return buf.ToString().ToLower();
        }

        public static string RecordSessionId()
        {
            if ((HttpContext.Current.Request.Cookies["RecordSessionId"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["RecordSessionId"].Value)))
            {
                return HttpContext.Current.Request.Cookies["RecordSessionId"].Value;
            }

            var newGid = Guid.NewGuid().ToString();
            HttpCookie newCookie = null;
            newCookie = new HttpCookie("RecordSessionId");
            newCookie.Expires = DateTime.Now.AddDays(-360);
            newCookie.Domain = ".zmjiudian.com";
            newCookie.Value = newGid;
            HttpContext.Current.Response.Cookies.Add(newCookie);

            return newGid;
        }
    }
}